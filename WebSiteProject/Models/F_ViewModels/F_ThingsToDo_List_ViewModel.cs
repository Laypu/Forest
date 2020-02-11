using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSiteProject.Models.F_ViewModels
{
    public class F_ThingsToDo_List_ViewModel
    {
        public int ItemID { get; set; }
        public string RelateImageFileName { get; set; }
        public string Title { get; set; }
        public int? ModelID { get; set; }
        public int HashTag_Type_ID { get; set; }
        public int? Sort { get; set; }
        public bool? IsVerift { get; set; }

    }
}