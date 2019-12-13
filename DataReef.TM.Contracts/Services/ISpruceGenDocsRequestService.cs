using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Models.Spruce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]    
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface ISpruceGenDocsRequestService : IDataService<GenDocsRequest>
    {
    }
}
