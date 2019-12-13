using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.Blobs;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Manages HiRes Imagery Purchases and Retrieval
    /// </summary>
    [RoutePrefix("api/v1/imagery")]
    public class ImageryController : ApiController
    {
        private IImageryService _imageryService;

        public ImageryController(IImageryService imageryService)
        {
            _imageryService = imageryService;
        }

        /// <summary>
        /// Gets a prevoiusly purchased image for a given property
        /// </summary>
        /// <param name="propertyID">property id for the purchase</param>
        /// <param name="top">Lat of the top cropping box side</param>
        /// <param name="left">Lon of the left cropping box side</param>
        /// <param name="bottom">Lat of the bottom cropping box side</param>
        /// <param name="right">Lon of the Right cropping box side</param>
        /// <param name="direction">Direction of Image.  Default is Down=Orthogonal</param>
        [HttpGet]
        [Route("{propertyID:guid}")] // this will not work if the request is NOT authenticated
        [ResponseType(typeof(BlobModel))]
        public IHttpActionResult GetExistingImage(Guid propertyID, float top = 0, float left = 0, float bottom = 0, float right = 0, string direction = "Down")
        {
            try
            {
                var blob = _imageryService.GetExistingHiResImageForProperty(propertyID, top, left, bottom, right, direction);
                return Ok<BlobModel>(blob);
            }
            catch (Exception ex)
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


        /// <summary>
        /// Purchases a high res image for the current cuser and given property
        /// </summary>
        /// <param name="propertyID">property id for the purchase</param>
        /// <param name="top">Lat of the top cropping box side</param>
        /// <param name="left">Lon of the left cropping box side</param>
        /// <param name="bottom">Lat of the bottom cropping box side</param>
        /// <param name="right">Lon of the Right cropping box side</param>
        /// <param name="direction">Direction of Image.  Default is Down=Orthogonal</param>
        [HttpGet]
        [Route("{propertyID:guid}/purchase")] // this will not work if the request is NOT authenticated
        [ResponseType(typeof(BlobModel))]
        public IHttpActionResult PurchaseImage(Guid propertyID, float top = 0, float left = 0, float bottom = 0, float right = 0, string direction = "Down")
        {
            try
            {
                var blob = _imageryService.PurchaseHighResImageAtCoordinates(propertyID, top, left, bottom, right, direction);
                return Ok<BlobModel>(blob);
            }
            catch (Exception ex)
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

        /// <summary>
        /// Returns a list of strings of the avaialable hi res image orientations for the given lat lon
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        [HttpGet]
        [Route("available")] // this will not work if the request is NOT authenticated
        public IHttpActionResult GetAvailableOrientations(float lat, float lon)
        {
            try
            {
                var ret = _imageryService.AvailableOrientationsAtLocation(lat, lon);
                return Ok(new { Orientations = ret });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("404"))
                {
                    return NotFound();
                }
                else
                {
                    return Content(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
        }

        /// <summary>
        /// Method that will migrate existing HiResImages from this database to Geo.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("migrate")]
        public IHttpActionResult MigrateHiResImagesToGeo()
        {
            _imageryService.MigrateHiResImages();
            return Ok();
        }
    }
}