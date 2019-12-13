using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.DataViews.SelfTrackedKPIs;
using System;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Reports
{
    public class SalesRepresentativeSelfTrackedReportRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<SelfTrackedStatistics> SelfTrackedStatistics { get; set; }
    }
}
