using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models.DTOs.Solar.Proposal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{

    [Table("ProposalsData", Schema = "solar")]
    public class ProposalData : EntityBase
    {
        [DataMember]
        public Guid FinancePlanID { get; set; }

        [DataMember]
        public Guid ProposalID { get; set; }

        [DataMember]
        public Guid ProposalTemplateID { get; set; }

        [DataMember]
        public DateTime ProposalDate { get; set; }

        [DataMember]
        public string ContractorID { get; set; }

        [DataMember]
        public string UserInputDataLinksJSON { get; set; }

        [DataMember]
        public string DocumentDataLinksJSON { get; set; }

        [DataMember]
        public Guid SalesRepID { get; set; }

        [DataMember]
        public DateTime? SignatureDate { get; set; }

        /// <summary>
        /// A JSON containing custom data entered by the customer or sales rep on the web proposal, and sent back to backend.
        /// </summary>
        [DataMember]
        public string ProposalDataJSON { get; set; }

        /// <summary>
        /// True if all the needed data is stored in No-SQL
        /// </summary>
        [DataMember]
        public bool UsesNoSQLAggregatedData { get; set; }

        /// <summary>
        /// Other proposal related data captured and stored here
        /// (e.g. Location of the user that signed/accepted the proposal)
        /// </summary>
        [DataMember]
        public string MetaInformationJSON { get; set; }


        [DataMember]
        public bool UsageCollected { get; set; }

        #region Computed Properties

        [NotMapped]
        public List<UserInputDataLinks> UserInputLinks
        {
            get
            {
                try
                {
                    return string.IsNullOrEmpty(UserInputDataLinksJSON) ? null : JsonConvert.DeserializeObject<List<UserInputDataLinks>>(UserInputDataLinksJSON);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                UserInputDataLinksJSON = value == null ? null : JsonConvert.SerializeObject(value);
            }
        }

        [NotMapped]
        public List<DocumentDataLink> DocumentDataLinks
        {
            get
            {
                try
                {
                    return string.IsNullOrEmpty(DocumentDataLinksJSON) ? null : JsonConvert.DeserializeObject<List<DocumentDataLink>>(DocumentDataLinksJSON);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                DocumentDataLinksJSON = value == null ? null : JsonConvert.SerializeObject(value);
            }
        }

        [NotMapped]
        public ProposalDataMetaInformation MetaInformation
        {
            get
            {
                try
                {
                    return string.IsNullOrEmpty(MetaInformationJSON) ? null : JsonConvert.DeserializeObject<ProposalDataMetaInformation>(MetaInformationJSON);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                MetaInformationJSON = value == null ? null : JsonConvert.SerializeObject(value);
            }
        }

        #endregion

        #region Navigation Properties

        //[ForeignKey(nameof(FinancePlanID))]
        //public FinancePlan FinancePlan { get; set; }

        #endregion

        public ProposalData Clone(Guid financePlanID)
        {
            ProposalData ret = (ProposalData)this.MemberwiseClone();
            ret.Reset();
            ret.FinancePlanID = financePlanID;

            return ret;
        }
    }
}
