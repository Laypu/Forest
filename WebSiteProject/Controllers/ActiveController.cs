﻿using Services.Interface;
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
    public class ActiveController : AppController
    {
        MasterPageManager _MasterPageManager;
        IModelWebsiteMapManager _IModelWebsiteMapManager;
        IMenuManager _IMenuManager;
        IModelActiveManager _IModelActiveManager;
        ISiteLayoutManager _ISiteLayoutManager;
        public ActiveController()
        {
            _MasterPageManager = new MasterPageManager(connectionstr, LangID,Common.GetLangDict());
            _IModelWebsiteMapManager = serviceinstance.ModelWebsiteMapManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IModelActiveManager = serviceinstance.ModelActiveManager; 
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
        }

        #region Index
        [NoCacheAttribute]
        public ActionResult Index(string itemid, string mid)
        {
            if (Session["ActiveIndexNoJsSearch"] != null) {Session.Remove("ActiveIndexNoJsSearch"); }
            
            if (string.IsNullOrEmpty(itemid))
            {
                return RedirectToAction("Index", "Home");
            }
            if (this.IsNojavascript) { return RedirectToAction("IndexNoJs",new { itemid=itemid, mid= mid }); }
            ViewBag.nowpage = "1";
            ViewBag.groupid = "";
            if (Session["messagemodelid"] != null) {
                if (Session["messagemodelid"].ToString() == itemid) {
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
            Session["messagepage"] = null;
            Session["messagegroup"] = null;
            Session["messagemodelid"] = null;
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }

            var unitmodel = _IModelActiveManager.GetUnitModel(itemid);
            ViewBag.ColumnNameMapping = unitmodel.ColumnNameMapping;
            ViewBag.ColumnSetting = unitmodel.UnitSettingColumnList;
            ActiveFrontIndexModel model = new ActiveFrontIndexModel();
            _MasterPageManager.SetModel<ActiveFrontIndexModel>(ref model, Device, LangID, mid);
            var mainmodel = _IModelActiveManager.Where(new ModelActiveEditMain() { ID = int.Parse(itemid) });
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
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.GroupList= _IModelActiveManager.GetGroupSelectList(itemid);
            model.Hasgroup= model.GroupList.Count() == 1 ? false : true;
            model.GroupList.First().Text = Common.GetLangText("全部");
            if (model.Hasgroup) {
                model.GroupList.Insert(1, new System.Web.Mvc.SelectListItem() { Text = Common.GetLangText("無分類"), Value = "0" });
            }
            model.MainID = itemid;
            model.MenuID= string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MaxTableCount = unitmodel.ShowCount.ToString();
 
            return View(model);
        }
        #endregion

        #region IndexNoJs
        [NoCacheAttribute]
        public ActionResult IndexNoJs(string itemid, string mid)
        {
            var page_list = Request.Form["page_list"].IsNullorEmpty() ? "" : Request.Form["page_list"];
            var pindex = Request.Form["pindex"].IsNullorEmpty() ? "1" : Request.Form["pindex"];
            var maxpage = Request.Form["maxpage"].IsNullorEmpty() ? "999" : Request.Form["maxpage"];
            var GroupId = "";
            var DisplayFrom ="";
            var DisplayTo ="";
            var keyword = "";
            if (Session["ActiveIndexNoJsSearch"] != null) {
                var dict = (Dictionary<string, string>)Session["ActiveIndexNoJsSearch"];
                if (dict != null)
                {
                    GroupId = dict.ContainsKey("GroupId") ? dict["GroupId"] : "";
                    DisplayFrom = dict.ContainsKey("DisplayFrom") ? dict["DisplayFrom"] : "";
                    DisplayTo = dict.ContainsKey("DisplayTo") ? dict["DisplayTo"] : "";
                    keyword = dict.ContainsKey("GroupId") ? dict["keyword"] : "";
                    if (GroupId.IsNullorEmpty() && DisplayFrom.IsNullorEmpty() && DisplayTo.IsNullorEmpty() && keyword.IsNullorEmpty())
                    {
                        Session.Remove("ActiveIndexNoJsSearch");
                    }
                }

            }
            ViewBag.GroupId = GroupId;
            ViewBag.DisplayFrom = DisplayFrom;
            ViewBag.DisplayTo = DisplayTo;
            ViewBag.keyword = keyword;
            var pagetype = Request.Form["pagetype"].IsNullorEmpty() ? "news_list" : Request.Form["pagetype"];
            ViewBag.pagetype = pagetype.AntiXssEncode();
            int nowpage = 1;
            try {
                nowpage = Request.Form["nowpage"] == null ? 1 : int.Parse(Request.Form["nowpage"]);
            } catch (Exception ex) {
                nowpage = 1;
            }

            if (pindex == "1")
            {
                nowpage = 1;
            }
            else if (pindex == "-1")
            {
                nowpage -= 1;
            }
            else if (pindex == "+1")
            {
                nowpage += 1;
            }
            else {
                try
                {
                    nowpage = int.Parse(pindex);
                }
                catch (Exception ex)
                {
                    nowpage = 1;
                }
            }
            if (page_list != "")
            {
                try
                {
                    nowpage = int.Parse(page_list);
                }
                catch (Exception ex)
                {
                    nowpage = 1;
                }
            }
            if (nowpage <= 0) { nowpage = 1; }
            try
            {
                var maxpagecnt = int.Parse(maxpage);
                if (nowpage > maxpagecnt) { nowpage = maxpagecnt; }
            }
            catch (Exception ex)
            {
                nowpage = 1;
            }

            var page = nowpage;
            //if(pindex)
                ViewBag.nowpage = page;
            if (string.IsNullOrEmpty(itemid))
            {
                return RedirectToAction("Index", "Home");
            }

            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }

            var unitmodel = _IModelActiveManager.GetUnitModel(itemid);
            ViewBag.ColumnNameMapping = unitmodel.ColumnNameMapping;
            ViewBag.ColumnSetting = unitmodel.UnitSettingColumnList;
            ActiveFrontIndexModel model = new ActiveFrontIndexModel();
            _MasterPageManager.SetModel<ActiveFrontIndexModel>(ref model, Device, LangID, mid);
            var mainmodel = _IModelActiveManager.Where(new ModelActiveEditMain() { ID = int.Parse(itemid) });
            var sitemenuid = string.IsNullOrEmpty(Request.Form["sitemenuid"]) ? "-1" : Request.Form["sitemenuid"];

            if (mainmodel.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request.Form["menutype"]) ? "1" : Request.Form["menutype"];

            if (menu != null)
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
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.GroupList = _IModelActiveManager.GetGroupSelectList(itemid);
            model.Hasgroup = model.GroupList.Count() == 1 ? false : true;
            model.GroupList.First().Text = Common.GetLangText("全部");
            if (model.Hasgroup)
            {
                model.GroupList.Insert(1, new System.Web.Mvc.SelectListItem() { Text = Common.GetLangText("無分類"), Value = "0" });
            }
            model.MainID = itemid;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MaxTableCount = unitmodel.ShowCount.ToString();
            var limit = unitmodel.ShowCount == null ? 12 : unitmodel.ShowCount.Value;
            var offset = ((page - 1) * limit);
            var searchmodel = new ActiveSearchModel()
            { 
                 Sort= "Sort",
                 Search="Y",
                 Limit = unitmodel.ShowCount==null?12: unitmodel.ShowCount.Value,
                 ModelID= int.Parse(model.MainID),
                 MenuId= model.MenuID,
                 Offset = offset,
                 GroupId= GroupId==""? (int?)null :int.Parse(GroupId),
                 DisplayFrom= DisplayFrom,
                 DisplayTo= DisplayTo,
                 Title=keyword
            };
            var data = _IModelActiveManager.PagingItemForWebSite(searchmodel.ModelID.ToString(), searchmodel, "");
            var sb = new System.Text.StringBuilder();
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            foreach (var _d in data.rows)
            {
                if (_d.Link_Mode == 1)
                {
                    if (searchmodel.MenuId != "-1")
                    {
                        sb.Append("<a href='" + Url.Action("ActiveView", new { id = _d.ItemID, mid = searchmodel.MenuId, page = searchmodel.NowPage, groupid = searchmodel.GroupId }) + "' title='" + _d.Title + "'>");
                    }
                    else
                    {
                        sb.Append("<a href='" + Url.Action("ActiveView", new { id = _d.ItemID, page = searchmodel.NowPage, groupid = searchmodel.GroupId }) + "' title='" + _d.Title + "'> ");
                    }
                }
                else
                {
                    sb.Append("<a href='#' style='cursor: default;pointer-events: none' title='" + _d.Title + "'>");
                }
                sb.Append("<div class='item'>");
                if (_d.RelatceImageFileName != "")
                {
                    sb.Append("<img src = '" + helper.Content("~/UploadImage/ActiveItem/" + _d.RelatceImageFileName) + "'  alt='' align='left' >");
                }
                else
                {
                    sb.Append("<img src = '" + baseimg + "' alt=''>");
                }
                sb.Append("<div class='date'>" + _d.PublicshDate + "</div>");
                if (_d.GroupName.IsNullorEmpty() == false)
                {
                    sb.Append("<div class='class'><span class='top_class'>" + _d.GroupName + "</span></div>");
                }
                sb.Append("<div class='title'>" + _d.Title + "</div>");
                sb.Append("</div></a>");
            }
            decimal pagecnt = -1;
            if (searchmodel.Limit != -1)
            {
                pagecnt = Math.Ceiling((decimal)data.total / (decimal)searchmodel.Limit);
            }
            ViewBag.Html = sb.ToString();

            var endcnt = (searchmodel.Offset + searchmodel.Limit) > data.total ? data.total : (searchmodel.Offset + searchmodel.Limit);
            ViewBag.TotalCntStr = data.total + "， " + Common.GetLangText("顯示") + " : " + (searchmodel.Offset + 1) + "~" + endcnt;
            ViewBag.maxpage = 0;
            if (data.total == 0 || unitmodel.ShowCount == -1) { ViewBag.showpagenum = "N"; }
            else
            {
                ViewBag.maxpage = pagecnt;
            }

            return View(model);
        }
        #endregion

        #region IndexNoJsSearch
        public ActionResult IndexNoJsSearch(string itemid, string mid)
        {
            var GroupId = Request.Form["GroupId"].IsNullorEmpty() ? "" : Request.Form["GroupId"];
            var DisplayFrom = Request.Form["DisplayFrom"].IsNullorEmpty() ? "" : Request.Form["DisplayFrom"];
            var DisplayTo = Request.Form["DisplayTo"].IsNullorEmpty() ? "" : Request.Form["DisplayTo"];
            var keyword = Request.Form["keyword"].IsNullorEmpty() ? "" : Request.Form["keyword"];
            var skey = new Dictionary<string, string>();
            if (GroupId != "" || DisplayFrom != "" || DisplayTo != "" || keyword != "")
            {
                skey.Add("GroupId", GroupId);
                skey.Add("DisplayFrom", DisplayFrom);
                skey.Add("DisplayTo", DisplayTo);
                skey.Add("keyword", keyword);
                Session["ActiveIndexNoJsSearch"] = skey;
            }
            else
            {
                if (Session["ActiveIndexNoJsSearch"] != null) { Session.Remove("ActiveIndexNoJsSearch"); }
            }
            return RedirectToAction("IndexNoJs", "Active", new { itemid = itemid, mid = mid });
        } 
        #endregion


        #region PagingItem
        public ActionResult PagingItem(ActiveSearchModel model)
        {
            var data = _IModelActiveManager.PagingItemForWebSite(model.ModelID.ToString(), model, "");
            var sb = new System.Text.StringBuilder();
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            foreach (var _d in data.rows) {
                if (_d.Link_Mode == 1) {
                    if (model.MenuId != "-1"){
                        sb.Append("<a href='" + Url.Action("ActiveView", new { id= _d .ItemID,mid= model.MenuId,page=model.NowPage, groupid = model.GroupId })+ "' title='"+ _d.Title+"'>"); }else {
                        sb.Append("<a href='" + Url.Action("ActiveView", new { id = _d.ItemID, page = model.NowPage,groupid=model.GroupId }) + "' title='"+ _d.Title +"'> ");}
                }
                else {
                    sb.Append("<a href='#' style='cursor: default;pointer-events: none' title='" + _d.Title+"'>");
                }
                sb.Append("<div class='item'>");
                if (_d.RelatceImageFileName != "")
                {
                    sb.Append("<img src = '" + helper.Content("~/UploadImage/ActiveItem/" + _d.RelatceImageFileName) + "'  alt='' align='left' >");
                    //sb.Append("<img src = '" + helper.Content("~/UploadImage/ActiveItem/" + _d.RelatceImageFileName) + "'  alt='"+ _d.Title +"' align='left' >");
                }
                else {
                   sb.Append("<img src = '" + baseimg + "' alt=''>");
                }
                sb.Append("<div class='date'>" + _d .PublicshDate+ "</div>");
                if (_d.GroupName.IsNullorEmpty() == false) {
                    sb.Append("<div class='class'><span class='top_class'>"+ _d.GroupName +"</span></div>");
                }
                sb.Append("<div class='title'>"+ _d.Title +"</div>");
                sb.Append("</div></a>");
            }
            decimal pagecnt = -1;
            if (model.Limit != -1) {
                 pagecnt = Math.Ceiling((decimal)data.total / (decimal)model.Limit);
            }         
            return Json(new string[] { sb.ToString() , data .total.ToString(), pagecnt .ToString()});
        }
        #endregion

        #region ActiveView
        public ActionResult ActiveView(string id, string mid,string page,string groupid)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Home");
            }
            Session["messagepage"] = page==null?"1": page;
            Session["messagegroup"] = groupid == null ? "" : groupid;
            var itemmodel = _IModelActiveManager.GetModelItem(id);
            if (itemmodel.ItemID == 0){return RedirectToAction("Index", "Home");}
            if (itemmodel.Enabled == false) { return RedirectToAction("Index", "Home"); }
            if(itemmodel.IsVerift==false) { return RedirectToAction("Index", "Home"); }
            Session["messagemodelid"] = itemmodel.ModelID.ToString();

            var isusedate = (itemmodel.StDate == null || itemmodel.StDate <= DateTime.Now) && (itemmodel.EdDate == null || itemmodel.EdDate.Value.AddDays(1) >= DateTime.Now);
            if (isusedate == false) { return RedirectToAction("Index", "Home"); }
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }

            ActiveFrontViewModel model = new ActiveFrontViewModel();
            _MasterPageManager.SetModel<ActiveFrontViewModel>(ref model, Device, LangID, mid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", itemmodel.ModelID.ToString(), id, LangID, itemmodel.Title);
            var unitmodel = _IModelActiveManager.GetUnitModel(itemmodel.ModelID.ToString());
            model.MainID = itemmodel.ModelID.ToString();
            model.ItemID = id;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            var mainmodel = _IModelActiveManager.Where(new ModelActiveEditMain() { ID = itemmodel.ModelID.Value });
            if (menu!=null)
            {
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.MainTitle = menu.MenuName;
            }
            else
            {
                if (mainmodel.Count() > 0){model.MainTitle = mainmodel.First().Name;}
            }

            if (itemmodel.GroupID != null)
            {
                model.GroupID = itemmodel.GroupID.ToString();
                model.GroupName = _IModelActiveManager.GetGroupName(itemmodel.GroupID.ToString());
            }
            model.SiteMenuID = string.IsNullOrEmpty(Request.Form["sitemenuid"]) ? "-1" : Request.Form["sitemenuid"];
            model.LinkStr = _MasterPageManager.GetFrontLinkString(id, mid, mainmodel.First().Name, model.SiteMenuID);
            model.Content = itemmodel.HtmlContent==null?"":itemmodel.HtmlContent.Replace("\n", "<br>").Replace("<<br>", "<\n").Replace("><br>", ">\n");
            model.IsForward = unitmodel.IsForward;
            model.IsPrint = unitmodel.IsPrint;
            model.IsShare = unitmodel.IsShare;
            model.ImageName = itemmodel.ImageFileName;
            model.ImageFileLocation = itemmodel.ImageFileLocation;
            model.LinkUrlDesc = itemmodel.LinkUrlDesc == null ? "" : itemmodel.LinkUrlDesc;
            model.PublicshDate = itemmodel.PublicshDate.Value.ToString("yyyy.MM.dd");
            model.ImageFileDesc = itemmodel.ImageFileDesc;
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            if (itemmodel.RelateImageFileName.IsNullorEmpty() == false)
            {
                itemmodel.RelateImageFileName = helper.Content("~/UploadImage/MessageItem/" + itemmodel.RelateImageFileName);
                var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri) { Path = itemmodel.RelateImageFileName, Query = null, };
                model.FBImage = urlBuilder.ToString();
            }
            else
            {
                if (itemmodel.HtmlContent!=null&& itemmodel.HtmlContent.IndexOf("UploadImage/MessageItem") >= 0)
                {
                    model.FBImage = "";
                }
                else
                {
                    var ImageFileName = helper.Content("~/img/logo_fb.jpg");
                    var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri) { Path = ImageFileName, Query = null, };
                    model.FBImage = urlBuilder.ToString();
                }
            }

            var fbtitle = model.Content.TrimgHtmlTag().Replace("\n", "").Replace("\t", "");
            model.FBTitle = fbtitle.Count() > 80 ? fbtitle.Substring(0, 80) : fbtitle;


            model.Title = itemmodel.Title;
            if (itemmodel.LinkUrl.IsNullorEmpty() == false){ model.LinkUrl = itemmodel.LinkUrl;}
            if (itemmodel.UploadFilePath.IsNullorEmpty() == false){
                model.DownloadID = itemmodel.ItemID.ToString();
                model.DownloadDesc = itemmodel.UploadFileDesc;
            }
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MenuType = string.IsNullOrEmpty(Request.Form["menutype"]) ? "1" : Request.Form["menutype"];
          var activerange= _IModelActiveManager.GetModelDataRange(itemmodel.ItemID.ToString());
            if (activerange.Count() > 0) {
                var slist = new List<string>();
                foreach (var rg in activerange) {
                    slist.Add(rg.StartDate + " ~ " + rg.EndDate);
                }
                model.ActiveRange = string.Join("、", slist.ToArray()); 
            }
            return View(model);
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
                Stream iStream = new FileStream(uploadfilepath+filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/octet-stream", oldfilename);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        #endregion

        #region Print
        public ActionResult Print(string id, string mid)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Home");
            }
            var itemmodel = _IModelActiveManager.GetModelItem(id);
            if (itemmodel.ItemID == 0) { return RedirectToAction("Index", "Home"); }
            if (itemmodel.Enabled == false) { return RedirectToAction("Index", "Home"); }
            Session["messagemodelid"] = itemmodel.ModelID.ToString();

            var isusedate = (itemmodel.StDate == null || itemmodel.StDate <= DateTime.Now) && (itemmodel.EdDate == null || itemmodel.EdDate.Value.AddDays(1) >= DateTime.Now);
            if (isusedate == false) { return RedirectToAction("Index", "Home"); }
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }

            ActiveFrontViewModel model = new ActiveFrontViewModel();
            _MasterPageManager.SetModel<ActiveFrontViewModel>(ref model, Device, LangID, mid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", itemmodel.ModelID.ToString(), id, LangID, itemmodel.Title);
            var unitmodel = _IModelActiveManager.GetUnitModel(itemmodel.ModelID.ToString());
            model.MainID = itemmodel.ModelID.ToString();
            model.ItemID = id;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            var mainmodel = _IModelActiveManager.Where(new ModelActiveEditMain() { ID = itemmodel.ModelID.Value });
            if (menu != null)
            {
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.MainTitle = menu.MenuName;
            }
            else
            {
                if (mainmodel.Count() > 0) { model.MainTitle = mainmodel.First().Name; }
            }
            if (itemmodel.GroupID != null)
            {
                model.GroupID = itemmodel.GroupID.ToString();
                model.GroupName = _IModelActiveManager.GetGroupName(itemmodel.GroupID.ToString());
            }
            model.SiteMenuID = string.IsNullOrEmpty(Request.Form["sitemenuid"]) ? "-1" : Request.Form["sitemenuid"];
            model.LinkStr = _MasterPageManager.GetFrontLinkString(id, mid, mainmodel.First().Name, model.SiteMenuID);
            model.Content = itemmodel.HtmlContent == null ? "" : itemmodel.HtmlContent.Replace("\n", "<br>").Replace("<<br>", "<\n").Replace("><br>", ">\n");
            model.ImageName = itemmodel.ImageFileName;
            model.ImageFileLocation = itemmodel.ImageFileLocation;
            model.PublicshDate = itemmodel.PublicshDate.Value.ToString("yyyy.MM.dd");
            model.ImageFileDesc = itemmodel.ImageFileDesc;
            model.Title = itemmodel.Title;
            if (itemmodel.LinkUrl.IsNullorEmpty() == false) { model.LinkUrl = itemmodel.LinkUrl; }
            if (itemmodel.UploadFilePath.IsNullorEmpty() == false)
            {
                model.DownloadID = itemmodel.ItemID.ToString();
                model.DownloadDesc = itemmodel.UploadFileDesc;
            }
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MenuType = string.IsNullOrEmpty(Request.Form["menutype"]) ? "1" : Request.Form["menutype"];
            var activerange = _IModelActiveManager.GetModelDataRange(itemmodel.ItemID.ToString());
            if (activerange.Count() > 0)
            {
                var slist = new List<string>();
                foreach (var rg in activerange)
                {
                    slist.Add(rg.StartDate + " ~ " + rg.EndDate);
                }
                model.ActiveRange = string.Join("、", slist.ToArray());
            }
            return View(model);
        }
        #endregion
    }
}