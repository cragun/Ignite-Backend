using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Microsoft.PowerBI.Models
{
    public class PBI_ProposalCreated : PBI_Base
    {
        public Guid SalesRepID { get; set; }
        public Guid PropertyID { get; set; }
        public Guid TerritoryID { get; set; }
        public Guid OUID { get; set; }
        public Guid? FinancePlanDefinitionID { get; set; }
        public Guid ProposalID { get; set; }
        public string FinancePlanDefinitionName { get; set; }
        public string FinanceProvider { get; set; }
        public int FinanceTermInYears { get; set; }
        public string ContractorID { get; set; }
        public string State { get; set; }

        public string FinancingType { get; set; }
        public string SystemDrawingType { get; set; }
        public int RoofPlanesCount { get; set; }

        public double SystemSize { get; set; }
        public double FinancingAmount { get; set; }
        public double TotalSavings { get; set; }
        public int PanelsCount { get; set; }
        public string Equipment { get; set; }
    }
}
