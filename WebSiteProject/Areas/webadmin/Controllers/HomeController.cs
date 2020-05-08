using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using Utilities;
using ViewModels;
using System.Configuration;
using System.IO;



namespace WebSiteProject.Areas.webadmin.Controllers
{
    [Authorize]
    public class HomeController : AppController
    {
        WebSiteProject.Models.ForestEntities db = new Models.ForestEntities();


        IADManager _IADManager;
        readonly SQLRepository<SQLModel.Models.Img> _imgsqlrepository;
        public HomeController()
        {
            _imgsqlrepository = new SQLRepository<SQLModel.Models.Img>(connectionstr);
        }

        #region Index 預設首頁
        // GET: Home
        public ActionResult Index(string menutype)
        {
            Session["menutype"] = menutype;
            Session.Timeout = 600;
            return View();
        }
        #endregion


        #region Index_ADMain 輪播牆檢視
        public ActionResult Index_ADMain(string home_type, string home_stype,int site_id) 
        {
            TempData["site_id"] = 1;

            return RedirectToAction("Index", "Ad", new { type = home_type, stype = home_stype, MenuType = 0 ,site_id = site_id});
        }
        #endregion


        #region Index_ADMain2 輪播牆檢視2
        public ActionResult Index_ADMain2(string home_type, string home_stype, int site_id)
        {
            TempData["site_id"] = 2;
            return RedirectToAction("Index", "Ad", new { type = home_type, stype = home_stype, MenuType = 0, site_id = site_id });
        }
        #endregion


        #region Index_FreeZone 自由區檢視
        public ActionResult Index_FreeZone(int id = 1)
        {
            var FreeZoneContent = db.FreeZoneContents.Where(f => f.FreeZoneTitleID == id).ToList();

            ViewBag.FreeZoneTitleID = db.FreeZoneTitles.Find(id).FreeZoneTitleID;
            ViewBag.FreeZoneTitle = db.FreeZoneTitles.Find(id).Title;
            ViewBag.FreeZoneTitleContent = Server.HtmlDecode(db.FreeZoneTitles.Find(id).FreeZoneTitleContent);


            //return View(FreeZoneContent);
            return View(FreeZoneContent);

        }
        #endregion


        #region Index_FreeZoneTitle_Edit 編輯自由區標題
        [ValidateInput(false)]
        public ActionResult Index_FreeZoneTitle_Edit(int _FreeZoneID, string _FreeZoneTitle, string _FreeZoneTitleContent)
        {
            //Title_Id預設為1
            int Title_id = 1;
            try
            {
                var FreeZoneTitle = db.FreeZoneTitles.Find(_FreeZoneID);
                FreeZoneTitle.Title = _FreeZoneTitle;
                FreeZoneTitle.FreeZoneTitleContent = Server.HtmlEncode(_FreeZoneTitleContent);
                db.Entry(FreeZoneTitle).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //取得Title_Id
                Title_id = (int)FreeZoneTitle.FreeZoneTitleID;
            }
            catch 
            {
                return RedirectToAction("Index_FreeZone");

            }
            TempData["Msg"] = "作業完成";


            //if (Title_id == 1)
            //{
            //    TempData["Select_ID"] = 3;
            //}
            //else
            //{
            //    TempData["Select_ID"] = 5;
            //}

            return RedirectToAction("Index_FreeZone", "Home", new { id = Title_id });
        }
        #endregion        


        #region Index_FreeZone_Edit 編輯自由區連結
        public ActionResult Index_FreeZone_Edit(int? FreeZoneContentID, string LinkName, string Link,int? FreeZoneTitleID, int IsCreate = 0)
        {
            //Title_Id預設為1
            int Title_id = 1;
            try
            {
                if(IsCreate == 1)
                {

                    WebSiteProject.Models.FreeZoneContent freezonecontent = new Models.FreeZoneContent();
                    freezonecontent.FreeZoneTitleID = FreeZoneTitleID;
                    freezonecontent.LinkName = LinkName;
                    freezonecontent.Link = Link;                    
                    db.Entry(freezonecontent).State = System.Data.Entity.EntityState.Added;
                    db.SaveChanges();

                    //取得Title_Id
                    Title_id = (int)freezonecontent.FreeZoneTitleID;
                }
                else
                {
                    var FreeZoneContent = db.FreeZoneContents.Find(FreeZoneContentID);
                    FreeZoneContent.LinkName = LinkName;
                    FreeZoneContent.Link = Link;
                    db.Entry(FreeZoneContent).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //取得Title_Id
                    Title_id = (int)FreeZoneContent.FreeZoneTitleID;
                }

            }
            catch
            {
                TempData["Msg"] = "作業失敗";
            }
            TempData["Msg"] = "作業完成";
            return RedirectToAction("Index_FreeZone", "Home", new { id = Title_id });
        }
        #endregion


