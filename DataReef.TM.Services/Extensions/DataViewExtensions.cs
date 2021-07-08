using DataReef.Core.Infrastructure.Authorization;
using DataReef.Integrations.Geo.DataViews;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews.OnBoarding;
using DataReef.TM.Models.DTOs.Blobs;
using DataReef.TM.Models.DTOs.PropertyAttachments;
using DataReef.TM.Models.DTOs.Signatures.Proposals;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using DataReef.TM.Services;
using DataReef.TM.Services.Services;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

internal static class DataViewExtensions
{
    static List<Dictionary<string, string>> dealerSettings = new List<Dictionary<string, string>> {
            new Dictionary<string, string>{ { "Proposal", "1" } },
            new Dictionary<string, string>{ { OUSetting.Solar_ContractorID, "0"}},
            new Dictionary<string, string>{ { "Adders", "2" }}
        };


    /// <summary>
    /// Handle OUSettings.
    /// Update existing ones, create new ones if they don't exist
    /// </summary>
    /// <param name="dv"></param>
    /// <param name="ouid"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static List<OUSetting> HandleSettings(this OnboardingOUDataView dv, Guid ouid, List<OUSetting> settings, Lazy<ICRUDAuditService> auditService, Lazy<BlobService> blobService)
    {
        var ret = new List<OUSetting>();

        if (dv.Settings?.Count > 0)
        {
            foreach (var sett in dv.Settings)
            {
                // if value is null or empty, soft delete the setting
                if (string.IsNullOrEmpty(sett.Value) || sett.IsDeleted)
                {
                    var setting = settings.FirstOrDefault(s => s.Name == sett.Name);
                    if (setting != null)
                    {
                        auditService.Value.DeleteEntity(setting, "HandleSettings - InternalPortal");

                        setting.IsDeleted = true;
                        setting.Updated(SmartPrincipal.UserId, "InternalPortal");
                        setting.Value = string.IsNullOrEmpty(sett.Value) ? setting.Value : sett.Value;
                    }
                    continue;
                }

                // Add Cash & Mortgage to the financing options if they are not present
                //if (dv.CashAndMortgageIDs?.Count > 0
                //    && sett.Name == OUSetting.Financing_Options)
                //{
                //    var value = JsonConvert.DeserializeObject<List<FinancingSettingsDataView>>(sett.Value);
                //    var ids = value
                //                .Select(id => id.PlanID)
                //                .ToList();
                //    // create cash & mortgage on first positions
                //    var cashAndMortgage = dv
                //            .CashAndMortgageIDs
                //            .Where(cm => !ids.Contains(cm))
                //            .Select((cm, idx) => new FinancingSettingsDataView
                //            {
                //                PlanID = cm,
                //                IsEnabled = "1",
                //                Order = $"{ idx }"
                //            })
                //            .ToList();
                //    // shift all plans w/ cash & mortgage
                //    value.ForEach(v =>
                //    {
                //        v.IncreaseOrder(cashAndMortgage.Count);
                //    });
                //    value.AddRange(cashAndMortgage);

                //    sett.Value = JsonConvert.SerializeObject(value, Formatting.None);
                //}

                settings.HandleSetting(ret, sett.Name, sett.Value, ouid, auditService, sett.Group, sett.ValueType);

                //if (sett.Name == OUSetting.Financing_Options)
                //{
                //    var financingOptionsSett = settings.FirstOrDefault(s => s.Name == OUSetting.Financing_Options) ?? ret.FirstOrDefault(s => s.Name == OUSetting.Financing_Options);
                //    if (financingOptionsSett != null)
                //    {
                //        OUSettingService.UpdateChildOUFinancingOptionSettings(financingOptionsSett);
                //    }
                //}
                if (sett.Name == OUSetting.Solar_IsTenant && sett.Value == "1")
                {
                    settings.HandleSetting(ret, "CanChangeDealerSettings", "1", ouid, auditService, valueType: SettingValueType.Bool, inheritable: false);
                    settings.HandleSetting(ret, OUSetting.Solar_ContractorID, dv.BasicInfo.OUName, ouid, auditService, OUSettingGroupType.DealerSettings, SettingValueType.String);
                }

                //handle base64 images in rich text
                if (sett.Name.StartsWith(OUSetting.Legion_Photos_Prefix)
                    && sett.Name.EndsWith(OUSetting.Legion_Photos_Suffix))
                {
                    var settingValue = JsonConvert.DeserializeObject<PhotosDefinitionSettingModel>(sett.Value);
                    if (settingValue != null)
                    {
                        settingValue.Header.Instructions = ReplaceBase64ImagesAndUpload(settingValue.Header.Instructions, ouid, blobService);
                        settingValue.Data.Tasks.ForEach(x =>
                        {
                            x.Instructions = ReplaceBase64ImagesAndUpload(x.Instructions, ouid, blobService);
                        });

                        sett.Value = JsonConvert.SerializeObject(settingValue);

                        settings.HandleSetting(ret, sett.Name, sett.Value, ouid, auditService, OUSettingGroupType.DealerSettings, SettingValueType.JSON, true);
                    }
                }
            }
        }

        settings.HandleSetting(ret, "AvailableDealerSettings", JsonConvert.SerializeObject(dealerSettings), ouid, auditService);
        settings.HandleSetting(ret, "BatchPrescreenSource", "1", ouid, auditService, OUSettingGroupType.ConfigurationFile, SettingValueType.String);
        settings.HandleSetting(ret, "Loan", "1", ouid, auditService, OUSettingGroupType.DealerSettings, SettingValueType.Bool);

        return ret;
    }

