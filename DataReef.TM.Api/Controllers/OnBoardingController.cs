using DataReef.Core.Logging;
using DataReef.Integrations.Google.Models;
using DataReef.Integrations.Google.Processors;
using DataReef.TM.Api.Classes;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DataViews.OnBoarding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/onboarding")]
    public class OnBoardingController : ApiController
    {
        private readonly IOUService ouService;
        private readonly ILogger logger;

        public OnBoardingController(IOUService ouService, ILogger logger)
        {
            this.ouService = ouService;
            this.logger = logger;
        }



        [Route("parsesheet")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ImportData(SheetOptions options)
        {
            var processor = new SheetProcessor();
            var data = processor.Process(options);

            return Ok(data);
        }

        [HttpPost]
        [Route("ous/create")]
        public async Task<IHttpActionResult> SaveOnboardingSettings(OnboardingOUDataView req)
        {
            if (req == null)
            {
                throw new ApplicationException("Invalid request. No data!");
            }
            req.Validate(true);

            await ouService.CreateNewOU(req);
            return Ok();
        }

        [HttpPatch]
        [Route("ous/edit/{ouid}")]
        public async Task<IHttpActionResult> EditOU([FromUri]Guid ouid, [FromBody] OnboardingOUDataView req)
        {
            if (req == null)
            {
                throw new ApplicationException("Invalid request. No data!");
            }
            req.Validate(false);

            await ouService.EditOU(ouid, req);
            return Ok();
        }

        [HttpPatch]
        [Route("ous/editsettings/{ouid}")]
        public async Task<IHttpActionResult> EditOUSettings([FromUri]Guid ouid, [FromBody] OnboardingOUSettingsDataView req)
        {
            if (req == null)
            {
                throw new ApplicationException("Invalid request. No data!");
            }

            await ouService.EditOUSettings(ouid, req);
            return Ok();
        }

        [HttpGet]
        [Route("lookup/{parentID?}")]
        public async Task<IHttpActionResult> GetOnboardingLookupData(Guid? parentID)
        {
            return Ok(await ouService.GetOnboardingLookupData(parentID));
        }

        [HttpGet]
        [Route("oustest")]
        public async Task<IHttpActionResult> AddOUSettingsTest()
        {
            await ouService.AddOUSettingsTest();
            return Ok("success");
        }

        //[HttpGet]
        //[Route("genericproposal/settings")]
        //public async Task<IHttpActionResult> AddGenericProposalOUSettings()
        //{
        //    await ouService.AddGenericProposalOUSettings();
        //    return Ok("success");
        //}
    }
}
