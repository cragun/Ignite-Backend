using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    /// <summary>
    /// The Device is a mobile device / client that accesses the Application.  Devices can be granted access or denied
    /// </summary>
    public class Device : EntityBase
    {
        #region Properties

        /// <summary>
        /// The UUID of the Device per the DeviceAPI
        /// </summary>
        [DataMember]
        [StringLength(100)]
        public string Uuid { get; set; }

        /// <summary>
        /// Name of the OS (Android, IOS, etc)
        /// </summary>
        [DataMember]
        [StringLength(50)]
        public string OsName { get; set; }

        /// <summary>
        /// Official Version of the OS
        /// </summary>
        [DataMember]
        [StringLength(50)]
        public string OsVersion { get; set; }

        /// <summary>
        /// Model of the Dervice (iPhone 5.1)
        /// </summary>
        [DataMember]
        [StringLength(50)]
        public string Model { get; set; }

        [DataMember]
        [StringLength(250)]
        public string APNDeviceToken { get; set; }

        #endregion

        #region Navigation

        [DataMember]
        [InverseProperty("Device")]
        public ICollection<UserDevice> UserDevices { get; set; }

        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            UserDevices = FilterEntityCollection(UserDevices, newInclusionPath);

        }

    }
}
