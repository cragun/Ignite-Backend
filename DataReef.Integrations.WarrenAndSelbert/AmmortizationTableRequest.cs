using System;
using System.Xml.Linq;

namespace DataReef.Integrations.WarrenAndSelbert
{
    public class AmmortizationTableRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public decimal AppraisedValuePerWatt { get; set; }
        public decimal PPARate { get; set; }
        public decimal SystemSizeInWatts { get; set; }
        public decimal PVWithPanel { get; set; }
        public decimal Year1Production { get; set; }
        public decimal CustomerPrepayment { get; set; }
        public decimal UpfrontRebateAssumptions { get; set; }
        public decimal UpfrontRebateAssumptionsMax { get; set; }
        public string State { get; set; }
        public decimal UtilityIndex { get; set; }
        public string ChannelType { get; set; }
        public int LastYear { get; set; }
        public decimal PeriodicRentEscalation { get; set; }
        public decimal PBIFraction { get; set; }
        public int PBIYears { get; set; }
        public decimal PBIAnnualDerate { get; set; }
        public DateTime SubstantialCompletionDate { get; set; }
        public decimal StateSales { get; set; }
        public decimal CurrentUtilityCost { get; set; }
        public decimal PostSolarUtilityCost { get; set; }
        public string CustomerUniqueID { get; set; }
        public string ProposalID { get; set; }

        public string ToXml()
        {
            XElement rootNode = new XElement("abc", new XAttribute("xmlns", "http://www.warren-selbert.com"), new XAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance"));
            XDocument xml = new XDocument(rootNode);
            rootNode.Add(new XElement("username", UserName));
            rootNode.Add(new XElement("password", Password));
            rootNode.Add(new XElement("version", "2"));
            rootNode.Add(new XElement("stoponerror", "0"));
            XElement batchNode = new XElement("batch", new XAttribute("id", "1"));
            rootNode.Add(batchNode);
            batchNode.Add(new XElement("filename", "ppa_model"));
            XElement inputCommandNode = new XElement("inputcommand", new XAttribute("name", "APIInput"));
            batchNode.Add(inputCommandNode);
            inputCommandNode.Add(SetValue("Appraised Value per Watt", AppraisedValuePerWatt));
            inputCommandNode.Add(SetValue("PPA Rate", PPARate));
            inputCommandNode.Add(SetValue("System Size (W)", PPARate));
            inputCommandNode.Add(SetValue("PV W/Panel", PVWithPanel));
	        inputCommandNode.Add(SetValue("Year 1 Production", Year1Production));
            inputCommandNode.Add(SetValue("Customer Prepayment", CustomerPrepayment));
            inputCommandNode.Add(SetValue("Upfront Rebate Assumptions ($/W) DC", UpfrontRebateAssumptions));
            inputCommandNode.Add(SetValue("Upfront Rebate Assumptions-Max", UpfrontRebateAssumptionsMax));
            inputCommandNode.Add(SetValue("State", State));
            inputCommandNode.Add(SetValue("Utility Index (Genability utility ID)", UtilityIndex));
            inputCommandNode.Add(SetValue("Channel Type", ChannelType));
            inputCommandNode.Add(SetValue("Last Year (at 0.7%)", LastYear));
            inputCommandNode.Add(SetValue("Periodic Rent Escalation", PeriodicRentEscalation + "%"));
            inputCommandNode.Add(SetValue("PBI Fraction", PBIFraction));
            inputCommandNode.Add(SetValue("PBI Years", PBIYears));
            inputCommandNode.Add(SetValue("PBI Annual Derate", PBIAnnualDerate + "%"));
            inputCommandNode.Add(SetValue("Substantial Completion Date", SubstantialCompletionDate));
            inputCommandNode.Add(SetValue("State Sales/Use Tax Rate", StateSales + "%"));
            inputCommandNode.Add(SetValue("Current Utility Cost", CurrentUtilityCost));
            inputCommandNode.Add(SetValue("Post-Solar Utility Cost", PostSolarUtilityCost));
            inputCommandNode.Add(SetValue("Customer unique ID", CustomerUniqueID));
            inputCommandNode.Add(SetValue("Proposal ID", ProposalID));
            inputCommandNode.Add(SetValue("ModSolar call version ID", "1.00.01"));
            inputCommandNode.Add(SetValue("IP address", "173.194.39.40"));
            inputCommandNode.Add(SetValue("Authorization code", "MODSOLAR-021513"));
            inputCommandNode.Add(SetValue("APIBatchID", "1"));

            XElement outputCommandNode = new XElement("outputcommand", new object[] { new XAttribute("name", "APIOutput"), new XElement("outputoption", "")});
            batchNode.Add(outputCommandNode);

            return xml.ToString();
        }

        private XElement SetValue(string name, object value)
        {
            var input = new XElement("input", new XAttribute("name", name));
            input.Value = value.ToString();
            return input;
        }
    }
}
