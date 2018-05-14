using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace CommunicationApp.Infrastructure
{
    public static class ErrorLogging
    {
        public static void LogError(Exception oEx)
        {
            HandleException(oEx);
        }

        public static void HandleException(Exception ex)
        {
            HttpContext ctxObject = HttpContext.Current;
            string strLogConnString = ConfigurationManager.ConnectionStrings["DataContext"].ConnectionString.ToString();
            string logDateTime = DateTime.Now.ToString("g");
            string strReqURL = (ctxObject.Request.Url != null) ? ctxObject.Request.Url.ToString() : String.Empty;
            string strReqQS = (ctxObject.Request.QueryString != null) ? ctxObject.Request.QueryString.ToString() : String.Empty;
            string strServerName = String.Empty;
            if (ctxObject.Request.ServerVariables["HTTP_REFERER"] != null)
            {
                strServerName = ctxObject.Request.ServerVariables["HTTP_REFERER"].ToString();
            }
            string strUserAgent = (ctxObject.Request.UserAgent != null) ? ctxObject.Request.UserAgent : String.Empty;
            string strUserIP = (ctxObject.Request.UserHostAddress != null) ? ctxObject.Request.UserHostAddress : String.Empty;
            string strUserAuthen = (ctxObject.User.Identity.IsAuthenticated.ToString() != null) ? ctxObject.User.Identity.IsAuthenticated.ToString() : String.Empty;
            string strUserName = (ctxObject.User.Identity.Name != null) ? ctxObject.User.Identity.Name : String.Empty;
            string strMessage = string.Empty, strSource = string.Empty, strTargetSite = string.Empty, strStackTrace = string.Empty;
            while (ex != null)
            {
                strMessage = ex.Message;
                strSource = ex.Source;
                strTargetSite = ex.TargetSite.ToString();
                strStackTrace = ex.StackTrace;
                ex = ex.InnerException;
            }

            if (strLogConnString.Length > 0)
            {
                SqlCommand strSqlCmd = new SqlCommand();
                strSqlCmd.CommandType = CommandType.StoredProcedure;
                strSqlCmd.CommandText = "SaveErrorExceptionLogs";
                SqlConnection sqlConn = new SqlConnection(strLogConnString);
                strSqlCmd.Connection = sqlConn;
                sqlConn.Open();
                try
                {
                    strSqlCmd.Parameters.Add(new SqlParameter("@Source", strSource));
                    strSqlCmd.Parameters.Add(new SqlParameter("@LogDateTime", logDateTime));
                    strSqlCmd.Parameters.Add(new SqlParameter("@Message", strMessage));
                    strSqlCmd.Parameters.Add(new SqlParameter("@QueryString", strReqQS));
                    strSqlCmd.Parameters.Add(new SqlParameter("@TargetSite", strTargetSite));
                    strSqlCmd.Parameters.Add(new SqlParameter("@StackTrace", strStackTrace));
                    strSqlCmd.Parameters.Add(new SqlParameter("@ServerName", strServerName));
                    strSqlCmd.Parameters.Add(new SqlParameter("@RequestURL", strReqURL));
                    strSqlCmd.Parameters.Add(new SqlParameter("@UserAgent", strUserAgent));
                    strSqlCmd.Parameters.Add(new SqlParameter("@UserIP", strUserIP));
                    strSqlCmd.Parameters.Add(new SqlParameter("@UserAuthentication", strUserAuthen));
                    strSqlCmd.Parameters.Add(new SqlParameter("@UserName", strUserName));
                    SqlParameter outParm = new SqlParameter("@EventId", SqlDbType.Int);
                    outParm.Direction = ParameterDirection.Output;
                    strSqlCmd.Parameters.Add(outParm);
                    strSqlCmd.ExecuteNonQuery();
                    strSqlCmd.Dispose();
                    sqlConn.Close();
                }
                catch (Exception exc)
                {

                    EventLog.WriteEntry(exc.Source, "Database Error From Exception Log!", EventLogEntryType.Error, 65535);
                }
                finally
                {
                    strSqlCmd.Dispose();
                    sqlConn.Close();
                }
            }
        }
    }
}
