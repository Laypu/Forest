using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using WebSiteProject.Code;

namespace WebSiteProject.Controllers
{
    public class FactsController : AppController
    {
        MasterPageManager _IMasterPageManager;
        ILangManager _ILangManager;
        ISiteLayoutManager _ISiteLayoutManager;
        ILoginManager _ILoginManager;
        IMenuManager _IMenuManager;
        IModelLinkManager _IModelLinkManager;
        ADRightDownManager _ADRightDownManager;

        WebSiteProject.Models.ForestEntities db = new Models.ForestEntities();

        public FactsController()
        {
            _IMasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
            _ILangManager = serviceinstance.LangManager;
            _ILoginManager = serviceinstance.LoginManager;
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
            int ShowCount = (int)db.ActiveUnitSettings.Where(p => p.ID == 6).FirstOrDefault().ShowCount;
            var datetime = DateTime.Now.Date;
            var model=db.ActiveItems.Where(p => DbFunctions.TruncateTime(p.PublicshDate) >= datetime && (p.StDate == null && p.EdDate == null)
                               || ((p.StDate != null && p.EdDate == null) && DbFunctions.TruncateTime(p.StDate) <= datetime)
                                || ((p.StDate == null && p.EdDate != null) && DbFunctions.TruncateTime(p.StDate) >= datetime)
                               || ((p.StDate != null && p.EdDate != null) && DbFunctions.TruncateTime(p.StDate) <= datetime && DbFunctions.TruncateTime(p.EdDate) >= datetime)
                                && p.Enabled == true).OrderBy(p=>p.Sort).Skip((nowpage - 1) * ShowCount).Take(ShowCount).ToList();
            double count = (double)model.Count();
            ViewBag.count = count;
            ViewBag.pageCount = Convert.ToInt16(Math.Ceiling(count / 3));
            ViewBag.NowPag = nowpage;
            ViewBag.ShowCount = ShowCount;
            ViewBag.Facts = model;

            return View(viewmodel);
        }
    }
}