namespace DataReef.TM.Models.DataViews.Financing
{
    public class FinancePlanDataModel
    {
        public FinancePlanIntegrationsModel Integrations { get; set; }

        /// <summary>
        /// SmartBOARD meta data.
        /// </summary>
        public FinancePlanSmartBOARDMeta SBMeta { get; set; }
    }
}
