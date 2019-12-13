using DataReef.Core.Attributes;
using DataReef.TM.Models.DTOs.Solar.Proposal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("RoofPlanes", Schema = "solar")]
    public class RoofPlane : EntityBase
    {
        [DataMember]
        public int Azimuth { get; set; }

        [DataMember]
        public double CenterX { get; set; }
        [DataMember]
        public double CenterY { get; set; }

        [DataMember]
        public double CenterLatitude { get; set; }
        [DataMember]
        public double CenterLongitude { get; set; }

        [DataMember]
        public string GenabilitySolarProviderProfileID { get; set; }

        [DataMember]
        public bool IsManuallyEntered { get; set; }

        [DataMember]
        public int ManuallyEnteredPanelsCount { get; set; }

        [DataMember]
        public double ModuleSpacing { get; set; }

        [DataMember]
        public double Pitch { get; set; }

        [DataMember]
        public int Racking { get; set; }

        [DataMember]
        public double RowSpacing { get; set; }

        [DataMember]
        public int Shading { get; set; }

        [DataMember]
        public Guid InverterID { get; set; }

        [DataMember]
        public Guid SolarPanelID { get; set; }

        [DataMember]
        public Guid SolarSystemID { get; set; }

        [DataMember]
        public double Tilt { get; set; }

        [DataMember]
        public double GridCenterX { get; set; }

        [DataMember]
        public double GridCenterY { get; set; }

        [NotMapped]
        public int PanelsCount
        {
            get
            {
                return IsManuallyEntered ? ManuallyEnteredPanelsCount : Panels?.Count() ?? 0;
            }
        }

        [NotMapped]
        public bool IsValid
        {
            get
            {
                const int MINIMUM_PLANE_POINTS = 3;
                return (IsManuallyEntered && ManuallyEnteredPanelsCount > 0) || (!IsManuallyEntered && Points != null && Points.Count >= MINIMUM_PLANE_POINTS);
            }
        }

        #region Navigation

        [DataMember]
        [ForeignKey("SolarSystemID")]
        [InverseProperty("RoofPlanes")]
        public SolarSystem SolarSystem { get; set; }

        [DataMember]
        [InverseProperty("RoofPlane")]
        [AttachOnUpdate]
        public ICollection<RoofPlanePoint> Points { get; set; }

        [DataMember]
        [InverseProperty("RoofPlane")]
        [AttachOnUpdate]
        public ICollection<RoofPlaneEdge> Edges { get; set; }

        [DataMember]
        [InverseProperty("RoofPlane")]
        [AttachOnUpdate]
        public ICollection<RoofPlanePanel> Panels { get; set; }

        [DataMember]
        [InverseProperty("RoofPlane")]
        [AttachOnUpdate]
        public ICollection<RoofPlaneObstruction> Obstructions { get; set; }

        [ForeignKey("SolarPanelID")]
        [DataMember]
        public SolarPanel SolarPanel { get; set; }

        [ForeignKey("InverterID")]
        [DataMember]
        public Inverter Inverter { get; set; }

        #endregion

        /// <summary>
        /// Sort of hash used to compare two RoofPlanes
        /// </summary>
        /// <returns></returns>
        public string PseudoHash()
        {
            return $"{Azimuth}-{CenterX}-{CenterY}-{CenterLatitude}-{CenterLongitude}-{IsManuallyEntered}-{ManuallyEnteredPanelsCount}-{ModuleSpacing}-{Pitch}-{Racking}-{RowSpacing}-{Shading}-{Tilt}-{PanelsCount}";
        }

        public int Production()
        {
            if (SolarPanel == null)
            {
                return 0;
            }
            return SolarPanel.Watts * PanelsCount;
        }


        public RoofPlane Clone(Guid solarSystemID, CloneSettings cloneSettings)
        {
            if (this.Panels == null) throw new MissingMemberException("Missing RoofPlane.Panels in the Object Graph");
            if (this.Points == null) throw new MissingMemberException("Missing RoofPlane.Points in the Object Graph");
            if (this.Obstructions == null) throw new MissingMemberException("Missing RoofPlane.Obstructions in the Object Graph");
            if (this.Edges == null) throw new MissingMemberException("Missing RoofPlane.Edges in the Object Graph");

            RoofPlane ret = (RoofPlane)this.MemberwiseClone();
            ret.Reset();
            ret.SolarSystem = null;
            ret.SolarPanel = null;
            ret.Inverter = null;
            ret.SolarSystemID = solarSystemID;

            ret.Edges = new List<RoofPlaneEdge>();
            foreach (var edge in this.Edges)
            {
                ret.Edges.Add(edge.Clone(this.Guid));
            }

            ret.Obstructions = new List<RoofPlaneObstruction>();
            foreach (var obs in this.Obstructions)
            {
                ret.Obstructions.Add(obs.Clone(this.Guid));
            }

            ret.Panels = new List<RoofPlanePanel>();
            foreach (var panel in this.Panels)
            {
                ret.Panels.Add(panel.Clone(this.Guid));
            }

            ret.Points = new List<RoofPlanePoint>();
            foreach (var point in this.Points)
            {
                ret.Points.Add(point.Clone(this.Guid));
            }



            return ret;
        }

    }
}