        #region Index_FreeZone_Delete 刪除連結
        public ActionResult Index_FreeZone_Delete(int id)
        {
            //Title_Id預設為1
            int Title_id = 1;

            try
            {
                var FreeZoneContent = db.FreeZoneContents.Find(id);
                db.FreeZoneContents.Remove(FreeZoneContent);
                db.SaveChanges();

                //取得Title_Id
                Title_id = (int)FreeZoneContent.FreeZoneTitleID;
            }
            catch
            {
                TempData["Msg"] = "作業失敗";
            }
            TempData["Msg"] = "作業完成";
            return RedirectToAction("Index_FreeZone", "Home", new { id = Title_id });
        }

        #endregion


        #region Index_Img 大型圖片
        public ActionResult Index_Img()
        {
            var Img = db.F_Index_Img.FirstOrDefault().Index_Img_Name ?? "";
            ViewBag.ImgName = Img;
            ViewBag.ImgPath = Url.Content("~/UploadImage/Index_Img/" + Img);
            return View();
        }
        #endregion


        #region Index_Img_Edit 大型圖片上傳
        [HttpPost]
        public ActionResult Index_Img_Edit(HttpPostedFileBase Index_Img_File)
        {
            string Index_Img_Name = "";


            if (Index_Img_File != null) //判斷是否有檔案
            {
                if (Index_Img_File.ContentLength > 0)  //若檔案不為空檔案
                {
                    string uploadPath = Server.MapPath("~/UploadImage/Index_Img/");
                    // 如果UploadFiles文件夹不存在则先创建
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    Index_Img_Name = Path.GetFileName(Index_Img_File.FileName);  //取得檔案名
                    var path = Path.Combine(Server.MapPath("~/UploadImage/Index_Img/"), Index_Img_Name);  //取得本機檔案路徑

                    


                    //若有重複則換名字_srart
                    while (System.IO.File.Exists(path))
                    {
                        Random rand = new Random();
                        Index_Img_Name = rand.Next().ToString() + "-" + Index_Img_Name;
                        path = Path.Combine(Server.MapPath("~/UploadImage/Index_Img/"), Index_Img_Name);
                    }
                    Index_Img_File.SaveAs(path);
                    //若有重複則換名字_end

                    
                    db.F_Index_Img.FirstOrDefault().Index_Img_Name = Index_Img_Name;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index_Img","Home");
        }
        #endregion



        //public async Task<ActionResult> TSS(string text = "1234")
        //{
        //    //string fileName = "fileName";
        //    //Task<ViewResult> task = Task.Run(() =>
        //    //{
        //    //    using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
        //    //    {
        //    //        speechSynthesizer.SetOutputToWaveFile(Server.MapPath("/UploadImage/fileName.wav"));
        //    //        speechSynthesizer.Speak(text);

        //    //        ViewBag.FileName = fileName + ".wav";
        //    //        return View();
        //    //    }
        //    //});
        //    //return await task;
        //    Task<FileContentResult> task = Task.Run(() =>
        //    {
        //        using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
        //        {
        //            MemoryStream stream = new MemoryStream();

        //            speechSynthesizer.SetOutputToWaveStream(stream);
        //            speechSynthesizer.Speak("E");
        //            speechSynthesizer.Speak("A");
        //            speechSynthesizer.Speak("2");
        //            speechSynthesizer.Speak("B");

        //            var bytes = stream.GetBuffer();
        //            var mp3bytes = ConvertWavStreamToMp3File(ref stream, Server.MapPath("/UploadImage/fileName.mp3"));
        //            return File(mp3bytes, "audio/mpeg");
        //        }
        //    });
        //    return await task;
        //}

        //private Task SomeJobBy3rdPtyLibrary()
        //{
        //    return Task.Factory.StartNew(() =>
        //    {
        //        var voice = new System.Speech.Synthesis.SpeechSynthesizer();
        //        voice.GetInstalledVoices()
        //            .ToList().ForEach((v) =>
        //            {
        //                Console.WriteLine(
        //                    v.VoiceInfo.Name + " " +
        //                    v.VoiceInfo.Culture.DisplayName);
        //            });
        //    });
        //}

        //private byte[] ConvertWavStreamToMp3File(ref MemoryStream ms, string savetofilename)
        //{
        //    //rewind to beginning of stream
        //    CheckAddBinPath();
        //    ms.Seek(0, SeekOrigin.Begin);
        //    MemoryStream msmp3 = new MemoryStream();
        //    using (var retMs = new MemoryStream())
        //    using (var rdr = new WaveFileReader(ms))
        //    using (var wtr = new LameMP3FileWriter(msmp3, rdr.WaveFormat, LAMEPreset.VBR_90))
        //    {
        //        rdr.CopyTo(wtr);
        //    }
        //    return msmp3.ToArray();
        //}
        public void CheckAddBinPath()
        {
            // find path to 'bin' folder
            var binPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "bin" });
            // get current search path from environment
            var path = Environment.GetEnvironmentVariable("PATH") ?? "";

            // add 'bin' folder to search path if not already present
            if (!path.Split(Path.PathSeparator).Contains(binPath, StringComparer.CurrentCultureIgnoreCase))
            {
                path = string.Join(Path.PathSeparator.ToString(), new string[] { path, binPath });
                Environment.SetEnvironmentVariable("PATH", path);
            }
        }
    }
}