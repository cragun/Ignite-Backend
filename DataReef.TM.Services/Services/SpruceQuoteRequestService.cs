using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models.Spruce;
using EntityFramework.Extensions;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataReef.Core.Infrastructure.Repository;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class SpruceQuoteRequestService : DataService<QuoteRequest>, ISpruceQuoteRequestService
    {
        public SpruceQuoteRequestService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory) { }

        public override QuoteRequest Insert(QuoteRequest entity)
        {
            using (var dataContext = new DataContext())
            {
                using (var transaction = dataContext.Database.BeginTransaction())
                {
                    try
                    {
                        var quotes = dataContext
                                                .QuoteRequests
                                                .Include("AppEmployment")
                                                .Include("CoAppEmployment")
                                                .Where(q => q.LegionPropertyID == entity.LegionPropertyID)
                                                .ToList();

                        var quoteRequestIds = quotes
                                                .Select(q => q.Guid)
                                                .ToList();

                        // need these checks because of an issue entityFramework.Extended has w/ emptyList.Contains
                        if (quoteRequestIds.Count > 0)
                        {
                            // delete from [Spruce].[Applicants]
                            dataContext
                                    .Applicants
                                    .Where(a => quoteRequestIds.Contains(a.Guid))
                                    .Delete();

                            // delete from [Spruce].[CoApplicants]
                            dataContext
                                    .CoApplicants
                                    .Where(a => quoteRequestIds.Contains(a.Guid))
                                    .Delete();

                            // delete from [Spruce].[Inits]
                            dataContext
                                    .Inits
                                    .Where(a => quoteRequestIds.Contains(a.Guid))
                                    .Delete();

                            // delete from [Spruce].[Properties]
                            dataContext
                                    .SpruceProperties
                                    .Where(a => quoteRequestIds.Contains(a.Guid))
                                    .Delete();

                            // delete from [Spruce].[GenDocsRequests]
                            dataContext
                                    .GenDocsRequests
                                    .Where(a => quoteRequestIds.Contains(a.Guid))
                                    .Delete();

                            // delete from [Spruce].[QuoteResponses] 
                            dataContext
                                    .QuoteResponses
                                    .Where(a => quoteRequestIds.Contains(a.Guid))
                                    .Delete();

                            // delete from [Spruce].[QuoteRequests]
                            dataContext
                                    .QuoteRequests
                                    .Where(a => quoteRequestIds.Contains(a.Guid))
                                    .Delete();
                        }

                        var appIds = quotes
                                    .Where(q => q.AppEmployment != null)
                                    .Select(q => q.AppEmployment.Guid)
                                    .ToList();

                        var coAppIds = quotes
                                        .Where(q => q.CoAppEmployment != null)
                                        .Select(q => q.CoAppEmployment.Guid)
                                        .ToList();

                        // need these checks because of an issue entityFramework.Extended has w/ emptyList.Contains
                        if (appIds.Count > 0)
                        {
                            // delete from [Spruce].[Employments]
                            dataContext
                                .Employments
                                .Where(a => appIds.Contains(a.Guid))
                                .Delete();

                            // delete from [Spruce].[IncomeDebts]
                            dataContext
                                .IncomeDebts
                                .Where(id => appIds.Contains(id.Guid))
                                .Delete();
                        }

                        // need these checks because of an issue entityFramework.Extended has w/ emptyList.Contains
                        if (coAppIds.Count > 0)
                        {
                            // delete from [Spruce].[Employments]
                            dataContext
                                .Employments
                                .Where(a => coAppIds.Contains(a.Guid))
                                .Delete();

                            // delete from [Spruce].[IncomeDebts]
                            dataContext
                                .IncomeDebts
                                .Where(id => coAppIds.Contains(id.Guid))
                                .Delete();
                        }

                        dataContext.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }

            return base.Insert(entity);
        }
    }
}
