using DataReef.Core;
using DataReef.Core.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Spruce
{
    [Table("QuoteRequests", Schema = "Spruce")]
    public class QuoteRequest : EntityBase
    {
        [DataMember]
        public Guid LegionPropertyID { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("LegionPropertyID")]
        public Property LegionProperty { get; set; }

        [DataMember]
        [InverseProperty("QuoteRequest")]
        [AttachOnUpdate]
        public Init SetupInfo { get; set; }

        [DataMember]
        [InverseProperty("QuoteRequest")]
        [AttachOnUpdate]
        public Applicant AppInfo { get; set; }

        [DataMember]
        [InverseProperty("QuoteRequest")]
        [AttachOnUpdate]
        public CoApplicant CoAppInfo { get; set; }

        [DataMember]
        [AttachOnUpdate]
        public Employment AppEmployment { get; set; }

        [DataMember]
        [AttachOnUpdate]
        public Employment CoAppEmployment { get; set; }

        [DataMember]
        [AttachOnUpdate]
        public IncomeDebt AppIncomeDebt { get; set; }

        [DataMember]
        [AttachOnUpdate]
        public IncomeDebt CoAppIncomeDebt { get; set; }

        [DataMember]
        [InverseProperty("QuoteRequest")]
        public SpruceProperty Property { get; set; }

        [DataMember]
        [InverseProperty("QuoteRequest")]
        [AttachOnUpdate]
        public QuoteResponse QuoteResponse { get; set; }


        [DataMember]
        [InverseProperty("QuoteRequest")]
        [AttachOnUpdate]
        public GenDocsRequest GenDocsRequest { get; set; }

        [DataMember]
        public string CallbackJSON { get; set; }



        #endregion

        public bool IsModelValid()
        {
            int methodID = Int32.Parse(SetupInfo.DeliveryMethodId);

            if (AppInfo != null)
            {
                AppInfo.QuoteRequest = this;
            }

            var isValid = AppInfo != null && AppInfo.IsValid(methodID);

            switch (methodID)
            {
                case 1:
                    return isValid
                        && AppEmployment != null
                        && AppIncomeDebt != null
                        && Property != null;
            }

            return isValid;
        }
    }
}
