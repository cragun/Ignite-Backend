using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Api.Classes.Requests;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.Properties;
using DataReef.TM.Models.DTOs.PropertyAttachments;
using DataReef.TM.Models.DTOs.SmartBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Description;
using DataReef.Auth.Helpers;
using System.Threading.Tasks;
using DataReef.TM.Models.PropertyAttachments;
using System.Web;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v2/properties")]
    public class Properties2Controller : ApiController
    {
        private readonly Func<IPropertyService> _propertyServiceFactory;
        private readonly ICsvReportingService _csvReportingService;
        private readonly Func<IPropertyAttachmentService> _propertyAttachmentServiceFactory;

        public Properties2Controller(Func<IPropertyService> propertyServiceFactory,
            ICsvReportingService csvReportingService,
            Func<IPropertyAttachmentService> propertyAttachmentServiceFactory)
        {
            _propertyServiceFactory = propertyServiceFactory;
            _csvReportingService = csvReportingService;
            _propertyAttachmentServiceFactory = propertyAttachmentServiceFactory;
        }

        [HttpPost]
        [Route("getproperties")]
        public async Task<IHttpActionResult> GetProperties([FromBody]GetPropertiesRequest propertiesRequest)
        {
            if (propertiesRequest == null ||
                (propertiesRequest.GeoPropertiesRequest == null && propertiesRequest.PropertiesRequest == null) ||
                propertiesRequest.TerritoryID == Guid.Empty)
                return BadRequest($"Invalid {nameof(propertiesRequest)}");

            var response = _propertyServiceFactory().GetProperties(propertiesRequest);

            return Ok(response);
        }

        [HttpGet]
        [Route("getpropertiesSearch/{territoryid}")]
        public async Task<IHttpActionResult> GetPropertiesSearch(Guid territoryid, string searchvalue)
        {
            if (string.IsNullOrEmpty(searchvalue))
                return BadRequest($"Invalid searchvalue.");

            var response = _propertyServiceFactory().GetPropertiesSearch(territoryid, searchvalue);

            return Ok(response);
        }

        public object GetPropertyValue(object car, string propertyName)
        {
            return car.GetType().GetProperties()
               .Single(pi => pi.Name == propertyName)
               .GetValue(car, null);
        }

        [HttpGet]
        [Route("sync/{propertyID:guid}")]
        public async Task<IHttpActionResult> SyncProperty(Guid propertyID, string include = "")
        {
            if (propertyID == Guid.Empty)
                return BadRequest($"Invalid {nameof(propertyID)}");

            var response = _propertyServiceFactory().SyncProperty(propertyID, include);


            string queryString = "Own or Rent,Length of Residence,Year Built,Income Level,Home Size,Bedrooms on Record,Bathrooms on Record,Phone Number,Home Phone Number";

            var rd = response.PropertyBag.ToList();
            List<Models.Geo.Field> propbags = new List<Models.Geo.Field>();
            foreach (string propname in queryString.Split(','))
            {
                var propbg = rd.Where(x => x.DisplayName == propname).FirstOrDefault();

                if(propbg != null)
                {
                    propbags.Add(propbg);
                }
            }

            var temp = propbags;
            propbags.AddRange(rd.Except(temp).ToList());
            response.PropertyBag = propbags;            

            return Ok(response);
        }


        [HttpGet]
        [Route("propertybag/{propertyID:guid}")]
        public async Task<IHttpActionResult> PropertyBagsbyID(Guid propertyID)
        {
            if (propertyID == Guid.Empty)
                return BadRequest($"Invalid {nameof(propertyID)}");

            var response = _propertyServiceFactory().PropertyBagsbyID(propertyID);


            string queryString = "Own or Rent,Length of Residence,Year Built,Income Level,Home Size,Bedrooms on Record,Bathrooms on Record,Phone Number,Home Phone Number";

            var rd = response.PropertyBag.ToList();
            List<Models.Geo.Field> propbags = new List<Models.Geo.Field>();
            foreach (string propname in queryString.Split(','))
            {
                var propbg = rd.Where(x => x.DisplayName == propname).FirstOrDefault();

                if (propbg != null)
                {
                    propbags.Add(propbg);
                }
            }

            var temp = propbags;
            propbags.AddRange(rd.Except(temp).ToList());
            response.PropertyBag = propbags;

            return Ok(response);
        }

        [HttpPost]
        [ResponseType(typeof(ICollection<Property>))]
        [Route("wkt")]
        public async Task<IHttpActionResult> GetPropertiesByShape([FromBody] GenericRequest<string> request)
        {
            var fileBytes = _csvReportingService.GeneratePropertyCsvReport(request.Request);

            if (fileBytes == null)
            {
                return BadRequest();
            }

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(fileBytes)
            };

            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = "report.csv"
            };

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return ResponseMessage(result);
        }

        [HttpPost]
        [ResponseType(typeof(PropertyAttachmentItemDTO))]
        [Route("attachments/image/upload")]
        [Route("attachments/uploadimage")]
        public async Task<IHttpActionResult> UploadImage([FromBody]UploadImageToPropertyAttachmentRequest uploadImageRequest)
        {
            if (uploadImageRequest == null ||
                string.IsNullOrEmpty(uploadImageRequest.ItemID) ||
                string.IsNullOrEmpty(uploadImageRequest.SectionID) ||
                (uploadImageRequest.Images?.Any() != true && uploadImageRequest.ImagesWithNotes?.Any() != true))
                return BadRequest($"Invalid {nameof(uploadImageRequest)}");

            var response = _propertyAttachmentServiceFactory()
                .UploadImage(uploadImageRequest.PropertyAttachmentID,
                    uploadImageRequest.PropertyAttachmentItemID,
                    uploadImageRequest.SectionID,
                    uploadImageRequest.ItemID,
                    uploadImageRequest.Images,
                    uploadImageRequest.ImagesWithNotes,
                    uploadImageRequest.Location);

            return Ok(response);
        }

        [HttpPost, HttpDelete]
        [ResponseType(typeof(PropertyAttachmentItemDTO))]
        [Route("attachments/image/delete")]
        public async Task<IHttpActionResult> DeleteAttachmentImage([FromBody]DeleteImageFromPropertyAttachmentRequest request)
        {
            if (request == null)
            {
                return BadRequest($"Invalid {nameof(request)}");
            }

            var result = _propertyAttachmentServiceFactory().DeleteImage(request.PropertyAttachmentItemID, request.ImageID);

            return Ok(result);
        }

        [HttpPatch]
        [ResponseType(typeof(PropertyAttachmentItemDTO))]
        [Route("attachments/image/editnotes")]
        public async Task<IHttpActionResult> EditNotesForImage([FromBody]EditPropertyAttachmentImageNotesRequest request)
        {
            if (request == null)
            {
                return BadRequest($"Invalid {nameof(request)}");
            }

            var result = _propertyAttachmentServiceFactory().EditImageNotes(request.PropertyAttachmentItemID, request.ImageID, request.Notes);

            return Ok(result);
        }

        [HttpPost]
        [Obsolete("Will be removed. Replaced by the DELETE method")]
        [ResponseType(typeof(PropertyAttachmentItemDTO))]
        [Route("attachments/deleteImage")]
        public async Task<IHttpActionResult> DeleteImage([FromBody]DeleteImageFromPropertyAttachmentRequest request)
        {
            if (request == null)
            {
                return BadRequest($"Invalid {nameof(request)}");
            }

            var result = _propertyAttachmentServiceFactory().DeleteImage(request.PropertyAttachmentItemID, request.ImageID);

            return Ok(result);
        }

        [HttpPatch]
        [Route("attachments/{guid}/edit")]
        public async Task<IHttpActionResult> EditAttachment(Guid guid, [FromBody]EditPropertyAttachmentRequest request)
        {
            _propertyAttachmentServiceFactory().UpdatePropertyAttachment(guid, request);
            return Ok();
        }

        [HttpPost]
        [Route("attachments/{guid}/submit")]
        [ResponseType(typeof(GenericResponse<bool>))]
        public async Task<IHttpActionResult> SubmitAttachment(Guid guid)
        {
            var response = _propertyAttachmentServiceFactory().SubmitPropertyAttachment(guid);
            return Ok(new GenericResponse<bool> { Response = response });
        }

        [HttpPost]
        [Route("attachments/{guid}/section/{sectionID}/submit")]
        [ResponseType(typeof(GenericResponse<bool>))]
        public async Task<IHttpActionResult> SubmitSectionIDAttachment(Guid guid, string sectionID)
        {
            var response = _propertyAttachmentServiceFactory().SubmitPropertyAttachmentSection(guid, sectionID);
            return Ok(new GenericResponse<bool> { Response = response });
        }

        [HttpPost]
        [Route("attachments/{guid}/section/{sectionID}/task/{taskID}/submit")]
        [ResponseType(typeof(GenericResponse<bool>))]
        public async Task<IHttpActionResult> SubmitTaskIDAttachment(Guid guid, string sectionID, string taskID)
        {
            var response = _propertyAttachmentServiceFactory().SubmitPropertyAttachmentTask(guid, sectionID, taskID);
            return Ok(new GenericResponse<bool> { Response = response });
        }


        [HttpGet]
        [ResponseType(typeof(ExtendedPropertyAttachmentDTO))]
        [Route("attachments/details/{propertyAttachmentID:guid}")]
        public async Task<IHttpActionResult> GetPropertyAttachment(Guid propertyAttachmentID)
        {
            var response = _propertyAttachmentServiceFactory().GetPropertyAttachmentData(propertyAttachmentID);

            return Ok(response);
        }

        [HttpGet]
        [ResponseType(typeof(IEnumerable<ExtendedPropertyAttachmentDTO>))]
        [Route("attachments/{propertyID:guid}")]
        public async Task<IHttpActionResult> GetPropertyAttachmentForProperty(Guid propertyID)
        {
            var response = _propertyAttachmentServiceFactory().GetPropertyAttachmentsForProperty(propertyID);

            return Ok(response);
        }

        [HttpPost]
        [Route("attachments/{propertyAttachmentID:guid}/review")]
        [ResponseType(typeof(GenericResponse<bool>))]
        public async Task<IHttpActionResult> ReviewPropertyAttachmentReview(Guid propertyAttachmentID, PropertyAttachmentSubmitReviewRequest request)
        {
            var response = _propertyAttachmentServiceFactory().ReviewPropertyAttachment(propertyAttachmentID, request);
            return Ok(new GenericResponse<bool> { Response = response });
        }


        [HttpGet]
        [ResponseType(typeof(IEnumerable<ExtendedPropertyAttachmentDTO>))]
        [Route("attachments")]
        public async Task<IHttpActionResult> GetAllPropertyAttachments(int pageIndex = 0, int itemsPerPage = 20, string query = "")
        {
            var response = _propertyAttachmentServiceFactory().GetPagedPropertyAttachments(pageIndex, itemsPerPage, query);

            return Ok(response);
        }

        [HttpPost]
        [ResponseType(typeof(CanCreateAppointmentResponse))]
        [Route("cancreateappointment")]
        public async Task<IHttpActionResult> CanCreateAppointment(CanCreateAppointmentRequest request)
        {
            var response = _propertyServiceFactory().CanAddAppointmentOnProperty(request);

            return Ok(response);
        }

        /// <summary>
        /// POST method used by SmartBoard to create a new appointment for a specified property
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiKey"></param>
        [HttpPost, Route("sb/{apiKey}")]
        [ResponseType(typeof(SBAppointmentDTO))]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<SBPropertyDTO> CreateNewPropertyFromSmartBoard(SBCreatePropertyRequest request, string apiKey)
        {

            bool checkTime = CryptographyHelper.checkTime(apiKey);
            string DecyptApiKey = CryptographyHelper.getDecryptAPIKey(apiKey);

            return _propertyServiceFactory().CreatePropertyFromSmartBoard(request, DecyptApiKey);
        }


        /// <summary>
        /// POST method used by SmartBoard to change customer's FirstName and LastName for a specified property
        /// </summary>
        /// <param name="request"></param>
        /// <param name="leadId"></param>
        [HttpPost, Route("sb/leadname/{leadId}")]
        [ResponseType(typeof(SBPropertyDTO))]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<IHttpActionResult> ChangePropertyNameFromSB(long leadId, [FromBody]SBPropertyNameDTO request)
        {
            var result = _propertyServiceFactory().EditPropertyNameFromSB(leadId,request);

            return Ok(result);
        }



        [HttpPost]
        [ResponseType(typeof(PropertyAttachmentItemDTO))]
        [Route("Appointment/utilityBill/upload/{PropertyId}")]
        public async Task<IHttpActionResult> UploadUtilityBill(Guid PropertyId)
        {
            var PicFile = HttpContext.Current.Request.Files["file"];
            if (PicFile != null && !string.IsNullOrWhiteSpace(PicFile.FileName))
            {

                System.IO.Stream fs = PicFile.InputStream;
                System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);


                UploadImageToPropertyAttachmentRequest uploadImageRequest = new UploadImageToPropertyAttachmentRequest();
                uploadImageRequest.Images = new List<string>();
                uploadImageRequest.Images.Add(base64String);

                if (uploadImageRequest == null || (uploadImageRequest.Images?.Any() != true))
                    return BadRequest($"Invalid {nameof(uploadImageRequest)}");

                var response = _propertyAttachmentServiceFactory().UploadUtilityBillImage(PropertyId, uploadImageRequest);
                return Ok(response);
            }

            return BadRequest("Could not find the file!");
        }


    }
}
