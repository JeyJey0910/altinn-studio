using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Altinn.Platform.Storage.Models;
using Altinn.Process;
using Altinn.Process.Elements;
using AltinnCore.Common.Configuration;
using AltinnCore.Common.Helpers;
using AltinnCore.Common.Services.Interfaces;
using AltinnCore.ServiceLibrary.Models;
using AltinnCore.ServiceLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Storage.Interface.Models;

namespace AltinnCore.Runtime.RestControllers
{
    /// <summary>
    /// Controller for setting and moving process flow of an instance.
    /// </summary>
    [Route("{org}/{app}/instances/{instanceOwnerId:int}/{instanceGuid:guid}/process")]
    [ApiController]
    [Authorize]
    public class ProcessController : ControllerBase
    {
        private const int MAX_ITERATIONS_ALLOWED = 1000;
        private readonly ILogger<ProcessController> logger;
        private readonly IInstance instanceService;
        private readonly IProcess processService;
        private readonly IInstanceEvent eventService;

        private readonly UserHelper userHelper;

        private BpmnReader ProcessModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessController"/>
        /// </summary>
        public ProcessController(
            ILogger<ProcessController> logger,
            IInstance instanceService,
            IProcess processService,
            IInstanceEvent eventService,
            IProfile profileService,
            IRegister registerService,
            IOptions<GeneralSettings> generalSettings)
        {
            this.logger = logger;
            this.instanceService = instanceService;
            this.processService = processService;
            this.eventService = eventService;

            userHelper = new UserHelper(profileService, registerService, generalSettings);
        } 

        /// <summary>
        /// Get the process state of an instance.
        /// </summary>
        /// <returns>the instance's process state</returns>
        [HttpGet]
        public async Task<ActionResult<ProcessState>> GetProcessState(
            [FromRoute] string org,
            [FromRoute] string app,
            [FromRoute] int instanceOwnerId,
            [FromRoute] Guid instanceGuid)
        {            
            Instance instance = await instanceService.GetInstance(app, org, instanceOwnerId, instanceGuid);

            if (instance == null)
            {
                return NotFound();
            }

            ProcessState processState = instance.Process;

            return Ok(processState);
        }

        /// <summary>
        /// Starts the process of an instance.
        /// </summary>
        /// <returns>The process state</returns>
        [HttpPost("start")]
        public async Task<ActionResult<ProcessState>> StartProcess(
            [FromRoute] string org,
            [FromRoute] string app,
            [FromRoute] int instanceOwnerId,
            [FromRoute] Guid instanceGuid,
            [FromQuery] string startEvent)
        {
            Instance instance = await instanceService.GetInstance(app, org, instanceOwnerId, instanceGuid);
            if (instance == null)
            {
                return NotFound();
            }

            if (instance.Process != null)
            {
                return Conflict($"Process is already started. Use next");
            }

            LoadProcessModel(org, app);

            string validStartElement = GetValidStartEventOrError(startEvent, out ActionResult startEventError);
            if (startEventError != null)
            {
                return startEventError;
            }

            // trigger start event
            Instance updatedInstance = await StartProcessOfInstance(org, app, instanceOwnerId, instanceGuid, instance, validStartElement);

            // trigger next task
            string nextValidElement = GetValidNextElementOrError(validStartElement, out ActionResult nextElementError);
            if (nextElementError != null)
            {
                return nextElementError;
            }

            updatedInstance = await UpdateProcessStateToNextElement(org, app, updatedInstance, instanceOwnerId, instanceGuid, nextValidElement);

            if (updatedInstance != null)
            {
                return Ok(updatedInstance.Process);
            }

            return StatusCode(500, $"Unknown error. Cannot change process state!");
        }                

