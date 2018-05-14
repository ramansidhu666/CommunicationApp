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
using CommunicationApp.Models;

namespace CommunicationApp.Web.Infrastructure.AsyncTask
{
    public class NewsletterJob : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            NewsletterModel model = new NewsletterModel();
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            int Admin = Convert.ToInt32(dataMap.GetString("AdminId"));
            model.CustomerIds = dataMap.GetString("CustomerIds");
            model.P1 = dataMap.GetString("P1");
            model.P2 = dataMap.GetString("P2");
            model.P3 = dataMap.GetString("P3");
            model.P4 = dataMap.GetString("P4");
            model.P5 = dataMap.GetString("P5");
            model.P6 = dataMap.GetString("P6");
            model.P7 = dataMap.GetString("P7");
            model.PP1 = dataMap.GetString("PP1");
            model.PP2 = dataMap.GetString("PP2");
            model.PP3 = dataMap.GetString("PP3");
            model.PP4 = dataMap.GetString("PP4");
            model.PP5 = dataMap.GetString("PP5");
            model.PP6 = dataMap.GetString("PP6");

            model.PG1 = dataMap.GetString("PG1");
            model.PG2 = dataMap.GetString("PG2");
            model.PG3 = dataMap.GetString("PG3");
            model.PG4 = dataMap.GetString("PG4");
            model.PG5 = dataMap.GetString("PG5");
            model.PG6 = dataMap.GetString("PG6");
            model.PG7 = dataMap.GetString("PG7");
            model.PPG1 = dataMap.GetString("PPG1");
            model.PPG2 = dataMap.GetString("PPG2");
            model.PPG3 = dataMap.GetString("PPG3");
            model.PPG4 = dataMap.GetString("PPG4");
            model.PPG5 = dataMap.GetString("PPG5");
            model.PPG6 = dataMap.GetString("PPG6");