    /// <summary>
    /// Handle OUSettings without any additional logic.
    /// Update existing ones, create new ones if they don't exist
    /// </summary>
    public static List<OUSetting> HandleSettingsLite(this OnboardingOUSettingsDataView dv, Guid ouid, List<OUSetting> settings, Lazy<ICRUDAuditService> auditService, Lazy<BlobService> blobService)
    {
        var ret = new List<OUSetting>();

        if (dv.Settings?.Count > 0)
        {
            foreach (var sett in dv.Settings)
            {
                // if value is null or empty, soft delete the setting
                if (string.IsNullOrEmpty(sett.Value) || sett.IsDeleted)
                {
                    var setting = settings.FirstOrDefault(s => s.Name == sett.Name);
                    if (setting != null)
                    {
                        auditService.Value.DeleteEntity(setting, "HandleSettings - InternalPortal");

                        setting.IsDeleted = true;
                        setting.Updated(SmartPrincipal.UserId, "InternalPortal");
                        setting.Value = string.IsNullOrEmpty(sett.Value) ? setting.Value : sett.Value;
                    }
                    continue;
                }

                //handle base64 images in rich text
                if (sett.Name.StartsWith(OUSetting.Legion_Photos_Prefix)
                    && sett.Name.EndsWith(OUSetting.Legion_Photos_Suffix))
                {
                    var settingValue = JsonConvert.DeserializeObject<PhotosDefinitionSettingModel>(sett.Value);
                    if (settingValue != null)
                    {
                        settingValue.Header.Instructions = ReplaceBase64ImagesAndUpload(settingValue.Header.Instructions, ouid, blobService);
                        settingValue.Data.Tasks.ForEach(x =>
                        {
                            x.Instructions = ReplaceBase64ImagesAndUpload(x.Instructions, ouid, blobService);
                        });

                        sett.Value = JsonConvert.SerializeObject(settingValue);

                        settings.HandleSetting(ret, sett.Name, sett.Value, ouid, auditService, OUSettingGroupType.DealerSettings, SettingValueType.JSON, true);
                    }
                }
                else
                {
                    settings.HandleSetting(ret, sett.Name, sett.Value, ouid, auditService, sett.Group, sett.ValueType);
                }
            }
        }

        return ret;
    }

