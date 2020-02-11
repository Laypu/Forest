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
   public  class PageLayoutActivityModel
    {
        public PageLayoutActivityModel()
        {

        }
        public int LangID { get; set; }
        public string Link { get; set; }
        public PageLayoutModelItem[] Items { get; set; }
    }

    public class PageLayoutModelItem
    {
        public PageLayoutModelItem()
        {

        }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int Index { get; set; }
        public string Desc { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
    }

}
