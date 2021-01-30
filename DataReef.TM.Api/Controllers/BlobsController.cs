using DataReef.TM.Api.Classes;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.Blobs;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataReef.TM.API.Controllers
{
    //[Authorize]
    [RoutePrefix("api/v1/blobs")]
    public class BlobsController : ApiController
    {
        private readonly IBlobService blobService;

        public BlobsController(IBlobService blobService)
        {
            this.blobService = blobService;
        }

        [HttpPost]
        [Route("{guid:guid}")]
        public async Task<string> Download(Guid guid)
        {
            var blob = await blobService.Download(guid);
            return Convert.ToBase64String(blob.Content);
        }

        /// <summary>
        /// Saves an image blob to the blob cloud.  The Guid is the guid of the associated Attachment object. Content is the Base64 encoded image
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> SaveBytes(BlobPost post)
        {
            try
            {
                if ((post.Guid == Guid.Empty && string.IsNullOrEmpty(post.Name)) || post.Content == null)
                {
                    return new System.Web.Http.Results.BadRequestResult(this);
                }

                string blobName = string.IsNullOrEmpty(post.Name) ? post.Guid.ToString() : post.Name;

                BlobModel model = new BlobModel
                {
                    Content = Convert.FromBase64String(post.Content),
                    ContentType = post.ContentType
                };

                blobService.UploadByName(blobName, model, post.AccessRights);

                return Ok(new { Url = blobService.GetFileURL(blobName) });
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// This method returns the binary content of the blob
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
       // [AllowAnonymous]
        [HttpGet]
        [Route("{guid:guid}")]
        public async Task<HttpResponseMessage> GetBytes(Guid guid)
        {
            try
            {
                var blob = await blobService.Download(guid);

                if (blob == null || blob.Content == null || blob.Content.Length == 0)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                MemoryStream ms = new MemoryStream(blob.Content);
                HttpResponseMessage response = new HttpResponseMessage { Content = new StreamContent(ms) };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(blob.ContentType);
                response.Content.Headers.ContentLength = blob.Content.Length;
                return response;
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("404"))
                {
                    HttpResponseMessage ret = new HttpResponseMessage(HttpStatusCode.NotFound);
                    ret.ReasonPhrase = "Not Found";
                    throw new HttpResponseException(ret);
                }
                else
                {
                    HttpResponseMessage ret = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    ret.ReasonPhrase = ex.Message;
                    throw new HttpResponseException(ret);
                }
            }
        }
    }
}
