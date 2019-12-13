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
using DataReef.Core.Infrastructure.Authorization;


namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Manages PersonalConnections ( Following )
    /// </summary>
    [RoutePrefix("api/v1/personalconnections")]
    public class PersonalConnectionsController : EntityCrudController<PersonalConnection>
    {
        private readonly IDataService<PersonalConnection> service;

        public PersonalConnectionsController(IDataService<PersonalConnection> service, ILogger logger)
            : base(service, logger)
        {
            this.service = service;
        }


        public override ICollection<PersonalConnection> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string include = "", string exclude = "", string fields = "")
        {
            string filter = string.Format("FromPersonID={0}",SmartPrincipal.UserId);
            ICollection<PersonalConnection> ret = this.service.List(false, 1, 200, filter, include, exclude, fields);
            return ret;

        }

       
    }
}
