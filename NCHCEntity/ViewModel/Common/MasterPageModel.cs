﻿using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
    public class MasterPageModel
    {
        public MasterPageModel() {
            ADMain = new List<ADBase>();
        }
        public string AdminHost { get; set; }
        public string LogoUrl { get; set; }
        public string LogoHeight { get; set; }
        public IList<ADBase> ADMain { get; set; }
        public IList<ADBase> ADCenter { get; set; }
        public IList<ADBase> ADRight { get; set; }
        public IList<ADBase> ADRightDown { get; set; }
        public IList<ADBase> ADDown { get; set; }
        public IList<ADBase> ADMobile { get; set; }
        public string TopMenu { get; set; }
        public string TopMobileMenu { get; set; }
        public string LeftMenu { get; set; }
        public string MainMenu { get; set; }
        public string FooterMenu { get; set; }
        public string FooterString { get; set; }
        public string SearchKey { get; set; }
        public string[] SEOScript { get; set; }
        public string IsShowSearch { get; set; }
        public int ShowModel { get; set; }
        public string BannerImage { get; set; }
        public string Device { get; set; }
        public string LangId { get; set; }
        public string SEOTitle { get; set; }
        public string SEOTitleOrg { get; set; }
        public string MasterMainTitle { get; set; }
        public string InnerLogoUrl { get; set; }
        public string PublishContent { get; set; }
        public string PrintContent { get; set; }
        public string Fontsize { get; set; }
        public string FBImage { get; set; }
        public string FBTitle { get; set; }
        public string PrintImageUrl { get; set; }
        public string SEOScriptCode { get; set; }

        public List<Menu> Footer { get; set; }

        public List<string> FooterMenuString { get; set; }
    }

}
