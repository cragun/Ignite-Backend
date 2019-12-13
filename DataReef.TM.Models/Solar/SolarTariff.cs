using DataReef.TM.Models.DTOs.Solar.Proposal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Solar
{
    [Table("Tariffs", Schema = "solar")]
    public class SolarTariff : EntityBase
    {
        [DataMember]
        public string TariffID { get; set; }

        [DataMember]
        public string TariffName { get; set; }

        [DataMember]
        public float PricePerKWH { get; set; }

        [DataMember]
        public string UtilityID { get; set; }

        [DataMember]
        public string UtilityName { get; set; }

        [DataMember]
        public string MasterTariffID { get; set; }

        [DataMember]
        public string TariffCode     { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("Guid")]
        public Proposal Proposal { get; set; }

        #endregion


        public SolarTariff Clone(Guid proposalID, CloneSettings cloneSettings)
        {
            SolarTariff ret = (SolarTariff)this.MemberwiseClone();
            ret.Reset();
            ret.Guid = proposalID;
            return ret;


        }

    }
}
