using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommunicationApp.Models
{
    public class RoleDetailEditorViewModel
    {
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsView { get; set; }
        public bool IsDelete { get; set; }
        public bool IsDetail { get; set; }
        public bool IsDownload { get; set; }

        public int RoleId { get; set; }
        public int FormId { get; set; }
        public string FormName { get; set; }
    }

    public class RoleDetailViewModel
    {
        public List<RoleDetailEditorViewModel> RoleDetail { get; set; }
        public RoleDetailViewModel()
        {
            this.RoleDetail = new List<RoleDetailEditorViewModel>();
        }
    }
}