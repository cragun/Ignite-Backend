using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    [DataContract]
    [Flags]
    public enum LoginAttemptResult
    {
        [EnumMember] Unknown,

        [EnumMember] Successful,

        [EnumMember] Failure
    }

    /// <summary>
    ///     Object used to authenticate.  Fill in UserName and Password and any other data.
    /// </summary>
    public class LoginAttempt : EntityBase
    {
        

        [DataMember(EmitDefaultValue = false)]
        public LoginAttemptResult Result { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string UserName { get; set; }

        /// <summary>
        ///     Unencrypted Password, Required
        /// </summary>
        [DataMember]
        [NotMapped]
        public string Password { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string IPAddress { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string DeviceID { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string OSVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string OSName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string DeviceName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string ApplicationName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string FailureReason { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool PasswordChangeRequired { get; set; }


        [DataMember(EmitDefaultValue = false)]
        public Guid? PersonID { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("PersonID")]
        public User User { get; set; }

        #endregion


        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            User = FilterEntity(User, newInclusionPath);

        }

    }
}