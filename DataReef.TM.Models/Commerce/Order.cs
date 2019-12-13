using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Commerce
{
    [DataContract]
    public enum OrderStatus
    {
        [EnumMember]
        Pending = 0,
        [EnumMember]
        InProgress = 1,


        [EnumMember]
        Completed = 4,

        [EnumMember]
        Error = 5
    }

    [DataContract]
    public enum OrderType
    {
        [EnumMember]
        NewLeads = 1,
        [EnumMember]
        UploadCSV = 2
    }

    [Table("Orders", Schema = "commerce")]
    public class Order : EntityBase
    {
        public Order()
        {
            this.Details = new List<OrderDetail>();
        }

        [DataMember]
        public int InitialRecords { get; set; }

        [DataMember]
        public int Results { get; set; }

        [DataMember]
        public Guid? PrescreenBatchId { get; set; }

        [StringLength(250)]
        [DataMember]
        public string CSVFilePath { get; set; }

        [DataMember]
        public OrderType OrderType { get; set; }

        [DataMember]
        public Guid? WorkflowId { get; set; }

        /// <summary>
        /// Guid of the person who owns this order
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }

        public OrderStatus Status { get; set; }

        [InverseProperty("Order")]
        public ICollection<OrderDetail> Details { get; set; }


    }
}
