using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;
using CommunicationApp.App_Start;
using System.Web.Mvc;
using Ninject;
using Ninject.Web.Common;
using System.Configuration;
using CommunicationApp.Data;
using CommunicationApp.Models;

namespace CommunicationApp.Web.Infrastructure.AsyncTask
{
    public class JobScheduler
    {

        public static void SendEmailProperty(string PropertyId, string Subject, string Body, string PropertyTypeStatus)
        {
          
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail EmailJobs = JobBuilder.Create<SendMail>().UsingJobData("PropertyId", PropertyId).UsingJobData("Subject", Subject).UsingJobData("Body", Body).UsingJobData("PropertyTypeStatus", PropertyTypeStatus).Build();

            ITrigger EmailTrigger = TriggerBuilder.Create()
                .WithSimpleSchedule(s => s.WithIntervalInSeconds(30)) //It will send with in 30 seconds
                .Build();

            scheduler.ScheduleJob(EmailJobs, EmailTrigger);
        }
        public static void SendNotificationProperty(string PropertyId,string Flag, string Message)
        {
            
           IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail EmailJobs = JobBuilder.Create<SendNotification>().UsingJobData("PropertyId", PropertyId).UsingJobData("Flag", Flag).UsingJobData("Message", Message).Build();

            ITrigger EmailTrigger = TriggerBuilder.Create()
                .WithSimpleSchedule(s => s.WithIntervalInSeconds(30)) //It will send with in 30 seconds
                .Build();
            scheduler.ScheduleJob(EmailJobs, EmailTrigger);
        }
        public static void SendNotificationOpenHouse(string AgentId, string Flag, string Message)
        {

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail EmailJobs = JobBuilder.Create<OpenHouseNotificationJob>().UsingJobData("AgentId", AgentId).UsingJobData("Flag", Flag).UsingJobData("Message", Message).Build();

            ITrigger EmailTrigger = TriggerBuilder.Create()
                .WithSimpleSchedule(s => s.WithIntervalInSeconds(30)) //It will send with in 30 seconds
                .Build();
            scheduler.ScheduleJob(EmailJobs, EmailTrigger);
        }
        public static void SendMessageNotification(string AdminId, string CustomerIds, string Message, string Heading, string ImageUrl, string IsWithImage)
        {

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail EmailJobs = JobBuilder.Create<SendMessageJob>().UsingJobData("AdminIs", AdminId).UsingJobData("CustomerIds", CustomerIds).UsingJobData("Message", Message).UsingJobData("Heading", Heading).UsingJobData("ImageUrl", ImageUrl).UsingJobData("IsWithImage", IsWithImage).Build();

            ITrigger EmailTrigger = TriggerBuilder.Create()
                .WithSimpleSchedule(s => s.WithIntervalInSeconds(30)) //It will send with in 30 seconds
                .Build();
            scheduler.ScheduleJob(EmailJobs, EmailTrigger);
        }

        public static void SendNewsLetter(string AdminId, string CustomerIds,NewsletterModel model)
        {

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail EmailJobs = JobBuilder.Create<NewsletterJob>().UsingJobData("AdminIs", AdminId).UsingJobData("CustomerIds", CustomerIds).UsingJobData("P1", model.P1).UsingJobData("P2", model.P2).UsingJobData("P3", model.P3).UsingJobData("P4", model.P4).UsingJobData("P5", model.P5).UsingJobData("P6", model.P6).UsingJobData("P7", model.P7).UsingJobData("PG1", model.PG1).UsingJobData("PG2", model.PG2).UsingJobData("PG3", model.PG3).UsingJobData("PG4", model.PG4).UsingJobData("PG5", model.PG5).UsingJobData("PG6", model.PG6).UsingJobData("PG7", model.PG7).UsingJobData("AdminLogo", model.AdminLogo).UsingJobData("PropertyPhoto", model.PropertyPhoto).UsingJobData("FirstContent", model.FirstContent).UsingJobData("SecondContent", model.SecondContent).UsingJobData("ThirdContent", model.ThirdContent).UsingJobData("TemplateType", model.TemplateType).UsingJobData("PP1", model.PP1).UsingJobData("PP2", model.PP2).UsingJobData("PP3", model.PP3).UsingJobData("PP4", model.PP4).UsingJobData("PP5", model.PP5).UsingJobData("PP6", model.PP6).UsingJobData("PPG1", model.PPG1).UsingJobData("PPG2", model.PPG2).UsingJobData("PPG3", model.PPG3).UsingJobData("PPG4", model.PPG4).UsingJobData("PPG5", model.PPG5).UsingJobData("PPG6", model.PPG6).Build();
            
            ITrigger EmailTrigger = TriggerBuilder.Create()
                .WithSimpleSchedule(s => s.WithIntervalInSeconds(30)) //It will send with in 30 seconds
                .Build();
            scheduler.ScheduleJob(EmailJobs, EmailTrigger);
        }

        public static void UpdateActiveUser()
        {

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail EmailJobs = JobBuilder.Create<ActiveUsersJob>().Build();

            ITrigger EmailTrigger = TriggerBuilder.Create()
                .WithSimpleSchedule(s => s.WithIntervalInSeconds(30)) //It will send with in 30 seconds
                .Build();
            scheduler.ScheduleJob(EmailJobs, EmailTrigger);
        }

       
       
    }

}