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
    
    public partial class GroupActive
    {
        public int ID { get; set; }
        public Nullable<int> Main_ID { get; set; }
        public Nullable<int> Site_ID { get; set; }
        public Nullable<int> Sort { get; set; }
        public string Group_Name { get; set; }
        public Nullable<bool> Seo_Manage { get; set; }
        public Nullable<bool> Readonly { get; set; }
        public Nullable<bool> Enabled { get; set; }
        public Nullable<System.DateTime> UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
    }
}
