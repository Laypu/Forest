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
using WebSiteProject.Code;
using ViewModels;
using System.ServiceModel.Syndication;
using Utilities;
using System.Configuration;
using static WebSiteProject.FilterConfig;

namespace WebSiteProject.Controllers
{
    public class DownloadController : AppController
    {
        MasterPageManager _MasterPageManager;
        IModelFileDownloadManager _IModelFileDownloadManager;
        IMenuManager _IMenuManager;
        ISiteLayoutManager _ISiteLayoutManager;
        public DownloadController()
        {
            _MasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _IMenuManager = serviceinstance.MenuManager;
            _IModelFileDownloadManager = serviceinstance.ModelFileDownloadManager; 
        }

        #region Index
        [NoCacheAttribute]
        public ActionResult Index(string itemid, string mid)
        {
            if (string.IsNullOrEmpty(itemid))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.nowpage = "1";
            ViewBag.groupid = "";
            if (Session["messagemodelid"] != null)
            {
                if (Session["messagemodelid"].ToString() == itemid)
                {
                    if (Session["messagepage"] != null)
                    {
                        ViewBag.nowpage = Session["messagepage"];
                    }
                    if (Session["messagegroup"] != null)
                    {
                        ViewBag.groupid = Session["messagegroup"];
                    }
                }
            }
            var unitmodel = _IModelFileDownloadManager.GetUnitModel(itemid);

            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            FileDownloadFrontIndexModel model = new FileDownloadFrontIndexModel();
            _MasterPageManager.SetModel<FileDownloadFrontIndexModel>(ref model, Device, LangID, mid);
            var mainmodel = _IModelFileDownloadManager.Where(new ModelFileDownloadMain() { ID = int.Parse(itemid) });
            var sitemenuid = string.IsNullOrEmpty(Request.Form["sitemenuid"]) ? "-1" : Request.Form["sitemenuid"];

            if (mainmodel.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request.Form["menutype"]) ? "1" : Request.Form["menutype"];
          
            if (menu!=null)
            {
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.Title = menu.MenuName;
            }
            else
            {
                if (mainmodel.Count() > 0)
                {
                    model.Title = mainmodel.First().Name;
                }
            }
            model.ColumnNameMapping = unitmodel.ColumnNameMapping;
            model.ColumnSetting = unitmodel.UnitSettingColumnList;
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.GroupList= _IModelFileDownloadManager.GetGroupSelectList(itemid, false);
            model.GroupList.First().Text= Common.GetLangText("全部");
            model.Hasgroup= model.GroupList.Count() == 1 ? false : true;
            if (model.Hasgroup)
            {
                model.GroupList.Insert(1, new System.Web.Mvc.SelectListItem() { Text = Common.GetLangText("無分類"), Value = "0" });
            }
            model.MainID = itemid;
            model.MenuID= string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MaxTableCount = unitmodel.ShowCount.ToString();
            var ColumnSetting = unitmodel.UnitSettingColumnList;
            if (unitmodel.Summary.IsNullorEmpty())
            {
                if (this.LangID == "1")
                {
                    model.tablesummary = "此為" + model.Title+ Common.GetLangText("列表")+ "，";
                    var idx = 0;
                    foreach (var c in ColumnSetting)
                    {
                        if (c.Sellected == 0) { continue; }
                        idx += 1;
                        if (c.Name == "代表圖") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["代表圖"] + "，"); }
                        if (c.Name == "發佈日期") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["發佈日期"] + "，"); }
                        if (c.Name == "標題") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["標題"] + "，"); }
                        if (c.Name == "類別") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["類別"] + "，"); }
                        if (c.Name == "檔案下載") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["檔案下載"] + "，"); }
                    }

                }
                else
                {
                    model.tablesummary = "This is " + model.Title + " " + Common.GetLangText("列表")+ "，";
                    var idx = 0;
                    foreach (var c in ColumnSetting)
                    {
                        if (c.Sellected == 0) { continue; }
                        idx += 1;
                        if (c.Name == "代表圖") { model.tablesummary += ("Column" + idx + " Is " + model.ColumnNameMapping["代表圖"] + "，"); }
                        if (c.Name == "發佈日期") { model.tablesummary += ("Column" + idx + " Is " + model.ColumnNameMapping["發佈日期"] + "，"); }
                        if (c.Name == "標題") { model.tablesummary += ("Column" + idx + " Is " + model.ColumnNameMapping["標題"] + "，"); }
                        if (c.Name == "類別") { model.tablesummary += ("Column" + idx + " Is " + model.ColumnNameMapping["類別"] + "，"); }
                        if (c.Name == "檔案下載") { model.tablesummary += ("Column" + idx + " Is " + model.ColumnNameMapping["檔案下載"] + "，"); }
                    }

                }
                model.tablesummary = model.tablesummary.TrimEnd('，');
            }
            else {
                model.tablesummary = unitmodel.Summary;
            }
            //此為 @Model.Title@Common.GetLangText("列表") , 表格欄位1為 @Model.ColumnNameMapping["代表圖"], 表格欄位2為 @Model.ColumnNameMapping["發佈日期"], 表格欄位3為 @Model.ColumnNameMapping["標題"] , 表格欄位4為 @Model.ColumnNameMapping["類別"], 表格欄位5為 @Model.ColumnNameMapping["檔案下載"]
            return View(model);
        }
        #endregion

        #region PagingItem
        public ActionResult PagingItem(FileDownloadSearchModel model)
        {
            var data = _IModelFileDownloadManager.PagingItemForWebSite(model.ModelID.ToString(), model, "");
            var unitmodel = _IModelFileDownloadManager.GetUnitModel(model.ModelID.ToString());
            var ColumnSetting = unitmodel.UnitSettingColumnList;
            var sb = new System.Text.StringBuilder();
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            foreach (var _d in data.rows) {
                sb.Append("<tr>");
                foreach (var c in ColumnSetting)
                {
                    if (c.Sellected == 0) { continue; }
                    if (c.Name == "代表圖")
                    {
                        sb.Append("<td scope='row' class='text-center'>");
                        if (_d.RelatceImageFileName != "")
                        {
                            sb.Append("<img src = '" + helper.Content("~/UploadImage/FileDownloadItem/" + _d.RelatceImageFileName) + "'  alt='" + _d.Title + "' align='left'></td>");
                        }
                        else
                        {
                            sb.Append("<img src = '" + baseimg + "' alt='" + _d.Title + "'></td>");
                        }
                    }
                    else if (c.Name == "發佈日期")
                    { sb.Append("<td class='text-center'>" + _d.PublicshDate + "</td>"); }
                    else if (c.Name == "標題")
                    { sb.Append("<td>" + _d.Title + "</td>"); }
                    else if (c.Name == "類別")
                    { sb.Append("<td class='text-center'>" + (_d.GroupName == null ? "" : _d.GroupName) + "</td>"); }
                    else if (c.Name == "檔案下載")
                    {
                        if (_d.UploadFilePath.IsNullorEmpty() == false)
                        {
                            sb.Append("<td class='text-center'><a href ='" + Url.Action("FileDownLoad", new { itemid = _d.ItemID }) + "' title='" + Common.GetLangText("檔案下載") +
                                "-" + _d.UploadFileDesc + "(" + Common.GetLangText("另開新視窗") + ")' target='_blank'>");
                            sb.Append("<span class='fa-stack fa-1g' aria-hidden='true'><i class='fa fa-square fa-stack-2x font-blue-steel' aria-hidden='true'></i>" +
                                "<i class='fas fa-download fa-stack-1x' aria-hidden='true'></i></span><span class='sr-only'>"+ Common.GetLangText("檔案下載") +"</span></a></td>");
                        }
                        else
                        {
                            sb.Append("<td class='text-center'></a></td>");
                        }
                    }
                }
                sb.Append("</tr>");
            }
            decimal pagecnt = -1;
            if (model.Limit != -1)
            {
                pagecnt = Math.Ceiling((decimal)data.total / (decimal)model.Limit);
            }
            return Json(new string[] { sb.ToString(), data.total.ToString(), pagecnt.ToString() });
        }
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string itemid)
        {
            var model = _IModelFileDownloadManager.GetModelItem(itemid);
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