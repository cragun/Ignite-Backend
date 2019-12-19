﻿using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.Credit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Allows to to create and list Batch Prescreens
    /// </summary>
    [RoutePrefix("api/v1/prescreeninstants")]
    public class PrescreenInstantsController : EntityCrudController<PrescreenInstant>
    {
        private readonly IPrescreenInstantService instantprescreenService;

        public PrescreenInstantsController(IPrescreenInstantService instantprescreenService, ILogger logger)
            : base(instantprescreenService, logger)
        {
            this.instantprescreenService = instantprescreenService;
        }

        public override PrescreenInstant Patch(System.Web.Http.OData.Delta<PrescreenInstant> item)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });
        }

        public override ICollection<PrescreenInstant> PostMany(List<PrescreenInstant> items)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });
        }

        public override HttpResponseMessage DeleteByGuid(Guid guid)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });
        }

        public override HttpResponseMessage Delete(PrescreenInstant item)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });   
        }

        public override PrescreenInstant Put(PrescreenInstant item)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });
        }

        public override PrescreenInstant Post(PrescreenInstant item)
        {
            return instantprescreenService.Insert(item);         
        }
    }
}