
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
    
public partial class LinkItem
{

    public int ItemID { get; set; }

    public Nullable<int> Lang_ID { get; set; }

    public Nullable<int> GroupID { get; set; }

    public Nullable<int> Sort { get; set; }

    public string Title { get; set; }

    public string ImageFileName { get; set; }

    public string ImageFileOrgName { get; set; }

    public string ImageFileDesc { get; set; }

    public string LinkUrl { get; set; }

    public string LinkUrlDesc { get; set; }

    public Nullable<System.DateTime> StDate { get; set; }

    public Nullable<System.DateTime> EdDate { get; set; }

    public Nullable<System.DateTime> PublicshDate { get; set; }

    public Nullable<bool> Enabled { get; set; }

    public Nullable<int> Link_Mode { get; set; }

    public Nullable<System.DateTime> UpdateDatetime { get; set; }

    public string UpdateUser { get; set; }

    public string Introduction { get; set; }

    public Nullable<System.DateTime> CreateDatetime { get; set; }

    public string CreateUser { get; set; }

    public string CreateName { get; set; }

    public string UpdateName { get; set; }

    public Nullable<bool> IsVerift { get; set; }

}

}
