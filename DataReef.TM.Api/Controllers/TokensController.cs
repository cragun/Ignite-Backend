using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Net.Http.Headers;
using DataReef.Core.Classes;
using DataReef.TM.Api.Classes;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Contracts.DataViews.Ledgers;
using DataReef.Core.Infrastructure.Authorization;
using System.Collections.Generic;

namespace DataReef.TM.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/v1/tokens")]
    public class TokensController : ApiController
    {
        private readonly ITokensProvider tokensProvider;

        public TokensController(ITokensProvider tokensProvider)
        {
            this.tokensProvider = tokensProvider;
        }

        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            Guid userID = SmartPrincipal.UserId;
            LedgerDataView ret = this.tokensProvider.GetLedgerDataViewForPerson(userID);
            return Ok<LedgerDataView>(ret);
        }

        [Route("forPerson/{personId:guid}")]
        [HttpGet]
        public IHttpActionResult GetForPerson(Guid personId)
        {
            LedgerDataView ret = this.tokensProvider.GetLedgerDataViewForPerson(personId);
            return Ok<LedgerDataView>(ret);
        }

        [Route("transfer")]
        [HttpPost]
        public IHttpActionResult Transfer(TransferDataCommand transfer)
        {
            this.tokensProvider.PerformTransfers(new List<TransferDataCommand>() { transfer });
            return Ok();
        }








    }
}
