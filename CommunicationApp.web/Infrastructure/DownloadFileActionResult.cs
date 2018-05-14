using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;

namespace CommunicationApp.Models
{
    public class DownloadFileActionResult : ActionResult
    {

        public List<CustomerExportModel> CustomerExportModel { get; set; }

       // public GridView ExcelGridView { get; set; }
        public string fileName { get; set; }


        public DownloadFileActionResult(List<CustomerExportModel> ExportModel, string pFileName)
        {

            CustomerExportModel = ExportModel;
            fileName = pFileName;

        }

        public void WriteTsv<T>(IEnumerable<T> data, TextWriter output)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor prop in props)
            {
                output.Write(prop.DisplayName); // header
                output.Write("\t");
            }
            output.WriteLine();
            foreach (T item in data)
            {
                foreach (PropertyDescriptor prop in props)
                {
                    output.Write(prop.Converter.ConvertToString(
                         prop.GetValue(item)));
                    output.Write("\t");
                }
                output.WriteLine();
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {

            HttpContext curContext = HttpContext.Current;

            curContext.Response.Clear();

            curContext.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);

            curContext.Response.Charset = "";

            curContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            curContext.Response.ContentType = "application/vnd.ms-excel";

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            //ExcelGridView.RenderControl(htw);
            
            byte[] byteArray = Encoding.ASCII.GetBytes(sw.ToString());

            MemoryStream s = new MemoryStream(byteArray);

            StreamReader sr = new StreamReader(s, Encoding.ASCII);
            WriteTsv(CustomerExportModel,curContext.Response.Output);
            curContext.Response.Write(sr.ReadToEnd());

            curContext.Response.End();

        }

    }
}