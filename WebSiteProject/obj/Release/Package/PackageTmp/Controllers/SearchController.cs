using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using ViewModels;

namespace WebSiteProject.Controllers
{
    public class SearchController : AppController
    {
        FirstPageManager _IFirstPageManager;
        IModelManager _IModelManager;
        ISearchManager _SearchManager;
        IMenuManager _IMenuManager;
        public SearchController()
        {
            _IFirstPageManager = new FirstPageManager(connectionstr, LangID);
            _IModelManager = new ModelManager(connectionstr);
            _SearchManager=new SearchManager(connectionstr);
            _IMenuManager = new MenuManager(new SQLRepository<Menu>(connectionstr));
        }

        #region Search
        public ActionResult Search(string key, string key2, string key3, string sel1, string sel2, string sellimit
            , string searchtype,string menutype,string menu1, string menu2, string menu3)
        {
            var langid = _IFirstPageManager.CheckLangID("");
            _SearchManager.SetKeyCount(key, langid);
            ViewBag.langid = langid;
            var model = _IFirstPageManager.GetModel(Device, langid);
            if (string.IsNullOrEmpty(key))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Key = key;
            ViewBag.Key2 = key2 == null ? "" : key2;
            ViewBag.Key3 = key3 == null ? "" : key3;
            ViewBag.Sel1 = sel1 == null ? "" : sel1;
            ViewBag.Sel2 = sel2 == null ? "" : sel2;
            ViewBag.SearchType = searchtype == null ? "1" : searchtype;
            //ViewBag.SelLimit = sellimit == null ? "10" : sellimit;

            ViewBag.MenuType = menutype == null ? "" : menutype;
            ViewBag.Menu1 = menu1 == null ? "" : menu1;
            ViewBag.Menu2 = menu2 == null ? "" : menu2;
            ViewBag.Menu3 = menu3 == null ? "" : menu3;
            IModelManager _IModelManager = new ModelManager(connectionstr);
            var indexmodel = _IModelManager.GetPageIndexSettingModel(langid);
            ViewBag.SelLimit = indexmodel.ShowCount;
            ViewBag.ColumnNameMapping = indexmodel.ColumnNameMapping;
            return View(model);
        }
        #endregion

        #region PaddingSearch
        public ActionResult PaddingSearch(AdvanceSearchModel model)
        {
            model.Key2 = model.Key2 == null ? "" : model.Key2;
            model.Key3 = model.Key3 == null ? "" : model.Key3;
            model.Sel1 = model.Sel1 == null ? "" : model.Sel1;
            model.Sel2 = model.Sel2 == null ? "" : model.Sel2;
            model.MenuType = model.MenuType == null ? "" : model.MenuType;
            model.Menu1 = model.Menu1 == null ? "" : model.Menu1;
            model.Menu2 = model.Menu2 == null ? "" : model.Menu2;
            model.Menu3 = model.Menu3 == null ? "" : model.Menu3;
            return Json(_SearchManager.Paging(model));
        }
        #endregion

        #region Index
        public ActionResult Index()
        {
            var langid = _IFirstPageManager.CheckLangID("");
            var model = _IFirstPageManager.GetModel(Device, langid);
            var indexmodel= _IModelManager.GetPageIndexSettingModel(LangID);
            ViewBag.content = indexmodel.HtmlContent;
            ViewBag.ColumnNameMapping = indexmodel.ColumnNameMapping;
            return View(model);
        }
        #endregion

        #region GetMenuTypeList
        public ActionResult GetMenuTypeList(string typeid,string level)
        {
            var langid = _IFirstPageManager.CheckLangID("");
            var data = _IMenuManager.GetMenuTypeList(typeid, langid, level);
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=''>" + Common.GetLangText("不拘") + "</option>");
            var itemarr = new int[] { 0, 1, 2, 3, 4, 7 };
            foreach (var l in data.Where(v=>v.LinkMode <= 2 && itemarr.Contains(v.ModelID) ))
            {
                sb.Append("<option value='" + l.ID.ToString() + "'>" + l.MenuName + "</option>");
            }
            return Json(sb.ToString());
        }
        #endregion

        #region GetMenuIDList
        public ActionResult GetMenuIDList(string menuid)
        {
            var langid = _IFirstPageManager.CheckLangID(menuid);
            var data = _IMenuManager.GetMenuIDList("999");
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=''>"+ Common.GetLangText("不拘") +"</option>");
            var itemarr = new int[] { 0, 1, 2, 3, 4, 7 };
            foreach (var l in data.Where(v => v.LinkMode <=2 && itemarr.Contains(v.ModelID)))
            {
                sb.Append("<option value='" + l.ID.ToString() + "'>" + l.MenuName + "</option>");
            }

            return Json(sb.ToString());
        }
        #endregion
        
    }
}