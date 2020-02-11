using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLModel.Models;

namespace WebSiteProject.Models.F_ViewModels
{
    public class Index_ViewModel
    {
        public string FreeZone1_Title { get; set; }

        public string FreeZone1_Content { get; set; }

        public List<FreeZoneContent> FreeZone1_Link { get; set; }

        public string FreeZone2_Title { get; set; }

        public string FreeZone2_Content { get; set; }

        public List<FreeZoneContent> FreeZone2_Link { get; set; }



    }
}