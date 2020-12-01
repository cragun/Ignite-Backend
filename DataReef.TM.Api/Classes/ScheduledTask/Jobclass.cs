using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using DataReef.Core;
using System.Data.Entity.Validation;

namespace DataReef.TM.Api.Classes.ScheduledTask
{
    public class Jobclass : Quartz.IJob
    {
        public void Execute(Quartz.IJobExecutionContext context)
        {
            ApiLogEntry apilog = new ApiLogEntry();
            apilog.Id = Guid.NewGuid();
            apilog.User = SmartPrincipal.UserId.ToString();
            apilog.Machine = Environment.MachineName;
            apilog.RequestContentType = "Scheduler Job " + DateTime.Now;

            using (var dc = new DataContext())
            {
                dc.ApiLogEntries.Add(apilog);
                dc.SaveChanges();

                var loginday = dc.AppSettings.FirstOrDefault(a => a.Key == Constants.LoginDays);
                int logindays = loginday != null ? Convert.ToInt32(loginday.Value) : 0; 

                try
                {
                    var Credentials = dc.Credentials.ToList();
                    var People = dc.People.ToList();
                    var Users = dc.Users.ToList();
                    var Authentications = dc.Authentications.ToList();

                    foreach (var c in Credentials)
                    {
                        if (c != null && !c.IsDeleted)
                        {
                            var dayvalidation = Authentications.Where(a => a.UserID == c.UserID).ToList();

                            DateTime oldDate = System.DateTime.UtcNow.AddDays(-(logindays));

                            var lastLoginCount = dayvalidation.Count(id => id.DateAuthenticated.Date >= oldDate.Date);

                            var person = People.SingleOrDefault(p => p.Guid == c.UserID
                                   && p.IsDeleted == false);

                            bool isDeactivate = false; 


                            //if (person != null && !person.IsDeleted && person.SBLastActivityDate != null)
                            //{
                            //    if (person.SBLastActivityDate.Value.Date >= oldDate.Date)
                            //    {
                            //        isDeactivate = true;
                            //    };
                            //}
                             
                            //if (dayvalidation.Count > 0 && lastLoginCount == 0 &&  isDeactivate == true)
                            if (dayvalidation.Count > 0 && lastLoginCount == 0)
                            { 
                                if (person != null && !person.IsDeleted)
                                {
                                    person.IsDeleted = true;
                                    person.SBActivityName = "Active";
                                    person.SBLastActivityDate = DateTime.Now.AddDays(-1).Date;
                                }

                                var user = Users.SingleOrDefault(u => u.PersonID == c.UserID
                                               && u.IsDeleted == false);

                                if (user != null && !user.IsDeleted)
                                {
                                    user.IsDeleted = true;
                                }

                                if (c != null && !c.IsDeleted)
                                {
                                    c.IsDeleted = true;
                                }

                                dc.SaveChanges();
                            }

                            //send mail 

                            string mailbody = "<p>SMARTBOARD ID : " + person.SmartBoardID + "</p><p>User ID : " + person.Guid + "</p><p>UserName : " + person.Name + "</p>"; 

                            Mail.Library.SendEmail("support@smartboardcrm.com", string.Empty, $"User Deactivation", mailbody , true);
                            Mail.Library.SendEmail("mdhakecha@gmail.com", string.Empty, $"User Deactivation", mailbody , true);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
    }
}