using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.ZipCodes;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DataReef.Core.Infrastructure.Repository;

namespace DataReef.TM.Services
{
    public class AreaPurchaseService : DataService<AreaPurchase>, IAreaPurchaseService
    {
        private readonly ILogger _logger;

        public AreaPurchaseService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory)
            : base(logger, unitOfWorkFactory)
        {
            _logger = logger;
        }

        public ICollection<AreaPurchase> GetPurchasesForOU(Guid ouid)
        {
            using (DataContext dataContext = new DataContext())
            {
                var rootOU = dataContext.OUs.SingleOrDefault(ou => ou.Guid == ouid);
                if (rootOU.RootOrganizationID.Value != ouid) rootOU = dataContext.OUs.SingleOrDefault(ou => ou.Guid == rootOU.RootOrganizationID.Value);
                var purchases = dataContext.ZipAreas.Where(za => za.OUID == rootOU.Guid).SelectMany(za => za.Purchases).OrderByDescending(p => p.DateCreated).ThenBy(p => p.CompletionDate).ToList();
                return purchases;
            }
        }

        public ICollection<AreaPurchaseDto> GetAllPendingPurchases()
        {
            using (DataContext dataContext = new DataContext())
            {
                var superUserOUIDs = dataContext.OUAssociations.Where(oua => oua.PersonID == SmartPrincipal.UserId && oua.OURoleID.Equals(OURole.PaymentProcessorID)).Select(oua => oua.OUID).ToList();
                if (!superUserOUIDs.Any()) return new List<AreaPurchaseDto>();

                var pendingPurchases = dataContext.AreaPurchases
                                        .Where(ap => ap.Status == AreaPurchaseStatus.Pending && ap.IsDeleted == false && superUserOUIDs.Contains(ap.OU.RootOrganizationID.Value))
                                        .Select(ap => new AreaPurchaseDto
                                        {
                                            Guid = ap.Guid,
                                            OUName = ap.OU.Name,
                                            OUID = ap.OUID,
                                            BuyerName = ap.Person.FirstName + " " + ap.Person.LastName,
                                            ZipAreaName = ap.Area.Name,
                                            NumberOfTokens = ap.NumberOfTokens,
                                            TokenPriceInDollars = ap.TokenPriceInDollars,
                                            DateCreated = ap.DateCreated
                                        }
                                        )
                                        .OrderBy(p => p.DateCreated).ToList();
                return pendingPurchases;
            }
        }

        public void UpdateProcessedPurchases(ICollection<AreaPurchase> processedPurchases)
        {
            if (processedPurchases == null || !processedPurchases.Any()) return;
            bool saveChanges = false;
            using (DataContext dataContext = new DataContext())
            {
                var superUserOUIDs = dataContext.OUAssociations.Where(oua => oua.PersonID == SmartPrincipal.UserId && oua.OURoleID.Equals(OURole.PaymentProcessorID)).Select(oua => oua.OUID).ToList();
                if (!superUserOUIDs.Any()) return;

                foreach (var processedPurchase in processedPurchases)
                {
                    if (!superUserOUIDs.Contains(processedPurchase.OUID)) continue;
                    var purchase = dataContext
                        .AreaPurchases
                        .Include(ap => ap.Area)
                        .SingleOrDefault(p => p.Guid == processedPurchase.Guid && p.IsDeleted == false && p.Area.IsDeleted == false);
                    if (purchase == null) continue;
                    purchase.Status = processedPurchase.Status;
                    purchase.ErrorString = processedPurchase.ErrorString;
                    purchase.CompletionDate = DateTime.UtcNow;
                    var area = purchase.Area;
                    if (purchase.Status == AreaPurchaseStatus.Completed)
                    {  
                        area.Status = ZIPAreaStatus.Pending;
                        area.ActiveStartDate = purchase.CompletionDate.Value.AddDays(3);
                    }
                    else if (purchase.Status == AreaPurchaseStatus.Denied)
                    {
                        var latestValidAreaPurchase = dataContext.AreaPurchases.Where(ap => ap.AreaID == area.Guid && ap.Status != AreaPurchaseStatus.Denied).OrderByDescending(ap => ap.DateCreated).FirstOrDefault();
                        if (latestValidAreaPurchase == null)
                        {
                            area.Status = ZIPAreaStatus.NotPurchased;
                            area.ActiveStartDate = null;
                            area.LastPurchaseDate = null;
                        }
                        else
                        {
                            area.Status = latestValidAreaPurchase.Status == AreaPurchaseStatus.Pending ? ZIPAreaStatus.Pending : ZIPAreaStatus.Active;
                            area.ActiveStartDate = latestValidAreaPurchase.CompletionDate.HasValue ? latestValidAreaPurchase.CompletionDate.Value.AddDays(3) : (DateTime?)null;
                            area.LastPurchaseDate = latestValidAreaPurchase.DateCreated;
                        }
                    }

                    saveChanges = true;
                }
                if (saveChanges) dataContext.SaveChanges();
            }
        }

        public override AreaPurchase Insert(AreaPurchase entity)
        {
            if (entity.Status != AreaPurchaseStatus.Pending || entity.CompletionDate.HasValue) throw new Exception("Cannot modify purchase status");
            return base.Insert(entity);
        }

        public override ICollection<AreaPurchase> InsertMany(ICollection<AreaPurchase> entities)
        {
            foreach (var entity in entities) if (StatusChangeAttempt(entity)) throw new Exception("Cannot modify purchase status");
            return base.InsertMany(entities);
        }

        public override U Update<U>(U entity, DataContext dataContext, bool saveContext = true)
        {
            var areaPurchase = entity as AreaPurchase;
            if (StatusChangeAttempt(areaPurchase))
            {
                using (DataContext dc = new DataContext())
                {
                    var superUserOUIDs = dataContext.OUAssociations.Where(oua => oua.PersonID == SmartPrincipal.UserId && oua.OUID == areaPurchase.OUID && oua.OURoleID.Equals(OURole.PaymentProcessorID));
                    if (!superUserOUIDs.Any()) throw new Exception("Cannot modify purchase status");
                }
            }
            return base.Update<U>(entity, dataContext);
        }

        private bool StatusChangeAttempt(AreaPurchase entity)
        {
            return entity.Status != AreaPurchaseStatus.Pending || entity.CompletionDate.HasValue;
        }
    }
}
