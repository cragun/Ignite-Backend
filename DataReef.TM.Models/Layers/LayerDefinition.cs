using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Layers
{
    public class LayerDefinition : EntityBase
    {
        /// <summary>
        /// The guid of the folder that the layer belongs to 
        /// </summary>
        [DataMember]
        public Guid FolderID { get; set; }

        [DataMember]
        [StringLength(200)]
        public string DescriptionString { get; set; }

        [DataMember]
        [StringLength(50)]
        public string LayerKey { get; set; }

        /// <summary>
        /// if not active, should not be displayed in any UI, for the client to choose
        /// </summary>
        [DataMember]
        public bool IsLayerActive { get; set; }

        /// <summary>
        /// Describes how the aggegate views are rendered
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

        /// <summary>
        /// the folder that the layer belongs to
        /// </summary>
        [DataMember]
        [ForeignKey("FolderID")]
        public LayerFolder LayerFolder { get; set; }

      
        #endregion

    }
}

