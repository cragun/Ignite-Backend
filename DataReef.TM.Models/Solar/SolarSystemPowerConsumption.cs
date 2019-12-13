using DataReef.TM.Models.DTOs.Solar.Proposal;
using DataReef.TM.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    public class SolarSystemPowerConsumption : PowerConsumption
    {
        public SolarSystemPowerConsumption()
        {
            AddDefaultExcludedProperties("SolarSystem");
        }

        [DataMember]
        public Guid SolarSystemID { get; set; }

        [DataMember]
        public PowerInformationSource? CostSource { get; set; }

        [DataMember]
        public PowerInformationSource? UsageSource { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey(nameof(SolarSystemID))]
        public SolarSystem SolarSystem { get; set; }

        #endregion

        internal SolarSystemPowerConsumption Clone(Guid solarSystemID, CloneSettings cloneSettings)
        {
            SolarSystemPowerConsumption ret = (SolarSystemPowerConsumption)MemberwiseClone();
            ret.Reset();
            ret.SolarSystemID = solarSystemID;
            ret.SolarSystem = null;
            return ret;
        }
    }
}