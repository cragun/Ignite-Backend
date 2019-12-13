using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Solar
{
    [Table("SystemProductionMonths", Schema = "solar")]
    public class SystemProductionMonth : EntityBase
    {
        #region Properties

        [DataMember]
        public Guid SystemProductionID { get; set; }

        [DataMember]
        public float Consumption { get; set; }

        [DataMember]
        public float PreSolarCost { get; set; }

        [DataMember]
        public float PostSolarCost { get; set; }

        [DataMember]
        public float Production { get; set; }

        [DataMember]
        public Int16 Month { get; set; }

        [DataMember]
        public Int16 Year { get; set; }

        #endregion


        #region Navigation

        [ForeignKey("SystemProductionID")]
        public SystemProduction SystemProduction { get; set; }

        #endregion

        public SystemProductionMonth Clone(Guid systemProductionID)
        {

            SystemProductionMonth ret = (SystemProductionMonth)this.MemberwiseClone();
            ret.Reset();
            ret.SystemProductionID = systemProductionID;
            ret.SystemProduction = null;
            return ret;

        }
    }
}
