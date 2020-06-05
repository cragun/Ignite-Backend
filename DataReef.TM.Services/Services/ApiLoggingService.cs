using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class ApiLoggingService : IApiLoggingService
    {
        public void LogToDatabase(ApiLogEntry apiLogEntry)
        {/*
            using(var dc = new DataContext())
            {
                if (dc.AppSettings.Any(s => s.Key == "LogApiCalls" && s.Value == "true"))
                {
                    dc.ApiLogEntries.Add(apiLogEntry);
                }
            }*/


            using (var dc = new DataContext())
            {
                //if (apiLogEntry.RequestMethod == "CreateNoteFromSmartboardtocheck")
                //{
                //    dc.ApiLogEntries.Add(apiLogEntry);
                //    dc.SaveChanges();
                //}
            }

        }
    }
}
