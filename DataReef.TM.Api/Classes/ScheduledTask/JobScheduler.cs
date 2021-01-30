﻿using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes.ScheduledTask
{
    public class JobScheduler
    {
        public static void Start()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler scheduler = schedFact.GetScheduler();

            scheduler.Start();

            IJobDetail job = JobBuilder.Create<Jobclass>().Build();

            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
            .WithDailyTimeIntervalSchedule(x => x
            .WithIntervalInHours(24) 
            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(12, 00)))
            .Build();


            scheduler.ScheduleJob(job, trigger);
        }
    }
}