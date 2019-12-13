using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes.Solar.Proposal
{
    /// <summary>
    /// The class that deals with retrieving the looked up charge value.
    /// </summary>
    public static class ChargeLookup
    {

        /// <summary>
        /// The list of look up elements.
        /// </summary>
        private static List<ChargeLookupItem> ChargeLookupList { get; set; }


        static ChargeLookup()
        {
            LoadLookupTable();
        }

        private static void LoadLookupTable()
        {
            ChargeLookupList = new List<ChargeLookupItem>();


            string[] ppaLines = System.IO.File.ReadAllLines(HttpContext.Current.Server.MapPath("~/ppa.txt"));

            foreach (string ppaLine in ppaLines)
            {
                string[] fields = ppaLine.Split(',');
                if (fields.Length == 5)
                {
                    ChargeLookupItem li = new ChargeLookupItem();
                    li.UtilityID = fields[0];
                    li.State = fields[1];
                    li.Charge = decimal.Parse(fields[2], CultureInfo.InvariantCulture);
                    decimal zeroEscalatorPrice = Decimal.Parse(fields[3], CultureInfo.InvariantCulture);
                    li.RequiredYield = int.Parse(fields[4], CultureInfo.InvariantCulture);
                    li.EscalationRate = decimal.Parse("2.9", CultureInfo.InvariantCulture);
                    
                    ChargeLookupList.Add(li);

                    if (zeroEscalatorPrice > 0)
                    {
                        li = new ChargeLookupItem();
                        li.UtilityID = fields[0];
                        li.State = fields[1];
                        li.Charge = decimal.Parse(fields[3], CultureInfo.InvariantCulture);
                        li.RequiredYield = int.Parse(fields[4], CultureInfo.InvariantCulture);
                        li.EscalationRate = decimal.Parse("0", CultureInfo.InvariantCulture);
                        
                        ChargeLookupList.Add(li);
                    }
                }

            }

        }
    

        /// <summary>
        /// Static method for retrieving charge value.
        /// </summary>
        /// <param name="market">The state.</param>
        /// <param name="escalationRate">The escalation rate.</param>
        /// <param name="yield">The yield value.</param>
        /// <returns>Returns the charge value.</returns>
        public static ChargeLookupItem LookupCharge(string utilityID,string state, decimal escalationRate, double yield)
        {
            ChargeLookupItem lookupItem = null;
       
            if (string.IsNullOrWhiteSpace(utilityID))
            {
                lookupItem = ChargeLookupList.FirstOrDefault(item => item.State.Equals(state) && item.EscalationRate == escalationRate);
            }
            else
            {
                lookupItem = ChargeLookupList.FirstOrDefault(item => item.State.Equals(state) && item.UtilityID.Equals(utilityID) && item.EscalationRate == escalationRate);
            }

            return lookupItem;
        }

    }


    /// <summary>
    /// Item of the lookup list.
    /// </summary>
    public class ChargeLookupItem
    {

        /// <summary>
        /// The Genability name of the Utility
        /// </summary>
        public string UtilityID { get; set; }

        /// <summary>
        /// The state for which the lookup is being made.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// The yield interval.
        /// </summary>
        public int RequiredYield { get; set; }

        /// <summary>
        /// The escalation rate.
        /// </summary>
        public decimal EscalationRate { get; set; }


        /// <summary>
        /// The charge value that is looked up.
        /// </summary>
        public decimal Charge { get; set; }
    }
}