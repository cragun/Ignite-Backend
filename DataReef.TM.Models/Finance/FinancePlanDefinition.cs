using DataReef.Core.Attributes;
using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Finance
{
    /// <summary>
    /// This table defines the finance plan
    /// </summary>
    [Table("PlanDefinitions", Schema = "finance")]
    public partial class FinancePlanDefinition : EntityBase
    {
        /// <summary>
        /// guid of the financial provider
        /// </summary>
        [DataMember]
        public Guid ProviderID { get; set; }

        /// <summary>
        /// type of plan
        /// </summary>
        [DataMember]
        public FinancePlanType Type { get; set; }

        /// <summary>
        /// Is the plan termporarily disabled ( vs a delete )
        /// </summary>
        [DataMember]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Plan number of years.
        /// </summary>
        [DataMember]
        public int TermInYears { get; set; }

        /// <summary>
        /// Plan number of months. There are plans like: 18 NI/NP + 20 years, and we need to model these as well
        /// If this is 0, we'll use TermInYears
        /// </summary>
        [DataMember]
        public int TermInMonths { get; set; }

        [DataMember]
        public int? FinanceOptionId { get; set; }

        [DataMember]
        public double? DealerFee { get; set; }

        [DataMember]
        public double? LenderFee { get; set; }

        [DataMember]
        public double? PPW { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public double? LenderFee { get; set; }

        [DataMember]
        public double? PPW { get; set; }

        /// <summary>
        /// This property is used for third party integrations.
        /// </summary>
        [DataMember]
        public FinancePlanIntegrationProvider IntegrationProvider { get; set; }

        /// <summary>
        /// This APR is used by Loans that use a 3rd party intagration
        /// </summary>
        [DataMember]
        public float Apr { get; set; }

        /// <summary>
        /// Used to store Finance Plan specific meta data (e.g. Provider internal IDs used on API integrations).
        /// </summary>
        [DataMember]
        public string MetaDataJSON { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("ProviderID")]
        public FinanceProvider Provider { get; set; }


        [AttachOnUpdate]
        [DataMember]
        public ICollection<FinanceDetail> Details { get; set; }

        [AttachOnUpdate]
        [DataMember]
        public ICollection<OUFinanceAssociation> Associations { get; set; }

        #endregion

        public int GetTermInMonths()
        {
            return TermInMonths == 0 ? TermInYears * 12 : TermInMonths;
        }

        public T GetMetaData<T>()
        {
            if (string.IsNullOrWhiteSpace(MetaDataJSON))
            {
                return default(T);
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(MetaDataJSON);
            }
            catch { }

            return default(T);
        }
    }
}
