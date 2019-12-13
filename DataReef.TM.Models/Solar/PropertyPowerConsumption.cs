using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    public class PropertyPowerConsumption : PowerConsumption
    {
        [DataMember]
        public Guid PropertyID { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey(nameof(PropertyID))]
        public Property Property { get; set; }

        #endregion
    }
}