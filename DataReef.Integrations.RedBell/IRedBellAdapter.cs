using System.ServiceModel;
using System.Web.ApplicationServices;
using DataReef.Integrations.RedBell.RedBellBeta;

namespace DataReef.Integrations.RedBell
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IRedBellAdapter
    {
        [OperationContract]
        decimal GetAveValue(OrderRequest rbRequest);
    }
}
