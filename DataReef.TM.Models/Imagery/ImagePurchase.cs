using DataReef.Core.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [BootstrapExcluded(BootstrapType = BootstrapType.Api)]
    [Table("ImagePurchases")]
    public class ImagePurchase : EntityBase
    {
        /// <summary>
        /// Guid of the userID who performed the purchase
        /// </summary>
        [DataMember]
        public Guid UserID { get; set; }

        /// <summary>
        /// Guid of the HighResolutionImage object
        /// </summary>
        [DataMember]
        public Guid ImageID { get; set; }

        /// <summary>
        /// OUID that the image belongs to
        /// </summary>
        [DataMember]
        public Guid OUID { get; set; }

        /// <summary>
        /// Lat of the property search
        /// </summary>
        [DataMember]
        public double Lat { get; set; }

        /// <summary>
        /// Lon of the property search
        /// </summary>
        [DataMember]
        public double Lon { get; set; }

        /// <summary>
        /// Number of tokens charged to account for this image
        /// </summary>
        [DataMember]
        public int Tokens { get; set; }

        /// <summary>
        /// Down, North, West, East,South
        /// </summary>
        [DataMember]
        [StringLength(100)]
        public string ImageType { get; set; }

        /// <summary>
        /// PropertyID 
        /// </summary>
        [DataMember]
        public Guid PropertyID { get; set; }

        /// <summary>
        /// the UniqueID of the property from the GeoServer
        /// </summary>
        [DataMember]
        [StringLength(50)]
        public string GlobalID { get; set; }

        /// <summary>
        /// did we have to obtain the image from the external provider
        /// </summary>
        [DataMember]
        public bool ImageWasCached { get; set; }

        /// <summary>
        /// the X Coordinate (in pixels) in the image of the Lat
        /// </summary>
        [DataMember]
        public float LocationX { get; set; }

        /// <summary>
        /// the Y coordinate (in pixels) in the image of the lat
        /// </summary>
        [DataMember]
        public float LocationY { get; set; }

        /// <summary>
        /// top Lat cordinate of requested frame
        /// </summary>
        [DataMember]
        public double Top { get; set; }

        /// left lon cordinate of requested frame
        [DataMember]
        public double Left { get; set; }

        /// bottom lat cordinate of requested frame
        [DataMember]
        public double Bottom { get; set; }

        /// right lon cordinate of requested frame
        [DataMember]
        public double Right { get; set; }

        /// <summary>
        /// date that the image was photographed
        /// </summary>
        [DataMember]
        public DateTime ImageDate { get; set; }

        #region Navigation


        #endregion
    }
}
