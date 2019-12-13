using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("Inverters", Schema = "solar")]
    public class Inverter:EntityBase
    {       
        [DataMember]
        public bool IsMicroInverter { get; set; }

        [DataMember]
        public string Model         { get; set; }

        [DataMember]
        public string Manufacturer  { get; set; }

        [DataMember]
        public double Efficiency    { get; set; }

        [DataMember]
        public int MaxSystemSizeInWatts { get; set; }
    }
}
