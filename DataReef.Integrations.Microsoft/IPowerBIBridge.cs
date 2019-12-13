using DataReef.Integrations.Microsoft.PowerBI.Models;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web.ApplicationServices;

namespace DataReef.Integrations.Microsoft
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IPowerBIBridge
    {
        void PushData(PBI_Base data);

        Task PushDataAsync(PBI_Base data);

        void PushData(PBI_Base[] data);

        Task PushDataAsync(PBI_Base[] data);

    }
}
