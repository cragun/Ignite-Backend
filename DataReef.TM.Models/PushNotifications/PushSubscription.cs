using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.PushNotifications
{
    public enum NotificationType
    {
        Regular = 0,
        Silent
    }

    /// <summary>
    /// Table used for devices that will subscribe to CRUD changes
    /// Name will be the Entity Name (e.g. Territory, OU)
    /// ExternalID will be used to to filter down the Subscriptions
    /// </summary>
    [Table("Subscriptions", Schema = "push")]
    public class PushSubscription : EntityBase
    {
        public NotificationType NotificationType { get; set; }

        public Guid DeviceId { get; set; }

        #region Navigation Properties

        [ForeignKey(nameof(DeviceId))]
        public Device Device { get; set; }

        #endregion

    }
}
