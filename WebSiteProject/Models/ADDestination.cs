
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
    
public partial class ADDestination
{

    public int ID { get; set; }

    public Nullable<int> Destination_Type_ID { get; set; }

    public Nullable<int> Lang_ID { get; set; }

    public Nullable<int> Site_ID { get; set; }

    public Nullable<int> Sort { get; set; }

    public string Type_ID { get; set; }

    public Nullable<System.DateTime> StDate { get; set; }

    public Nullable<System.DateTime> EdDate { get; set; }

    public string AD_Name { get; set; }

    public string Img_Show_Name { get; set; }

    public string Img_Name_Ori { get; set; }

    public string Img_Name_Thumb { get; set; }

    public Nullable<int> AD_Width { get; set; }

    public Nullable<int> AD_Height { get; set; }

    public string Link_Href { get; set; }

    public string Link_Mode { get; set; }

    public Nullable<bool> Fixed { get; set; }

    public Nullable<System.DateTime> Review_Date { get; set; }

    public Nullable<bool> Enabled { get; set; }

    public Nullable<System.DateTime> Create_Date { get; set; }

    public Nullable<System.DateTime> UpdateDatetime { get; set; }

    public string UpdateUser { get; set; }

    public string SType { get; set; }

    public string UploadVideoFileName { get; set; }

    public string UploadVideoFilePath { get; set; }

    public string UploadVideoFileDesc { get; set; }

    public string ADDesc { get; set; }



    public virtual F_Destination_Type F_Destination_Type { get; set; }

}

}