using System;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Solar.Proposal
{
    public class SolarProposalResponse
    {

        public SolarProposalResponse()
        {
            Months = new List<EnergyMonthDetails>();
        }

        public Guid ProposalID { get; set; }

        public ICollection<EnergyMonthDetails> Months { get; set; }


        /// <summary>
        /// Watts projected that solar system will provide (Total for all months)
        /// </summary>
        public decimal Consumption { get; set; }

        /// <summary>
        /// Total consumption after calculating adders reduction
        /// </summary>
        public decimal PostAddersConsumption { get; set; }

        /// <summary>
        /// Total consumption after calculating adders reduction and subtracting production
        /// </summary>
        public decimal PostSolarConsumption { get; set; }

        /// <summary>
        /// Watts projected that solar system will provide  (Total for all months)
        /// </summary>
        public decimal Production { get; set; }

        /// <summary>
        /// energy (electric company) cost before solar  (Total for all months)
        /// </summary>
        public decimal PreSolarCost { get; set; }

        /// <summary>
        /// electric company cost after solar  (Total for all months)
        /// </summary>
        public decimal PostSolarCost { get; set; }

    }
}
