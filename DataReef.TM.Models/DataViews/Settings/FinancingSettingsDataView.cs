using DataReef.TM.Models.DataViews.Financing;
using System;

namespace DataReef.TM.Models.DataViews.Settings
{
    public class FinancingSettingsDataView
    {
        public FinancingSettingsDataView() { }

        public FinancingSettingsDataView(Guid planId, bool isEnabled, int order, FinancePlanDataModel data)
        {
            PlanID = planId;
            IsEnabled = isEnabled ? "1" : "0";
            Order = $"{order}";
            Data = data;
        }

        public Guid PlanID { get; set; }
        public string IsEnabled { get; set; }
        public string Order { get; set; }
        public string DealerFee { get; set; }

        /// <summary>
        /// Custom data, mostly regarding 3rd party integrations, payment factors, etc.
        /// </summary>
        public FinancePlanDataModel Data { get; set; }


        public bool GetIsEnabled()
        {
            return IsEnabled == "1";
        }

        public int GetOrder()
        {
            int ret = 0;
            int.TryParse(Order, out ret);
            return ret;
        }

        public void SetOrder(int order)
        {
            Order = $"{order}";
        }

        public void IncreaseOrder(int offset)
        {
            if (offset == 0)
            {
                return;
            }
            SetOrder(GetOrder() + offset);
        }

        public double? GetDealerFee()
        {
            return string.IsNullOrWhiteSpace(DealerFee) ? (double?)null : double.Parse(DealerFee.TrimEnd('%'));
        }

        public void SetDealerFee(double fee)
        {
            DealerFee = $"{fee}";
        }

    }
}
