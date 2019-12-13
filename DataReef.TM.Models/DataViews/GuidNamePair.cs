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
    public class GuidNamePair
    {
        public GuidNamePair() { }

        public GuidNamePair(Guid guid, string name)
        {
            Guid = guid;
            Name = name;
        }

        public GuidNamePair(EntityBase entity)
        {
            Guid = entity.Guid;
            Name = entity.Name;
        }

        [DataMember]
        public Guid Guid { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
