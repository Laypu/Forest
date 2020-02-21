﻿using System;
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
    public class Destination_IndexController : Controller
    {
        ForestEntities db = new ForestEntities();

        // GET: webadmin/destination
        public ActionResult MainTitleAndContent()
        {
            
            Destination_Index DES = new Destination_Index();
            DES.Destination_Title = Server.HtmlDecode(db.Destination_Index.FirstOrDefault().Destination_Title);
            DES.Destination_Context = Server.HtmlDecode(db.Destination_Index.FirstOrDefault().Destination_Context);
            DES.Destination_ID = db.Destination_Index.FirstOrDefault().Destination_ID;
            return View(DES);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MainTitleAndContent_Edit(int _Destination_Index_ID,string _Destination_Index_Title, string _Destination_Index_Context)
        {

                var DES = db.Destination_Index.FirstOrDefault();
                DES.Destination_Title = _Destination_Index_Title;
                DES.Destination_Context = Server.HtmlEncode(_Destination_Index_Context);
                db.Entry(DES).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            
           
            
            return RedirectToAction("MainTitleAndContent");
        }

        
        [HttpGet]
        public ActionResult Destination(int? F_MenuType)
        {
            var DES = db.F_Destination_Type.ToList();
            Session["F_MenuType"] = F_MenuType;
            return View(DES);
        }

        public ActionResult Upload(HttpPostedFileBase Img_File)
        {
            string Index_Img_Name = "";

            if (Img_File != null) //判斷是否有檔案
            {
                if (Img_File.ContentLength > 0)  //若檔案不為空檔案
                {
                    string uploadPath = Server.MapPath("~/UploadImage/Destination_Img/");
                    // 如果UploadFiles文件夾不存在則先創建
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    Index_Img_Name = Path.GetFileName(Img_File.FileName);  //取得檔案名
                    var path = Path.Combine(Server.MapPath("~/UploadImage/Destination_Img/"), Index_Img_Name);  //取得本機檔案路徑




                    //若有重複則換名字_srart
                    while (System.IO.File.Exists(path))
                    {
                        Random rand = new Random();
                        Index_Img_Name = rand.Next().ToString() + "-" + Index_Img_Name;
                        path = Path.Combine(Server.MapPath("~/UploadImage/Destination_Img/"), Index_Img_Name);
                    }
                    Img_File.SaveAs(path);
                    //若有重複則換名字_end

                }

            }
            return RedirectToAction("Destination");





        }

        [HttpPost]
        public ActionResult Destination(F_Destination_Type[] F_DES,int? F_MenuType)
        {
            
            Session["F_MenuType"] = F_MenuType;


            //批次更改
            if (ModelState.IsValid)
            {
                //int Des_ID;
                for (int i = 0; i < F_DES.Length; i++)
                {

                    //Des_ID = F_DES[i].Destination_Type_ID;
                    //db.Entry(F_DES[i]).State = EntityState.Detached;
                    if (F_DES[i].Destination_Type_ImgName == null)
                    {
                        db.Entry(F_DES[i]).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        
                        db.Entry(F_DES[i]).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                    }

                }
                TempData["Msg"] = "作業完成";
                return RedirectToAction("Destination");

            }
            else 
            {
                TempData["Msg"] = "作業失敗";
                return RedirectToAction("Destination");
            }
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


        // GET: webadmin/Destination_Index
        public ActionResult Index()
        {
            return View(db.Destination_Index.ToList());
        }

        // GET: webadmin/Destination_Index/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination_Index Destination_Index = db.Destination_Index.Find(id);
            if (Destination_Index == null)
            {
                return HttpNotFound();
            }
            return View(Destination_Index);
        }

        // GET: webadmin/Destination_Index/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: webadmin/Destination_Index/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Destination_Type_ID,Destination_Type_Title1,Destination_Type_Title2,Destination_Type_CreateDate,Destination_Type_ImgName,Destination_Type_Link,Destination_Type_Description")] Destination_Index Destination_Index)
        {
            if (ModelState.IsValid)
            {
                db.Destination_Index.Add(Destination_Index);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Destination_Index);
        }

        // GET: webadmin/Destination_Index/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination_Index Destination_Index = db.Destination_Index.Find(id);
            if (Destination_Index == null)
            {
                return HttpNotFound();
            }
            return View(Destination_Index);
        }

        // POST: webadmin/Destination_Index/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Destination_Type_ID,Destination_Type_Title1,Destination_Type_Title2,Destination_Type_CreateDate,Destination_Type_ImgName,Destination_Type_Link,Destination_Type_Description")] Destination_Index Destination_Index)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Destination_Index).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Destination_Index);
        }

        // GET: webadmin/Destination_Index/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination_Index Destination_Index = db.Destination_Index.Find(id);
            if (Destination_Index == null)
            {
                return HttpNotFound();
            }
            return View(Destination_Index);
        }

        // POST: webadmin/Destination_Index/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Destination_Index Destination_Index = db.Destination_Index.Find(id);
            db.Destination_Index.Remove(Destination_Index);
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

        public ActionResult LoadData()
        {
            var datas = db.F_Destination_Type.AsEnumerable().Select(n => new
            {
                n.Destination_Type_ID,
                n.Destination_Type_Title1,
                n.Destination_Type_Title2,
                n.Destination_Type_Description,
                n.Destination_Type_CreateDate,
                n.Destination_Type_Link,
                n.Destination_Type_ImgName
            }).ToList();


            return Json(new { data = datas }, JsonRequestBehavior.AllowGet);

        }
    }
}
