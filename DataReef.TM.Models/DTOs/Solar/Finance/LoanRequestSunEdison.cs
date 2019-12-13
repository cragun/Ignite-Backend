using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class LoanRequestSunEdison
    {

        public LoanRequestSunEdison()
        {
            this.System = new SolarSystemDefinition();
            this.Financing = new List<SolarFinancingDefinition>();
            this.Consumption = new List<MonthlyPower>();
            this.Months = 12 * 25;
            this.Incentives = new List<SolarIncentive>();
        }

        /// <summary>
        /// Unique identifier from the client to identify scenarios
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// the definition of the System
        /// </summary>
        public SolarSystemDefinition System { get; set; }

        public bool IncludeMonthsInResponse { get; set; }


        public ICollection<SolarIncentive> Incentives
        {
            get;
            set;
        }

        /// <summary>
        /// A collection of the types of financing that for this scenario
        /// </summary>
        public ICollection<SolarFinancingDefinition> Financing
        {
            get;
            set;
        }

        /// <summary>
        /// Jan - Dec of the last 12 months of power consumption in kWh
        /// </summary>
        public ICollection<MonthlyPower> Consumption
        {
            get;
            set;
        }

        /// <summary>
        /// Lenght of scenario analysis in months.  default is 300 (25 years).  Start Month is Month 1
        /// </summary>
        public int Months { get; set; }
    }
}
