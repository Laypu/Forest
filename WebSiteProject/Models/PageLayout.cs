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
    
    public partial class PageLayout
    {
        public int ID { get; set; }
        public Nullable<int> LangID { get; set; }
        public Nullable<int> BlockWidth { get; set; }
        public string Title { get; set; }
        public string ImgNameOri { get; set; }
        public string ImgShowName { get; set; }
        public string ImageDesc { get; set; }
        public Nullable<int> LinkMode { get; set; }
        public Nullable<int> ModelID { get; set; }
        public Nullable<int> ModelItemID { get; set; }
        public Nullable<int> MenuItem { get; set; }
        public Nullable<int> MenuLevel1 { get; set; }
        public Nullable<int> MenuLevel2 { get; set; }
        public Nullable<int> MenuLevel3 { get; set; }
        public Nullable<int> OpenMode { get; set; }
        public Nullable<int> Sort { get; set; }
        public Nullable<bool> Status { get; set; }
        public string OpenModeCust { get; set; }
        public Nullable<System.DateTime> UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public string SType { get; set; }
        public string ModelItemList { get; set; }
    }
}
