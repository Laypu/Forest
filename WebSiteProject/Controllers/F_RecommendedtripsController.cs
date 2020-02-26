using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using WebSiteProject.Code;


namespace WebSiteProject.Controllers
{
    public class F_RecommendedtripsController : AppController
    {
        MasterPageManager _IMasterPageManager;
        ILangManager _ILangManager;
        ISiteLayoutManager _ISiteLayoutManager;
        ILoginManager _ILoginManager;
        IMenuManager _IMenuManager;
        IModelLinkManager _IModelLinkManager;
        ADRightDownManager _ADRightDownManager;

        WebSiteProject.Models.ForestEntities db = new Models.ForestEntities();

        public F_RecommendedtripsController()
        {
            _IMasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
            _ILangManager = serviceinstance.LangManager;
            _ILoginManager = serviceinstance.LoginManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IModelLinkManager = serviceinstance.ModelLinkManager;
            _ADRightDownManager = new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
        }
        // GET: F_Recommendedtrips
        public ActionResult Index(int? langid)
        {
            var site_id = 11; //這是ThingsToDo的輪播ID
            if (Session["LangID"] == null)
            {
                var DefaultLang = System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultLang"];
                _ILangManager = serviceinstance.LangManager;
                var alllang = _ILangManager.GetAll();
                if (alllang != null)
                {
                    if (alllang.Any(v => v.Lang_Name == DefaultLang))
                    {
                        langid = alllang.Where(v => v.Lang_Name == DefaultLang).First().ID.Value;
                    }
                }
                Session["LangID"] = langid.ToString();
                Session.Timeout = 600;
            }
            else
            {
                if (langid == null)
                {
                    int _langid = 1;
                    if (int.TryParse(Session["LangID"].ToString(), out _langid) == false)
                    {
                        langid = 1;
                    }
                    else { langid = _langid; }
                }
                else
                {
                    Session["LangID"] = langid.ToString();
                }
            }

            HomeViewModel viewmodel = new HomeViewModel();
            //讀取logo圖片
            _IMasterPageManager.SetModel<HomeViewModel>(ref viewmodel, "P", langid.ToString(), "");
            viewmodel.SEOScript = _IMasterPageManager.GetSEOData("", "", langid.ToString());
            viewmodel.ADMain = _IMasterPageManager.GetADMain("P", langid.ToString(), site_id);
            viewmodel.ADMobile = _IMasterPageManager.GetADMain("M", langid.ToString(), site_id);
            viewmodel.TrainingSiteData = _ISiteLayoutManager.GetTrainingSiteData(Common.GetLangText("另開新視窗")).AntiXss(new string[] { "class" });

            var sitemenu = _ISiteLayoutManager.PagingMain(new ViewModels.SearchModelBase()
            {
                Limit = 100,
                Key = Device,
                NowPage = 1,
                Offset = 0,
                Sort = "ID",
                LangId = langid.ToString()
            });
            if (sitemenu.total > 0)
            {
                var layoutpagelist = ((List<PageLayout>)sitemenu.rows);
                if (layoutpagelist.Any(v => v.Title == "焦點新聞"))
                {
                    viewmodel.PageLayoutModel1 = _IMasterPageManager.GetSiteLayout(sitemenu.rows, "焦點新聞", langid.ToString()).First();
                }
                else { viewmodel.PageLayoutModel1 = new HomePageLayoutModel(); }
                if (layoutpagelist.Any(v => v.Title == "活動專區"))
                {
                    viewmodel.PageLayoutModel2 = _IMasterPageManager.GetSiteLayout(sitemenu.rows, "活動專區", langid.ToString()).First();
                }
                else { viewmodel.PageLayoutModel2 = new HomePageLayoutModel(); }
            }
            else
            {
                viewmodel.PageLayoutModel1 = new HomePageLayoutModel();
                viewmodel.PageLayoutModel2 = new HomePageLayoutModel();
                ViewBag.sitemenu = new List<PageLayout>();
                ViewBag.sitemenupart = "";
            }
            viewmodel.BannerImage = "";
            viewmodel.PageLayoutOP1 = _ISiteLayoutManager.GetPageLayoutOP1Edit(langid.ToString());
            viewmodel.PageLayoutOP2 = _ISiteLayoutManager.GetPageLayoutOP2Edit(langid.ToString());
            viewmodel.PageLayoutOP3 = _ISiteLayoutManager.GetPageLayoutOP3Edit(langid.ToString());
            viewmodel.PageLayoutActivityModel = _ISiteLayoutManager.PageLayoutActivity(langid.ToString());

            viewmodel.LinkItems = _IModelLinkManager.PagingItem("Y", new SearchModelBase()
            {
                LangId = this.LangID,
                Limit = -1,
                Sort = "Sort"
            }).rows;



            ////標題和內容
            //ViewBag.Title = Server.HtmlDecode(db.RecommendedTrips_Index.FirstOrDefault().RecommendedTrips_Index_Title);
            ViewBag.Content = Server.HtmlDecode(db.RecommendedTrips_Index.FirstOrDefault().RecommendedTrips_Index_Content);

            //五大標題+圖
            ViewBag.F_Thingtodo_Type = db.F_Thingtodo_Type.ToList();

            return View(viewmodel);
        }
    }
}