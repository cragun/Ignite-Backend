using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability.Enums
{
    public enum SavingAnalysisPropertyInputs
    {
        /// <summary>
        /// The ID of the usage profile that you want to use for this scenario. Can be set for before, solar, and after. If nothing is set for before or after, the default behavior is to use the account's default profile. You can add more than one profile per scenario and combine them using the operator property on this input. The operator can be + or -, which indicates whether you want to add or subtract the indicated profile's values from the other ones for that scenario.
        /// SCENARIOS: before, after, solar
        /// </summary>
        profileId,

        /// <summary>
        /// The alternative ID for the profile that you want to add to the analysis.
        /// SCENARIOS: before, after, solar
        /// </summary>
        providerProfileId,

        /// <summary>
        /// If you want to use a typical baseline for the usage in a particular scenario, set a data value of TYPICAL for this property. For solar, use SOLAR_PV. You can use this if you don't have usage or solar data for the account and you just want to do a quick analysis.
        /// SCENARIOS: before, after, solar
        /// </summary>
        baselineType,

        /// <summary>
        /// The tariff that will be used for this scenario. If this is not set, it defaults to whatever is currently set as the masterTariffId on the account.
        /// SCENARIOS: before, after
        /// </summary>
        masterTariffId,

        /// <summary>
        /// The rate at which the cost of energy raises for every year of the analysis. Use a dataValue of 3.5 to use 3.5%, for example.
        /// SCENARIOS: before, after, solar
        /// </summary>
        rateInflation,

        /// <summary>
        /// The rate at which energy production from the solar system degrades. If your degradation rate is 0.5% per year, use a value of 0.5 here.
        /// SCENARIOS: solar, after
        /// </summary>
        solarDegradation,

        /// <summary>
        /// You can make the project last however long you want. The default is 20 years.
        /// SCENARIOS: before, after, solar
        /// </summary>
        projectDuration,

        /// <summary>
        /// If you don't have an existing solar profile, you can use solarPvLoadOffset to specify a percentage of the customer's existing load to offset with a PV system. The system will be a south facing and tilted at 30 degrees. By default (if you just set baselineType to SOLAR_PV for the solar scenario), this will be set to 80%.
        /// SCENARIOS: solar, after
        /// </summary>
        solarPvLoadOffset,

        /// <summary>
        /// Alternative name for solarPvLoadOffset.
        /// SCENARIOS: solar, after
        /// </summary>
        loadOffset,

        /// <summary>
        /// If you don't have an existing usage profile, use this option in conjunction with a baselineType of TYPICAL to size your customer's annual usage. It will use a typical profile from our database and scale it up or down to the annual value that you specify.
        /// SCENARIOS: before, after
        /// </summary>
        loadSize

    }
}
