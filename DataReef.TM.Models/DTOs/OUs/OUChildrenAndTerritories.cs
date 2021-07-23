using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.OUs
{
    [DataContract]
    [NotMapped]
    public class OUChildrenAndTerritories
    {
        [DataMember]
        public Guid OUID { get; set; }
         
        [DataMember]
        public IEnumerable<EntityWithShape> Children { get; set; }

        [DataMember]
        public IEnumerable<EntityWithShape> Territories { get; set; } 
    }
     

    [DataContract]
    [NotMapped]
    public class OUAndTerritoryForPerson
    {
        [DataMember]
        public Guid OUID { get; set; }

        [DataMember]
        public string Name { get; set; }
          
        [DataMember]
        public EntityWithShape Territory { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    } 
}
