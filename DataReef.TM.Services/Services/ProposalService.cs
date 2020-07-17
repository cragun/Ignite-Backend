using DataReef.Core.Extensions;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.Engines.FinancialEngine.Loan;
using DataReef.Integrations.Microsoft;
using DataReef.Integrations.Microsoft.PowerBI.Models;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Contracts.Services.NoSQL;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.ClientAPI;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.Blobs;
using DataReef.TM.Models.DTOs.Proposals;
using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models.DTOs.Signatures.Proposals;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.DTOs.Solar.Proposal;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.FinancialIntegration;
using DataReef.TM.Models.Solar;
using DataReef.TM.Services.Extensions;
using DataReef.TM.Services.InternalServices;
using DataReef.TM.Services.Services.ProposalAddons.Sigora;
using DataReef.TM.Services.Services.ProposalAddons.TriSMART;
using DataReef.TM.Services.Services.ProposalAddons.TriSMART.Models;
using EntityFramework.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class ProposalService : DataService<Proposal>, IProposalService
    {
        private static readonly string ProposalDocument = "Proposal";
        private static readonly string ContractDocument = "Contract";
        private static readonly int maxResolution = 1000;
        private static readonly int ProposalMediaItemMaxResolution = 480;
        private static readonly int sharedMediaItemsExpirationDays = 30;
        private readonly Lazy<IBlobService> _blobService;
        private readonly Lazy<IFinancingCalculator> _loanCalculator;
        private readonly Lazy<IOUSettingService> _ouSettingService;
        private Lazy<IPowerBIBridge> _powerBIBridge;
        private Lazy<IUtilServices> _utilServices;
        private Lazy<IFinancePlanDefinitionService> _financePlanDefinitionService;
        private readonly Lazy<ISolarSalesTrackerAdapter> _solarSalesTrackerAdapter;
        private readonly Lazy<INoSQLDataService> _noSqlDataService;

        private readonly string _templateDefaultUrl = ConfigurationManager.AppSettings["Proposal.Template.DefaultBaseUrl"];

        public ProposalService(Lazy<IBlobService> blobService,
            ILogger logger,
            Lazy<IFinancingCalculator> loanCalculator,
            Lazy<IOUSettingService> ouSettingService,
            Lazy<IPowerBIBridge> powerBIBridge,
            Lazy<IUtilServices> utilServices,
            Lazy<IFinancePlanDefinitionService> financePlanDefinitionService,
            Func<IUnitOfWork> unitOfWorkFactory,
            Lazy<ISolarSalesTrackerAdapter> solarSalesTrackerAdapter,
            Lazy<INoSQLDataService> noSqlDataService)
            : base(logger, unitOfWorkFactory)
        {
            _blobService = blobService;
            _loanCalculator = loanCalculator;
            _ouSettingService = ouSettingService;
            _powerBIBridge = powerBIBridge;
            _utilServices = utilServices;
            _financePlanDefinitionService = financePlanDefinitionService;
            _solarSalesTrackerAdapter = solarSalesTrackerAdapter;
            _noSqlDataService = noSqlDataService;
        }

        public override Proposal Insert(Proposal entity)
        {
            entity.PrepareNavigationProperties();
            var proposal = base.Insert(entity);

            Task.Factory.StartNew(() =>
            {
                //  TODO: in a hurry to ship the fix, hardcoding the includes, must create a recursive method to get all the references at all levels from a particular entity
                var ret = Get(entity.Guid, @"SolarSystem.AdderItems,SolarSystem.PowerConsumption,SolarSystem.RoofPlanes.Points,SolarSystem.RoofPlanes.Edges,SolarSystem.RoofPlanes.Panels,SolarSystem.RoofPlanes.Obstructions.ObstructionPoints,SolarSystem.SystemProduction.Months,SolarSystem.FinancePlans.Documents,Tariff");
                if (ret != null)
                {
                    ret.SaveResult = proposal.SaveResult;

                    if (ret.SaveResult.Success)
                    {
                        Guid? ouid = null;
                        var roofPlanes = entity
                                            .SolarSystem?
                                            .RoofPlanes?
                                            .ToList();

                        using (var dc = new DataContext())
                        {
                            // set the SolarPanel property on roof planes. 
                            // We'll need it for PBI integration
                            if (roofPlanes?.Count > 0)
                            {
                                var panelIds = roofPlanes
                                                    .Select(rp => rp.SolarPanelID)
                                                    .ToList();
                                var panels = dc
                                            .SolarPanel
                                            .Where(sp => panelIds.Contains(sp.Guid))
                                            .ToList();

                                foreach (var plane in roofPlanes)
                                {
                                    plane.SolarPanel = panels.FirstOrDefault(p => p.Guid == plane.SolarPanelID);
                                }
                            }

                            ouid = dc.Properties.Where(p => p.Guid == ret.PropertyID).Select(p => p.Territory.OUID).SingleOrDefault();
                        }
                        if (ouid.HasValue)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                var pbi = entity?.SolarSystem?.PowerConsumption?.ToPBI(ret.PropertyID, ouid.Value);
                                _powerBIBridge.Value.PushDataAsync(pbi);

                                var roofPBI = entity
                                                .SolarSystem?
                                                .RoofPlanes?
                                                .ToList()?
                                                .ToPBI(entity.PropertyID, ouid.Value, true);
                                if (roofPBI != null)
                                {
                                    _powerBIBridge.Value.PushDataAsync(roofPBI);
                                }
                                base.ProcessApiWebHooks(ret.PropertyID, ApiObjectType.Customer, EventDomain.Proposal, EventAction.Created, ouid.Value);
                            });
                        }
                    }
                }
            });

            return proposal;
        }

        public override Proposal Update(Proposal entity)
        {

            if (entity == null)
            {
                return null;
            }

            if (entity.SolarSystem != null)
            {
                try
                {
                    entity.SolarSystem.ValidateSystemValid();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }

            }

            var newRoofPlanes = (entity
                                .SolarSystem?
                                .RoofPlanes?
                                .ToList() ?? new List<RoofPlane>());

            var roofPlanesHashSet = new HashSet<string>(newRoofPlanes.Select(rp => rp.PseudoHash()));

            entity.PrepareNavigationProperties();

            using (var dataContext = new DataContext())
            {
                using (var transaction = dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        Guid ouid;
                        List<RoofPlane> oldRoofPlanes = null;
                        // for some reasong, if using the same context to retrieve this data
                        // update navigation properties below will crash.
                        using (var dc = new DataContext())
                        {
                            var fPlan = dc
                                            .FinancePlans
                                            .FirstOrDefault(fp => fp.SolarSystemID == entity.Guid);

                            ProposalData pData = null;
                            if (fPlan != null)
                            {
                                pData = dc
                                            .ProposalData
                                            .FirstOrDefault(pd => pd.FinancePlanID == fPlan.Guid);
                            }

                            // if the client is trying to patch a Proposal that is signed, 
                            // we need to store the old Request & Response
                            // When signing a proposal w/ a custom Utility Inflation Rate, these 2 values get recalculated
                            // and stored on the FinancePlan object. 
                            if (pData?.SignatureDate != null
                                && entity.SolarSystem?.FinancePlans != null)
                            {
                                foreach (var plan in entity.SolarSystem.FinancePlans)
                                {
                                    plan.RequestJSON = fPlan.RequestJSON;
                                    plan.ResponseJSON = fPlan.ResponseJSON;
                                }
                            }

                            // if we use the same datacontext, save will get messed up and crash the request
                            ouid = dc
                                           .Properties
                                           .Include(p => p.Territory)
                                           .FirstOrDefault(p => p.Guid == entity.PropertyID)
                                           .Territory
                                           .OUID;

                            if (entity.SolarSystem != null)
                            {
                                oldRoofPlanes = dc
                                                    .RoofPlanes
                                                    .Include(rp => rp.Panels)
                                                    .Where(rp => rp.SolarSystemID == entity.SolarSystem.Guid)
                                                    .ToList();

                                // set the SolarPanel property on roof planes. 
                                // We'll need it for PBI integration
                                if (newRoofPlanes?.Count > 0)
                                {
                                    var panelIds = newRoofPlanes
                                                        .Select(rp => rp.SolarPanelID)
                                                        .ToList();
                                    var panels = dc
                                                .SolarPanel
                                                .Where(sp => panelIds.Contains(sp.Guid))
                                                .ToList();

                                    foreach (var plane in newRoofPlanes)
                                    {
                                        plane.SolarPanel = panels.FirstOrDefault(p => p.Guid == plane.SolarPanelID);
                                    }
                                }
                            }
                        }
                        if (entity.SolarSystem != null)
                        {
                            var solarSystem = entity.SolarSystem;
                            solarSystem.Proposal = entity;
                            solarSystem.Guid = entity.Guid;


                            // DELETE FROM [solar].[PowerConsumption]
                            dataContext
                                    .PowerConsumption.OfType<SolarSystemPowerConsumption>()
                                    .Where(pc => pc.SolarSystemID == solarSystem.Guid)
                                    .Delete();

                            var adderItemIds = dataContext.AdderItems.Where(ai => ai.SolarSystemID == solarSystem.Guid).Select(ai => ai.Guid).ToList();
                            if (adderItemIds.Any())
                            {
                                //  DELETE ROOF PLANE DETAILS
                                if (dataContext.RoofPlaneDetails.Any(rpd => adderItemIds.Contains(rpd.AdderItemID)))
                                {
                                    dataContext
                                        .RoofPlaneDetails
                                        .Where(rpd => adderItemIds.Contains(rpd.AdderItemID))
                                        .Delete();
                                }
                            }

                            //  DELETE ADDER ITEMS
                            if (dataContext.AdderItems.Any(ai => ai.SolarSystemID == solarSystem.Guid))
                            {
                                dataContext.AdderItems.Where(ai => ai.SolarSystemID == solarSystem.Guid).Delete();
                            }

                            var roofPlaneIds = oldRoofPlanes
                                                .Select(rp => rp.Guid)
                                                .ToList();

                            var oldRoofPlaneHashSet = new HashSet<string>(oldRoofPlanes.Select(rp => rp.PseudoHash()));

                            if (newRoofPlanes.Count > 0
                                && ((oldRoofPlanes.Count == 0) || (oldRoofPlanes.Count > 0 && !roofPlanesHashSet.SetEquals(oldRoofPlaneHashSet))))
                            {
                                var roofPBI = newRoofPlanes.ToPBI(entity.PropertyID, ouid, !(oldRoofPlanes?.Count > 0));
                                _powerBIBridge.Value.PushDataAsync(roofPBI);
                            }

                            if (roofPlaneIds.Any())
                            {
                                dataContext
                                        .RoofPlanePoints
                                        .Where(pp => roofPlaneIds.Contains(pp.RoofPlaneID))
                                        .Delete();
                                dataContext
                                        .RoofPlaneEdges
                                        .Where(pe => roofPlaneIds.Contains(pe.RoofPlaneID))
                                        .Delete();
                                dataContext
                                        .RoofPlanePanels
                                        .Where(ppl => roofPlaneIds.Contains(ppl.RoofPlaneID))
                                        .Delete();

                                var roofPlaneObstructionIds = dataContext.RoofPlaneObstructions
                                    .Where(rpo => roofPlaneIds.Contains(rpo.RoofPlaneID))
                                    .Select(rpo => rpo.Guid)
                                    .ToList();
                                if (roofPlaneObstructionIds.Any())
                                {
                                    dataContext.ObstructionPoints
                                        .Where(op => roofPlaneObstructionIds.Contains(op.RoofPlaneObstructionID))
                                        .Delete();
                                    dataContext
                                        .RoofPlaneObstructions
                                        .Where(po => roofPlaneIds.Contains(po.RoofPlaneID))
                                        .Delete();
                                }

                                if (dataContext.RoofPlaneDetails.Any(rpd => roofPlaneIds.Contains(rpd.RoofPlaneID)))
                                {
                                    dataContext.RoofPlaneDetails
                                        .Where(rpd => roofPlaneIds.Contains(rpd.RoofPlaneID))
                                        .Delete();
                                }

                                dataContext
                                        .RoofPlanes
                                        .Where(rp => roofPlaneIds.Contains(rp.Guid))
                                        .Delete();

                            }

                            var remainingFinancePlanIds = entity
                                                            .SolarSystem
                                                            .FinancePlans
                                                            .Select(fp => fp.Guid)
                                                            .ToList();

                            var financePlanIds = dataContext
                                .FinancePlans
                                .Where(fp => fp.SolarSystemID == solarSystem.Guid)
                                .Select(fp => fp.Guid)
                                .ToList();

                            var remainingFinanceDocuments = dataContext.FinanceDocuments.Where(fd => remainingFinancePlanIds.Contains(fd.FinancePlanID)).ToList();

                            // deleting the FinanceDocument entities that are not being sent with the Update request for this Proposal and their Amazon contents
                            var financePlanIdsDeleteDocuments = financePlanIds.Except(remainingFinancePlanIds).ToList();
                            if (financePlanIdsDeleteDocuments.Any())
                            {
                                DeleteDocuments(financePlanIdsDeleteDocuments, dataContext);
                            }

                            // deleting the FinanceDocument entities that are being sent with the Update request for this Proposal, these will be re-added to the database with updated values
                            if (remainingFinancePlanIds.Count > 0)
                            {
                                dataContext
                                    .FinanceDocuments
                                    .Where(fd => remainingFinancePlanIds.Contains(fd.FinancePlanID))
                                    .Delete();
                            }

                            //var proposalData =
                            //    dataContext
                            //        .ProposalData
                            //        .Where(pd => financePlanIds.Contains(pd.FinancePlanID))
                            //        .ToList();

                            //  DELETE ProposalData
                            //if (proposalData.Any())
                            //{
                            //    dataContext
                            //        .ProposalData
                            //        .Where(pd => financePlanIds.Contains(pd.FinancePlanID))
                            //        .Delete();
                            //}

                            // DELETE FROM [solar].[FinancePlans]
                            dataContext
                                    .FinancePlans
                                    .Where(fp => fp.SolarSystemID == solarSystem.Guid)
                                    .Delete();

                            // DELETE FROM [solar].[SystemProductionMonths]
                            dataContext
                                    .SystemProductionMonths
                                    .Where(spm => spm.SystemProductionID == solarSystem.Guid)
                                    .Delete();

                            // DELETE FROM [solar].[SystemsProduction]
                            dataContext
                                    .SystemsProduction
                                    .Where(sp => sp.Guid == solarSystem.Guid)
                                    .Delete();

                            dataContext.SaveChanges();
                            foreach (var financePlan in entity.SolarSystem.FinancePlans)
                            {
                                financePlan.Documents = remainingFinanceDocuments.Where(pd => pd.FinancePlanID == financePlan.Guid).ToList();
                                //financePlan.ProposalData = proposalData.Where(pd => pd.FinancePlanID == financePlan.Guid).ToList();
                            }
                        }

                        var ret = base.Update(entity, dataContext);

                        if (entity.Tariff != null) base.Update(entity.Tariff, dataContext);

                        if (entity.SolarSystem != null)
                        {
                            base.Update(entity.SolarSystem, dataContext);
                            UpdateNavigationProperties(entity.SolarSystem, dataContext: dataContext);

                            if (entity.SolarSystem.RoofPlanes != null)
                            {
                                foreach (var roofPlane in entity.SolarSystem.RoofPlanes)
                                {
                                    base.Update(roofPlane, dataContext);
                                    UpdateNavigationProperties(roofPlane, dataContext: dataContext);

                                    if (roofPlane.Obstructions != null)
                                    {
                                        foreach (var roofPlaneObstruction in roofPlane.Obstructions)
                                        {
                                            base.Update(roofPlaneObstruction, dataContext);
                                            UpdateNavigationProperties(roofPlaneObstruction, dataContext: dataContext);

                                            if (roofPlaneObstruction.ObstructionPoints != null)
                                            {
                                                foreach (var obstructionPoint in roofPlaneObstruction.ObstructionPoints)
                                                {
                                                    base.Update(obstructionPoint, dataContext);
                                                    UpdateNavigationProperties(obstructionPoint, dataContext: dataContext);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (entity.SolarSystem.AdderItems != null)
                            {
                                foreach (var adderItem in entity.SolarSystem.AdderItems)
                                {
                                    base.Update(adderItem, dataContext);
                                    UpdateNavigationProperties(adderItem, dataContext: dataContext);

                                    if (adderItem.RoofPlaneDetails != null)
                                    {
                                        foreach (var roofPlaneDetail in adderItem.RoofPlaneDetails)
                                        {
                                            base.Update(roofPlaneDetail, dataContext);
                                            UpdateNavigationProperties(roofPlaneDetail, dataContext: dataContext);
                                        }
                                    }
                                }
                            }

                            if (entity.SolarSystem.SystemProduction != null)
                            {
                                base.Update(entity.SolarSystem.SystemProduction, dataContext);
                                UpdateNavigationProperties(entity.SolarSystem.SystemProduction, dataContext: dataContext);
                            }
                        }

                        //update territory DateLastModified
                        var property = dataContext.Properties.Include(p => p.Territory).FirstOrDefault(x => x.Guid == entity.PropertyID);
                        property.Territory?.Updated(SmartPrincipal.UserId);
                        dataContext.SaveChanges();

                        transaction.Commit();

                        Task.Factory.StartNew(() =>
                        {
                            var pbi = entity?.SolarSystem?.PowerConsumption?.ToPBI(ret.PropertyID, ouid);
                            if (pbi != null)
                            {
                                _powerBIBridge.Value.PushDataAsync(pbi);
                            }

                            try
                            {
                                base.ProcessApiWebHooks(ret.PropertyID, ApiObjectType.Customer, EventDomain.Proposal, EventAction.Changed, entity.Property.Territory.OUID);
                            }
                            catch (Exception)
                            {
                            }
                        });

                        return ret;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Delete documents from both DB and AWS for given financePlanId
        /// </summary>
        public void DeleteDocuments(List<Guid> financePlanIds, object dataContextObj, FinanceDocumentType? documentType = null)
        {
            var action = new Action<DataContext>((DataContext dc) =>
            {
                var docs = dc
                            .FinanceDocuments
                            .Where(fd => financePlanIds.Contains(fd.FinancePlanID) && (!documentType.HasValue || fd.DocumentType == documentType.Value))
                            .ToList();
                var files = new List<string>();

                foreach (var doc in docs)
                {
                    if (!string.IsNullOrWhiteSpace(doc.SignedURL))
                        files.Add(doc.SignedURL);
                    if (!string.IsNullOrWhiteSpace(doc.UnsignedURL))
                        files.Add(doc.UnsignedURL);
                    dc
                    .FinanceDocuments
                    .Remove(doc);
                }
                dc.SaveChanges();

                // Delete documents from Blob Storage
                foreach (var file in files)
                {
                    var url = new Uri(file);
                    var documentName = url.AbsolutePath.TrimStart('/');
                    try
                    {
                        _blobService.Value.DeleteByName(documentName);

                        var signatureProvider = new DataReef.Integrations.Hancock.IntegrationProvider();
                        signatureProvider.DeleteDocument(new Guid(new Uri(file).Segments.Last().Replace("_unsigned", "").Replace(".pdf", "")));
                    }
                    catch { }
                }
            });

            if (dataContextObj == null)
            {
                using (var dataContext = new DataContext())
                {
                    action(dataContext);
                }
            }
            else
            {
                action((DataContext)dataContextObj);
            }
        }

        public ICollection<ProposalLite> GetProposalsLite(Guid ouID, DateTime startDate, DateTime endDate, bool deep = true, int pageNumber = 1, int pageSize = 1000)
        {

            List<ProposalLite> ret = new List<ProposalLite>();

            using (DataContext dc = new DataContext())
            {
                ret = dc.Database.SqlQuery<ProposalLite>("exec proc_ProposalsLiteForOu @OUID,@StartDate,@EndDate,@Deep,@PageNumber,@PageSize",
                        new SqlParameter("@OUID", ouID),
                        new SqlParameter("@StartDate", startDate),
                        new SqlParameter("@EndDate", endDate),
                        new SqlParameter("@Deep", deep),
                        new SqlParameter("@PageNumber", pageNumber),
                        new SqlParameter("@PageSize", pageSize)
                    ).ToList();
            }

            return ret;
        }

        public string GetAgreementForProposal(Guid proposalID)
        {
            using (var dataContext = new DataContext())
            {
                var proposal = dataContext
                    .Proposal
                    .Include(x => x.Property)
                    .Include(x => x.Property.Territory)
                    .FirstOrDefault(x => x.Guid == proposalID);

                if (proposal == null)
                {
                    return null;
                }
                var settings = _ouSettingService.Value.GetSettingsByOUID(proposal.Property.Territory.OUID);
                var agreementsBaseUrl = settings.FirstOrDefault(x => x.Name.Equals("Proposal.Agreements.BaseUrl")).Value;

                var proposalData = dataContext.ProposalData.Where(x => x.ProposalID == proposalID).OrderByDescending(x => x.DateCreated).FirstOrDefault();
                if (proposalData == null)
                {
                    return null;
                }

                return $"{agreementsBaseUrl}{proposalData.Guid}?webview=1";
            }
        }

        public CreateProposalDataResponse CreateProposalData(DocumentSignRequest request)
        {
            //var json = new JavaScriptSerializer().Serialize(request);
            //if (json != null)
            //{
            //    ApiLogEntry apilog = new ApiLogEntry();
            //    apilog.Id = Guid.NewGuid();
            //    apilog.User = "/CreateProposalData/service";
            //    apilog.Machine = Environment.MachineName;
            //    apilog.RequestContentType = "";
            //    apilog.RequestRouteTemplate = "";
            //    apilog.RequestRouteData = "";
            //    apilog.RequestIpAddress = "";
            //    apilog.RequestMethod = "";
            //    apilog.RequestHeaders = "";
            //    apilog.RequestTimestamp = DateTime.UtcNow;
            //    apilog.RequestUri = "";
            //    apilog.ResponseContentBody = "";
            //    apilog.RequestContentBody = json;

            //    using (var dc = new DataContext())
            //    {
            //        dc.ApiLogEntries.Add(apilog);
            //        dc.SaveChanges();
            //    }
            //}

            using (var dataContext = new DataContext())
            {
                var financePlan = dataContext
                    .FinancePlans
                    .Include(fp => fp.FinancePlanDefinition.Provider)
                    .FirstOrDefault(fp => fp.Guid == request.FinancePlanID);

                if (financePlan == null) throw new ApplicationException("Invalid finance plan");

                var data = new ProposalData
                {
                    FinancePlanID = request.FinancePlanID,
                    ProposalTemplateID = request.ProposalTemplateID,
                    ProposalID = financePlan.SolarSystemID,
                    ProposalDate = SmartPrincipal.DeviceDate.LocalDateTime,
                    ContractorID = request.ContractorID,
                    SalesRepID = SmartPrincipal.UserId,
                    ProposalDataJSON = request.ProposalDataJSON,
                };

                //TODO: upload binary (images) UserInput & DocumentData to S3, and create UserInputLinks and DocumentDataLinks
                // The location should be: <s3-bucket> / proposal-data / <proposalData.Guid> /  <userInput.Guid> | <documentData.Guid>

                data.UserInputLinks = UploadUserInputData(request.UserInput, data);
                data.DocumentDataLinks = UploadDocumentData(request.DocumentData, data);

                var proposal = dataContext
                                .Proposal
                                .Include(fp => fp.Property.Territory)
                                .Include(fp => fp.SolarSystem.FinancePlans)
                                .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Panels))
                                .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.SolarPanel))
                                .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Inverter))
                                .FirstOrDefault(p => p.Guid == financePlan.SolarSystemID);
                if (proposal == null)
                {
                    throw new ApplicationException("Invalid proposal");
                }

                var ouSettings = OUSettingService.GetOuSettings(proposal.Property.Territory.OUID);

                //var contractorOUSettings = GetOUSettingsForContractorID(data.ContractorID);
                var documentUrls = GetProposalURLs(data.ContractorID, data.Guid, null, ouSettings);
                var proposalUrl = documentUrls.FirstOrDefault().Url;
                proposal.ProposalURL = proposalUrl;

                if (request.Location?.HasValue() == true)
                {
                    var metaInfo = data.MetaInformation ?? new ProposalDataMetaInformation();
                    metaInfo.GenerateLocation = request.Location;
                    data.MetaInformation = metaInfo;
                }

                try
                {
                    PushProposalDataToNoSQL(request.FinancePlanID, data);
                    data.UsesNoSQLAggregatedData = true;
                }
                catch
                {
                    data.UsesNoSQLAggregatedData = false;
                }

                dataContext.ProposalData.Add(data);

                proposal.Property?.Territory?.Updated(SmartPrincipal.UserId);

                dataContext.SaveChanges();

                var sendEmail = ouSettings.GetValueAsBool(OUSetting.Proposal_Features_SendEmailToSalesRepOnGenerate, true);
                var attachPDF = ouSettings.GetValueAsBool(OUSetting.Proposal_Features_AttachPDFOnGenerate, true);

                if (sendEmail)
                {
                    // Send email only to the sales rep.
                    var property = proposal.Property;
                    var planName = financePlan.Name;

                    var propertyAddress = proposal.GetPropertyAddress();

                    var salesRepEmailAddress = dataContext
                                        .People
                                        .Where(p => p.Guid == data.SalesRepID)
                                        .Select(p => p.EmailAddressString)
                                        .SingleOrDefault();

                    var homeOwnerName = property.Name;

                    string ccEmails = ouSettings?.FirstOrDefault(os => os.Name == OUSetting.Proposal_Features_EmailsToCC)?.Value;

                    Task.Factory.StartNew(() =>
                    {
                        var body = $"You will find the proposal for {homeOwnerName} at {propertyAddress} [{planName}] using the attached file or the link below: <br/> <br/> <a href='{proposalUrl}'>Proposal for {homeOwnerName}</a> <br/>";
                        List<System.Net.Mail.Attachment> attachments = null;

                        if (attachPDF)
                        {
                            var proposalPDF = _utilServices.Value.GetPDF($"{proposalUrl}");
                            if (proposalPDF != null)
                            {
                                attachments = new List<System.Net.Mail.Attachment> {
                            new System.Net.Mail.Attachment(new MemoryStream(proposalPDF), $"Proposal [{planName.AsFileName()}].pdf", "application/pdf")
                            };
                            }
                        }
                        Mail.Library.SendEmail(salesRepEmailAddress, ccEmails, $"Created Proposal for {homeOwnerName} at {propertyAddress}", body, true, attachments);
                    });
                }

                //var isSolarSalesTrackerEnabled = ouSettings.GetValue<SSTSettings>(SolarTrackerResources.SettingName)?.Enabled?.IsTrue() == true;

                // Don't push data to SST for now, 
                // we'll update once they will provide an update API

                //if (isSolarSalesTrackerEnabled)
                //{
                //    //  send information to SST
                //    Task.Factory.StartNew(() =>
                //    {
                //        _solarSalesTrackerAdapter.Value.SubmitSolarData(financePlan.Guid);
                //    });
                //}

                // Push information to PowerBI
                var pbi = new PBI_ProposalCreated
                {
                    ContractorID = request.ContractorID,
                    ProposalID = proposal.Guid,
                    FinancePlanDefinitionID = financePlan.FinancePlanDefinitionID,
                    FinancePlanDefinitionName = financePlan.FinancePlanDefinition?.Name,
                    FinancingType = financePlan.FinancePlanType.ToString(),
                    FinanceProvider = financePlan.FinancePlanDefinition?.Provider?.Name,
                    FinanceTermInYears = financePlan.FinancePlanDefinition?.TermInYears ?? 0,
                    TerritoryID = proposal.Property?.TerritoryID ?? Guid.Empty,
                    OUID = proposal.Property?.Territory?.OUID ?? Guid.Empty,
                    SalesRepID = SmartPrincipal.UserId,
                    PropertyID = proposal.PropertyID,
                    State = proposal.State,
                    SystemSize = proposal.SolarSystem?.SystemSize ?? 0,
                    FinancingAmount = (double)financePlan.Request.AmountToFinance,
                    TotalSavings = (double)financePlan.Response.TotalSavings,
                    PanelsCount = proposal.SolarSystem?.PanelCount ?? 0,
                    SystemDrawingType = proposal.DesignSystemType.ToString(),
                    RoofPlanesCount = proposal.SolarSystem?.RoofPlanes?.Count ?? 0,
                    Equipment = proposal.SolarSystem?.GetEquipmentInfo()
                };

                _powerBIBridge.Value.PushDataAsync(pbi);

                return new CreateProposalDataResponse
                {
                    ProposalID = data.Guid,
                    ProposalUrl = $"{proposalUrl}?webview=1",
                };
            }
        }

        private Proposal PushProposalDataToNoSQL(Guid financePlanId, ProposalData proposalData)
        {
            using (var dataContext = new DataContext())
            {
                var financePlan = dataContext
                    .FinancePlans
                    .FirstOrDefault(fp => fp.Guid == financePlanId);

                var proposal = dataContext.Proposal
                                                .Include(p => p.Property.Territory.OU)
                                                .Include(p => p.Property.Occupants)
                                                .Include(p => p.Property.PropertyBag)
                                                .Include(p => p.Tariff)
                                                .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Points))
                                                .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Edges))
                                                .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Panels))
                                                .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Obstructions))
                                                .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.SolarPanel))
                                                .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Inverter))
                                                .Include(p => p.SolarSystem.AdderItems)
                                                .Include(p => p.SolarSystem.PowerConsumption)
                                                .Include(p => p.SolarSystem.SystemProduction)
                                                .Include(p => p.SolarSystem.FinancePlans.Select(fp => fp.SolarSystem.Proposal.Tariff))
                                                .Include(p => p.SolarSystem.FinancePlans.Select(fp => fp.FinancePlanDefinition.Details))
                                                .Include(p => p.SolarSystem.FinancePlans.Select(fp => fp.FinancePlanDefinition.Provider))
                                                .FirstOrDefault(p => p.Guid == financePlan.SolarSystemID);

                proposal.Guid = proposalData.Guid;
                _noSqlDataService.Value.PutValue(proposal);
                return proposal;
            }
        }

        private List<OUSetting> GetOUSettingsForContractorID(string contractorID)
        {
            var contractorIDSettings = _ouSettingService.Value.List(filter: $"Name={OUSetting.Solar_ContractorID}&Value={contractorID}").ToList();
            var ouid = contractorIDSettings?.FirstOrDefault()?.OUID;

            if (ouid.HasValue)
            {
                return OUSettingService.GetOuSettings(ouid.Value);
            }
            return null;
        }

        private List<SignedDocumentDTO> GetProposalURLs(string contractorID, Guid propososalDataGuid, List<SignedDocumentDTO> signedDocuments = null, List<OUSetting> ouSettings = null)
        {
            var result = new List<SignedDocumentDTO>();

            var baseUrl = _templateDefaultUrl;

            var contractorIdSettings = ouSettings?.Where(ous => ous.Name == OUSetting.Solar_ContractorID)?.ToList()
                ?? _ouSettingService.Value.List(filter: $"name={OUSetting.Solar_ContractorID}&value={contractorID}").ToList();

            if (contractorIdSettings.Any())
            {
                var ouid = contractorIdSettings.FirstOrDefault().OUID;
                var settings = _ouSettingService.Value.GetSettings(ouid, null);
                if (settings.ContainsKey(OUSetting.Proposal_TemplateBaseUrl))
                {
                    baseUrl = settings
                                    .FirstOrDefault(s => s.Key.Equals(OUSetting.Proposal_TemplateBaseUrl, StringComparison.InvariantCultureIgnoreCase))
                                    .Value?
                                    .Value ?? baseUrl;
                }
            }

            baseUrl = baseUrl.TrimEnd('/');

            result.Add(new SignedDocumentDTO { Name = "Proposal", Url = $"{baseUrl}/{propososalDataGuid}" });

            if (signedDocuments != null)
            {
                result.AddRange(signedDocuments.Select(sd => new SignedDocumentDTO { Name = sd.Name, Url = $"{baseUrl}{sd.Url}" }).ToList());
            }

            return result;
        }

        private SignedDocumentDTO GetSignedAgreementUrl(string contractorID, Guid propososalDataGuid, List<OUSetting> ouSettings = null)
        {
            var result = new List<SignedDocumentDTO>();

            var baseUrl = _templateDefaultUrl;

            var contractorIdSettings = ouSettings?.Where(ous => ous.Name == OUSetting.Solar_ContractorID)?.ToList()
                ?? _ouSettingService.Value.List(filter: $"name={OUSetting.Solar_ContractorID}&value={contractorID}").ToList();

            if (contractorIdSettings.Any())
            {
                var ouid = contractorIdSettings.FirstOrDefault().OUID;
                var settings = _ouSettingService.Value.GetSettings(ouid, null);
                if (settings.ContainsKey("Proposal.Agreements.BaseUrl"))
                {
                    baseUrl = settings
                                    .FirstOrDefault(s => s.Key.Equals("Proposal.Agreements.BaseUrl", StringComparison.InvariantCultureIgnoreCase))
                                    .Value?
                                    .Value ?? baseUrl;
                }
            }

            baseUrl = baseUrl.TrimEnd('/');

            return new SignedDocumentDTO { Name = "Installation Agreement", Url = $"{baseUrl}/{propososalDataGuid}" };
        }

        private List<Tuple<string, string>> GetProposalDocumentsURLs(string contractorID, Guid propososalDataGuid, List<OUSetting> ouSettings = null)
        {
            var templateDefaultUrl = $"{_templateDefaultUrl}{propososalDataGuid}";

            var contractorIdSettings = ouSettings?.Where(ous => ous.Name == OUSetting.Solar_ContractorID)?.ToList()
                ?? _ouSettingService.Value.List(filter: $"name={OUSetting.Solar_ContractorID}&value={contractorID}").ToList();

            if (!contractorIdSettings.Any())
            {
                return new List<Tuple<string, string>> { new Tuple<string, string>(ProposalDocument, templateDefaultUrl) };
            }

            var ouid = contractorIdSettings.FirstOrDefault().OUID;
            return GetProposalUrlForOUID(ouid, propososalDataGuid);
        }

        private List<Tuple<string, string>> GetProposalUrlForOUID(Guid ouid, Guid propososalDataGuid)
        {
            var templateDefaultUrl = $"{_templateDefaultUrl}{propososalDataGuid}";
            var ouSettings = _ouSettingService.Value.GetSettings(ouid, null);
            if (!ouSettings.ContainsKey(Models.OUSetting.Proposal_TemplateBaseUrl))
            {

                return new List<Tuple<string, string>> { new Tuple<string, string>(ProposalDocument, templateDefaultUrl) };
            }

            var baseAddress = ouSettings
                                    .FirstOrDefault(s => s.Key.Equals(Models.OUSetting.Proposal_TemplateBaseUrl, StringComparison.InvariantCultureIgnoreCase))
                                    .Value
                                    .Value;

            var contractRelativePath = (ouSettings.ContainsKey(OUSetting.Proposal_Features_PostSignInternalPaths) ? ouSettings[OUSetting.Proposal_Features_PostSignInternalPaths] : null)?.Value;

            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                return new List<Tuple<string, string>> { new Tuple<string, string>(ProposalDocument, templateDefaultUrl) };
            }

            baseAddress = baseAddress.TrimEnd('/');
            var result = new List<Tuple<string, string>>();
            result.Add(new Tuple<string, string>(ProposalDocument, $"{baseAddress}/{propososalDataGuid}"));

            if (contractRelativePath != null)
            {
                result.Add(new Tuple<string, string>(ContractDocument, $"{baseAddress}/{contractRelativePath.Trim('/')}/{propososalDataGuid}"));
            }

            return result;
        }

        public Proposal SignAgreement(Guid proposalDataId, DocumentSignRequest request)
        {
            using (var dataContext = new DataContext())
            {
                var data = dataContext
                            .ProposalData
                            .FirstOrDefault(pd => pd.Guid == proposalDataId);

                if (data == null)
                {
                    throw new ApplicationException("Could not find Proposal Data!");
                }

                var proposal = dataContext
                                .Proposal
                                .Include(x => x.Property)
                                .Include(x => x.Property.Territory)
                                .FirstOrDefault(x => x.Guid == data.ProposalID);
                if (proposal == null)
                {
                    throw new ApplicationException("Could not find the proposal");
                }

                var reqObjectHasDataJSON = !string.IsNullOrWhiteSpace(request.ProposalDataJSON);

                if (reqObjectHasDataJSON)
                {
                    var currentObjectHasDataJSON = !string.IsNullOrWhiteSpace(data.ProposalDataJSON);

                    // if the object already contains a JSON, will merge the two
                    if (currentObjectHasDataJSON)
                    {
                        var mergeSettings = new JsonMergeSettings
                        {
                            MergeArrayHandling = MergeArrayHandling.Union
                        };

                        var propData = JObject.Parse(data.ProposalDataJSON);
                        var reqData = JObject.Parse(request.ProposalDataJSON);
                        propData.Merge(reqData, mergeSettings);

                        data.ProposalDataJSON = propData.ToString();
                    }
                    else
                    {
                        data.ProposalDataJSON = request.ProposalDataJSON;
                    }
                }

                if (request.UserInput != null)
                {
                    var existingUserInputLinks = data.UserInputLinks ?? new List<UserInputDataLinks>();

                    var newLinks = UploadUserInputData(request.UserInput, data);
                    if (newLinks?.Any() == true)
                    {
                        existingUserInputLinks.AddRange(newLinks);
                    }

                    data.UserInputLinks = existingUserInputLinks;
                }
                // save changes first, because the SignatureDate needs to have value when we generate the PDFs
                dataContext.SaveChanges();

                List<SignedDocumentDTO> signedDocuments = new List<SignedDocumentDTO>();
                if (!string.IsNullOrWhiteSpace(request.ProposalDataJSON))
                {
                    try
                    {
                        var proposalCustomData = JObject.Parse(request.ProposalDataJSON);

                        signedDocuments.AddRange(proposalCustomData["SignedDocuments"].ToObject<List<SignedDocumentDTO>>());
                    }
                    catch (Exception) { }
                }

                //add the agreement
                var contractorID = request.ContractorID ?? data.ContractorID;
                var ouSettings = OUSettingService.GetOuSettings(proposal.Property.Territory.OUID);
                if (signedDocuments?.Any(x => x.Name == "Installation Agreement") != true)
                {
                    var agreementSignedDocument = GetSignedAgreementUrl(contractorID, data.Guid, ouSettings);
                    signedDocuments.Add(agreementSignedDocument);
                }

                var attachmentPDFs = new List<Tuple<byte[], string>>();
                signedDocuments?
                        .ForEach(d =>
                        {
                            d.Description = $"{d.Name}";
                            d.ProposalDataID = proposalDataId;

                            if (d.Url.Contains("?webview=1"))
                            {
                                d.Url = d.Url.Replace("?webview=1", string.Empty);
                            }

                            if (d.Url.Contains("&webview=1"))
                            {
                                d.Url = d.Url.Replace("&webview=1", string.Empty);
                            }

                            var pdfContent = _utilServices.Value.GetPDF(d.Url);

                            d.PDFUrl = _blobService.Value.UploadByNameGetFileUrl($"proposal-data/{data.Guid}/documents/{DateTime.UtcNow.Ticks}.pdf",
                                 new BlobModel
                                 {
                                     Content = pdfContent,
                                     ContentType = "application/pdf"
                                 }, BlobAccessRights.PublicRead);

                            if (request.Location?.HasValue() == true)
                            {
                                d.AcceptLocation = request.Location;
                            }

                            attachmentPDFs.Add(new Tuple<byte[], string>(pdfContent, $"{d.Description}.pdf"));
                        });

                var proposalDocuments =
                    string.IsNullOrEmpty(proposal.SignedDocumentsJSON)
                    ? new List<SignedDocumentDTO>()
                    : JsonConvert.DeserializeObject<List<SignedDocumentDTO>>(proposal.SignedDocumentsJSON);
                proposalDocuments.AddRange(signedDocuments);

                proposal.SignedDocumentsJSON = JsonConvert.SerializeObject(proposalDocuments);

                _solarSalesTrackerAdapter.Value.SignAgreement(proposal, "2", signedDocuments?.FirstOrDefault(d => d.Name == "Proposal"));

                // update territory DateLastModified
                proposal.Property?.Territory?.Updated(SmartPrincipal.UserId);

                dataContext.SaveChanges();

                try
                {
                    PushProposalDataToNoSQL(request.FinancePlanID, data);
                }
                catch
                {
                }

                //Send email with the installation agreement
                var sendEmail = ouSettings.GetValueAsBool(OUSetting.Proposal_Agreement_Features_SendEmailOnSign, true);
                var attachPDF = ouSettings.GetValueAsBool(OUSetting.Proposal_Agreement_Features_SendEmailOnSign, true);

                if (sendEmail)
                {

                    var property = proposal.Property;

                    var propertyAddress = proposal.GetPropertyAddress();

                    var salesRepEmailAddress = dataContext
                                        .People
                                        .Where(p => p.Guid == data.SalesRepID)
                                        .Select(p => p.EmailAddressString)
                                        .SingleOrDefault();

                    var disableSendingToCustomer = ouSettings?
                                        .FirstOrDefault(os => os.Name == OUSetting.Proposal_Features_SendEmailToCustomer_Disabled)?
                                        .Value == "1";

                    var homeOwnerName = property.Name;

                    var homeOwnerEmailAddress = dataContext
                                        .Fields
                                        .Where(f => f.DisplayName == "Email Address" && f.PropertyId == proposal.PropertyID)
                                        .Select(f => f.Value)
                                        .FirstOrDefault();

                    string ccEmails = ouSettings?.FirstOrDefault(os => os.Name == OUSetting.Proposal_Agreement_Features_EmailsToCC)?.Value;

                    //var planName = financePlan.Name;

                    var documentsLinks = string.Join("<br/>", signedDocuments.Select(pd => $"<a target='_blank' href='{pd.Url}'>{pd.Name} for {homeOwnerName}<a/>"));

                    Task.Factory.StartNew(() =>
                    {
                        List<System.Net.Mail.Attachment> attachments = null;
                        if (attachPDF && attachmentPDFs.Count > 0)
                        {
                            attachments = attachmentPDFs
                                            .Select(pdf => new System.Net.Mail.Attachment(new MemoryStream(pdf.Item1), pdf.Item2))
                                            .ToList();
                        }

                        var body = $"You will find the Installation Agreement for {homeOwnerName} at {propertyAddress} using the link below: <br/> <br/> {documentsLinks} <br/>";
                        var to = disableSendingToCustomer ? salesRepEmailAddress : $"{salesRepEmailAddress};{homeOwnerEmailAddress}";

                        Mail.Library.SendEmail(to, ccEmails, $"Installation Agreement for {homeOwnerName} at {propertyAddress}", body, true, attachments);
                    });
                }


                return proposal;
            }
        }


        public Proposal SignProposal(Guid proposalDataId, DocumentSignRequest request)
        {
            //var json = new JavaScriptSerializer().Serialize(request);
            //if (json != null)
            //{
            //    ApiLogEntry apilog = new ApiLogEntry();
            //    apilog.Id = Guid.NewGuid();
            //    apilog.User = "/sign/proposal/service";
            //    apilog.Machine = Environment.MachineName;
            //    apilog.RequestContentType = "";
            //    apilog.RequestRouteTemplate = "";
            //    apilog.RequestRouteData = "";
            //    apilog.RequestIpAddress = "";
            //    apilog.RequestMethod = "";
            //    apilog.RequestHeaders = "";
            //    apilog.RequestTimestamp = DateTime.UtcNow;
            //    apilog.RequestUri = "";
            //    apilog.ResponseContentBody = "";
            //    apilog.RequestContentBody = json;

            //    using (var dc = new DataContext())
            //    {
            //        dc.ApiLogEntries.Add(apilog);
            //        dc.SaveChanges();
            //    }
            //}

            using (var dataContext = new DataContext())
            {
                var data = dataContext
                            .ProposalData
                            //.Include(pd => pd.FinancePlan.SolarSystem.Proposal.Property)
                            //.Include(pd => pd.FinancePlan.FinancePlanDefinition)
                            .FirstOrDefault(pd => pd.Guid == proposalDataId);

                if (data == null)
                {
                    throw new ApplicationException("Could not find Proposal Data!");
                }

                if (data.SignatureDate.HasValue)
                {
                    throw new ApplicationException("Proposal already signed!");
                }

                var financePlan = dataContext
                                    .FinancePlans
                                    .Include(fp => fp.SolarSystem.PowerConsumption)
                                    .Include(fp => fp.SolarSystem.Proposal.Property.Territory)
                                    .Include(fp => fp.SolarSystem.Proposal.Property.Appointments)
                                    .Include(fp => fp.SolarSystem.Proposal.Property.Appointments.Select(prop => prop.Assignee))
                                    .Include(fp => fp.SolarSystem.Proposal.Property.Appointments.Select(prop => prop.Creator))
                                    .Include(fp => fp.FinancePlanDefinition)
                                    .FirstOrDefault(fp => fp.Guid == data.FinancePlanID);

                if (financePlan == null)
                {
                    throw new ApplicationException("Could not find Proposal Data!");
                }

                var contractorID = request.ContractorID ?? data.ContractorID;

                request.UtilityInflationRate = request.UtilityInflationRate.HasValue ? request.UtilityInflationRate.Value / 100 : request.UtilityInflationRate;

                var utilityInflationRate = financePlan.Request.UtilityInflationRate;

                // if the server sends Utility Inflation Rate, and it's different than what we have
                // we update the request & the response objects
                if (request.UtilityInflationRate.HasValue
                    && Math.Abs(financePlan.Request.UtilityInflationRate - request.UtilityInflationRate.Value) > 0.00001)
                {
                    var finPlan = financePlan;
                    var req = finPlan.Request;
                    utilityInflationRate = req.UtilityInflationRate = request.UtilityInflationRate.Value;
                    req.IncludeMonthsInResponse = true;
                    var resp = _loanCalculator.Value.CalculateLoan(req, financePlan.FinancePlanDefinition);
                    finPlan.Request = req;
                    finPlan.Response = resp;
                }

                var reqObjectHasDataJSON = !string.IsNullOrWhiteSpace(request.ProposalDataJSON);

                if (reqObjectHasDataJSON)
                {
                    var currentObjectHasDataJSON = !string.IsNullOrWhiteSpace(data.ProposalDataJSON);

                    // if the object already contains a JSON, will merge the two
                    if (currentObjectHasDataJSON)
                    {
                        var mergeSettings = new JsonMergeSettings
                        {
                            MergeArrayHandling = MergeArrayHandling.Union
                        };

                        var propData = JObject.Parse(data.ProposalDataJSON);
                        var reqData = JObject.Parse(request.ProposalDataJSON);
                        propData.Merge(reqData, mergeSettings);

                        data.ProposalDataJSON = propData.ToString();
                    }
                    else
                    {
                        data.ProposalDataJSON = request.ProposalDataJSON;
                    }
                }

                if (request.Location?.HasValue() == true)
                {
                    var metaInfo = data.MetaInformation ?? new ProposalDataMetaInformation();
                    metaInfo.AcceptLocation = request.Location;
                    data.MetaInformation = metaInfo;
                }

                data.SignatureDate = SmartPrincipal.DeviceDate.LocalDateTime;

                if (request.UserInput != null)
                {
                    var existingUserInputLinks = data.UserInputLinks ?? new List<UserInputDataLinks>();

                    var newLinks = UploadUserInputData(request.UserInput, data);
                    if (newLinks?.Any() == true)
                    {
                        existingUserInputLinks.AddRange(newLinks);
                    }

                    data.UserInputLinks = existingUserInputLinks;
                }
                // save changes first, because the SignatureDate needs to have value when we generate the PDFs
                dataContext.SaveChanges();

                var proposal = financePlan.SolarSystem.Proposal;
                //List<OUSetting> ouSettings = GetOUSettingsForContractorID(contractorID);
                var ouSettings = OUSettingService.GetOuSettings(proposal.Property.Territory.OUID);
                List<SignedDocumentDTO> signedDocuments = null;
                if (!string.IsNullOrWhiteSpace(request.ProposalDataJSON))
                {
                    try
                    {
                        var proposalCustomData = JObject.Parse(request.ProposalDataJSON);

                        signedDocuments = proposalCustomData["SignedDocuments"].ToObject<List<SignedDocumentDTO>>();
                    }
                    catch (Exception) { }
                }

                var documentUrls = GetProposalURLs(contractorID, data.Guid, signedDocuments, ouSettings);
                var planName = financePlan.Name;


                float apr = financePlan.FinancePlanDefinition.Apr;
                float year = financePlan.FinancePlanDefinition.TermInYears;
                var FinanceProvider = dataContext
                            .FinanceProviders
                            .FirstOrDefault(fd => fd.Guid == financePlan.FinancePlanDefinition.ProviderID);


                var attachmentPDFs = new List<Tuple<byte[], string>>();
                documentUrls?
                        .ForEach(d =>
                        {

                            d.ProviderName = FinanceProvider?.Name;
                            d.Apr = apr;
                            d.Year = year;


                            d.Description = $"{d.Name} [{planName.AsFileName()}]";
                            d.ProposalDataID = proposalDataId;
                            if (d.Name == "Proposal")
                            {
                                d.EnergyBillUrl = data.UserInputLinks?.FirstOrDefault(lnk => lnk.Type == UserInputDataType.EnergyBill)?.ContentURL;
                            }

                            var pdfContent = _utilServices.Value.GetPDF(d.Name == "Proposal" ? d.Url + "?customizeproposal=1" : d.Url);

                            d.PDFUrl = _blobService.Value.UploadByNameGetFileUrl($"proposal-data/{data.Guid}/documents/{DateTime.UtcNow.Ticks}.pdf",
                                 new BlobModel
                                 {
                                     Content = pdfContent,
                                     ContentType = "application/pdf"
                                 }, BlobAccessRights.PublicRead);

                            attachmentPDFs.Add(new Tuple<byte[], string>(pdfContent, $"{d.Description}.pdf"));
                        });

                proposal.SignedDocumentsJSON = JsonConvert.SerializeObject(documentUrls);

                // update territory DateLastModified
                proposal.Property?.Territory?.Updated(SmartPrincipal.UserId);

                dataContext.SaveChanges();

                try
                {
                    PushProposalDataToNoSQL(request.FinancePlanID, data);
                }
                catch
                {
                }

                // Push the proposal to SB
                var response = _solarSalesTrackerAdapter.Value.AttachProposal(proposal, proposalDataId, documentUrls?.FirstOrDefault(d => d.Name == "Proposal"));
                if (response != null && response.Message.Type.Equals("error"))
                {
                    proposal.SBProposalError = response.Message.Text + ". This lead will not be saved in SMARTBoard until it's added.";
                }


                var pbi = new PBI_ProposalSigned
                {
                    ProposalID = proposal.Guid,
                    UtilityInflationRate = utilityInflationRate * 100
                };

                _powerBIBridge.Value.PushDataAsync(pbi);

                var sendEmail = ouSettings.GetValueAsBool(OUSetting.Proposal_Features_SendEmailOnSign, true);
                var attachPDF = ouSettings.GetValueAsBool(OUSetting.Proposal_Features_AttachPDFOnSign, true);

                if (sendEmail)
                {
                    // send the email
                    var property = proposal.Property;

                    var propertyAddress = proposal.GetPropertyAddress();

                    var salesRepEmailAddress = dataContext
                                        .People
                                        .Where(p => p.Guid == data.SalesRepID)
                                        .Select(p => p.EmailAddressString)
                                        .SingleOrDefault();

                    var disableSendingToCustomer = ouSettings?
                                        .FirstOrDefault(os => os.Name == OUSetting.Proposal_Features_SendEmailToCustomer_Disabled)?
                                        .Value == "1";

                    var homeOwnerName = property.Name;

                    var homeOwnerEmailAddress = dataContext
                                        .Fields
                                        .Where(f => f.DisplayName == "Email Address" && f.PropertyId == proposal.PropertyID)
                                        .Select(f => f.Value)
                                        .FirstOrDefault();

                    string ccEmails = ouSettings?.FirstOrDefault(os => os.Name == OUSetting.Proposal_Features_EmailsToCC)?.Value;

                    //var planName = financePlan.Name;

                    var documentsLinks = string.Join("<br/>", documentUrls.Select(pd => $"<a target='_blank' href='{pd.Url}'>{pd.Name} for {homeOwnerName}<a/>"));

                    Task.Factory.StartNew(() =>
                    {
                        var mediaItems = GetProposalMediaItemsAsShareableLinks(proposal.Guid);

                        List<System.Net.Mail.Attachment> attachments = null;
                        if (attachPDF && attachmentPDFs.Count > 0)
                        {
                            attachments = attachmentPDFs
                                            .Select(pdf => new System.Net.Mail.Attachment(new MemoryStream(pdf.Item1), pdf.Item2))
                                            .ToList();
                        }
                        var mediaItemLinks = string.Join("<br/>", mediaItems.Select(mi => $"<a target='_blank' href='{mi.Value}'>{mi.Key}</a>"));
                        var mediaItemsBody = mediaItems.Count == 0 ? "" : $"<br/>Attached documents and images: <br/>{mediaItemLinks}";

                        var body = $"You will find the proposal for {homeOwnerName} at {propertyAddress} [{planName}] using the link below: <br/> <br/> {documentsLinks} <br/>{mediaItemsBody}";
                        var to = disableSendingToCustomer ? salesRepEmailAddress : $"{salesRepEmailAddress};{homeOwnerEmailAddress}";

                        //Mail.Library.SendEmail(to, ccEmails, $"Signed Proposal for {homeOwnerName} at {propertyAddress}", body, true, attachments);
                        Mail.Library.SendEmail("hevin.android@gmail.com", ccEmails, $"Signed Proposal for {homeOwnerName} at {propertyAddress}", body, true, attachments);
                    });
                }
                //var isSolarSalesTrackerEnabled = ouSettings.GetValue<SSTSettings>(SolarTrackerResources.SettingName)?.Enabled?.IsTrue() == true;
                ////var isSolarSalesTrackerEnabled = ouSettings.FirstOrDefault(ous => ous.Name == SolarTrackerResources.OuSettings.Enabled)?.Value?.IsTrue() == true;

                //if (isSolarSalesTrackerEnabled)
                //{
                //    //  send proposal to SolarTracker
                //    Task.Factory.StartNew(() =>
                //    {
                //        _solarSalesTrackerAdapter.Value.SubmitSolarData(request.FinancePlanID);
                //    });
                //}

                return proposal;
            }
        }

        public void UpdateProposalDataJSON(Guid proposalDataId, string proposalDataJSON)
        {
            if (string.IsNullOrWhiteSpace(proposalDataJSON))
            {
                proposalDataJSON = null;
            }
            else
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject(proposalDataJSON);
                    if (obj == null)
                    {
                        throw new ArgumentException("Invalid proposal data JSON!");
                    }
                }
                catch
                {
                    throw new ArgumentException("Invalid proposal data JSON!");
                }
            }

            using (var uow = UnitOfWorkFactory())
            {
                var entity = uow
                        .Get<ProposalData>()
                        .FirstOrDefault(pd => pd.Guid == proposalDataId);

                if (entity == null)
                {
                    throw new KeyNotFoundException("Proposal data object not found!");
                }

                // TODO: might need to merge the jsons and not replace
                entity.ProposalDataJSON = proposalDataJSON;
                entity.Updated(SmartPrincipal.UserId);
                uow.SaveChanges();
            }
        }

        private List<UserInputDataLinks> UploadUserInputData(List<UserInputData> userInput, ProposalData proposalData)
        {
            if (userInput?.Any() != true) return null;

            var userInputLinks = new List<UserInputDataLinks>();
            foreach (var userInputData in userInput)
            {
                if (userInputData.Content == null)
                {
                    continue;
                }

                var img = userInputData.Content.ToImage();

                var thumbnail = img.GetResizedImageContent(Math.Min(img.Width, maxResolution), Math.Min(img.Height, maxResolution));
                var imgPath = $"proposal-data/{proposalData.Guid}/{userInputData.Guid}-{userInputData.Type}".ToLowerInvariant();

                var imageUrl = _blobService.Value.UploadByNameGetFileUrl(imgPath,
                        new BlobModel
                        {
                            Content = userInputData.Content,
                            ContentType = userInputData.GetContentType()
                        }, BlobAccessRights.PublicRead);
                var thumbnailUrl = _blobService.Value.UploadByNameGetFileUrl($"{imgPath}_thumb",
                        new BlobModel
                        {
                            Content = thumbnail,
                            ContentType = "image/jpeg"
                        }, BlobAccessRights.PublicRead);

                var userInputDataLink = new UserInputDataLinks(userInputData, imageUrl, thumbnailUrl);
                userInputLinks.Add(userInputDataLink);

                if (userInputData.ValidationContent == null || !userInputData.ValidationContent.Any())
                {
                    continue;
                }

                foreach (var validation in userInputData.ValidationContent)
                {
                    var validationImageUrl = _blobService.Value.UploadByNameGetFileUrl(
                        $"proposal-data/{proposalData.Guid}/userInput-{userInputData.Type}/validation/{Guid.NewGuid()}".ToLowerInvariant(),
                        new BlobModel
                        {
                            Content = validation,
                            ContentType = "image/png"
                        }, BlobAccessRights.PublicRead);

                    userInputDataLink.ValidationUrl.Add(validationImageUrl);
                }
            }

            return userInputLinks;
        }

        private List<DocumentDataLink> UploadDocumentData(List<DocumentData> documentData, ProposalData proposalData)
        {
            if (documentData?.Any() != true) return null;

            var documentDataLinks = new List<DocumentDataLink>();
            foreach (var docData in documentData.Where(d => d.ContentType == ContentType.Image))
            {
                var thumbnail = docData.Content.ToThumbnail(maxResolution, maxResolution);
                var documentUrl = _blobService.Value.UploadByNameGetFileUrl($"proposal-data/{proposalData.Guid}/{docData.Guid}".ToLowerInvariant(),
                        new BlobModel
                        {
                            Content = docData.Content,
                            ContentType = docData.GetContentType()
                        }, BlobAccessRights.PublicRead);
                var thumbnailUrl = _blobService.Value.UploadByNameGetFileUrl($"proposal-data/{proposalData.Guid}/{docData.Guid}_thumb".ToLowerInvariant(),
                        new BlobModel
                        {
                            Content = thumbnail.ToByteArray(),
                            ContentType = "image/jpeg"
                        }, BlobAccessRights.PublicRead);
                documentDataLinks.Add(new DocumentDataLink(docData, documentUrl, thumbnailUrl));
            }

            return documentDataLinks;
        }

        public Proposal2DataView GetProposalDataView(Guid proposalDataId, double? utilityInflationRate, bool roundAmounts = false)
        {
            using (var dataContext = new DataContext())
            {
                var proposalData = dataContext
                                    .ProposalData
                                    .FirstOrDefault(pd => pd.Guid == proposalDataId);

                if (proposalData == null)
                {
                    throw new ApplicationException("Proposal does not exist!");
                }

                Proposal proposal = null;
                FinancePlan financePlan = null;

                if (proposalData.UsesNoSQLAggregatedData)
                {
                    proposal = _noSqlDataService.Value.GetValue<Proposal>(proposalData.Guid.ToString());

                    if (proposal != null)
                    {
                        // fix some references after deserializing from DynamoDB
                        financePlan = proposal?.SolarSystem?.FinancePlans?.FirstOrDefault();
                        financePlan.SolarSystem = proposal?.SolarSystem;
                    }
                }

                if (proposal == null)
                {
                    proposal = dataContext.Proposal
                                               .Include(p => p.Property.Territory.OU)
                                               .Include(p => p.Property.Occupants)
                                               .Include(p => p.Property.PropertyBag)
                                               .Include(p => p.Tariff)
                                               .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Points))
                                               .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Edges))
                                               .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Panels))
                                               .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Obstructions))
                                               .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.SolarPanel))
                                               .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Inverter))
                                               .Include(p => p.SolarSystem.AdderItems)
                                               .Include(p => p.SolarSystem.PowerConsumption)
                                               .Include(p => p.SolarSystem.SystemProduction)
                                               .Include(p => p.SolarSystem.FinancePlans.Select(fp => fp.SolarSystem.Proposal.Tariff))
                                               .Include(p => p.SolarSystem.FinancePlans.Select(fp => fp.FinancePlanDefinition.Details))
                                               .Include(p => p.SolarSystem.FinancePlans.Select(fp => fp.FinancePlanDefinition.Provider))
                                               .FirstOrDefault(p => p.Guid == proposalData.ProposalID);

                    financePlan = proposal.SolarSystem.FinancePlans.FirstOrDefault();
                }

                if (proposal == null)
                {
                    throw new ApplicationException("Proposal data does not exist!");
                }


                var contractorSetting = dataContext
                                .OUSettings
                                .Include(ous => ous.OU)
                                .FirstOrDefault(ous => ous.Value == proposalData.ContractorID);

                proposal.SalesRep = dataContext
                                        .People
                                        .Include(p => p.PhoneNumbers)
                                        .FirstOrDefault(p => p.Guid == proposalData.SalesRepID);


                var request = financePlan.Request;
                var response = financePlan.Response;
                var financePlanDefinition = financePlan.FinancePlanDefinition;

                if (utilityInflationRate.HasValue)
                {
                    request.UtilityInflationRate = utilityInflationRate.Value / 100;
                }

                switch (financePlanDefinition.Type)
                {
                    default:
                    case FinancePlanType.Loan:
                        response = _loanCalculator.Value.CalculateLoan(request, financePlanDefinition);
                        break;
                    case FinancePlanType.Lease:
                        response = _loanCalculator.Value.CalculateLease(request);
                        break;
                }
                //response = _loanCalculator.Value.CalculateLoan(request, financePlanDefinition);

                var param = new ProposalDVConstructor
                {
                    Data = proposalData,
                    DealerName = contractorSetting?.OU?.Name ?? proposalData.ContractorID,
                    Proposal = proposal,
                    FinancePlan = financePlan,
                    Request = request,
                    Response = response,
                    UtilityInflationRate = utilityInflationRate,
                    DeviceDate = SmartPrincipal.DeviceDate.LocalDateTime,
                };

                var proposalDV = new Proposal2DataView(param, roundAmounts);

                // get the OU settings
                var settings = OUSettingService.GetOuSettings(proposal.Property.Territory.OUID);

                // Based on settings, add additional information to the proposal
                var summaryFeature = settings?.FirstOrDefault(sett => sett.Name == OUSetting.Proposal_Features_Summary);

                var logoUrl = settings?.FirstOrDefault(sett => sett.Name == OUSetting.LegionOULogoImageUrl)?.Value;
                var showLogo = settings?.FirstOrDefault(sett => sett.Name == OUSetting.LegionOUUseLogoInProposal)?.Value == "1";

                if (showLogo && !string.IsNullOrEmpty(logoUrl) && proposalDV.BasicInfo != null)
                {
                    proposalDV.BasicInfo.CompanyLogoUrl = logoUrl;
                }

                var proposalSettings = settings?.Where(s => s.Name.StartsWith("Proposal.", StringComparison.OrdinalIgnoreCase))?.ToList();

                if (proposalSettings != null)
                {
                    proposalDV.Settings = proposalDV.Settings ?? new Dictionary<string, string>();
                    foreach (var sett in proposalSettings)
                    {
                        proposalDV.Settings.Add(sett.Name, sett.Value);
                    }
                }

                if (summaryFeature?.Value == "1")
                {
                    proposalDV.Settings = proposalDV.Settings ?? new Dictionary<string, string>();
                    var trismartEnhancer = new TrismartProposalEnhancement(_loanCalculator.Value, _ouSettingService.Value);

                    trismartEnhancer.EnhanceProposalData(proposalDV, new TriSmartConstructor
                    {
                        UtilityInflationRate = utilityInflationRate / 100,
                        Data = proposalData,
                        FinancePlan = financePlan,
                        Proposal = proposal,
                        Settings = settings
                    });
                }

                // Add Sigora's enhancements.
                var sigoraEnhancer = new SigoraProposalEnhancer();
                sigoraEnhancer.EnhanceProposalData(proposalDV, param, roundAmounts);


                return proposalDV;
            }
        }

        public List<DocumentDataLink> GetProposalDocuments(Guid proposalID)
        {
            using (var uow = UnitOfWorkFactory())
            {
                return uow
                            .Get<ProposalData>()
                            .FirstOrDefault(pd => pd.ProposalID == proposalID)?
                            .DocumentDataLinks;
            }
        }

        public List<DocumentDataLink> GetAllProposalDocuments(Guid proposalID)
        {
            using (var uow = UnitOfWorkFactory())
            {
                return uow
                            .Get<ProposalData>()
                            .Where(pd => pd.ProposalID == proposalID && pd.DocumentDataLinksJSON != null)?
                            .ToList()?
                            .SelectMany(x => x.DocumentDataLinks)?
                            .ToList();
            }
        }

        public List<ProposalMediaItem> GetProposalMediaItems(Guid proposalID)
        {
            using (var uow = UnitOfWorkFactory())
            {
                return uow
                        .Get<ProposalMediaItem>()
                        .Where(pmi => pmi.ProposalID == proposalID && !pmi.IsDeleted)
                        .ToList();
            }
        }

        public List<Models.DataViews.KeyValue> GetProposalMediaItemsAsShareableLinks(Guid proposalID)
        {
            var mediaItems = GetProposalMediaItems(proposalID);

            return mediaItems
                        .Select(mi => new Models.DataViews.KeyValue
                        {
                            Key = mi.Name,
                            Value = _blobService.Value.GetFileURL(mi.Url, expirationDays: sharedMediaItemsExpirationDays)
                        })
                        .ToList();
        }


        public BlobModel GetProposalMediaItemContent(Guid proposalMediaID, bool thumb = false)
        {
            using (var uow = UnitOfWorkFactory())
            {
                var mediaItem = uow
                        .Get<ProposalMediaItem>()
                        .FirstOrDefault(pmi => pmi.Guid == proposalMediaID && !pmi.IsDeleted);

                if (mediaItem == null)
                {
                    return null;
                }

                return _blobService.Value.DownloadByName(mediaItem.GetAWSFileName());
            }
        }

        public string UploadProposalDoc(Guid propertyID, string DocId, ProposalMediaUploadRequest request)
        {
            try
            {
                string resp = "";
                using (var dataContext = new DataContext())
                {
                    var property = dataContext.Properties.Include(p => p.Territory).FirstOrDefault(pd => pd.Guid == propertyID);

                    if (property == null)
                    {
                        resp = "Please Add Property Data!";
                        return resp;

                    }
                    ProposalMediaItem proposalMediaItem = new ProposalMediaItem();

                    //using (var uow = UnitOfWorkFactory())
                    //{

                    proposalMediaItem = new ProposalMediaItem
                    {
                        ProposalID = property.Guid,
                        MimeType = request.ContentType,
                        MediaItemType = request.MediaItemType,
                        Name = request.Name
                    };
                    string thumbUrl = proposalMediaItem.BuildUrl();
                    var docUrl = _blobService.Value.UploadByNameGetFileUrl(thumbUrl,

                            new BlobModel
                            {

                                Content = request.Content,
                                ContentType = request.ContentType,
                            }, BlobAccessRights.Private);
                    proposalMediaItem.Url = docUrl;
                    //uow.Add(proposalMediaItem);
                    //uow.SaveChanges();
                    //  }
                    _solarSalesTrackerAdapter.Value.UploadDocumentItem(property, DocId, proposalMediaItem);
                    resp = "success";
                    return resp;
                }
            }

            catch (Exception ex)

            {
                return ex.Message;
            }
        }


        //public List<ProposalMediaItem> UploadProposalDocumentItem(Guid propertyID, string DocId, List<ProposalMediaUploadRequest> request)
        //{
        //    var json = JsonConvert.SerializeObject(request);

        //    ApiLogEntry apilog = new ApiLogEntry();

        //    if (json != null)
        //    {
        //        apilog.Id = Guid.NewGuid();
        //        apilog.User = SmartPrincipal.UserId.ToString();
        //        apilog.Machine = Environment.MachineName;
        //        apilog.RequestContentType = propertyID.ToString();
        //        apilog.RequestRouteTemplate = "";
        //        apilog.RequestRouteData = "";
        //        apilog.RequestIpAddress = "";
        //        apilog.RequestMethod = "UploadProposalMediaItem";
        //        apilog.RequestHeaders = "";
        //        apilog.RequestTimestamp = DateTime.UtcNow;
        //        apilog.RequestUri = json.ToString();
        //        apilog.ResponseContentBody = "";
        //        apilog.RequestContentBody = "";

        //        using (var dc = new DataContext())
        //        {
        //            dc.ApiLogEntries.Add(apilog);
        //            dc.SaveChanges();
        //        }
        //    }

        //    var result = new List<ProposalMediaItem>();

        //    using (var dataContext = new DataContext())
        //    {
        //        var data = dataContext
        //                    .Proposal
        //                    .FirstOrDefault(pd => pd.PropertyID == propertyID);


        //        if (data == null)
        //        {
        //            throw new ApplicationException("Could not find Proposal Data!");
        //        }

        //        ProposalMediaItem proposalMediaItem = new ProposalMediaItem();
        //        using (var uow = UnitOfWorkFactory())
        //        {
        //            foreach (var item in request)
        //            {
        //                proposalMediaItem = new ProposalMediaItem
        //                {
        //                    ProposalID = data.Guid,
        //                    Notes = item.Notes,
        //                    MimeType = item.ContentType,
        //                    MediaItemType = item.MediaItemType,
        //                    Name = item.Name
        //                };

        //                string thumbUrl = proposalMediaItem.BuildUrl();

        //                var docUrl = _blobService.Value.UploadByNameGetFileUrl(thumbUrl,
        //                        new BlobModel
        //                        {
        //                            Content = item.Content,
        //                            ContentType = item.ContentType,
        //                        }, BlobAccessRights.Private);

        //                proposalMediaItem.Url = docUrl;

        //                uow.Add(proposalMediaItem);

        //                result.Add(proposalMediaItem);
        //            }
        //            uow.SaveChanges();

        //        }

        //        var proposalData = dataContext
        //                   .ProposalData
        //                   .FirstOrDefault(pd => pd.ProposalID == data.Guid);

        //        var financePlan = dataContext
        //                            .FinancePlans
        //                            .Include(fp => fp.SolarSystem.PowerConsumption)
        //                            .Include(fp => fp.SolarSystem.Proposal.Property.Territory)
        //                            .Include(fp => fp.SolarSystem.Proposal.Property.Appointments)
        //                            .Include(fp => fp.SolarSystem.Proposal.Property.Appointments.Select(prop => prop.Assignee))
        //                            .Include(fp => fp.SolarSystem.Proposal.Property.Appointments.Select(prop => prop.Creator))
        //                            .FirstOrDefault(fp => fp.SolarSystemID == data.Guid);

        //        if (financePlan == null)
        //        {
        //            throw new ApplicationException("Could not find Proposal Data!");
        //        }

        //        var proposal = financePlan.SolarSystem.Proposal;

        //        _solarSalesTrackerAdapter.Value.UploadDocumentItem(proposal, DocId, proposalMediaItem);

        //        return result;
        //    }
        //}


        public List<ProposalMediaItem> UploadProposalMediaItem(Guid proposalID, List<ProposalMediaUploadRequest> request)
        {
            var result = new List<ProposalMediaItem>();

            using (var uow = UnitOfWorkFactory())
            {
                foreach (var item in request)
                {
                    var proposalMediaItem = new ProposalMediaItem
                    {
                        ProposalID = proposalID,
                        Notes = item.Notes,
                        MimeType = item.ContentType,
                        MediaItemType = item.MediaItemType,
                        Name = item.Name
                    };

                    var mediaItemUrl = proposalMediaItem.BuildUrl();
                    string thumbUrl = null;
                    if (proposalMediaItem.MediaItemType == ProposalMediaItemType.Image)
                    {
                        thumbUrl = proposalMediaItem.BuildThumbUrl();
                        var img = item.Content.ToImage();
                        var thumbnail = img.GetResizedImageContent(Math.Min(img.Width, ProposalMediaItemMaxResolution), Math.Min(img.Height, ProposalMediaItemMaxResolution));

                        if (!string.IsNullOrEmpty(thumbUrl))
                        {
                            var thumbnailUrl = _blobService.Value.UploadByNameGetFileUrl(thumbUrl,
                                    new BlobModel
                                    {
                                        Content = thumbnail,
                                        ContentType = item.ContentType,
                                    }, BlobAccessRights.Private);
                        }
                    }

                    var imageUrl = _blobService.Value.UploadByNameGetFileUrl(mediaItemUrl,
                            new BlobModel
                            {
                                Content = item.Content,
                                ContentType = item.ContentType,
                            }, BlobAccessRights.Private);

                    proposalMediaItem.Url = mediaItemUrl;
                    proposalMediaItem.ThumbUrl = thumbUrl;

                    uow.Add(proposalMediaItem);

                    result.Add(proposalMediaItem);
                }
                uow.SaveChanges();

                return result;
            }
        }

        public void CopyProposalMediaItems(Guid sourceProposalID, Guid destinationProposalID)
        {
            using (var uow = UnitOfWorkFactory())
            {
                var mediaItems = uow
                                .Get<ProposalMediaItem>()
                                .Where(pmi => pmi.ProposalID == sourceProposalID && !pmi.IsDeleted)
                                .ToList()
                                .Where(pmi => !string.IsNullOrWhiteSpace(pmi.Url) && !string.IsNullOrWhiteSpace(pmi.ThumbUrl))
                                .ToList();

                if (mediaItems.Count == 0)
                {
                    return;
                }

                foreach (var item in mediaItems)
                {
                    var newMediaItem = new ProposalMediaItem
                    {
                        ProposalID = destinationProposalID,
                        Notes = item.Notes,
                        MimeType = item.MimeType,
                        Name = item.Name,
                    };

                    var destinationUrl = newMediaItem.Url = newMediaItem.BuildUrl();
                    _blobService.Value.CopyItem(item.GetAWSFileName(), destinationUrl, BlobAccessRights.Private);

                    var originalThumbUrl = item.GetAWSFileName(true);

                    if (!string.IsNullOrWhiteSpace(originalThumbUrl))
                    {
                        var destinationThumbUrl = newMediaItem.BuildThumbUrl();
                        newMediaItem.ThumbUrl = destinationThumbUrl;
                        _blobService.Value.CopyItem(originalThumbUrl, destinationThumbUrl, BlobAccessRights.Private);
                    }

                    uow.Add(newMediaItem);
                }

                uow.SaveChanges();
            }
        }

        public void UpdateProposal(Guid proposalId, ProposalUpdateRequest request)
        {
            using (var uow = UnitOfWorkFactory())
            {
                var proposal = uow
                                .Get<Proposal>()
                                .Include(p => p.Property)
                                .Include(p => p.Property.Territory)
                                .Include(p => p.SolarSystem.RoofPlanes)
                                .FirstOrDefault(p => p.Guid == proposalId);

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    proposal.Name = request.Name;
                }
                var roofPlanes = proposal?.SolarSystem?.RoofPlanes;

                if (request.RoofPlanes != null && roofPlanes?.Any() == true)
                {
                    foreach (var plane in request.RoofPlanes)
                    {
                        var dbPlane = roofPlanes.FirstOrDefault(rp => rp.Guid == plane.Id);

                        if (dbPlane == null)
                        {
                            throw new ArgumentException($"Invalid RoofPlane Id: {plane.Id}");
                        }

                        dbPlane.Azimuth = plane.Azimuth ?? dbPlane.Azimuth;
                        dbPlane.Shading = plane.Shading ?? dbPlane.Shading;
                        dbPlane.Tilt = plane.Tilt ?? dbPlane.Tilt;
                    }
                }

                var tags = proposal.Tags ?? new List<string>();

                if (request.Tags?.Insert?.Any() == true)
                {
                    foreach (var tag in request.Tags.Insert)
                    {
                        if (!tags.Contains(tag))
                        {
                            tags.Add(tag);
                        }
                    }
                }

                if (request.Tags?.Delete?.Any() == true)
                {
                    foreach (var tag in request.Tags.Delete)
                    {
                        if (tags.Contains(tag))
                        {
                            tags.Remove(tag);
                        }
                    }
                }

                proposal.Tags = tags;

                HandleKeyValuesCrud(uow, request.KeyValues);

                proposal.Updated(null, $"ClientAPI: {SmartPrincipal.OuId}");

                //update territory DateLastModified
                proposal.Property?.Territory?.Updated(SmartPrincipal.UserId);

                uow.SaveChanges();
            }
        }

        public LoanResponse ReCalculateFinancing(FinancePlan financePlan)
        {
            switch (financePlan.FinancePlanType)
            {
                default:
                case FinancePlanType.Loan:
                    var data = _financePlanDefinitionService.Value.Get(financePlan.FinancePlanDefinitionID.Value, "Details");
                    return _loanCalculator.Value.CalculateLoan(financePlan.Request, data);
                case FinancePlanType.Lease:
                    return _loanCalculator.Value.CalculateLease(financePlan.Request);
            }
        }

        private void CloneProposalData(IUnitOfWork uow, Guid oldFinancePlanId, Guid newFinancePlanId, bool rebuildJSON)
        {
            //using (var uow = UnitOfWorkFactory())
            //{
            var needToDisposeUOW = uow == null;

            uow = uow ?? UnitOfWorkFactory();
            var item = uow
                            .Get<ProposalData>()
                            .FirstOrDefault(pd => pd.FinancePlanID == oldFinancePlanId);

            var newFinPlan = uow
                        .Get<FinancePlan>()
                        .FirstOrDefault(fp => fp.Guid == newFinancePlanId);

            var clone = item.Clone(newFinancePlanId);

            clone.UsesNoSQLAggregatedData = true;
            clone.ProposalID = newFinPlan.SolarSystemID;
            clone.FinancePlanID = newFinancePlanId;

            if (rebuildJSON)
            {
                PushProposalDataToNoSQL(newFinancePlanId, clone);
            }
            else
            {
                if (item.UsesNoSQLAggregatedData)
                {
                    var proposal = _noSqlDataService.Value.GetValue<Proposal>(item.Guid.ToString());
                    proposal.Guid = clone.Guid;
                    _noSqlDataService.Value.PutValue(proposal);
                }
            }

            uow.Add(clone);

            if (needToDisposeUOW)
            {
                uow.SaveChanges();
                uow.Dispose();
            }
        }

        public void CloneProposal(ProposalCloneCoreParams req)
        {
            using (var uow = UnitOfWorkFactory())
            {
                if (req == null)
                {
                    return;
                }

                if (req.ProposalDataGuids?.Any() == true)
                {
                    foreach (var item in req.ProposalDataGuids)
                    {
                        CloneProposalData(uow, item.Key, item.Value, true);
                    }
                }

                HandleKeyValuesCrud(uow, req.KeyValues);
                uow.SaveChanges();
            }
        }

        private void HandleKeyValuesCrud(IUnitOfWork uow, KeyValuesCrud keyValues)
        {
            if (keyValues == null)
                return;

            var needToDisposeUOW = uow == null;

            uow = uow ?? UnitOfWorkFactory();

            if (keyValues.Insert?.Any() == true)
            {
                foreach (var keyValue in keyValues.Insert)
                {
                    uow.Add(keyValue);
                }
            }

            if (keyValues.Update?.Any() == true)
            {
                var ids = keyValues.Update.Select(kv => kv.Guid).ToList();
                var entities = uow.Get<Models.KeyValue>().Where(kv => ids.Contains(kv.Guid)).ToList();

                foreach (var keyValue in keyValues.Update)
                {
                    var entity = entities.FirstOrDefault(e => e.Guid == keyValue.Guid);
                    if (entity == null)
                    {
                        throw new ApplicationException($"KeyValue object to update with Guid: {keyValue.Guid} does not exist!");
                    }
                    entity.ObjectID = keyValue.ObjectID;
                    entity.Key = keyValue.Key;
                    entity.Value = keyValue.Value;
                    entity.Notes = keyValue.Notes;
                    entity.Updated(null, $"ClientAPI: {SmartPrincipal.OuId}");
                }
            }

            if (keyValues.Delete?.Any() == true)
            {
                var keyValuesToDelete = uow
                                        .Get<Models.KeyValue>()
                                        .Where(kv => keyValues.Delete.Contains(kv.Guid))
                                        .ToList();
                uow.DeleteMany(keyValuesToDelete);
            }

            if (needToDisposeUOW)
            {
                uow.SaveChanges();
                uow.Dispose();
            }



        }


        public List<DocType> GetDocumentType()
        {

            List<DocType> typeList = new List<DocType>();

            typeList.Add(new DocType() { Id = 1, Name = "Proposal" });
            typeList.Add(new DocType() { Id = 2, Name = "Contract" });
            typeList.Add(new DocType() { Id = 3, Name = "Reference" });
            typeList.Add(new DocType() { Id = 4, Name = "Design" });

            return typeList;
        }

        public List<Models.DataViews.KeyValue> GetAddersIncentives(Guid ProposalID)
        {
            using (var dc = new DataContext())
            {

                var Proposalsdata = dc.ProposalData.FirstOrDefault(x => x.Guid == ProposalID);
                if (Proposalsdata == null)
                {
                    throw new Exception("Proposal not found");
                }

                var Proposal = dc.Proposal.FirstOrDefault(x => x.Guid == ProposalID || x.Guid == Proposalsdata.ProposalID);
                if (Proposal == null)
                {
                    throw new Exception("Proposal not found");
                }

                var property = dc.Properties.FirstOrDefault(x => !x.IsDeleted && !x.IsArchive && x.Guid == Proposal.PropertyID);
                if (property == null)
                {
                    throw new Exception("Property not found");
                }

                var Territory = dc.Territories.FirstOrDefault(x => x.Guid == property.TerritoryID);
                if (Territory == null)
                {
                    throw new Exception("Territory not found");
                }


                return dc.OUSettings.Where(x => x.OUID == Territory.OUID && (x.Name == "Adders" || x.Name == "Incentives")).Select(mi => new Models.DataViews.KeyValue
                {
                    Key = mi.Name,
                    Value = mi.Value
                }).ToList();
            }
        }

        public SystemCostItem AddAddersIncentives(AdderItem adderItem, Guid ProposalID)
        {
            using (var dataContext = new DataContext())
            {
                var existingProposal = dataContext.ProposalData.FirstOrDefault(i => i.Guid == ProposalID);
                if (existingProposal == null)
                {
                    throw new Exception("Data Not Found");
                }

                var existingadderItem = dataContext.AdderItems.FirstOrDefault(i => i.Type == adderItem.Type && i.Name == adderItem.Name && i.Cost == adderItem.Cost && i.SolarSystemID == ProposalID && i.TemplateID == adderItem.Guid);

                if (existingadderItem != null)
                {
                    throw new Exception("Already added");
                }

                var solarSystem = dataContext.SolarSystem.FirstOrDefault(i => i.Guid == existingProposal.ProposalID);
                if (solarSystem == null)
                {
                    throw new Exception("Solar System not found");
                }

                adderItem = AdderItem.ToDbModel(adderItem, existingProposal.ProposalID);
                dataContext.AdderItems.Add(adderItem);
                dataContext.SaveChanges();

                if (existingProposal.UsesNoSQLAggregatedData == true)
                {
                    PushProposalDataToNoSQL(existingProposal.FinancePlanID, existingProposal);
                }

                var data = new SystemCostItem(adderItem, solarSystem.SystemSize, false);
                return data;
            }
        }

        public SystemCostItem UpdateQuantityAddersIncentives(AdderItem adderItem, Guid ProposalID)
        {
            using (var dataContext = new DataContext())
            {
                var existingProposal = dataContext.ProposalData.FirstOrDefault(i => i.Guid == ProposalID);
                if (existingProposal == null)
                {
                    throw new Exception("Data Not Found");
                }

                var existingadderItem = dataContext.AdderItems.FirstOrDefault(i => i.Guid == adderItem.Guid);

                if (existingadderItem == null)
                {
                    throw new Exception("Data not found");
                }

                existingadderItem.Quantity = adderItem.Quantity == 0 ? 1 : adderItem.Quantity;
                dataContext.SaveChanges();

                if (existingProposal.UsesNoSQLAggregatedData == true)
                {
                    PushProposalDataToNoSQL(existingProposal.FinancePlanID, existingProposal);
                }

                var solarSystem = dataContext.SolarSystem.FirstOrDefault(i => i.Guid == existingadderItem.SolarSystemID);
                if (solarSystem == null)
                {
                    throw new Exception("Solar System not found");
                }

                var data = new SystemCostItem(existingadderItem, solarSystem.SystemSize, false);
                return data;
            }
        }

        public void UpdateExcludeProposalData(string excludeProposalJSON, Guid ProposalID)
        {
            using (var dataContext = new DataContext())
            {
                var existing = dataContext.ProposalData.FirstOrDefault(i => i.Guid == ProposalID);

                if (existing == null)
                {
                    throw new Exception("Data not found");
                }

                existing.excludeProposalJSON = excludeProposalJSON;
                dataContext.SaveChanges();
            }
        }

        public void DeleteAddersIncentives(Guid adderID, Guid ProposalID)
        {
            using (var dataContext = new DataContext())
            {
                dataContext
               .AdderItems
               .Where(pc => pc.Guid == adderID)
               .Delete();

                dataContext.SaveChanges();

                var existingProposal = dataContext.ProposalData.FirstOrDefault(i => i.Guid == ProposalID);
                if (existingProposal == null)
                {
                    throw new Exception("Data Not Found");
                }

                if (existingProposal.UsesNoSQLAggregatedData == true)
                {
                    PushProposalDataToNoSQL(existingProposal.FinancePlanID, existingProposal);
                }
            }
        }

        public void UpdateProposalFinancePlan(Guid ProposalID, FinancePlan financePlan)
        {
            using (var dataContext = new DataContext())
            {
                var existingProposal = dataContext.ProposalData.FirstOrDefault(i => i.Guid == ProposalID);

                if (existingProposal == null)
                {
                    throw new Exception("Data not found");
                }

                var existingFinancePlan = dataContext.FinancePlans.FirstOrDefault(i => i.Guid == existingProposal.FinancePlanID);

                if (existingFinancePlan == null)
                {
                    throw new Exception("Plan not found");
                }

                var FinancePlanDefination = dataContext.FinancePlaneDefinitions.FirstOrDefault(i => i.Guid == financePlan.FinancePlanDefinitionID);

                if (FinancePlanDefination == null)
                {
                    throw new Exception("Plan not found");
                }

                var result = JsonConvert.DeserializeObject<LoanRequest>(existingFinancePlan.RequestJSON);
                result.FinancePlanData = FinancePlanDefination.MetaDataJSON;

                existingFinancePlan.RequestJSON = JsonConvert.SerializeObject(result);
                existingFinancePlan.ResponseJSON = financePlan.ResponseJSON;
                existingFinancePlan.Name = financePlan.Name;
                existingFinancePlan.FinancePlanType = financePlan.FinancePlanType;
                existingFinancePlan.FinancePlanDefinitionID = financePlan.FinancePlanDefinitionID;

                dataContext.SaveChanges();

                if (existingProposal.UsesNoSQLAggregatedData == true)
                {
                    PushProposalDataToNoSQL(existingProposal.FinancePlanID, existingProposal);
                }


            }
        }

    }
}
