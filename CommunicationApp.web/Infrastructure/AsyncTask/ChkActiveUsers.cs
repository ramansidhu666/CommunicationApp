using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Data.Entity;
using System.Web.Configuration;
using System.Configuration;
//using System.Net;
//using System.Text;
//using Newtonsoft.Json.Linq;


using Quartz;
using CommunicationApp.Infrastructure;
using CommunicationApp.Web.Models;
using CommunicationApp.Services;
using AutoMapper;
using CommunicationApp.Entity;
using CommunicationApp.Core.Infrastructure;
using CommunicationApp.Web.Infrastructure.PushNotificationFile;
using System.Data;

namespace CommunicationApp.Web.Infrastructure.AsyncTask
{
    public class ActiveUsersJob : IJob
    {

        public void Execute(IJobExecutionContext context)
        {

            JobDataMap dataMap = context.JobDetail.JobDataMap;           

            //check which person shutdown your phone without logout.

            CommonClass CommonClass = new Services.CommonClass();
            string QStr = "";
            DataTable dt = new DataTable();
            QStr = "Select * From Customer";
            dt = CommonClass.GetDataSet(QStr).Tables[0];

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["LastUpdatedOn"] != DBNull.Value)
                {
                    UpdateActiveUser(Convert.ToDateTime(dt.Rows[0]["LastUpdatedOn"]), Convert.ToInt32(dt.Rows[0]["CustomerId"]));
                }

            }


            //end
        }

        public void UpdateActiveUser(DateTime LastUpdatedOn, int CustomerId)
        {


            TimeSpan ts = DateTime.Now - Convert.ToDateTime(LastUpdatedOn);
            if (ts.Days > 0)
            {

                CommonClass CommonClass = new Services.CommonClass();
                string QStr = "";
                DataTable dt = new DataTable();
                QStr = "Select * From Customer where CustomerId=" + CustomerId;
                dt = CommonClass.GetDataSet(QStr).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    QStr = "update Customer set  IsAvailable=0 where CustomerId=" + CustomerId;
                    CommonClass.GetDataSet(QStr);
                }
            }



        }

    }
}