using DataReef.Core;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Api.Classes;
using DataReef.TM.Api.Classes.Requests;
using DataReef.TM.Api.Classes.ViewModels;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.DTOs.Common;
using DataReef.TM.Models.DTOs.Inquiries;
using DataReef.TM.Models.DTOs.Persons;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Reporting.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.OutputCache.V2;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Managers CRUD for Person
    /// </summary>
    [RoutePrefix("api/v1/people")]
    public class PeopleController : EntityCrudController<Person>
    {
        private readonly IPersonService peopleService;
        private readonly IPersonSettingService settingsService;
        private readonly IBlobService blobService;
        private readonly ICurrentLocationService currentLocationService;
        private readonly IInquiryService inquiryService;

        public PeopleController(IPersonService peopleService,
            IPersonSettingService settingsService,
            IBlobService blobService,
            ILogger logger,
            ICurrentLocationService currentLocationService,
            IInquiryService inquiry)
            : base(peopleService, logger)
        {
            this.peopleService = peopleService;
            this.blobService = blobService;
            this.currentLocationService = currentLocationService;
            this.inquiryService = inquiry;
            this.settingsService = settingsService;
        }


        [Route("{personID:guid}/locations/{date}")]
        [HttpGet]
        public IHttpActionResult GetLocationsByDate(string personID, string date)
        {
            try
            {
                List<CurrentLocation> ret = currentLocationService.GetCurrentLocationsForPersonAndDate(Guid.Parse(personID), DateTime.Parse(date)).ToList();
                return Ok<List<CurrentLocation>>(ret);

            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [Route("locations/latest")]
        [HttpPost]
        [ResponseType(typeof(ICollection<CurrentLocation>))]
        public IHttpActionResult GetLatestLocationsForPeopleIds(GenericRequest<List<Guid>> request)
        {
            if (request == null || request.Request == null || request.Request.Count == 0)
            {
                return Ok(new List<CurrentLocation>());
            }

            var ret = currentLocationService.GetLatestLocations(request.Request);
            return Ok(ret);
        }

        [Route("{personID:guid}/image")]
        [HttpGet]
        public HttpResponseMessage GetImage(string personID)
        {
            try
            {
                Guid guid = Guid.Parse(personID);

                var blob = blobService.Download(guid);

                if (blob == null || blob.Content == null || blob.Content.Length == 0)
                    throw new HttpResponseException(HttpStatusCode.NotFound);

                MemoryStream ms = new MemoryStream(blob.Content);
                HttpResponseMessage response = new HttpResponseMessage { Content = new StreamContent(ms) };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(blob.ContentType);
                response.Content.Headers.ContentLength = blob.Content.Length;
                return response;
            }
            catch (System.Exception ex)
            {
                HttpResponseMessage ret = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                ret.ReasonPhrase = ex.Message;
                throw new HttpResponseException(ret);
            }
        }

        [Route("mine")]
        [HttpPost]
        [ResponseType(typeof(List<Person>))]
        public IHttpActionResult GetMine(OUMembersRequest request)
        {
            var ret = peopleService.GetMine(request);
            return Ok(ret);
        }

        [Route("me")]
        [HttpGet]
        [ResponseType(typeof(PersonDTO))]
        public IHttpActionResult GetMe(string include = "")
        {
            var includeString = "PhoneNumbers";
            if (!string.IsNullOrEmpty(include))
            {
                includeString += $"&{include}";
            }
            var person = peopleService.GetPersonDTO(SmartPrincipal.UserId, includeString);

            return Ok(person);
        }

        [HttpGet]
        [Route("{personID:Guid}/inquiryStats")]
        [ResponseType(typeof(ICollection<InquiryStatisticsForPerson>))]
        public IHttpActionResult GetPersonSummaryForCustomStatuses(Guid personID, string dispositions = "", DateTime? specifiedDay = null)
        {
            var inquiryStatuses = dispositions
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

            var data = inquiryService.GetInquiryStatisticsForPerson(personID, inquiryStatuses, specifiedDay);
            return Ok(data);
        }

        [Route("{guid:guid}/mayedit")]
        [HttpGet]
        [ResponseType(typeof(Person))]
        public IHttpActionResult GetMayEdit(Guid guid, string include = "", string exclude = "", string fields = "")
        {
            var ret = peopleService.GetMayEdit(guid, true, include, exclude, fields);
            ret.RemoveDefaultExcludedProperties("MayEdit");
            return Ok(ret);
        }

        /// <summary>
        /// Method used to undelete a person, w/ associated User and Credential
        /// This method will also send an email letting the person know that the account has been reactivated
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        [Route("{personID:guid}/reactivate")]
        [HttpPost]
        public IHttpActionResult Reactivate(Guid personID)
        {
            peopleService.Reactivate(personID);
            return Ok();
        }

        [HttpGet]
        [Route("{personID:guid}/settings/{group?}")]
        [ResponseType(typeof(Dictionary<string, ValueTypePair<SettingValueType, string>>))]
        public IHttpActionResult GetAllSettings(Guid personID, PersonSettingGroupType? group = null)
        {
            var data = settingsService.GetSettings(personID, group);
            return Ok(data);
        }

        [HttpPost]
        [Route("{personID:guid}/settings/{group?}")]
        [ResponseType(typeof(Dictionary<string, ValueTypePair<SettingValueType, string>>))]
        public IHttpActionResult SetAllSettings([FromUri]Guid personID, [FromBody]Dictionary<string, ValueTypePair<SettingValueType, string>> data, [FromUri]PersonSettingGroupType? group = null)
        {
            settingsService.SetSettings(personID, group, data);
            return Ok();
        }

        [HttpGet]
        [Route("integration/token")]
        [ResponseType(typeof(Jwt))]
        public IHttpActionResult GenerateIntegrationToken()
        {
            var token = peopleService.GenerateIntegrationToken();

            return Ok(new Jwt
            {
                Token = token.Guid.ToString(),
                Expiration = token.ExpirationDate.ToUnixTime()
            });
        }

        [HttpGet]
        [Route("integration/webview/parameters")]
        [ResponseType(typeof(GenericResponse<IntegrationParameters>))]
        public IHttpActionResult GetWebViewParams()
        {
            var token = peopleService.GenerateIntegrationToken();

            var resp = new GenericResponse<IntegrationParameters>
            {
                Response = new IntegrationParameters
                {
                    LegionToken = token.Guid.ToString()
                }
            };
            return Ok(resp);
        }

        [HttpGet]
        [Route("crm/dispositions")]
        [ResponseType(typeof(GenericResponse<List<CRMDisposition>>))]
        public IHttpActionResult GetCRMDispositions()
        {
            return Ok(new GenericResponse<List<CRMDisposition>> { Response = peopleService.CRMGetAvailableNewDispositions() });
        }

        [HttpGet]
        [Route("crm/dispositionfilters")]
        [ResponseType(typeof(List<CRMDisposition>))]
        public IHttpActionResult GetNewCRMDispositions()
        {
            return Ok(peopleService.CRMGetAvailableNewDispositions());
        }

        [HttpPost]
        [Route("crm/data")]
        [ResponseType(typeof(PaginatedResult<Property>))]
        public IHttpActionResult GetCRMData(CRMFilterRequest request)
        {
            var result = peopleService.CRMGetProperties(request);
            SetupSerialization(result.Data, request.Include, request.Exclude, request.Fields);
            return Ok(result);
        }

        [HttpGet]
        [Route("mysurvey")]
        [ResponseType(typeof(GenericResponse<string>))]
        public IHttpActionResult GetMySurvey()
        {
            var result = peopleService.GetUserSurvey(SmartPrincipal.UserId);
            return Ok(new GenericResponse<string> { Response = result });
        }

        [HttpPost]
        [Route("savemysurvey")]
        [ResponseType(typeof(GenericResponse<string>))]
        public IHttpActionResult SaveMySurvey([FromBody] GenericRequest<string> request)
        {
            var result = peopleService.SaveUserSurvey(SmartPrincipal.UserId, request.Request);
            return Ok(new GenericResponse<string> { Response = result });
        }

        [HttpGet]
        [Route("surveyurl/{propertyID}")]
        [ResponseType(typeof(GenericResponse<string>))]
        public IHttpActionResult GetSurveyUrl(Guid propertyID)
        {
            var result = peopleService.GetSurveyUrl(SmartPrincipal.UserId, propertyID);
            return Ok(new GenericResponse<string> { Response = result });
        }

        [HttpGet]
        [Route("crm/LeadSources/{ouid}")]
        [ResponseType(typeof(GenericResponse<List<CRMLeadSource>>))]
        public IHttpActionResult GetCRMLeadSources(Guid ouid)
        {
            return Ok(new GenericResponse<List<CRMLeadSource>> { Response = peopleService.CRMGetAvailableLeadSources(ouid) });
        }


        [Route("PersonClock/{min}")]
        [HttpGet]
        [ResponseType(typeof(PersonClockTime))]
        public IHttpActionResult PersonClock(long min)
        {
            var person = peopleService.GetPersonClock(SmartPrincipal.UserId, min);

            return Ok(person);
        }


        [Route("deleteuser/{guid}")]
        [HttpPost]
        public HttpResponseMessage DeleteUserByGuid(Guid guid)
        {
            var ret = base.DeleteByGuid(guid);
            return ret;
        }


        public override HttpResponseMessage DeleteByGuid(Guid guid)
        {
            OUsControllerCacheInvalidation();

            return base.DeleteByGuid(guid);
        }


        public override Person Post(Person item)
        {
            OUsControllerCacheInvalidation();

            return base.Post(item);
        }

        public override Person Patch(System.Web.Http.OData.Delta<Person> item)
        {
            OUsControllerCacheInvalidation();

            return base.Patch(item);
        }

        public override HttpResponseMessage Delete(Person item)
        {
            OUsControllerCacheInvalidation();

            return base.Delete(item);
        }

        public override ICollection<Person> PostMany(List<Person> items)
        {
            OUsControllerCacheInvalidation();

            return base.PostMany(items);
        }

        public override Person Put(Person item)
        {
            OUsControllerCacheInvalidation();

            return base.Put(item);
        }

        /// <summary>
        /// Invalidate cache for OUsController GET methods.
        /// </summary>
        public void OUsControllerCacheInvalidation()
        {
            var cache = Configuration.CacheOutputConfiguration().GetCacheOutputProvider(Request);
            string controllerName = typeof(OUsController).FullName;

            foreach (var key in cache.AllKeys)
            {
                if (key.StartsWith(controllerName, StringComparison.CurrentCultureIgnoreCase))
                {
                    cache.Remove(key);
                }
            }
        }
    }
}
