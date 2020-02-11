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
using Utilities;
using ViewModels;
using System.Configuration;

namespace WebSiteProject.Controllers
{
    public class PageController : AppController
    {
        MasterPageManager _MasterPageManager;
        IModelWebsiteMapManager _IModelWebsiteMapManager;
        IMenuManager _IMenuManager;
        IModelPageEditManager _IModelPageEditManager;
        ISiteLayoutManager _ISiteLayoutManager;
        public PageController()
        {
            _MasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _IModelWebsiteMapManager = serviceinstance.ModelWebsiteMapManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IModelPageEditManager = serviceinstance.ModelPageEditManager;
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
        }

        #region Index
        public ActionResult Index(string itemid, string pageitemid, string mid)
        {
            if (string.IsNullOrEmpty(itemid)){return RedirectToAction("Index", "Home"); }
            int tempid = 0;
            if(int.TryParse(itemid, out tempid) == false)
            {
                return RedirectToAction("Index", "Home");
            }
            var itemmodelList = _IModelPageEditManager.GetModelItem(itemid);
            if (itemmodelList == null || itemmodelList.Count() == 0) { return RedirectToAction("Index", "Home"); }
            var mainmodel = _IModelPageEditManager.Where(new ModelPageEditMain() { ID = int.Parse(itemid) });
            if (mainmodel == null|| mainmodel.Count()==0) { return RedirectToAction("Index", "Home"); }
            var unitmodel = _IModelPageEditManager.GetUnitModel(itemid);
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            PageFrontIndexModel model = new PageFrontIndexModel();
            _MasterPageManager.SetModel<PageFrontIndexModel>(ref model, Device, LangID, mid);
            model.MenuType = string.IsNullOrEmpty(Request.Form["menutype"]) ? "1" : Request.Form["menutype"];
            model.SiteMenuID = string.IsNullOrEmpty(Request.Form["sitemenuid"]) ? "-1" : Request.Form["sitemenuid"]; 
            model.ModelItemList = _IModelPageEditManager.GetSelectList(itemid);
            PageIndexItem itemmodel = new PageIndexItem();        
            model.ItemCnt = itemmodelList.Count();
            ViewBag.pageitemid = "";
            if (itemmodelList.Count() > 0)
            {
                if (pageitemid.IsNullorEmpty())
                {
                    itemmodel = itemmodelList.First();
                    model.PageItemID = itemmodelList.First().ItemID.Value;
                }
                else
                {
                    if (int.TryParse(pageitemid, out tempid) == false)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    model.PageItemID = int.Parse(pageitemid);
                    var tempobj = itemmodelList.Where(v => v.ItemID == int.Parse(pageitemid));
                    if (tempobj.Count() > 0) { itemmodel = tempobj.First(); }
                }
            }
            if (itemmodel.ModelID == null){return RedirectToAction("Index", "Home");}
            if (itemmodel.ItemID == 0){ return RedirectToAction("Index", "Home");}
            if (itemmodel.IsVerift == false) { return RedirectToAction("Index", "Home"); }
            if (menu!=null)
            {
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.Title = menu.MenuName==null?"" : menu.MenuName;
            }
            else
            {
                if (mainmodel.Count() > 0)
                {
                    model.Title = mainmodel.First().Name;
                }
            }
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            if (itemmodel.ImageFileName.IsNullorEmpty() == false)
            {
                itemmodel.ImageFileName = helper.Content("~/UploadImage/PageEdit/" + itemmodel.ImageFileName);
                var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri) { Path = itemmodel.ImageFileName, Query = null, };
                model.FBImage = urlBuilder.ToString();
            }
            else
            {
                //UploadImage/PageEdit
                if (itemmodel.HtmlContent != null && itemmodel.HtmlContent.IndexOf("UploadImage/PageEdit") >= 0)
                {
                    model.FBImage = "";
                }
                else {
                    var ImageFileName = helper.Content("~/img/logo_fb.jpg");
                    var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri) { Path = ImageFileName, Query = null, };
                    model.FBImage = urlBuilder.ToString();
                }
            }
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title, true);
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, model.SiteMenuID);
            model.HtmlContent = itemmodel.HtmlContent==null?"": itemmodel.HtmlContent.Replace("\n", "<br>").Replace("<<br>", "<\n").Replace("><br>", ">\n" );
            var fbtitle = model.HtmlContent.TrimgHtmlTag().Replace("\n", "").Replace("\t", "");
            model.FBTitle = fbtitle.Count()>80? fbtitle.Substring(0, 80) : fbtitle;
            model.MainID = itemid;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ImageFileLocation = itemmodel.ImageFileLocation;
            model.ItemID = itemmodel.ItemID.ToString();
            model.ImageName = itemmodel.ImageFileName;
            model.ImageFileDesc = itemmodel.ImageFileDesc;
            model.IsForward = unitmodel.IsForward;
            model.IsPrint = unitmodel.IsPrint;
            model.IsShare = unitmodel.IsShare;
            model.LinkUrlDesc = itemmodel.LinkUrlDesc == null ? "" : itemmodel.LinkUrlDesc;
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            if (itemmodel.LinkUrl.IsNullorEmpty() == false)
            {
                model.LinkUrl = itemmodel.LinkUrl;
            }
            if (itemmodel.UploadFilePath.IsNullorEmpty() == false)
            {
                model.DownloadID = itemmodel.ItemID.ToString();
                model.DownloadDesc = itemmodel.UploadFileDesc;
            }
            return View(model);
        } 
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string itemid)
        {
            var model = _IModelPageEditManager.GetlPageItem(itemid);
            string filepath = model.UploadFilePath;
            string oldfilename = model.UploadFileName;
            var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
            if (uploadfilepath.IsNullorEmpty())
            {
                uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
            }
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
        public ActionResult Forward(string itemid, string mid,string pageitemid)
        {
            var unitmodel = _IModelPageEditManager.GetUnitModel(itemid);
            var langid = _MasterPageManager.CheckLangID(mid);
            var sitemodel = _ISiteLayoutManager.GetSiteLayout(Device, langid);
            ViewBag.FooterString = sitemodel.FowardHtmlContent;
            ViewBag.LogoUrl = sitemodel.FowardImageUrl;
            if (string.IsNullOrEmpty(mid) == false)
            {
                var menu = _IMenuManager.GetModel(mid);
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                ViewBag.Title = menu.MenuName;
            }
            else
            {
                var mainmodel = _IModelPageEditManager.Where(new ModelPageEditMain() { ID = int.Parse(itemid) });
                if (mainmodel == null) { return RedirectToAction("Index", "Home"); }
                if (mainmodel.Count() > 0)
                {
                    ViewBag.Title = mainmodel.First().Name;
                }
            }
            var hostUrl = string.Format("{0}://{1}",
              Request.Url.Scheme,
              Request.Url.Authority);
            if (string.IsNullOrEmpty(mid) == false && string.IsNullOrEmpty(pageitemid) == false)
            {
                ViewBag.Url = hostUrl + Url.Action("Index", "Page", new { itemid = itemid, mid = mid, pageitemid = pageitemid });
            }
            else if (string.IsNullOrEmpty(mid) == false && string.IsNullOrEmpty(pageitemid))
            {
                ViewBag.Url = hostUrl + Url.Action("Index", "Page", new { itemid = itemid, mid = mid });
            }
            else
            {
                ViewBag.Url = hostUrl + Url.Action("Index", "Page", new { itemid = itemid });
            }

            return View();
        }
        #endregion

        #region Print
        public ActionResult Print(string itemid, string pageitemid, string mid)
        {
            if (string.IsNullOrEmpty(itemid)) { return RedirectToAction("Index", "Home"); }
            int tempid = 0;
            if (int.TryParse(itemid, out tempid) == false)
            {
                return RedirectToAction("Index", "Home");
            }
            var itemmodel = _IModelPageEditManager.GetlPageItem(itemid);
            var mainmodel = _IModelPageEditManager.Where(new ModelPageEditMain() { ID = itemmodel.ModelID.Value });
            if (mainmodel == null || mainmodel.Count() == 0) { return RedirectToAction("Index", "Home"); }
            var unitmodel = _IModelPageEditManager.GetUnitModel(itemid);
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            PageFrontIndexModel model = new PageFrontIndexModel();
            _MasterPageManager.SetModel<PageFrontIndexModel>(ref model, Device, LangID, mid);
            model.MenuType = string.IsNullOrEmpty(Request.Form["menutype"]) ? "1" : Request.Form["menutype"];
            model.SiteMenuID = string.IsNullOrEmpty(Request.Form["sitemenuid"]) ? "-1" : Request.Form["sitemenuid"];
            model.ModelItemList = _IModelPageEditManager.GetSelectList(itemmodel.ModelID.Value.ToString());
            model.ItemCnt = 1;
            ViewBag.pageitemid = "";
            
            if (itemmodel.ModelID == null) { return RedirectToAction("Index", "Home"); }
            if (itemmodel.ItemID == 0) { return RedirectToAction("Index", "Home"); }
            if (menu != null)
            {
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.Title = menu.MenuName == null ? "" : menu.MenuName;
            }
            else
            {
                if (mainmodel.Count() > 0)
                {
                    model.Title = mainmodel.First().Name;
                }
            }
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title, true);
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, model.SiteMenuID);
            model.HtmlContent = itemmodel.HtmlContent == null ? "" : itemmodel.HtmlContent.Replace("\n", "<br>").Replace("<<br>", "<\n").Replace("><br>", ">\n");
            model.Title = itemmodel.ItemName;
            model.MainID = itemid;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ImageFileLocation = itemmodel.ImageFileLocation;
            model.ImageName = itemmodel.ImageFileName;
            model.ImageFileDesc = itemmodel.ImageFileDesc;
            model.IsForward = unitmodel.IsForward;
            model.IsPrint = unitmodel.IsPrint;
            model.IsShare = unitmodel.IsShare;
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            if (itemmodel.LinkUrl.IsNullorEmpty() == false)
            {
                model.LinkUrl = itemmodel.LinkUrl;
            }
            if (itemmodel.UploadFilePath.IsNullorEmpty() == false)
            {
                model.DownloadID = itemmodel.ItemID.ToString();
                model.DownloadDesc = itemmodel.UploadFileDesc;
            }
            return View(model);
        }
        #endregion
    }
}