using ResourceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels
{
   public  class PageLayoutOP1Model
    {
        public PageLayoutOP1Model()
        {

        }
        public bool IsShow { get; set; }
        public int LangID { get; set; }
        public string LeftTitle { get; set; }
        public string RightTitle { get; set; }
        public string Introduction { get; set; }
        public string RightLink { get; set; }
        public int RightLinkMode { get; set; }
        public PageLayoutOP1ModelItem[] LeftItem { get; set; }
        public PageLayoutOP1ModelItem[] RightItem { get; set; }
    }

    public class PageLayoutOP1ModelItem
    {
        public PageLayoutOP1ModelItem()
        {

        }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int Index { get; set; }
        public string Desc { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public int Link_Mode { get; set; }


    }

}
