﻿using SQLModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLModel.Models
{
    public class SiteConfig
    {
        [Key]
        [IsSequence]
        public int? ID { get; set; }
        public string Login_Title { get; set; }
        public string Img_Name_Ori1 { get; set; }
        public string Img_Name_Thumb1 { get; set; }
        public string Img_Name_Ori2 { get; set; }
        public string Img_Name_Thumb2 { get; set; }
        public string Comp_Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public string Page_Title { get; set; }
        public string Img_Name_Ori3 { get; set; }
        public string Img_Name_Thumb3 { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public string UpdateUser { get; set; }
        public string MailServerIP { get; set; }
        public int? Port { get; set; }
        public string EMailAccount { get; set; }
        public string EMailPassword { get; set; }
        public bool? IsAuthMailServer { get; set; }
        public string QuestionnaireFinishDesc { get; set; }
        public string InvoiceDesc { get; set; }
        public string InvoiceMailSender { get; set; }
        public string InvoiceMailSenderMail { get; set; }
        public string InvoiceMailSenderTitle { get; set; }
        public int? FirstPageMaxLinkItem { get; set; }
        
    }
}
