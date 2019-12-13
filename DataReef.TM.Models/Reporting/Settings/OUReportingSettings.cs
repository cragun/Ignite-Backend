using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Reporting.Settings
{
    public class OUReportingSettings
    {
        public IEnumerable<string> WorkingRepsDispositions { get; set; }

        public IEnumerable<OUReportingSettingsItem> ReportItems { get; set; }

        public IEnumerable<PersonReportingSettingsItem> PersonReportItems { get; set; }


    }

    public class OUReportingSettingsItem
    {
        public string ColumnName { get; set; }

        public IEnumerable<string> IncludedDispositions { get; set; }

        public IEnumerable<ConditionalReportingDisposition> ConditionalIncludedDispositions { get; set; }

        public bool CalculatePerRep { get; set; }
    }

    public class PersonReportingSettingsItem
    {
        public string ColumnName { get; set; }

        public IEnumerable<string> IncludedDispositions { get; set; }

        public IEnumerable<ConditionalReportingDisposition> ConditionalIncludedDispositions { get; set; }

        public IEnumerable<string> IncludedPersonKpis { get; set; }
    }

    public class ConditionalReportingDisposition
    {
        public IEnumerable<string> InitialDispositions { get; set; }

        public string FinalDisposition { get; set; }
    }
}
