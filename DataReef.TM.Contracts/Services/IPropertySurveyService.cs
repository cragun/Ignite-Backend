using DataReef.TM.Models.DataViews;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataReef.TM.Models.DTOs;
using Property = DataReef.TM.Models.Property;
using DataReef.TM.Models.Solar;
using DataReef.TM.Models.DTOs.Properties;
using DataReef.TM.Models.DTOs.SmartBoard;
using DataReef.TM.Models;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IPropertySurveyService : IDataService<PropertySurvey>
    {
        [OperationContract]
        Task<IEnumerable<PropertySurveyDTO>> GetPropertySurveysForUser(Guid userID, int pageIndex = 0, int itemsPerPage = 20);

        [OperationContract]
        PropertySurveyDTO GetPropertySurveyDTO(Guid propertySurveyID);
    }
}
