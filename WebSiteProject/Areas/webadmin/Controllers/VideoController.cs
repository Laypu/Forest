﻿using Services.Interface;
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
    public class VideoController : AppController
    {
        IModelVideoManager _IModelVideoManager;
        public VideoController()
        {
            _IModelVideoManager = serviceinstance.ModelVideoManager;
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult Index()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            Session["IsFromClick"] = "Y";
            ViewBag.Title = "影音管理";
            return View();
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult UnitSetting(string mainid)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.modelid = mainid.AntiXssEncode();
            var model = _IModelVideoManager.GetUnitModel(mainid);
            var maindata = _IModelVideoManager.Where(new  ModelVideoMain()
            {
                ID = int.Parse(mainid)
            });
            if (maindata.Count() > 0) { ViewBag.Title = maindata.First().Name; }
            return View(model);
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult GroupEdit(string mainid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.mainid = mainid.AntiXssEncode();
            return View();
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult VideoEdit(string mainid, string itemid="-1")
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            var isview = Request.Form["isview"] == null ? "" : Request.Form["isview"];
            ViewBag.isview = isview.AntiXssEncode();
            if (isview == null || isview == "")
            {
                CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            }
            ViewBag.grouplist = _IModelVideoManager.GetAllGroupSelectList(mainid);
            VideoEditModel model = null;
            model = _IModelVideoManager.GetModelByID(mainid, itemid);
            return View(model);
        }

        //====
        #region EditUnit
        public ActionResult EditUnit(string mainid, string name)
        {
            if (Request.IsAuthenticated)
            {
                var str = "";
                int newid = 0;
                if (mainid == "-1" ||string.IsNullOrEmpty(mainid))
                {
                    Common.SetLogs(this.UserID, this.Account, "新增影音管理單元名稱=" + name);
               
                    str = _IModelVideoManager.AddUnit(name, this.LanguageID, this.Account,ref newid);
                }
                else
                {
                    Common.SetLogs(this.UserID, this.Account, "修改影音管理單元名稱 ID=" + mainid + " 改為:" + name);
                    str = _IModelVideoManager.UpdateUnit(name, mainid, this.Account);
                }
                return Json(str);
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region PagingMain
        public ActionResult PagingMain(SearchModelBase model)
        {
            model.LangId = this.LanguageID;
            return Json(_IModelVideoManager.Paging(model));
        }
        #endregion

        #region EditSeq
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSeq(int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更影音管理單元管理排序 ID=" + id + "排序=" + seq);
                return Json(_IModelVideoManager.UpdateSeq(id.Value, seq, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetMainDelete
        public ActionResult SetMainDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除下列影音管理=" + delaccount);
                return Json(_IModelVideoManager.Delete(idlist, delaccount, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion
        //==
        #region ModelItem
        [AuthoridUrl("Model/Index", "")]
        //[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ModelItem(string mainid,string menuindex)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            if (Session["IsFromClick"] != null)
            {
                ViewBag.IsFromClick = "Y";
            }
            var grouplist = _IModelVideoManager.GetAllGroupSelectList(mainid);
            grouplist.Insert(0, new System.Web.Mvc.SelectListItem() { Text = "全部", Value = "" });
            ViewBag.grouplist = grouplist;
            ViewBag.mainid = mainid.AntiXssEncode();
           var maindata = _IModelVideoManager.Where(new ModelVideoMain()
            {
                ID = int.Parse(mainid)
            });
            if (maindata.Count() > 0) { ViewBag.Title = maindata.First().Name; }
          
            return View();
        }
        #endregion

        #region PagingItem
        public ActionResult PagingItem(VideoSearchModel model)
        {
            return Json(_IModelVideoManager.PagingItem(model.ModelID.ToString(),model));
        }
        #endregion

        //group
        #region PagingGroup
        public ActionResult PagingGroup(SearchModelBase model)
        {
            return Json(_IModelVideoManager.PagingGroup(model));
        }
        #endregion

        #region EditGroup
        public ActionResult EditGroup(string name, string id,string mainid)
        {
            if (id == "-1" || id.IsNullorEmpty())
            {
                Common.SetLogs(this.UserID, this.Account, "新增影音管理群組名稱=" + name + " mainid=" + mainid);
                return Json(_IModelVideoManager.EditGroup(name, id, mainid,this.Account));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改影音管理群組名稱=" + name + " id=" + id);
                return Json(_IModelVideoManager.EditGroup(name, id, mainid,this.Account));
            }

        }
        #endregion

        #region EditGroupSeq
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGroupSeq(int? id, int seq, string mainid)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更影音管理群組排序MainID=" + mainid + " ID =" + id + "排序=" + seq);
                return Json(_IModelVideoManager.UpdateGroupSeq(id.Value, seq, mainid, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetGroupDelete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetGroupDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除影音管理群組=" + delaccount);
                return Json(_IModelVideoManager.DeleteGroup(idlist, delaccount, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region SetGroupStatus
        public ActionResult SetGroupStatus(string id, bool status, string account, string username)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定影音管理群組id=" + id + "為" + status);
                return Json(_IModelVideoManager.UpdateGroupStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        //==column
        #region PagingColumn
        public ActionResult PagingColumn(SearchModelBase model)
        {
            return Json(_IModelVideoManager.ColumnPaging(model));
        }
        #endregion

        #region SetColumnStatus
        public ActionResult SetColumnStatus(string id, bool status, string account, string username)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IModelVideoManager.UpdateColumnStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region EditColumnSeq
        public ActionResult EditColumnSeq(int? id, int seq, string mainid)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IModelVideoManager.UpdateColumnSeq(id.Value, seq,  this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SaveUnit
        public ActionResult SaveUnit(VideoUnitSettingModel model)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定影音管理單元管理");
                return Json(_IModelVideoManager.SetUnitModel(model, this.Account));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        //===
        #region SEOSetting
        [AuthoridUrl("Model/Index", "")]
        public ActionResult SEOSetting(string modelid)
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.mainid = modelid.AntiXssEncode();
            return View(_IModelVideoManager.GetSEO(modelid));
        }
        #endregion

        #region SaveSEO
        public ActionResult SaveSEO(SEOViewModel model)
        {
            Common.SetLogs(this.UserID, this.Account, "修改影音管理SEO");
            return Json(_IModelVideoManager.SaveSEO(model, this.LanguageID));
        } 
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string modelid, string itemid)
        {
            var model = _IModelVideoManager.GetModelByID(modelid, itemid);
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

        #region SaveItem_Original
        //public ActionResult SaveItem(VideoEditModel model)
        //{
        //    if (model.UploadFile != null)
        //    {
        //        model.UploadFileName = model.UploadFile.FileName.Split('\\').Last();
        //        var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
        //        if (uploadfilepath.IsNullorEmpty())
        //        {
        //            uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
        //        }
        //        var newpath = uploadfilepath + "\\VideoItem\\";
        //        if (System.IO.Directory.Exists(newpath) == false)
        //        {
        //            System.IO.Directory.CreateDirectory(newpath);
        //        }
        //        var guid = Guid.NewGuid();
        //        var filename = DateTime.Now.Ticks + "." + model.UploadFile.FileName.Split('.').Last();
        //        var path = newpath + filename;
        //        model.UploadFilePath = "\\VideoItem\\" + filename;
        //        model.UploadFile.SaveAs(path);
        //    }

        //    if (model.ImageFile != null)
        //    {
        //        var root = Request.PhysicalApplicationPath;
        //        model.ImageFileOrgName = model.ImageFile.FileName.Split('\\').Last();
        //        var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
        //        if (uploadfilepath.IsNullorEmpty())
        //        {
        //            uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
        //        }
        //        var newfilename = DateTime.Now.Ticks + "_" + model.ImageFileOrgName;
        //        var path = root + "\\UploadImage\\VideoItem\\" + newfilename;
        //        if (System.IO.Directory.Exists(root + "\\UploadImage\\VideoItem\\") == false)
        //        {
        //            System.IO.Directory.CreateDirectory(root + "\\UploadImage\\VideoItem\\");
        //        }
        //        model.ImageFile.SaveAs(path);
        //        model.ImageFileName = newfilename;
        //    }

        //    if (model.RelateImageFile != null)
        //    {
        //        var root = Request.PhysicalApplicationPath;
        //        model.RelateImageFileOrgName = model.RelateImageFile.FileName.Split('\\').Last();
        //        var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
        //        if (uploadfilepath.IsNullorEmpty())
        //        {
        //            uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
        //        }
        //        var newfilename = DateTime.Now.Ticks + "_" + model.RelateImageFileOrgName;
        //        var path = root + "\\UploadImage\\VideoItem\\" + newfilename;
        //        if (System.IO.Directory.Exists(root + "\\UploadImage\\VideoItem\\") == false)
        //        {
        //            System.IO.Directory.CreateDirectory(root + "\\UploadImage\\VideoItem\\");
        //        }
        //        model.RelateImageFile.SaveAs(path);
        //        model.RelateImageName = newfilename;
        //    }

        //    model.HtmlContent = HttpUtility.UrlDecode(model.HtmlContent);
        //    model.Title = HttpUtility.UrlDecode(model.Title);
        //    if (model.ItemID == -1)
        //    {
        //        Common.SetLogs(this.UserID, this.Account, "新增部落客文章管理=" + model.Title);
        //        return Json(_IModelVideoManager.CreateItem(model, this.LanguageID, this.Account));
        //    }
        //    else
        //    {
        //        Common.SetLogs(this.UserID, this.Account, "修改部落客文章管理ID=" + model.ItemID + " Name=" + model.Title);
        //        return Json(_IModelVideoManager.UpdateItem(model, this.LanguageID, this.Account));
        //    }
        //}
        #endregion


        #region SaveItem_New
        public ActionResult SaveItem(VideoEditModel model)
        {
            if (model.UploadFile != null)
            {                                                           
                /////////////////////////////////////////////////////////////////////
                model.UploadFileName = model.UploadFile.FileName.Split('\\').Last();

                string uploadPath = Server.MapPath("~/UploadFile/VideoItem/");
                string filename = "";
                // 如果UploadFiles文件夹不存在则先创建
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                filename = model.UploadFile.FileName.Split('\\').Last();  //取得檔案名
                var path = Path.Combine(Server.MapPath("~/UploadFile/VideoItem/"), filename);  //取得本機檔案路徑

                //若有重複則換名字_srart
                while (System.IO.File.Exists(path))
                {
                    Random rand = new Random();
                    filename = rand.Next().ToString() + "-" + filename;
                    path = Path.Combine(Server.MapPath("~/UploadFile/VideoItem/"), filename);
                }

                model.UploadFilePath = "\\VideoItem\\" + filename;
                model.UploadFile.SaveAs(path);
                //若有重複則換名字_end
                //////////////////////////////////////////////////////////////////////                     
            }

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
                var path = root + "\\UploadImage\\VideoItem\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\VideoItem\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\VideoItem\\");
                }
                model.ImageFile.SaveAs(path);
                model.ImageFileName = newfilename;
            }

            if (model.RelateImageFile != null)
            {
                var root = Request.PhysicalApplicationPath;
                model.RelateImageFileOrgName = model.RelateImageFile.FileName.Split('\\').Last();
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
                }
                var newfilename = DateTime.Now.Ticks + "_" + model.RelateImageFileOrgName;
                var path = root + "\\UploadImage\\VideoItem\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\VideoItem\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\VideoItem\\");
                }
                model.RelateImageFile.SaveAs(path);
                model.RelateImageName = newfilename;
            }


            model.HtmlContent = HttpUtility.UrlDecode(model.HtmlContent);
            model.Title = HttpUtility.UrlDecode(model.Title);
            if (model.ItemID == -1)
            {
                Common.SetLogs(this.UserID, this.Account, "新增部落客文章管理=" + model.Title);
                return Json(_IModelVideoManager.CreateItem(model, this.LanguageID, this.Account));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改部落客文章管理ID=" + model.ItemID + " Name=" + model.Title);
                return Json(_IModelVideoManager.UpdateItem(model, this.LanguageID, this.Account));
            }
        }
        #endregion

        #region UpdateItemSeq
        public ActionResult UpdateItemSeq(int modelid,int id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改部落客文章管理排序ID=" + id + " sequence=" + seq);
                return Json(_IModelVideoManager.UpdateItemSeq(modelid,id, seq,this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetItemStatus
        public ActionResult SetItemStatus(string id, bool status, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改部落客文章管理狀態ID=" + id + " status=" + status);
                return Json(_IModelVideoManager.SetItemStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetItemDelete
        public ActionResult SetItemDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除部落客文章管理=" + delaccount);
                return Json(_IModelVideoManager.DeleteItem(idlist, delaccount, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

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
                var root = Server.MapPath("~/UploadImage/VideoItem/");
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/VideoItem/" + filename);
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
                var root = Server.MapPath("~/UploadImage/VideoItem/");
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/VideoItem/" + filename);
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