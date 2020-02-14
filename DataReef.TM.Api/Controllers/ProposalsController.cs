using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Api.Classes.Infrastructure;
using DataReef.TM.Api.Classes.Requests;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.Proposals;
using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models.DTOs.Signatures.Proposals;
using DataReef.TM.Models.Solar;
//using GoogleMaps.LocationServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/proposals")]
    public class ProposalsController : EntityCrudController<Proposal>
    {
        private IProposalService _proposalService;
        private Lazy<IPersonService> _personService;

        public ProposalsController(IProposalService dataService, Lazy<IPersonService> personService, ILogger logger)
            : base(dataService, logger)
        {
            _proposalService = dataService;
            _personService = personService;
        }


        //[AllowAnonymous]
        //[InjectAuthPrincipal]
        //[Route("getlatlongFromaddress")]
        //[HttpGet]
        //public IHttpActionResult getlatlongFromaddress(string gapikey)
        //{
        //    AddressData a = new AddressData // Belgium
        //    {
        //        Address = "Rue du Cornet 6",
        //        City = "VERVIERS",
        //        State = null,
        //        Country = "Belgium",
        //        Zip = "B-4800"
        //    };

        //    var gls = new GoogleLocationService(gapikey);
        //    var latlong = gls.GetLatLongFromAddress(a);

        //    if (latlong == null)
        //        return null;
        //    var latitude = latlong.Latitude;
        //    var longitude = latlong.Longitude;
        //    return Ok(latitude);
        //}

        [AllowAnonymous]
        [InjectAuthPrincipal]
        [Route("data/{proposalDataId}/{utilityInflationRate=null}")]
        [HttpGet]
        public Proposal2DataView GetProposalData(Guid proposalDataId, double? utilityInflationRate, bool roundAmounts = false)
        {
            return _proposalService.GetProposalDataView(proposalDataId, utilityInflationRate, roundAmounts); ;
        }

        [Route("generate/proposal")]
        [HttpPost]
        [ResponseType(typeof(CreateProposalDataResponse))]
        public async Task<IHttpActionResult> GenerateProposal()
        {
            var request = await GetProposalRequest();
            var response = _proposalService.CreateProposalData(request);

            return Ok(response);
        }

        [Route("{proposalId}/documents/agreement")]
        [HttpPost]
        [ResponseType(typeof(GenericResponse<string>))]
        public async Task<IHttpActionResult> GenerateAgreementUrl(Guid proposalId)
        {
            var response = _proposalService.GetAgreementForProposal(proposalId);
            if (response == null)
            {
                return BadRequest();
            }

            return Ok(new GenericResponse<string> { Response = response });
        }


        [HttpPost, Route("generate/proposal/dummy")]
        public IHttpActionResult GenerateProposalDummy(DocumentSignRequest request)
        {
            return Ok();
        }

        [Route("{proposalDataId}/sign/document")]
        [HttpPost]
        [ResponseType(typeof(Proposal))]
        public async Task<IHttpActionResult> SignDocument(Guid proposalDataId)
        {
            var request = await GetProposalRequest();

            var response = _proposalService.SignAgreement(proposalDataId, request);
            return Ok(response);
        }

        [Route("{proposalId}/documents", Order = 0)]
        [HttpGet]
        [ResponseType(typeof(List<DocumentDataLink>))]
        public async Task<IHttpActionResult> GetAllProposalDocuments(Guid proposalId)
        {
            var response = _proposalService.GetAllProposalDocuments(proposalId);
            return Ok(response);
        }

        [Route("{proposalDataId}/sign/proposal")]
        [HttpPost]
        [ResponseType(typeof(Proposal))]
        public async Task<IHttpActionResult> SignProposal(Guid proposalDataId)
        {
            var request = await GetProposalRequest();

            var response = _proposalService.SignProposal(proposalDataId, request);
            return Ok(response);
        }

        /// <summary>
        /// Update ProposalData.ProposalDataJSON for given proposalDataId.
        /// The ProposalDataJSON will be read from the Body of the request.
        /// </summary>
        /// <param name="proposalDataId"></param>
        /// <returns></returns>
        [Route("data/{proposalDataId}/update/proposalDataJSON")]
        [HttpPatch]
        public async Task<IHttpActionResult> UpdateProposalDataJSON([FromUri]Guid proposalDataId)//, [FromBody] GenericRequest<string> request = null)
        {
            var data = await Request.Content.ReadAsStringAsync();
            try
            {
                var req = JsonConvert.DeserializeObject<GenericRequest<string>>(data);
                data = req?.Request ?? data;
            }
            catch { }

            if (string.IsNullOrWhiteSpace(data))
            {
                return BadRequest("Could not read the data!");
            }

            _proposalService.UpdateProposalDataJSON(proposalDataId, data);
            return Ok();
        }

        [Route("{proposalID:guid}/media/all")]
        [ResponseType(typeof(List<ProposalMediaItem>))]
        [HttpGet]
        public IHttpActionResult GetAllMediaItems(Guid proposalID)
        {
            return Ok(_proposalService.GetProposalMediaItems(proposalID));
        }

        [Route("{proposalID:guid}/media/shareablelinks")]
        [ResponseType(typeof(List<KeyValue>))]
        [HttpGet]
        public IHttpActionResult GetMediaItemsAsShareableLinks(Guid proposalID)
        {
            return Ok(_proposalService.GetProposalMediaItemsAsShareableLinks(proposalID));
        }


        [Route("media/{proposalMediaId:guid}/{thumb}")]
        [ResponseType(typeof(byte[]))]
        [HttpGet]
        public HttpResponseMessage GetMediaItem(Guid proposalMediaID, bool thumb = false)
        {
            var blob = _proposalService.GetProposalMediaItemContent(proposalMediaID, thumb);
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
        /// Upload multiple images using a multi-part body
        /// </summary>
        /// <param name="proposalID"></param>
        /// <returns></returns>
        [Route("{proposalID:guid}/media/upload")]
        [ResponseType(typeof(List<ProposalMediaItem>))]
        public async Task<IHttpActionResult> UploadProposalMedia([FromUri]Guid proposalID)
        {

            var data = await GetMultiPartData<List<MediaItemData>>("MediaItemInfo");
            if (data.Item1 == null ||
                (data.Item2?.Count ?? 0) == 0 ||
                data.Item1.Count != data.Item2.Count)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var request = data
                            .Item2
                            .Select(async (f, idx) => new ProposalMediaUploadRequest
                            {
                                Content = await f.Content.ReadAsByteArrayAsync(),
                                ContentType = data.Item1[idx].ContentType,
                                Notes = data.Item1[idx]?.Notes,
                                MediaItemType = data.Item1[idx].MediaItemType,
                                Name = f.Name
                            })
                            .Select(t => t.Result)
                            .ToList();

            return Ok(_proposalService.UploadProposalMediaItem(proposalID, request));
        }

        [Route("saveAndCloneMediaItems")]
        [HttpPost]
        public Proposal CloneMediaItems([FromBody] Proposal item, [FromUri]Guid originalProposalID)
        {
            var response = base.Post(item);
            _proposalService.CopyProposalMediaItems(originalProposalID, item.Guid);
            return response;
        }

        [Route("autosave")]
        [HttpPost]
        public void AutosavePost(Proposal item)
        {
            base.Post(item);
        }

        [Route("autosave")]
        [HttpPatch]
        public void AutosavePatch(System.Web.Http.OData.Delta<Proposal> item)
        {
            base.Patch(item);
        }

        [AllowAnonymous]
        [InjectAuthPrincipal]
        [Route("survey/{personID}/{propertyID}")]
        [HttpGet]
        public IHttpActionResult GetSurvey(Guid personID, Guid propertyID)
        {
            var result = _personService.Value.GetUserSurvey(personID, propertyID);
            return Ok(new GenericResponse<string> { Response = result });
        }

        [AllowAnonymous]
        [InjectAuthPrincipal]
        [Route("survey/{personID}/{propertyID}")]
        [HttpPost]
        public IHttpActionResult SaveSurvey(Guid personID, Guid propertyID, [FromBody] GenericRequest<string> request)
        {
            var result = _personService.Value.SavePropertySurvey(personID, propertyID, request?.Request);
            return Ok(new GenericResponse<string> { Response = result });
        }

        private async Task<DocumentSignRequest> GetProposalRequest()
        {
            var data = await GetMultiPartData<DocumentSignRequest>("RequestValue");
            await data.Item1.ProcessBinaryData(data.Item2);
            return data.Item1;
        }

        private async Task<Tuple<T, List<RequestFile>>> GetMultiPartData<T>(string jsonPartName)
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());
            T result = default(T);

            if (!string.IsNullOrEmpty(jsonPartName))
            {
                //access form data
                NameValueCollection formData = provider.FormData;
                string requestJSON = null;
                if (formData.Count == 1)
                {
                    requestJSON = formData[0];
                }
                else if (formData.Count > 1)
                {
                    requestJSON = formData[jsonPartName];
                }

                if (string.IsNullOrWhiteSpace(requestJSON))
                {
                    throw new ApplicationException("Could not find the request value JSON!");
                }

                result = JsonConvert.DeserializeObject<T>(requestJSON);
            }
            return new Tuple<T, List<RequestFile>>(result, provider.Files);
        }

        private string SerializeObject(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            return JsonConvert.SerializeObject(obj);
        }

        private T DeserializeString<T>(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}