using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DataViews
{
    [DataContract]
    [NotMapped]
    public class Analytic
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public double Value { get; set; }
    }
}