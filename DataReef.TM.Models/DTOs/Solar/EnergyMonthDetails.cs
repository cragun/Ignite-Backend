using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar
{
    /// <summary>
    /// Consumption, production and cost details for a month.
    /// </summary>
    public class EnergyMonthDetails
    {
        /// <summary>
        /// 1 to x 
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 1 to 12
        /// 1 = January
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// In kWh
        /// </summary>
        public decimal Production { get; set; }

        /// <summary>
        /// Customer's Current Consumption In kWh
        /// </summary>
        public decimal Consumption { get; set; }

        /// <summary>
        /// Consumption after applying adders that reduce the consumption (In kWh)
        /// </summary>
        public decimal PostAddersConsumption { get; set; }


        public decimal PostAddersConsumptionOrConsumption => PostAddersConsumption != 0 ? PostAddersConsumption : Consumption;


        /// <summary>
        /// Consumption after adders and solar production (In kWh)
        /// </summary>
        public decimal PostSolarConsumption { get; set; }

        public decimal PostSolarConsumptionOrConsumption => PostSolarConsumption != 0 ? PostSolarConsumption : PostAddersConsumptionOrConsumption;

        public decimal PricePerKWH { get; set; }


        /// <summary>
        /// In $
        /// </summary>
        public decimal PreSolarCost { get; set; }

        /// <summary>
        /// In $
        /// </summary>
        public decimal PostSolarCost { get; set; }

        public void SetPostAddersConsumption(decimal percentage)
        {
            PostAddersConsumption = Consumption - (Consumption * (percentage / 100));
        }

        public EnergyMonthDetails Clone(decimal percentage = 0)
        {
            var ret = MemberwiseClone() as EnergyMonthDetails;
            ret.SetPostAddersConsumption(percentage);
            return ret;
        }


    }
}