            model.PropertyPhoto = dataMap.GetString("PropertyPhoto");//
            model.AdminLogo = dataMap.GetString("AdminLogo");
            model.FirstContent = dataMap.GetString("FirstContent");
            model.SecondContent = dataMap.GetString("SecondContent");
            model.ThirdContent = dataMap.GetString("ThirdContent");
            model.TemplateType = dataMap.GetString("TemplateType");
            CommonClass CommonClass = new Services.CommonClass();
            string QStr = "";
            DataTable dt = new DataTable();
            QStr = "select CustomerId from Customer where ParentId=" + Admin;
            dt = CommonClass.GetDataSet(QStr).Tables[0];
            var Ids = model.CustomerIds.Split(',');
            if (Ids != null)
            {
                foreach (var CustomerId in Ids)
                {

                    try
                    {

                        QStr = "";
                        DataTable dt2 = new DataTable();
                        QStr = "Select * From Customer where CustomerId = " + CustomerId + " and IsActive=1";
                        dt2 = CommonClass.GetDataSet(QStr).Tables[0];
                        if (dt2.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt2.Rows)
                            {
                                
                                var EmailId = dr["EmailId"].ToString();                                
                                //Send mail
                                MailMessage mail = new MailMessage();

                                string FromEmailID = WebConfigurationManager.AppSettings["FromEmailID"];
                                string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];

                                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                                int _Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
                                Boolean _UseDefaultCredentials = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                                Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
                                mail.To.Add(new MailAddress(EmailId));
                                mail.From = new MailAddress(FromEmailID);
                                mail.Subject = "News Letter";
                                string msgbody = "";
                                var Template = "";
                                if (model.TemplateType == "first_NwsLtr")
                                {
                                    Template = "Templates/FirstTemplate.html";
                                }
                                else if (model.TemplateType == "second_NwsLtr")
                                {
                                    Template = "Templates/SecondTemplate.html";
                                }
                                else if (model.TemplateType == "thirld_NwsLtr")
                                {
                                    Template = "Templates/ThirldNewsLetter.html";
                                }
                                else if (model.TemplateType == "fourth_NwsLtr")
                                {
                                    Template = "Templates/FourthNewsLetter.html";
                                }
                                else if (model.TemplateType == "fifth_NwsLtr")
                                {
                                    Template = "Templates/FifthNewsLetter.html";
                                }
                                else if (model.TemplateType == "sixth_NwsLtr")
                                {
                                    Template = "Templates/SixthNewsLetter.html";
                                }

                               // using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(Template)))
                                using (StreamReader reader = new StreamReader(Path.Combine(HttpRuntime.AppDomainAppPath, Template)))
                                {
                                    
                                    msgbody = reader.ReadToEnd();

                                    //Replace UserName and Other variables available in body Stream
                                    msgbody = msgbody.Replace("{PropertyPhoto}", model.PropertyPhoto);
                                    msgbody = msgbody.Replace("{logopath}", model.AdminLogo);
                                    msgbody = msgbody.Replace("{SecondContent}", model.SecondContent);
                                    msgbody = msgbody.Replace("{ThirdContent}", model.ThirdContent);

                                    if (model.TemplateType == "first_NwsLtr")
                                    {
                                        //first template editing
                                     msgbody=    FirstNewsletter(model, msgbody);
                                    }
                                    else if (model.TemplateType == "second_NwsLtr")
                                    {
                                        //second template editing
                                        msgbody = SecondNewsletter(model, msgbody);
                                    }
                                  
                                   

                                }
                                
                                mail.BodyEncoding = System.Text.Encoding.UTF8;
                                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                                System.Net.Mail.AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(msgbody, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                                System.Net.Mail.AlternateView htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(msgbody, null, "text/html");

                                mail.AlternateViews.Add(plainView);
                                mail.AlternateViews.Add(htmlView);
                               // mail.Body = msgbody;
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
                        }

                    }
                    catch (Exception ex)
                    {
                        string ErrorMsg = ex.ToString();
                    }
                }
            }
            

        }

        public string FirstNewsletter(NewsletterModel model, string msgbody)
        {
          


            //for First template
            if (model.P1 != null)
            {
                msgbody = msgbody.Replace("{P1}", model.P1);
            }
            else
            {
                msgbody = msgbody.Replace("{P1}", "1. Detail and de-clutter.");
            }
            if (model.P2 != null)
            {
                msgbody = msgbody.Replace("{P2}", model.P2);
            }
            else
            {
                msgbody = msgbody.Replace("{P2}", "2. Fire up the oven.");
            }
            if (model.P3 != null)
            {
                msgbody = msgbody.Replace("{P3}", model.P3);
            }
            else
            {
                msgbody = msgbody.Replace("{P3}", "3. Lighten up.");
            }
            if (model.P4 != null)
            {
                msgbody = msgbody.Replace("{P4}", model.P4);
            }
            else
            {
                msgbody = msgbody.Replace("{P4}", "4. Lock it up.");
            }
            if (model.P5 != null)
            {
                msgbody = msgbody.Replace("{P5}", model.P5);
            }
            else
            {
                msgbody = msgbody.Replace("{P5}", "5. Get rid of the pets. ");
            }

            if (model.P6 != null)
            {
                msgbody = msgbody.Replace("{P6}", model.P6);
            }
            else
            {
                msgbody = msgbody.Replace("{P6}", "6. Make yourself disappear.");
            }
            if (model.P7 != null)
            {
                msgbody = msgbody.Replace("{P7}", model.P7);
            }
            else
            {
                msgbody = msgbody.Replace("{P7}", "7. Sell the neighborhood ");
            }


            msgbody = msgbody.Replace("{PG1}", model.PG1);
            msgbody = msgbody.Replace("{PG2}", model.PG2);
            msgbody = msgbody.Replace("{PG3}", model.PG3);
            msgbody = msgbody.Replace("{PG4}", model.PG4);
            msgbody = msgbody.Replace("{PG5}", model.PG5);
            msgbody = msgbody.Replace("{PG6}", model.PG6);
            msgbody = msgbody.Replace("{PG7}", model.PG7);
            //End


            return msgbody;
        }

        public string SecondNewsletter(NewsletterModel model, string msgbody)
        {
            //for Second template
            if (model.PP1 != null)
            {
                msgbody = msgbody.Replace("{PP1}", model.PP1);
            }
            else
            {
                msgbody = msgbody.Replace("{PP1}", "1. Finding an area of volunteer service");
            }
            if (model.PP2 != null)
            {
                msgbody = msgbody.Replace("{PP2}", model.PP2);
            }
            else
            {
                msgbody = msgbody.Replace("{PP2}", "2. Volunteer Time or Space ");
            }
            if (model.PP3 != null)
            {
                msgbody = msgbody.Replace("{PP3}", model.PP3);
            }
            else
            {
                msgbody = msgbody.Replace("{PP3}", "3. Teach a Class");
            }
            if (model.PP4 != null)
            {
                msgbody = msgbody.Replace("{PP4}", model.PP4);
            }
            else
            {
                msgbody = msgbody.Replace("{PP4}", "4. Teach a Class");
            }
            if (model.PP5 != null)
            {
                msgbody = msgbody.Replace("{PP5}", model.PP5);
            }
            else
            {
                msgbody = msgbody.Replace("{PP5}", "5.  Adopt a Project ");
            }
            if (model.PP6 != null)
            {
                msgbody = msgbody.Replace("{PP6}", model.PP6);
            }
            else
            {
                msgbody = msgbody.Replace("{PP6}", " Community Involvement Suggestions for Realtor ");
            }

            msgbody = msgbody.Replace("{PPG1}", model.PPG1);
            msgbody = msgbody.Replace("{PPG2}", model.PPG2);
            msgbody = msgbody.Replace("{PPG3}", model.PPG3);
            msgbody = msgbody.Replace("{PPG4}", model.PPG4);
            msgbody = msgbody.Replace("{PPG5}", model.PPG5);
            msgbody = msgbody.Replace("{PPG6}", model.PPG6);

            //End
            return msgbody;
        }
    }
}