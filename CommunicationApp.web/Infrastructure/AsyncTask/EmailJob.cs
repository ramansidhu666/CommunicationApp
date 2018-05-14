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
    public class SendMail : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            PropertyModel PropertyModel = new Models.PropertyModel();
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            int PropertyId = Convert.ToInt32(dataMap.GetString("PropertyId"));
            string Subject = dataMap.GetString("Subject");
            string Body = dataMap.GetString("Body");
            string PropertyTypeStatus = dataMap.GetString("PropertyTypeStatus");
            CommonClass CommonClass = new Services.CommonClass();
            string QStr = "";
            DataTable dt = new DataTable();
            QStr = "select p.PropertyId,p.Style,p.LocationPrefered,p.PropertyType,c.TrebId,c.FirstName,c.MiddleName,c.LastName,c.EmailId from Property p,Customer c where  p.CustomerId=c.CustomerId and p.PropertyId=" + PropertyId;
            dt = CommonClass.GetDataSet(QStr).Tables[0];

            if (dt.Rows.Count > 0)
            {
                string FirstName = dt.Rows[0]["FirstName"].ToString() + " " + dt.Rows[0]["MiddleName"].ToString() + " " + dt.Rows[0]["LastName"].ToString();
                string EmailId = dt.Rows[0]["EmailId"].ToString();
                PropertyModel.PropertyTypeStatus=PropertyTypeStatus;
                PropertyModel.Style=dt.Rows[0]["Style"].ToString();
                PropertyModel.PropertyType=dt.Rows[0]["PropertyType"].ToString();
                PropertyModel.LocationPrefered=dt.Rows[0]["LocationPrefered"].ToString();
                PropertyModel.CustomerTrebId=dt.Rows[0]["TrebId"].ToString();


                SendMailToUser(FirstName,EmailId, PropertyModel);
                SendMailToAdmin(FirstName, EmailId, PropertyModel, Subject, Body);
            }
          
          
           
           
        }
        public void SendMailToUser(string UserName, string EmailAddress, PropertyModel PropertyType)
        {
            try
            {
                var ConcateData = "";
                if (PropertyType.Style != "" && PropertyType.Style != null)
                {
                    ConcateData += PropertyType.Style + ",";
                }
                if (PropertyType.PropertyType != "" && PropertyType.PropertyType != null)
                {
                    ConcateData += PropertyType.PropertyType + ",";
                }
                if (PropertyType.LocationPrefered != "" && PropertyType.LocationPrefered != null)
                {
                    ConcateData += PropertyType.LocationPrefered;
                }

                // Send mail.
                MailMessage mail = new MailMessage();
                string Logourl = CommonCls.GetURL() + "/images/EmailLogo.png";
                string Imageurl = CommonCls.GetURL() + "/images/EmailPic.png";

                string FromEmailID = WebConfigurationManager.AppSettings["FromEmailID"];
                string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];
                string ToEmailID = WebConfigurationManager.AppSettings["ToEmailID"];
                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                int _Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(new MailAddress(EmailAddress));
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = "Property successfully registered. ";
                //string LogoPath = ClsCommon.GetURL() + "/images/logo.png";
                string msgbody = "";
                msgbody += "";
                msgbody += " <div>";
                msgbody += " <div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear " + UserName + ",</h2>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:0 11px 0 0;'>Treb Id:</p> <span style='float:left; font-size:14px; font-family:arial; margin:5px 0 0 0;'><b>" + PropertyType.CustomerTrebId + "<b></span>";
                msgbody += " </div>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:0 11px 0 0;'>This message refers to your post under:</p> <span style='float:left; font-size:14px; font-family:arial; margin:5px 0 0 0;'><b>" + PropertyType.PropertyTypeStatus + "<b></span>";
                msgbody += " </div>";

                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Your post is under review, we will notify you once approved.</p>";

                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Team</span>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268  </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'> Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='http://www.only4agents.com'>Web: www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='#'>Email: info@only4agents.com</a>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:5px 0;'>";
                msgbody += "<img style='float:left; width:500px;' src='" + Logourl + "' />";
                msgbody += "</div>";
                msgbody += "</div>";
                msgbody += "</div>";
                mail.Body = msgbody;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "smtp.gmail.com"; //_Host;
                smtp.Port = _Port;
                //smtp.UseDefaultCredentials = _UseDefaultCredentials;
                smtp.Credentials = new System.Net.NetworkCredential(FromEmailID, FromEmailPassword);// Enter senders User name and password
                smtp.EnableSsl = _EnableSsl;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
        }

        public void SendMailToAdmin(string UserName, string EmailAddress, PropertyModel PropertyType, string Subject, string Body)
        {
            try
            {
                string Logourl = CommonCls.GetURL() + "/images/EmailLogo.png";
                string Imageurl = CommonCls.GetURL() + "/images/EmailPic.png";

                // Send mail.
                MailMessage mail = new MailMessage();

                string FromEmailID = WebConfigurationManager.AppSettings["FromEmailID"];
                string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];
                string ToEmailID = WebConfigurationManager.AppSettings["ToEmailID"];
                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                int _Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(new MailAddress(ToEmailID));
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = Subject;
                //string LogoPath = ClsCommon.GetURL() + "/images/logo.png";
                string msgbody = "";
                msgbody += "";
                msgbody += "<div>";
                msgbody += "<div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear Admin,</h2>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:5px 11px 0 0;'>Property Type :</p> <span style='float:left; font-size:14px; font-family:arial; margin:10px 0 0 0;'><b>" + PropertyType.PropertyTypeStatus + "</b></span>";
                msgbody += "</div>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>" + Body + "";//
                msgbody += "</p>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>User Name: " + UserName + "";//
                msgbody += "</p>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>Treb Id: " + PropertyType.CustomerTrebId + "";//
                msgbody += "</p>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Team</span>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268  </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'> Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='http://www.only4agents.com'>Web: www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='#'>Email: info@only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='http://app.only4agents.com/'>Click here to login: www.app.only4agents.com</a>";
                msgbody += " </div>";
                msgbody += "<div style='float:left; width:100%; margin:12px 0 6px 0;'>";
                //msgbody +="<img style='float:left; width:150px;' src='data:image/png;base64,"+ LogoBase64 +"' /> <img style='float:left; width:310px; margin:30px 0 0 30px;' src='data:image/png;base64,"+ ImageBase64 +"' />";
                msgbody += "<img style='float:left; width:500px;' src='" + Logourl + "' /> ";
                //<img style='float:left; width:310px; margin:30px 0 0 30px;' src='" + Imageurl + "' />
                msgbody += "</div>";
                msgbody += "</div>";
                msgbody += "</div>";
                mail.Body = msgbody;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "smtp.gmail.com"; //_Host;
                smtp.Port = _Port;
                //smtp.UseDefaultCredentials = _UseDefaultCredentials;
                smtp.Credentials = new System.Net.NetworkCredential(FromEmailID, FromEmailPassword);// Enter senders User name and password
                smtp.EnableSsl = _EnableSsl;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
        }


    }
}