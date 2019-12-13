using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("Panels", Schema = "solar")]
    public class SolarPanel:EntityBase
    {

        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// very important field used to calculate the Watts or KW in a system
        /// </summary>
        [DataMember]
        public int Watts { get; set; }

        /// <summary>
        /// used for layout in proposal drawing.  Determines how to render the panel (in mm)
        /// </summary>
        [DataMember]
        public int Width { get; set; }

        /// <summary>
        /// used for layout in proposal drawing.  Determines how to render the panel (in mm)
        /// </summary>
        [DataMember]
        public int Height { get; set; }

        /// <summary>
        /// Thickness/depth (in mm)
        /// </summary>
        [DataMember]
        public double Thickness { get; set; }

        /// <summary>
        /// Weight in kg
        /// </summary>
        [DataMember]
        public double Weight { get; set; }

        [DataMember]
        public int NumberOfCells { get; set; }

        /// <summary>
        /// The type of module to use. 0 = Standard, 1 = Premium, 2 = Thin Film.
        /// </summary>
        [DataMember]
        public int ModuleType { get; set; }


        [DataMember]
        public double Efficiency { get; set; }


        /// <summary>
        /// PV or Thermal.  This is important for the Front End to automatically sync Adders
        /// </summary>
        [DataMember]
        [StringLength(50)]
        public string SolarType { get; set; }


        /// <summary>
        /// HTML Color for rendering in UI
        /// </summary>
        [DataMember]
        [StringLength(50)]
        public string PanelColor { get; set; }


        #region Navigation




        #endregion

    }
}
