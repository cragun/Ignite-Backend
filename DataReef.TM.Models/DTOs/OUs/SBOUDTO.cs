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
    public class SBOUDTO
    {
        [DataMember]
        public Guid Guid { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsArchived { get; set; }

        [DataMember]
        public bool IsDisabled { get; set; }

        [DataMember]
        public IEnumerable<SBTerritoryDTO> Territories { get; set; }

        [DataMember]
        public IEnumerable<SBOUDTO> Children { get; set; }

        [DataMember]
        public bool HasChildren { get; set; }

        public SBOUDTO(OU ouObject, bool includeChildren = true)
        {
            if(ouObject != null)
            {
                Guid = ouObject.Guid;
                Name = ouObject.Name;
                IsArchived = ouObject.IsArchived;
                IsDisabled = ouObject.IsDisabled;

                HasChildren = ouObject.Children?.Any() == true;
                if (ouObject.Children?.Any() == true && includeChildren)
                {
                    Children = ouObject
                        ?.Children
                        ?.Where(o => !o.IsDeleted && !o.IsDisabled && !o.IsArchived)
                        ?.Select(c => new SBOUDTO(c, false));
                }

                if(ouObject.Territories?.Any() == true)
                {
                    Territories = ouObject
                        ?.Territories
                        ?.Where(t => !t.IsDeleted && !t.IsArchived)
                        ?.Select(t => new SBTerritoryDTO(t));
                }
            }
            
        }
    }
}
