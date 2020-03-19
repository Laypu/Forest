using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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
            if (DES != null)
            {
                DES.Destination_Title = Server.HtmlDecode(db.Destination_Index.FirstOrDefault().Destination_Title);
                DES.Destination_Context = Server.HtmlDecode(db.Destination_Index.FirstOrDefault().Destination_Context);
                DES.Destination_ID = db.Destination_Index.FirstOrDefault().Destination_ID;
                return View(DES);
            }
            else 
            {
                if (ModelState.IsValid)
                {
                    DES.Destination_Context = "";
                    DES.Destination_Title = "";

                    db.Destination_Index.Add(DES);
                    db.SaveChanges();

                }
            }

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

        public ActionResult Upload(HttpPostedFileBase Img_File,string Img_FileName)
        {
            string Index_Img_Name = "";

            if (Img_File != null) //判斷是否有檔案
            {
                if (Img_File.ContentLength > 0)  //若檔案不為空檔案
                {
                    
                    // 如果UploadFiles文件夾不存在則先創建
                    
                    if (!Directory.Exists(Server.MapPath("~/UploadImage/Destination_Img/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/UploadImage/Destination_Img/"));
                        
                    }
                    
                    Index_Img_Name = Img_FileName;  //取得檔案名
                    var path = Path.Combine(Server.MapPath("~/UploadImage/Destination_Img/"), Index_Img_Name);  //取得本機檔案路徑


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
                //var OldDT = "";
                if (F_DES != null)
                {
                    for (var i = 0; i < F_DES.Length; i++)
                    {
                        db.Entry(F_DES[i]).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();
                TempData["Msg"] = "作業完成";
                return RedirectToAction("Destination");

            }
            else 
            {
                TempData["Msg"] = "作業失敗";
                return Json(new { success = true, message = "作業失敗" }, JsonRequestBehavior.AllowGet);
            }
        }




        // GET: webadmin/Destination_Index/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.Destination_Fare = db.Destination_Fare.Where(m =>m.Destination_Type_ID == id).ToList();
            ViewBag.ADDestinations = db.ADDestinations.Where(m => m.Destination_Type_ID == id).OrderBy(M=>M.Sort).ToList();
            ViewBag.TypeID = id;
            ViewBag.TypeName = db.F_Destination_Type.Where(m=>m.Destination_Type_ID==id).First().Destination_Type_Title1;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            F_Destination_Type Destination_Type = db.F_Destination_Type.Find(id);
            if (Destination_Type == null)
            {
                return HttpNotFound();
            }
            return View(Destination_Type);
        }

        // POST: webadmin/Destination_Index/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(F_Destination_Type Destination_Type, Destination_Fare[] DF ,ADDestination[] AD_Des)
        {
            if (ModelState.IsValid)
            {

                db.Entry(Destination_Type).State = EntityState.Modified;

                if (DF != null)
                {
                    for (var i = 0; i < DF.Length; i++)
                    {
                        db.Entry(DF[i]).State = EntityState.Modified;
                    }
                }
                if (AD_Des != null)
                {
                    for (var i = 0; i < AD_Des.Length; i++)
                    {
                        db.Entry(AD_Des[i]).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();
                TempData["Msg"] = "作業完成";
                return RedirectToAction("Destination");
            }
            TempData["Msg"] = "作業失敗";
            return Json(new { success = true, message = "作業失敗" }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Createtr(int? TypeID,string TypeName)
        {
            //ViewBag.Destination_Type_ID = new SelectList(db.F_Destination_Type, "Destination_Type_ID", "Destination_Type_Title1" + " " + "Destination_Type_Title2");
            ViewBag.DesTypeName = TypeName;
            ViewBag.DesTypeID = TypeID;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Createtr(Destination_Fare DFTR)
        {

            if (ModelState.IsValid)
            {
                db.Destination_Fare.Add(DFTR);
                db.SaveChanges();

            }
            TempData["Msg"] = "新增成功";
            return RedirectToAction("Edit",new {id = DFTR.Destination_Type_ID });
        }

        [HttpGet]
        public ActionResult Five_ThingsToDo_HashTag()
        {
            var DESH = db.F_HashTag_Type.ToList();
            return View(DESH);
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
        public ActionResult Create(F_Destination_Type F_DES, HttpPostedFileBase Img_File)
        {

            string Index_Img_Name = "";

            if (Img_File != null) //判斷是否有檔案
            {
                if (Img_File.ContentLength > 0)  //若檔案不為空檔案
                {

                    // 如果UploadFiles文件夾不存在則先創建

                    if (!Directory.Exists(Server.MapPath("~/UploadImage/Destination_Img/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/UploadImage/Destination_Img/"));

                    }

                    Index_Img_Name = Path.GetFileName(Img_File.FileName);  //取得檔案名
                    var path = Path.Combine(Server.MapPath("~/UploadImage/Destination_Img/"), Index_Img_Name);  //取得本機檔案路徑


                    
                     Img_File.SaveAs(path);
                    
                }
            }


            if (ModelState.IsValid)
            {
                db.F_Destination_Type.Add(new Models.F_Destination_Type
                {
                    Destination_Type_Title1 = F_DES.Destination_Type_Title1,
                    Destination_Type_Title2 = F_DES.Destination_Type_Title2,
                    Destination_Type_CreateDate = DateTime.Now,
                    Destination_Type_Description = F_DES.Destination_Type_Description,
                    Destination_Type_ImgName = F_DES.Destination_Type_ImgName,
                    Destination_Type_ImgDescription = F_DES.Destination_Type_ImgDescription,
                    Destination_Type_Link = "#"

                });
                db.SiteLists.Add(new Models.SiteList
                {
                    Sort = db.SiteLists.Max(SL=>SL.SiteList_ID)+1,
                    SiteList_Name_ch = "目的地",
                    SiteList_Name_en = F_DES.Destination_Type_Title1
                });
               
                db.SaveChanges();
                
            }

            return RedirectToAction("Destination");
        }
       
        // POST: webadmin/Destination_Index/Delete/5
        [HttpPost]
        public ActionResult Delete(int? chargeID)
        {
            using (ForestEntities db = new ForestEntities())
            {
                
                    Destination_Fare Destination_Fare = db.Destination_Fare.Find(chargeID);
                    db.Destination_Fare.Remove(Destination_Fare);
                    db.SaveChanges();
                

                return Json(new { success = true, message = "刪除成功" }, JsonRequestBehavior.AllowGet);


            }
            
        }

        [HttpPost]
        public ActionResult Destination_IndexDelete(int? Desid)
        {
            using (ForestEntities db = new ForestEntities())
            {
                if (db.Message_DesHash.Any(m => m.Destination_Type_ID == Desid))
                {
                    return Json(new { success = true, message = "刪除失敗,有文章與此目的地有關" }, JsonRequestBehavior.AllowGet);
                }
                else 
                {
                    F_Destination_Type f_Destination_Type = db.F_Destination_Type.Find(Desid);
                    db.F_Destination_Type.Remove(f_Destination_Type);
                    db.SaveChanges();
                    return Json(new { success = true, message = "刪除成功" }, JsonRequestBehavior.AllowGet);
                }
                    


            }

        }

        
        [HttpGet]
        public ActionResult CreateAD(int? TypeID, string TypeName , int? AdID)
        {
            ViewBag.ADid = AdID;
            ViewBag.DesTypeName = TypeName;
            ViewBag.DesTypeID = TypeID;
            if (AdID != null)
            {
                var ADC = db.ADDestinations.Find(AdID);
                //ViewBag.Destination_Type_ID = new SelectList(db.F_Destination_Type, "Destination_Type_ID", "Destination_Type_Title1" + " " + "Destination_Type_Title2");
                
                return View(ADC);
            }
            else
            {
                return View();
            }
            
        }

        [HttpPost]
        public ActionResult CreateAD(ADDestination AD_Des,HttpPostedFileBase Img_File, HttpPostedFileBase Video_File,string OldImg,string OldVideo)
        {
            
            string Index_Img_Name = "";
            var NewImgFile = "";
            if (Img_File != null) //判斷是否有檔案
            {
                if (Img_File.ContentLength > 0)  //若檔案不為空檔案
                {

                    // 如果UploadFiles文件夾不存在則先創建

                    if (!Directory.Exists(Server.MapPath("~/UploadImage/Destination_AD/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/UploadImage/Destination_AD/"));

                    }

                    Index_Img_Name = Path.GetFileName(Img_File.FileName);  //取得檔案名
                    NewImgFile = DateTime.Now.Ticks + "_" + Index_Img_Name;
                    var path = Path.Combine(Server.MapPath("~/UploadImage/Destination_AD/"), NewImgFile);  //取得本機檔案路徑



                    Img_File.SaveAs(path);

                }
            }

            string Index_Video_Name = "";
            var NewVideoFile = "";
            if (Video_File != null) //判斷是否有檔案
            {
                if (Video_File.ContentLength > 0)  //若檔案不為空檔案
                {

                    // 如果UploadFiles文件夾不存在則先創建

                    if (!Directory.Exists(Server.MapPath("~/UploadImage/Destination_AD/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/UploadImage/Destination_AD/"));

                    }

                    Index_Video_Name = Path.GetFileName(Video_File.FileName);  //取得檔案名
                    NewVideoFile = DateTime.Now.Ticks + "_" + Index_Video_Name;
                    var path = Path.Combine(Server.MapPath("~/UploadImage/Destination_AD/"), NewVideoFile);  //取得本機檔案路徑



                    Video_File.SaveAs(path);

                }
            }



            var sort = db.ADDestinations.Where(m => m.Destination_Type_ID == AD_Des.Destination_Type_ID).Count() + 1;
            if (ModelState.IsValid)
            {
                if (AD_Des.ID <= 0)
                {
                    db.ADDestinations.Add(new ADDestination
                    {

                        Destination_Type_ID = AD_Des.Destination_Type_ID,
                        StDate = AD_Des.StDate,
                        EdDate = AD_Des.EdDate,
                        AD_Name = AD_Des.AD_Name,
                        Img_Name_Ori = NewImgFile,
                        UploadVideoFileName = NewVideoFile,
                        Link_Href = AD_Des.Link_Href,
                        Link_Mode = AD_Des.Link_Mode,
                        Create_Date = DateTime.Now,
                        Sort = sort,
                        Enabled = true
                    });
                    
                    db.SaveChanges();
                    TempData["Msg"] = "新增成功";
                    

                }
                else
                {

                    db.Entry(AD_Des).State = EntityState.Modified;
                    if (NewImgFile == "")
                    {
                        db.ADDestinations.Where(m=>m.ID == AD_Des.ID).First().Img_Name_Ori = OldImg;
                    }
                    else
                    {
                        db.ADDestinations.Where(m => m.ID == AD_Des.ID).First().Img_Name_Ori = NewImgFile;
                       
                    }

                    if (NewVideoFile == "")
                    {
                        db.ADDestinations.Where(m => m.ID == AD_Des.ID).First().UploadVideoFileName = OldVideo;
                    }
                    else
                    {
                        db.ADDestinations.Where(m => m.ID == AD_Des.ID).First().UploadVideoFileName = NewVideoFile;
                        
                    }
                    db.SaveChanges();
                    TempData["Msg"] = "修改成功";
                    
                }
                 

            }
            return RedirectToAction("Edit", new { id = AD_Des.Destination_Type_ID });
        }

        [HttpPost]
        public ActionResult DeleteAD(List<int> ADids,int DESTypeID)
        {
            using (ForestEntities db = new ForestEntities())
            {
                foreach (var item1 in ADids)
                {
                    ADDestination adDestination = db.ADDestinations.Find(item1);
                    db.ADDestinations.Remove(adDestination);
                    db.SaveChanges();

                }
                if (db.ADDestinations.Where(m => m.Destination_Type_ID == DESTypeID).Count() > 0)
                {
                    var Newdb = db.ADDestinations.Where(m => m.Destination_Type_ID == DESTypeID).OrderBy(m => m.Sort);
                    var i = 1;
                    foreach (var item2 in Newdb)
                    {
                        item2.Sort = i;
                        i++;
                    }
                    db.SaveChanges();
                }

                return Json(new { success = true, message = "刪除成功" }, JsonRequestBehavior.AllowGet);


            }

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
