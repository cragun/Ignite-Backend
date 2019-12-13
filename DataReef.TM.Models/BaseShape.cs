using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models
{
    [DataContract]
    public abstract class BaseShape : EntityBase
    {
        #region Properties

        [DataMember]
        public Guid ShapeID { get; set; }

        [StringLength(50)]
        [DataMember]
        public string ShapeTypeID { get; set; }

        [DataMember]
        public string WellKnownText { get; set; }

        [DataMember]
        public float CentroidLat { get; set; }

        [DataMember]
        public float CentroidLon { get; set; }

        [DataMember]
        public float Radius { get; set; }

        [DataMember]
        public Guid? ParentID { get; set; }

        [DataMember]
        public long ResidentCount { get; set; }

        #endregion

        #region Navigation

        #endregion

        #region ShouldSerialize


        #endregion


        public static string UserDrawnTerritoryShapeTypeID = "user-drawn-territory";
    }

}
