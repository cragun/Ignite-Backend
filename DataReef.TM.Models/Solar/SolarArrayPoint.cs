using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{

    [Table("ArraySegments", Schema = "solar")]
    public class SolarArrayPoint:EntityBase
    {

        [DataMember]
        public Guid SolarArrayID { get; set; }

        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }

        [DataMember]
        public int Index { get; set; }

        #region Navigation

        [ForeignKey("SolarArrayID")]
        [DataMember]
        public SolarArray SolarArray { get; set; }

        #endregion

    }
}
