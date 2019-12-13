using DataReef.TM.Models.DataViews.SelfTrackedKPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Reports
{
    public class OrganizationSelfTrackedReportRow
    {
        public Guid Id { get; set; }
        public string OfficeName { get; set; }
        public ICollection<SelfTrackedStatistics> SelfTrackedStatistics { get; set; }
    }
}
