using DataReef.TM.Models.DataViews.Inquiries;
using System;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Reports
{
    public class OrganizationReportRow
    {
        public Guid Id { get; set; }
        public string OfficeName { get; set; }
        public int TotalReps { get; set; }
        public InquiryStatisticsByDate WorkingReps { get; set; }
        public ICollection<InquiryStatisticsForOrganization> InquiryStatistics { get; set; }
    }
}

