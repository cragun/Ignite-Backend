using DataReef.Core.Attributes;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    [DataContract(IsReference = true)]
    [Versioned]
    public class Person : EntityBase
    {

        private PersonSummary summary = new PersonSummary();

        public Person()
        {
            // Add the MayEdit property to exclude
            AddDefaultExcludedProperties("MayEdit");
        }

        #region Properties

        [DataMember]
        [NotMapped]
        public string SocialSecurityRaw { get; set; }

        [DataMember]
        public string SocialSecurityEntrypted { get; set; }



        [DataMember(EmitDefaultValue = false)]
        [StringLength(15)]
        public string Prefix
        {
            get;
            set;
        }

        [DataMember]
        public PersonSummary Summary
        {
            get { return this.summary; }
            set { this.summary = value; }
        }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(100)]
        [Display(Name = "First Name", ShortName = "FN", Prompt = "Please enter the First Name")]
        public string FirstName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string MiddleName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(100)]
        public string LastName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(100)]
        public string PreferredName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(10)]
        public string Suffix { get; set; }

        [DataMember(EmitDefaultValue = false)]
        // please do not increase this value as this is the maximum value for the unique index (450 unicode characters x 2bytes each = 900 bytes)
        [StringLength(450)]
        [Index(IsUnique = true, IsClustered = false)]
        public string EmailAddressString { get; set; }


        [DataMember(EmitDefaultValue = false)]
        public string OnlineAppointmentPara { get; set; }
        
        [DataMember]
        public DateTime? LastLoginDate { get; set; }

        [DataMember]
        public DateTime? LastActivityDate { get; set; }

        [DataMember]
        public string fcm_token { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(250)]
        public string ActivityName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string BuildVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<string> EmailAddresses
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.EmailAddressString))
                {
                    string[] ret = this.EmailAddressString.Split(',', ';');
                    return ret.ToList();
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value != null)
                {
                    string val = string.Join(",", value.ToArray());
                    this.EmailAddressString = val;
                }
                else
                {
                    this.EmailAddressString = null;
                }
            }
        }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string SalesRepID { get; set; }

        [NotMapped]
        [JsonIgnore]
        public List<string> MayEditSections { get; set; }

        [DataMember]
        [NotMapped]
        public string MayEdit
        {
            get
            {
                if (MayEditSections == null || MayEditSections.Count == 0) return null;
                return String.Join(",", MayEditSections);
            }
            set
            {
                MayEditSections = new List<string>();
                if (!String.IsNullOrEmpty(value)) MayEditSections.AddRange(value.Split(','));
            }
        }

        [DataMember]
        public PersonType PersonType { get; set; }

        [DataMember]
        [StringLength(25)]
        public string SmartBoardID { get; set; }

        [NotMapped]
        public bool IsFavourite { get; set; }

        #endregion

        #region Navigation

        [DataMember(EmitDefaultValue = false)]
        public ICollection<Attachment> Attachments { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ICollection<Address> Addresses { get; set; }

        [InverseProperty("FromPerson")]
        [DataMember(EmitDefaultValue = false)]
        public ICollection<PersonalConnection> ConnectionsFrom { get; set; }

        [InverseProperty("ToPerson")]
        [DataMember(EmitDefaultValue = false)]
        public ICollection<PersonalConnection> ConnectionsTo { get; set; }

        [InverseProperty("ToPerson")]
        [DataMember]
        public ICollection<UserInvitation> ConnectionInvitationsReceived { get; set; }

        [InverseProperty("FromPerson")]
        [DataMember]
        public ICollection<UserInvitation> ConnectionInvitationsSent { get; set; }

        [InverseProperty("Person")]
        [DataMember]
        public ICollection<Reminder> Reminders { get; set; }


        [InverseProperty("Person")]
        [DataMember(EmitDefaultValue = false)]
        public ICollection<Note> Notes { get; set; }

        [DataMember]
        public ICollection<Identification> Identification { get; set; }


        [DataMember(EmitDefaultValue = false)]
        public ICollection<PhoneNumber> PhoneNumbers { get; set; }

        [DataMember]
        public User User { get; set; }

        /// <summary>
        /// The settings of a person.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public ICollection<PersonSetting> PersonSettings { get; set; }        

        /// <summary>
        /// AccountAssociations links a person to an Account.  A person must have an AccountAssoication for ever account in which they have an OUAssoications
        /// </summary>
        [InverseProperty("Person")]
        [DataMember]
        public ICollection<AccountAssociation> AccountAssociations { get; set; }


        /// <summary>
        /// OUAssoications link a person to the OU they have access to.. this object houses the role that the person plans in the OU
        /// </summary>
        [InverseProperty("Person")]
        [DataMember]
        public ICollection<OUAssociation> OUAssociations { get; set; }


        /// <summary>
        /// Assignments link a person to the territories he/she has access to
        /// </summary>
        [InverseProperty("Person")]
        [DataMember]
        public ICollection<Assignment> Assignments { get; set; }

        /// <summary>
        /// Favorite territory assigned to this person
        /// </summary>
        [InverseProperty("Person")]
        [DataMember]
        public ICollection<FavouriteTerritory> FavouriteTerritories { get; set; }

        /// <summary>
        /// Appointments assigned to this person
        /// </summary>
        [InverseProperty("Assignee")]
        [DataMember]
        public ICollection<Appointment> AssignedAppointments { get; set; }

        /// <summary>
        /// Appointments created by this person
        /// </summary>
        [InverseProperty("Creator")]
        [DataMember]
        [JsonIgnore]
        public ICollection<Appointment> CreatedAppointments { get; set; }

        #endregion

        public string FullName => $"{FirstName} {LastName}";

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Attachments = FilterEntityCollection(Attachments, newInclusionPath);
            Addresses = FilterEntityCollection(Addresses, newInclusionPath);
            ConnectionsFrom = FilterEntityCollection(ConnectionsFrom, newInclusionPath);
            ConnectionsTo = FilterEntityCollection(ConnectionsTo, newInclusionPath);
            ConnectionInvitationsReceived = FilterEntityCollection(ConnectionInvitationsReceived, newInclusionPath);
            ConnectionInvitationsSent = FilterEntityCollection(ConnectionInvitationsSent, newInclusionPath);
            Reminders = FilterEntityCollection(Reminders, newInclusionPath);
            Notes = FilterEntityCollection(Notes, newInclusionPath);
            PhoneNumbers = FilterEntityCollection(PhoneNumbers, newInclusionPath);
            PersonSettings = FilterEntityCollection(PersonSettings, newInclusionPath);
            AccountAssociations = FilterEntityCollection(AccountAssociations, newInclusionPath);
            OUAssociations = FilterEntityCollection(OUAssociations, newInclusionPath);
            Assignments = FilterEntityCollection(Assignments, newInclusionPath);
            FavouriteTerritories = FilterEntityCollection(FavouriteTerritories, newInclusionPath);

            User = FilterEntity(User, newInclusionPath);
        }
    }
}