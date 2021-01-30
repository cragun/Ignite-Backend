using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.ClientApi.Models;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DTOs.Blobs;
using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Dynamic;
using System.Drawing;
using DataReef.Imaging.Renderings;
using DataReef.Core.Extensions;
using Newtonsoft.Json;
using DataReef.TM.Models.DTOs.Proposals;
using DataReef.TM.Models.DTOs.Solar.Proposal;
using DataReef.TM.Models.DataViews.ClientAPI;

namespace DataReef.TM.ClientApi.Controllers
{
    /// <summary>
    /// Proposals
    /// </summary>
    [RoutePrefix("proposals")]
    public class ProposalsController : ApiController
    {

        private IProposalService propService;
        private IOUService ouService;
        private IPersonService personService;
        private IKeyValueService keyValueService;
        private IOUSettingService ouSettingService;
        private Lazy<IProposalIntegrationAuditService> proposalIntegrationAuditService;

        public ProposalsController(IProposalService propService,
            IOUService ouService,
            IPersonService personService,
            IKeyValueService keyValueService,
            IOUSettingService ouSettingService,
            Lazy<IProposalIntegrationAuditService> proposalIntegrationAuditService)
        {
            this.propService = propService;
            this.ouService = ouService;
            this.personService = personService;
            this.keyValueService = keyValueService;
            this.ouSettingService = ouSettingService;
            this.proposalIntegrationAuditService = proposalIntegrationAuditService;
        }

        /// <summary>
        /// Get proposals
        /// </summary>
        /// <param name="ouid">Optional organization id</param>
        /// <param name="startDate">Optional start date filter</param>
        /// <param name="endDate">Optional end date filter</param>
        /// <param name="pageNumber">Page number, default 1</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")] // this will not work if the request is NOT authenticated
        [ResponseType(typeof(GenericResponse<List<ProposalLite>>))]
        public IHttpActionResult GetProposals(Guid? ouid = null, System.DateTime? startDate = null, System.DateTime? endDate = null, int pageNumber = 1)
        {

            if (ouid.HasValue)
            {
                var guidList = new List<Guid>() { SmartPrincipal.OuId };
                var ouGuids = this.ouService.GetHierarchicalOrganizationGuids(guidList);

                if (!ouGuids.Contains(ouid.Value))
                {
                    return NotFound();
                }
            }

            Guid rootOUID = ouid.HasValue ? ouid.Value : SmartPrincipal.OuId;

            if (!startDate.HasValue) startDate = new DateTime(2010, 1, 1);
            if (!endDate.HasValue) endDate = new DateTime(2020, 12, 31);

            List<ProposalLite> content = this.propService.GetProposalsLite(rootOUID, startDate.Value, endDate.Value, true, pageNumber, 1000).ToList();

            var ret = new GenericResponse<List<ProposalLite>>(content);
            return Ok(ret);

        }

        /// <summary>
        /// Get proposal
        /// </summary>
        /// <param name="proposalID">The proposal id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{proposalID:guid}")] // this will not work if the request is NOT authenticated
        [ResponseType(typeof(GenericResponse<ProposalUnformattedDataView>))]
        public IHttpActionResult GetProposal(Guid proposalID)
        {

            var guidList = new List<Guid>() { SmartPrincipal.OuId };
            var ouGuids = this.ouService.GetHierarchicalOrganizationGuids(guidList);

            Proposal prop = propService.Get(proposalID,
                "SolarSystem.FinancePlans,Property.Occupants,Property.Territory.OU,Property.PropertyBag,Tariff,SolarSystem.RoofPlanes.Points,SolarSystem.RoofPlanes.Edges,SolarSystem.RoofPlanes.Panels,SolarSystem.RoofPlanes.Obstructions,SolarSystem.RoofPlanes.SolarPanel,SolarSystem.RoofPlanes.Inverter,SolarSystem.PowerConsumption,SolarSystem.SystemProduction,SolarSystem.AdderItems,MediaItems").Result;


            if (prop == null)
            {
                return NotFound();
            }
            if (prop.SolarSystem == null)
            {
                return Content(HttpStatusCode.ExpectationFailed, "Solar system information is missing!");
            }
            //if (prop.SolarSystem.FinancePlans == null || prop.SolarSystem.FinancePlans.Count == 0)
            //{
            //    return Content(HttpStatusCode.ExpectationFailed, "No proposal has been generated!");
            //}
            if (!ouGuids.Contains(prop.Property.Territory.OUID))
            {
                return NotFound();
            }

            //get keyValues
            var keyValues = this.keyValueService.List(filter: $"ObjectID={proposalID}", itemsPerPage: 100);

            var settings = ouSettingService.GetSettingsByOUID(prop.Property.Territory.OUID);
            var integrationAudits = proposalIntegrationAuditService.Value.List(filter: $"ProposalID={proposalID}");

            var ctorParam = new ProposalUnformattedDataViewConstructor
            {
                Proposal = prop,
                FinancePlan = prop.SolarSystem.FinancePlans?.FirstOrDefault(),
                KeyValues = keyValues,
                Settings = settings,
                IntegrationAudits = integrationAudits
            };
            //transform into a user friendly model ( DataView )
            var dataView = new ProposalUnformattedDataView(ctorParam);

            var ret = new GenericResponse<ProposalUnformattedDataView>(dataView);
            return Ok(ret);
        }


