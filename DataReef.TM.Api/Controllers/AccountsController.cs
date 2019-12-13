using System;
using System.Collections.Generic;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.IO;
using System.Net.Http.Headers;
using System.Web.Http.Description;


namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Managers CRUD for Person
    /// </summary>
    [RoutePrefix("api/v1/accounts")]
    public class AccountsController : EntityCrudController<Account>
    {
        private readonly IDataService<Account> accountService;

        public AccountsController(IDataService<Account> accountService, ILogger logger)
            : base(accountService, logger)
        {
            this.accountService = accountService;
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Person>))]
        [Route("{accountID:guid}/people")]
        public IHttpActionResult GetPeople(Guid accountID)
        {

            List<Person> ret = new List<Person>();
            Account account = this.accountService.Get(accountID, "Associations.Person");

            if (account == null || account.Associations == null)
            {
                return NotFound();
            }

            List<AccountAssociation> asses = account.Associations.ToList();
            foreach (AccountAssociation aAss in asses)
            {
                ret.Add(aAss.Person);
            }

            return Ok<ICollection<Person>>(ret);
        }

    }
}
