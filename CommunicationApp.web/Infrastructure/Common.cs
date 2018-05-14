using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using CommunicationApp.Models;
using System.Configuration;
using System.Net;
using System.Text;
using System.Data;
using CommunicationApp.Services;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;

namespace CommunicationApp.Infrastructure
{
    enum RoleAction
    {
        view,
        create,
        edit,
        delete,
        detail,
        download
    }
    
    public class CommonCls
    {
        public static string SendGCM_Notifications(string regId, string value,bool IsJson=false)
        {
            try
            {
                if (true)
                {
                    //regId = regId.TrimEnd(',').ToString();

                    //Bharat Testing
                    //regId = "APA91bGGmOXoREb_7Z8nHS2iFhdjIVvuCa888Xcebheh1opCLFFuWjiUkJl9tmKJ0mNRySd0Xri639Jc2kfRoGOF3FSaD_jiOB_dXHR-4nSnaLXVb6mW2foW0hZbUveYCjud2E3Y3vH61KyVLBco42SpMmR1dv6RsA";

                  
                    var applicationID = ConfigurationManager.AppSettings["FCM_ApplicationID"].ToString();
                    var SENDER_ID = ConfigurationManager.AppSettings["FCM_SenderID"].ToString();
                    // var value = dtGCMRegistrationID.Rows[0].ToString();
                    //WebRequest tRequest;
                    var tRequest = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    tRequest.ContentType = "application/json"; //" application/x-www-form-urlencoded;charset=UTF-8";
                    tRequest.Headers.Add(HttpRequestHeader.Authorization, "key=" + applicationID + "");
                    tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

                    //Data_Post Format
                    string postData="";
                    if (IsJson)
                    {
                        postData = "{ \"collapse_key\": \"CommunicationApp\",  \"time_to_live\": 0,  \"delay_while_idle\": false,  \"data\": {    \"message\": " + value + ",    \"time\": \"" + System.DateTime.Now.ToString() + "\" },  \"registration_ids\":[\"" + regId.Replace("'", "\"") + "\"]}";
                    }
                    else
                    {
                        postData = "{ \"collapse_key\": \"CommunicationApp\",  \"time_to_live\": 0,  \"delay_while_idle\": false,  \"data\": {    \"message\": \"" + value + "\",    \"time\": \"" + System.DateTime.Now.ToString() + "\" },  \"registration_ids\":[\"" + regId.Replace("'", "\"") + "\"]}";
                    }

                    Console.WriteLine(postData);
                    Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    tRequest.ContentLength = byteArray.Length;

                    Stream dataStream = tRequest.GetRequestStream();

                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse tResponse = tRequest.GetResponse();

                    dataStream = tResponse.GetResponseStream();

                    StreamReader tReader = new StreamReader(dataStream);

                    String sResponseFromServer = tReader.ReadToEnd();
                    
                    tReader.Close();
                    dataStream.Close();
                    tResponse.Close();
                    return sResponseFromServer;
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        return text;
                    }
                }
            }

        }

      
        public static void TestSendFCM_Notifications(string regId, string flag, string value,int count, bool IsJson = false)
        {
            try
            {
                var applicationID = ConfigurationManager.AppSettings["FCM_ApplicationID"].ToString();
                var senderId = ConfigurationManager.AppSettings["FCM_SenderID"].ToString();
                string deviceId = regId;
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                var data = new
                {
                    to = deviceId,
                    //notification = value,
                    //FlagsAttribute = "Friend",
                    notification = new
                    {
                        body = value,
                        title = "CommunicationApp",
                        icon = "CommunicationApp",
                        badge = count,
                        sound = "cat.caf",
                    },
                    data = new
                    {
                        FlagsAttribute = flag,
                        testabc="434",
                    },
                    priority = "high"

                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                //NotificationMessage nm = new NotificationMessage();
                //nm.Title = "Friendlier";
                //nm.Message = value;
                //nm.ItemId = 1;

                //var values = new JavaScriptSerializer().Serialize(nm);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                //string postData = "{\"collapse_key\":\"score_update\",\"time_to_live\":108,\"delay_while_idle\":true,\"data\": { \"message\" : " + values + ",\"time\": " + "\"" + System.DateTime.Now.ToString() + "\"},\"registration_ids\":[\"" + deviceId + "\"]}";

                //Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                //tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                //FCMResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<FCMResponse>(sResponseFromServer);

                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                // Response.Write(ex.Message);
            }


        }
        public static JsonReturnModel CreateMessage(string Status, object Message)
        {
            JsonReturnModel jsonReturn = new JsonReturnModel();
            jsonReturn.Status = Status;
            jsonReturn.Message = Message;
            return jsonReturn;
        }
        public static string GetAlphaNumericCode(int length=6)
        {
            Random random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return result;
        }
        public static string GetNumericCode(int length = 4)
        {
            Random random = new Random();
            var chars = "0123456789";
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return result;
        }
        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }
        public static string GetURL()
        {
            Boolean IsLive =Convert.ToBoolean(ConfigurationManager.AppSettings["IsLive"].ToString());
            string URL = "";
            if (IsLive)
            {
                URL = ConfigurationManager.AppSettings["LiveURL"].ToString();
            }
            else
            {
                URL = ConfigurationManager.AppSettings["LocalURL"].ToString();
            }
            return URL;
        }
        public static bool CreateThumbnail(string ImageURL, int Width, int Height, bool maintainAspectRatio)
        {
            bool Success = false;
            //bool Success = true;
            string FullUrl = System.Web.HttpContext.Current.Server.MapPath(ImageURL);

            byte[] imageData = null;
            FileInfo fileInfo = new FileInfo(FullUrl);
            long imageFileLength = fileInfo.Length;
            FileStream fs = new FileStream(FullUrl, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            imageData = br.ReadBytes((int)imageFileLength);

            br.Dispose();
            fs.Close();

           // System.IO.File.Delete(FullUrl);

            Bitmap bmp = null;
            try
            {
                MemoryStream memStream = new MemoryStream(imageData);
               
                System.Drawing.Image img = System.Drawing.Image.FromStream(memStream);
                if (maintainAspectRatio)
                {
                    AspectRatio aspectRatio = new AspectRatio();
                    aspectRatio.WidthAndHeight(img.Width, img.Height, Width, Height);
                   bmp = new Bitmap(img, aspectRatio.Width, aspectRatio.Height);
                    //bmp = new Bitmap(aspectRatio.Width, aspectRatio.Height);
                  
                }
                else
                {
                    bmp = new Bitmap(img, Width, Height);

                }
                memStream.Dispose();
               // img.Dispose();
                bmp.Save(FullUrl);
                
               
                Success = true;
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message.ToString();
                ErrorLogging.LogError(ex);
                Success = false;
            }
            return Success;
        }

        public static string ErrorLog(string ErrorMessage)
        {
            if (ErrorMessage.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
            {
                return "fk";
            }
            else
            {
                return "";
            }
        }
        public static string SendMailToUser(string UserName, string Email, string Password, int MessageType,string TrebId)
        {
            string result = "";
            string EmailTitle = (MessageType == 1 ? "Registration Sucessfully" : "Forget Password");
            try
            {
                string Logourl = CommonCls.GetURL() + "/images/EmailLogo.png";
                string Imageurl = CommonCls.GetURL() + "/images/EmailPic.png";
                // Send mail.
                MailMessage mail = new MailMessage();
                string FromEmailID = WebConfigurationManager.AppSettings["FromEmailID"];
                string FromEmailPassword = WebConfigurationManager.AppSettings["FromEmailPassword"];
                string ToBCCEmailID = WebConfigurationManager.AppSettings["ToBCCEmailID"];
                //Password = AmebaSoftwares.Infrastructure.CustomMembershipProvider.DecryptString(user.Password);
                string _Host = WebConfigurationManager.AppSettings["SmtpServer"];
                int _Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(WebConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(Email);
                // mail.Bcc.Add(ToBCCEmailID);
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = EmailTitle;
                string msgbody = "";
                msgbody += "<div>";
                msgbody += "<div style='float:left;width:100%;'>";
                msgbody += "<h2 style='float:left; width:100%; font-size:16px; font-family:arial; margin:0 0 10px;'>Dear " + UserName + ",</h2>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:5px 11px 0 0;'>TrebId :</p> <span style='float:left; font-size:16px; font-family:arial; margin:10px 0 0 0;'>" + TrebId + "</span>";
                msgbody += "</div>";
                msgbody += "<div style='float:left;width:100%; margin:4px 0;'><p style='float:left; font-size:14px; font-family:arial; line-height:25px; margin:5px 11px 0 0;'>Password :</p> <span style='float:left; font-size:16px; font-family:arial; margin:10px 0 0 0;'>" + Password + "</span>";
                msgbody += "</div>";
                msgbody += "<p style='float:left;width:100%; font-size:14px; font-family:arial; line-height:25px; margin:0;'>It is your new changed password for login.";
                msgbody += "</p>";
                msgbody += "<span style='float:left; font-size:15px; font-family:arial; margin:12px 0 0 0; width:100%;'>Team</span>";
                msgbody += "<div style='width:auto; float:left;'>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'>Call: 416 844 5725 | 647 977 3268 </p>";
                msgbody += " <p style='font-family:arial; font-size:12px; font-weight:bold; margin:0px; padding:0px;'> Fax: 647 497 5646 </p>";
                msgbody += " <a style='color:red; width:100%; font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='http://www.only4agents.com'>Web: www.only4agents.com</a>";
                msgbody += " <a style='color:red; width:100%;font-size:13px; float:left; margin:5px 0 0 0px; text-decoration:none; font-family:arial;' href='#'>Email: info@only4agents.com</a>";
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
                smtp.Host = _Host;
                smtp.Port = _Port;
                smtp.UseDefaultCredentials = _UseDefaultCredentials;
                smtp.Credentials = new System.Net.NetworkCredential
                (FromEmailID, FromEmailPassword);// Enter senders User name and password
                smtp.EnableSsl = _EnableSsl;
                smtp.Send(mail);
                return "success";
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return result;
        }
    }    
}

   