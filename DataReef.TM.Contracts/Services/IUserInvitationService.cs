using System.ServiceModel;
using DataReef.TM.Models;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models.DTOs.Persons;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IUserInvitationService : IDataService<UserInvitation>
    {

        UserInvitation Insert(UserInvitation entity, DataContext dc);

        UserInvitation SilentInsertFromSmartboard(CreateUserDTO user, string apiKey);
    }
}