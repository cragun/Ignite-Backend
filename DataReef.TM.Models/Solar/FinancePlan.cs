using DataReef.Core.Attributes;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.DTOs.Solar.Proposal;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Finance;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("FinancePlans", Schema = "solar")]
    public class FinancePlan : EntityBase
    {
        [DataMember]
        public Guid SolarSystemID { get; set; }

        [DataMember]
        public SolarProviderType SolarProviderType { get; set; }

        [DataMember]
        public FinancePlanType FinancePlanType { get; set; }

        [DataMember]
        public Guid? FinancePlanDefinitionID { get; set; }

        [DataMember]
        public string RequestJSON { get; set; }

        [DataMember]
        public string ResponseJSON { get; set; }

        [DataMember]
        public string PricingQuoteID { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("SolarSystemID")]
        public SolarSystem SolarSystem { get; set; }

        [DataMember]
        [InverseProperty("FinancePlan")]
        [AttachOnUpdate]
        public ICollection<FinanceDocument> Documents { get; set; }

        //[DataMember]
        //[InverseProperty("FinancePlan")]
        //[AttachOnUpdate]
        //public ICollection<ProposalData> ProposalData { get; set; }

        [DataMember]
        [ForeignKey(nameof(FinancePlanDefinitionID))]
        public FinancePlanDefinition FinancePlanDefinition { get; set; }

        #endregion

        #region Computed Properties

        private LoanRequest _request;
        [NotMapped]
        [JsonIgnore]
        public LoanRequest Request
        {
            get
            {
                return GetRequest();
            }
            set
            {
                _request = value;
                if (value != null)
                {
                    RequestJSON = JsonConvert.SerializeObject(value);
                }
                else
                {
                    RequestJSON = null;
                }
            }
        }

        private LoanResponse _response;
        [NotMapped]
        [JsonIgnore]
        public LoanResponse Response
        {
            get
            {
                if (_response == null)
                {
                    try
                    {
                        _response = JsonConvert.DeserializeObject<LoanResponse>(ResponseJSON);
                    }
                    catch { }

                }

                return _response;
            }
            set
            {
                _response = value;
                if (value != null)
                {
                    ResponseJSON = JsonConvert.SerializeObject(value);
                }
                else
                {
                    ResponseJSON = null;
                }
            }
        }
        #endregion

        public LoanRequest GetRequest(bool newInstance = false)
        {
            if (newInstance)
            {
                try
                {
                    return JsonConvert.DeserializeObject<LoanRequest>(RequestJSON);
                }
                catch { }
            }
            else
            {
                if (_request == null)
                {
                    try
                    {
                        _request = JsonConvert.DeserializeObject<LoanRequest>(RequestJSON);
                    }
                    catch { }
                }
                return _request;
            }
            return null;
        }

        public LoanResponse GetResponse(bool newInstance = false)
        {
            if (newInstance)
            {
                try
                {
                    return JsonConvert.DeserializeObject<LoanResponse>(ResponseJSON);
                }
                catch { }
            }
            else
            {
                if (_response == null)
                {
                    try
                    {
                        _response = JsonConvert.DeserializeObject<LoanResponse>(ResponseJSON);
                    }
                    catch { }
                }
                return _response;
            }
            return null;
        }

        public FinancePlan Clone(Guid solarSystemID, CloneSettings cloneSettings)
        {
            //if (this.ProposalData == null) throw new MissingMemberException("Missing FinancePlan.ProposalData from Object Graph");
            if (this.Documents == null) throw new MissingMemberException("Missing FinancePlan.Documents from Object Graph");


            FinancePlan ret = (FinancePlan)this.MemberwiseClone();
            ret.Reset();
            ret.SolarSystemID = solarSystemID;
            ret.SolarSystem = null;
            ret.FinancePlanDefinition = null;

            ret.Documents = new List<FinanceDocument>();
            foreach (var doc in this.Documents)
            {
                ret.Documents.Add(doc.Clone(this.Guid));
            }

            //ret.ProposalData = new List<ProposalData>();
            //foreach (var pd in this.ProposalData)
            //{
            //    ret.ProposalData.Add(pd.Clone(this.Guid));
            //}



            return ret;

        }

    }
}