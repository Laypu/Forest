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
    
    public partial class F_Destination_Type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public F_Destination_Type()
        {
            this.ADDestinations = new HashSet<ADDestination>();
            this.Destination_Fare = new HashSet<Destination_Fare>();
            this.Message_DesHash = new HashSet<Message_DesHash>();
            this.RecommendedTrips = new HashSet<RecommendedTrip>();
        }
    
        public int Destination_Type_ID { get; set; }
        public string Destination_Type_Title1 { get; set; }
        public string Destination_Type_Title2 { get; set; }
        public Nullable<System.DateTime> Destination_Type_CreateDate { get; set; }
        public string Destination_Type_ImgName { get; set; }
        public string Destination_Type_ImgDescription { get; set; }
        public string Destination_Type_Link { get; set; }
        public string Destination_Type_Description { get; set; }
        public string Destination_Type_MapName { get; set; }
        public string Destination_Type_Location { get; set; }
        public string Destination_Type_ServiceHours { get; set; }
        public string Destination_Type_Area { get; set; }
        public string Destination_Type_Altitude { get; set; }
        public Nullable<decimal> Temp_Spring { get; set; }
        public Nullable<decimal> Temp_Summer { get; set; }
        public Nullable<decimal> Temp_Autumn { get; set; }
        public Nullable<decimal> Temp_Winter { get; set; }
        public string FPI { get; set; }
        public string FPII { get; set; }
        public string FPIII { get; set; }
        public string FPIV { get; set; }
        public string FPV { get; set; }
        public string FPVI { get; set; }
        public string FPVII { get; set; }
        public string FPVIII { get; set; }
        public string FPIX { get; set; }
        public string FPX { get; set; }
        public string Banner_Img { get; set; }
        public string Recommend_Img { get; set; }
        public string Recommend_Detail_Img { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADDestination> ADDestinations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Destination_Fare> Destination_Fare { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Message_DesHash> Message_DesHash { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RecommendedTrip> RecommendedTrips { get; set; }
    }
}
