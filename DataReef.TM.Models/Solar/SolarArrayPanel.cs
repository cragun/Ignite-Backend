using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;


namespace DataReef.TM.Models.Solar
{
    [Table("ArrayPanels", Schema = "solar")]
    public class SolarArrayPanel:EntityBase
    {
        [DataMember]
        public Guid SolarArrayID { get; set; }

        [DataMember]
        public bool IsHidden { get; set; }

        [DataMember]
        public int Row { get; set; }

        [DataMember]
        public int Column { get; set; }

        [DataMember]
        public string WellKnownText { get; set; }

        [DataMember]
        public double X1 { get; set; }

        [DataMember]
        public double X2 { get; set; }

        [DataMember]
        public double Y1 { get; set; }

        [DataMember]
        public double Y2 { get; set; }

        [DataMember]
        public double CentroidX { get; set; }

        [DataMember]
        public double CentroidY { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("SolarArrayID")]
        [InverseProperty("Panels")]
        public SolarArray SolarArray { get; set; }

        #endregion
    }
}
