using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Models;

namespace WebSiteProject.Areas.webadmin.Controllers
{
    public class F_Destination_TypeController : Controller
    {
        private ForestEntities db = new ForestEntities();

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
