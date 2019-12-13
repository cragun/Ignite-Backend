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


namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Managers CRUD for PersonSetting.
    /// </summary>
    [RoutePrefix("api/v1/personsettings")]
    public class PersonSettingsController : EntityCrudController<PersonSetting>
    {
        private readonly IDataService<PersonSetting> personSettingsService;

        public PersonSettingsController(IDataService<PersonSetting> service, ILogger logger)
            : base(service, logger)
        {
            this.personSettingsService = service;
        }

    }
}
