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
    public class Destination_MapLegendController : Controller
    {
        private ForestEntities db = new ForestEntities();

        // GET: webadmin/Destination_MapLegend
        public ActionResult Index()
        {
            return View(db.Destination_MapLegend.ToList());
        }

        // GET: webadmin/Destination_MapLegend/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination_MapLegend destination_MapLegend = db.Destination_MapLegend.Find(id);
            if (destination_MapLegend == null)
            {
                return HttpNotFound();
            }
            return View(destination_MapLegend);
        }

        // GET: webadmin/Destination_MapLegend/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: webadmin/Destination_MapLegend/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Destination_MAP_LEGEND_ID,Destination_MAP_LEGEND_Name,Destination_Img,Destination_Img_Mobile")] Destination_MapLegend destination_MapLegend)
        {
            if (ModelState.IsValid)
            {
                db.Destination_MapLegend.Add(destination_MapLegend);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(destination_MapLegend);
        }

        // GET: webadmin/Destination_MapLegend/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination_MapLegend destination_MapLegend = db.Destination_MapLegend.Find(id);
            if (destination_MapLegend == null)
            {
                return HttpNotFound();
            }
            return View(destination_MapLegend);
        }

        // POST: webadmin/Destination_MapLegend/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Destination_MAP_LEGEND_ID,Destination_MAP_LEGEND_Name,Destination_Img,Destination_Img_Mobile")] Destination_MapLegend destination_MapLegend)
        {
            if (ModelState.IsValid)
            {
                db.Entry(destination_MapLegend).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(destination_MapLegend);
        }

        // GET: webadmin/Destination_MapLegend/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination_MapLegend destination_MapLegend = db.Destination_MapLegend.Find(id);
            if (destination_MapLegend == null)
            {
                return HttpNotFound();
            }
            return View(destination_MapLegend);
        }

        // POST: webadmin/Destination_MapLegend/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Destination_MapLegend destination_MapLegend = db.Destination_MapLegend.Find(id);
            db.Destination_MapLegend.Remove(destination_MapLegend);
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
