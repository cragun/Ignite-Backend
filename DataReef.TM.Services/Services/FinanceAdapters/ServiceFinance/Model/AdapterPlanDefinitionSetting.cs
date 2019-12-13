using System.Collections.Generic;

namespace DataReef.TM.Services.Services.FinanceAdapters.ServiceFinance.Model
{
    public class AdapterPlanDefinitionSetting
    {
        public string AdapterName { get; set; }

        public List<AdapterPlanDefinitionSettingItem> Settings { get; set; }
    }

    public class AdapterPlanDefinitionSettingItem
    {
        public string ProgramType { get; set; }
    }
}
