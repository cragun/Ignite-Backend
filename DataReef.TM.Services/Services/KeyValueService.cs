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
    public class KeyValueService : DataService<KeyValue>, IKeyValueService
    {

        public KeyValueService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory)
            : base(logger, unitOfWorkFactory)
        {
        }

      
        //public override System.Collections.Generic.ICollection<Layer> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "")
        //{
        //    Guid rootOUID = SmartPrincipal.OuId;
        //    return GetLayersForOU(rootOUID, deletedItems);
        //}

        //public ICollection<Layer> GetLayersForOU(Guid ouID, bool deletedItems)
        //{
        //    List<Layer> ret = new List<Layer>();

        //    using (DataContext dc = new DataContext())
        //    {

        //        ret = dc
        //            .Database
        //            .SqlQuery<Layer>("exec proc_LayersForOU {0}", ouID)
        //            .Where(o => deletedItems || (!deletedItems && !o.IsDeleted))
        //            .ToList();
        //    }

        //    return ret;
        //}

        public KeyValue Upsert(KeyValue kv)
        {

            if (string.IsNullOrWhiteSpace(kv.Key))
            {
                throw new ApplicationException("Key cannot be null");
            }

            try
            {
                using (DataContext dc = new DataContext())
                {

                    //next see if the ObjectID is a Proposal, Property or Account
                    bool canContinue = false;
                    if (dc.Proposal.Any(pp => pp.Guid == kv.ObjectID)) canContinue = true;
                    else if (dc.Accounts.Any(pp => pp.Guid == kv.ObjectID)) canContinue = true;
                    else if (dc.Properties.Any(pp => pp.Guid == kv.ObjectID)) canContinue = true;
                    else if (dc.Inquiries.Any(pp => pp.Guid == kv.ObjectID)) canContinue = true;
                    else if (dc.Users.Any(pp => pp.Guid == kv.ObjectID)) canContinue = true;
                    else if (dc.OUs.Any(pp => pp.Guid == kv.ObjectID)) canContinue = true;


                    if (!canContinue)
                    {
                        throw new ApplicationException("Invalid ObjectID - must be Proposal, Account, Disposition, USer,OU or Property");
                    }


                    var existing = dc.KeyValues.Where(kkv => kkv.ObjectID == kv.ObjectID && kkv.Key == kv.Key).FirstOrDefault();

                    if (existing == null)
                    {
                        dc.KeyValues.Add(kv);
                        kv.DateCreated = System.DateTime.UtcNow;
                        kv.DateLastModified = kv.DateCreated;
                        kv.Version = 1;
                        dc.SaveChanges();
                        kv.SaveResult = SaveResult.SuccessfulInsert;
                    }
                    else
                    {
                        if (existing.Value != kv.Value)
                        {
                            existing.Value = kv.Value;
                            existing.Version += 1;
                            existing.DateLastModified = System.DateTime.UtcNow;
                            dc.SaveChanges();
                            kv.SaveResult = SaveResult.SuccessfulUpdate;
                        }
                    }
                }
            }
            catch(System.Exception ex)
            {
                kv.SaveResult = SaveResult.FromException(ex, Core.Enums.DataAction.Update);
            }

            return kv;
            
        }
    }
}