        /// <summary>
        /// Gets a list of the next process elements that can be reached from the current process element.
        /// </summary>
        /// <returns>list of next process elements (tasks or events)</returns>
        [HttpGet("next")]
        public async Task<ActionResult> GetNextElements(
            [FromRoute] string org,
            [FromRoute] string app,
            [FromRoute] int instanceOwnerId,
            [FromRoute] Guid instanceGuid)
        {
            Instance instance = await instanceService.GetInstance(app, org, instanceOwnerId, instanceGuid);
            if (instance == null)
            {
                return NotFound();
            }

            LoadProcessModel(org, app);

            if (instance.Process == null)
            {
                return Conflict($"Process is not started. Use start!");
            }

            string currentTaskId = instance.Process.CurrentTask?.ElementId;

            if (currentTaskId == null)
            {
                return Conflict($"Instance does not have valid info about currentTask");
            }

            List<string> nextElementIds = ProcessModel.NextElements(currentTaskId);

            if (nextElementIds.Count == 0)
            {
                return NotFound("Cannot find any valid process elements that can be reached from current task");
            }

            return Ok(nextElementIds);           
        }

        /// <summary>
        /// Change the instance's process state to next process element in accordance with process flow.
        /// </summary>
        /// <returns>new process state</returns>
        [HttpPut("next")]
        public async Task<ActionResult<ProcessState>> NextElement(
            [FromRoute] string org,
            [FromRoute] string app,
            [FromRoute] int instanceOwnerId,
            [FromRoute] Guid instanceGuid,
            [FromQuery] string elementId)
        {
            Instance instance = await instanceService.GetInstance(app, org, instanceOwnerId, instanceGuid);
            if (instance == null)
            {
                return NotFound("Cannot find instance!");
            }

            if (instance.Process == null)
            {
                return Conflict($"Process is not started. Use start!");
            }

            LoadProcessModel(org, app);

            string currentElementId = instance.Process.CurrentTask?.ElementId;

            if (currentElementId == null)
            {
                return Conflict($"Instance does not have current task information!");
            }            

            string nextElement = GetValidNextElementOrError(currentElementId, elementId, out ActionResult nextElementError);
            if (nextElementError != null)
            {
                return nextElementError;
            }

            if (InstanceIsValid(instance))
            {
                Instance changedInstance = await UpdateProcessStateToNextElement(org, app, instance, instanceOwnerId, instanceGuid, nextElement);

                return Ok(changedInstance.Process);
            }
            else
            {
                return Conflict("Cannot complete/close current task {currentTaskId}. Task is not valid!");
            }
        }

        /// <summary>
        /// Attemts to close the process by running start and next until an end event is reached.
        /// </summary>
        /// <returns></returns>
        [HttpPut("completeProcess")]
        public async Task<ActionResult<ProcessState>> CompleteProcess(
            [FromRoute] string org,
            [FromRoute] string app,
            [FromRoute] int instanceOwnerId,
            [FromRoute] Guid instanceGuid)
        {
            Instance instance = await instanceService.GetInstance(app, org, instanceOwnerId, instanceGuid);

            if (instance == null)
            {
                return NotFound("Cannot find instance");
            }

            LoadProcessModel(org, app);

            if (instance.Process == null)
            {
                string startEvent = GetValidStartEventOrError(null, out ActionResult startEventError);

                if (startEventError != null)
                {
                    return startEventError;
                }

                instance = await StartProcessOfInstance(org, app, instanceOwnerId, instanceGuid, instance, startEvent);
            }

            string currentTaskId = instance.Process.CurrentTask?.ElementId ?? instance.Process.StartEvent;

            if (currentTaskId == null)
            {
                return Conflict($"Instance does not have valid currentTask");
            }

            // do next until end event is reached or task cannot be completed.
            int counter = 0;
            do
            {
                if (!InstanceIsValid(instance))
                {
                    return Conflict($"Instance is not valid in task {currentTaskId}. Automatic completion of process is stopped");
                }

                List<string> nextElements = ProcessModel.NextElements(currentTaskId);

                if (nextElements.Count > 1)
                {
                    return Conflict($"Cannot complete process. Multiple outgoing sequence flows detected from task {currentTaskId}. Please select manually among {nextElements}");
                }

                string nextElement = nextElements.First();

                instance = await UpdateProcessStateToNextElement(org, app, instance, instanceOwnerId, instanceGuid, nextElement);

                currentTaskId = instance.Process.CurrentTask?.ElementId;
            }
            while (instance.Process.EndEvent == null || counter > MAX_ITERATIONS_ALLOWED);

            if (counter > 1000)
            {
                return StatusCode(500, $"More than {counter} iterations detected in process. Possible loop. Fix app process definition!");
            }

            return Ok(instance.Process);
        }

