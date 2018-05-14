﻿using System;
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
using System.Data.SqlClient;

namespace CommunicationApp.Web.Infrastructure.AsyncTask
{
    public class SendNotification : IJob
    {
        //public INotification _Notification { get; set; }

        //public SendNotification(INotification Notification)
        //{
        //    this._Notification = Notification;//


        //}
        public void Execute(IJobExecutionContext context)
        {

            JobDataMap dataMap = context.JobDetail.JobDataMap;
            
            int PropertyId = Convert.ToInt32(dataMap.GetString("PropertyId"));          
            string Flag = dataMap.GetString("Flag");
            string Message = dataMap.GetString("Message");
            CommonClass CommonClass=new Services.CommonClass();
            string QStr = "";
            DataTable dt = new DataTable();
            QStr = "Select CustomerId From Property where PropertyId= " + PropertyId;
            dt = CommonClass.GetDataSet(QStr).Tables[0];
            int count = 0;
           if(dt.Rows.Count>0)
           {
               //Save Notification
               Notification Notification = new Notification();
               Notification.NotificationSendBy = 1;
               Notification.NotificationSendTo = Convert.ToInt32(dt.Rows[0]["CustomerId"]);
               Notification.IsRead = false;
               Notification.RequestMessage = Message;
               
               SendNotificationsToUsers(Convert.ToInt32(dt.Rows[0]["CustomerId"]), Message, Flag,count);
           }
          
        }
        public void SendNotificationsToUsers(int CustomerId,string message,string flag,int count   )
        {
            int Flag = flag!="" ?Convert.ToInt32(flag):0;
            var ParentId = "16466";
            string Message = message;
            //send notification
           // var Customers = _CustomerService.GetCustomers().Where(c => c.CustomerId != CustomerId && c.IsActive == true).ToList();
            CommonClass CommonClass = new Services.CommonClass();
            string QStr = "";
            
            DataTable dt = new DataTable();
            QStr = "Select ParentId From Customer where CustomerId = " + CustomerId + " and IsActive=1";
            dt = CommonClass.GetDataSet(QStr).Tables[0];
            if (dt.Rows.Count > 0)
            {
                ParentId = dt.Rows[0]["ParentId"].ToString();
            }
            QStr = "";
            QStr = "Select * From Customer where CustomerId != " + CustomerId + " and IsActive=1";
            dt = CommonClass.GetDataSet(QStr).Tables[0];

            try
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string sql = "insert into [notification](RequestMessage,NotificationSendTo,NotificationSendBy,Flag,IsRead) values(@RequestMessage,@NotificationSendTo,@NotificationSendBy,@Flag,@IsRead)";
                        try
                        {
                            string constring = ConfigurationManager.ConnectionStrings["DataContext"].ConnectionString;
                            using (SqlConnection con = new SqlConnection(constring))
                            {
                                using (SqlCommand cmd = new SqlCommand(sql, con))
                                {
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@RequestMessage", Message);
                                    cmd.Parameters.AddWithValue("@NotificationSendTo", dr["CustomerId"].ToString());
                                    cmd.Parameters.AddWithValue("@NotificationSendBy", CustomerId);
                                    cmd.Parameters.AddWithValue("@Flag", Flag);
                                    cmd.Parameters.AddWithValue("@IsRead", false);
                                    con.Open();
                                    int rowsAffected = cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                    con.Close();
                                    con.Dispose();
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        var ApplicationId = dr["ApplicationId"].ToString();
                        var DeviceType = dr["DeviceType"].ToString();
                        var name = dr["FirstName"].ToString();
                        var Trebid = dr["Trebid"].ToString();

                        //var IsNotificationSoundOn = Convert.ToBoolean(dr["IsNotificationSoundOn"]);
                        if (ApplicationId != null && ApplicationId != "" && DeviceType != null && DeviceType != "")
                        {
                            bool NotificationStatus = true;

                            string JsonMessage = "{\"Flag\":\"" + Flag + "\",\"Message\":\"" + Message + "\",\"ParentId\":\"" + ParentId + "\"}";

                            if (DeviceType == EnumValue.GetEnumDescription(EnumValue.DeviceType.Android))
                            {
                                CommonCls.SendGCM_Notifications(ApplicationId, JsonMessage, true);
                            }
                            else
                            {
                                string constring = ConfigurationManager.ConnectionStrings["DataContext"].ConnectionString;

                                using (SqlConnection con = new SqlConnection(constring))
                                {
                                    con.Open();
                                    using (SqlCommand thisCommand = new SqlCommand("SELECT COUNT(*) FROM Notification where NotificationSendTo=@NotificationSendTo and isread=0 ", con))
                                    {

                                        thisCommand.Parameters.AddWithValue("@NotificationSendTo", Convert.ToInt32(dr["CustomerId"].ToString()));
                                        count = (int)(thisCommand.ExecuteScalar());
                                    }
                                    
                                    con.Close();
                                    con.Dispose();
                                }
                                //Dictionary<string, object> Dictionary = new Dictionary<string, object>();
                                //Dictionary.Add("Flag", Flag);
                                //Dictionary.Add("Message", Message);
                                //Dictionary.Add("ParentId", ParentId);
                                //NotificationStatus = PushNotificatinAlert.SendPushNotification(ApplicationId, message, flag.ToString(), JsonMessage, Dictionary, 1, Convert.ToBoolean(true));
                                CommonCls.TestSendFCM_Notifications(ApplicationId, JsonMessage, Message,count, true);
                            }


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonCls.ErrorLog(ex.ToString());
            }
           
          
        }
    }
}