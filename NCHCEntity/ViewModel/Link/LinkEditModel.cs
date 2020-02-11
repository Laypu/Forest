﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class LinkEditModel
    {
        public LinkEditModel() {
            ItemID = -1;
            PublicshStr = DateTime.Now.ToString("yyyy/MM/dd");
            Title = "";
        }
        public int ModelID { get; set; }
        public int ItemID { get; set; }
        public int Group_ID { get; set; }
        public DateTime? StDate { get; set; }
        public DateTime? EdDate { get; set; }
        public string StDateStr { get; set; }
        public string EdDateStr { get; set; }
        public string PublicshStr { get; set; }
        public string Title { get; set; }
        public int? Link_Mode { get; set; }
        public string LinkUrl { get; set; }
        public string LinkUrlDesc { get; set; }
        public string ImageUrl { get; set; }
        public string ImageFileDesc { get; set; }
        public string ImageFileLocation { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public string ImageFileOrgName { get; set; }
        public string ImageFileName { get; set; }
        public string Introduction { get; set; }
        public string VerifyStatus { get; set; }
        public string VerifyUser { get; set; }
        public string VerifyDateTime { get; set; }
        public string CreateUser { get; set; }
        public string CreateDatetime { get; set; }
        public string UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
    }
}
