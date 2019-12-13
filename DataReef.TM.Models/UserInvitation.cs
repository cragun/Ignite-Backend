using DataReef.Core.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DataReef.Core.Enums;

namespace DataReef.TM.Models
{
    [DataContract]
    public enum InvitationStatus
    {
        [EnumMember] Pending,

        [EnumMember] Accepted,

        [EnumMember] Expired
    }

    [MailAttribute(CrudAction=CrudAction.Insert, MailMethod="UserInvitationCreated")]
    public class UserInvitation : EntityBase
    {
        public UserInvitation()
        {
            InvitationCode = Guid.NewGuid().ToString();
            SetExpirationDate();
        }

        #region Properties

        /// <summary>
        ///     The guid of the person that is doing the inviting.
        /// </summary>
        [DataMember]
        public Guid FromPersonID { get; set; }

        /// <summary>
        /// Email address of the person being invited
        /// </summary>
        [DataMember]
        [EmailAddress]
        [StringLength(100)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// First Name of the person being invited
        /// </summary>
        [DataMember]
        [StringLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        ///Last Name of the person being invited 
        /// </summary>
        [DataMember]
        [StringLength(50)]
        public string LastName { get; set; }

        /// <summary>
        /// a Guid of the invitation, different from main guid for obfuscation
        /// </summary>
        [DataMember]
        [StringLength(100)]
        public string InvitationCode { get; set; }

        /// <summary>
        /// Status of the Invite
        /// </summary>
        [DataMember]
        public InvitationStatus Status { get; set; }

        /// <summary>
        ///     Date the user accepted the invitation
        /// </summary>
        [DataMember]
        public DateTime? DateAccepted { get; set; }

        /// <summary>
        ///     Date the the invitation will expire.  30 days by default.  UTC DateTime
        /// </summary>
        [DataMember]
        public DateTime ExpirationDate { get; set; }


        /// <summary>
        ///     The PersonID of the person, once they get signed up and turned into a person.  Should not be added by client
        /// </summary>
        [DataMember]
        public Guid? ToPersonID { get; set; }

        /// <summary>
        ///     Guid of OU to which person is invited
        /// </summary>
        [DataMember]
        public Guid OUID { get; set; }

        /// <summary>
        /// OURole that the person can act in the OU
        /// </summary>
        [DataMember]
        public Guid RoleID { get; set; }

        #endregion

        #region Navigation

        [DataMember]
        [ForeignKey("FromPersonID")]
        public Person FromPerson { get; set; }

        [DataMember]
        [ForeignKey("ToPersonID")]
        public Person ToPerson { get; set; }

        [DataMember]
        [ForeignKey("OUID")]
        public OU OU { get; set; }

        [DataMember]
        [ForeignKey("RoleID")]
        public OURole Role { get; set; }

        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            FromPerson  = FilterEntity(FromPerson,  newInclusionPath);
            ToPerson    = FilterEntity(ToPerson,    newInclusionPath);
            OU          = FilterEntity(OU,          newInclusionPath);
            Role        = FilterEntity(Role,        newInclusionPath);
        }

        /// <summary>
        /// Set the expiration date to UtcNow + 30 days.
        /// </summary>
        public void SetExpirationDate()
        {
            ExpirationDate = DateTime.UtcNow.AddDays(30);
        }

    }
}