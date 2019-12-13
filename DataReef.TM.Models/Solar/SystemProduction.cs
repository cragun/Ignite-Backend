using DataReef.Core.Attributes;
using DataReef.TM.Models.DTOs.Solar.Proposal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("SystemsProduction", Schema = "solar")]
    public class SystemProduction : EntityBase
    {
        #region properties

        //[DataMember]
        //public Guid SolarSystemID { get; set; }

        [DataMember]
        public float Consumption { get; set; }

        [DataMember]
        public float PreSolarCost { get; set; }

        [DataMember]
        public float PostSolarCost { get; set; }

        [DataMember]
        public float Production { get; set; }

        [DataMember]
        public string TariffID { get; set; }

        [DataMember]
        public float? PostAddersConsumption { get; set; }

        [DataMember]
        public float? PostSolarConsumption { get; set; }

        #endregion

        #region Navigation

        [ForeignKey("Guid")]
        [DataMember]
        public SolarSystem SolarSystem { get; set; }

        [DataMember]
        [InverseProperty("SystemProduction")]
        [AttachOnUpdate]
        public ICollection<SystemProductionMonth> Months { get; set; }

        #endregion

        public SystemProduction Clone(Guid solarSystemId, CloneSettings cloneSettings)
        {
            if (this.Months == null) throw new MissingMemberException("Missing SolarSystem.SystemProduction.Months in Object Graph");

            SystemProduction ret = (SystemProduction)this.MemberwiseClone();
            ret.Reset();
            ret.Guid = solarSystemId;
            ret.SolarSystem = null;

            ret.Months = Months
                            .Select(m => m.Clone(ret.Guid))
                            .ToList();
            return ret;
        }

        public float UsageOffset()
        {
            return Consumption == 0 ? 0 : Production * 100 / Consumption;
        }
    }
}
