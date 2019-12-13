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
    public class PersonLite
    {
        public PersonLite()
        {
            this.Organizations = new List<Guid>();
        }

        [DataMember]
        public Guid Guid { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public ICollection<Guid> Organizations { get; set; }

        [DataMember]
        public bool IsActive { get; set; }
    }
}
