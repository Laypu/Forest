using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using Utilities;
using System.Configuration;
using System.IO;
using WebSiteProject.Code;

namespace WebSiteProject.Areas.webadmin.Controllers
{
    [Authorize]
    public class SiteController : AppController
    {
        ISiteLayoutManager _ISiteLayoutManager;
        IMenuManager _IMenuManager;
        IModelMessageManager _IMessageManager;
        IModelActiveManager _IModelActiveManager;
        IModelLinkManager _IModelLinkManager;
        ISiteConfigManager _ISiteConfigManager;
        readonly SQLRepository<Img> _imgsqlrepository;
        public SiteController()
        {
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IMessageManager = serviceinstance.MessageManager;
            _IModelActiveManager = serviceinstance.ModelActiveManager;
            _IModelLinkManager = serviceinstance.ModelLinkManager;
            _imgsqlrepository = new SQLRepository<Img>(connectionstr);
            _ISiteConfigManager = serviceinstance.SiteConfigManager;
        }
        [AuthoridUrl("Site/PageLayout", "")]
        public ActionResult PageLayout(string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (string.IsNullOrEmpty(stype)) { stype = "P"; ; }
            ViewBag.Stype = stype.AntiXssEncode();
            if (stype == "M")
            {
                ViewBag.Title = "入口首頁配置(手機板)";
            }
            else
            {
                ViewBag.Title = "入口首頁配置";
            }
            return View();
        }


        //[AuthoridUrl("Site/PageLayout", "stype")]
        //public ActionResult PageLayoutEdit(string id,string stype)
        //{
        //    CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
        //    if (string.IsNullOrEmpty(stype)) { stype = "P"; }
        //    if (stype == "P")
        //    {
        //        ViewBag.L1option = _IMenuManager.GetMenuOption(int.Parse(this.LanguageID), 1, -1, 1);
        //    }
        //    else {
        //        ViewBag.L1option = _IMenuManager.GetMenuOption(int.Parse(this.LanguageID), 1, -1, 4);
        //    }

        //    var model = _ISiteLayoutManager.GetModel("1", stype);
        //    var model2 = _ISiteLayoutManager.GetModel("2", stype);
        //    var modellist = new List<PageLayoutModel>() { model, model2 };
        //    return View(modellist);
        //}

        [AuthoridUrl("Site/PageLayout", "")]
        #region PageNewsEdit
        public ActionResult PageNewsEdit(string id, string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (string.IsNullOrEmpty(stype)) { stype = "P"; }
            if (stype == "P")
            {
                ViewBag.L1option = _IMenuManager.GetMenuOption(int.Parse(this.LanguageID), 1, -1, 1, 2);
            }
            else
            {
                ViewBag.L1option = _IMenuManager.GetMenuOption(int.Parse(this.LanguageID), 1, -1, 4, 2);
            }

            var model = _ISiteLayoutManager.GetModel("焦點新聞", stype, this.LanguageID);
            ViewBag.itemlist = model.ModelItemList.IsNullorEmpty() ? "" : model.ModelItemList;
            var modellist = new List<PageLayoutModel>() { model };
            return View(modellist);
        }
        #endregion

        [AuthoridUrl("Site/PageLayout", "")]
        #region PageActiveEdit
        public ActionResult PageActiveEdit(string id, string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (string.IsNullOrEmpty(stype)) { stype = "P"; }
            if (stype == "P")
            {
                ViewBag.L1option = _IMenuManager.GetMenuOption(int.Parse(this.LanguageID), 1, -1, 1, 0);
            }
            else
            {
                ViewBag.L1option = _IMenuManager.GetMenuOption(int.Parse(this.LanguageID), 1, -1, 4, 0);
            }

            var model = _ISiteLayoutManager.GetModel("活動專區", stype, this.LanguageID);
            ViewBag.itemlist = model.ModelItemList.IsNullorEmpty() ? "" : model.ModelItemList;
            var modellist = new List<PageLayoutModel>() { model };
            return View(modellist);
        }
        #endregion

