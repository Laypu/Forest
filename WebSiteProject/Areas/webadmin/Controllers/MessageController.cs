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
    
    public class MessageController : AppController
    {
        WebSiteProject.Models.ForestEntities db = new Models.ForestEntities();


        IModelMessageManager _IMessageManager;
        public MessageController()
        {
            _IMessageManager = serviceinstance.MessageManager;
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult Index()
        {
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            Session["IsFromClick"] = "Y";
            ViewBag.Title = "部落客文章管理";
            return View();
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult UnitSetting(string mainid)
        {
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            ViewBag.modelid = mainid.AntiXssEncode(); ;
            var model = _IMessageManager.GetUnitModel(mainid);
            var maindata = _IMessageManager.Where(new ModelMessageMain()
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
            ViewBag.mainid = mainid.AntiXssEncode(); ;
            return View();
        }
        [AuthoridUrl("Model/Index", "")]
        public ActionResult MessageEdit(string mainid, string itemid="-1")
        {
            var isview = Request.Form["isview"] == null ? "" : Request.Form["isview"];
            ViewBag.isview = isview.AntiXssEncode();
            if (mainid.IsNullorEmpty()) { return RedirectToAction("Index"); }
            if (isview == null || isview == "")
            {
                CheckAuth(System.Reflection.MethodBase.GetCurrentMethod());
            }
            ViewBag.grouplist = _IMessageManager.GetAllGroupSelectList(mainid);
            MessageEditModel model = null;
            model = _IMessageManager.GetModelByID(mainid, itemid);

            //旅遊資訊標籤
            ViewBag.HashTag = db.F_HashTag_Type.ToList();

            //旅遊資訊關聯
            ViewBag.Sub_HashTag = db.F_Sub_HashTag_Type.Where(s => s.MessageItem_ID.ToString() == itemid).ToList();

            //目的地關聯
            var Destinations_ID = new List<SelectListItem>();
            foreach (var item in db.F_Destination_Type)
            {
                Destinations_ID.Add(new SelectListItem()
                {
                    Text = item.Destination_Type_Title1 + " " + item.Destination_Type_Title2,
                    Value = Convert.ToString(item.Destination_Type_ID),
                    Selected = false
                });
            }
            ViewBag.Destination_Type_ID = Destinations_ID;
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
                    Common.SetLogs(this.UserID, this.Account, "新增部落客文章管理單元名稱=" + name);
               
                    str = _IMessageManager.AddUnit(name, this.LanguageID, this.Account,ref newid);
                }
                else
                {
                    Common.SetLogs(this.UserID, this.Account, "修改部落客文章管理單元名稱 ID=" + mainid + " 改為:" + name);
                    str = _IMessageManager.UpdateUnit(name, mainid, this.Account);
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
            return Json(_IMessageManager.Paging(model));
        }
        #endregion

        #region EditSeq
        public ActionResult EditSeq(int? id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更部落客文章管理單元管理排序 ID=" + id + "排序=" + seq);
                return Json(_IMessageManager.UpdateSeq(id.Value, seq, this.LanguageID, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetMainDelete
        public ActionResult SetMainDelete(string[] idlist, string delaccount)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除下列部落客文章管理=" + delaccount);
                return Json(_IMessageManager.Delete(idlist, delaccount, this.LanguageID, this.Account, this.UserName));
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
            var grouplist = _IMessageManager.GetAllGroupSelectList(mainid);
            grouplist.Insert(0, new System.Web.Mvc.SelectListItem() { Text = "全部", Value = "" });
            ViewBag.grouplist = grouplist;
            ViewBag.mainid = mainid.AntiXssEncode(); ;
           var maindata = _IMessageManager.Where(new ModelMessageMain()
            {
                ID = int.Parse(mainid)
            });
            if (maindata.Count() > 0) { ViewBag.Title = maindata.First().Name; }

            //旅遊資訊的訊息模組
            if (mainid == "9")
            {
                //取得HashTag
                var HashTag = db.F_HashTag_Type.Select(c => new
                {
                    c.HashTag_Type_ID,
                    c.HashTag_Type_Name,                    
                }).OrderBy(c => c.HashTag_Type_ID);
                //var CategoryMID = db.Products.Find(id).CategorySmall.CategoryMID;
                ViewBag.HashTag = new SelectList(HashTag, "HashTag_Type_ID", "HashTag_Type_Name");
                
                
                var Destinations_ID = new List<SelectListItem>();
                foreach (var item in db.F_Destination_Type)
                {
                    Destinations_ID.Add(new SelectListItem()
                    {
                        Text = item.Destination_Type_Title1 + " " + item.Destination_Type_Title2,
                        Value = Convert.ToString(item.Destination_Type_ID),
                        Selected = false
                    });
                }
                ViewBag.Destination_Type_ID = Destinations_ID;

            }
          
            return View();
        }
        #endregion

        #region PagingItem
        public ActionResult PagingItem(MessageSearchModel model)
        {            
           return Json(_IMessageManager.PagingItem(model.ModelID.ToString(), model));                        
        }
        #endregion

        //group
        #region PagingGroup
        public ActionResult PagingGroup(SearchModelBase model)
        {
            return Json(_IMessageManager.PagingGroup(model));
        }
        #endregion

        #region EditGroup
        public ActionResult EditGroup(string name, string id,string mainid)
        {
            if (id == "-1" || id.IsNullorEmpty())
            {
                Common.SetLogs(this.UserID, this.Account, "新增部落客文章管理群組名稱=" + name + " mainid=" + mainid);
                return Json(_IMessageManager.EditGroup(name, id, mainid,this.Account));
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改部落客文章管理群組名稱=" + name + " id=" + id);
                return Json(_IMessageManager.EditGroup(name, id, mainid,this.Account));
            }

        }
        #endregion

        #region EditGroupSeq
        public ActionResult EditGroupSeq(int? id, int seq, string mainid)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "變更部落客文章管理群組排序MainID=" + mainid + " ID =" + id + "排序=" + seq);
                return Json(_IMessageManager.UpdateGroupSeq(id.Value, seq, mainid, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SetGroupDelete
        public ActionResult SetGroupDelete(string[] idlist, string delaccount, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "刪除部落客文章管理群組=" + delaccount);
                return Json(_IMessageManager.DeleteGroup(idlist, delaccount, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region SetGroupStatus
        public ActionResult SetGroupStatus(string id, bool status, string account, string username)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定部落客文章管理群組id=" + id + "為" + status);
                return Json(_IMessageManager.UpdateGroupStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        //==column
        #region PagingColumn
        public ActionResult PagingColumn(SearchModelBase model)
        {
            return Json(_IMessageManager.ColumnPaging(model));
        }
        #endregion

        #region SetColumnStatus
        public ActionResult SetColumnStatus(string id, bool status, string account, string username)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IMessageManager.UpdateColumnStatus(id, status, this.Account, this.UserName));
            }
            else { return Json("請先登入"); }

        }
        #endregion

        #region EditColumnSeq
        public ActionResult EditColumnSeq(int? id, int seq, string mainid)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IMessageManager.UpdateColumnSeq(id.Value, seq,  this.Account, this.UserName));
            }
            else { return Json("請先登入"); }
        }
        #endregion

        #region SaveUnit
        public ActionResult SaveUnit(MessageUnitSettingModel model)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "設定部落客文章管理單元管理");
                return Json(_IMessageManager.SetUnitModel(model, this.Account));
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
            return View(_IMessageManager.GetSEO(modelid));
        }
        #endregion

        #region SaveSEO
        public ActionResult SaveSEO(SEOViewModel model)
        {
            Common.SetLogs(this.UserID, this.Account, "修改部落客文章管理SEO");
            return Json(_IMessageManager.SaveSEO(model, this.LanguageID));
        } 
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string modelid, string itemid)
        {
            var model = _IMessageManager.GetModelByID(modelid, itemid);
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

        #region SaveItem
        public ActionResult SaveItem(MessageEditModel model, List<int> HashTag_Type,int Destination_Type_ID)
        {
            if (model.UploadFile != null)
            {
                model.UploadFileName = model.UploadFile.FileName.Split('\\').Last();
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
                }
                var newpath = uploadfilepath + "\\MessageItem\\";
                if (System.IO.Directory.Exists(newpath) == false)
                {
                    System.IO.Directory.CreateDirectory(newpath);
                }
                var guid = Guid.NewGuid();
                var filename = DateTime.Now.Ticks + "." + model.UploadFile.FileName.Split('.').Last();
                var path = newpath + filename;
                model.UploadFilePath = "\\MessageItem\\"+ filename;
                model.UploadFile.SaveAs(path);
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
                var path = root + "\\UploadImage\\MessageItem\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\MessageItem\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\MessageItem\\");
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
                var path = root + "\\UploadImage\\MessageItem\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\MessageItem\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\MessageItem\\");
                }
                model.RelateImageFile.SaveAs(path);
                model.RelateImageName = newfilename;
            }

            model.HtmlContent = HttpUtility.UrlDecode(model.HtmlContent);
            model.Title = HttpUtility.UrlDecode(model.Title);
            if (model.ItemID == -1)
            {                
                Common.SetLogs(this.UserID, this.Account, "新增部落客文章管理=" + model.Title);
                //return Json(_IMessageManager.CreateItem(model, this.LanguageID, this.Account, HashTag_Type));
                int result = _IMessageManager.CreateItem(model, this.LanguageID, this.Account, HashTag_Type);

                if (result > 0) //result為itemID >0為新增成功
                {
                    //增加欄位
                    foreach (var item in HashTag_Type)
                    {
                        WebSiteProject.Models.F_Sub_HashTag_Type Sub_HashTag = new Models.F_Sub_HashTag_Type();
                        Sub_HashTag.MessageItem_ID = result;
                        Sub_HashTag.HashTag_Type_ID = item;
                        db.F_Sub_HashTag_Type.Add(Sub_HashTag);
                        db.SaveChanges();                    
                    };
                    
                        WebSiteProject.Models.Message_DesHash MDHash = new Models.Message_DesHash();
                        MDHash.MessageItem_ID = result;
                        MDHash.Destination_Type_ID = Destination_Type_ID;
                        db.Message_DesHash.Add(MDHash);
                        db.SaveChanges();
                    


                    return Json("成功");
                };
                return Json("失敗");
            }
            else
            {
                Common.SetLogs(this.UserID, this.Account, "修改部落客文章管理ID=" + model.ItemID + " Name=" + model.Title);
                //return Json(_IMessageManager.UpdateItem(model, this.LanguageID, this.Account, HashTag_Type));
                int result = _IMessageManager.UpdateItem(model, this.LanguageID, this.Account, HashTag_Type);
                if (result > 0) //result為itemID >0為新增成功
                {
                    var Old_HashTag = db.F_Sub_HashTag_Type.Where(a => a.MessageItem_ID == model.ItemID).ToList(); ;
                    //先全部刪除舊的
                    var Old_DesHash = db.Message_DesHash.Where(b => b.MessageItem_ID == model.ItemID).ToList();
                    foreach (var item in Old_HashTag)
                    {
                        var Old_HashTag_Item = db.F_Sub_HashTag_Type.Find(item.Sub_HashTag_Type_ID);
                        db.Entry(Old_HashTag_Item).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();                        
                    };
                    foreach (var item in Old_DesHash)
                    {
                        var Old_DesHash_Item = db.Message_DesHash.Find(item.Destination_Type_ID);
                        db.Entry(Old_DesHash_Item).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();
                    };

                    //新增加
                    foreach (var item in HashTag_Type)
                    {
                        WebSiteProject.Models.F_Sub_HashTag_Type Sub_HashTag = new Models.F_Sub_HashTag_Type();
                        Sub_HashTag.MessageItem_ID = model.ItemID;
                        Sub_HashTag.HashTag_Type_ID = item;
                        db.F_Sub_HashTag_Type.Add(Sub_HashTag);
                        db.SaveChanges();
                    };
                    
                        WebSiteProject.Models.Message_DesHash MDHash = new Models.Message_DesHash();
                        MDHash.MessageItem_ID = model.ItemID;
                        MDHash.Destination_Type_ID = Destination_Type_ID;
                        db.Message_DesHash.Add(MDHash);
                        db.SaveChanges();
                    



                    return Json("成功");
                };
                return Json("失敗");
            }
        }
        #endregion

        #region UpdateItemSeq
        public ActionResult UpdateItemSeq(int modelid,int id, int seq, string type)
        {
            if (Request.IsAuthenticated)
            {
                Common.SetLogs(this.UserID, this.Account, "修改部落客文章管理排序ID=" + id + " sequence=" + seq);
                return Json(_IMessageManager.UpdateItemSeq(modelid,id, seq,this.Account, this.UserName));
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
                return Json(_IMessageManager.SetItemStatus(id, status, this.Account, this.UserName));
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
                return Json(_IMessageManager.DeleteItem(idlist, delaccount, this.Account, this.UserName));
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
                var root = Request.PhysicalApplicationPath+ "/UploadImage/MessageItem/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                //if (System.IO.Directory.Exists(Server.MapPath("/UploadImage/MessageItem/")) == false)
                //{
                //    System.IO.Directory.CreateDirectory(Server.MapPath("/UploadImage/MessageItem/"));
                //}
                upload.SaveAs(root + filename);
                // var imageUrl = "http://"+Request.Url.Authority+Url.Content("/UploadImage/MessageItem/" + filename);
                 imageUrl = Url.Content((Request.ApplicationPath=="/"?"": Request.ApplicationPath )+ "/UploadImage/MessageItem/" + filename);
                var vMessage = string.Empty;
                result = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + imageUrl + "\", \"" + vMessage + "\");</script></body></html>";
            }
            return Json(new
            {
                uploaded = 1,
                fileName = filename,
                url = imageUrl
            });
            //return Content(result);

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
                var last = upload.FileName.Split('.').Last();
                filename = DateTime.Now.Ticks + "." + last;
                var root = Request.PhysicalApplicationPath + "/UploadImage/MessageItem/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/MessageItem/" + filename);
                var vMessage = string.Empty;
                result = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + imageUrl + "\", \"" + vMessage + "\");</script></body></html>";
            }
            return Json(new
            {
                uploaded = 1,
                fileName = filename,
                url = imageUrl
            });
            //return Content(result);

        }
        #endregion


        //===20200109_Select_ThingsToDo
        public ActionResult ShowThingsToDoList(int? ThingsToDoID, int? Destination_Type_ID)
        {
            var _ThingsToDo_Data = db.F_Sub_HashTag_Type.Where(f => f.HashTag_Type_ID == ThingsToDoID).Select(f => new { f.MessageItem_ID});
            
            return Json(_ThingsToDo_Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowDestination(int? Destination_Type_ID, int? ThingsToDoID)
        {
            var DestinationData = db.Message_DesHash.Where(MDH => MDH.Destination_Type_ID == Destination_Type_ID).Select(MDH => new { MDH.MessageItem_ID });
            return Json(DestinationData, JsonRequestBehavior.AllowGet);
        }

    }
}