using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.ClientApi.Models;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DataReef.TM.ClientApi.Controllers
{
    /// <summary>
    /// Users
    /// </summary>
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {

        private IPersonService personService;
        private IDataService<User> userService;
        private IOUService ouService;

        public UsersController(IPersonService personService, IDataService<User> userService,IOUService ouService)
        {
            this.personService = personService;
            this.userService = userService;
            this.ouService = ouService;
        }

        /// <summary>
        /// Get users in organization
        /// </summary>
        /// <param name="ouID">The organization id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{ouID:guid}")]
        [ResponseType(typeof(GenericResponse<List<PersonDataView>>))]
        public IHttpActionResult GetUsersForOu(Guid ouID)
        {

            List<PersonDataView> content = new List<PersonDataView>();

            var guidList = new List<Guid>() { SmartPrincipal.OuId };
            var ouGuids = this.ouService.GetHierarchicalOrganizationGuids(guidList);

            if (!ouGuids.Contains(ouID))
            {
                return NotFound();
            }

            var people = this.personService.GetPeopleForOU(ouID, false);

            foreach (var person in people)
            {
                PersonDataView dv = new PersonDataView()
                {
                    EmailAddress = person.EmailAddress,
                    FirstName = person.FirstName,
                    Id = person.Guid,
                    LastName = person.LastName,
                    IsActive = true,
                    PhoneNumber = person.PhoneNumber
                };
                content.Add(dv);
            }

            GenericResponse<List<PersonDataView>> ret = new GenericResponse<List<PersonDataView>>(content);
            return Ok(ret);
        }

        [HttpGet]
        [Route("{ouID:guid}/search/{query}")]
        [ResponseType(typeof(GenericResponse<List<PersonLite>>))]
        public IHttpActionResult GetUsersForOu(Guid ouID, string query)
        {
            query = query.ToLowerInvariant();
            var people = this.personService.GetPeopleForOU(ouID, false)?.Where(p => p.EmailAddress.ToLowerInvariant().Contains(query));

            return Ok(people);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(GenericResponse<List<PersonDataView>>))]
        public IHttpActionResult GetUsers()
        {

            List<PersonDataView> content = new List<PersonDataView>();

            var guidList = new List<Guid>() { SmartPrincipal.OuId };
            var ouGuids = this.ouService.GetHierarchicalOrganizationGuids(guidList);

            if (!ouGuids.Contains(SmartPrincipal.OuId))
            {
                return NotFound();
            }
            
            var people = this.personService.GetPeopleForOU(SmartPrincipal.OuId, true);

            foreach (var person in people)
            {
                PersonDataView dv = new PersonDataView()
                {
                    EmailAddress = person.EmailAddress,
                    FirstName = person.FirstName,
                    Id = person.Guid,
                    LastName = person.LastName,
                    IsActive = true,
                    PhoneNumber =  person.PhoneNumber
                };
                content.Add(dv);
            }

            GenericResponse<List<PersonDataView>> ret = new GenericResponse<List<PersonDataView>>(content);
            return Ok(ret);
        }

        //[HttpGet]
        //[Route("public")]
        //[AllowAnonymous] // this will work even if the request is not authenticated, It can also be set on the controller like in AuthController
        //public IHttpActionResult PublicContent()
        //{
        //    return Ok("Public content");
        //}

        //[HttpGet]
        //[Route("private")] // this will not work if the request is NOT authenticated
        //public async Task<IHttpActionResult> PrivateMethod()
        //{
        //    // async is not needed for this method. Its only recomended you work with await/async if you are IO bound and you need to free some threads (like calling a wcf service)
        //    // replace async Task<IHttpActionResult> with IHttpActionResult
        //    return Ok("Super secret private content");
        //}




    }
}