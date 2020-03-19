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
    public class Destination_MapLegendController : Controller
    {
        private ForestEntities db = new ForestEntities();

        // GET: webadmin/Destination_MapLegend
        public ActionResult Index()
        {
            return View(db.Destination_MapLegend.ToList());
        }

        [HttpPost]
        public ActionResult Index(Destination_MapLegend[] DML, int? F_MenuType)
        {

            Session["F_MenuType"] = F_MenuType;


            //批次更改
            if (ModelState.IsValid)
            {
                //int Des_ID;
                for (int i = 0; i < DML.Length; i++)
                {
                    
                        db.Entry(DML[i]).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                    
                }
                TempData["Msg"] = "作業完成";
                return RedirectToAction("Index");

            }
            else
            {
                TempData["Msg"] = "作業失敗";
                return RedirectToAction("Index");
            }
        }



        public ActionResult Upload(HttpPostedFileBase Img_File)
        {
            string Index_Img_Name = "";

            if (Img_File != null) //判斷是否有檔案
            {
                if (Img_File.ContentLength > 0)  //若檔案不為空檔案
                {

                    // 如果UploadFiles文件夾不存在則先創建

                    if (!Directory.Exists(Server.MapPath("~/UploadImage/MapLEGEND_Img/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/UploadImage/MapLEGEND_Img/"));

                    }

                    Index_Img_Name = Path.GetFileName(Img_File.FileName);  //取得檔案名
                    var NewImgName = DateTime.Now.Ticks + "_" + Index_Img_Name;
                    var path = Path.Combine(Server.MapPath("~/UploadImage/MapLEGEND_Img/"), NewImgName);  //取得本機檔案路徑


                    //若有重複則不儲存
                    if (System.IO.File.Exists(path))
                    {
                        //Random rand = new Random();
                        //Index_Img_Name = rand.Next().ToString() + "-" + Index_Img_Name;
                        //path = Path.Combine(Server.MapPath("~/UploadImage/ThingsToDo_Img/"), Index_Img_Name);
                    }
                    else
                    {
                        Img_File.SaveAs(path);
                    }

                    //若有重複則換名字_end

                }

            }
            return RedirectToAction("Index");





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
