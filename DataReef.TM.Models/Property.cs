using DataReef.Core.Attributes;
using DataReef.TM.Models.Credit;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Geo;
using DataReef.TM.Models.PropertyAttachments;
using DataReef.TM.Models.Solar;
using DataReef.TM.Models.Surveys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    [Table("Properties")]
    public class Property : PropertyBase
    {
        public Property()
        {

        }

        /// <summary>
        /// The id for NetSuite. Customer.
        /// </summary>
        [DataMember]
        public string NetSuiteSECustomerID { get; set; }

        /// <summary>
        /// The provider account id for Genability Account.
        /// </summary>
        [DataMember]
        public string GenabilityProviderAccountID { get; set; }

        /// <summary>
        /// The account id for Genability Account.
        /// </summary>
        [DataMember]
        public string GenabilityAccountID { get; set; }

        /// <summary>
        /// Used by front end to archive a property so its no longer visible in street sheet
        /// </summary>
        [DataMember]
        public Boolean IsArchive { get; set; }

        [DataMember]
        public Guid TerritoryID { get; set; }

        [DataMember]
        [StringLength(50)]
        public string LatestDisposition { get; set; }

        /// <summary>
        /// Pitch used as default on roof planes
        /// </summary>
        [DataMember]
        public double? Pitch { get; set; }

        [DataMember]
        public bool ApplyConsumptionSlope { get; set; }

        [DataMember]
        [Index()]
        public override string Name { get => base.Name; set => base.Name = value; }

        /// <summary>
        /// Elastic Search unique identifier.
        /// </summary>
        [DataMember]
        [StringLength(50)]
        public string AddressID { get; set; }

        /// <summary>
        /// Elastic index name used for the AddressID.
        /// </summary>
        [DataMember]
        [StringLength(15)]
        public string ESIndexName { get; set; }

        [DataMember]
        [StringLength(200)]
        public string UtilityProviderID { get; set; }

        [DataMember]
        [StringLength(200)]
        public string UtilityProviderName { get; set; }

        [DataMember]
        public int? LeadSourceId { get; set; }

        [DataMember]
        public bool UsageCollected { get; set; }      
        

        [DataMember]
        public Nullable<int> DispositionTypeId { get; set; }

        [DataMember]
        [StringLength(200)]
        public string SunnovaLeadID { get; set; }


        #region Navigation

        [ForeignKey("TerritoryID")]
        [DataMember]
        public Territory Territory { get; set; }

        [DataMember, AttachOnUpdate]
        public ICollection<Inquiry> Inquiries { get; set; }

        [DataMember]
        public ICollection<PrescreenInstant> Prescreens { get; set; }

        [DataMember]
        [InverseProperty(nameof(PropertyPowerConsumption.Property))]
        public ICollection<PropertyPowerConsumption> PowerConsumptions { get; set; }

        [NotMapped]
        [DataMember]
        public bool IsGeoProperty { get; set; }

        [NotMapped]
        [DataMember]
        public DateTime? LastProposalDate { get; set; }

        /// <summary>
        /// The reminders associated with the Property
        /// </summary>
        [DataMember]
        [InverseProperty("Property")]
        public ICollection<Reminder> Reminders { get; set; }

        [ForeignKey("Guid")]
        [DataMember]
        public Survey72 Survey { get; set; }

        [DataMember]
        [InverseProperty(nameof(Proposal.Property))]
        public ICollection<Proposal> Proposals { get; set; }

        [DataMember, AttachOnUpdate]
        [InverseProperty(nameof(Appointment.Property))]
        public ICollection<Appointment> Appointments { get; set; }

        [DataMember]
        [InverseProperty(nameof(PropertyAttachment.Property))]
        public virtual ICollection<PropertyAttachment> Attachments { get; set; }

        [DataMember]
        [InverseProperty(nameof(PropertyNote.Property))]
        public virtual ICollection<PropertyNote> PropertyNotes { get; set; }

        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Inquiries = FilterEntityCollection(Inquiries, newInclusionPath);
            Prescreens = FilterEntityCollection(Prescreens, newInclusionPath);
            Reminders = FilterEntityCollection(Reminders, newInclusionPath);
            PowerConsumptions = FilterEntityCollection(PowerConsumptions, newInclusionPath);

            Territory = FilterEntity(Territory, newInclusionPath);
            Survey = FilterEntity(Survey, newInclusionPath);
        }

        public override void PrepareNavigationProperties(Guid? createdById = null)
        {
            base.PrepareNavigationProperties();

            if (Inquiries != null)
            {
                foreach (var item in Inquiries)
                {
                    item.PropertyID = Guid;
                    if (!item.CreatedByID.HasValue && createdById.HasValue)
                    {
                        item.CreatedByID = createdById;
                    }
                }
            }

            if (Reminders != null)
            {
                foreach (var item in Reminders)
                {
                    item.PropertyID = Guid;
                    if (!item.CreatedByID.HasValue && createdById.HasValue)
                    {
                        item.CreatedByID = createdById;
                    }
                }
            }

            if (Survey != null)
            {
                Survey.Guid = Guid;
                if (!Survey.CreatedByID.HasValue && createdById.HasValue)
                {
                    Survey.CreatedByID = createdById;
                }
            }

            if (Appointments != null)
            {
                foreach (var app in Appointments)
                {
                    app.PropertyID = Guid;
                    if (!app.CreatedByID.HasValue && createdById.HasValue)
                    {
                        app.CreatedByID = createdById;
                    }
                }
            }

            if (PropertyBag != null)
            {
                foreach (var item in PropertyBag)
                {
                    item.PropertyId = Guid;
                    if (!item.CreatedByID.HasValue && createdById.HasValue)
                    {
                        item.CreatedByID = createdById;
                    }
                }
            }

            if (Occupants != null)
            {
                foreach (var item in Occupants)
                {
                    item.PrepareNavigationProperties();
                    if (!item.CreatedByID.HasValue && createdById.HasValue)
                    {
                        item.CreatedByID = createdById;
                    }
                }
            }
        }

        private DbGeometry _location;
        public DbGeometry Location()
        {
            if (_location != null)
            {
                return _location;
            }

            if (!Latitude.HasValue || !Longitude.HasValue)
            {
                return null;
            }

            _location = DbGeometry.PointFromText($"POINT({Longitude} {Latitude})", DbGeometry.DefaultCoordinateSystemId);
            return _location;
        }

        public Appointment GetLatestAppointment()
        {
            return Appointments?
                        .OrderByDescending(a => a.DateCreated)?
                        .FirstOrDefault();
        }

        [NotMapped]
        public int? PropertyNotesCount { get; set; }
        
    }
}