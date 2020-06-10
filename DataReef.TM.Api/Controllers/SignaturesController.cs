using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.Signatures;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/sign")]
    public class SignaturesController : ApiController
    {
        private readonly ILogger _logger;
        private readonly ISignatureService _signatureService;

        public SignaturesController(ILogger logger, ISignatureService signatureService)
        { 
            _logger = logger;
            _signatureService = signatureService;
        }

        /// <summary>
        /// The requests should not reach this endpoint, as the client app should intercept the RightSignature redirect and close the signature screen.
        /// </summary>
        [HttpGet]
        [Route("completed")]
        public async Task<IHttpActionResult> SignCompleted()
        {
            return Ok(new { });
        }

        [HttpPost]
        [Route("emailreminder")]
        public async Task<IHttpActionResult> SendEmailReminder(string contractID)
        {
            _signatureService.SendEmailReminder(contractID);
            return Ok(new { });
        }

        [HttpPost]
        [Route("RightSignatureContractCallback")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> RightSignatureLoanContractCallback(HttpRequestMessage request)
        {
            var xmlSerializer = new DataContractSerializer(typeof(Callback));
            using (var stream = request.Content.ReadAsStreamAsync().Result)
            {
                var callback = (Callback)xmlSerializer.ReadObject(stream);
                _signatureService.RightSignatureContractCallback(callback);
            }
            return Ok(new { });
        }

    }
}