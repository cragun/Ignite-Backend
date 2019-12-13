using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.ClientAPI
{
    public class RoofPlaneEditDataView
    {
        public Guid Id { get; set; }
        //azimuth, tilt, shading

        /// <summary>
        /// New Azimuth value
        /// Will only update this Azimuth if this property is set.
        /// </summary>
        public int? Azimuth { get; set; }

        /// <summary>
        /// New Tilt value.
        /// Will only update this Tilt if this property is set.
        /// </summary>
        public double? Tilt { get; set; }

        /// <summary>
        /// New Shading value.
        /// Will only update this Shading if this property is set.
        /// </summary>
        public int? Shading { get; set; }
    }
}
