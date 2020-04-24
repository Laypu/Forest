using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using ViewModels;
using WebSiteProject.Models.F_ViewModels;


namespace WebSiteProject.Controllers
{
    public class F_ThingsToDoController : AppController
    {
        MasterPageManager _IMasterPageManager;
        ILangManager _ILangManager;
        ISiteLayoutManager _ISiteLayoutManager;
        ILoginManager _ILoginManager;
        IMenuManager _IMenuManager;
        IModelLinkManager _IModelLinkManager;
        ADRightDownManager _ADRightDownManager;

        WebSiteProject.Models.ForestEntities db = new Models.ForestEntities();

        public F_ThingsToDoController()
        {
            _IMasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
            _ILangManager = serviceinstance.LangManager;
            _ILoginManager = serviceinstance.LoginManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IModelLinkManager = serviceinstance.ModelLinkManager;
            _ADRightDownManager = new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
        }

        // GET: F_ThingsToDo
        public ActionResult Index(int? langid)
        {
            var site_id = 4; //這是ThingsToDo的輪播ID
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



            //標題和內容
            ViewBag.Title = Server.HtmlDecode(db.F_Thingtodo_Index.FirstOrDefault().F_Thingtodo_Index_Title);
            ViewBag.Content = Server.HtmlDecode(db.F_Thingtodo_Index.FirstOrDefault().F_Thingtodo_Index_Content);

            //五大標題+圖
            ViewBag.F_Thingtodo_Type = db.F_Thingtodo_Type.ToList();



            return View(viewmodel);
        }


        #region ThingToDo_List
        public ActionResult ThinngsToDo_List(int? langid, int F_TTD_Id)
        {
            var site_id = 5; //這是ThingsToDo的輪播ID
            var mode_id = 9; //這是ThingsToDo的ModeID
            var list_id = 1; //這是ThingsToDo的訊息列表ID

            switch (F_TTD_Id)
            {
                case 1:
                    site_id = 5; //這是ThingsToDo的輪播ID
                    list_id = 1; //這是ThingsToDo的訊息列表ID

                    break;
                case 2:
                    site_id = 6; //這是ThingsToDo的輪播ID
                    list_id = 2; //這是ThingsToDo的訊息列表ID

                    break;
                case 3:
                    site_id = 7; //這是ThingsToDo的輪播ID
                    list_id = 3; //這是ThingsToDo的訊息列表ID

                    break;
                case 4:
                    site_id = 8; //這是ThingsToDo的輪播ID
                    list_id = 4; //這是ThingsToDo的訊息列表ID
                    break;
                case 5:
                    site_id = 9; //這是ThingsToDo的輪播ID
                    list_id = 5; //這是ThingsToDo的訊息列表ID

                    break;
            }            

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

            //Thingstodo_Description
            ViewBag.F_Thingtodo_Type_Description = db.F_Thingtodo_Type.Find(F_TTD_Id).F_Thingtodo_Type_Description;

            //Title
            ViewBag.F_Thingtodo_Title = db.F_Thingtodo_Type.Where( f => f.F_Thingtodo_Type_ID == F_TTD_Id).FirstOrDefault().F_Thingtodo_Type_Title1 + " " + db.F_Thingtodo_Type.Where(f => f.F_Thingtodo_Type_ID == F_TTD_Id).FirstOrDefault().F_Thingtodo_Type_Title2;

            //Thingstodo_各類別_Description
            ViewBag.F_Thingtodo__Description = Server.HtmlDecode(db.F_Thingtodo_Type.Find(F_TTD_Id).F_Thingtodo_Type_Description);

            ViewBag.F_TTD_Id = F_TTD_Id;
            //List
            var q = from M in db.MessageItems
                    join H in db.F_Sub_HashTag_Type
                    on M.ItemID equals H.MessageItem_ID
                    where H.HashTag_Type_ID == F_TTD_Id && M.Enabled == true
                    orderby M.Sort
                    select M;

            ViewBag.Five_Thingstodo_List = q.ToList();


            //ViewBag.Five_Thingstodo_List = db.MessageItems.Join(
            //db.F_Sub_HashTag_Type,
            //f => f.ItemID,
            //f1 => f1.MessageItem_ID,
            //(f, f1) => new WebSiteProject.Models.F_ViewModels.F_ThingsToDo_List_ViewModel
            //{
            //    ItemID = f.ItemID,
            //    RelateImageFileName = f.RelateImageFileName,
            //    Title = f.Title,
            //    ModelID = f.ModelID,
            //    HashTag_Type_ID = f1.HashTag_Type_ID,
            //    Sort = f.Sort,
            //    IsVerift = f.IsVerift
            //}
            //).Where(f => f.ModelID == mode_id && f.HashTag_Type_ID == list_id && f.IsVerift == true).OrderBy(f => f.Sort).ToList();

            ViewBag.Message10Hash = db.Message10Hash.ToList();


            return View(viewmodel);
        }
        #endregion


        #region Article
        public ActionResult Article(int? langid, int listid,string TTDTitle)
        {
            #region 模組勿動
            var site_id = 4; //這是ThingsToDo的輪播ID
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
            #endregion

            ViewBag.TTD_Detail = db.MessageItems.Find(listid);
            ViewBag.MessageBanner = db.MessageBanners.Where(m => m.MessageItem_ID == listid).FirstOrDefault().MessageBanner_Img;
            ViewBag.Category = TTDTitle;
            ViewBag.Unit = db.MessageUnitSettings.Where(p => p.MainID == 9).Select(p => new UnitPrint { isPrint = (bool)p.IsPrint, isForward = (bool)p.IsForward, isRSS = (bool)p.IsRSS, isShare = (bool)p.IsShare }).FirstOrDefault();


            return View(viewmodel);
        }
        #endregion


        #region GetHasgTag
        [HttpGet]
        public ActionResult GetHashTag(int id)
        {
            var HashTag_Data = db.F_HashTag_Type.Where(f => f.F_Sub_HashTag_Type.Any( f1 => f1.MessageItem_ID == id)).Select( f => new { f.HashTag_Type_Name, f.HashTag_Type_Link});
            
            return Json(HashTag_Data,JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}