        [AuthoridUrl("Site/PageLayout", "")]
        #region PageLayoutOP1Edit
        public ActionResult PageLayoutOP1Edit()
        {
            var model = _ISiteLayoutManager.GetPageLayoutOP1Edit(this.LanguageID);
            return View(model);
        }
        #endregion

        [AuthoridUrl("Site/PageLayout", "")]
        #region PageLayoutOP2Edit
        public ActionResult PageLayoutOP2Edit()
        {
            var model = _ISiteLayoutManager.GetPageLayoutOP2Edit(this.LanguageID);
            return View(model);
        }
        #endregion

        [AuthoridUrl("Site/PageLayout", "")]
        #region PageLayoutOP3Edit
        public ActionResult PageLayoutOP3Edit()
        {
            var model = _ISiteLayoutManager.GetPageLayoutOP3Edit(this.LanguageID);
            return View(model);
        }
        #endregion

        [AuthoridUrl("Site/PageLayout", "")]
        #region PageActivity
        public ActionResult PageActivity()
        {
            var model = _ISiteLayoutManager.PageLayoutActivity(this.LanguageID);
            return View(model);
        }
        #endregion


        [AuthoridUrl("Site/PageLayout", "")]
        #region PageLayoutLink
        public ActionResult PageLayoutLink()
        {
            return View();
        }
        #endregion

        [AuthoridUrl("Site/PageLayout", "")]
        #region LinkEdit
        public ActionResult LinkEdit(string itemid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            LinkEditModel model = null;
            if (itemid.IsNullorEmpty())
            {
                model = new LinkEditModel();
            }
            else
            {
                model = _IModelLinkManager.GetModelByID("", itemid);
            }
            return View(model);
        }
        #endregion

        #region SiteLayout
        [AuthoridUrl("Site/SiteLayout", "stype")]
        public ActionResult SiteLayout(string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (string.IsNullOrEmpty(stype)) { stype = "P"; ; }
            var model = _ISiteLayoutManager.GetSiteLayout(stype, LanguageID);
            if (stype == "M")
            {
                ViewBag.Title = "版面管理(手機板)";
            }
            else
            {
                ViewBag.Title = "網站版面資訊設定";
            }

            return View(model);
        }
        #endregion

        #region FowardSetting
        [AuthoridUrl("Site/SiteLayout", "stype")]
        public ActionResult FowardSetting(string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _ISiteLayoutManager.GetSiteLayout(stype, LanguageID);
            return View(model);
        }
        #endregion

        #region PrintSetting
        [AuthoridUrl("Site/SiteLayout", "stype")]
        public ActionResult PrintSetting(string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _ISiteLayoutManager.GetSiteLayout(stype, LanguageID);
            return View(model);
        }
        #endregion

        #region Page404Setting
        [AuthoridUrl("Site/SiteLayout", "stype")]
        public ActionResult Page404Setting(string stype)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _ISiteLayoutManager.GetSiteLayout(stype, LanguageID);
            return View(model);
        }
        #endregion

