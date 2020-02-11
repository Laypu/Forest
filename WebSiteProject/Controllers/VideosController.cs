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
    public class VideosController : AppController
    {
        MasterPageManager _MasterPageManager;
        IModelWebsiteMapManager _IModelWebsiteMapManager;
        IMenuManager _IMenuManager;
        IModelVideoManager _IModelVideoManager;
        ISiteLayoutManager _ISiteLayoutManager;
        public VideosController()
        {
            _MasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _IModelWebsiteMapManager = serviceinstance.ModelWebsiteMapManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IModelVideoManager = serviceinstance.ModelVideoManager; 
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
        }

        #region Index
        [NoCacheAttribute]
        public ActionResult Index(string itemid, string mid)
        {
            if (string.IsNullOrEmpty(itemid))
            {
                return RedirectToAction("Index", "Home");
            }
            if (Session["VideoIndexNoJsSearch"] != null) { Session.Remove("VideoIndexNoJsSearch"); }
            if (this.IsNojavascript) { return RedirectToAction("IndexNoJs", new { itemid = itemid, mid = mid }); }
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
            var unitmodel = _IModelVideoManager.GetUnitModel(itemid);
            ViewBag.ColumnNameMapping = unitmodel.ColumnNameMapping;
            ViewBag.ColumnSetting = unitmodel.UnitSettingColumnList;
            ViewBag.IntroductionHtml = unitmodel.IntroductionHtml;
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            VideoFrontIndexModel  model = new VideoFrontIndexModel();
            _MasterPageManager.SetModel< VideoFrontIndexModel>(ref model, Device, LangID, mid);
            var mainmodel = _IModelVideoManager.Where(new  ModelVideoMain() { ID = int.Parse(itemid) });
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
            model.GroupList= _IModelVideoManager.GetGroupSelectList(itemid);
            model.Hasgroup= model.GroupList.Count() == 1 ? false : true;
            model.GroupList.First().Text = Common.GetLangText("全部");           

            if (model.Hasgroup)
            {
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
            var DisplayFrom = "";
            var DisplayTo = "";
            var keyword = "";
            if (Session["VideoIndexNoJsSearch"] != null)
            {
                var dict = (Dictionary<string, string>)Session["VideoIndexNoJsSearch"];
                if (dict != null)
                {
                    GroupId = dict.ContainsKey("GroupId") ? dict["GroupId"] : "";
                    DisplayFrom = dict.ContainsKey("DisplayFrom") ? dict["DisplayFrom"] : "";
                    DisplayTo = dict.ContainsKey("DisplayTo") ? dict["DisplayTo"] : "";
                    keyword = dict.ContainsKey("GroupId") ? dict["keyword"] : "";
                    if (GroupId.IsNullorEmpty() && DisplayFrom.IsNullorEmpty() && DisplayTo.IsNullorEmpty() && keyword.IsNullorEmpty())
                    {
                        Session.Remove("VideoIndexNoJsSearch");
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
            try
            {
                nowpage = Request.Form["nowpage"] == null ? 1 : int.Parse(Request.Form["nowpage"]);
            }
            catch (Exception ex)
            {
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
            else
            {
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

            var unitmodel = _IModelVideoManager.GetUnitModel(itemid);
            ViewBag.ColumnNameMapping = unitmodel.ColumnNameMapping;
            ViewBag.ColumnSetting = unitmodel.UnitSettingColumnList;
            ViewBag.IntroductionHtml = unitmodel.IntroductionHtml;
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            VideoFrontIndexModel model = new VideoFrontIndexModel();
            _MasterPageManager.SetModel<VideoFrontIndexModel>(ref model, Device, LangID, mid);
            var mainmodel = _IModelVideoManager.Where(new ModelVideoMain() { ID = int.Parse(itemid) });
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
            model.GroupList = _IModelVideoManager.GetGroupSelectList(itemid);
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
            var searchmodel = new VideoSearchModel()
            {
                Sort = "Sort",
                Search = "Y",
                Limit = unitmodel.ShowCount == null ? 12 : unitmodel.ShowCount.Value,
                ModelID = int.Parse(model.MainID),
                MenuId = model.MenuID,
                Offset = offset,
                GroupId = GroupId == "" ? (int?)null : int.Parse(GroupId),
                DisplayFrom = DisplayFrom,
                DisplayTo = DisplayTo,
                Title = keyword
            };
            var data = _IModelVideoManager.PagingItemForWebSite(searchmodel.ModelID.ToString(), searchmodel, "");
            var sb = new System.Text.StringBuilder();
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            foreach (var _d in data.rows)
            {
                if (_d.Link_Mode == 1)
                {
                    if (searchmodel.MenuId != "-1")
                    {
                        sb.Append("<a href='" + Url.Action("VideoView", new { id = _d.ItemID, mid = searchmodel.MenuId, page = searchmodel.NowPage, groupid = searchmodel.GroupId }) + "' title='" + _d.Title + "'>");
                    }
                    else
                    {
                        sb.Append("<a href='" + Url.Action("VideoView", new { id = _d.ItemID, page = searchmodel.NowPage, groupid = searchmodel.GroupId }) + "' title='" + _d.Title + "'> ");
                    }
                }
                else
                {
                    sb.Append("<a href='#' style='cursor: default;pointer-events: none' title='" + _d.Title + "'>");
                }
                sb.Append("<div class='item'>");
                if (_d.RelatceImageFileName.IsNullorEmpty() == false)
                {
                    sb.Append("<img src = '" + helper.Content("~/UploadImage/VideoItem/" + _d.RelatceImageFileName) + "'  alt='' align='left'>");
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
                Session["VideoIndexNoJsSearch"] = skey;
            }
            else
            {
                if (Session["VideoIndexNoJsSearch"] != null) { Session.Remove("VideoIndexNoJsSearch"); }
            }
            return RedirectToAction("IndexNoJs", "Videos", new { itemid = itemid, mid = mid });
        }
        #endregion

        #region PagingItem
        public ActionResult PagingItem(VideoSearchModel model)
        {
            var data = _IModelVideoManager.PagingItemForWebSite(model.ModelID.ToString(), model, "");
            var sb = new System.Text.StringBuilder();
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            foreach (var _d in data.rows) {
                if (_d.Link_Mode == 1) {
                    if (model.MenuId != "-1"){
                        sb.Append("<a href='" + Url.Action("VideoView", new { id= _d .ItemID,mid= model.MenuId,page=model.NowPage, groupid = model.GroupId })+ "' title='"+ _d.Title+"'>"); }else {
                        sb.Append("<a href='" + Url.Action("VideoView", new { id = _d.ItemID, page = model.NowPage,groupid=model.GroupId }) + "' title='"+ _d.Title +"'> ");}
                }
                else {
                    sb.Append("<a href='#' style='cursor: default;pointer-events: none' title='"+ _d.Title+"'>");
                }
                sb.Append("<div class='item'>");
                if (_d.RelatceImageFileName.IsNullorEmpty()==false)
                {
                    // sb.Append("<img src = '" + helper.Content("~/UploadImage/VideoItem/" + _d.RelatceImageFileName) + "'  alt='"+ _d.Title +"' align='left'>");
                    sb.Append("<img src = '" + helper.Content("~/UploadImage/VideoItem/" + _d.RelatceImageFileName) + "'  alt='' align='left'>");
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

        #region VideoView
        public ActionResult VideoView(string id, string mid,string page,string groupid)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Home");
            }
            Session["messagepage"] = page==null?"1": page;
            Session["messagegroup"] = groupid == null ? "" : groupid;
            var itemmodel = _IModelVideoManager.GetModelItem(id);
            if (itemmodel.ItemID == 0){return RedirectToAction("Index", "Home");}
            if (itemmodel.Enabled == false) { return RedirectToAction("Index", "Home"); }
            if (itemmodel.IsVerift == false) { return RedirectToAction("Index", "Home"); }
            Session["messagemodelid"] = itemmodel.ModelID.ToString();

            var isusedate = (itemmodel.StDate == null || itemmodel.StDate <= DateTime.Now) && (itemmodel.EdDate == null || itemmodel.EdDate.Value.AddDays(1) >= DateTime.Now);
            if (isusedate == false) { return RedirectToAction("Index", "Home"); }
            VideoFrontViewModel model = new VideoFrontViewModel();
            _MasterPageManager.SetModel<VideoFrontViewModel>(ref model, Device, LangID, mid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", itemmodel.ModelID.ToString(), id, LangID, itemmodel.Title);
            var unitmodel = _IModelVideoManager.GetUnitModel(itemmodel.ModelID.ToString());
            model.MainID = itemmodel.ModelID.ToString();
            model.ItemID = id;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            var mainmodel = _IModelVideoManager.Where(new ModelVideoMain() { ID = itemmodel.ModelID.Value });
            if (string.IsNullOrEmpty(mid) == false)
            {
                var menu = _IMenuManager.GetModel(mid);
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
                model.GroupName = _IModelVideoManager.GetGroupName(itemmodel.GroupID.ToString());
            }
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            if (itemmodel.RelateImageFileName.IsNullorEmpty() == false)
            {
                itemmodel.RelateImageFileName = helper.Content("~/UploadImage/VideoItem/" + itemmodel.RelateImageFileName);
            }
            else
            {
                itemmodel.RelateImageFileName = baseimg;
            }
            var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri) { Path = itemmodel.RelateImageFileName, Query = null, };
            model.FBImage = urlBuilder.ToString();
            model.SiteMenuID = string.IsNullOrEmpty(Request.Form["sitemenuid"]) ? "-1" : Request.Form["sitemenuid"];
            model.LinkStr = _MasterPageManager.GetFrontLinkString(id, mid, mainmodel.First().Name, model.SiteMenuID);
            model.Content = itemmodel.HtmlContent==null?"":itemmodel.HtmlContent.Replace("\n", "<br>").Replace("<<br>", "<\n").Replace("><br>", ">\n");

            var fbtitle = model.Content.TrimgHtmlTag().Replace("\n", "").Replace("\t", "");
            model.FBTitle = fbtitle.Count() > 80 ? fbtitle.Substring(0, 80) : fbtitle;

            model.IsForward = unitmodel.IsForward;
            model.IsPrint = unitmodel.IsPrint;
            model.IsShare = unitmodel.IsShare;
            model.ImageName = itemmodel.ImageFileName;
            model.ImageFileLocation = itemmodel.ImageFileLocation;
            model.PublicshDate = itemmodel.PublicshDate.Value.ToString("yyyy/MM/dd");
            model.ImageFileDesc = itemmodel.ImageFileDesc;
            model.Title = itemmodel.Title;
            model.VideoLink = itemmodel.VideoLink;
            model.VideoHasMore = itemmodel.IsShowMoreVideo.Value;
            model.LinkUrlDesc = itemmodel.LinkUrlDesc == null ? "" : itemmodel.LinkUrlDesc;
            if (model.VideoHasMore) {
                var vstrarr= _IModelVideoManager.GetVideoMore(model.MainID, model.ItemID,model.MenuID);
                model.VideoMore = vstrarr[0];
                model.VideoMoreNoScript = vstrarr[1];
            }
            if (model.VideoLink.IsNullorEmpty() == false) {
                if (model.VideoLink.IndexOf("embed") != 0)
                {
                    var keyindex = model.VideoLink.IndexOf("watch?v=");
                    var key = "";
                    if (keyindex == -1) { key = model.VideoLink; } else {
                        key = model.VideoLink.Substring(keyindex+8);
                    }
                    model.VideoLink = "https://www.youtube.com/embed/" + key;
                }
            }
               

            if (itemmodel.LinkUrl.IsNullorEmpty() == false){ model.LinkUrl = itemmodel.LinkUrl;}
            if (itemmodel.UploadFilePath.IsNullorEmpty() == false){
                model.DownloadID = itemmodel.ItemID.ToString();
                model.DownloadDesc = itemmodel.UploadFileDesc;
            }
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MenuType = string.IsNullOrEmpty(Request.Form["menutype"]) ? "1" : Request.Form["menutype"];
  
            return View(model);
        }
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string itemid)
        {
            var model = _IModelVideoManager.GetModelItem(itemid);
            if (model.Enabled == false) { return RedirectToAction("Index", "Home"); }
            var isusedate = (model.StDate == null || model.StDate <= DateTime.Now) && (model.EdDate == null || model.EdDate.Value.AddDays(1) >= DateTime.Now);
            if (isusedate == false) { return RedirectToAction("Index", "Home"); }
            string filepath = model.UploadFilePath;
            string oldfilename = model.UploadFileName;
            //var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
            //if (uploadfilepath.IsNullorEmpty())
            //{
            //    uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
            //}
            string uploadfilepath = Server.MapPath("~/UploadFile/");
            if (filepath != "")
            {
                //取得檔案名稱
                string filename = System.IO.Path.GetFileName(filepath);
                if (string.IsNullOrEmpty(oldfilename)) { oldfilename = filename; }
                //讀成串流
                Stream iStream = new FileStream(uploadfilepath+filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                //回傳出檔案
                return File(iStream, "application/octet-stream", oldfilename);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        #endregion

        #region Forward
        public ActionResult Forward(string itemid, string mid, string menutype, string sitemenuid)
        {
            var unitmodel = _IModelVideoManager.GetUnitModel(itemid);
            var sitemodel = _ISiteLayoutManager.GetSiteLayout(Device, "1");
            ViewBag.FooterString = sitemodel.FowardHtmlContent;
            ViewBag.LogoUrl = sitemodel.FowardImageUrl;
            if (string.IsNullOrEmpty(mid) == false && mid!="-1")
            {
                var menu = _IMenuManager.GetModel(mid);
                ViewBag.Title = menu.MenuName;
            }
            else
            {
                var itemmodel = _IModelVideoManager.GetModelItem(itemid);
                if (itemmodel == null) { return RedirectToAction("Index", "Home"); }
                if (itemmodel != null)
                {
                    ViewBag.Title = itemmodel.Title;
                }
            }
            var hostUrl = string.Format("{0}://{1}",
              Request.Url.Scheme,
              Request.Url.Authority);
            if (sitemenuid != "-1")
            {
                ViewBag.Url = hostUrl + Url.Action("VideoView", "Video", new { id = itemid, mid = mid, sitemenuid = sitemenuid, menutype = menutype });
            }
            else if (string.IsNullOrEmpty(mid) == false)
            {
                ViewBag.Url = hostUrl + Url.Action("VideoView", "Video", new { id = itemid, mid = mid });
            }
            else
            {
                ViewBag.Url = hostUrl + Url.Action("VideoView", "Video", new { id = itemid });
            }
            return View();
        }
        #endregion

        #region Print
        public ActionResult Print(string id, string mid)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Home");
            }
            var itemmodel = _IModelVideoManager.GetModelItem(id);
            if (itemmodel.ItemID == 0) { return RedirectToAction("Index", "Home"); }
            if (itemmodel.Enabled == false) { return RedirectToAction("Index", "Home"); }
            if (itemmodel.IsVerift == false) { return RedirectToAction("Index", "Home"); }
            Session["messagemodelid"] = itemmodel.ModelID.ToString();

            var isusedate = (itemmodel.StDate == null || itemmodel.StDate <= DateTime.Now) && (itemmodel.EdDate == null || itemmodel.EdDate.Value.AddDays(1) >= DateTime.Now);
            if (isusedate == false) { return RedirectToAction("Index", "Home"); }
            VideoFrontViewModel model = new VideoFrontViewModel();
            _MasterPageManager.SetModel<VideoFrontViewModel>(ref model, Device, LangID, mid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", itemmodel.ModelID.ToString(), id, LangID, itemmodel.Title);
            var unitmodel = _IModelVideoManager.GetUnitModel(itemmodel.ModelID.ToString());
            model.MainID = itemmodel.ModelID.ToString();
            model.ItemID = id;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            var mainmodel = _IModelVideoManager.Where(new ModelVideoMain() { ID = itemmodel.ModelID.Value });
            if (string.IsNullOrEmpty(mid) == false)
            {
                var menu = _IMenuManager.GetModel(mid);
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
                model.GroupName = _IModelVideoManager.GetGroupName(itemmodel.GroupID.ToString());
            }
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            if (itemmodel.RelateImageFileName.IsNullorEmpty() == false)
            {
                itemmodel.RelateImageFileName = helper.Content("~/UploadImage/VideoItem/" + itemmodel.RelateImageFileName);
            }
            else
            {
                itemmodel.RelateImageFileName = baseimg;
            }
            var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri) { Path = itemmodel.RelateImageFileName, Query = null, };
            model.FBImage = urlBuilder.ToString();
            model.SiteMenuID = string.IsNullOrEmpty(Request.Form["sitemenuid"]) ? "-1" : Request.Form["sitemenuid"];
            model.LinkStr = _MasterPageManager.GetFrontLinkString(id, mid, mainmodel.First().Name, model.SiteMenuID);
            model.Content = itemmodel.HtmlContent == null ? "" : itemmodel.HtmlContent.Replace("\n", "<br>").Replace("<<br>", "<\n").Replace("><br>", ">\n");

            var fbtitle = model.Content.TrimgHtmlTag().Replace("\n", "").Replace("\t", "");
            model.FBTitle = fbtitle.Count() > 80 ? fbtitle.Substring(0, 80) : fbtitle;

            model.IsForward = unitmodel.IsForward;
            model.IsPrint = unitmodel.IsPrint;
            model.IsShare = unitmodel.IsShare;
            model.ImageName = itemmodel.ImageFileName;
            model.ImageFileLocation = itemmodel.ImageFileLocation;
            model.PublicshDate = itemmodel.PublicshDate.Value.ToString("yyyy/MM/dd");
            model.ImageFileDesc = itemmodel.ImageFileDesc;
            model.Title = itemmodel.Title;
            model.VideoLink = itemmodel.VideoLink;
            model.VideoHasMore = itemmodel.IsShowMoreVideo.Value;
            model.LinkUrlDesc = itemmodel.LinkUrlDesc == null ? "" : itemmodel.LinkUrlDesc;
            if (model.VideoHasMore)
            {
                var vstrarr = _IModelVideoManager.GetVideoMore(model.MainID, model.ItemID, model.MenuID);
                model.VideoMore = vstrarr[0];
                model.VideoMoreNoScript = vstrarr[1];
            }
            if (model.VideoLink.IsNullorEmpty() == false)
            {
                if (model.VideoLink.IndexOf("embed") != 0)
                {
                    var keyindex = model.VideoLink.IndexOf("watch?v=");
                    var key = "";
                    if (keyindex == -1) { key = model.VideoLink; }
                    else
                    {
                        key = model.VideoLink.Substring(keyindex + 8);
                    }
                    model.VideoLink = "https://www.youtube.com/embed/" + key;
                }
            }
            if (itemmodel.LinkUrl.IsNullorEmpty() == false) { model.LinkUrl = itemmodel.LinkUrl; }
            if (itemmodel.UploadFilePath.IsNullorEmpty() == false)
            {
                model.DownloadID = itemmodel.ItemID.ToString();
                model.DownloadDesc = itemmodel.UploadFileDesc;
            }
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MenuType = string.IsNullOrEmpty(Request.Form["menutype"]) ? "1" : Request.Form["menutype"];

            return View(model);
        }
        #endregion

        
    }
}