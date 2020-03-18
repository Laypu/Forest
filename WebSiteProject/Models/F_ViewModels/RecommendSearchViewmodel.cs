using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSiteProject.Models.F_ViewModels
{
    public class RecommendSearchViewmodel
    {
        public RecommendSearchViewmodel()
        {
            Dstination_typ = "";
            Day_Id = "";
            F_HashTag = "";
        }
       public string Dstination_typ { get; set; }
        public string Day_Id { get; set; }
        public string F_HashTag { get; set; }

        public string HashTag { get; set; }
    }
}