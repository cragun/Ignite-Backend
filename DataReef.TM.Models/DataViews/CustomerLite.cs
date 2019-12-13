using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews
{
    [DataContract]
    [NotMapped]

    public class CustomerLite
    {
        [DataMember]
        public Guid PropertyId { get; set; }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public double Lat { get; set; }

        [DataMember]
        public double Lon { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string OrganizationName { get; set; }

        [DataMember]
        public Guid OrganizationId { get; set; }

        [DataMember]
        public string SalesPersonName { get; set; }

        [DataMember]
        public Guid SalesPersonId { get; set; }

        [DataMember]
        public System.DateTime DateCreated { get; set; }

        [DataMember]
        public System.DateTime DateLastModified { get; set; }





    }
}
