using DataReef.Core.Attributes;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Finance;
using DataReef.TM.Models.Layers;
using DataReef.TM.Models.Reporting;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    /// <summary>
    /// OU or Organization Unit is the the core class that represents a Company, Regions, Division, Centers, etc within the Organization . 
    /// </summary>
    [DataContract(IsReference = true)]
    public class OU : EntityBase
    {

        private OUSummary summary = new OUSummary();

        public OU()
        {
            Addresses = new List<Address>();
            PhoneNumbers = new List<PhoneNumber>();
            IsDeletableByClient = true;
            AddDefaultExcludedProperties("Children");
        }

        #region Properties

        /// <summary>
        /// An optional identifier that the Organization may use to identify a Center, or a Region or a Store, etc..
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string Number { get; set; }

        /// <summary>
        /// Optional Guid of the Parent OU.  If this OU is the root of an organization, this field should be null
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public Guid? ParentID { get; set; }

        [DataMember]
        public float CentroidLat { get; set; }

        [DataMember]
        public float CentroidLon { get; set; }

        [DataMember]
        public float Radius { get; set; }

        [StringLength(100)]
        [DataMember]
        public string BatchPrescreenTableName { get; set; }

        /// <summary>
        /// Is the OU Active in the UI.  Back end logic does not care about IsActive.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public bool IsDisabled { get; set; }

        [DataMember]
        [StringLength(50)]
        public string PartnerID { get; set; }

        /// <summary>
        /// The Guid of the Account that owns the OU.  Used for Flat Iteration of all OUs of an account 
        /// </summary>
        [DataMember]
        public Guid AccountID { get; set; }

        [DataMember]
        public OUSummary Summary
        {
            get
            {
                return this.summary;
            }
            set
            {
                this.summary = value;
            }
        }

        [DataMember]
        public bool IsDeletableByClient { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Guid? RootOrganizationID { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string RootOrganizationName { get; set; }

        [DataMember]
        public OUType? OrganizationType { get; set; }

        [DataMember]
        [StringLength(50)]
        public string CompanyID { get; set; }

        [DataMember]
        public int ShapesVersion { get; set; }

        [DataMember]
        public float? TokenPriceInDollars { get; set; }

        [DataMember]
        public ActivityType ActivityTypes { get; set; }

        [DataMember]
        public bool IsArchived { get; set; }

        [DataMember]
        public bool IsTerritoryAdd { get; set; }

        [DataMember]
        public int MinModule { get; set; }

        /// <summary>
        /// Property used by the Portal, to populate the breadcrumb
        /// Will contain the list of ancestors based on current user's access
        /// </summary>
        [DataMember]
        [NotMapped]
        public List<GuidNamePair> Ancestors { get; set; }

        [DataMember]
        [NotMapped]
        public bool IsFavourite { get; set; }
        #endregion

        #region Navigation

        /// <summary>
        /// The parent OU
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [ForeignKey("ParentID")]
        public OU Parent { get; set; }


        /// <summary>
        /// Child OUs
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [AttachOnUpdate]
        public ICollection<OU> Children { get; set; }


        /// <summary>
        /// Multiple addresses, mailing or physicall
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [AttachOnUpdate]
        public ICollection<Address> Addresses { get; set; }

        [StringLength(100)]
        [DataMember]
        public string Website { get; set; }


        /// <summary>
        /// Phone Numbers for the OU
        /// </summary>
        [InverseProperty("OU")]
        [DataMember(EmitDefaultValue = false)]
        [AttachOnUpdate]
        public ICollection<PhoneNumber> PhoneNumbers { get; set; }

        /// <summary>
        /// values to any Setting golbally or specifically defined
        /// </summary>
        [DataMember]
        [AttachOnUpdate]
        [InverseProperty("OU")]
        public ICollection<OUSetting> Settings { get; set; }


        /// <summary>
        /// Collection of Territories that belong to the OU
        /// </summary>
        [DataMember]
        [InverseProperty("OU")]
        [AttachOnUpdate]
        public ICollection<Territory> Territories { get; set; }

        /// <summary>
        /// Collection of OUAssociations
        /// </summary>
        [DataMember]
        [InverseProperty("OU")]
        [AttachOnUpdate]
        public ICollection<OUAssociation> Associations { get; set; }

        [DataMember]
        [InverseProperty("OU")]
        [AttachOnUpdate]
        public ICollection<WebHook> WebHooks { get; set; }


        [DataMember]
        [InverseProperty("OU")]
        [AttachOnUpdate]
        public ICollection<OUShape> Shapes { get; set; }

        [DataMember]
        [InverseProperty("OU")]
        [AttachOnUpdate]
        public ICollection<OUFinanceAssociation> FinanceAssociations { get; set; }


        [DataMember]
        [InverseProperty("OU")]
        [AttachOnUpdate]
        public ICollection<OUReport> OUReports { get; set; }

        /// <summary>
        /// Favorite territory assigned to this person
        /// </summary>
        [InverseProperty("OU")]
        [DataMember]
        public ICollection<FavouriteOu> FavouriteOus { get; set; }

        /// <summary>
        /// The account that owns the OU
        /// </summary>
        [DataMember]
        [ForeignKey("AccountID")]
        public Account Account { get; set; }

        /// <summary>
        /// Delimited string of Wellknown Texts that define the boundaries of the OU.   Client should speak to an array that encapuslates this collection
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public virtual string WellKnownText { get; set; }


        [DataMember(EmitDefaultValue = false)]
        [ForeignKey("RootOrganizationID")]
        public OU RootOrganization { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [InverseProperty("OU")]
        public ICollection<OUMediaItem> OUMediaItems { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [InverseProperty("OU")]
        public ICollection<ApiKey> ApiKeys { get; set; }


        [InverseProperty("OU")]
        [DataMember]
        public ICollection<OULayer> OULayers { get; set; }

        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, this.Guid.ToString(), out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Children = FilterEntityCollection(Children, newInclusionPath);
            Addresses = FilterEntityCollection(Addresses, newInclusionPath);
            PhoneNumbers = FilterEntityCollection(PhoneNumbers, newInclusionPath);
            Territories = FilterEntityCollection(Territories, newInclusionPath);
            Associations = FilterEntityCollection(Associations, newInclusionPath);
            Shapes = FilterEntityCollection(Shapes, newInclusionPath);
            OUReports = FilterEntityCollection(OUReports, newInclusionPath);
            OUMediaItems = FilterEntityCollection(OUMediaItems, newInclusionPath);
            OULayers = FilterEntityCollection(OULayers, newInclusionPath);
            WebHooks = FilterEntityCollection(WebHooks, newInclusionPath);
            ApiKeys = FilterEntityCollection(ApiKeys, newInclusionPath);
            FavouriteOus = FilterEntityCollection(FavouriteOus, newInclusionPath);

            Parent = FilterEntity(Parent, newInclusionPath);
            Account = FilterEntity(Account, newInclusionPath);
            RootOrganization = FilterEntity(RootOrganization, newInclusionPath);
        }

        public override void PrepareNavigationProperties(Guid? createdById = null)
        {
            if (Children == null || Children.Count == 0)
            {
                return;
            }

            foreach (var child in Children)
            {
                child.RootOrganizationID = child.RootOrganizationID ?? RootOrganizationID;
            }
        }

        public bool IsRoot
        {
            get { return !ParentID.HasValue || Guid.Equals(RootOrganizationID); }
        }
    }
}
