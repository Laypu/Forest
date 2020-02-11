﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class MessageSearchModel : SearchModelBase
    {
        public MessageSearchModel()
        {
            Sort = "Sort";
            MessageType = "";
            ListType = "1";
        }
        public int? GroupId { get; set; }
        public string MenuId { get; set; }
        public string MessageType { get; set; }
        public string PublicshDateFrom { get; set; }
        public string PublicshDateTo { get; set; }
        public string DisplayFrom { get; set; }
        public string DisplayTo { get; set; }
        public string Title { get; set; }
        public string Enabled { get; set; }
        public string ListType { get; set; }
        public string IsRange { get; set; }
        public int? ThingsToDoID { get; set; }
    }
}
