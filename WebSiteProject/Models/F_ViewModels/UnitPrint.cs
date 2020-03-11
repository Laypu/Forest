using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSiteProject.Models.F_ViewModels
{
    public class UnitPrint
    {
        public int? UnitID { get; set; }
        public bool isPrint { get; set; }
        public bool isForward { get; set; }
        public bool isRSS { get; set; }
        public bool isShare { get; set; }
    }
}