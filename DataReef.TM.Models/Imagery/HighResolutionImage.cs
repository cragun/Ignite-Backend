using DataReef.Core.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [BootstrapExcluded(BootstrapType = BootstrapType.Api)]
    [Table("HighResolutionImages")]
    public class HighResolutionImage : EntityBase
    {
        [Index("idx_hires_coords",0)]
        [DataMember]
        public double Top { get; set; }

        [Index("idx_hires_coords", 1)]
        [DataMember]
        public double Left { get; set; }

        [Index("idx_hires_coords", 2)]
        [DataMember]
        public double Bottom { get; set; }

        [Index("idx_hires_coords", 3)]
        [DataMember]
        public double Right { get; set; }

        [DataMember]
        public int Width { get; set; }

        [DataMember]
        public int Height { get; set; }

        [DataMember]
        public double MapUnitsPerPixelX { get; set; }

        [DataMember]
        public double MapUnitsPerPixelY { get; set; }

        [DataMember]
        public double SkewX { get; set; }

        [DataMember]
        public double SkewY { get; set; }

        [DataMember]
        public int Resolution { get; set; }

        [StringLength(100)]
        public string Source { get; set; }
    }
}
