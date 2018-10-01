/* 
 * Gitea API.
 *
 * This documentation describes the Gitea API.
 *
 * OpenAPI spec version: 1.1.1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using SwaggerDateConverter = AltinnCore.RepositoryClient.Client.SwaggerDateConverter;

namespace AltinnCore.RepositoryClient.Model
{
    /// <summary>
    /// TrackedTime worked time for an issue / pr
    /// </summary>
    [DataContract]
    public partial class TrackedTime :  IEquatable<TrackedTime>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackedTime" /> class.
        /// </summary>
        /// <param name="Created">Created.</param>
        /// <param name="Id">Id.</param>
        /// <param name="IssueId">IssueId.</param>
        /// <param name="Time">Time in seconds.</param>
        /// <param name="UserId">UserId.</param>
        public TrackedTime(DateTime? Created = default(DateTime?), long? Id = default(long?), long? IssueId = default(long?), long? Time = default(long?), long? UserId = default(long?))
        {
            this.Created = Created;
            this.Id = Id;
            this.IssueId = IssueId;
            this.Time = Time;
            this.UserId = UserId;
        }
        
        /// <summary>
        /// Gets or Sets Created
        /// </summary>
        [DataMember(Name="created", EmitDefaultValue=false)]
        public DateTime? Created { get; set; }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name="id", EmitDefaultValue=false)]
        public long? Id { get; set; }

        /// <summary>
        /// Gets or Sets IssueId
        /// </summary>
        [DataMember(Name="issue_id", EmitDefaultValue=false)]
        public long? IssueId { get; set; }

        /// <summary>
        /// Time in seconds
        /// </summary>
        /// <value>Time in seconds</value>
        [DataMember(Name="time", EmitDefaultValue=false)]
        public long? Time { get; set; }

        /// <summary>
        /// Gets or Sets UserId
        /// </summary>
        [DataMember(Name="user_id", EmitDefaultValue=false)]
        public long? UserId { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class TrackedTime {\n");
            sb.Append("  Created: ").Append(Created).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  IssueId: ").Append(IssueId).Append("\n");
            sb.Append("  Time: ").Append(Time).Append("\n");
            sb.Append("  UserId: ").Append(UserId).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as TrackedTime);
        }

        /// <summary>
        /// Returns true if TrackedTime instances are equal
        /// </summary>
        /// <param name="input">Instance of TrackedTime to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(TrackedTime input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Created == input.Created ||
                    (this.Created != null &&
                    this.Created.Equals(input.Created))
                ) && 
                (
                    this.Id == input.Id ||
                    (this.Id != null &&
                    this.Id.Equals(input.Id))
                ) && 
                (
                    this.IssueId == input.IssueId ||
                    (this.IssueId != null &&
                    this.IssueId.Equals(input.IssueId))
                ) && 
                (
                    this.Time == input.Time ||
                    (this.Time != null &&
                    this.Time.Equals(input.Time))
                ) && 
                (
                    this.UserId == input.UserId ||
                    (this.UserId != null &&
                    this.UserId.Equals(input.UserId))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.Created != null)
                    hashCode = hashCode * 59 + this.Created.GetHashCode();
                if (this.Id != null)
                    hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.IssueId != null)
                    hashCode = hashCode * 59 + this.IssueId.GetHashCode();
                if (this.Time != null)
                    hashCode = hashCode * 59 + this.Time.GetHashCode();
                if (this.UserId != null)
                    hashCode = hashCode * 59 + this.UserId.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
