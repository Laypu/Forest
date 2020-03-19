using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utilities;
using ViewModels;
using WebSiteProject.Code;

namespace WebSiteProject.Controllers
{
    public class FactsController : AppController
    {
        MasterPageManager _IMasterPageManager;
        ILangManager _ILangManager;
        ISiteLayoutManager _ISiteLayoutManager;
        IModelActiveManager _IModelActiveManager;
        IMenuManager _IMenuManager;
        IModelLinkManager _IModelLinkManager;
        ADRightDownManager _ADRightDownManager;

        WebSiteProject.Models.ForestEntities db = new Models.ForestEntities();

        public FactsController()
        {
            _IMasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
            _ILangManager = serviceinstance.LangManager;
            _IModelActiveManager = serviceinstance.ModelActiveManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IModelLinkManager = serviceinstance.ModelLinkManager;
            _ADRightDownManager = new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
        }
        // GET: Facts
        public ActionResult Index(int? langid,int nowpage = 0, int jumpPage = 0)
        {
            var site_id = 14; //這是Recommendedtrips的輪播ID
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
            int ShowCount = (int)db.ActiveUnitSettings.Where(p => p.MainID==8).FirstOrDefault().ShowCount;
            var datetime = DateTime.Now.Date;
            var model=db.ActiveItems.Where(p => (DbFunctions.TruncateTime(p.PublicshDate) <= datetime || (p.StDate == null && p.EdDate == null)
                               || ((p.StDate != null && p.EdDate == null) && DbFunctions.TruncateTime(p.StDate) <= datetime)
                                || ((p.StDate == null && p.EdDate != null) && DbFunctions.TruncateTime(p.StDate) >= datetime)
                               || ((p.StDate != null && p.EdDate != null) && DbFunctions.TruncateTime(p.StDate) <= datetime && DbFunctions.TruncateTime(p.EdDate) >= datetime))
                                && p.Enabled == true);
            double count = (double)model.Count();
            ViewBag.count = count;
            ViewBag.pageCount = Convert.ToInt32(Math.Ceiling(count / ShowCount));
            ViewBag.NowPag = nowpage;
            ViewBag.ShowCount = ShowCount;
            ViewBag.Facts = model.OrderBy(p => p.Sort).Skip((nowpage - 1) * ShowCount).Take(ShowCount).ToList();
            TempData["Page"] = nowpage;
            return View(viewmodel);
        }
        public ActionResult Fact_Detail(int? langid,string ItemID,string ModelID)
        {
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
            #endregion
            ItemID = Server.HtmlEncode(ItemID);
            ModelID = Server.HtmlEncode(ModelID);
            var id = Int32.Parse(ItemID);
            var mid = Int32.Parse(ModelID);
            var mode = db.ActiveItems.Where(p => p.ItemID == id && p.ModelID == mid).FirstOrDefault();
            if(mode==null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.ActiveItemDetail = mode;
            ViewBag.Unit = db.ActiveUnitSettings.Where(p => p.MainID == mode.ModelID).Select(p => new Models.F_ViewModels.UnitPrint { isPrint = (bool)p.IsPrint, isForward = (bool)p.IsForward, isRSS = (bool)p.IsRSS, isShare = (bool)p.IsShare }).FirstOrDefault();
            return View(viewmodel);
        }
        #region Print
        public ActionResult Print(string ItemID, string ModelID)
        {
            ItemID = Server.HtmlEncode(ItemID);
            ModelID = Server.HtmlEncode(ModelID);
            var id = Int32.Parse(ItemID);
            var mid = Int32.Parse(ModelID);
            var mode = db.ActiveItems.Where(p => p.ItemID == id && p.ModelID == mid).FirstOrDefault();
            if (mode == null)
            {
                return RedirectToAction("Index");
            }
            return View(mode);
        }
        #endregion
        #region FileDownLoad
        public ActionResult FileDownLoad(string itemid)
        {
            var model = _IModelActiveManager.GetModelItem(itemid);
            if (model.Enabled == false) { return RedirectToAction("Index", "Home"); }
            var isusedate = (model.StDate == null || model.StDate <= DateTime.Now) && (model.EdDate == null || model.EdDate.Value.AddDays(1) >= DateTime.Now);
            if (isusedate == false) { return RedirectToAction("Index", "Home"); }
            string filepath = model.UploadFilePath;
            string oldfilename = model.UploadFileName;
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
    }
}