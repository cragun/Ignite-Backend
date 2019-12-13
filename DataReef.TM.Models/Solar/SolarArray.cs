using DataReef.Core.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;


namespace DataReef.TM.Models.Solar
{

    [DataContract]
    public enum PanelOrientation
    {
        [EnumMember]
        Portrait,

        [EnumMember]
        Landscape

    }

    [DataContract]
    public enum Racking
    {
        /// <summary>
        /// use this for room
        /// </summary>
        [EnumMember]
        FixedTilt,


        /// <summary>
        /// use this for off roof
        /// </summary>
        [EnumMember]
        Freestanding

    }

    /// <summary>
    /// A Solar System is made up of one or more Arrays.  Arrays hold a collection of Panels
    /// </summary>
    /// 
    [Table("Arrays", Schema = "solar")]
    public class SolarArray : EntityBase
    {

        public SolarArray()
        {
            AddDefaultExcludedProperties("SolarSystem");
        }
        /// <summary>
        /// look at system first, this can override it
        /// </summary>
        [DataMember]
        public Guid? SolarPanelID { get; set; }

        [DataMember]
        public Guid SolarSystemID { get; set; }

        [DataMember]
        public PanelOrientation PanelOrientation { get; set; }

        [DataMember]
        public Guid? InverterID { get; set; }

        [DataMember]
        public int Azimuth { get; set; }

        [DataMember]
        public int? RidgeLineAzimuth { get; set; }

        [DataMember]
        public int Tilt { get; set; }

        [DataMember]
        public Racking Racking { get; set; }

        /// <summary>
        /// Centimeters
        /// </summary>
        [DataMember]
        public double ModuleSpacing { get; set; }

        /// <summary>
        /// Meters
        /// </summary>
        [DataMember]
        public double RowSpacing { get; set; }

        [DataMember]
        public double SolarArrayRotation { get; set; }

        [DataMember]
        public double AnchorPointX { get; set; }
        [DataMember]
        public double AnchorPointY { get; set; }

        [DataMember]
        public double CenterX { get; set; }
        [DataMember]
        public double CenterY { get; set; }

        [DataMember]
        public double CenterLatitude { get; set; }
        [DataMember]
        public double CenterLongitude { get; set; }

        [DataMember]
        public bool IsManuallyEntered { get; set; }

        [DataMember]
        public Int16 Shading { get; set; }

        [DataMember]
        public double PanXOffset { get; set; }

        [DataMember]
        public double PanYOffset { get; set; }

        [DataMember]
        public string BoundingRect { get; set; }

        [DataMember]
        public string Bounds { get; set; }

        [DataMember]
        public int ManuallyEnteredPanelsCount { get; set; }

        /// <summary>
        /// The provider profile id for Solar Usage Profile (="SOLAR_PV_" + Proposal.Guid)
        /// </summary>
        [DataMember]
        public string GenabilitySolarProviderProfileID { get; set; }

        [DataMember]
        public bool FireOffsetIsEnabled { get; set; }

        [DataMember]
        public double FireOffset { get; set; }

        #region Navigation

        /// <summary>
        /// points make up the polygon.  There are anchorpoints between each segment.  Used to recreate array shape
        /// </summary>
        [DataMember]
        [AttachOnUpdate]
        public ICollection<SolarArrayPoint> Points { get; set; }

        /// <summary>
        /// the many to many collection of panels and for this array  The SolarArrayPanel defines the panel and its position and other attributes within the array
        /// </summary>
        [DataMember]
        [InverseProperty("SolarArray")]
        [AttachOnUpdate]
        public ICollection<SolarArrayPanel> Panels { get; set; }

        [ForeignKey("SolarPanelID")]
        [DataMember]
        public SolarPanel SolarPanel { get; set; }

        [ForeignKey("SolarSystemID")]
        [InverseProperty("SolarArrays")]
        [DataMember]
        public SolarSystem SolarSystem { get; set; }

        [ForeignKey("InverterID")]
        [DataMember]
        public Inverter Inverter { get; set; }

        #endregion

        [NotMapped]
        public int ActivePanelsCount
        {
            get
            {
                return IsManuallyEntered ? ManuallyEnteredPanelsCount : Panels == null ? 0 : Panels.Count(p => !p.IsHidden);
            }
        }

        public bool IsArrayValid()
        {
            return (IsManuallyEntered && ManuallyEnteredPanelsCount > 0) || (!IsManuallyEntered && Points != null && Points.Count > 0);
        }
    }
}
