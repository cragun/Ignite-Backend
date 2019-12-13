using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using System.Web.Mvc;
using Microsoft.Practices.ObjectBuilder2;
using DataReef.TM.Api.Areas.HelpPage.Extensions;
using DataReef.TM.Api.Areas.HelpPage.Models;
using DataReef.TM.Api.Areas.HelpPage.Services;
using DataReef.TM.Api.Areas.HelpPage.Services.ModelDescriptions;
using DataReef.TM.Api.Common;
using System.Web.Http.OData;

namespace DataReef.TM.Api.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page
    /// </summary>
    [RouteArea("HelpPage", AreaPrefix = "help")]
    public class HelpController : Controller
    {
        private readonly IApiObjectSampleGeneratorService _apiObjectSampleGenerator;
        private readonly IApiModelDescriptionGenerator _apiModelDescriptionGenerator;

        private const string ErrorViewName = "Error";

        /// <summary>
        /// Actual constructor
        /// </summary>
        public HelpController(IApiObjectSampleGeneratorService apiObjectSampleGenerator, IApiModelDescriptionGenerator apiModelDescriptionGenerator)
        {
            this.Configuration = GlobalConfiguration.Configuration;
            this._apiObjectSampleGenerator = apiObjectSampleGenerator;
            this._apiModelDescriptionGenerator = apiModelDescriptionGenerator;
        }

        /// <summary>
        /// HttpConfiguration
        /// </summary>
        public HttpConfiguration Configuration { get; private set; }

        /// <summary>
        /// Basic controller action that returns data for the main index page
        /// </summary>
        /// <returns>The <see cref="ActionResult"/> for the index page</returns>
        [System.Web.Mvc.Route("")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Retrieve information about a specific controller
        /// </summary>
        /// <param name="controllerName">The name of the controller</param>
        /// <returns>The <see cref="ActionResult"/> for the controller description page</returns>
        [System.Web.Mvc.Route("Controller/{controllerName}")]
        public ActionResult Controller(string controllerName)
        {
            var apiControllerDescriptors = GetControllerActionsDescription(controllerName, this.Configuration);

            var model = new HelpPageControllerModel { Actions = apiControllerDescriptors, ControllerName = controllerName };

            return View(model);
        }

        /// <summary>
        /// Retrieve information about a entity type
        /// </summary>
        /// <param name="modelName">The name of the model</param>
        /// <returns>The <see cref="ActionResult"/> containing the model description</returns>
        [System.Web.Mvc.Route("ResourceModel/{modelName}")]
        public ActionResult ResourceModel(string modelName)
        {
            if (!String.IsNullOrEmpty(modelName))
            {
                ModelDescription modelDescription = this._apiModelDescriptionGenerator.GetOrCreateModelDescription(modelName);
                if (modelDescription != null)
                    return View(modelDescription);
            }

            return View(ErrorViewName);
        }

        /// <summary>
        /// Retrieve information about a specific controller action
        /// </summary>
        /// <param name="actionIdentifier">The action identifier composed of: HttpRequestType-ControllerName-ActionName</param>
        /// <returns>The <see cref="JsonResult"/> corresponding to the controller action</returns>
        [System.Web.Mvc.Route("ActionInteraction/{actionIdentifier}")]
        public JsonResult ActionInteraction(string actionIdentifier)
        {
            var actionIdentifierParts = actionIdentifier.Split('-');
            var actionType = new HttpMethod(actionIdentifierParts[0]);
            var controllerName = actionIdentifierParts[1];
            var actionName = actionIdentifierParts[2];

            var model = GetControllerInteractionAction(controllerName, actionName, actionType, this.Configuration, this._apiObjectSampleGenerator);

            return new JsonNetResult { Data = model };
        }

        private static readonly Type[] FilterByDescriptor = { typeof(ComplexTypeModelDescription), typeof(EnumTypeModelDescription) };
        private static readonly Type[] ExcludedDisplayModels = { typeof(Delta<>), typeof(Object), typeof(IntPtr) };

        /// <summary>
        /// Retrieve information about all the controllers and models the API exposes
        /// </summary>
        /// <returns>The <see cref="JsonResult"/> containing the full API description</returns>
        [System.Web.Mvc.Route("ApiDescriptionDashboard")]
        public JsonResult ApiDescriptionDashboard()
        {
            var controllerApiDescriptions = GetControllerApiDescriptions(this.Configuration);
            var documentationProvider = this.Configuration.Services.GetDocumentationProvider();
            var model = new HelpPageApiDescriptionModel();

            controllerApiDescriptions.ForEach(cad => model.ResouceModels.Add(new HelpPageApiResourceModel
            {
                Name = cad.ControllerName,
                Description = documentationProvider.GetDocumentation(cad.ControllerDescriptor),
                Type = HelpPageApiResourceType.Controller,
                Url = Url.Action("Controller", new { controllerName = cad.ControllerName })
            }));

            foreach (var apiModelDescription in this._apiModelDescriptionGenerator.GetAllModelDescriptions())
            {

                if (FilterByDescriptor.Contains(apiModelDescription.GetType()) &&
                    ExcludedDisplayModels.All(m => m.IsGenericType && apiModelDescription.ModelType.IsGenericType ? apiModelDescription.ModelType.GetGenericTypeDefinition() != m : m != apiModelDescription.ModelType))
                    model.ResouceModels.Add(new HelpPageApiResourceModel
                    {
                        Name = apiModelDescription.Name,
                        Description = apiModelDescription.Documentation,
                        Type = apiModelDescription.ModelType.IsEnum ? HelpPageApiResourceType.Enum : HelpPageApiResourceType.ComplexType,
                        Url = Url.Action("ResourceModel", new { modelName = apiModelDescription.Name })
                    });
            }

            return new JsonNetResult { Data = model };
        }

        private static HelpPageControllerActionInteractionModel GetControllerInteractionAction(string controllerName, string actionName, HttpMethod httpMethod,
            HttpConfiguration configuration, IApiObjectSampleGeneratorService apiObjectSampleGenerator)
        {
            var controllerApiDescription = GetControllerApiDescriptions(configuration).FirstOrDefault(cad => cad.ControllerName == controllerName);

            if (controllerApiDescription == null)
                return null;

            var apiDescription = controllerApiDescription.ApiDescription.FirstOrDefault(a => a.ActionDescriptor.ActionName.Equals(actionName) && a.HttpMethod == httpMethod);

            if (apiDescription == null)
                return null;

            var sampleGenerationErrors = new Collection<string>();
            var actionInteractionModel = new HelpPageControllerActionInteractionModel
            {
                ActionUrl = apiDescription.RelativePath.ToAbsoluteUrl(),
                SampleResponses = apiObjectSampleGenerator.GenerateResponseSampleForApi(apiDescription, sampleGenerationErrors),
                SampleRequests = apiObjectSampleGenerator.GenerateRequestSampleForApi(apiDescription, sampleGenerationErrors),
                SampleGenerationErrors = sampleGenerationErrors,
                HttpActionVerb = httpMethod.ToString()
            };

            configuration.LoadHelpPageControllerActionInteractionModel(actionInteractionModel, apiDescription);

            return actionInteractionModel;
        }

        private static ICollection<HelpPageControllerActionDescriptionModel> GetControllerActionsDescription(string controllerName, HttpConfiguration configuration)
        {
            var controllerActions = new List<HelpPageControllerActionDescriptionModel>();

            var controllerApiDescription = GetControllerApiDescriptions(configuration).FirstOrDefault(cad => cad.ControllerName == controllerName);
            if (controllerApiDescription == null)
                return controllerActions;

            var actionGroupedApiDescriptions = controllerApiDescription.ApiDescription.GroupBy(ead => new { ead.ActionDescriptor.ActionName, ead.HttpMethod });
            foreach (var actionGroupedApiDescription in actionGroupedApiDescriptions)
            {
                var mostFriendlyActionDescription = GetMostFriendlyActionDescrption(actionGroupedApiDescription);

                var helpPageControllerActionModel = CreateHelpPageControllerActionModel(mostFriendlyActionDescription, actionGroupedApiDescription);
                configuration.LoadHelpPageControllerActionModel(helpPageControllerActionModel, mostFriendlyActionDescription);
                controllerActions.Add(helpPageControllerActionModel);
            }

            return controllerActions;
        }

        private static IEnumerable<ControllerApiDescription> GetControllerApiDescriptions(HttpConfiguration configuration)
        {
            var controllerDescriptions = new List<ControllerApiDescription>();

            var apiDescriptions = configuration.Services.GetApiExplorer().ApiDescriptions;
            foreach (var apiDescription in apiDescriptions)
            {
                var controllerDescriptor = apiDescription.ActionDescriptor.ControllerDescriptor;
                var controllerName = controllerDescriptor.ControllerName;
                var existingControllerApiDescription = controllerDescriptions.FirstOrDefault(cd => cd.ControllerName.Equals(controllerName, StringComparison.InvariantCulture));

                if (existingControllerApiDescription == null)
                {
                    controllerDescriptions.Add(new ControllerApiDescription
                    {
                        ControllerName = controllerName,
                        ControllerDescriptor = controllerDescriptor,
                        ApiDescription = new List<ApiDescription> { apiDescription }
                    });

                    continue;
                }

                existingControllerApiDescription.ApiDescription.Add(apiDescription);
            }

            return controllerDescriptions;
        }

        private static ApiDescription GetMostFriendlyActionDescrption(IEnumerable<ApiDescription> apiDescriptions)
        {
            // the apiDescription that has Route of type HttpRoute is an explicit description, the others HostedHttpRoutes are inferred
            var apiDescription = apiDescriptions.FirstOrDefault(ad => ad.Route.GetType() == typeof(HttpRoute));

            if (apiDescription != null)
                return apiDescription;

            return apiDescriptions.OrderBy(ad => ad.RelativePath).First();
        }

        private static HelpPageControllerActionDescriptionModel CreateHelpPageControllerActionModel(ApiDescription mostFriendlyActionDescription, IEnumerable<ApiDescription> actionGroupedApiDescription)
        {
            var isCrudApiAction = mostFriendlyActionDescription.ActionDescriptor.ActionBinding.ActionDescriptor.GetCustomAttributes<CrudApiActionAttribute>().Any();
            return new HelpPageControllerActionDescriptionModel
            {
                ActionName = mostFriendlyActionDescription.ActionDescriptor.ActionName,
                ActionDescription = mostFriendlyActionDescription.Documentation,
                ActionType = mostFriendlyActionDescription.HttpMethod,
                ActionUri = mostFriendlyActionDescription.RelativePath,
                IsPartOfCrudApi = isCrudApiAction,
                AlternativeUris = actionGroupedApiDescription.Where(g => g.RelativePath != mostFriendlyActionDescription.RelativePath).Select(a => a.RelativePath)
            };
        }

        [DebuggerDisplay("{ControllerName, nq} - ActionDescriptions: {ApiDescription.Count}")]
        private class ControllerApiDescription
        {
            public string ControllerName { get; set; }
            public HttpControllerDescriptor ControllerDescriptor { get; set; }
            public List<ApiDescription> ApiDescription { get; set; }
        }
    }
}