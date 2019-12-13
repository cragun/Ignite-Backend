using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    /// <summary>
    /// This object is the Many to Many object between People and Devices.  When a user deactives a device, he/she is Disabling the relaitonship between the Person and the Device.  Multiple users (people) can access the application
    /// from the same device.. hence the many to many relationship
    /// </summary>
    [DataContract]
    public class UserDevice : EntityBase
    {
        #region Properties

        /// <summary>
        /// Guid of the person who uses the device
        /// </summary>
        [DataMember]
        public Guid UserID { get; set; }

        /// <summary>
        /// Guid of the Device
        /// </summary>
        [DataMember]
        public Guid DeviceID { get; set; }

        /// <summary>
        /// Disallows access to the API from this device/user combo.  Used if Device is Lost/Stolen, for example
        /// </summary>
        [DataMember]
        public bool IsDisabled { get; set; }

        #endregion

        #region Navigation

        [DataMember]
        [ForeignKey("UserID")]
        public User User { get; set; }

        [DataMember]
        [ForeignKey("DeviceID")]
        public Device Device { get; set; }

        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            User    = FilterEntity(User,    newInclusionPath);
            Device  = FilterEntity(Device,  newInclusionPath);

        }

    }
}
