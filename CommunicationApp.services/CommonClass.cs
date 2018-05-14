using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace CommunicationApp.Services
{
    public class Connection
    {
        /// <summary>
        /// Connection String
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DataContext"].ConnectionString;
            }
        }
    }

    public class CommonClass
    {
        string ConnectionString = "";
        public CommonClass()
        {

            ConnectionString = Connection.ConnectionString;
        }

       
        //Execute Non Scalar
        public string ExecuteNonQuery(string QStr)
        {
            string ErrorMessage = "";
            SqlConnection conn = null;
            SqlCommand cmd = null;
            //if (ConnectionString == string.Empty)
            //{
            //    ConnectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            //}

            try
            {
                conn = new SqlConnection(ConnectionString);
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                cmd = new SqlCommand(QStr, conn);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                conn.Close();

                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    ErrorMessage = "FK";
                }
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Close();
                }

            }

            return ErrorMessage;
        }
        public Object GetSingleValue(string QStr)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            //if (ConnectionString == string.Empty)
            //{
            //    ConnectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            //}
            try
            {
                conn = new SqlConnection(ConnectionString);
                cmd = new SqlCommand();
                Object returnValue;

                cmd.CommandText = QStr;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                returnValue = cmd.ExecuteScalar();
                conn.Close();

                return returnValue;

            }
            catch (Exception)
            {
                //HttpContext.Current.Response.Redirect("~/ErrorRedirect.aspx", false);
                return '0';
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        public byte[] GetImageValue(string QStr)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            //if (ConnectionString == string.Empty)
            //{
            //    ConnectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            //}
            try
            {
                conn = new SqlConnection(ConnectionString);
                cmd = new SqlCommand();
                byte[] returnValue;

                cmd.CommandText = QStr;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                returnValue = (byte[])cmd.ExecuteScalar();
                conn.Close();

                return returnValue;

            }
            catch (Exception)
            {
                //HttpContext.Current.Response.Redirect("~/ErrorRedirect.aspx", false);
                return null;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        public DataSet GetDataSet(String queryString)
        {
            //if (ConnectionString == string.Empty)
            //{
            //    ConnectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            //}

            DataSet ds = new DataSet();

            try
            {
                // Connect to the database and run the query.
                SqlConnection connection = new SqlConnection(ConnectionString);
                SqlDataAdapter adapter = new SqlDataAdapter(queryString, connection);

                // Fill the DataSet.
                adapter.Fill(ds);
                connection.Close();
                connection.Dispose();

            }
            catch (Exception)
            {

                // The connection failed. Display an error message.
                //Message.Text = "Unable to connect to the database.";

            }

            return ds;

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

        public SqlParameter[] InsertData(object[] SqlObj, SqlParameter[] outputParameter, string ProcedureName)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;

            try
            {
                // Create Object to get store procedure parameters
                object[] SqlParam = new object[SqlObj.Length + outputParameter.Length];

                conn = new SqlConnection(ConnectionString);
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();

                // Start Code to get SQL parameter from Stored Procedure
                SqlCommand myCommand = new SqlCommand();
                myCommand.Connection = conn;
                myCommand.CommandText = ProcedureName;
                myCommand.CommandType = CommandType.StoredProcedure;

                SqlCommandBuilder.DeriveParameters(myCommand);

                for (int i = 0; i < myCommand.Parameters.Count - 1; i++)
                {
                    SqlParam[i] = myCommand.Parameters[i + 1].ParameterName.ToString();
                }

                // End code to get SQL parameter from Stored Procedure

                // Start Code to Insert data into table using Stored Procedure
                cmd = new SqlCommand(ProcedureName, conn);

                for (int i = 0; i < SqlObj.Length; i++)
                {
                    SqlParameter sp = new SqlParameter();
                    sp.ParameterName = SqlParam[i].ToString();
                    sp.Value = SqlObj[i];
                    cmd.Parameters.Add(sp);
                }
                //add the output parameters
                for (int i = 0; i < outputParameter.Length; i++)
                {
                    cmd.Parameters.Add(outputParameter[i]);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();

                //End Code to Insert data into table using stored procedure 
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return outputParameter;
        }

        public void BindNotification()
        {
            string QStr = string.Empty;
            //Func<DateTime, IEnumerable<DateTime>> clockQuery = start => from offset in Enumerable.Range(0, 144) select start.AddMinutes(10 * offset);
            //cmbNotification.Items.Clear();
            //foreach (var time in clockQuery(DateTime.Today))
            //{
            //    cmbNotification.Items.Add(time.ToString("hh:mm"));
            //}


            DataTable dtNotification = new DataTable();
            dtNotification.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            dtNotification.Columns.Add(new DataColumn("Value", Type.GetType("System.Int32")));

            for (int i = 0; i <= 24; i++)
            {
                int OffSet = 10;
                int OffSetInterval = 5;

                if (i == 24)
                {
                    OffSet = 00;
                    OffSetInterval = 1;
                }
                else if (i > 0)
                {
                    OffSet = 00;
                    OffSetInterval = 6;
                }

                for (int j = 1; j <= OffSetInterval; j++)
                {
                    DataRow dr = dtNotification.NewRow();
                    dr["Name"] = string.Format("{0:00}", Convert.ToDecimal(i)) + ":" + string.Format("{0:00}", Convert.ToDecimal(OffSet));
                    dr["Value"] = (i * 60) + OffSet;
                    dtNotification.Rows.Add(dr);
                    OffSet = OffSet + 10;
                }

            }

           

            //int Value =Convert.ToInt32(cmbNotification.SelectedValue);
        }
    }
}