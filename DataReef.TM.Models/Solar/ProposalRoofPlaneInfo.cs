using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Solar
{
    [Table("ProposalRoofPlaneInfo", Schema = "solar")]
    public class ProposalRoofPlaneInfo : EntityBase
    {
        [DataMember]
        public string ArrayName { get; set; }
        [DataMember]
        /// <summary>
        /// Size in Watts
        /// </summary>
        public decimal Size { get; set; }

        [DataMember]
        /// <summary>
        /// TargetOffset in percentage
        /// </summary>
        public int TargetOffset { get; set; }

        [DataMember]
        /// <summary>
        /// ArrayOffset in percentage
        /// </summary>
        public int ArrayOffset { get; set; }

        [DataMember]
        public int Tilt { get; set; }

        [DataMember]
        public int Azimuth { get; set; }

        [DataMember]
        /// <summary>
        /// Genability PVWatts5 Usage Profile name.
        /// </summary>
        public string ProfileName { get; set; }

        [DataMember]
        /// <summary>
        /// Genability PVWatts5 Usage Profile ID.
        /// </summary>
        public string ProviderProfileId { get; set; }

        private double _shading;

        [DataMember]
        public double Losses
        {
            get
            {
                return _shading;
            }
            set
            {
                _shading = value;
            }
        }

        [DataMember]
        public double Shading
        {
            get
            {
                return _shading;
            }
            set
            {
                _shading = value;
            }
        }

        [DataMember]
        public decimal InverterEfficiency { get; set; }

        [DataMember]

        public double PanelsEfficiency { get; set; }

        [DataMember]
        /// <summary>
        /// The type of module to use. 0 = Standard, 1 = Premium, 2 = Thin Film.
        /// This property is dynamically calculated based on <see cref="InverterEfficiency"/>
        /// Specs: 
        /// If efficiency is closer to 15% use 0, 
        /// If efficiency is closer to 19% use 1,
        /// </summary>
        public int ModuleType
        {
            get
            {
                if (PanelsEfficiency < 16)
                {
                    return 0;
                }
                if (PanelsEfficiency < 20)
                {
                    return 1;
                }
                return 2;
            }
        }

        [DataMember]
        public Guid ProposalId { get; set; }

        #region Navigation

        [ForeignKey("ProposalId")]
        [DataMember]
        public Proposal proposal { get; set; }

        #endregion Navigation
    }
}
