﻿using DataReef.Core.Infrastructure.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{
    public partial class IntegrationsController
    {
        [Route("sunnova/login/callback")]
        [HttpPost]
        [InjectAuthPrincipal]
        [AllowAnonymous]

        public IHttpActionResult LoginCallback()
        {
            return Ok();
        }
    }
}