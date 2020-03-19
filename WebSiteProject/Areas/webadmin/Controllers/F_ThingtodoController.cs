using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Models;

namespace WebSiteProject.Areas.webadmin.Controllers
{
    public class F_ThingtodoController : Controller
    {

        ForestEntities db = new ForestEntities();

        // GET: webadmin/F_Thingtodo
        public ActionResult MainTitleAndContent()
        {
            F_Thingtodo_Index TTD = new F_Thingtodo_Index();
            TTD.F_Thingtodo_Index_Title = Server.HtmlDecode(db.F_Thingtodo_Index.FirstOrDefault().F_Thingtodo_Index_Title);
            TTD.F_Thingtodo_Index_Content = Server.HtmlDecode(db.F_Thingtodo_Index.FirstOrDefault().F_Thingtodo_Index_Content);
            TTD.F_Thingtodo_Index_ID = db.F_Thingtodo_Index.FirstOrDefault().F_Thingtodo_Index_ID;
            return View(TTD);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MainTitleAndContent_Edit(int _Thingtodo_Index_ID, string _Thingtodo_Index_Title, string _Thingtodo_Index_Content)
        {
            _Thingtodo_Index_ID = 1;
            var TTD = db.F_Thingtodo_Index.Find(_Thingtodo_Index_ID);
            TTD.F_Thingtodo_Index_Title = _Thingtodo_Index_Title;
            TTD.F_Thingtodo_Index_Content = Server.HtmlEncode(_Thingtodo_Index_Content);
            db.Entry(TTD).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("MainTitleAndContent");
        }

        [HttpGet]
        public ActionResult Five_Thingstodo(int? F_MenuType)
        {
            var TTD = db.F_Thingtodo_Type.ToList();

            Session["F_MenuType"] = F_MenuType;

            return View(TTD);
        }

        
        //上傳檔案
        public ActionResult Upload(HttpPostedFileBase Img_File)
        {
            string Index_Img_Name = "";

            if (Img_File != null) //判斷是否有檔案
            {
                if (Img_File.ContentLength > 0)  //若檔案不為空檔案
                {
                    string uploadPath = Server.MapPath("~/UploadImage/ThingsToDo_Img/");
                    // 如果UploadFiles文件夾不存在則先創建
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    
                    Index_Img_Name = Path.GetFileName(Img_File.FileName);  //取得檔案名
                    
                    var NewImgFile = DateTime.Now.Ticks + "_" + Index_Img_Name;
                    var path = Path.Combine(Server.MapPath("~/UploadImage/ThingsToDo_Img/"), NewImgFile);  //取得本機檔案路徑


                    //若有重複則不儲存
                    if (System.IO.File.Exists(path))
                    {
                        //Random rand = new Random();
                        //Index_Img_Name = rand.Next().ToString() + "-" + Index_Img_Name;
                        //path = Path.Combine(Server.MapPath("~/UploadImage/ThingsToDo_Img/"), Index_Img_Name);
                    }
                    else {
                        Img_File.SaveAs(path);
                    }
                    
                    //若有重複則換名字_end

                }

            }
            return RedirectToAction("Five_Thingstodo");





        }

        [HttpPost]
        public ActionResult Five_Thingstodo(F_Thingtodo_Type[] F_things ,int? F_MenuType)
        {
            Session["F_MenuType"] = F_MenuType;

                //批次更改
                if (ModelState.IsValid)
            {
                //int Des_ID;
                for (int i = 0; i < F_things.Length; i++)
                {

                    if (F_things[i].F_Thingtodo_Type_ImgName == null)
                    {
                        db.Entry(F_things[i]).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {

                        db.Entry(F_things[i]).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                }
                TempData["Msg"] = "作業完成";
                return RedirectToAction("Five_Thingstodo");

            }
            else
            {
                TempData["Msg"] = "作業失敗";
                return RedirectToAction("Five_Thingstodo");
            }
        
        }


        [HttpGet]
        public ActionResult Five_ThingsToDo_HashTag()
        {
            var TTDH = db.F_HashTag_Type.ToList();
            return View(TTDH);
        }        

        [HttpPost]
        public ActionResult Five_ThingsToDo_HashTag_Edit(int? HashTag_ID,string HashTag_Name,string HashTag_Link, int IsCreate = 0)
        {
            try
            {
                if(HashTag_Name.Trim() == "")
                {
                    TempData["Msg"] = "名稱不得為空值";
                    return RedirectToAction("Five_ThingsToDo_HashTag");
                }
                else
                {
                    //判斷
                    if (IsCreate != 1)
                    {
                        var TTDH = db.F_HashTag_Type.Find(HashTag_ID);
                        TTDH.HashTag_Type_Name = HashTag_Name;
                        TTDH.HashTag_Type_Link = HashTag_Link;
                        db.Entry(TTDH).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        F_HashTag_Type TTDH = new F_HashTag_Type();
                        TTDH.HashTag_Type_Name = HashTag_Name;
                        TTDH.HashTag_Type_Link = HashTag_Link;
                        db.F_HashTag_Type.Add(TTDH);
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
            var TTDH = db.F_HashTag_Type.Find(id);
            db.Entry(TTDH).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("Five_ThingsToDo_HashTag");
        }
    }
}