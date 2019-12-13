using DataReef.Core.Classes;
using DataReef.Core.Enums;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.Integrations;
using DataReef.Integrations.Common.Geo;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.Properties;
using DataReef.TM.Models.DTOs.SmartBoard;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.PubSubMessaging;
using DataReef.TM.Models.Solar;
using DataReef.TM.Services.Extensions;
using DataReef.TM.Services.InternalServices.Geo;
using DataReef.TM.Services.Services.FinanceAdapters.SolarSalesTracker;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Data.SqlTypes;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;
using Property = DataReef.TM.Models.Property;
using PropertyAttribute = DataReef.TM.Models.PropertyAttribute;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class PropertySurveyService : DataService<PropertySurvey>, IPropertySurveyService
    {

        public PropertySurveyService(ILogger logger,
            Func<IUnitOfWork> unitOfWorkFactory)
            : base(logger, unitOfWorkFactory)
        {
            
        }

        public IEnumerable<PropertySurveyDTO> GetPropertySurveysForUser(Guid userID, int pageIndex = 0, int itemsPerPage = 20)
        {
            using(var dc = new DataContext())
            {
                var matchingSurveys = dc
                    .PropertySurveys
                    .Where(x => !x.IsDeleted && x.PersonID == userID)
                    ?.GroupBy(x => x.PropertyID)
                    ?.Select(x => x.OrderByDescending(p => p.DateCreated).FirstOrDefault())
                    ?.OrderByDescending(x => x.DateCreated)
                    ?.Skip(pageIndex * itemsPerPage)
                    ?.Take(itemsPerPage)
                    ?.ToList();

                var propertyIds = matchingSurveys?.Select(x => x.PropertyID) ?? new List<Guid>();
                var matchingProperties = dc.Properties.Where(x => propertyIds.Contains(x.Guid)).ToList();

                return matchingSurveys?.Select(x =>
                {
                    x.Property = matchingProperties.FirstOrDefault(p => p.Guid == x.PropertyID);

                    return new PropertySurveyDTO(x);
                });
            }
        }

        public PropertySurveyDTO GetPropertySurveyDTO(Guid propertySurveyID)
        {
            using(var dc = new DataContext())
            {
                var propertySurvey = dc
                    .PropertySurveys
                    .Include(x => x.Property)
                    .FirstOrDefault(x => !x.IsDeleted && x.Guid == propertySurveyID);

                return new PropertySurveyDTO(propertySurvey);
            }
        }
    }
}
