﻿using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utilities;
using ViewModels;
using WebSiteProject.Code;
using WebSiteProject.Models.F_ViewModels;

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
        #region recommended_list
        public ActionResult recommended_list(int? langid, RecommendSearchViewmodel search)
        {
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

            var Destination_Typ = db.F_Destination_Type;
            var Day_ID_ = db.RecommendedTrips_Day;
            var F_HashTag_Type_ = db.F_HashTag_Type;
            var Destinations_ID = new List<SelectListItem>();
            foreach (var item in Destination_Typ)
            {
                Destinations_ID.Add(new SelectListItem()
                {
                    Text = item.Destination_Type_Title1 + " " + item.Destination_Type_Title2,
                    Value = Convert.ToString(item.Destination_Type_ID),
                    Selected = false
                });
            }
            var Day_ID = new List<SelectListItem>();
            foreach (var item in Day_ID_)
            {
                Day_ID.Add(new SelectListItem()
                {
                    Text = item.RecommendedTrips_Day_Name,
                    Value = Convert.ToString(item.RecommendedTrips_Day_ID),
                    Selected = false
                });
            }
            var HashTag_Type = new List<SelectListItem>();
            foreach (var item in F_HashTag_Type_)
            {
                HashTag_Type.Add(new SelectListItem()
                {
                    Text = item.HashTag_Type_Name,
                    Value = Convert.ToString(item.HashTag_Type_ID),
                    Selected = false
                });
            }
            ViewBag.day_id = search.Day_Id == "" || search.Day_Id ==null? "-1" : search.Day_Id;
            ViewBag.dstination_typ = search.Dstination_typ == "" || search.Dstination_typ == null ? "-1" : search.Dstination_typ;
            ViewBag.f_HashTag = search.F_HashTag == "" || search.F_HashTag == null ? "-1" : search.F_HashTag;
            if (ViewBag.day_id != "-1")
            {
                Day_ID.Where(q => q.Value == ViewBag.day_id).First().Selected = true;
            }
            if (ViewBag.dstination_typ != "-1" )
                {
                    HashTag_Type.Where(q => q.Value == ViewBag.dstination_typ).First().Selected = true;
                }
             
           if (ViewBag.f_HashTag != "-1")
            {
                Destinations_ID.Where(q => q.Value == ViewBag.f_HashTag).First().Selected = true;
            }
            ViewBag.RecommendedTrips_Day_ID = Day_ID;
            ViewBag.RecommendedTrips_Destinations_ID = Destinations_ID;
            ViewBag.F_HashTag_Type = HashTag_Type;
            return View(viewmodel);
        }
        #endregion
        #region Show_list
        public ActionResult show_list(RecommendSearchViewmodel recommendSearch)
        {
            var mode = new List<RecommendedSearchModel>();
            mode = db.RecommendedTrips.Where(p=>(p.RecommendedTrips_StarDay==null && p.RecommendedTrips_EndDay == null)
            ||((p.RecommendedTrips_StarDay != null && p.RecommendedTrips_EndDay == null)&& DbFunctions.TruncateTime(p.RecommendedTrips_StarDay).Value<=DateTime.Now.Date)
             || ((p.RecommendedTrips_StarDay == null && p.RecommendedTrips_EndDay != null) && DbFunctions.TruncateTime(p.RecommendedTrips_EndDay).Value >= DateTime.Now.Date)
            || ((p.RecommendedTrips_StarDay != null && p.RecommendedTrips_EndDay != null) && DbFunctions.TruncateTime(p.RecommendedTrips_StarDay).Value <= DateTime.Now.Date && DbFunctions.TruncateTime(p.RecommendedTrips_EndDay).Value >= DateTime.Now.Date)).Select(p => new RecommendedSearchModel { RecommendedTrips_ID = p.RecommendedTrips_ID, RecommendedTrips_Title = p.RecommendedTrips_Title, RecommendedTrips_Day_Name = p.RecommendedTrips_Day.RecommendedTrips_Day_Name, RecommendedTrips_Day_ID = p.RecommendedTrips_Day.RecommendedTrips_Day_ID, RecommendedTrips_Destinations_ID = p.RecommendedTrips_Destinations_ID,RecommendedTrips_Index_Content=p.RecommendedTrips_Content,RecommendedTrips_Img=p.RecommendedTrips_Img,RecommendedTrips_Img_Description=p.RecommendedTrips_Img_Description, RecommendedTrips_Img_Img=p.F_Destination_Type.Recommend_Img}).ToList();
            //mode = db.V_RecommendedTripsForWebadmin.GroupBy(o=>o.RecommendedTrips_ID).Select(p=>new RecommendedSearchModel {RecommendedTrips_ID=p.Key, RecommendedTrips_Title=p.FirstOrDefault().RecommendedTrips_Title, RecommendedTrips_Day_Name=p.FirstOrDefault().RecommendedTrips_Day_Name,HashTag_Type_ID=p.FirstOrDefault().HashTag_Type_ID,RecommendedTrips_Day_ID=p.FirstOrDefault().RecommendedTrips_Day_ID,RecommendedTrips_Destinations_ID=p.FirstOrDefault().RecommendedTrips_Destinations_ID}).ToList();
            if (recommendSearch.Day_Id != "-1")
            {
                mode = mode.Where(p => p.RecommendedTrips_Day_ID == Convert.ToInt32(recommendSearch.Day_Id)).ToList();
            }
            if (recommendSearch.Dstination_typ != "-1")
            {
                mode = mode.Where(p => p.RecommendedTrips_Destinations_ID == Convert.ToInt32(recommendSearch.Dstination_typ)).ToList();
            }
            if (recommendSearch.F_HashTag != "-1")
            {
                mode = (from t1 in mode
                        join t2 in db.RecommendedTrips_HashTag_Type on t1.RecommendedTrips_ID equals t2.RecommendedTrips_ID
                        where t2.HashTag_Type_ID == Convert.ToInt32(recommendSearch.F_HashTag)
                        select new RecommendedSearchModel { RecommendedTrips_ID = t1.RecommendedTrips_ID, RecommendedTrips_Title = t1.RecommendedTrips_Title, RecommendedTrips_Day_Name = t1.RecommendedTrips_Day_Name, RecommendedTrips_Day_ID = t1.RecommendedTrips_Day_ID, HashTag_Type_ID = t2.HashTag_Type_ID, RecommendedTrips_Destinations_ID = t1.RecommendedTrips_Destinations_ID,RecommendedTrips_Index_Content=t1.RecommendedTrips_Index_Content, RecommendedTrips_Img = t1.RecommendedTrips_Img , RecommendedTrips_Img_Description = t1.RecommendedTrips_Img_Description, RecommendedTrips_Img_Img = t1.RecommendedTrips_Img_Img }
                        ).ToList();
            }
            ViewBag.HashTage = db.RecommendedTrips_HashTag.ToList();
            ViewBag.count = mode.Count();
            return PartialView(mode);
        }
        #endregion
        #region recommended_Detail
        public ActionResult recommended_Detail(int? langid,int RecommendedTrips_ID=1,int nowpage = 0,int jumpPage=0)
        {
            ViewBag.Unit = db.MessageUnitSettings.Where(p=>p.MainID==-1).Select(p=>new UnitPrint {isPrint=(bool)p.IsPrint,isForward= (bool)p.IsForward,isRSS= (bool)p.IsRSS,isShare= (bool)p.IsShare }).FirstOrDefault();
            #region page action計算
            if (nowpage == 0 && jumpPage != 0)
            {
                nowpage = jumpPage;
            }
            else if (nowpage == 0 && jumpPage == 0)
            {
                nowpage = 1;
            }
            #endregion
            #region 模組物動
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
            #endregion
            var model = db.RecommendedTrips;
            ViewBag.ID = model.Find(RecommendedTrips_ID).RecommendedTrips_ID;
            ViewBag.Title = model.Find(RecommendedTrips_ID).RecommendedTrips_Title;
            ViewBag.day = model.Find(RecommendedTrips_ID).RecommendedTrips_Day.RecommendedTrips_Day_Name;
            ViewBag.content = model.Find(RecommendedTrips_ID).RecommendedTrips_Content;
            ViewBag.location = model.Find(RecommendedTrips_ID).RecommendedTrips_Location;
            ViewBag.HtmlContent= model.Find(RecommendedTrips_ID).RecommendedTrips_HtmContent;
            ViewBag.NowPag = nowpage;
            ViewBag.Recommend_Detail_Img = model.Find(RecommendedTrips_ID).F_Destination_Type.Recommend_Detail_Img;
            ViewBag.LinkUrl = model.Find(RecommendedTrips_ID).RecommendedTrips_LinkUrl;
            ViewBag.LinkUrlDES = model.Find(RecommendedTrips_ID).RecommendedTrips_LinkUrlDesc;
            ViewBag.Upfile = model.Find(RecommendedTrips_ID).RecommendedTrips_UploadFileDesc; ;
            ViewBag.UpfileDES= model.Find(RecommendedTrips_ID).RecommendedTrips_UploadFileName;
            ViewBag.pageprt=Url.Action("Print", "F_Recommendedtrips", new { id = RecommendedTrips_ID });
            return View(viewmodel);
        }
        #endregion
        #region RecommendedTrip_Travel_list
        public ActionResult RecommendedTrip_Travel_list(int id,int page=1)
        {
            double count = (double)db.RecommendedTrip_Travel.Where(p => p.RecommendedTrip_ID == id).Count();
            ViewBag.count = count;
            ViewBag.pageCount = Convert.ToInt16(Math.Ceiling(count / 3));
            var model = db.RecommendedTrip_Travel.Where(p => p.RecommendedTrip_ID == id).OrderBy(p => p.RecommendedTrip_Travel_ID).Skip((page - 1) * 3).Take(3).ToList();
            //var modelindex = model.GroupBy(o => o.RecommendedTrip_Travel_ID).Select((o) => new { o.Key });
            //if (modelindex.FirstOrDefault()!=null)
            //{
            //    ViewBag.star = modelindex.FirstOrDefault().Key;
            //    ViewBag.end = modelindex.LastOrDefault().Key;
            //}
            ViewBag.NowPag = page;
            return PartialView(model);
        }
        #endregion
        #region FileDownLoad
        public ActionResult FileDownLoad(int itemid)
        {
            var model = db.RecommendedTrips.Find(itemid);
            string filepath = model.RecommendedTrips_UploadFilePath;
            string oldfilename = model.RecommendedTrips_UploadFileName;
            var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
            if (uploadfilepath.IsNullorEmpty())
            {
                uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
            }
            if (filepath != "")
            {
                string filename = System.IO.Path.GetFileName(filepath);
                if (string.IsNullOrEmpty(oldfilename)) { oldfilename = filename; }
                Stream iStream = new FileStream(uploadfilepath + filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/octet-stream", oldfilename);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }
        #endregion
        #region Print
        public ActionResult Print(int id = 1)
        {
            var model = db.RecommendedTrips.Find(id);
            return View(model);
        }
        #endregion
    }

}