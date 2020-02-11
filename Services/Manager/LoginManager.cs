using Services.Interface;
using SQLModel;
using SQLModel.Models;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Utilities;
using ViewModels;
using ViewModels.DBModels;

namespace Services.Manager
{
    public class LoginManager : ILoginManager
    {
        readonly SQLRepository<Users> _sqlrepository;
        readonly SQLRepository<Student> _sthdentsqlrepository;
        readonly SQLRepository<GroupUser> _authoritygroupsqlrepository;
        readonly SQLRepository<StudentFormSetting> _formsettingsqlrepository;
        public LoginManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.Users;
            _authoritygroupsqlrepository= sqlinstance.GroupUser;
            _sthdentsqlrepository= sqlinstance.Student;
            _formsettingsqlrepository= sqlinstance.StudentFormSetting;
        }
        public string[] GetCaptchImage()
        {
            CaptchaImage _captcha = new CaptchaImage();
            _captcha.LineNoise =CaptchaImage.LineNoiseLevel.None;
            _captcha.BackgroundNoise = CaptchaImage.BackgroundNoiseLevel.None;
            _captcha.TextLength = 4;
            _captcha.Width = 74;
            _captcha.Height = 30;
            Bitmap _bmp = _captcha.RenderImage();
            var base64str = "";
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                _bmp.Save(ms, ImageFormat.Png);
                base64str = string.Format("data:{0};base64,{1}", "image/png", Convert.ToBase64String(ms.ToArray()));
                _bmp.Dispose();
                ms.Dispose();
            }
            return new string[] { _captcha.Text,base64str } ;
        }

        public AdminMemberModel ValidateUser(string account,string password)
        {
            AdminMemberModel _adminuser=null;
            var user = _sqlrepository.GetByWhere("Account=@1 and PWD=@2", new object[] { account , password }).ToArray();
            if (user.Count() > 0) {
                _adminuser = new AdminMemberModel();
                _adminuser.ID= user.First().ID.Value;
                _adminuser.Account = account;
                _adminuser.Status= user.First().Enabled.Value;
                _adminuser.Name = user.First().User_Name;
                var groupname = _authoritygroupsqlrepository.GetByWhere("ID=@1", new object[] { user.First().Group_ID });
                _adminuser.GroupId = user.First().Group_ID.Value;
                if (groupname.Count() == 0)
                {
                    _adminuser.GroupName = "";
                }
                else {
                    _adminuser.GroupName = groupname.First().Group_Name;
                }
            }
            return _adminuser;
        }

        public string ForgetPassword(string email)
        {
            var student = _sthdentsqlrepository.GetByWhere("EMail=@1", new object[] { email });
            if (student.Count() == 0)
            {
                return "此EMail未登錄";
            }
            else {
                try
                {
                    System.Net.Mail.MailMessage mailmessage = new System.Net.Mail.MailMessage();
                    var host = System.Web.Configuration.WebConfigurationManager.AppSettings["smtphost"];
                    var mailfrom = System.Web.Configuration.WebConfigurationManager.AppSettings["mailfrom"];
                    var setting = _formsettingsqlrepository.GetAll();
                    if (setting.Count() > 0)
                    {
                        var forgermessage = setting.First().StudentForgetPW == null ? "" : setting.First().StudentForgetPW; 
                        if (setting.First().SenderEMail.IsNullorEmpty() == false)
                        {
                            var NoticeSenderEMail = setting.First().StudentSenderEMail;
                            var NoticeSenderName = setting.First().StudentSenderName;
                            var NoticeSubject ="忘記密碼通知信";
                            NoticeSenderEMail = string.IsNullOrEmpty(setting.First().StudentSenderEMail) ? mailfrom : setting.First().StudentSenderEMail;
                            NoticeSenderName = string.IsNullOrEmpty(setting.First().StudentSenderName) ? "忘記密碼通知信" : setting.First().StudentSenderName;
                            var slist = setting.First().ReceiveMail.Split(';');
                            mailmessage.From = new MailAddress(NoticeSenderEMail, NoticeSenderName);
                            mailmessage.To.Add(new MailAddress(email));
                            mailmessage.SubjectEncoding = System.Text.Encoding.UTF8;
                            mailmessage.Subject = NoticeSubject;
                            mailmessage.BodyEncoding = System.Text.Encoding.UTF8;
                            string body = forgermessage+ "<br/>" + "您的密碼【" + student.First().Password + "】";
                            mailmessage.Body = body;
                            mailmessage.IsBodyHtml = true;
                            mailmessage.Priority = MailPriority.High;
                            var ur = System.Web.Configuration.WebConfigurationManager.AppSettings["mailuser"];
                            var pw = System.Web.Configuration.WebConfigurationManager.AppSettings["mailpassword"];
                            var port = System.Web.Configuration.WebConfigurationManager.AppSettings["mailport"];
                            //SmtpClient client2 = new SmtpClient(host);
                            //client2.Send(mailmessage);
                            if (string.IsNullOrEmpty(pw) == false)
                            {
                                SmtpClient client = new SmtpClient(host, int.Parse(port));
                                client.EnableSsl = true;
                                client.Credentials = new NetworkCredential(ur, pw);
                                client.Send(mailmessage);
                            }
                            else
                            {
                                SmtpClient client2 = new SmtpClient(host);
                                client2.Send(mailmessage);
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    NLogManagement.SystemLogInfo("通知信寄信失敗 原因:" + ex.Message);
                }
            }
            return "密碼已寄出";
        }

    }
}
