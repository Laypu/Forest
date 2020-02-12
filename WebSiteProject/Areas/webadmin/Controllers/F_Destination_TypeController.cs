using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Models;

namespace WebSiteProject.Areas.webadmin.Controllers
{
    public class F_Destination_TypeController : Controller
    {
        ForestEntities db = new ForestEntities();

        // GET: webadmin/F_Thingtodo
        public ActionResult MainTitleAndContent()
        {
            F_Destination_Type DES = new F_Destination_Type();
            //DES.F_Thingtodo_Index_Title = Server.HtmlDecode(db.F_Thingtodo_Index.FirstOrDefault().F_Thingtodo_Index_Title);
            //DES.F_Thingtodo_Index_Content = Server.HtmlDecode(db.F_Thingtodo_Index.FirstOrDefault().F_Thingtodo_Index_Content);
            //DES.F_Thingtodo_Index_ID = db.F_Thingtodo_Index.FirstOrDefault().F_Thingtodo_Index_ID;
            return View(DES);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MainTitleAndContent_Edit(int _Thingtodo_Index_ID, string _Thingtodo_Index_Title, string _Thingtodo_Index_Content)
        {
            var DES = db.F_Thingtodo_Index.Find(_Thingtodo_Index_ID);
            DES.F_Thingtodo_Index_Title = _Thingtodo_Index_Title;
            DES.F_Thingtodo_Index_Content = Server.HtmlEncode(_Thingtodo_Index_Content);
            db.Entry(DES).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("MainTitleAndContent");
        }

        [HttpGet]
        public ActionResult Five_Thingstodo(int? F_MenuType)
        {
            var DES = db.F_Thingtodo_Type.ToList();

            Session["F_MenuType"] = F_MenuType;

            return View(DES);
        }

        [HttpPost]
        public ActionResult Five_Thingstodo_Edit(int _Thingtodo_Type_ID, string _Thingtodo_Type_Title1, string _Thingtodo_Type_Title2, string _Thingtodo_Type_Description, HttpPostedFileBase Index_Img_File)

        {
            string Index_Img_Name = "";


            if (Index_Img_File != null) //判斷是否有檔案
            {
                if (Index_Img_File.ContentLength > 0)  //若檔案不為空檔案
                {
                    string uploadPath = Server.MapPath("~/UploadImage/ThingsToDo_Img/");
                    // 如果UploadFiles文件夹不存在则先创建
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    Index_Img_Name = Path.GetFileName(Index_Img_File.FileName);  //取得檔案名
                    var path = Path.Combine(Server.MapPath("~/UploadImage/ThingsToDo_Img/"), Index_Img_Name);  //取得本機檔案路徑




                    //若有重複則換名字_srart
                    while (System.IO.File.Exists(path))
                    {
                        Random rand = new Random();
                        Index_Img_Name = rand.Next().ToString() + "-" + Index_Img_Name;
                        path = Path.Combine(Server.MapPath("~/UploadImage/ThingsToDo_Img/"), Index_Img_Name);
                    }
                    Index_Img_File.SaveAs(path);
                    //若有重複則換名字_end


                    db.F_Thingtodo_Type.Where(t => t.F_Thingtodo_Type_ID == _Thingtodo_Type_ID).FirstOrDefault().F_Thingtodo_Type_ImgName = Index_Img_Name;
                    db.SaveChanges();
                }
            }


            try
            {
                var DES = db.F_Thingtodo_Type.Find(_Thingtodo_Type_ID);
                DES.F_Thingtodo_Type_Title1 = _Thingtodo_Type_Title1;
                DES.F_Thingtodo_Type_Title2 = _Thingtodo_Type_Title2;
                DES.F_Thingtodo_Type_Description = _Thingtodo_Type_Description;
                db.Entry(DES).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                TempData["Msg"] = "作業失敗";
                return RedirectToAction("Five_Thingstodo");
            }
            TempData["Msg"] = "作業完成";

            return RedirectToAction("Five_Thingstodo");
        }


        [HttpGet]
        public ActionResult Five_ThingsToDo_HashTag()
        {
            var DESH = db.F_HashTag_Type.ToList();
            return View(DESH);
        }

        [HttpPost]
        public ActionResult Five_ThingsToDo_HashTag_Edit(int? HashTag_ID, string HashTag_Name, string HashTag_Link, int IsCreate = 0)
        {
            try
            {
                if (HashTag_Name.Trim() == "")
                {
                    TempData["Msg"] = "名稱不得為空值";
                    return RedirectToAction("Five_ThingsToDo_HashTag");
                }
                else
                {
                    if (IsCreate != 1)
                    {
                        var DESH = db.F_HashTag_Type.Find(HashTag_ID);
                        DESH.HashTag_Type_Name = HashTag_Name;
                        DESH.HashTag_Type_Link = HashTag_Link;
                        db.Entry(DESH).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        F_HashTag_Type DESH = new F_HashTag_Type();
                        DESH.HashTag_Type_Name = HashTag_Name;
                        DESH.HashTag_Type_Link = HashTag_Link;
                        db.F_HashTag_Type.Add(DESH);
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                TempData["Msg"] = "失敗";
                return RedirectToAction("Five_ThingsToDo_HashTag");
            }
            TempData["Msg"] = "成功";
            return RedirectToAction("Five_ThingsToDo_HashTag");
        }

        public ActionResult Five_ThingsToDo_HashTag_Delete(int id)
        {
            var DESH = db.F_HashTag_Type.Find(id);
            db.Entry(DESH).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("Five_ThingsToDo_HashTag");
        }
    

    // GET: webadmin/F_Destination_Type
    public ActionResult Index()
        {
            return View(db.F_Destination_Type.ToList());
        }

        // GET: webadmin/F_Destination_Type/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            F_Destination_Type f_Destination_Type = db.F_Destination_Type.Find(id);
            if (f_Destination_Type == null)
            {
                return HttpNotFound();
            }
            return View(f_Destination_Type);
        }

        // GET: webadmin/F_Destination_Type/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: webadmin/F_Destination_Type/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Destination_Type_ID,Destination_Type_Title1,Destination_Type_Title2,Destination_Type_CreateDate,Destination_Type_ImgName,Destination_Type_Link,Destination_Type_Description")] F_Destination_Type f_Destination_Type)
        {
            if (ModelState.IsValid)
            {
                db.F_Destination_Type.Add(f_Destination_Type);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(f_Destination_Type);
        }

        // GET: webadmin/F_Destination_Type/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            F_Destination_Type f_Destination_Type = db.F_Destination_Type.Find(id);
            if (f_Destination_Type == null)
            {
                return HttpNotFound();
            }
            return View(f_Destination_Type);
        }

        // POST: webadmin/F_Destination_Type/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Destination_Type_ID,Destination_Type_Title1,Destination_Type_Title2,Destination_Type_CreateDate,Destination_Type_ImgName,Destination_Type_Link,Destination_Type_Description")] F_Destination_Type f_Destination_Type)
        {
            if (ModelState.IsValid)
            {
                db.Entry(f_Destination_Type).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(f_Destination_Type);
        }

        // GET: webadmin/F_Destination_Type/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            F_Destination_Type f_Destination_Type = db.F_Destination_Type.Find(id);
            if (f_Destination_Type == null)
            {
                return HttpNotFound();
            }
            return View(f_Destination_Type);
        }

        // POST: webadmin/F_Destination_Type/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            F_Destination_Type f_Destination_Type = db.F_Destination_Type.Find(id);
            db.F_Destination_Type.Remove(f_Destination_Type);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