        #region SaveSiteLayout
        public ActionResult SaveSiteLayout(SiteLayoutModel model)
        {

            //if (model.LogoImgHeight == null) { model.LogoImgHeight = 80; }
            //if (model.FirstPageImgHeight == null) { model.FirstPageImgHeight = 219; }
            //if (model.InsidePageImgHeight == null) { model.InsidePageImgHeight = 219; }
            if (Request.IsAuthenticated)
            {
                if (model.LogoImageFile != null)
                {
                    var fileformat = model.LogoImageFile.FileName.Split('.');
                    var fullfilename = model.LogoImageFile.FileName.Split('\\').Last();
                    var orgfilename = fullfilename.Substring(0, fullfilename.LastIndexOf("."));
                    long ticks = DateTime.Now.Ticks;
                    var root = Request.PhysicalApplicationPath;
                    var filename = ticks + "." + fileformat.Last();
                    var checkpath = root + "\\UploadImage\\SiteLayout\\";
                    if (System.IO.Directory.Exists(checkpath) == false)
                    {
                        System.IO.Directory.CreateDirectory(checkpath);
                    }
                    model.LogoImgNameOri = ticks + "_" + fullfilename;
                    var path = root + "\\UploadImage\\SiteLayout\\" + model.LogoImgNameOri;
                    model.LogoImageFile.SaveAs(path);
                    model.LogoImgShowName = fullfilename;
                    var thumbpath = root + "\\UploadImage\\SiteLayout\\" + ticks + "_thumb." + fileformat.Last();
                    model.LogoImgNameThumb = ticks + "_thumb." + fileformat.Last();
                    var imgdata = _imgsqlrepository.GetByWhere("item=@1", new object[] { "logo" });
                    int height = 80;
                    var chartheight = 80;
                    if (imgdata.Count() > 0) { height = imgdata.First().height.Value; }
                    if (chartheight > height) { chartheight = height; };
                    var haspath = Utilities.UploadImg.uploadImgThumbMaxHeight(path, thumbpath, chartheight, fileformat.Last());
                    if (haspath == "") { model.LogoImgNameThumb = ""; }
                }

                if (model.InnerLogoImageFile != null)
                {
                    var fileformat = model.InnerLogoImageFile.FileName.Split('.');
                    var fullfilename = model.InnerLogoImageFile.FileName.Split('\\').Last();
                    var orgfilename = fullfilename.Substring(0, fullfilename.LastIndexOf("."));
                    long ticks = DateTime.Now.Ticks;
                    var root = Request.PhysicalApplicationPath;
                    var filename = ticks + "." + fileformat.Last();
                    var checkpath = root + "\\UploadImage\\SiteLayout\\";
                    if (System.IO.Directory.Exists(checkpath) == false)
                    {
                        System.IO.Directory.CreateDirectory(checkpath);
                    }
                    model.InnerLogoImgNameOri = ticks + "_" + fullfilename;
                    var path = root + "\\UploadImage\\SiteLayout\\" + model.InnerLogoImgNameOri;
                    model.InnerLogoImageFile.SaveAs(path);
                    model.InnerLogoImgShowName = fullfilename;
                    var thumbpath = root + "\\UploadImage\\SiteLayout\\" + ticks + "_thumb." + fileformat.Last();
                    model.InnerLogoImgNameThumb = ticks + "_thumb." + fileformat.Last();
                    var imgdata = _imgsqlrepository.GetByWhere("item=@1", new object[] { "logo" });
                    int height = 80;
                    var chartheight = 80;
                    if (imgdata.Count() > 0) { height = imgdata.First().height.Value; }
                    if (chartheight > height) { chartheight = height; };
                    var haspath = Utilities.UploadImg.uploadImgThumbMaxHeight(path, thumbpath, chartheight, fileformat.Last());
                    if (haspath == "") { model.InnerLogoImgNameThumb = ""; }
                }
                model.PublishContent = HttpUtility.UrlDecode(model.PublishContent);
                model.HtmlContent = HttpUtility.UrlDecode(model.HtmlContent);
                model.LangID = LanguageID;
                return Json(_ISiteLayoutManager.EditSiteLayout(model));
            }
            else
            {
                return Json("請先登入");
            }
        }
        #endregion
        //pagemain
        #region PagingPageLayout
        public ActionResult PagingPageLayout(SearchModelBase model)
        {
            model.LangId = this.LanguageID;
            return Json(_ISiteLayoutManager.PagingMain(model));
        }
        #endregion

        //pagemain
        #region GetMenOption
        public ActionResult GetMenOption(int menuitem, int level, int parentid, int modelid)
        {
            return Json(_IMenuManager.GetMenuOption(int.Parse(this.LanguageID), level, parentid, menuitem, modelid));
        }
        #endregion