        private void LoadProcessModel(string org, string app)
        {
            using (Stream definitions = processService.GetProcessDefinition(org, app))
            {
                ProcessModel = BpmnReader.Create(definitions);
            }
        }

        private string GetValidNextElementOrError(string currentElement, out ActionResult nextElementError)
        {
            nextElementError = null;
            string nextElementId = null;

            List<string> nextElements = ProcessModel.NextElements(currentElement);

            if (nextElements.Count > 1)
            {
                nextElementError = Conflict($"There is more than one element reachable from element {currentElement}");
            }
            else
            {
                nextElementId = nextElements.First();
            }

            return nextElementId;
        }

        private string GetValidStartEventOrError(string proposedStartEvent, out ActionResult startEventError)
        {
            startEventError = null;

            List<string> possibleStartEvents = ProcessModel.StartEvents();

            if (!string.IsNullOrEmpty(proposedStartEvent))
            {
                if (possibleStartEvents.Contains(proposedStartEvent))
                {
                    return proposedStartEvent;
                }
                else
                {
                    startEventError = Conflict($"There is no such start event as '{proposedStartEvent}' in the process definition.");
                    return null;
                }
            }

            if (possibleStartEvents.Count == 1)
            {
                return possibleStartEvents.First();
            }
            else if (possibleStartEvents.Count > 1)
            {
                startEventError = Conflict($"There are more than one start events available. Chose one: {possibleStartEvents}");
                return null;
            }
            else
            {
                startEventError = Conflict($"There is no start events in process definition. Cannot start process!");
                return null;
            }
        }

        private async Task<Instance> StartProcessOfInstance(string org, string app, int instanceOwnerId, Guid instanceGuid, Instance instance, string validStartElement)
        {
            DateTime now = DateTime.UtcNow;

            instance.Process = new ProcessState
            {
                Started = now,
                StartEvent = validStartElement,
            };
            Instance updatedInstance = await instanceService.UpdateInstance(instance, app, org, instanceOwnerId, instanceGuid);
            List<InstanceEvent> events = new List<InstanceEvent>
            {
                GenerateProcessChangeEvent("process:StartEvent", updatedInstance, now)
            };

            await DispatchEvents(org, app, events);

            return updatedInstance;
        }

        private async Task<Instance> UpdateProcessStateToNextElement(string org, string app, Instance instance, int instanceOwnerId, Guid instanceGuid, string nextElementId)
        {
            List<InstanceEvent> events = ChangeProcessStateAndGenerateEvents(instance, nextElementId);

            Instance changedInstance = await instanceService.UpdateInstance(instance, app, org, instanceOwnerId, instanceGuid);

            await DispatchEvents(org, app, events);

            return changedInstance;
        }

        private async Task DispatchEvents(string org, string app, List<InstanceEvent> events)
        {
            foreach (InstanceEvent instanceEvent in events)
            {
                await eventService.SaveInstanceEvent(instanceEvent, org, app);
            }
        }

