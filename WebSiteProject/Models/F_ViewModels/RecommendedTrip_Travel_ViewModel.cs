using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSiteProject.Models.F_ViewModels
{
    public class RecommendedTrip_Travel_ViewModel
    {
        public int? RecommendedTrip_Travel_ID { get; set; }
        public int? RecommendedTrips_ID { get; set; }
        public string RecommendedTrip_Travel_Title { get; set; }
        public string RecommendedTrips_Cate_Name { get; set; }
    }
}