using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using AltinnCore.ServiceLibrary.ServiceMetadata;
using AltinnCore.ServiceLibrary;
using AltinnCore.ServiceLibrary.Enums;
using AltinnCore.ServiceLibrary.ServiceMetadata;
using AltinnCore.ServiceLibrary.Services.Interfaces;
using AltinnCore.ServiceLibrary.Models;

namespace AltinnCoreServiceImplementation.Template
{
    /// <summary>
    /// This is the Template ServiceImplementation that handles the service events
    /// </summary>
    public class ServiceImplementation: IServiceImplementation
    {
        private IPlatformServices platformServices;

        private SERVICE_MODEL_NAME SERVICE_MODEL_NAME;

        private RequestContext _requestContext;

        private StartServiceModel _startServiceModel;

        private ModelStateDictionary _modelState;

        private ServiceContext _serviceContext;

        private CalculationHandler _calculationHandler;

        private ValidationHandler _validationHandler;

        private InstantiationHandler _instantiationHandler;



        public ServiceImplementation()
        {
            _calculationHandler = new CalculationHandler();
            _instantiationHandler = new InstantiationHandler();
            _validationHandler = new ValidationHandler();
        }

        /// <summary>
        /// Creates a new instance of the serviceModel used by the service
        /// </summary>
        /// <returns></returns>
        public object CreateNewServiceModel()
        {
            return new SERVICE_MODEL_NAME();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Type GetServiceModelType()
        {
            return typeof(SERVICE_MODEL_NAME);
        }

        public async Task<bool> RunServiceEvent(ServiceEventType serviceEvent)
        {
            if (serviceEvent.Equals(ServiceEventType.Calculation))
            {
                _calculationHandler.Calculate(this.SERVICE_MODEL_NAME);
            }
            else if (serviceEvent.Equals(ServiceEventType.Validation))
            {
                _validationHandler.Validate(this.SERVICE_MODEL_NAME, this._requestContext, this._modelState);
            }
            else if (serviceEvent.Equals(ServiceEventType.Instantiation))
            {
                _instantiationHandler.Instansiate(this.SERVICE_MODEL_NAME);
            }

            return true;
        }

        public async Task<bool> HandleGetDataEvent()
        {
			return true;
        }

        public void HandleInstansiationEvent(ServiceContext serviceContext)
        {
            
        }

        public void HandleValidateInstansiationEvent(StartServiceModel startServiceModel, ModelStateDictionary modelState)
        {

        }

        public void HandleValidationEvent(ModelStateDictionary modelState)
        {

        }

        public void SetContext(RequestContext requestContext)
        {
            this._requestContext = requestContext;
        }

        public void SetContext(RequestContext requestContext, ServiceContext serviceContext, StartServiceModel startServiceModel, ModelStateDictionary modelState)
        {
            this._requestContext = requestContext;
            this._modelState = modelState;
            this._startServiceModel = startServiceModel;
            this._serviceContext = serviceContext;
        }

        public void SetPlatformServices(IPlatformServices platformServices)
        {
            this.platformServices = platformServices;
        }

        public void SetServiceModel(object model)
        {
            this.SERVICE_MODEL_NAME = (SERVICE_MODEL_NAME)model;
        }
    }
}
