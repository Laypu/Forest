using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utilities;
using WebSiteProject.Code;
using WebSiteProject.Models;

namespace WebSiteProject.Areas.webadmin.Controllers
{
    public class RecommendedtripsController : Controller
    {
        ForestEntities db = new ForestEntities();
        // GET: webadmin/Recommendedtrips
        public ActionResult RecommendedtripsIndex()
        {
            RecommendedTrips_Index RED = new RecommendedTrips_Index();
            //RED.RecommendedTrips_Index_Title = Server.HtmlDecode(db.RecommendedTrips_Index.FirstOrDefault().RecommendedTrips_Index_Title);
            RED.RecommendedTrips_Index_Content = Server.HtmlDecode(db.RecommendedTrips_Index.FirstOrDefault().RecommendedTrips_Index_Content);
            RED.RecommendedTrips_Index_ID = db.RecommendedTrips_Index.FirstOrDefault().RecommendedTrips_Index_ID;
            return View(RED);
        }
        [ValidateInput(false)]
        public ActionResult RecommendedtripsIndex_Edit(int RecommendedTrips_Index_ID, string RecommendedTrips_Index_Content)
        {
            RecommendedTrips_Index_ID = 1;
            var RED = db.RecommendedTrips_Index.Find(RecommendedTrips_Index_ID);
            RED.RecommendedTrips_Index_Content = Server.HtmlEncode(RecommendedTrips_Index_Content);
            db.Entry(RED).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["Msg"] = "修改成功";
            return RedirectToAction("RecommendedtripsIndex");
        }
        public ActionResult Recommendedtrips_item()
        {
            return View();
        }
        public ActionResult Recommendedtrips_Edit(string itemid="-1")
        { 
            var Destination_Typ = db.F_Destination_Type;
            var Day_ID_= db.RecommendedTrips_Day;
            var Destinations_ID = new List<SelectListItem>();
            foreach(var item in Destination_Typ)
            {
                Destinations_ID.Add(new SelectListItem()
                {
                    Text = item.Destination_Type_Title1+" "+item.Destination_Type_Title2,
                    Value = Convert.ToString(item.Destination_Type_ID),
                    Selected = false
                });
            }
            var Day_ID = new List<SelectListItem>();
            foreach (var item in Day_ID_)
            {
                Day_ID.Add(new SelectListItem()
                {
                    Text = item.RecommendedTrips_Day_Name ,
                    Value = Convert.ToString(item.RecommendedTrips_Day_Day_ID),
                    Selected = false
                });
            }
            //旅遊資訊標籤
            ViewBag.HashTag = db.F_HashTag_Type.ToList();
            var model = db.RecommendedTrips.Where(p => p.RecommendedTrips_ID.ToString() == itemid);
            if(model.FirstOrDefault() !=null)
            { 
              if (model.FirstOrDefault().RecommendedTrips_Day_ID != null)
              {
                Day_ID.Where(q => q.Value == Convert.ToString(model.FirstOrDefault().RecommendedTrips_Day_ID)).First().Selected = true;
              }
              if (model.FirstOrDefault().RecommendedTrips_Destinations_ID != null)
              {
                Destinations_ID.Where(q => q.Value == Convert.ToString(model.FirstOrDefault().RecommendedTrips_Destinations_ID)).First().Selected = true;
              }
            }
            ViewBag.RecommendedTrips_Day_ID = Day_ID;
            ViewBag.RecommendedTrips_Destinations_ID = Destinations_ID;
            var RE = new RecommendedTrip();
            if (model.Count()>0)
            {
                var ReD = model.First();
                RE.RecommendedTrips_ID = ReD.RecommendedTrips_ID;
                RE.RecommendedTrips_Day_ID = ReD.RecommendedTrips_Day_ID;
                RE.RecommendedTrips_Img = ReD.RecommendedTrips_Img;
                RE.RecommendedTrips_Title = ReD.RecommendedTrips_Title;
                RE.RecommendedTrips_Content = ReD.RecommendedTrips_Content;
                RE.RecommendedTrips_Location = ReD.RecommendedTrips_Location;
                RE.RecommendedTrips_HtmContent = ReD.RecommendedTrips_HtmContent;
            }
            else
            {
                RE.RecommendedTrips_ID = -1;
                RE.RecommendedTrips_Day_ID = null;
                RE.RecommendedTrips_Img = "";
                RE.RecommendedTrips_Title = "";
                RE.RecommendedTrips_Content = "";
                RE.RecommendedTrips_Location = "";
                RE.RecommendedTrips_HtmContent = "";
            }
            //旅遊資訊關聯
            ViewBag.Sub_HashTag = db.RecommendedTrips_HashTag_Type.Where(s => s.RecommendedTrips_ID.ToString() == itemid).ToList();
            return View(RE);
        }
        #region SaveItem
        [HttpPost]
        public ActionResult SaveItem(RecommendedTrip recommendedTrip,HttpPostedFileBase fileImag, List<int> HashTag_Type)
        {
            
                if (fileImag != null)
                {
                    var root = Request.PhysicalApplicationPath;
                    var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                    if (uploadfilepath.IsNullorEmpty())
                    {
                        uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
                    }
                    var newfilename = DateTime.Now.Ticks + "_" + recommendedTrip.RecommendedTrips_Img;
                    var path = root + "\\UploadImage\\RecommendedTrips\\" + newfilename;
                    if (System.IO.Directory.Exists(root + "\\UploadImage\\RecommendedTrips\\") == false)
                    {
                        System.IO.Directory.CreateDirectory(root + "\\UploadImage\\RecommendedTrips\\");
                    }
                    fileImag.SaveAs(path);
                    recommendedTrip.RecommendedTrips_Img = newfilename;
                }
                recommendedTrip.RecommendedTrips_HtmContent = HttpUtility.UrlDecode(recommendedTrip.RecommendedTrips_HtmContent);
                recommendedTrip.RecommendedTrips_Title = HttpUtility.UrlDecode(recommendedTrip.RecommendedTrips_Title);
                recommendedTrip.RecommendedTrips_Content= HttpUtility.UrlDecode(recommendedTrip.RecommendedTrips_Content);
            if (recommendedTrip.RecommendedTrips_ID.ToString() == "-1")
            {
                RecommendedTrip recom = new RecommendedTrip() {
                  RecommendedTrips_Day_ID = recommendedTrip.RecommendedTrips_Day_ID,
                  RecommendedTrips_Destinations_ID=recommendedTrip.RecommendedTrips_Destinations_ID,
                  RecommendedTrips_Img=recommendedTrip.RecommendedTrips_Img,
                    RecommendedTrips_Img_Description = recommendedTrip.RecommendedTrips_Img_Description,
                    RecommendedTrips_Title = recommendedTrip.RecommendedTrips_Title,
                    RecommendedTrips_Content = recommendedTrip.RecommendedTrips_Content,
                    RecommendedTrips_Location = recommendedTrip.RecommendedTrips_Location,
                    RecommendedTrips_HtmContent = recommendedTrip.RecommendedTrips_HtmContent,
                };
                db.RecommendedTrips.Add(recom);
                var r= db.SaveChanges();
                if(r>0)
                {

                    //增加欄位
                    foreach (var item in HashTag_Type)
                    {
                        RecommendedTrips_HashTag_Type HashTag = new RecommendedTrips_HashTag_Type();
                        HashTag.RecommendedTrips_ID = recommendedTrip.RecommendedTrips_ID;
                        HashTag.HashTag_Type_ID = item;
                        db.RecommendedTrips_HashTag_Type.Add(HashTag);
                        db.SaveChanges();
                    };
                    return Json("成功");
                }
                return Json("失敗");
            }
            else
            {
                var old_SizeChart = recommendedTrip.RecommendedTrips_Img;
                if (old_SizeChart != null)
                {
                    string fullpath = Request.MapPath("~/UploadImage/RecommendedTrips/" + recommendedTrip.RecommendedTrips_Img);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                }
                db.Entry(recommendedTrip).State = System.Data.Entity.EntityState.Modified;
                var r = db.SaveChanges();
                if (r > 0) //result為itemID >0為新增成功
                {
                    var Old_HashTag = db.RecommendedTrips_HashTag_Type.Where(a => a.RecommendedTrips_ID == recommendedTrip.RecommendedTrips_ID).ToList(); ;
                    //先全部刪除舊的
                    foreach (var item in Old_HashTag)
                    {
                        var Old_HashTag_Item = db.RecommendedTrips_HashTag_Type.Find(item.RecommendedTrips_ID);
                        db.Entry(Old_HashTag_Item).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();
                    };

                    //新增加
                    foreach (var item in HashTag_Type)
                    {
                        RecommendedTrips_HashTag_Type Sub_HashTag =new RecommendedTrips_HashTag_Type();
                        Sub_HashTag.RecommendedTrips_ID = recommendedTrip.RecommendedTrips_ID;
                        Sub_HashTag.HashTag_Type_ID = item;
                        db.RecommendedTrips_HashTag_Type.Add(Sub_HashTag);
                        db.SaveChanges();
                    };
                    return Json("成功");
                };
                return Json("失敗");
            }
            
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
                var root = Request.PhysicalApplicationPath + "/UploadImage/RecommendedTrips/";
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
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/RecommendedTrips/" + filename);
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
                var root = Request.PhysicalApplicationPath + "/UploadImage/RecommendedTrips/";
                if (System.IO.Directory.Exists(root) == false)
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                upload.SaveAs(root + filename);
                imageUrl = Url.Content((Request.ApplicationPath == "/" ? "" : Request.ApplicationPath) + "/UploadImage/RecommendedTrips/" + filename);
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
    }
}