using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Models.Spruce;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface ISpruceQuoteRequestService : IDataService<QuoteRequest>
    {
    }
}