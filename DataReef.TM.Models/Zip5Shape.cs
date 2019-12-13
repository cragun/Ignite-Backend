using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    public class Zip5Shape : EntityBase
    {
        #region Properties
        
        /// <summary>
        /// The Id of the shape from the geo server
        /// </summary>
        [DataMember]
        public Guid ShapeID { get; set; }

        public String ShapeTypeID { get { return "zip5"; } }

        [DataMember]
        public String WellKnownText { get; set; }

        [DataMember]
        public float CentroidLat { get; set; }

        [DataMember]
        public float CentroidLon { get; set; }

        [DataMember]
        public float Radius { get; set; }

        [DataMember]
        public int ResidentCount { get; set; }

        #endregion Properties

        #region Navigation

        [ForeignKey("Guid")]
        public ZipArea ZipArea { get; set; }

        #endregion Navigation
    }
}
