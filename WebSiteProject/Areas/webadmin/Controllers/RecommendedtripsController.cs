using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utilities;
using ViewModels;
using WebSiteProject.Code;
using WebSiteProject.Models;
using WebSiteProject.Models.F_ViewModels;

namespace WebSiteProject.Areas.webadmin.Controllers
{
    public class RecommendedtripsController : Controller
    {
        ForestEntities db = new ForestEntities();
        // GET: webadmin/Recommendedtrips
        public ActionResult Index()
        {
            return View();
        }
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
        public ActionResult Recommendedtrips_item(string Dstination_typ = "", string Day_Id = "", string F_HashTag="")
        {
            var Destination_Type = db.F_Destination_Type;
            var Day_ID_ = db.RecommendedTrips_Day;
            var F_HashTag_Type_ = db.F_HashTag_Type;
            var Destinations_ID = new List<SelectListItem>();
            foreach (var item in Destination_Type)
            {
                Destinations_ID.Add(new SelectListItem()
                {
                    Text = item.Destination_Type_Title1 + " " + item.Destination_Type_Title2,
                    Value = Convert.ToString(item.Destination_Type_ID),
                    Selected = false
                });
            }
            var Day_ID = new List<SelectListItem>();
            foreach (var item in Day_ID_)
            {
                Day_ID.Add(new SelectListItem()
                {
                    Text = item.RecommendedTrips_Day_Name,
                    Value = Convert.ToString(item.RecommendedTrips_Day_ID),
                    Selected = false
                });
            }
            var HashTag_Type = new List<SelectListItem>();
            foreach (var item in F_HashTag_Type_)
            {
                HashTag_Type.Add(new SelectListItem()
                {
                    Text = item.HashTag_Type_Name,
                    Value = Convert.ToString(item.HashTag_Type_ID),
                    Selected = false
                });
            }
             if (Day_Id != "")
             {
                Day_ID.Where(q => q.Value == Day_Id).First().Selected = true;
            }
            if (F_HashTag != "")
                {
                HashTag_Type.Where(q => q.Value == F_HashTag).First().Selected = true;
            }
            if (Dstination_typ != "")
            {
                Destinations_ID.Where(q => q.Value == Dstination_typ).First().Selected = true;
            }
            ViewBag.RecommendedTrips_Day_ID = Day_ID;
            ViewBag.RecommendedTrips_Destinations_ID = Destinations_ID;
            ViewBag.F_HashTag_Type = HashTag_Type;
            //var searchmoel =new  List<V_RecommendedTripsForWebadmin>();
            var mode = new List<RecommendedSearchModel>();
            mode = db.RecommendedTrips.Select(p => new RecommendedSearchModel { RecommendedTrips_ID = p.RecommendedTrips_ID, RecommendedTrips_Title = p.RecommendedTrips_Title, RecommendedTrips_Day_Name = p.RecommendedTrips_Day.RecommendedTrips_Day_Name, RecommendedTrips_Day_ID = p.RecommendedTrips_Day.RecommendedTrips_Day_ID, RecommendedTrips_Destinations_ID = p.RecommendedTrips_Destinations_ID,sort=p.Sort,starDate=p.RecommendedTrips_StarDay,EndDate=p.RecommendedTrips_EndDay }).OrderBy(p=>p.sort).ToList();
            //mode = db.V_RecommendedTripsForWebadmin.GroupBy(o=>o.RecommendedTrips_ID).Select(p=>new RecommendedSearchModel {RecommendedTrips_ID=p.Key, RecommendedTrips_Title=p.FirstOrDefault().RecommendedTrips_Title, RecommendedTrips_Day_Name=p.FirstOrDefault().RecommendedTrips_Day_Name,HashTag_Type_ID=p.FirstOrDefault().HashTag_Type_ID,RecommendedTrips_Day_ID=p.FirstOrDefault().RecommendedTrips_Day_ID,RecommendedTrips_Destinations_ID=p.FirstOrDefault().RecommendedTrips_Destinations_ID}).ToList();
            if (Day_Id != "")
            {
                mode = mode.Where(p => p.RecommendedTrips_Day_ID == Convert.ToInt32(Day_Id)).ToList();
            }
            if (Dstination_typ != "")
            {
                mode = mode.Where(p => p.RecommendedTrips_Destinations_ID == Convert.ToInt32(Dstination_typ)).ToList();
            }
            if (F_HashTag != "")
            {
                mode = (from t1 in mode
                        join t2 in db.RecommendedTrips_HashTag_Type on t1.RecommendedTrips_ID equals t2.RecommendedTrips_ID
                        where t2.HashTag_Type_ID== Convert.ToInt32(F_HashTag)
                        select new RecommendedSearchModel { RecommendedTrips_ID=t1.RecommendedTrips_ID, RecommendedTrips_Title=t1.RecommendedTrips_Title, RecommendedTrips_Day_Name=t1.RecommendedTrips_Day_Name, RecommendedTrips_Day_ID= t1.RecommendedTrips_Day_ID, HashTag_Type_ID = t2.HashTag_Type_ID, RecommendedTrips_Destinations_ID=t1.RecommendedTrips_Destinations_ID, sort = t1.sort, starDate = t1.starDate, EndDate = t1.EndDate }
                        ).OrderBy(p => p.sort).ToList();
            }
            //var mode = new RecommendedSearchModel();        
            //foreach (var item in searchmoel)
            //{
            //    mode.RecommendedTrips_ID = item.RecommendedTrips_ID;
            //    mode.RecommendedTrips_Title = item.RecommendedTrips_Title;
            //    mode.RecommendedTrips_Day_Name = item.RecommendedTrips_Day_Name;
            //    mod.Add(mode);
            //}
            return View(mode);
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
                    Value = Convert.ToString(item.RecommendedTrips_Day_ID),
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
                RE.RecommendedTrips_Img_Description = ReD.RecommendedTrips_Img_Description;
                RE.RecommendedTrips_UploadFilePath = ReD.RecommendedTrips_UploadFilePath;
                RE.RecommendedTrips_UploadFileDesc = ReD.RecommendedTrips_UploadFileDesc;
                RE.RecommendedTrips_UploadFileName = ReD.RecommendedTrips_UploadFileName;
                RE.RecommendedTrips_StarDay = ReD.RecommendedTrips_StarDay;
                RE.RecommendedTrips_EndDay = ReD.RecommendedTrips_EndDay;
                RE.Sort = ReD.Sort;
                RE.InFront = ReD.InFront;
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
                RE.RecommendedTrips_UploadFilePath = "";
                RE.RecommendedTrips_UploadFileDesc = "";
                RE.RecommendedTrips_UploadFileName = "";
            }
            //旅遊資訊關聯
            var seodata = db.RecommendedTrips_HashTag.Where(p => p.RecommendedTrips_Id.ToString() == itemid);
            ViewBag.Sub_HashTag = db.RecommendedTrips_HashTag_Type.Where(s => s.RecommendedTrips_ID.ToString() == itemid).ToList();
            ViewBag.RecommendedTrips_HashTahg = seodata.Count() == 0 ? new string[10] : new string[] {
                        seodata.First().RecommendedTrips_keyword0,seodata.First().RecommendedTrips_keyword1,seodata.First().RecommendedTrips_keyword2,seodata.First().RecommendedTrips_keyword3,seodata.First().RecommendedTrips_keyword4
                    ,seodata.First().RecommendedTrips_keyword5,seodata.First().RecommendedTrips_keyword6,seodata.First().RecommendedTrips_keyword7,seodata.First().RecommendedTrips_keyword8,seodata.First().RecommendedTrips_keyword9};
            return View(RE);
        }
        #region RecommendedTrip_TravelItem
        public ActionResult RecommendedTrip_Travel_Item(int? RID)
        {
            //var RecommendedTrip = db.RecommendedTrips;
            //var RecommendedTrip_ID = new List<SelectListItem>();
            //foreach (var item in RecommendedTrip)
            //{
            //    RecommendedTrip_ID.Add(new SelectListItem()
            //    {
            //        Text = item.RecommendedTrips_Title,
            //        Value = Convert.ToString(item.RecommendedTrips_ID),
            //        Selected = false
            //    });
            //}
            //if (RecommendedTripID != "")
            //{
            //    RecommendedTrip_ID.Where(q => q.Value == RecommendedTripID).First().Selected = true;
            //}
            //ViewBag.RecommendedTrips_ID = RecommendedTrip_ID;
               var  model = db.RecommendedTrip_Travel.Where(P => P.RecommendedTrip_ID == RID).ToList();
            return PartialView (model);
        }
        #endregion
        #region RecommendedTrip_Trave_Edit
        public ActionResult RecommendedTrip_Trave_Edit(int? Rid, string id="-1")
        {
            var RecommendedTrip = db.RecommendedTrips;
            var RecommendedTrip_ID = new List<SelectListItem>();
            foreach (var item in RecommendedTrip)
            {
                RecommendedTrip_ID.Add(new SelectListItem()
                {
                    Text = item.RecommendedTrips_Title,
                    Value = Convert.ToString(item.RecommendedTrips_ID),
                    Selected = false
                });
            }
            var model = db.RecommendedTrip_Travel.Where(p => p.RecommendedTrip_Travel_ID.ToString() == id);
            if (model.FirstOrDefault() != null)
            {
                if (model.FirstOrDefault().RecommendedTrip_ID != null)
                {
                    RecommendedTrip_ID.Where(q => q.Value == Convert.ToString(model.FirstOrDefault().RecommendedTrip_ID)).First().Selected = true;
                }
            }
            ViewBag.RecommendedTrips_ID = RecommendedTrip_ID;
            var RE = new RecommendedTrip_Travel();
            if (model.Count() > 0)
            {
                var ReD = model.First();
                RE.RecommendedTrip_Travel_ID = ReD.RecommendedTrip_Travel_ID;
                RE.RecommendedTrip_ID = ReD.RecommendedTrip_ID;
                RE.RecommendedTrip_Travel_Img = ReD.RecommendedTrip_Travel_Img;
                RE.RecommendedTrip_Travel_Title = ReD.RecommendedTrip_Travel_Title;
                RE.RecommendedTrip_Travel_Content = ReD.RecommendedTrip_Travel_Content;
                //RE.RecommendedTrip_Travel_Img_Description = ReD.RecommendedTrip_Travel_Img_Description;
                RE.RecommendedTrip_Travel_Link = ReD.RecommendedTrip_Travel_Link;
            }
            else
            {
                RE.RecommendedTrip_Travel_ID = -1;
                RE.RecommendedTrip_ID = Rid;
                RE.RecommendedTrip_Travel_Title = "";
                RE.RecommendedTrip_Travel_Content = "";
                //RE.RecommendedTrip_Travel_Img_Description = "";
                RE.RecommendedTrip_Travel_Img = "";
                RE.RecommendedTrip_Travel_Link = "";
            }
            return View(RE);
        }
        #endregion
        #region SaveItem_Trave
        [HttpPost]
        public ActionResult SaveItem_Trave(RecommendedTrip_Travel recommendedTrip_Travel, HttpPostedFileBase fileImag)
        {

            if (fileImag != null)
            {
                var root = Request.PhysicalApplicationPath;
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
                }
                var newfilename = DateTime.Now.Ticks + "_" + fileImag.FileName;
                var path = root + "\\UploadImage\\RecommendedTrips\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\RecommendedTrips\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\RecommendedTrips\\");
                }
                fileImag.SaveAs(path);
                recommendedTrip_Travel.RecommendedTrip_Travel_Img = newfilename;
            }
            if (recommendedTrip_Travel.RecommendedTrip_Travel_ID.ToString() == "-1")
            {
                RecommendedTrip_Travel recom = new RecommendedTrip_Travel()
                {
                    RecommendedTrip_ID = recommendedTrip_Travel.RecommendedTrip_ID,
                    RecommendedTrip_Travel_Title = recommendedTrip_Travel.RecommendedTrip_Travel_Title,
                    RecommendedTrip_Travel_Content = recommendedTrip_Travel.RecommendedTrip_Travel_Content,
                    RecommendedTrip_Travel_Img = recommendedTrip_Travel.RecommendedTrip_Travel_Img,
                    //RecommendedTrip_Travel_Img_Description = recommendedTrip_Travel.RecommendedTrip_Travel_Img_Description,
                    RecommendedTrip_Travel_Link = recommendedTrip_Travel.RecommendedTrip_Travel_Link,
                };
                db.RecommendedTrip_Travel.Add(recom);
                var r = db.SaveChanges();
                if (r > 0)
                {
                    return Json("成功");
                }
                return Json("失敗");
            }
            else
            {
                if (fileImag != null)
                {

                    var old_SizeChart = db.RecommendedTrips.Find(recommendedTrip_Travel.RecommendedTrip_Travel_ID).RecommendedTrips_Img;
                    if (old_SizeChart != null)
                    {
                        string fullpath = Request.MapPath("~/UploadImage/RecommendedTrips/" + old_SizeChart);
                        if (System.IO.File.Exists(fullpath))
                        {
                            System.IO.File.Delete(fullpath);
                        }
                    }
                }
                db.Entry(recommendedTrip_Travel).State = System.Data.Entity.EntityState.Modified;
                var r = db.SaveChanges();
                if (r > 0) //result為itemID >0為新增成功
                {
                    return Json("成功");
                };
                return Json("失敗");
            }

        }
        #endregion
        #region SaveItem
        [HttpPost]
        public ActionResult SaveItem(RecommendedTrip recommendedTrip,HttpPostedFileBase fileImag, HttpPostedFileBase uploadfile, List<int> HashTag_Type,List<string> Keywords)
        {
            var iswriteseo = false;
            var isHash = false;
            if ((Keywords != null && Keywords.Any(v => v.IsNullorEmpty() == false)))
            {
                iswriteseo = true;
            }
            if((HashTag_Type != null))
            {
                isHash = true;
            }
            var imgname = recommendedTrip.RecommendedTrips_Img;
            var filname = recommendedTrip.RecommendedTrips_UploadFileName;
            if (fileImag != null)
                {
                    var root = Request.PhysicalApplicationPath;
                    //var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                    //if (uploadfilepath.IsNullorEmpty())
                    //{
                    //    uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
                    //}
                    var newfilename = DateTime.Now.Ticks + "_" + fileImag.FileName;
                    var path = root + "\\UploadImage\\RecommendedTrips\\" + newfilename;
                    if (System.IO.Directory.Exists(root + "\\UploadImage\\RecommendedTrips\\") == false)
                    {
                        System.IO.Directory.CreateDirectory(root + "\\UploadImage\\RecommendedTrips\\");
                    }
                    fileImag.SaveAs(path);
                    recommendedTrip.RecommendedTrips_Img = newfilename;
            }
            if (uploadfile != null)
                {
                var root = Request.PhysicalApplicationPath;
                 //var uploadfilepath1 = ConfigurationManager.AppSettings["UploadFile"];
                 //   if (uploadfilepath1.IsNullorEmpty())
                 //   {
                 //       uploadfilepath1 = Request.PhysicalApplicationPath + "\\UploadFile";
                 //   }
                    var newpath = root + "\\UploadFile\\RecommendedTrips\\";
                    if (System.IO.Directory.Exists(newpath) == false)
                    {
                        System.IO.Directory.CreateDirectory(newpath);
                    }
                    var guid = Guid.NewGuid();
                    var filename = DateTime.Now.Ticks + "." + uploadfile.FileName.Split('.').Last();
                    var path1 = newpath + filename;
                    recommendedTrip.RecommendedTrips_UploadFileName = filename;
                    recommendedTrip.RecommendedTrips_UploadFilePath = "\\RecommendedTrips\\" + filename;
                uploadfile.SaveAs(path1);
            }
           
                recommendedTrip.RecommendedTrips_HtmContent = HttpUtility.UrlDecode(recommendedTrip.RecommendedTrips_HtmContent);
                recommendedTrip.RecommendedTrips_Title = HttpUtility.UrlDecode(recommendedTrip.RecommendedTrips_Title);
                recommendedTrip.RecommendedTrips_Content= HttpUtility.UrlDecode(recommendedTrip.RecommendedTrips_Content);
            if (recommendedTrip.RecommendedTrips_ID.ToString() == "-1")
            {
                var olddata = db.RecommendedTrips;
                if(olddata.FirstOrDefault()!=null)
                {
                    foreach (var odata in olddata)
                    {
                        odata.Sort = odata.Sort + 1;
                    }
                }
                db.SaveChanges();
                RecommendedTrip recom = new RecommendedTrip()
                {
                    RecommendedTrips_Day_ID = recommendedTrip.RecommendedTrips_Day_ID,
                    RecommendedTrips_Destinations_ID = recommendedTrip.RecommendedTrips_Destinations_ID,
                    RecommendedTrips_Img = recommendedTrip.RecommendedTrips_Img,
                    RecommendedTrips_Img_Description = recommendedTrip.RecommendedTrips_Img_Description,
                    RecommendedTrips_Title = recommendedTrip.RecommendedTrips_Title,
                    RecommendedTrips_Content = recommendedTrip.RecommendedTrips_Content,
                    RecommendedTrips_Location = recommendedTrip.RecommendedTrips_Location,
                    RecommendedTrips_HtmContent = recommendedTrip.RecommendedTrips_HtmContent,
                    RecommendedTrips_StarDay = recommendedTrip.RecommendedTrips_StarDay,
                    RecommendedTrips_EndDay = recommendedTrip.RecommendedTrips_EndDay,
                    RecommendedTrips_UploadFileDesc = recommendedTrip.RecommendedTrips_UploadFileDesc,
                    RecommendedTrips_UploadFileName = recommendedTrip.RecommendedTrips_UploadFileName,
                    RecommendedTrips_UploadFilePath = recommendedTrip.RecommendedTrips_UploadFilePath,
                    RecommendedTrips_LinkUrl = recommendedTrip.RecommendedTrips_LinkUrl,
                    RecommendedTrips_LinkUrlDesc = recommendedTrip.RecommendedTrips_LinkUrlDesc,
                    Sort = 1,
                    InFront = 1,
                };
                db.RecommendedTrips.Add(recom);
                var r= db.SaveChanges();
                if(r>0)
                {
                    if(isHash)
                    {
                        //增加欄位
                        foreach (var item in HashTag_Type)
                        {
                            RecommendedTrips_HashTag_Type HashTag = new RecommendedTrips_HashTag_Type();
                            HashTag.RecommendedTrips_ID = recom.RecommendedTrips_ID;
                            HashTag.HashTag_Type_ID = item;
                            db.RecommendedTrips_HashTag_Type.Add(HashTag);
                            db.SaveChanges();
                        };
                    }
                    if(iswriteseo)
                    {     
                        db.RecommendedTrips_HashTag.Add(new RecommendedTrips_HashTag()
                    {
                        RecommendedTrips_Id = recom.RecommendedTrips_ID,
                        RecommendedTrips_keyword0 = Keywords[0],
                        RecommendedTrips_keyword1 = Keywords[1],
                        RecommendedTrips_keyword2 = Keywords[2],
                        RecommendedTrips_keyword3 = Keywords[3],
                        RecommendedTrips_keyword4 = Keywords[4],
                        RecommendedTrips_keyword5 = Keywords[5],
                        RecommendedTrips_keyword6 = Keywords[6],
                        RecommendedTrips_keyword7 = Keywords[7],
                        RecommendedTrips_keyword8 = Keywords[8],
                        RecommendedTrips_keyword9 = Keywords[9],
                    });
                    db.SaveChanges();
                    }
                    return Json("成功");
                }
                return Json("失敗");
            }
            else
            {
                if (uploadfile != null)
                {
                    var old_SizeChart = filname;
                    if (old_SizeChart != null)
                    {
                        string fullpath = Request.MapPath("~/UploadFile/RecommendedTrips/" + old_SizeChart);
                        if (System.IO.File.Exists(fullpath))
                        {
                            System.IO.File.Delete(fullpath);
                        }
                    }
                }
                if (fileImag != null)
                {
                    var old_SizeChart = imgname;
                    if (old_SizeChart != null)
                    {
                        string fullpath = Request.MapPath("~/UploadImage/RecommendedTrips/" + old_SizeChart);
                        if (System.IO.File.Exists(fullpath))
                        {
                            System.IO.File.Delete(fullpath);
                        }
                    }
                }
                    db.Entry(recommendedTrip).State = System.Data.Entity.EntityState.Modified;
                var r = db.SaveChanges();
                if (r > 0) //result為itemID >0為新增成功
                {
                    if (isHash)
                    {
                        var Old_HashTag = db.RecommendedTrips_HashTag_Type.Where(a => a.RecommendedTrips_ID == recommendedTrip.RecommendedTrips_ID).ToList(); ;
                        if (Old_HashTag.Count() != 0)
                        {
                            //先全部刪除舊的
                            foreach (var item in Old_HashTag)
                            {
                                var Old_HashTag_Item = db.RecommendedTrips_HashTag_Type.Find(item.RecommendedTrips_HashTag_ID);
                                db.Entry(Old_HashTag_Item).State = System.Data.Entity.EntityState.Deleted;
                                db.SaveChanges();
                            };

                            //新增加
                            foreach (var item in HashTag_Type)
                            {
                                RecommendedTrips_HashTag_Type Sub_HashTag = new RecommendedTrips_HashTag_Type();
                                Sub_HashTag.RecommendedTrips_ID = recommendedTrip.RecommendedTrips_ID;
                                Sub_HashTag.HashTag_Type_ID = item;
                                db.RecommendedTrips_HashTag_Type.Add(Sub_HashTag);
                                db.SaveChanges();
                            };
                        }
                    }
                        if (iswriteseo)
                        {
                            var HashMode = db.RecommendedTrips_HashTag.Where(p => p.RecommendedTrips_Id == recommendedTrip.RecommendedTrips_ID);
                            var HashTagcout = HashMode.Count();
                            if(HashTagcout==0)
                            {
                                db.RecommendedTrips_HashTag.Add(new RecommendedTrips_HashTag()
                                {
                                    RecommendedTrips_Id = recommendedTrip.RecommendedTrips_ID,
                                    RecommendedTrips_keyword0 = Keywords[0],
                                    RecommendedTrips_keyword1 = Keywords[1],
                                    RecommendedTrips_keyword2 = Keywords[2],
                                    RecommendedTrips_keyword3 = Keywords[3],
                                    RecommendedTrips_keyword4 = Keywords[4],
                                    RecommendedTrips_keyword5 = Keywords[5],
                                    RecommendedTrips_keyword6 = Keywords[6],
                                    RecommendedTrips_keyword7 = Keywords[7],
                                    RecommendedTrips_keyword8 = Keywords[8],
                                    RecommendedTrips_keyword9 = Keywords[9],
                                });
                                db.SaveChanges();
                            }
                            else
                            {
                            var Hash = db.RecommendedTrips_HashTag.Find(HashMode.FirstOrDefault().RecommendedTrips_HashTag_Id);
                            Hash.RecommendedTrips_keyword0 = Keywords[0];
                                Hash.RecommendedTrips_keyword1 = Keywords[1];
                                Hash.RecommendedTrips_keyword2 = Keywords[2];
                                Hash.RecommendedTrips_keyword3 = Keywords[3];
                                Hash.RecommendedTrips_keyword4 = Keywords[4];
                                Hash.RecommendedTrips_keyword5 = Keywords[5];
                                Hash.RecommendedTrips_keyword6 = Keywords[6];
                                Hash.RecommendedTrips_keyword7 = Keywords[7];
                                Hash.RecommendedTrips_keyword8 = Keywords[8];
                                Hash.RecommendedTrips_keyword9 = Keywords[9];
                          
                                db.Entry(Hash).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }    
                        }
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
        #region FileDownLoad
        public ActionResult FileDownLoad( int itemid)
        {
            var model = db.RecommendedTrips.Find(itemid);
            string filepath = model.RecommendedTrips_UploadFilePath;
            string oldfilename = model.RecommendedTrips_UploadFileName;
            var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
            if (uploadfilepath.IsNullorEmpty())
            {
                uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
            }
            if (filepath != "")
            {
                string filename = System.IO.Path.GetFileName(filepath);
                if (string.IsNullOrEmpty(oldfilename)) { oldfilename = filename; }
                Stream iStream = new FileStream(uploadfilepath + filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/octet-stream", oldfilename);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }
        #endregion
        #region UpdateSeq
        public ActionResult UpdateSeq(int itemid,int seq,string Type)
        {
            try
            {
                var fromseq = db.RecommendedTrips.Find(itemid);
                var nowseq = fromseq.Sort;
                var maxsort = db.RecommendedTrips.Max(p => p.Sort).Value;
                seq = seq == 0 ? 1 : seq;
                if (Type == "btn")
                {
                    seq = seq > maxsort ? maxsort : seq;
                    if (seq == -1)
                    {

                        seq = maxsort;
                    }     
                }
                else
                {
                    int checkval;
                    if (!Int32.TryParse(seq.ToString(), out checkval)) //確認是否是整數
                    {
                        return Json("更新作業失敗", JsonRequestBehavior.AllowGet);
                    }
                    if(seq> maxsort)
                    {
                        return Json("更新作業失敗", JsonRequestBehavior.AllowGet);
                    }
                    if(seq <0)
                    {
                        return Json("更新作業失敗", JsonRequestBehavior.AllowGet);
                    }
                }
                var toseq = db.RecommendedTrips.Where(p => p.Sort == seq);
                fromseq.Sort = toseq.First().Sort;
                toseq.First().Sort = nowseq;
                db.Entry(fromseq).State = System.Data.Entity.EntityState.Modified;
                //db.Entry(toseq).State = System.Data.Entity.EntityState.Modified;
                var r = db.SaveChanges();
                if (r > 0)
                {
                    return Json("更新作業成功", JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                return Json("更新作業失敗", JsonRequestBehavior.AllowGet);
            }
            return Json("更新作業失敗", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region UnitPrint
        public ActionResult UnitPrint()
        {
            var model=db.MessageUnitSettings.Where(p => p.MainID == -1).Select(p => new UnitPrint { isPrint = (bool)p.IsPrint, isForward = (bool)p.IsForward, isRSS = (bool)p.IsRSS, isShare = (bool)p.IsShare,UnitID=p.ID }).FirstOrDefault();
            return View(model);
        }
        #endregion
        #region SaveUnit
        public ActionResult SaveUnit(UnitPrint unit)
        {
            var Msunit = db.MessageUnitSettings.Find(unit.UnitID);
            Msunit.IsForward = unit.isForward;
            Msunit.IsPrint = unit.isPrint;
            Msunit.IsShare = unit.isShare;
            db.Entry(Msunit).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json("修改成功");
        }
        #endregion
    }
}