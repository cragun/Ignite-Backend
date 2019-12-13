using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class MortgageDetails
    {
        /// <summary>
        /// Date of the Ipoteca (Hipoteca for us Spanish Awesome speaking people )
        /// </summary>
        public System.DateTime Date { get; set; }


        /// <summary>
        /// Rate.  Annual Percentage Rate
        /// </summary>
        public double Apr { get; set; }

        /// <summary>
        /// Amount of the mortgage
        /// </summary>
        public decimal OriginalBalance { get; set; }


        /// <summary>
        /// The current balance of the mortage.  Calculated
        /// </summary>
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// Term in Months. Length of the loan
        /// </summary>
        public int Term { get; set; }
    }
}
