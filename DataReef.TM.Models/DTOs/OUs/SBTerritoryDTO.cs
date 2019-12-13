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
    public class SBTerritoryDTO
    {
        [DataMember]
        public Guid Guid { get; set; }

        [DataMember]
        public string Name { get; set; }

       

        public SBTerritoryDTO(Territory ouObject)
        {
            if(ouObject != null)
            {
                Guid = ouObject.Guid;
                Name = ouObject.Name;
            }
            
        }
    }
}
