using DataReef.TM.Models.DTOs.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    /// <summary>
    /// Class that encapsulates the information needed for PPA creation.
    /// </summary>
    public class PowerPurchaseAgreementRequest
    {
        /// <summary>
        /// The state.
        /// </summary>
        public string Market                        { get; set; }

        /// <summary>
        /// The name of the power company.. this is the name of the LSE as it comes from Genability
        /// </summary>
        public string UtilityName                   { get; set; }

        /// <summary>
        /// The Genability lseId of the powercompany
        /// </summary>
        public string UtilityID                     { get; set; }

        /// <summary>
        /// Annual production in kWh.
        /// </summary>
        public int AnnualProduction                 { get; set; }

        /// <summary>
        /// System Size in W.
        /// </summary>
        public double SystemSize                    { get; set; }

        /// <summary>
        /// Annual electicity price.
        /// </summary>
        public decimal AnnualElectricityBill        { get; set; }

        public double AnnualOutputDegradation       { get; set; }

        /// <summary>
        /// Escalation rate for charge per year.
        /// </summary>
        public decimal EscalationRate               { get; set; }

        /// <summary>
        /// The price per kWh from the electric company.
        /// </summary>
        public decimal UtilityPricePerKWH           { get; set; }

        /// <summary>
        /// Production and  Consumption in a period... 12 months will be sent in.
        /// </summary>
        public MonthlyPower[] Consumption           { get; set; }

        /// <summary>
        /// The solar usage profile name.
        /// </summary>
        public string SolarProfileName              { get; set; }

        /// <summary>
        /// The ID of the electricity usage profile.
        /// </summary>
        public string ElectricityProviderProfileID  { get; set; }

        /// <summary>
        /// The IDs of the PVWatts 5 Usage Profiles. Each profile is built on a {SystemSize, Azimuth, Tilt} set.
        /// </summary>
        public List<string> SolarProviderProfileIDs { get; set; }

        /// <summary>
        /// If set to true, the solar rate will be increased by an amount of 0.02$.
        /// </summary>
        public bool? IsPricingB                     { get; set; }
    }
}
