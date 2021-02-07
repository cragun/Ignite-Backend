using System.Net;
using System.Web.Http;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using System;
using System.Web.Http.Description;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/users")]
    public class UsersController : ApiController
    {
        private readonly IDataService<User> userDataService;
        private readonly IDataService<Person> personService;
        private readonly IDataService<Territory> territoryService;
        private readonly IOUService _ouService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userDataService"></param>
        public UsersController(IDataService<User> userDataService, IDataService<Person> personService, IDataService<Territory> territoryService, IOUService ouService)
        {
            this.userDataService        = userDataService;
            this.personService          = personService;
            this.territoryService       = territoryService;
            this._ouService             = ouService;
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Territory>))]
        [Route("{userID:guid}/territories")]
        public async Task<IHttpActionResult> GetTerritories(Guid userID)
        {

            List<Territory> ret = new List<Territory>();
            Person person = personService.Get(userID, "Assignments.Territory");

            if (person == null || person.Assignments==null )
            {
                return NotFound();
            }

            List<Assignment> assignments = person.Assignments.ToList();

            foreach (Assignment ass in assignments)
            {
                ret.Add(ass.Territory);
            }

            return Ok<ICollection<Territory>>(ret);

        }

        [HttpGet]
        [ResponseType(typeof(Territory))]
        [Route("{userID:guid}/territories/{territoryID:guid}")]
        public async Task<IHttpActionResult> GetTerritoriesForUser(Guid userID,Guid territoryID)
        {

            Territory ret = null;

            Person person = personService.Get(userID, "Assignments.Territory");

            if (person == null || person.Assignments == null)
            {
                return NotFound();
            }


            List<Assignment> assignments = person.Assignments.ToList();

            Assignment ass = assignments.Where(aaa => aaa.TerritoryID == territoryID).FirstOrDefault();
            if (ass == null || ass.Territory==null)
            {
                return NotFound();
            }
            return Ok<Territory>(ass.Territory);

        }


        [HttpGet]
        [ResponseType(typeof(bool))]
        [Route("{userID:guid}/territories/access/{territoryID:guid}")]
        public async Task<IHttpActionResult> GetUserHasTerritoryAccess(Guid userID, Guid territoryID)
        {
            Person person               = personService.Get(userID, "Assignments.Territory, OUAssociations.OURole");
            Territory territory         = territoryService.Get(territoryID, "OU");

            if (person == null || person.IsDeleted || territory == null || territory.IsDeleted || territory.OU == null || territory.OU.IsDeleted)
            {
                return NotFound();
            }

            OU ou               = territory.OU;
            List<Guid> ouIDs    = new List<Guid> { ou.Guid };

            //check if there are any assignments of the user for this particular territory
            bool hasAccess  = person.Assignments.Any(a => !a.IsDeleted && a.TerritoryID.Equals(territoryID));

            if (!hasAccess && person.OUAssociations != null && person.OUAssociations.Count != 0)
            {
                while (ou.ParentID.HasValue)
                {
                    ou = this._ouService.Get(ou.ParentID.Value);
                    if(!ou.IsDeleted)
                    {
                        ouIDs.Add(ou.Guid);
                    }
                }

                hasAccess = person.OUAssociations.Any(oua => !oua.IsDeleted && 
                                                              ouIDs.Contains(oua.OUID) &&
                                                              (oua.OURole.IsOwner || oua.OURole.IsAdmin));

            }

            return Ok<bool>(hasAccess);
        }


        [System.Web.Http.HttpGet]
        [Route("")]
        public async Task<User> Get()
        {
            var user = userDataService.Get(SmartPrincipal.UserId, "Person");
            if (user == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return user;
        }
    }
}