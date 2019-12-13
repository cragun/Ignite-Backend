using System;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class TerritoryDataView
    {
        public TerritoryDataView(Territory territory)
        {
            TerritoryID = territory.Guid;
            Name = territory.Name;
            PropertyCount = territory.PropertyCount;
            CompletedCount = territory.Summary?.CompletedCount ?? 0;
            PositivePrescreenNotContactedCount = territory.Summary?.PositivePrescreenNotContactedCount ?? 0;
            SaleCount = territory.Summary?.SaleCount ?? 0;
            PrescreenPassCount = territory.Summary?.PrescreenPassCount ?? 0;
            AssignmentCount = territory.Summary?.AssignmentCount ?? 0;
            PercentCompleted = territory.Summary?.PercentCompleted ?? 0;
        }

        public TerritoryDataView()
        {
        }

        public Guid TerritoryID { get; set; }

        public string Name { get; set; }

        public int? PropertyCount { get; set; }

        public int CompletedCount { get; set; }

        public int PositivePrescreenNotContactedCount { get; set; }

        public int SaleCount { get; set; }

        public int PrescreenPassCount { get; set; }

        public int AssignmentCount { get; set; }

        public double PercentCompleted { get; set; }
    }
}
