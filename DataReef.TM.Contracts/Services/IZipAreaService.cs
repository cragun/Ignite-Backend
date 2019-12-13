using DataReef.TM.Models;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    public interface IZipAreaService : IDataService<ZipArea>
    {
    }
}
