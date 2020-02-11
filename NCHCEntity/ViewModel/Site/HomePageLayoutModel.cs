using ResourceLibrary;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class HomeViewModel: MasterPageModel
    {
        public HomeViewModel()
        {

        }
        public HomePageLayoutModel PageLayoutModel1 { get; set; }
        public HomePageLayoutModel PageLayoutModel2 { get; set; }
        public PageLayoutOP1Model PageLayoutOP1 { get; set; }
        public PageLayoutOP2Model PageLayoutOP2 { get; set; }
        public PageLayoutOP3Model PageLayoutOP3 { get; set; }
        public IList<LinkItemResult> LinkItems { get; set; }
        public PageLayoutActivityModel PageLayoutActivityModel { get; set; }
        public string TrainingSiteData { get; set; }
    }

    public  class HomePageLayoutModel
    {
        public HomePageLayoutModel()
        {
            ID = -1;
            LinkUrl = new List<string>();
            LinkImageSrc = new List<string>();
            PublicshDate = new List<string>();
            HtmlContent = new List<string>();
            ModelGroup = new List<string>();
            JustView = new List<bool>();
            Title = new List<string>();
        }
        public int ID { get; set; }
        public int MenuID { get; set; }
        public string Name { get; set; }
        public string ImageOri { get; set; }
        public List<string> Title { get; set; }
        public List<string> LinkUrl { get; set; }
        public List<string> LinkImageSrc { get; set; }
        public List<string> PublicshDate { get; set; }
        public List<string> HtmlContent { get; set; }
        public List<string> ModelGroup { get; set; }
        public List<bool> JustView { get; set; }
        public string MoreLink { get; set; }
    }
}
