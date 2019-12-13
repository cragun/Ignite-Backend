using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// The summary for an account analysis.
    /// </summary>
    public class AccountAnalysisSummary
    {
            public decimal? preTotalCost                  { get; set; }

            public decimal? preTotalKWh                   { get; set; }

            public decimal? netAvoidedCost                { get; set; }

            public decimal? postTotalCost                 { get; set; }

            public decimal? lifetimeSolarCost             { get; set; }

            public decimal? lifeTimeUtilityAvoidedRate    { get; set; }

            public decimal? lifetimeWithoutCost           { get; set; }

            public decimal? postTotalMinCost              { get; set; }

            public decimal? netAvoidedCostPctOffset       { get; set; }

            public decimal? netAvoidedKWh                 { get; set; }

            public decimal? netAvoidedKWhPctOffset        { get; set; }

            public decimal? postTotalKWh                  { get; set; }

            public decimal? preTotalRate                  { get; set; }

            public decimal? postTotalNonMinCost           { get; set; }

            public decimal? lifetimeAvoidedCost           { get; set; }

            public decimal? postTotalRate                 { get; set; }

            public decimal? netAvoidedRate                { get; set; }

            public decimal? lifeTimeUtilityAfterCost      { get; set; }

            public decimal? preTotalNonMinCost            { get; set; }

            public decimal? preTotalMinCost               { get; set; }
    }
}
