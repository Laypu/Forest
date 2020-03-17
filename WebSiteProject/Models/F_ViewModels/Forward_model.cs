using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSiteProject.Models.F_ViewModels
{
    public class Forward_model
    {
        public string Sender { get; set; }
        public string SenderEMail { get; set; }
        public string ForwardEMail { get; set; }
        public string ForwardMessage { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
    }
}