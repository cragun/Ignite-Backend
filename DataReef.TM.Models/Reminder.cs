using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;


namespace DataReef.TM.Models
{
    /// <summary>
    /// Reminders for a person are just that.. reminders to follow up on something
    /// </summary>
    public class Reminder:EntityBase
    {
        /// <summary>
        /// Person that the reminder belongs to
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }

        /// <summary>
        /// Optional Guid of a property related to the reminder
        /// </summary>
        [DataMember]
        public Guid? PropertyID { get; set; }

        /// <summary>
        /// UTC DAteTime of the reminder
        /// </summary>
        [DataMember]
        public System.DateTime ReminderDate { get;set;}

        /// <summary>
        /// description of the reminder (content)
        /// </summary>
        [StringLength(1000)]
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// should the client device put this into the calendar
        /// </summary>
        [DataMember]
        public bool IncludeInCalendar { get; set; }

        /// <summary>
        /// should an email be sent for the reminder
        /// </summary>
        [DataMember]
        public bool EmailReminder { get; set; }

        /// <summary>
        /// Mintues (int) ahead the the ReminderDate that the reminder should be triggered
        /// </summary>
        [DataMember]
        public int ReminderMinutes { get; set; }

        /// <summary>
        /// Flag that tells the system whether the reminder has been triggered
        /// </summary>
        [DataMember]
        public bool Reminded { get; set; }


        #region Navigation

        [ForeignKey("PersonID")]
        [DataMember]
        public Person Person { get; set; }

        [ForeignKey("PropertyID")]
        [DataMember]
        public Property Property { get; set; }

         #endregion


        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Person      = FilterEntity(Person,      newInclusionPath);
            Property    = FilterEntity(Property,    newInclusionPath);

        }
    }
}
