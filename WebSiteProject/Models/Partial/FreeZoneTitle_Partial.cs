using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSiteProject.Models
{   
    [MetadataType(typeof(FreeZoneTitleMetadata))]
    public partial class FreeZoneTitle
    {
        public class FreeZoneTitleMetadata
        {
            public int FreeZoneTitleID { get; set; }
            public string Title { get; set; }

            [AllowHtml]
            public string FreeZoneTitleContent { get; set; }
        }
       
    }
}