        #region GetModelItem
        public ActionResult GetModelItem(string modelid)
        {
            var returnstr = _IMenuManager.GetModelItemList(modelid, LanguageID);
            return Json(returnstr);
        }
        #endregion

        //pagemain
        #region SavePageLayoutEdit
        public ActionResult SavePageLayoutEdit(List<PageLayoutModel> model)
        {
            if (Request.IsAuthenticated)
            {
                var idx = 1;
                foreach (var tmodel in model)
                {
                    tmodel.LangID = int.Parse(this.LanguageID);
                    if (tmodel.ID <= 0)
                    {
                        tmodel.ID = idx;
                        _ISiteLayoutManager.Create(tmodel, LanguageID, this.Account, this.UserName);
                    }
                    else
                    {
                        _ISiteLayoutManager.Update(tmodel, this.Account, this.UserName);
                    }
                    idx++;
                }
            }
            return Json("");
        }
        #endregion

        #region EditSeq
        public ActionResult EditSeq(int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更入口首頁配置排序 ID=" + id + "排序=" + seq);
                return Json(_ISiteLayoutManager.UpdateSeq(id.Value, seq, LanguageID, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetMainDelete
        public ActionResult SetMainDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除下列入口首頁配置=" + delaccount);
                return Json(_ISiteLayoutManager.Delete(idlist, delaccount, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SaveFowardSiteLayout
        public ActionResult SaveFowardSiteLayout(SiteLayoutModel model)
        {

            if (Request.IsAuthenticated)
            {
                if (model.FowardImageFile != null)
                {
                    var fileformat = model.FowardImageFile.FileName.Split('.');
                    var fullfilename = model.FowardImageFile.FileName.Split('\\').Last();
                    var orgfilename = fullfilename.Substring(0, fullfilename.LastIndexOf("."));
                    long ticks = DateTime.Now.Ticks;
                    var root = Request.PhysicalApplicationPath;
                    var filename = ticks + "." + fileformat.Last();
                    var checkpath = root + "\\UploadImage\\SiteLayout\\";
                    if (System.IO.Directory.Exists(checkpath) == false)
                    {
                        System.IO.Directory.CreateDirectory(checkpath);
                    }
                    model.FowardImgNameOri = ticks + "_" + fullfilename;
                    var path = root + "\\UploadImage\\SiteLayout\\" + model.FowardImgNameOri;
                    model.FowardImageFile.SaveAs(path);
                    model.FowardImgShowName = fullfilename;
                    var thumbpath = root + "\\UploadImage\\SiteLayout\\" + ticks + "_thumb." + fileformat.Last();
                    model.FowardImgNameThumb = ticks + "_thumb." + fileformat.Last();
                    var imgdata = _imgsqlrepository.GetByWhere("item=@1", new object[] { "site_forward" });
                    int height = 150;
                    var chartheight = 150;
                    if (imgdata.Count() > 0) { height = imgdata.First().height.Value; }
                    if (chartheight > height) { chartheight = height; };

                    var haspath = Utilities.UploadImg.uploadImgThumbMaxHeight(path, thumbpath, chartheight, fileformat.Last());
                    if (haspath == "") { model.FowardImgNameThumb = ""; }
                }

                model.FowardHtmlContent = HttpUtility.UrlDecode(model.FowardHtmlContent);
                return Json(_ISiteLayoutManager.EditFowardSiteLayout(model));
            }
            else
            {
                return Json("請先登入");
            }
        }
        #endregion

        #region SavePrintSiteLayout
        public ActionResult SavePrintSiteLayout(SiteLayoutModel model)
        {

            if (Request.IsAuthenticated)
            {
                if (model.PrintImageFile != null)
                {
                    var fileformat = model.PrintImageFile.FileName.Split('.');
                    var fullfilename = model.PrintImageFile.FileName.Split('\\').Last();
                    var orgfilename = fullfilename.Substring(0, fullfilename.LastIndexOf("."));
                    long ticks = DateTime.Now.Ticks;
                    var root = Request.PhysicalApplicationPath;
                    var filename = ticks + "." + fileformat.Last();
                    var checkpath = root + "\\UploadImage\\SiteLayout\\";
                    if (System.IO.Directory.Exists(checkpath) == false)
                    {
                        System.IO.Directory.CreateDirectory(checkpath);
                    }
                    model.PrintImgNameOri = ticks + "_" + fullfilename;
                    var path = root + "\\UploadImage\\SiteLayout\\" + model.PrintImgNameOri;
                    model.PrintImageFile.SaveAs(path);
                    model.PrintImgShowName = fullfilename;
                    var thumbpath = root + "\\UploadImage\\SiteLayout\\" + ticks + "_thumb." + fileformat.Last();
                    model.PrintImgNameThumb = ticks + "_thumb." + fileformat.Last();
                    var imgdata = _imgsqlrepository.GetByWhere("item=@1", new object[] { "site_forward" });
                    int height = 150;
                    var chartheight = 150;
                    if (imgdata.Count() > 0) { height = imgdata.First().height.Value; }
                    if (chartheight > height) { chartheight = height; };

                    var haspath = Utilities.UploadImg.uploadImgThumbMaxHeight(path, thumbpath, chartheight, fileformat.Last());
                    if (haspath == "") { model.PrintImgNameThumb = ""; }
                }

                model.PrintHtmlContent = HttpUtility.UrlDecode(model.PrintHtmlContent);
                return Json(_ISiteLayoutManager.EditPrintSiteLayout(model));
            }
            else
            {
                return Json("請先登入");
            }
        }
        #endregion

        #region Save404SiteLayout
        public ActionResult Save404SiteLayout(SiteLayoutModel model)
        {

            if (Request.IsAuthenticated)
            {

                model.Page404HtmlContent = HttpUtility.UrlDecode(model.Page404HtmlContent);
                return Json(_ISiteLayoutManager.EditPage404SiteLayout(model));
            }
            else
            {
                return Json("請先登入");
            }
        }
        #endregion

        #region SiteLangText
        [AuthoridUrl("Site/SiteLangText", "")]
        public ActionResult SiteLangText()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            var model = _ISiteLayoutManager.GetSiteLangText(LanguageID);
            return View(model);
        }
        #endregion

        #region SaveSiteLangText
        public ActionResult SaveSiteLangText(SiteLangTextModel model)
        {
            var str = _ISiteLayoutManager.SaveSiteLangText(model, LanguageID);
            Common.SetAllLangKey();
            return Json(str);
        }
        #endregion

        #region PagingItemMessage
        public ActionResult PagingItemMessage(MessageSearchModel model)
        {
            if (model.Key.IsNullorEmpty()) { return Json(new Paging<MessageItemResult>()); }
            var menudata = _IMenuManager.GetModel(model.Key);
            model.ModelID = menudata.ModelItemID;
            model.Enabled = "1";
            return Json(_IMessageManager.PagingItem(model.ModelID.ToString(), model));
        }
        #endregion

        #region PagingItemActive
        public ActionResult PagingItemActive(ActiveSearchModel model)
        {
            if (model.Key.IsNullorEmpty()) { return Json(new Paging<ActiveItemResult>()); }
            var menudata = _IMenuManager.GetModel(model.Key);
            model.ModelID = menudata.ModelItemID;
            model.Enabled = "1";
            return Json(_IModelActiveManager.PagingItem(model.ModelID.ToString(), model));
        }
        #endregion

        #region SavePageLayoutOP1
        public ActionResult SavePageLayoutOP1Edit(PageLayoutOP1Model model)
        {
            model.Introduction = HttpUtility.UrlDecode(model.Introduction);
            if (model.LeftItem.Length > 0) { model.LeftItem[0].Desc = model.LeftItem[0].Desc == null ? "" : HttpUtility.UrlDecode(model.LeftItem[0].Desc); }
            if (model.LeftItem.Length > 1) { model.LeftItem[1].Desc = model.LeftItem[1].Desc == null ? "" : HttpUtility.UrlDecode(model.LeftItem[1].Desc); }
            if (model.LeftItem.Length > 2) { model.LeftItem[2].Desc = model.LeftItem[2].Desc == null ? "" : HttpUtility.UrlDecode(model.LeftItem[2].Desc); }
            if (model.LeftItem.Length > 3) { model.LeftItem[3].Desc = model.LeftItem[3].Desc == null ? "" : HttpUtility.UrlDecode(model.LeftItem[3].Desc); }
            if (model.RightItem.Length > 0) { model.RightItem[0].Desc = model.RightItem[0].Desc == null ? "" : HttpUtility.UrlDecode(model.RightItem[0].Desc); }
            if (model.RightItem.Length > 1) { model.RightItem[1].Desc = model.RightItem[1].Desc == null ? "" : HttpUtility.UrlDecode(model.RightItem[1].Desc); }
            if (model.RightItem.Length > 2) { model.RightItem[2].Desc = model.RightItem[2].Desc == null ? "" : HttpUtility.UrlDecode(model.RightItem[2].Desc); }

            model.LangID = int.Parse(this.LanguageID);
            string rs = _ISiteLayoutManager.SavePageLayoutOP1Edit(model);
            return Json(rs);
        }
        #endregion

        #region SavePageLayoutOP2
        public ActionResult SavePageLayoutOP2Edit(PageLayoutOP2Model model)
        {
            model.Introduction = HttpUtility.UrlDecode(model.Introduction);
            model.LangID = int.Parse(this.LanguageID);
            string rs = _ISiteLayoutManager.SavePageLayoutOP2Edit(model);
            return Json(rs);
        }
        #endregion

        #region SavePageLayoutOP3
        public ActionResult SavePageLayoutOP3Edit(PageLayoutOP3Model model)
        {
            model.Introduction = HttpUtility.UrlDecode(model.Introduction);
            model.LangID = int.Parse(this.LanguageID);
            string rs = _ISiteLayoutManager.SavePageLayoutOP3Edit(model);
            return Json(rs);
        }
        #endregion

        #region SavePageActivity
        public ActionResult SavePageActivity(PageLayoutActivityModel model)
        {
            model.LangID = int.Parse(this.LanguageID);
            if (model.Items.Length > 0) { model.Items[0].Desc = model.Items[0].Desc == null ? "" : HttpUtility.UrlDecode(model.Items[0].Desc); }
            if (model.Items.Length > 1) { model.Items[1].Desc = model.Items[1].Desc == null ? "" : HttpUtility.UrlDecode(model.Items[1].Desc); }
            if (model.Items.Length > 2) { model.Items[2].Desc = model.Items[2].Desc == null ? "" : HttpUtility.UrlDecode(model.Items[2].Desc); }
            if (model.Items.Length > 3) { model.Items[3].Desc = model.Items[3].Desc == null ? "" : HttpUtility.UrlDecode(model.Items[3].Desc); }
            if (model.Items.Length > 4) { model.Items[4].Desc = model.Items[4].Desc == null ? "" : HttpUtility.UrlDecode(model.Items[4].Desc); }
            if (model.Items.Length > 5) { model.Items[5].Desc = model.Items[5].Desc == null ? "" : HttpUtility.UrlDecode(model.Items[5].Desc); }
            if (model.Items.Length > 6) { model.Items[6].Desc = model.Items[6].Desc == null ? "" : HttpUtility.UrlDecode(model.Items[6].Desc); }

            string rs = _ISiteLayoutManager.SavePageActivity(model);
            return Json(rs);
        }
        #endregion

        #region SaveItem
        public ActionResult SaveItem(LinkEditModel model)
        {
            if (model.ImageFile != null)
            {
                var root = Request.PhysicalApplicationPath;
                model.ImageFileOrgName = model.ImageFile.FileName.Split('\\').Last();
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
                }
                var newfilename = DateTime.Now.Ticks + "_" + model.ImageFileOrgName;
                var path = root + "\\UploadImage\\LinkItem\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\LinkItem\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\LinkItem\\");
                }
                model.ImageFile.SaveAs(path);
                model.ImageFileName = newfilename;
            }

            model.Title = HttpUtility.UrlDecode(model.Title);
            if (model.ItemID == -1)
            {
                Common.SetLogs(this.UserID, this.Account, "新增網站連結=" + model.Title);
                return Json(_IModelLinkManager.CreateItem(model, this.LanguageID, this.Account));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "新增網站連結=" + model.ItemID + " Name=" + model.Title);
                return Json(_IModelLinkManager.UpdateItem(model, this.LanguageID, this.Account));
            }
        }
        #endregion

