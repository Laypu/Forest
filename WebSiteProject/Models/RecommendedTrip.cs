//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebSiteProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class RecommendedTrip
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RecommendedTrip()
        {
            this.RecommendedTrips_HashTag_Type = new HashSet<RecommendedTrips_HashTag_Type>();
        }
    
        public int RecommendedTrips_ID { get; set; }
        public Nullable<int> RecommendedTrips_Day_ID { get; set; }
        public string RecommendedTrips_img { get; set; }
        public string RecommendedTrips_Title { get; set; }
        public string RecommendedTrips_Content { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RecommendedTrips_HashTag_Type> RecommendedTrips_HashTag_Type { get; set; }
        public virtual RecommendedTrips_Day RecommendedTrips_Day { get; set; }
    }
}