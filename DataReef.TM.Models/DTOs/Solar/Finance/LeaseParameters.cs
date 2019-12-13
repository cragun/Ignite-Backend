using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class LeaseParameters
    {
        private decimal _pricePerKWh;

        /// <summary>
        /// Lease price per kWh
        /// </summary>
        public decimal LeasePricePerkWh
        {
            get
            {
                return _pricePerKWh;
            }
            set
            {
                _pricePerKWh = value;
            }
        }

        /// <summary>
        /// Add this property as a little hack because the iPad app uses 2 properties in different scenarios
        /// </summary>
        public decimal PricePerkWh
        {
            get
            {
                return _pricePerKWh;
            }
            set
            {
                _pricePerKWh = value;
            }
        }

        /// <summary>
        /// Annual escalator percentage
        /// </summary>
        public decimal Escalator { get; set; }

        /// <summary>
        /// Monthly lease tax percentage. Will be added to monthly lease payment.
        /// </summary>
        public decimal MonthlyLeaseTax { get; set; }

        /// <summary>
        /// Production increase percentage
        /// </summary>
        public decimal ProductionIncrease { get; set; }

    }
}
