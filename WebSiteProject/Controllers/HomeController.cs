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
    public class HomeController : AppController
    {
        MasterPageManager _IMasterPageManager;
        ILangManager _ILangManager;
        ISiteLayoutManager _ISiteLayoutManager;
        ILoginManager _ILoginManager;
        IMenuManager _IMenuManager;
        IModelLinkManager _IModelLinkManager;
        ADRightDownManager _ADRightDownManager;

        WebSiteProject.Models.ForestEntities db = new Models.ForestEntities();

        public HomeController()
        {
            _IMasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
            _ILangManager = serviceinstance.LangManager;
            _ILoginManager = serviceinstance.LoginManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IModelLinkManager = serviceinstance.ModelLinkManager;
            _ADRightDownManager =new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
        }
        // GET: WebSite/Home
        public ActionResult Index(int? langid)
        {
            var site_id = 1; //這是Index的輪播ID
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
            else {
                if (langid == null)
                {
                    int _langid = 1;
                    if (int.TryParse(Session["LangID"].ToString(), out _langid) == false)
                    {
                        langid = 1;
                    }
                    else{ langid = _langid;}
                }
                else {
                    Session["LangID"] = langid.ToString();
                }
            }

            HomeViewModel viewmodel = new HomeViewModel();
            //讀取logo圖片
            _IMasterPageManager.SetModel<HomeViewModel>(ref viewmodel,"P", langid.ToString(), "");
            viewmodel.SEOScript = _IMasterPageManager.GetSEOData("", "", langid.ToString());
            viewmodel.ADMain = _IMasterPageManager.GetADMain("P", langid.ToString(), site_id);
            viewmodel.ADMobile = _IMasterPageManager.GetADMain("M", langid.ToString(), site_id);
            viewmodel.TrainingSiteData = _ISiteLayoutManager.GetTrainingSiteData(Common.GetLangText("另開新視窗")).AntiXss(new string[] { "class"});
            
            var sitemenu = _ISiteLayoutManager.PagingMain(new ViewModels.SearchModelBase()
            {
                Limit = 100,
                Key = Device,
                NowPage = 1,
                Offset = 0,
                Sort = "ID",
                LangId = langid.ToString()
            });
            if (sitemenu.total>0)
            {
                var layoutpagelist= ((List<PageLayout>)sitemenu.rows);
                if (layoutpagelist.Any(v=>v.Title== "焦點新聞")) {
                    viewmodel.PageLayoutModel1 = _IMasterPageManager.GetSiteLayout(sitemenu.rows, "焦點新聞", langid.ToString()).First();
                }
                else { viewmodel.PageLayoutModel1 = new HomePageLayoutModel(); }
                if (layoutpagelist.Any(v => v.Title == "活動專區"))
                {
                    viewmodel.PageLayoutModel2 = _IMasterPageManager.GetSiteLayout(sitemenu.rows, "活動專區", langid.ToString()).First();
                }else { viewmodel.PageLayoutModel2 = new HomePageLayoutModel(); }
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
                 Sort="Sort"
            }).rows;

            //自由編輯區1
            var FreeZone1 = db.FreeZoneTitles.Find(1);            
            ViewBag.FreeZone1_Title = FreeZone1.Title;
            ViewBag.FreeZone1_Content = Server.HtmlDecode(FreeZone1.FreeZoneTitleContent.safeHtmlFragment());
            ViewBag.FreeZone1_Link = db.FreeZoneContents.Where(f => f.FreeZoneTitleID == 1).ToList();

            //自由編輯區2
            var FreeZone2 = db.FreeZoneTitles.Find(2);
            ViewBag.FreeZone2_Title = FreeZone2.Title;
            ViewBag.FreeZone2_Content = Server.HtmlDecode(FreeZone2.FreeZoneTitleContent.safeHtmlFragment());
            ViewBag.FreeZone2_Link = db.FreeZoneContents.Where(f => f.FreeZoneTitleID == 2).ToList();

            //輪播牆2
            ViewBag.ADMain2 = db.ADMains.Where(ad => ad.Site_ID == 2 && ad.SType == "P").ToList();

            //大圖片區
            ViewBag.BImgName = db.F_Index_Img.FirstOrDefault().Index_Img_Name;

            //五大標題+圖
            ViewBag.F_Thingtodo_Type = db.F_Thingtodo_Type.ToList();

            return View(viewmodel);
        }
        public ActionResult Info()
        {
            return View();
        }

        public ActionResult ADRigthDownDetail(string email)
        {
            var langid = _IMasterPageManager.CheckLangID("");
            var model = _IMasterPageManager.GetModel(Device, langid,"");
            var imagestrArr = _ILoginManager.GetCaptchImage();
            ViewBag.catchstr = imagestrArr[0];
            ViewBag.langid = langid;
            ViewBag.stype = Device;
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult noJavascript()
        {
            Session["NoJacascript"] = 1;
            return Json(Url.Content("~/img/40x40.png"));
        }

        public ActionResult NoJsTemplate()
        {
            return PartialView();
        }
        public ActionResult IndexCh()
        {
            return PartialView();
        }
        public ActionResult IndexEn()
        {
            return PartialView();
        }
        public ActionResult LoginForm()
        {
            return PartialView();
        }
        public ActionResult ChangeLang(string lang)
        {
            var langlist = _ILangManager.GetAll();
            IList<Lang> uselang;
            if (lang != null)
            {
                uselang = langlist.Where(v => v.ID == int.Parse(lang)).ToList();
            }
            else
            {
                var nowlang = this.LangID;
                uselang = langlist.Where(v => v.ID != int.Parse(nowlang)).ToList();
                if (uselang.Count() > 0)
                {
                    lang = uselang.First().ID.ToString();
                }
                else
                {
                    lang = this.LangID;
                }
            }
            if (uselang.Count() > 0)
            {
                var dtype = uselang.First().Domain_Type;
                if (dtype == "1")
                {
                    Session["LangID"] = lang;
                }
                else if (dtype == "2")
                {
                    Session["LangID"] = uselang.First().Link_Lang_ID;
                }
                else if (dtype == "3")
                {
                    return Json(uselang.First().Link_Href);
                }
                Session.Timeout = 600;
            }
            return RedirectToAction("Index","Home");
        }
        public ActionResult SetLang(string lang)
        {
            var langlist = _ILangManager.GetAll();
            IList<Lang> uselang;
            if (lang != null)
            {
                 uselang = langlist.Where(v => v.ID == int.Parse(lang)).ToList();
            }
            else {
                var nowlang = this.LangID;
                uselang = langlist.Where(v => v.ID != int.Parse(nowlang)).ToList();
                if (uselang.Count() > 0) {
                    lang = uselang.First().ID.ToString();
                }
                else {
                    lang = this.LangID;
                }
            }
            if (uselang.Count() > 0)
            {
                var dtype = uselang.First().Domain_Type;
                if (dtype == "1")
                {
                    Session["LangID"] = lang;
                }
                else if (dtype == "2")
                {
                    Session["LangID"] = uselang.First().Link_Lang_ID;
                }
                else if (dtype == "3")
                {
                    return Json(uselang.First().Link_Href);
                }
                Session.Timeout = 600;
            }

            return Json("");
        }

        protected IAuthenticationManager Authentication
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        [AllowAnonymous]
        #region CaptchRefresh
        public ActionResult CaptchRefresh()
        {
            var imagestrArr = _ILoginManager.GetCaptchImage();
            Session["Captch"] = imagestrArr[0];
            return Json(new string[] { imagestrArr[0], imagestrArr[1] });
        }
        #endregion

        public ActionResult CheckDownload(string mid)
        {       
            return Json("");
        }

        #region FileDownLoad
        public ActionResult FileDownLoad(string mid)
        {
            var model = _IMenuManager.GetModel(mid);
            if (string.IsNullOrEmpty(model.LinkUploadFilePath))
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
            else {
                if (System.IO.File.Exists(model.LinkUploadFilePath) == false) {
                    return Redirect(Request.UrlReferrer.AbsoluteUri);
                }
            }
            string filepath = model.LinkUploadFilePath;
            string oldfilename = model.LinkUploadFileName;
            if (filepath != "")
            {
                string filename = System.IO.Path.GetFileName(filepath);
                if (string.IsNullOrEmpty(oldfilename)) { oldfilename = filename; }
                Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                TempData["model"] = model;
                return File(iStream, "application/octet-stream", oldfilename);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }
        #endregion

        #region PagingItem
        public ActionResult PagingADRightDownItem(ADSearchModel model)
        {
            var data = _ADRightDownManager.PagingForWebSite( model);
            return Json(data);
        }
        #endregion

        #region PagingItem
        public ActionResult GetAuthError()
        {
            if (Session["ShowMessage"] != null)
            {
                var message = Session["ShowMessage"];
                Session.Remove("ShowMessage");
                return Json(message);
            }
            return Json("");
        }
        #endregion

        #region SetFontSize
        public ActionResult SetFontSize(string size)
        {
            Session["fontsize"] = size;
            return Json("");
        }
        #endregion

        #region F_Video_Index
        public ActionResult F_Video_Index(int? langid, int id = 5)
        {
            var site_id = 10; //這是Index的輪播ID

            //模組勿動
            #region 模組
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


            

            WebSiteProject.Models.ForestEntities db = new Models.ForestEntities();
            ViewBag.F_Video = db.VideoItems.Where(f => f.ModelID == 5 && f.Enabled == true && f.IsVerift == true).OrderBy(f => f.Sort).ToList();



            //if (db.VideoItems.Any(f => f.StDate == null))
            //{
            //    if (db.VideoItems.Any(f => f.EdDate == null))
            //    {
            //        ViewBag.F_Video = db.VideoItems.Where(f => f.ModelID == 5 && f.Enabled == true && f.IsVerift == true ).OrderBy(f => f.Sort).ToList();
            //    }
            //    ViewBag.F_Video = db.VideoItems.Where(f => f.ModelID == 5 && f.Enabled == true && f.IsVerift == true && f.EdDate >= date).OrderBy(f => f.Sort).ToList();
            //}
            //else if (db.VideoItems.Any(f => f.EdDate == null))
            //{
            //    ViewBag.F_Video = db.VideoItems.Where(f => f.ModelID == 5 && f.Enabled == true && f.IsVerift == true && f.StDate <= date).OrderBy(f => f.Sort).ToList();

            //}
            //else {
            //    ViewBag.F_Video = db.VideoItems.Where(f => f.ModelID == 5 && f.Enabled == true && f.IsVerift == true && f.StDate <= date && f.EdDate >= date).OrderBy(f => f.Sort).ToList();

            //}

            return View(viewmodel);
        }
        #endregion

        #region F_Video_Detail
        public ActionResult F_Video_Detail(int? langid, int itemid)
        {
            //模組勿動
            #region 模組
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


            WebSiteProject.Models.ForestEntities db = new Models.ForestEntities();
            ViewBag.F_Video = db.VideoItems.Where(f => f.ModelID == 5 && f.Enabled == true && f.IsVerift == true && f.ItemID == itemid).ToList();

            return View(viewmodel);
        }
        #endregion


        public ActionResult HashTag(int? langid=1, string HashTag ="test")
        {

            //ViewBag.MessageItems = db.MessageItems.ToList();
            var site_id = 3;
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

            ViewBag.F_Destination_Type = db.F_Destination_Type.ToList();

            ViewBag.Destination_Fare = db.Destination_Fare.ToList();

            var q = from M in db.MessageItems
                    join HT in db.Message10Hash
                    on M.ItemID equals HT.MessageItem_ID
                    
                    where HT.Message10Hash_1H.ToLower() == HashTag.ToLower() ||
                          HT.Message10Hash_2H.ToLower() == HashTag.ToLower() ||
                          HT.Message10Hash_3H.ToLower() == HashTag.ToLower() ||
                          HT.Message10Hash_4H.ToLower() == HashTag.ToLower() ||
                          HT.Message10Hash_5H.ToLower() == HashTag.ToLower() ||
                          HT.Message10Hash_6H.ToLower() == HashTag.ToLower() ||
                          HT.Message10Hash_7H.ToLower() == HashTag.ToLower() ||
                          HT.Message10Hash_8H.ToLower() == HashTag.ToLower() ||
                          HT.Message10Hash_9H.ToLower() == HashTag.ToLower() ||
                          HT.Message10Hash_10H.ToLower() == HashTag.ToLower()
                    select M;

            ViewBag.MessageItems = q.ToList();

            ViewBag.Message10Hash = db.Message10Hash.ToList();

            return View(viewmodel);
        }

        public ActionResult ArticleSearch(int? langid, string Hashtag)
        {
            var site_id = 4;
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

            var q = from M in db.MessageItems
                    join H in db.Message10Hash
                    on M.ItemID equals H.MessageItem_ID
                    where H.Message10Hash_1H == Hashtag ||
                          H.Message10Hash_2H == Hashtag ||
                          H.Message10Hash_3H == Hashtag ||
                          H.Message10Hash_4H == Hashtag ||
                          H.Message10Hash_5H == Hashtag ||
                          H.Message10Hash_6H == Hashtag ||
                          H.Message10Hash_7H == Hashtag ||
                          H.Message10Hash_8H == Hashtag ||
                          H.Message10Hash_9H == Hashtag ||
                          H.Message10Hash_10H == Hashtag
                    select M;

            ViewBag.Hash = q.ToList();

            ViewBag.Message10Hash = db.Message10Hash.ToList();

            ViewBag.HashTagName = Hashtag;
            return View(viewmodel);
        }

        public ActionResult Article(int? langid = 1, int? Aid = 37, string Hashtag = "test")
        {
            ViewBag.Unit = db.MessageUnitSettings.Where(p => p.MainID == 9).Select(p => new UnitPrint { isPrint = (bool)p.IsPrint, isForward = (bool)p.IsForward, isRSS = (bool)p.IsRSS, isShare = (bool)p.IsShare }).FirstOrDefault();
            //ViewBag.PagePrt = Url.Action("Print", "Home", new { id = RecommendedTrips_ID });


            var site_id = 4;
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
            
            ViewBag.DesHash = db.MessageItems.Where(M => M.ItemID == Aid).ToList();
            ViewBag.MessageBanner = db.MessageBanners.Where(B => B.MessageItem_ID == Aid).First().MessageBanner_Img;
            ViewBag.HashTagName = Hashtag;
            return View(viewmodel);
        }



    }
}