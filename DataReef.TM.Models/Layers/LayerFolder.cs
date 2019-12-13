using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Layers
{
    public class LayerFolder : EntityBase
    {
        [DataMember]
        [StringLength(200)]
        public string DescriptionString { get; set; }

        [DataMember]
        public Guid? ParentID { get; set; }

        /// <summary>
        /// A string to identify an image for the folder.  Not required
        /// </summary>
        [DataMember]
        [StringLength(100)]
        public string ImageName { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("ParentID")]
        public LayerFolder Parent { get; set; }

        [DataMember]
        [InverseProperty("Parent")]
        public ICollection<LayerFolder> Children { get; set; }

        [DataMember]
        [InverseProperty("LayerFolder")]
        public ICollection<LayerDefinition> LayerDefinitions { get; set; }

        #endregion
    }
}
