using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;


namespace DataReef.TM.Models.Solar
{
    [Table("PowerConsumption", Schema = "solar")]
    [KnownType(typeof(SolarSystemPowerConsumption))]
    [KnownType(typeof(PropertyPowerConsumption))]
    public abstract class PowerConsumption : EntityBase
    {
        [DataMember]
        public int Year { get; set; }

        [DataMember]
        public int Month { get; set; }

        [DataMember]
        public decimal Watts { get; set; }

        [DataMember]
        public decimal Cost { get; set; }

        [DataMember]
        public bool IsManuallyEntered { get; set; }
    }
}