        private string GetValidNextElementOrError(string currentElementId, string proposedElementId, out ActionResult nextElementError)
        {
            nextElementError = null;

            List<string> possibleNextElements = ProcessModel.NextElements(currentElementId);

            if (!string.IsNullOrEmpty(proposedElementId))
            {
                if (possibleNextElements.Contains(proposedElementId))
                {
                    return proposedElementId;
                }
                else
                {
                    nextElementError = Conflict($"Process element id '{proposedElementId}' is not found in app's process model (bpmn)");
                    return null;
                }
            }

            if (possibleNextElements.Count == 1)
            {
                return possibleNextElements.First();
            }
            
            if (possibleNextElements.Count > 1)
            {
                nextElementError = Conflict($"There are more than one outgoing sequence flows, please select one '{possibleNextElements}'");
                return null;
            }

            if (possibleNextElements.Count == 0)
            {
                nextElementError = Conflict($"There are no outoging sequence flows from current element. Cannot find next process element. Error in bpmn file!");
                return null;
            }

            return null;
        }

        /// <summary>
        ///  Todo: Fix validation code of the instance's currentTask.
        /// </summary>        
        /// <returns>try if validation is OK, false otherwise</returns>
        private bool InstanceIsValid(Instance instance)
        {
            return true;            
        }

        private List<InstanceEvent> ChangeProcessStateAndGenerateEvents(Instance instance, string nextElementId)
        {
            List<InstanceEvent> events = new List<InstanceEvent>();

            ProcessState currentState = instance.Process;

            string previousElementId = currentState.CurrentTask?.ElementId;

            ElementInfo nextElementInfo = ProcessModel.GetElementInfo(nextElementId);

            DateTime now = DateTime.UtcNow;
            int flow = 1;

            if (IsStartEvent(previousElementId))
            {
                flow = 1;
            }

            if (IsTask(previousElementId))
            {
                if (currentState.CurrentTask != null && currentState.CurrentTask.Flow.HasValue)
                {
                    flow = currentState.CurrentTask.Flow.Value;
                }

                events.Add(GenerateProcessChangeEvent("process:EndTask", instance, now));
            }

            if (IsEndEvent(nextElementId))
            {
                currentState.CurrentTask = null;
                currentState.Ended = now;
                currentState.EndEvent = nextElementId;                

                events.Add(GenerateProcessChangeEvent("process:EndEvent", instance, now));
            }
            else if (IsTask(nextElementId))
            {
                currentState.CurrentTask = new ProcessElementInfo
                {
                    Flow = flow + 1, 
                    ElementId = nextElementId,
                    Name = nextElementInfo.Name,
                    Started = now,
                    AltinnTaskType = nextElementInfo.AltinnTaskType,
                    Validated = null,
                };

                events.Add(GenerateProcessChangeEvent("process:StartTask", instance, now));
            }

            // current state points to the instance's process object. The following statement is unnecessary, but clarifies logic.
            instance.Process = currentState;

            return events;
        }

        private InstanceEvent GenerateProcessChangeEvent(string eventType, Instance instance, DateTime now)
        {
            UserContext userContext = userHelper.GetUserContext(HttpContext).Result;

            InstanceEvent instanceEvent = new InstanceEvent
            {
                InstanceId = instance.Id,
                InstanceOwnerId = instance.InstanceOwnerId,
                EventType = eventType,
                CreatedDateTime = now,
                UserId = userContext.UserId,
                AuthenticationLevel = userContext.AuthenticationLevel,
                ProcessInfo = instance.Process,
            };

            return instanceEvent;
        }

        private bool IsTask(string nextElementId)
        {
            List<string> tasks = ProcessModel.Tasks();
            return tasks.Contains(nextElementId);            
        }

        private bool IsStartEvent(string startEventId)
        {
            List<string> startEvents = ProcessModel.StartEvents();

            return startEvents.Contains(startEventId);
        }

        private bool IsEndEvent(string nextElementId)
        {
            List<string> endEvents = ProcessModel.EndEvents();

            return endEvents.Contains(nextElementId);            
        }
    }   
}
