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
    
    public partial class VideoUnitSetting
    {
        public int ID { get; set; }
        public Nullable<int> MainID { get; set; }
        public Nullable<bool> IsPrint { get; set; }
        public Nullable<bool> IsForward { get; set; }
        public Nullable<bool> IsRSS { get; set; }
        public Nullable<bool> IsShare { get; set; }
        public Nullable<bool> MemberAuth { get; set; }
        public Nullable<bool> ClassOverview { get; set; }
        public Nullable<int> ShowCount { get; set; }
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public string Column6 { get; set; }
        public string Column7 { get; set; }
        public Nullable<System.DateTime> UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<bool> GeneralStudentAuth { get; set; }
        public Nullable<bool> EnterpriceStudentAuth { get; set; }
        public Nullable<bool> VIPAuth { get; set; }
        public Nullable<bool> EMailAuth { get; set; }
        public string ColumnSetting { get; set; }
        public string IntroductionHtml { get; set; }
    }
}
