using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Models;

namespace WebSiteProject.Controllers
{
    public class Destination_IndexController : Controller
    {
        private ForestEntities db = new ForestEntities();

        // GET: Destination_Index
        public ActionResult Index()
        {
            return View(db.Destination_Index.ToList());
        }

        // GET: Destination_Index/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination_Index destination_Index = db.Destination_Index.Find(id);
            if (destination_Index == null)
            {
                return HttpNotFound();
            }
            return View(destination_Index);
        }

        // GET: Destination_Index/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Destination_Index/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Destination_ID,Destination_Title,Destination_Context,Destination_Img,Destination_Img_Mobile")] Destination_Index destination_Index)
        {
            if (ModelState.IsValid)
            {
                db.Destination_Index.Add(destination_Index);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(destination_Index);
        }

        // GET: Destination_Index/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination_Index destination_Index = db.Destination_Index.Find(id);
            if (destination_Index == null)
            {
                return HttpNotFound();
            }
            return View(destination_Index);
        }

        // POST: Destination_Index/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Destination_ID,Destination_Title,Destination_Context,Destination_Img,Destination_Img_Mobile")] Destination_Index destination_Index)
        {
            if (ModelState.IsValid)
            {
                db.Entry(destination_Index).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(destination_Index);
        }

        // GET: Destination_Index/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination_Index destination_Index = db.Destination_Index.Find(id);
            if (destination_Index == null)
            {
                return HttpNotFound();
            }
            return View(destination_Index);
        }

        // POST: Destination_Index/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Destination_Index destination_Index = db.Destination_Index.Find(id);
            db.Destination_Index.Remove(destination_Index);
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
