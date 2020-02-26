using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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