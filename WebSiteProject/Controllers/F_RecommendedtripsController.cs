using CaptchaMvc.HtmlHelpers;
using NAudio.Lame;
using NAudio.Wave;
using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Speech.Synthesis;
using System.Threading.Tasks;
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
        #region Index
        // GET: F_Recommendedtrips
        public ActionResult Index(int? langid)
        {
            var site_id = 15; //這是Recommendedtrips的輪播ID
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
            ////標題和內容
            //ViewBag.Title = Server.HtmlDecode(db.RecommendedTrips_Index.FirstOrDefault().RecommendedTrips_Index_Title);
            ViewBag.Content = Server.HtmlDecode(db.RecommendedTrips_Index.FirstOrDefault().RecommendedTrips_Index_Content)/*.safeHtmlFragment*/;

            //五大標題+圖
            ViewBag.F_Destination_Type = db.F_Destination_Type.ToList();

            return View(viewmodel);
        }
#endregion
        #region recommended_list
        public ActionResult recommended_list(int? langid, RecommendSearchViewmodel search)
        {
            #region 基本模組
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
            #endregion
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
            if(!search.HashTag.IsNullorEmpty())
            {
                search.Day_Id = "-1";
                search.Dstination_typ = "-1";
                search.F_HashTag = "-1";
                ViewBag.HashTag = search.HashTag.AntiXss();
            }
            else
            {
                ViewBag.HashTag = "";
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
                Destinations_ID.Where(q => q.Value == ViewBag.dstination_typ).First().Selected = true;
                }
             
           if (ViewBag.f_HashTag != "-1")
            {
                HashTag_Type.Where(q => q.Value == ViewBag.f_HashTag).First().Selected = true;
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
            var datetime = DateTime.Now.Date;
            if (!recommendSearch.HashTag.IsNullorEmpty())
            {
                var HasT = recommendSearch.HashTag.ToUpper();
                var Has = db.RecommendedTrips_HashTag.Where(p => p.RecommendedTrips_keyword0.ToUpper() == HasT ||
                                                               p.RecommendedTrips_keyword1.ToUpper() == HasT ||
                                                               p.RecommendedTrips_keyword2.ToUpper() == HasT ||
                                                               p.RecommendedTrips_keyword3.ToUpper() == HasT ||
                                                               p.RecommendedTrips_keyword4.ToUpper() == HasT ||
                                                               p.RecommendedTrips_keyword5.ToUpper() == HasT ||
                                                               p.RecommendedTrips_keyword6.ToUpper() == HasT ||
                                                               p.RecommendedTrips_keyword7.ToUpper() == HasT ||
                                                               p.RecommendedTrips_keyword8.ToUpper() == HasT ||
                                                               p.RecommendedTrips_keyword9.ToUpper() == HasT).Select(p=>p.RecommendedTrips_Id).ToList();
                var Re = db.RecommendedTrips.Where(p => ((bool)p.Enabled == true && p.RecommendedTrips_StarDay == null && p.RecommendedTrips_EndDay == null)
                             || ((p.RecommendedTrips_StarDay != null && p.RecommendedTrips_EndDay == null) && DbFunctions.TruncateTime(p.RecommendedTrips_StarDay) <= datetime)
                              || ((p.RecommendedTrips_StarDay == null && p.RecommendedTrips_EndDay != null) && DbFunctions.TruncateTime(p.RecommendedTrips_EndDay) >= datetime)
                             || ((p.RecommendedTrips_StarDay != null && p.RecommendedTrips_EndDay != null) && DbFunctions.TruncateTime(p.RecommendedTrips_StarDay) <= datetime && DbFunctions.TruncateTime(p.RecommendedTrips_EndDay) >= datetime)).ToList();
                
                foreach(var item in Has)
                {
                    RecommendedSearchModel re = new RecommendedSearchModel();
                    var m = Re.Where(p => p.RecommendedTrips_ID == item).FirstOrDefault();
                    re.RecommendedTrips_ID = m.RecommendedTrips_ID;
                    re.RecommendedTrips_Title = m.RecommendedTrips_Title;
                    re.RecommendedTrips_Day_Name = m.RecommendedTrips_Day.RecommendedTrips_Day_Name;
                   re.RecommendedTrips_Day_ID = m.RecommendedTrips_Day.RecommendedTrips_Day_ID;
                    re.RecommendedTrips_Destinations_ID = m.RecommendedTrips_Destinations_ID;
                   re.RecommendedTrips_Index_Content = m.RecommendedTrips_Content;
                   re.RecommendedTrips_Img = m.RecommendedTrips_Img;
                   re.RecommendedTrips_Img_Description = m.RecommendedTrips_Img_Description;
                    re.RecommendedTrips_Img_Img = m.F_Destination_Type.Recommend_Img;
                    re.sort = m.Sort;
                    mode.Add(re);
                }
            }
            else { 
             mode = db.RecommendedTrips.Where(p => ((bool)p.Enabled == true && p.RecommendedTrips_StarDay == null && p.RecommendedTrips_EndDay == null)
                           || ((p.RecommendedTrips_StarDay != null && p.RecommendedTrips_EndDay == null) && DbFunctions.TruncateTime(p.RecommendedTrips_StarDay) <= datetime)
                            || ((p.RecommendedTrips_StarDay == null && p.RecommendedTrips_EndDay != null) && DbFunctions.TruncateTime(p.RecommendedTrips_EndDay) >= datetime)
                           || ((p.RecommendedTrips_StarDay != null && p.RecommendedTrips_EndDay != null) && DbFunctions.TruncateTime(p.RecommendedTrips_StarDay) <= datetime && DbFunctions.TruncateTime(p.RecommendedTrips_EndDay) >= datetime)).Select(p => new RecommendedSearchModel { RecommendedTrips_ID = p.RecommendedTrips_ID, RecommendedTrips_Title = p.RecommendedTrips_Title, RecommendedTrips_Day_Name = p.RecommendedTrips_Day.RecommendedTrips_Day_Name, RecommendedTrips_Day_ID = p.RecommendedTrips_Day.RecommendedTrips_Day_ID, RecommendedTrips_Destinations_ID = p.RecommendedTrips_Destinations_ID, RecommendedTrips_Index_Content = p.RecommendedTrips_Content, RecommendedTrips_Img = p.RecommendedTrips_Img, RecommendedTrips_Img_Description = p.RecommendedTrips_Img_Description, RecommendedTrips_Img_Img = p.F_Destination_Type.Recommend_Img,sort=p.Sort }).ToList();
            }
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
                        select new RecommendedSearchModel { RecommendedTrips_ID = t1.RecommendedTrips_ID, RecommendedTrips_Title = t1.RecommendedTrips_Title, RecommendedTrips_Day_Name = t1.RecommendedTrips_Day_Name, RecommendedTrips_Day_ID = t1.RecommendedTrips_Day_ID, HashTag_Type_ID = t2.HashTag_Type_ID, RecommendedTrips_Destinations_ID = t1.RecommendedTrips_Destinations_ID,RecommendedTrips_Index_Content=t1.RecommendedTrips_Index_Content, RecommendedTrips_Img = t1.RecommendedTrips_Img , RecommendedTrips_Img_Description = t1.RecommendedTrips_Img_Description, RecommendedTrips_Img_Img = t1.RecommendedTrips_Img_Img, sort = t1.sort }
                        ).ToList();
            }
            ViewBag.HashTage = db.RecommendedTrips_HashTag.ToList();
            ViewBag.count = mode.Count();
            return PartialView(mode.OrderBy(o=>o.sort));
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
            var site_id = 17; //這是ThingsToDo的輪播ID
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
            var Type_ID = db.ADMains.Where(p => p.Type_ID == RecommendedTrips_ID.ToString()).FirstOrDefault();
            var Type = "main";
            if(Type_ID!=null)
            {
                Type = Type_ID.Type_ID;
            }
            _IMasterPageManager.SetModel<HomeViewModel>(ref viewmodel, "P", langid.ToString(), "");
            viewmodel.SEOScript = _IMasterPageManager.GetSEOData("", "", langid.ToString());
            viewmodel.ADMain = _IMasterPageManager.GetADMain_Article("P", langid.ToString(), site_id, Type);
            viewmodel.ADMobile = _IMasterPageManager.GetADMain_Article("M", langid.ToString(), site_id, Type);
            viewmodel.TrainingSiteData = _ISiteLayoutManager.GetTrainingSiteData(Common.GetLangText("另開新視窗")).AntiXss(new string[] { "class" });
            #endregion
            var model = db.RecommendedTrips.Find(RecommendedTrips_ID);
            if(db.RecommendedTrips.Where(p=>p.RecommendedTrips_ID==RecommendedTrips_ID).FirstOrDefault()==null)
            {
                return RedirectToAction("recommended_list");
            }
            ViewBag.ID = model.RecommendedTrips_ID;
            ViewBag.Title = model.RecommendedTrips_Title;
            ViewBag.day = model.RecommendedTrips_Day.RecommendedTrips_Day_Name;
            //ViewBag.content = model.RecommendedTrips_Content;
            //ViewBag.location = model.RecommendedTrips_Location;
            //ViewBag.HtmlContent= model.RecommendedTrips_HtmContent;
            ViewBag.Remode = model;
            ViewBag.NowPag = nowpage;
            ViewBag.Recommend_Detail_Img = model.F_Destination_Type.Recommend_Detail_Img;
            ViewBag.LinkUrl = model.RecommendedTrips_LinkUrl;
            ViewBag.LinkUrlDES = model.RecommendedTrips_LinkUrlDesc;
            ViewBag.Upfile = model.RecommendedTrips_UploadFileDesc; ;
            ViewBag.UpfileDES= model.RecommendedTrips_UploadFileName;
            ViewBag.pageprt=Url.Action("Print", "F_Recommendedtrips", new { id = RecommendedTrips_ID });
        //    var hostUrl = string.Format("{0}://{1}",
        //Request.Url.Scheme,
        //Request.Url.Authority);
        //    ViewBag.url = hostUrl + Url.Action("recommended_Detail", "F_Recommendedtrips", new { RecommendedTrips_ID = RecommendedTrips_ID });
            return View(viewmodel);
        }
        #endregion
        #region RecommendedTrip_Travel_list
        public ActionResult RecommendedTrip_Travel_list(int id,int page=1)
        {
            double count = (double)db.RecommendedTrip_Travel.Where(p => p.RecommendedTrip_ID == id).Count();
            ViewBag.count = count;
            ViewBag.pageCount = Convert.ToInt16(Math.Ceiling(count / 3));
            ViewBag.NowPag = page;
            var model = db.RecommendedTrip_Travel.Where(p => p.RecommendedTrip_ID == id).OrderBy(p => p.RecommendedTrip_Travel_ID).Skip((page - 1) * 3).Take(3).ToList();
            //var modelindex = model.GroupBy(o => o.RecommendedTrip_Travel_ID).Select((o) => new { o.Key });
            //if (modelindex.FirstOrDefault()!=null)
            //{
            //    ViewBag.star = modelindex.FirstOrDefault().Key;
            //    ViewBag.end = modelindex.LastOrDefault().Key;
            //}
            
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
        #region FoGetCaptvalue
        public ActionResult GetCaptvalue(string token)
        {
            var a = CaptchaMvc.HtmlHelpers.CaptchaHelper.GetCaptchaManager(this);
            var c = a.StorageProvider.Value(token, CaptchaMvc.Interface.TokenType.Validation);
            string speak = "1234";
            if (c != null)
            {
                speak = c.Value.ToString();
            }
            return Content(speak);
        }
        #endregion
        #region Forward_OK
        public ActionResult Forward_OK(int? langid,string check="")
        {
            var site_id = 15; //這是Recommendedtrips的輪播ID
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
            string mess = "";
            if(check=="True")
            {
                mess = "Forward success";
                ViewBag.mess = mess.AntiXss();
            }
            else
            {
                mess = "Forward  failure";
                ViewBag.mess = mess.AntiXss();
            }
            return View(viewmodel);
        }
        #endregion
        #region Forward
        public ActionResult Forward(Forward_model model, string btn = "", string CaptchaInputText = "")
        {
            string ErroMessage = string.Empty;
            if (btn == "")
            {
                //itemid = Server.HtmlEncode(itemid);

                //ViewBag.itemid = itemid;
                //var itemmodel = db.RecommendedTrips.Find(int.Parse(itemid));
                //    if (itemmodel == null) { return RedirectToAction("Index", "Home"); }
                //    if (itemmodel != null)
                //    {
                //        model.Title = itemmodel.RecommendedTrips_Title;
                //    }


                //if(string.IsNullOrEmpty(itemid)==false)
                //{
                //    model.Url =
                //}
                return View(model);
            }
            else
            {
                var info = new Dictionary<string, string>();
                try
                {
                    var echeck = new EmailAddressAttribute();

                    if (model.Sender.IsNullorEmpty())
                    {
                        info.Add("Sender", "");
                        model.btn = "Sender";
                        model.ErroMessage += "Sender" + " required";
                        return View(model);
                    }
                    if (model.SenderEMail.IsNullorEmpty())
                    {
                        info.Add("SenderEMail", "");
                        model.btn = "SenderEMail";
                        model.ErroMessage += "Sender EMail" + " required";
                        return View(model);
                    }
                    else
                    {
                        if (echeck.IsValid(model.SenderEMail) == false)
                        {
                            info.Add("SenderEMailFormat", "");
                            model.btn = "SenderEMail_Erro";
                            model.ErroMessage += "Sender EMail" + " formal erro";
                            return View(model);
                        }
                    }
                    if (model.ForwardEMail.IsNullorEmpty())
                    {
                        info.Add("ForwardEMail", "");
                        model.btn = "ForwardEMail";
                        model.ErroMessage += "Forward EMail" + " required";
                        return View(model);
                    }
                    else
                    {
                        var fsplit = model.ForwardEMail.Split(';');
                        foreach (var v in fsplit)
                        {
                            if (echeck.IsValid(v) == false)
                            {
                                if (v == "") { continue; }
                                info.Add("ForwardEMailFormat", "");
                                model.btn = "ForwardEMail_Erro";
                                model.ErroMessage += "Forward EMail" + " formal erro";
                                return View(model);
                            }
                        }
                    }
                    if (CaptchaInputText.IsNullorEmpty())
                    {
                        info.Add("Captcha", "");
                        model.btn = "Captcha";

                        return View(model);
                    }
                    if (!this.IsCaptchaValid("驗證失敗!"))
                    {
                        info.Add("Captcha_v", "");
                        model.btn = "Captcha_v";

                        return View(model);
                    }
                    if (info.Count() == 0)
                    {
                        var host = System.Web.Configuration.WebConfigurationManager.AppSettings["smtphost"];
                        var mailfrom = System.Web.Configuration.WebConfigurationManager.AppSettings["mailfrom"];
                        var NoticeSenderEMail = mailfrom;
                        var NoticeSubject = model.Title;
                        var slist = model.ForwardEMail.Split(';');
                        MailMessage message = new MailMessage();
                        message.From = new MailAddress(model.SenderEMail, model.Sender);
                        foreach (var sender in slist)
                        {
                            message.To.Add(new MailAddress(sender));
                        }
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        message.Subject = NoticeSubject;
                        message.BodyEncoding = System.Text.Encoding.UTF8;
                        string body = model.Sender + Common.GetLangText("寄了一則訊息給你喔") + "<br/> " + Common.GetLangText("給您的訊息") + ":" + model.ForwardMessage +
                            "<br/>" + model.Url;
                        message.Body = body;
                        message.IsBodyHtml = true;
                        message.Priority = MailPriority.High;
                        var ur = System.Web.Configuration.WebConfigurationManager.AppSettings["mailuser"];
                        var pw = System.Web.Configuration.WebConfigurationManager.AppSettings["mailpassword"];
                        var port = System.Web.Configuration.WebConfigurationManager.AppSettings["mailport"];
                        if (string.IsNullOrEmpty(pw) == false)
                        {
                            SmtpClient client = new SmtpClient(host, int.Parse(port));
                            client.EnableSsl = true;
                            client.Credentials = new NetworkCredential(ur, pw);
                            client.Send(message);
                        }
                        else
                        {
                            SmtpClient client2 = new SmtpClient(host);
                            client2.Send(message);
                        }
                        info.Add("result", "ok");
                        return RedirectToAction("Forward_OK", "F_Recommendedtrips", new { check = "True" });
                    }
                    else
                    {
                        info.Add("result", "error");
                        return RedirectToAction("Forward_OK", "F_Recommendedtrips", new { check = "False" });
                    }
                }
                catch (Exception ex)
                {
                    info.Add("result", "exception");
                    info.Add("errorinfo", "寄信失敗:" + ex.Message);
                    return RedirectToAction("Forward_OK", "F_Recommendedtrips", new { check = "False" });
                }
                return Json("send_error");
            }
        }
        #endregion
        public async Task<ActionResult> PlayVoice(string token)
        {
            try
            {
                var a = CaptchaMvc.HtmlHelpers.CaptchaHelper.GetCaptchaManager(this);
                var c = a.StorageProvider.Value(token, CaptchaMvc.Interface.TokenType.Validation);

                string speak = "1234";
                if (c != null)
                {
                    speak = c.Value.ToString();
                }

                logger.Debug("1.播放聲音，播放數字：" + speak);

                Task<FileContentResult> task = Task.Run(() =>
                {
                using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
                {
                        //CultureInfo keyboardCulture = System.Windows.Forms.InputLanguage.CurrentInputLanguage.Culture;
                        //InstalledVoice neededVoice = speechSynthesizer.GetInstalledVoices(keyboardCulture).FirstOrDefault();
                       //var check=speechSynthesizer.GetInstalledVoices().Where(v => v.VoiceInfo.Name == "Microsoft Zira Desktop").FirstOrDefault();
                       // if(check!=null)
                       // {
                       //     speechSynthesizer.SelectVoice("Microsoft Zira Desktop");
                       // }
                        MemoryStream stream = new MemoryStream();

                        speechSynthesizer.SetOutputToWaveStream(stream);
                        
                        var textarr = speak.ToArray();

                        foreach (var t in textarr)
                        {
                            speechSynthesizer.Speak(t.ToString());
                        }

                        var bytes = stream.GetBuffer();
                        var mp3bytes = ConvertWavStreamToMp3File(ref stream, Server.MapPath("//UploadImage/fileName.mp3"));

                        return File(mp3bytes, "audio/mpeg");

                    }
                });

                return await task;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "播放聲音異常，error:" + ex.ToString());
                throw ex;
            }

        }

        private byte[] ConvertWavStreamToMp3File(ref MemoryStream ms, string savetofilename)
        {
            CheckAddBinPath();
            ms.Seek(0, SeekOrigin.Begin);
            MemoryStream msmp3 = new MemoryStream();
            using (var retMs = new MemoryStream())
            using (var rdr = new WaveFileReader(ms))
            using (var wtr = new LameMP3FileWriter(msmp3, rdr.WaveFormat, LAMEPreset.VBR_90))
            {
                rdr.CopyTo(wtr);
            }
            return msmp3.ToArray();
        }

        public void CheckAddBinPath()
        {
            var binPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "bin" });
            var path = Environment.GetEnvironmentVariable("PATH") ?? "";
            if (!path.Split(Path.PathSeparator).Contains(binPath, StringComparer.CurrentCultureIgnoreCase))
            {
                path = string.Join(Path.PathSeparator.ToString(), new string[] { path, binPath });
                Environment.SetEnvironmentVariable("PATH", path);
            }
        }
    }

}