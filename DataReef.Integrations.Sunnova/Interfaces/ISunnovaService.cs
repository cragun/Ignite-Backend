using System.ServiceModel;
using System.Web.ApplicationServices;

namespace DataReef.Integrations.Sunnova.Interfaces
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface ISunnovaService
    {
    }
}
