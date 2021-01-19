using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    [DataContract]
    public enum OUSettingGroupType
    {
        [EnumMember]
        ConfigurationFile = 1,

        [EnumMember]
        DealerSettings = 2,

        [EnumMember]
        HiddenConfigs = 3
    }

    [DataContract]
    public class OUSetting : EntityBase
    {
        [DataMember]
        public Guid OUID { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public SettingValueType ValueType { get; set; }
        [DataMember]
        public OUSettingGroupType Group { get; set; }
        [DataMember]
        public bool Inheritable { get; set; }

        #region Navigation properties

        [DataMember]
        [ForeignKey("OUID")]
        public OU OU { get; set; }

        #endregion

        public void CopyFrom(OUSetting values)
        {
            Value = values.Value;
            Group = values.Group;
            ValueType = values.ValueType;
            Inheritable = values.Inheritable;
        }

        public T GetValue<T>()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(Value);
            }
            catch { }

            return default(T);
        }

        public object GetValue(Type type)
        {
            try
            {
                return JsonConvert.DeserializeObject(Value, type);
            }
            catch { }

            return null;
        }

        public static readonly string Epc_Statuses = "Epc.Statuses";
        public static readonly string Proposal_TemplateBaseUrl = "Proposal.Template.BaseUrl";
        public static readonly string Proposal_TemplateGenericUrl = "Proposal.Template.GenericUrl";
        public static readonly string Proposal_Features_Summary = "Proposal.Features.Summary";
        public static readonly string Proposal_Features_PostSignInternalPaths = "Proposal.Features.PostSign.InternalPaths";
        public static readonly string Proposal_Features_EmailsToCC = "Proposal.Features.EmailAddress.CC";
        public static readonly string Proposal_Features_SendEmailToSalesRepOnGenerate = "Proposal.Features.Generate.SendEmailToSalesRep";
        public static readonly string Proposal_Features_AttachPDFOnGenerate = "Proposal.Features.Generate.AttachPDFToEmail";
        public static readonly string Proposal_Features_SendEmailOnSign = "Proposal.Features.Sign.SendEmail";
        public static readonly string Proposal_Features_AttachPDFOnSign = "Proposal.Features.Sign.AttachPDFToEmail";
        public static readonly string Proposal_Features_SendEmailToCustomer_Disabled = "Proposal.Features.Sign.SendEmailToCustomer.Disabled";
        public static readonly string Proposal_GenericSettings = "Proposal.GenericSettings";

        public static readonly string Proposal_Agreement_Features_SendEmailOnSign = "Proposal.Agreements.Features.Sign.SendEmail";
        public static readonly string Proposal_Agreement_Features_AttachPDFOnSign = "Proposal.Agreements.Features.Sign.AttachPDFToEmail";
        public static readonly string Proposal_Agreement_Features_EmailsToCC = "Proposal.Agreements.Features.EmailAddress.CC";

        public static readonly string Financing_Options = "Financing.Options";
        public static readonly string Financing_PlansOrder = "Financing.PlansOrder";
        public static readonly string Financing_AllowReorder = "Financing.AllowReorder";

        public static readonly string Solar_Losses = "Losses";

        public static readonly string Solar_Losses_Soiling = "Solar.SystemLosses.Soiling";
        public static readonly string Solar_Losses_Snow = "Solar.SystemLosses.Snow";
        public static readonly string Solar_Losses_Mismatch = "Solar.SystemLosses.Mismatch";
        public static readonly string Solar_Losses_Wiring = "Solar.SystemLosses.Wiring";
        public static readonly string Solar_Losses_Connections = "Solar.SystemLosses.Connections";
        public static readonly string Solar_Losses_LightInductedDegradation = "Solar.SystemLosses.LightInductedDegradation";
        public static readonly string Solar_Losses_NamePlateRating = "Solar.SystemLosses.NamePlateRating";
        public static readonly string Solar_Losses_Age = "Solar.SystemLosses.Age";
        public static readonly string Solar_Losses_Availability = "Solar.SystemLosses.Availability";

        public static readonly string Solar_Defaults_FireOffsets = "Solar.Defaults.FireOffsets";
        public static readonly string Solar_Defaults_Shading = "SolarDefaultShading";

        public static readonly string Solar_IsTenant = "Solar.IsTenant";
        public static readonly string Solar_ContractorID = "Contractor ID";
        public static readonly string Solar_Panels = "SolarPanels";
        public static readonly string NewSolar_Panels = "Solar.Panels";
        public static readonly string Solar_Inverters = "Inverters";
        public static readonly string NewSolar_Inverters = "Solar.Inverters";
        public static readonly string Solar_DefaultRoofTilt = "SolarDefaultRoofTilt";
        public static readonly string Solar_UtilityInflationRate = "SolarUtilityInflationRate";

        public static readonly string Solar_AvailableDispositions = "AvailableDispositions";
        public static readonly string NewDispositions = "Disposition.Options";
        public static readonly string Inquiry_Dispositions = "Inquiry.Dispositions";

        public static readonly string LegionOUFreeHiResImages = "Legion.OU.FreeHiResImages";
        public static readonly string LegionOULogoImageUrl = "Legion.OU.LogoImageUrl";
        public static readonly string LegionOUUseLogoInProposal = "Legion.OU.UseLogoInProposal";
        public static readonly string LegionOULeadSource = "Legion.OU.LeadSource";
        public static readonly string LegionOUPersonClockInfo = "Legion.OU.PersonClockInfo";


        public static readonly string Legion_EventMessageHandlers = "Legion.Internal.EventMessage.Handlers";

        public static readonly string Utility_Rates = "Solar.Energy.Utility.NewRates";

        public static readonly string Legion_Photos_Options = "Legion.Features.Property.Attachments.Options";
        public static readonly string Legion_Photos_Prefix = "Legion.Features.Property.Attachments";
        public static readonly string Legion_Photos_Suffix = "Definition";

        public static readonly string OU_Reporting_Settings = "Reporting.Settings";
    }
}
