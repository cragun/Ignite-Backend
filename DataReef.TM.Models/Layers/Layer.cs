using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System;

namespace DataReef.TM.Models.Layers
{

    [DataContract]
    public enum VisualizationType
    {
        [EnumMember]
        SizedCircles=0,

        [EnumMember]
        HeatMap=1,

        [EnumMember]
        Shape=2

    }

    public class Layer:EntityBase
    {

        /// <summary>
        /// the category that the layer should be grouped in by the UI
        /// </summary>
        [DataMember]
        [StringLength(100)]
        public string Category { get; set; }


        [DataMember]
        [StringLength(200)]
        public string Description { get; set; }


        [DataMember]
        [StringLength(50)]
        public string LayerKey { get; set; }

        
        /// <summary>
        /// if not active, should not be displayed in any UI, for the client to choose
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }
        

        /// <summary>
        ///Describes how the aggegate views are rendered
        /// </summary>
        [DataMember]
        public VisualizationType VisualizationType { get; set; }


        /// <summary>
        /// The hex of the default color
        /// </summary>
        [DataMember]
        [StringLength(10)]
        public string DefaultColor { get; set; }

        [DataMember]
        public DateTime LastCompileDate { get; set; }

        #region Navigation 

        [InverseProperty("Layer")]
        [DataMember]
        public ICollection<OULayer> OULayers { get; set; }

        #endregion

    }
}
