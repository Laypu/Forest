
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

        this.RecommendedTrip_Travel = new HashSet<RecommendedTrip_Travel>();

    }


    public int RecommendedTrips_ID { get; set; }

    public Nullable<int> RecommendedTrips_Day_ID { get; set; }

    public Nullable<int> RecommendedTrips_Destinations_ID { get; set; }

    public string RecommendedTrips_Img { get; set; }

    public string RecommendedTrips_Title { get; set; }

    public string RecommendedTrips_Content { get; set; }

    public string RecommendedTrips_Location { get; set; }

    public string RecommendedTrips_HtmContent { get; set; }

    public string RecommendedTrips_Img_Description { get; set; }

    public string RecommendedTrips_UploadFileName { get; set; }

    public string RecommendedTrips_UploadFilePath { get; set; }

    public string RecommendedTrips_UploadFileDesc { get; set; }

    public string RecommendedTrips_LinkUrl { get; set; }

    public string RecommendedTrips_LinkUrlDesc { get; set; }

    public Nullable<System.DateTime> RecommendedTrips_StarDay { get; set; }

    public Nullable<System.DateTime> RecommendedTrips_EndDay { get; set; }

    public Nullable<int> Sort { get; set; }

    public Nullable<int> InFront { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<RecommendedTrip_Travel> RecommendedTrip_Travel { get; set; }

    public virtual RecommendedTrips_Day RecommendedTrips_Day { get; set; }

    public virtual F_Destination_Type F_Destination_Type { get; set; }

}

}