        #region PagingLinkItem
        public ActionResult PagingLinkItem(SearchModelBase model)
        {
            model.LangId = this.LanguageID;
            return Json(_IModelLinkManager.PagingItem(model.ModelID.ToString(), model));
        }
        #endregion

        #region UpdateLinkItemSeq
        public ActionResult UpdateLinkItemSeq(int id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改網站連結排序ID=" + id + " sequence=" + seq);
                return Json(_IModelLinkManager.UpdateItemSeq(int.Parse(this.LanguageID), id, seq, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetLinkItemStatus
        public ActionResult SetLinkItemStatus(string id, bool status, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改網站連結狀態ID=" + id + " status=" + status);
                return Json(_IModelLinkManager.SetItemStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetItemLinkDelete
        public ActionResult SetItemLinkDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除網站連結=" + delaccount);
                return Json(_IModelLinkManager.DeleteItem(idlist, delaccount, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region SetImage
        public ActionResult SetImage(string imageindex, string base64)
        {
            try
            {
                var model = _ISiteLayoutManager.PageLayoutActivity(this.LanguageID);
                var image = model.Items[int.Parse(imageindex) - 1];
                var tpath = Server.MapPath(image.FilePath);
                if (System.IO.File.Exists(tpath))
                {
                    string[] pd = base64.Split(',');
                    var bytes = Convert.FromBase64String(pd.Length > 1 ? pd[1] : pd[0]);
                    using (var imageFile = new FileStream(tpath, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }
                }
                else
                {
                    return Json("路徑=" + System.Web.HttpContext.Current.Request.PhysicalApplicationPath + image.FilePath + "查無檔案");
                }
                return Json("設定完成");
            }
            catch (Exception ex)
            {
                return Json("發生錯誤:" + ex.Message);
            }
            return Json("作業完成");
        }
        #endregion

        #region Upload
        public ActionResult Upload(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string result = "";
            var filename = "";
            var imageUrl = "";
            if (upload != null && upload.ContentLength > 0)
            {
                //儲存圖片至Server
                var last = upload.FileName.Split('.').Last();
                filename = DateTime.Now.Ticks + "." + last;
                var root = Request.PhysicalApplicationPath + "/UploadImage/SiteLayout/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/SiteLayout/" + filename);
                var vMessage = string.Empty;
                result = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + imageUrl + "\", \"" + vMessage + "\");</script></body></html>";
            }
            return Json(new
            {
                uploaded = 1,
                fileName = filename,
                url = imageUrl
            });

        }
        #endregion

        #region UploadFile
        public ActionResult UploadFile(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string result = "";
            var filename = "";
            var imageUrl = "";
            if (upload != null && upload.ContentLength > 0)
            {
                //儲存圖片至Server
                var last = upload.FileName.Split('.').Last();
                filename = DateTime.Now.Ticks + "." + last;
                var root = Request.PhysicalApplicationPath + "/UploadImage/SiteLayout/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/SiteLayout/" + filename);
                var vMessage = string.Empty;
                result = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + imageUrl + "\", \"" + vMessage + "\");</script></body></html>";
            }
            return Json(new
            {
                uploaded = 1,
                fileName = filename,
                url = imageUrl
            });
        }
        #endregion

    }
}