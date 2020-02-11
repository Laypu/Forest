using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using Utilities;
using ViewModels;

namespace WebSiteProject.Controllers
{
    public class StudentController : AppController
    {
        FirstPageManager _IFirstPageManager;
        IModelWebsiteMapManager _IModelWebsiteMapManager;
        IMenuManager _IMenuManager;
        IStudentManager _IStudentManager;
        ILoginManager _ILoginManager;
        readonly SQLRepository<Member> _membersqlrepository;
        readonly SQLRepository<Company> _companysqlrepository;
        public StudentController()
        {
            _IFirstPageManager = new FirstPageManager(connectionstr, LangID);
            _IModelWebsiteMapManager = new ModelWebsiteMapManager(new SQLRepository<ModelWebsiteMapMain>(connectionstr));
            _IMenuManager = new MenuManager(new SQLRepository<Menu>(connectionstr));
            _IStudentManager = new StudentManager(new SQLRepository<Student>(connectionstr));
            _ILoginManager = new LoginManager(new SQLRepository<Users>(connectionstr));
            _membersqlrepository = new SQLRepository<Member>(Common.connectionMemberStr);
            _companysqlrepository = new SQLRepository<Company>(Common.connectionMemberStr);
        }
        #region JoinInfo
        public ActionResult JoinInfo()
        {
            var langid = _IFirstPageManager.CheckLangID("");
            var model = _IFirstPageManager.GetModel(Device, langid);
            var smodel = _IStudentManager.GetFormSettingEdit();
            ViewBag.StudentRight = smodel.StudentRight;
            return View(model);
        }
        #endregion

        #region CreateComplete
        public ActionResult CreateComplete()
        {
            var langid = _IFirstPageManager.CheckLangID("");
            var model = _IFirstPageManager.GetModel(Device, langid);
            var smodel = _IStudentManager.GetFormSettingEdit();
            ViewBag.StudentFinish = smodel.StudentFinish;
            return View(model);
        }
        #endregion

        
        #region Create
        public ActionResult Create()
        {
            var langid = _IFirstPageManager.CheckLangID("");
            var model = _IFirstPageManager.GetModel(Device, langid);
            var smodel = _IStudentManager.GetFormSettingEdit();
            ViewBag.StudentRight = smodel.StudentRight;
            var imagestrArr = _ILoginManager.GetCaptchImage();
            ViewBag.catchstr = imagestrArr[0];
            ViewBag.image = imagestrArr[1];
            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit()
        {
            var langid = _IFirstPageManager.CheckLangID("");
            var model = _IFirstPageManager.GetModel(Device, langid);
            ViewBag.studentmodel = _IStudentManager.GetStudentEdit(UserID, Common.connectionMemberStr);
            return View(model);
        }
        #endregion

        #region ChangePassword
        public ActionResult ChangePassword()
        {
            var langid = _IFirstPageManager.CheckLangID("");
            var model = _IFirstPageManager.GetModel(Device, langid);
            ViewBag.studentmodel = _IStudentManager.GetStudentEdit(UserID, Common.connectionMemberStr);
            return View(model);
        }
        #endregion

        #region CheckAccount
        public ActionResult CheckAccount(string account)
        {
            var cnt = _IStudentManager.CheckAccount(account);
            if (cnt > 0) { return Json("Y"); } else { return Json(""); }
         
        }
        #endregion

        #region CheckMember
        public ActionResult CheckMember(string taxid)
        {
            var company = _companysqlrepository.GetJoinByWhere<Company>("a.TaxID=@1 and b.Active=1", new object[] { taxid },
                            "a.MemberID=b.MemberID", "a.*", "Company as a,Member as b").ToList();
            if (company.Count() > 0) {
                var chnname = company.First().NameCHN == null ? "" : company.First().NameCHN;
                var engname = company.First().NameENG == null ? "" : company.First().NameENG;
                return Json(new string[] {  chnname, engname });
            }
            return Json("");
        }
        #endregion

        #region EditStudent
        public ActionResult EditStudent(StudentEdit model)
        {
            return Json(_IStudentManager.CreateStudent(model,Common.connectionMemberStr));
        }
        #endregion

        #region UpdateStudent
        public ActionResult UpdateStudent(StudentEdit model)
        {
            return Json(_IStudentManager.UpdateStudent(model, Common.connectionMemberStr));
        }
        #endregion

        [AllowAnonymous]
        #region CaptchRefresh
        public ActionResult CaptchRefresh()
        {
            var imagestrArr = _ILoginManager.GetCaptchImage();
            Session["Captch"] = imagestrArr[0];
            return Json(new string[] { imagestrArr[0], imagestrArr[1] });
        }
        #endregion

        #region SetNewPassword
        public ActionResult SetNewPassword(string OrgPassword,string NewPassword,string ID)
        {
            if (Request.IsAuthenticated)
            {
                return Json(_IStudentManager.ChangePassword(OrgPassword, NewPassword, ID));
            }
            else { return Json(Common.GetLangText("需先登入學員!")); }
          
        }
        #endregion

    }
}