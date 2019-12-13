using System;

namespace DataReef.TM.Models.DTOs.FinanceAdapters.SMARTBoard
{
    public class SBUsageAttribute : Attribute
    {
        public int Month { get; set; }
        public SBUsageType UsageType { get; set; }
    }

    public enum SBUsageType
    {
        Consumption,
        Bill
    }

}
