using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
     public class RecommendedSearchModel
    {
        public int? RecommendedTrips_ID { get; set; }
        public int? RecommendedTrips_Day_ID { get; set; }
        public int? RecommendedTrips_Destinations_ID { get; set; }
        public int? HashTag_Type_ID { get; set; }
        public int? sort { get; set; }
        public DateTime? starDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string RecommendedTrips_Day_Name { get; set; }
        public string RecommendedTrips_Title { get; set; }
        public string RecommendedTrips_Index_Content { get; set; }
        public string RecommendedTrips_Img { get; set; }
        public string RecommendedTrips_Img_Description { get; set; }
        public string RecommendedTrips_Img_Img { get; set; }

        public bool Enabled { get; set; }
    }
}