    private static string ReplaceBase64ImagesAndUpload(string html, Guid ouid, Lazy<BlobService> blobService)
    {
        if (string.IsNullOrEmpty(html))
        {
            return html;
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        //add styles to make images display properly on mobile devices
        var htmlNode = doc.DocumentNode?.SelectSingleNode("/html");

        //content is HTML, but not wrapped in a <html> tag
        if (htmlNode == null)
        {
            //re-create wrapped HTML structure
            doc = new HtmlDocument();
            var node = HtmlNode.CreateNode("<html><head><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><style>body{background-color:#DDD}img{width:100%;max-width:600px}</style></head></html>");
            doc.DocumentNode.AppendChild(node);


            node?.AppendChild(HtmlNode.CreateNode($"<body>{html}</body>"));

        }


        bool styleNodeAlreadyPresent = doc.DocumentNode
            ?.Descendants("style")
            ?.Any(e =>
            {
                var styleContent = e?.InnerHtml;
                if (!string.IsNullOrEmpty(styleContent))
                {
                    return styleContent.Equals("body{background-color:#DDD}img{width:100%;max-width:600px}");
                }
                return false;
            }) ?? false;

        if (styleNodeAlreadyPresent != true)
        {
            var bodyNode = doc.DocumentNode?.SelectSingleNode("//body");
            bodyNode?.AppendChild(HtmlNode.CreateNode("<style>body{background-color:#DDD}img{width:100%;max-width:600px}</style>"));
        }

        doc.DocumentNode
            ?.Descendants("img")
            ?.Where(e =>
            {
                string src = e.GetAttributeValue("src", null) ?? "";
                return !string.IsNullOrEmpty(src) && src.StartsWith("data:image");
            })
            ?.ToList()
            ?.ForEach(x =>
            {
                string currentSrcValue = x.GetAttributeValue("src", null);

                var srcParts = currentSrcValue.Split(',');
                if (srcParts.Length > 1)
                {
                    var imageValue = srcParts[1];//Base64 part of string
                    byte[] imageData = Convert.FromBase64String(imageValue);

                    string contentType = null;
                    var contentTypeParts = srcParts[0].Split(';');
                    if (contentTypeParts.Length > 0)
                    {
                        contentType = contentTypeParts[0].Replace("data:", string.Empty);
                    }

                    var imageUrl =
                            blobService.Value
                            .UploadByNameGetFileUrl(
                                $"ous/{ouid}/photosdefinitions/{Guid.NewGuid()}",
                                new BlobModel
                                {
                                    Content = imageData,
                                    ContentType = contentType ?? "image/jpeg"
                                },
                                BlobAccessRights.PublicRead);
                    x.SetAttributeValue("src", imageUrl);
                }

            });

        return doc.DocumentNode.OuterHtml;
    }


    public static string ValidateContractorID(this OnboardingOUDataView dv, Guid ouid, DataContext dc)
    {
        var contractorSetting = dv
                                    .Settings?
                                    .FirstOrDefault(s => s.Name == OUSetting.Solar_ContractorID);

        if (contractorSetting != null)
        {
            // Get First ContractorID going up, to see if it's the same
            var setting = OUSettingService
                                .GetOuSettings(ouid)?
                                .FirstOrDefault(s => s.Name == OUSetting.Solar_ContractorID);

            if (setting == null)
            {
                return null;
            }

            // if provider ContractorID is different, we check to see if it does not conflict w/ other OUs
            if (setting.Value != contractorSetting.Value)
            {
                var contractor = dc
                                    .OUSettings
                                    .Include(o => o.OU)
                                    .FirstOrDefault(s => s.Name == OUSetting.Solar_ContractorID && s.Value == contractorSetting.Value && !s.IsDeleted);

                return contractor?.OU?.Name;
            }
            else
            {
                // if it's the same as ancestor's, we'll remove it from settings list
                if (setting.Value == contractorSetting.Value)
                {
                    dv.Settings.Remove(contractorSetting);
                }
            }
        }
        return null;
    }


    public static void HandleLogoImage(this OnboardingOUDataView dv, Guid ouid, List<OUSetting> existingSettings, BlobService blobService)
    {
        if (dv?.BasicInfo?.LogoImage == null)
            return;

        var logoImage = dv.BasicInfo.LogoImage?.Split(',')?[1];

        if (!string.IsNullOrEmpty(logoImage))
        {
            var logoSettingGuid = Guid.NewGuid();

            var logoSetting = existingSettings.FirstOrDefault(s => s.Name == OUSetting.LegionOULogoImageUrl);

            if (logoSetting != null)
            {
                logoSettingGuid = logoSetting.Guid;
                try
                {
                    var start = logoSetting.Value.IndexOf("/ous/");
                    var end = logoSetting.Value.IndexOf("?");
                    var path = logoSetting.Value.Substring(start + 1, end - start - 1);
                    blobService.DeleteByName(path);
                }
                catch { }
            }

            var imageUrl = blobService.UploadByNameGetFileUrl($"ous/{ouid}/{logoSettingGuid}",
                new BlobModel
                {
                    Content = Convert.FromBase64String(logoImage),
                    ContentType = "image/jpeg"
                }, BlobAccessRights.PublicRead);

            dv.Settings.Add(new OUSettingDataView
            {
                Name = OUSetting.LegionOULogoImageUrl,
                Value = imageUrl,
                Group = OUSettingGroupType.ConfigurationFile,
                ValueType = SettingValueType.String
            });
        }
    }

    public static void HandleGenericProposalLogoImage(this OnboardingOUDataView dv, Guid ouid, List<OUSetting> existingSettings, BlobService blobService)
    {
        if (dv.GenericProposalSettings.HeaderLogoImage == null && dv.GenericProposalSettings.FooterLogoImage == null)
            return;

        var headerlogoImage = dv.GenericProposalSettings.HeaderLogoImage?.Split(',')?[1];
        var footerlogoImage = dv.GenericProposalSettings.HeaderLogoImage?.Split(',')?[1];

        var logoSetting = existingSettings.FirstOrDefault(s => s.Name == OUSetting.GenericProposal_Settings);
        if (!string.IsNullOrEmpty(headerlogoImage) || !string.IsNullOrEmpty(footerlogoImage))
        {
            var logoSettingGuid = Guid.NewGuid();
            if (logoSetting != null)
            {
                var settings = logoSetting.GetValue<NewOUGenericProposalsDataView>();
                if (settings != null)
                {
                    logoSettingGuid = logoSetting.Guid;
                    if (!string.IsNullOrEmpty(headerlogoImage))
                    {
                        try
                        {
                            var start = settings.HeaderLogoUrl.IndexOf("/ous/generic-proposal/header/");
                            var end = settings.HeaderLogoUrl.IndexOf("?");
                            var path = settings.HeaderLogoUrl.Substring(start + 1, end - start - 1);
                            blobService.DeleteByName(path);
                        }
                        catch { }

                        var headerUrl = blobService.UploadByNameGetFileUrl($"ous/generic-proposal//header/{ouid}/{logoSettingGuid}",
                             new BlobModel
                             {
                                 Content = Convert.FromBase64String(headerlogoImage),
                                 ContentType = "image/jpeg"
                             }, BlobAccessRights.PublicRead);

                        dv.GenericProposalSettings.HeaderLogoUrl = headerUrl;
                    }
                    else if (!string.IsNullOrEmpty(footerlogoImage))
                    {
                        try
                        {
                            var start = settings.FooterLogoUrl.IndexOf("/ous/generic-proposal/footer/");
                            var end = settings.FooterLogoUrl.IndexOf("?");
                            var path = settings.FooterLogoUrl.Substring(start + 1, end - start - 1);
                            blobService.DeleteByName(path);
                        }
                        catch { }

                        var footerUrl = blobService.UploadByNameGetFileUrl($"ous/generic-proposal/footer/{ouid}/{logoSettingGuid}",
                             new BlobModel
                             {
                                 Content = Convert.FromBase64String(footerlogoImage),
                                 ContentType = "image/jpeg"
                             }, BlobAccessRights.PublicRead);

                        dv.GenericProposalSettings.FooterLogoUrl = footerUrl;
                    }
                }
            }

            dv.Settings.Add(new OUSettingDataView
            {
                Name = OUSetting.GenericProposal_Settings,
                Value = JsonConvert.SerializeObject(new NewOUGenericProposalsDataView
                {
                    FooterLogoUrl = dv.GenericProposalSettings.FooterLogoUrl,
                    HeaderLogoUrl = dv.GenericProposalSettings.HeaderLogoUrl,
                    WelcomeText = dv.GenericProposalSettings.WelcomeText,
                    Color = dv.GenericProposalSettings.Color
                }),
                Group = OUSettingGroupType.ConfigurationFile,
                ValueType = SettingValueType.String
            });
        }
    }

    private static OUSetting GetByName(this List<OUSetting> settings, string name, Guid ouid)
    {
        return settings?.FirstOrDefault(s => s.Name == name && s.OUID == ouid);
    }

    public static void HandleSetting(this List<OUSetting> existingSettings, List<OUSetting> newSettings, string name, string value, Guid ouid, Lazy<ICRUDAuditService> auditService, OUSettingGroupType group = OUSettingGroupType.ConfigurationFile, SettingValueType valueType = SettingValueType.JSON, bool inheritable = true)
    {
        var setting = existingSettings.GetByName(name, ouid);
        if (setting != null)
        {
            auditService.Value.UpdateValue(setting.Guid, setting.Name, nameof(OUSetting), setting.Value, value, "Handle Setting - InternalPortal");

            setting.Value = value;
            setting.Group = group;
            setting.Inheritable = inheritable;
            setting.ValueType = valueType;
            setting.Updated(SmartPrincipal.UserId, "InternalPortal");
            setting.IsDeleted = false;
            return;
        }
        newSettings.Add(new OUSetting
        {
            Name = name,
            OUID = ouid,
            Value = value,
            Group = group,
            Inheritable = inheritable,
            ValueType = valueType,
            CreatedByID = SmartPrincipal.UserId,
            CreatedByName = "InternalPortal"
        });
    }

    public static HighResImage ToHighResImg(this HighResolutionImage img)
    {
        return new HighResImage
        {
            Guid = img.Guid,
            DateCreated = img.DateCreated,
            CreatedBy = img.CreatedByID,
            Left = img.Left,
            Right = img.Right,
            Top = img.Top,
            Bottom = img.Bottom,
            Width = img.Width,
            Height = img.Height,
            MapUnitsPerPixelX = img.MapUnitsPerPixelX,
            MapUnitsPerPixelY = img.MapUnitsPerPixelY,
            SkewX = img.SkewX,
            SkewY = img.SkewY,
            Resolution = img.Resolution,
            Source = img.Source,
            IsDeleted = img.IsDeleted
        };
    }

    public static ProposalFinancePlanOption ToPlanOption(this LoanResponse data, string name, string description, PlanOptionType optionType, decimal? introPayment = null, FinancePlanType? planType = null)
    {
        // use the forecastData to generate the 'Smart' plan
        // use month 9 for 1-18
        //var monthFirst = planType == FinancePlanType.Lease ? data.Months[2] : data.Months[16];
        // use month 20 for 19 - end
        //var monthSecond = data.Months[19];
        var isITCApplied = optionType != PlanOptionType.Standard;

        var introMonthlyPayment = introPayment ?? data.IntroMonthlyPayment;
        var mainMonthlyPayment = !isITCApplied ? data.MonthlyPaymentNoITC : data.MonthlyPayment;


        // TODO: remove the param and the code below once we finalize loanpal / Sunnova integration
        if (planType.HasValue)
        {
            introMonthlyPayment = (planType.Value == FinancePlanType.Lease ? data.Months[2] : data.Months[16]).PaymentAmount;
            mainMonthlyPayment = data.Months[19].PaymentAmount;
        }
        var year1 = data.Years[0];
        var year3 = data.Years[2];
        var year30 = data.Years.LastOrDefault();

        var newUtilityBill = year1.ElectricityBillWithSolar / 12;
        //var monthlySavings = (year1.ElectricityBillWithoutSolar / 12) - (mainMonthlyPayment + newUtilityBill);


        //as per new calulation
        var monthlySavings = (year1.ElectricityBillWithoutSolar / 12) - (introMonthlyPayment + newUtilityBill);
        var annualSavings = monthlySavings * 12;
        var totalCost = data.AmountFinanced == 0 ? data.SolarSystemCost : data.AmountFinanced;

        return new ProposalFinancePlanOption
        {
            Name = name,
            PlanOptionType = optionType,
            Description = description,
            Balance = data.AmountFinanced,
            PaymentFactorsFirstPeriod = data.PaymentFactorsFirstPeriod,
            Payment18M = introPayment ?? data.IntroMonthlyPayment,
            Payment19M = mainMonthlyPayment,
            NewUtilityBill = newUtilityBill,
            MonthlySavings = monthlySavings,
            AnnualSavings = annualSavings,
            CumulativeSavings3Y = data.Years.Where(y => y.Year <= 3).Sum(y => y.GetSavings(isITCApplied)),
            CumulativeSavings30Y = data.Years.Sum(y => y.GetSavings(isITCApplied)),
            Breakeven = data.Years.FindIndex(y => data.Years.Where(x => x.Year <= y.Year).Sum(x => x.GetSavings(isITCApplied)) > data.AmountFinanced) + 1,
            ROI = totalCost == 0 ? 0 : data.Years.Take(data.Years.Count).Sum(y => y.GetSavings(isITCApplied) / totalCost * 100) / data.Years.Count,

            SolarkWhRate30Y = data.GetLCOE()
        };
    }
}
