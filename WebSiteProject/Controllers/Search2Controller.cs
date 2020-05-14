using Services.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using WebSiteProject.Code;
using WebSiteProject.Models.F_ViewModels;

namespace WebSiteProject.Controllers
{
    public class Search2Controller : AppController
    {
        MasterPageManager _IMasterPageManager;
        public Search2Controller()
        {
            _IMasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
        }
        WebSiteProject.Models.ForestEntities db = new Models.ForestEntities();
        // GET: Search2
        public ActionResult Search(string Key, int nowpage = 0, int jumpPage = 0)
        {
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
            List<SearChModel> serch = new List<SearChModel>();
            int pageCount = (int)db.PageIndexSettings.Where(o => o.ID == 1).FirstOrDefault().ShowCount;
            var langid = _IMasterPageManager.CheckLangID("");
            //_SearchManager.SetKeyCount(key, langid);
            ViewBag.langid = langid;
            var model = new MasterPageModel();
            _IMasterPageManager.SetModel<MasterPageModel>(ref model, Device, LangID, "");
                if (string.IsNullOrEmpty(Key))
                {
                    return RedirectToAction("Index", "Home");
                }
                var Recom = db.RecommendedTrips.Where(k => k.RecommendedTrips_Title.Contains(Key) || k.RecommendedTrips_HtmContent.Contains(Key)).ToList();
                if(Recom.Count()>0)
                {
             
                   foreach (var item in Recom)
                    {
                        SearChModel sear = new SearChModel();
                        sear.RunPage = "F_Recommendedtrips/recommended_Detail";
                        sear.Itemid = item.RecommendedTrips_ID;
                        sear.Modelid = 0;
                        sear.Title = item.RecommendedTrips_Title;
                    serch.Add(sear);
                    }
                }
            var Fact = db.ActiveItems.Where(k => k.Title.Contains(Key) || k.HtmlContent.Contains(Key)).ToList();
            if (Fact.Count() > 0)
            {

                foreach (var item in Fact)
                {
                    SearChModel sear = new SearChModel();
                    sear.RunPage = "Facts/Fact_Detail";
                    sear.Itemid = item.ItemID;
                    sear.Modelid = (int)item.ModelID;
                    sear.Title = item.Title;
                    serch.Add(sear);
                }
            }
            var Video = db.VideoItems.Where(k => k.Title.Contains(Key) || k.HtmlContent.Contains(Key)).ToList();
            if (Video.Count() > 0)
            {

                foreach (var item in Video)
                {
                    SearChModel sear = new SearChModel();
                    sear.RunPage = "Home/F_Video_Detail";
                    sear.Itemid = item.ItemID;
                    sear.Modelid = 0;
                    sear.Title = item.Title;
                    serch.Add(sear);
                }
            }
            var Dest = db.MessageItems.Where(k => k.Title.Contains(Key) || k.HtmlContent.Contains(Key)).ToList();
            if (Dest.Count() > 0)
            {

                foreach (var item in Dest)
                {
                    var Message_DesHash = db.Message_DesHash.Where(o => o.MessageItem_ID == item.ItemID).Select(o=>o.Destination_Type_ID).FirstOrDefault();
                    var F_Destination_Type = db.F_Destination_Type.Where(o => o.Destination_Type_ID == Message_DesHash);
                    SearChModel sear = new SearChModel();
                    sear.RunPage = "Destination_Index/Article";
                    sear.Itemid = item.ItemID;
                    sear.Modelid = 0;
                    sear.Cate = F_Destination_Type.First().Destination_Type_Title1+" " + F_Destination_Type.First().Destination_Type_Title2;
                    sear.Title = item.Title;
                    serch.Add(sear);
                }
            }
            var count = (double)serch.Count();
            ViewBag.Key = Key;
            ViewBag.count = count;
            ViewBag.pageCount = Convert.ToInt16(Math.Ceiling(count / pageCount));
            ViewBag.NowPag = nowpage;
            ViewBag.PagTak = pageCount;
            ViewBag.Search = serch.OrderBy(p =>p.RunPage).Skip((nowpage - 1) * pageCount).Take(pageCount);
                model.SEOScript = _IMasterPageManager.GetSEOData("", "", langid, Common.GetLangText("搜尋結果"));
                return View(model);

        }
    }
}