        /// <summary>
        /// Update the proposal
        /// </summary>
        /// <param name="proposalID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{proposalID:guid}/update")]
        public IHttpActionResult UpdateProposal(Guid proposalID, [FromBody]ProposalUpdateRequest request)
        {
            try
            {
                propService.UpdateProposal(proposalID, request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get the Proposal Attachment's content by AttachmentID
        /// </summary>
        /// <param name="proposalAttachmentID">Proposal Attachment ID</param>
        /// <param name="thumb">True for thumbnail, false for full resolution. Default is false.</param>
        /// <returns></returns>
        [Route("attachments/{proposalAttachmentID:guid}/{thumb?}")]
        [ResponseType(typeof(byte[]))]
        [HttpGet]
        public HttpResponseMessage GetMediaItem(Guid proposalAttachmentID, bool thumb = false)
        {
            var blob = propService.GetProposalMediaItemContent(proposalAttachmentID, thumb);
            var ms = new MemoryStream(blob.Content);
            HttpResponseMessage response = new HttpResponseMessage
            {
                Content = new StreamContent(ms),
                StatusCode = HttpStatusCode.OK
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(blob.ContentType);
            response.Content.Headers.ContentLength = blob.Content.Length;
            return response;
        }

        /// <summary>
        /// Get roof rendering
        /// </summary>
        /// <param name="proposalID">The proposal id</param>
        /// <param name="view">The view, default "wireframe"</param>
        /// <param name="width">The width, default 200</param>
        /// <param name="height">The height, default 200</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{proposalID:guid}/roof")]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage GetRoofRendering(Guid proposalID, string view = "wireframe", int width = 200, int height = 200)
        {

            if (width < 100) width = 100;
            if (width > 2000) width = 2000;
            if (height < 100) height = 100;
            if (height > 2000) height = 2000;

            RenderStyle style = RenderStyle.WireFrame;

            try
            {
                style = (RenderStyle)Enum.Parse(typeof(RenderStyle), view);
            }
            catch (Exception)
            {
            }

            if (style == RenderStyle.SiteMap)
            {
                var docs = propService.GetProposalDocuments(proposalID);
                var roofTopImageUrl = docs?
                                        .FirstOrDefault(d => d.Type == DocumentDataType.SolarSystem)?
                                        .ContentURL;

                if (!string.IsNullOrWhiteSpace(roofTopImageUrl))
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            // download the image from S3
                            var content = client.DownloadData(roofTopImageUrl);
                            // resize it to the given width & height
                            content = content.GetResizedContent(width, height);

                            var result = new HttpResponseMessage(HttpStatusCode.OK);
                            result.Content = new ByteArrayContent(content);
                            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                            return result;
                        }
                    }
                    catch
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Error retrieving the rooftop image.");
                    }
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, "Rooftop image not found.");
            }

            var guidList = new List<Guid>() { SmartPrincipal.OuId };
            var ouGuids = this.ouService.GetHierarchicalOrganizationGuids(guidList);

            Proposal prop = propService.Get(proposalID, "SolarSystem,Property.Territory").Result;

            string json = prop.SolarSystem.SystemJSON;

            if (prop == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid ProposalID");
            }
            else if (prop.SolarSystem == null)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, "Incomplete Solar Design - No System");
            }
            else if (!ouGuids.Contains(prop.Property.Territory.OUID))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Access Denied");
            }
            else if (string.IsNullOrWhiteSpace(json))
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, "Incomplete Solar Design - No SystemJson");
            }

            RenderingData renderingData = new RenderingData(width, height);
            renderingData.LoadFromJson(json);

            var img = DesignRenderer.RenderDesign(renderingData, width, height, style);

            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(ms.ToArray());
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                return result;
            }


        }

        /// <summary>
        /// Clones and modifies an existing proposal
        /// </summary>
        /// <param name="proposalID">The proposal id</param>
        /// <param name="request">The proposal details to be modified</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{proposalID:guid}/clone")]
        [ResponseType(typeof(GenericResponse<CloneProposalResponse>))]
        public IHttpActionResult CloneProposal(Guid proposalID, ProposalCloneRequest request)
        {
            try
            {
                //data validation
                if (request == null)
                {
                    return Content(HttpStatusCode.PreconditionFailed, "Missing ProposalCloneRequest");
                }
                else if (proposalID == Guid.Empty)
                {
                    return Content(HttpStatusCode.PreconditionFailed, "Missing Proposal Guid");
                }
                else if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return Content(HttpStatusCode.PreconditionFailed, "Must provide a Name for the new proposal");
                }


                var guidList = new List<Guid>() { SmartPrincipal.OuId };
                var ouGuids = this.ouService.GetHierarchicalOrganizationGuids(guidList);

                //first get the existing proposal
                Proposal existingProposal = propService.Get(proposalID,
                    "SolarSystem.FinancePlans.Documents,SolarSystem.RoofPlanes.Obstructions.ObstructionPoints,SolarSystem.SystemProduction.Months,Property.Territory,SolarSystem.FinancePlans,Tariff,SolarSystem.RoofPlanes.Points,SolarSystem.RoofPlanes.Edges,SolarSystem.RoofPlanes.Panels,SolarSystem.RoofPlanes.Obstructions,SolarSystem.RoofPlanes.SolarPanel,SolarSystem.RoofPlanes.Inverter,SolarSystem.PowerConsumption,SolarSystem.SystemProduction,SolarSystem.AdderItems").Result;


                //business logic and security checks. 
                if (existingProposal == null)
                {
                    return Content(HttpStatusCode.NotFound, "Invalid ProposalID");
                }
                else if (existingProposal.SolarSystem == null)
                {
                    return Content(HttpStatusCode.ExpectationFailed, "Incomplete Solar Design - No System");
                }
                else if (!ouGuids.Contains(existingProposal.Property.Territory.OUID))
                {
                    return Content(HttpStatusCode.Forbidden, "Access Denied");
                }

                //do not get any keyvalues. they wont transfer

                //next update the existing data

                if (request.RoofPlanes != null)
                {
                    foreach (var roofPlane in request.RoofPlanes)
                    {
                        var existingRoofPlane = existingProposal.SolarSystem.RoofPlanes.FirstOrDefault(rp => rp.Guid == roofPlane.RoofPlaneID);
                        if (existingRoofPlane == null)//user sent us a roof plane that does not exist.  fail
                        {
                            return Content(HttpStatusCode.NotFound, $"Roof Plane {roofPlane.RoofPlaneID} does not exist in Proposal {proposalID}.");
                        }

                        //update the Tilt.  This is strange. We store the "Tilt" as the angle of the roof.  The "Pitch" is the Tan of the Radians of the Tilt

                        existingRoofPlane.Shading = roofPlane.Shading ?? existingRoofPlane.Shading;
                        existingRoofPlane.Tilt = roofPlane.Tilt ?? existingRoofPlane.Tilt;
                        existingRoofPlane.Pitch = roofPlane.Pitch ?? existingRoofPlane.Pitch;
                        existingRoofPlane.Azimuth = roofPlane.Azimuth ?? existingRoofPlane.Azimuth;
                    }
                }

                //next if tags are given, replace them
                if (request.Tags != null && request.Tags.Any())
                {
                    existingProposal.Tags = request.Tags;
                }

                List<AdderItem> newAddersCollection = null;

                //merge adders
                if (request.Adders != null)
                {

                    newAddersCollection = new List<AdderItem>(existingProposal.SolarSystem.AdderItems);


                    //verify that all udpate adders are NOT an empty guid
                    foreach (var updateAdder in request.Adders.Update)
                    {
                        if (updateAdder.AdderId == Guid.Empty)
                        {
                            return Content(HttpStatusCode.ExpectationFailed, $"You have an Update Adder without a valid Guid - an Empty Guid. All updates must match a valid AdderID ( Guid )");
                        }
                    }

                    //verify that all insert adders are an empty guid
                    foreach (var insertAdder in request.Adders.Insert)
                    {
                        if (insertAdder.AdderId != Guid.Empty)
                        {
                            return Content(HttpStatusCode.ExpectationFailed, $"You have supplied an Insert adder Guid {insertAdder.AdderId.ToString()} in the Adders.Insert section.  All adder inserts must omit the AdderID as Ignite API will create the Guid for you.");
                        }
                    }


                    //remove any adders to delete
                    foreach (var deleteGuid in request.Adders.Delete)
                    {
                        var existingAdder = newAddersCollection.FirstOrDefault(aa => aa.Guid == deleteGuid);
                        if (existingAdder == null) //if an adder guid is provided, it must exist or whole transaction will fail
                        {
                            return Content(HttpStatusCode.NotFound, $"Delete Adder  {deleteGuid} does not exist in Proposal {proposalID}.");
                        }
                        else
                        {

                            newAddersCollection.Remove(existingAdder);
                        }
                    }


                    foreach (var adder in request.Adders.Update)
                    {
                        var existingAdder = newAddersCollection.FirstOrDefault(aa => aa.Guid == adder.AdderId);
                        if (existingAdder == null) //if an adder guid is provided, it must exist or whole transaction will fail
                        {
                            return Content(HttpStatusCode.NotFound, $"Adder {adder.AdderId} does not exist in Proposal {proposalID}.");
                        }
                        else
                        {
                            //can only update a few properties for now- maybe for ever?
                            if (adder.Cost.HasValue) existingAdder.Cost = adder.Cost.Value;
                            if (adder.IsPaidBySalesPerson.HasValue) existingAdder.CanBePaidForByRep = adder.IsPaidBySalesPerson.Value;
                            if (adder.Quantity.HasValue) existingAdder.Quantity = adder.Quantity.Value;
                            if (!string.IsNullOrWhiteSpace(adder.Name)) existingAdder.Name = adder.Name;
                        }
                    }

                    List<AdderItem> adderTemplates = new List<AdderItem>();

                    //if there are any insert adders, we have to get the adder tempates
                    if (request.Adders != null && request.Adders.Insert.Any())
                    {
                        var settings = ouSettingService.GetSettings(existingProposal.OUID.Value, null).Result;
                        var adderSetting = settings.FirstOrDefault(s => s.Key == "Adders");
                        var json = adderSetting.Value;
                        adderTemplates = JsonConvert.DeserializeObject<List<AdderItem>>(json.Value);
                    }


                    foreach (var adder in request.Adders.Insert)
                    {

                        if (adder.TemplateID == Guid.Empty) return Content(HttpStatusCode.ExpectationFailed, $"Insert Adder {adder.AdderId} is missing a vaid TemplateID.  This is the Guid of the Adder.  Call the GET adders to receive to the adders for the given Organization ");

                        var template = adderTemplates.FirstOrDefault(tt => tt.Guid == adder.TemplateID);

                        if (template == null)
                        {
                            return Content(HttpStatusCode.ExpectationFailed, $"Insert Adder {adder.AdderId} has a TemplateID {adder.TemplateID} which is not found in the adders for this organization: {existingProposal.OUID}.");
                        }

                        var newAdder = template;
                        newAdder.SolarSystemID = existingProposal.Guid;

                        if (adder.Cost.HasValue) newAdder.Cost = adder.Cost.Value;
                        if (adder.IsPaidBySalesPerson.HasValue) newAdder.CanBePaidForByRep = adder.IsPaidBySalesPerson.Value;
                        if (adder.Quantity.HasValue) newAdder.Quantity = adder.Quantity.Value;
                        if (!string.IsNullOrWhiteSpace(adder.Name)) newAdder.Name = adder.Name;
                        newAddersCollection.Add(newAdder);
                    }
                }

                // If tre request overrides the price per kWh, clone the FinancePlan and replace it w/ new values
                if ((request.ProductionCosts?.Count ?? 0) > 0 && existingProposal.SolarSystem.FinancePlans != null)
                {
                    foreach (var finPlan in existingProposal.SolarSystem.FinancePlans)
                    {
                        var req = finPlan.Request;
                        req.ProductionCosts = request.ProductionCosts;
                        finPlan.Request = req;
                        finPlan.Response = propService.ReCalculateFinancing(finPlan);
                    }
                }

                // Set the new System Months if present in the Request
                if ((request.SystemMonths?.Count ?? 0) > 0)
                {
                    existingProposal.SolarSystem.PowerConsumption = request
                                                .SystemMonths
                                                .Select(sm => new SolarSystemPowerConsumption
                                                {
                                                    Year = sm.Year,
                                                    Month = sm.Month,
                                                    CostSource = sm.CostSource,
                                                    UsageSource = sm.UsageSource,
                                                    Watts = sm.Watts,
                                                    Cost = sm.Cost,
                                                })
                                                .ToList();
                }

                //clone it
                existingProposal.SolarSystem.AdderItems = newAddersCollection;

                var cloneSettings = new CloneSettings();

                var newProposal = existingProposal.Clone(request.Name, cloneSettings);

                //save it
                try
                {
                    var savedProposal = this.propService.Insert(newProposal);

                    // Clone ProposalData (after the new proposal has been saved to DB)
                    var oldFinancePlanIDs = existingProposal
                            .SolarSystem
                            .FinancePlans
                            .Select(fp => fp.Guid)
                            .ToList();

                    var newFinancePlanIDs = newProposal
                                            .SolarSystem
                                            .FinancePlans
                                            .Select(fp => fp.Guid)
                                            .ToList();

                    var cloneParams = new ProposalCloneCoreParams
                    {
                        ProposalDataGuids = oldFinancePlanIDs.Select((of, idx) => new KeyValuePair<Guid, Guid>(of, newFinancePlanIDs[idx])).ToList(),
                        KeyValues = request.KeyValues,
                    };
                    propService.CloneProposal(cloneParams);

                    //transform into a user friendly model ( DataView )

                    CloneProposalResponse response = new Models.CloneProposalResponse();
                    response.NewProposalID = newProposal.Guid;
                    var ret = new GenericResponse<CloneProposalResponse>(response);
                    return Ok(ret);
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.Message == "Sequence contains no elements")
                    {
                        //todo:
                        //it really did save. there is some bug in the core we need to debug

                        CloneProposalResponse response = new Models.CloneProposalResponse();
                        response.NewProposalID = newProposal.Guid;
                        var ret = new GenericResponse<CloneProposalResponse>(response);
                        return Ok(ret);
                    }
                    else
                    {
                        throw ex;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get the proposal as pdf
        /// </summary>
        /// <param name="proposalID">The proposal id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{proposalID:guid}/pdf")] // this will not work if the request is NOT authenticated
        public HttpResponseMessage GetProposalPDF(Guid proposalID)
        {

            var guidList = new List<Guid>() { SmartPrincipal.OuId };
            var ouGuids = this.ouService.GetHierarchicalOrganizationGuids(guidList);

            Proposal prop = propService.Get(proposalID).Result;

            if (prop == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid ProposalID");
            }
            else if (prop.SolarSystem == null)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, "Incomplete Solar Design - No System");
            }
            else if (prop.SolarSystem.FinancePlans == null || prop.SolarSystem.FinancePlans.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed, "No Financing Plan Associated With Proposal");
            }
            else if (!ouGuids.Contains(prop.Property.Territory.OUID))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Access Denied");
            }

            //get the documents from Hancock
            DataReef.Integrations.Hancock.IntegrationProvider hancock = new Integrations.Hancock.IntegrationProvider();
            var docs = hancock.GetExecutedDocumentsForProposalID(proposalID);

            if (docs == null || docs.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "No Executed Documents");
            }

            var doc = docs.FirstOrDefault(dd => dd.DocumentType.ToLower() == "proposal");

            if (doc == null || doc.Document == null || doc.Document.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "No Executed Proposal Document");
            }

            byte[] data = System.Convert.FromBase64String(doc.Document);


            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new MemoryStream(data);
            stream.Position = 0;

            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/pdf");
            //  new MediaTypeHeaderValue("application/octet-stream");
            return result;

        }
    }
}