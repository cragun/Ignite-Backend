using System;
using System.Linq;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.DataAccess.Database;
using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Authorization;
using System.ServiceModel.Activation;
using System.ServiceModel;
using DataReef.Core.Services;
using System.Collections.Generic;
using DataReef.Core.Infrastructure.Repository;
using DataReef.TM.Models.Layers;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class LayerService : DataService<Layer>, ILayerService
    {

        public LayerService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory)
            : base(logger, unitOfWorkFactory)
        {
        }





        public override System.Collections.Generic.ICollection<Layer> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "")
        {
            Guid rootOUID = SmartPrincipal.OuId;
            return GetLayersForOU(rootOUID, deletedItems);
        }

        public ICollection<Layer> GetLayersForOU(Guid ouID, bool deletedItems)
        {
            List<Layer> ret = new List<Layer>();

            using (DataContext dc = new DataContext())
            {

                ret = dc
                    .Database
                    .SqlQuery<Layer>("exec proc_LayersForOU {0}", ouID)
                    .Where(o => deletedItems || (!deletedItems && !o.IsDeleted))
                    .ToList();
            }

            return ret;
        }


    }
}
