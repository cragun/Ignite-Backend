using System.ServiceModel;
using System.Web.ApplicationServices;
using DataReef.Integrations.MailChimp.Models;

namespace DataReef.Integrations.MailChimp
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IMailChimpAdapter
    {
        [OperationContract]
        MailChimpUserDetails RegisterUser(string emailAddress);

        [OperationContract]
        MailChimpUserDetails UnregisterUser(string emailAddress);
    }
}
