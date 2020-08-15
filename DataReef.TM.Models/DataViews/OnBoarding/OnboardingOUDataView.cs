using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.OnBoarding
{
    public class OnboardingOUDataView
    {
        public Guid ParentID { get; set; }
        public Guid ProposalTemplateID { get; set; }

        public string FireOffsetsData { get; set; }

        public string AvailableDispositionsData { get; set; }

        public List<OUSettingDataView> Settings { get; set; }

        public NewOUBasicInfoDataView BasicInfo { get; set; }
        public NewOUEquipmentDataView Equipment { get; set; }

        public NewOUFinancingDataView Financing { get; set; }
        public NewOUPermissionsDataView Permissions { get; set; }
        public NewOURoofDetailsDataView RoofDetails { get; set; }

        //public List<Guid> CashAndMortgageIDs { get; set; }

        public void Validate(bool isNew)
        {
            if (isNew && ParentID == Guid.Empty)
            {
                throw new ApplicationException("Invalid request. No ParentID!");
            }
            if (isNew && string.IsNullOrWhiteSpace(BasicInfo?.OUName))
            {
                throw new ApplicationException("Invalid request. No OU Name!");
            }
            //if (isNew && string.IsNullOrWhiteSpace(BasicInfo?.OwnerFirstName))
            //{
            //    throw new ApplicationException("Invalid request. No Owner First Name!");
            //}
            if (isNew && string.IsNullOrWhiteSpace(BasicInfo?.OwnerLastName))
            {
                throw new ApplicationException("Invalid request. No Owner Last Name!");
            }
            if (BasicInfo == null
                || BasicInfo.States == null
                || BasicInfo?.States?.Count == 0)
            {
                throw new ApplicationException("Invalid request. No States!");
            }

            //if (Financing?.ProposalFlowType != FinanceProviderProposalFlowType.None
            //    && string.IsNullOrWhiteSpace(Financing?.ProposalFlowData))
            //{
            //    throw new ApplicationException("Proposal Flow data is missing!");
            //}

            //if (string.IsNullOrWhiteSpace(FireOffsetsData))
            //{
            //    throw new ApplicationException("Invalid request. No DefaultOffsets!");
            //}
            //if (Financing == null
            //    || Financing.SelectedFinancePlans == null
            //    || Financing.SelectedFinancePlans.Count == 0)
            //{
            //    throw new ApplicationException("Invalid request. No Finance Plans!");
            //}
            //if (Equipment == null)
            //{
            //    throw new ApplicationException("Invalid request. No Equipment data!");
            //}
            //if (Equipment.SelectedPanels == null
            //    || Equipment.SelectedPanels.Count == 0)
            //{
            //    throw new ApplicationException("Invalid request. No Solar Panels!");
            //}
            //if (Equipment.SelectedInverters == null ||
            //    Equipment.SelectedInverters.Count == 0)
            //{
            //    throw new ApplicationException("Invalid request. No Inverters!");
            //}
            //if (Equipment.DefaultPanel == null || Equipment.DefaultPanel == Guid.Empty)
            //{
            //    throw new ApplicationException("Invalid request. No default panel!");
            //}
        }
    }

    public abstract class NewOUBaseDataView
    {
        public bool InheritFromParent { get; set; }
    }

    public class NewOUBasicInfoDataView : NewOUBaseDataView
    {
        public string OUName { get; set; }

        public List<string> States { get; set; }

        public string OwnerFirstName { get; set; }

        public string OwnerLastName { get; set; }

        public string OwnerEmail { get; set; }

        public bool IsSolarTenant { get; set; }

        public string LogoImage { get; set; }
        public bool UseLogoOnProposal { get; set; }
        public bool IsTerritoryAdd { get; set; }
        public int MinModule { get; set; }
    }

    public class NewOUEquipmentDataView : NewOUBaseDataView
    {
        public List<Guid> SelectedPanels { get; set; }

        public List<Guid> SelectedInverters { get; set; }
        public Guid DefaultPanel { get; set; }

        /// <summary>
        /// It will be a JSON (array of <see cref="OrgSettingDataView"/>)
        /// </summary>
        public string DefaultLosses { get; set; }

        public double DefaultUtilityInflationRate { get; set; }

        public double DefaultSystemDegradation { get; set; }
    }

    public class NewOUFinancingDataView : NewOUBaseDataView
    {
        public List<FinancingSettingsDataView> SelectedFinancePlans { get; set; }
        public FinanceProviderProposalFlowType ProposalFlowType { get; set; }
        public string ProposalFlowData { get; set; }
    }

    public class NewOUPermissionsDataView : NewOUBaseDataView
    {
        public bool EnablePreQualfication { get; set; }
        public bool EnableProposals { get; set; }
        public bool EnableFinancingReorder { get; set; }
    }

    public class NewOURoofDetailsDataView : NewOUBaseDataView
    {
        public int DefaultRoofTilt { get; set; }
    }

    public class OUSettingDataView
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public OUSettingGroupType Group { get; set; }
        public SettingValueType ValueType { get; set; }
        public bool IsDeleted { get; set; }

    }

}

