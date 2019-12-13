using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.OUs
{
    public class EntityWithShape
    {
        public Guid Guid { get; set; }

        public string Name { get; set; }

        public string WellKnownText { get; set; }

        public int ShapesVersion { get; set; }

        public EntityWithShape(OU ou)
        {
            Guid = ou.Guid;
            Name = ou.Name;
            WellKnownText = ou.WellKnownText;
            ShapesVersion = ou.ShapesVersion;
        }

        public EntityWithShape(Territory territory)
        {
            Guid = territory.Guid;
            Name = territory.Name;
            WellKnownText = territory.WellKnownText;
            ShapesVersion = territory.ShapesVersion;
        }
    }
}
