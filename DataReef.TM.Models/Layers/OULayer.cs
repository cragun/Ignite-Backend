using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System;

namespace DataReef.TM.Models.Layers
{
    /// <summary>
    /// defines the Layers that are visible for the OU.  Can define these layers at the top OU and API will return the layer for any sub OU if that sub ou has NO layers defined.
    /// </summary>
    public class OULayer:EntityBase
    {
        /// <summary>
        /// Guid of the OU that is subsribed to this layer
        /// </summary>
        [DataMember]
        public Guid OUID { get; set; }

        /// <summary>
        /// Guid of the Layer that the OU is subscribing to
        /// </summary>
        [DataMember]
        public Guid LayerID { get; set; }

        /// <summary>
        /// The Hex color as defined by the OU
        /// </summary>
        [DataMember]
        [StringLength(10)]
        public string Color { get; set; }

        /// <summary>
        /// to be used by the client as a favorite ( future feature )
        /// </summary>
        [DataMember]
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Can sub OUs see this layer.  Yes this to define layers at the root and let all sub OUs consume them.  But with some layers you may not wish to do this for security / privacy
        /// </summary>
        [DataMember]
        public bool IsVisibleDownStream { get; set; }

        /// <summary>
        /// Can parent OUs see this layer?  A child OU may have its own proprietary data in which case sharing up stream could be a violation of secruity and privacy.
        /// </summary>
        [DataMember]
        public bool IsVisibleUpStream { get; set; }


        #region Navigation 

        [ForeignKey("LayerID")]
        [DataMember]
        public Layer Layer { get; set; }


        [ForeignKey("OUID")]
        [DataMember]
        public OU OU { get; set; }

        #endregion



    }
}
