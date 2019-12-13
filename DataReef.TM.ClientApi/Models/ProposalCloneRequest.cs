using DataReef.TM.Models.DataViews.ClientAPI;
using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.ClientApi.Models
{

    public class RoofPlaneCloneDataView
    {
        /// <summary>
        /// Guid of the existing roof plane
        /// </summary>
        public Guid RoofPlaneID { get; set; }


        /// <summary>
        /// New pitch for Roof Plane.  A missing value will not be modified
        /// </summary>
        public int? Pitch { get; set; }

        /// <summary>
        /// New Tilt for Roof Plane.  A missing value will not be modified
        /// </summary>
        public double? Tilt { get; set; }

        /// <summary>
        /// New shading value for roof plane.  A missing value will not be modified
        /// </summary>
        public int? Shading { get; set; }

        /// <summary>
        /// New Azimuth value for roof plane.  A missing value will not be modified
        /// </summary>
        public int? Azimuth { get; set; }
    }

    /// <summary>
    /// Solar system billing and consumption information for a month
    /// </summary>
    public class SystemMonthCloneDataView
    {
        public decimal Consumption
        {
            get { return Watts; }
            set { Watts = value; }
        }

        public decimal PreSolarCost
        {
            get { return Cost; }
            set { Cost = value; }
        }

        //public float PostSolarCost { get; set; }
        public decimal Watts { get; set; }

        public decimal Cost { get; set; }

        public Int16 Month { get; set; }

        public Int16 Year { get; set; }

        public PowerInformationSource? CostSource { get; set; }

        public PowerInformationSource? UsageSource { get; set; }
    }

    public class ProposalCloneRequest
    {

        public ProposalCloneRequest()
        {
            this.Adders = new Models.AdderCrud();
        }


        /// <summary>
        /// Guid of the existing proposal to clone
        /// </summary>
        public Guid ProposalID { get; set; }

        /// <summary>
        /// Name of the new Proposal
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Delimited string of Tags ( comma or semi colon ) will replace if exist
        /// </summary>
        public List<String> Tags { get; set; }


        public AdderCrud Adders { get; set; }

        public KeyValuesCrud KeyValues { get; set; }

        /// <summary>
        /// Roof planes to modify.  Guid must match existing roof plane.  An missing roof plane will not be modified
        /// </summary>
        public ICollection<RoofPlaneCloneDataView> RoofPlanes { get; set; }

        /// <summary>
        /// Array of Production kWh and Price per Watt pair.
        /// </summary>
        public List<ProductionPrice> ProductionCosts { get; set; }

        /// <summary>
        /// Override monthly billing and usage data.
        /// </summary>
        public ICollection<SystemMonthCloneDataView> SystemMonths { get; set; }
    }
}