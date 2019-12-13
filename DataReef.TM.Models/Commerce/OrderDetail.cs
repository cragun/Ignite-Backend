using DataReef.TM.Models.DTOs.Commerce;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Commerce
{
    [Table("OrderDetails", Schema = "commerce")]
    public class OrderDetail : EntityBase
    {
        [DataMember]
        public Guid OrderID { get; set; }

        [DataMember]
        public string Json { get; set; }

        [ForeignKey("OrderID")]
        public Order Order { get; set; }

        [JsonIgnore]
        [NotMapped]
        public bool DeSerializeDetails { get; set; }

        private LeadOrderDetailDto _details;
        [NotMapped]
        public LeadOrderDetailDto Details
        {
            get
            {
                if (_details != null)
                {
                    return _details;
                }

                if (!DeSerializeDetails || string.IsNullOrWhiteSpace(Json))
                    return null;
                _details = JsonConvert.DeserializeObject<LeadOrderDetailDto>(Json);
                return _details;
            }
            set { }
        }
    }
}