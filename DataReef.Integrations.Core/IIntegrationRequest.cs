using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Core
{
    public interface IIntegrationRequest
    {
        Guid PersonID { get; set; }
        string IntegratorName { get; set; }
        string DeviceID { get; set; }
        Guid OUID { get; set; }
        string OUName { get; set; }
        string RequestType { get; set; }
        IIntegrationData IntegrationData { get; set; }
    }
}
