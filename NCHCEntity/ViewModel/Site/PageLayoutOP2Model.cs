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
   public  class PageLayoutOP2Model
    {
        public PageLayoutOP2Model()
        {

        }
        public bool IsShow { get; set; }
        public int LangID { get; set; }
        public string Title { get; set; }
        public string Introduction { get; set; }
        public string Link { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public int LinkMode { get; set; }
        
    }


}
