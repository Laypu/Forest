using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.DBModels
{
    public class SQLRepositoryInstances : DbContext
    {
        public SQLRepositoryInstances(string dbname) : base(dbname) { }
        public SQLRepository<T> GetSQInstances<T>() where T : class { return new SQLRepository<T>(base._dbname); }
    
        public virtual SQLRepository<ActiveItem> ActiveItem { get { return new SQLRepository<ActiveItem>(base._dbname); } }
        public virtual SQLRepository<ActiveDateRange> ActiveDateRange { get { return new SQLRepository<ActiveDateRange>(base._dbname); } }
        public virtual SQLRepository<AdminFunctionAuth> AdminFunctionAuth { get { return new SQLRepository<AdminFunctionAuth>(base._dbname); } }
        public virtual SQLRepository<ActiveUnitSetting> ActiveUnitSetting { get { return new SQLRepository<ActiveUnitSetting>(base._dbname); } }
        public virtual SQLRepository<ActivePhoto> ActivePhoto { get { return new SQLRepository<ActivePhoto>(base._dbname); } }
        public virtual SQLRepository<ActivePhotoCount> ActivePhotoCount { get { return new SQLRepository<ActivePhotoCount>(base._dbname); } }
        public virtual SQLRepository<AdminFunction> AdminFunction { get { return new SQLRepository<AdminFunction>(base._dbname); } }
        public virtual SQLRepository<ColumnSetting> ColumnSetting { get { return new SQLRepository<ColumnSetting>(base._dbname); } }
        public virtual SQLRepository<ClickCountTable> ClickCountTable { get { return new SQLRepository<ClickCountTable>(base._dbname); } }
        public virtual SQLRepository<FileDownloadItem> FileDownloadItem { get { return new SQLRepository<FileDownloadItem>(base._dbname); } }
        public virtual SQLRepository<FileDownloadUnitSetting> FileDownloadUnitSetting { get { return new SQLRepository<FileDownloadUnitSetting>(base._dbname); } }
        public virtual SQLRepository<Menu> Menu { get { return new SQLRepository<Menu>(base._dbname); } }
        public virtual SQLRepository<MessageItem> MessageItem { get { return new SQLRepository<MessageItem>(base._dbname); } }
        public virtual SQLRepository<ModelActiveEditMain> ModelActiveEditMain { get { return new SQLRepository<ModelActiveEditMain>(base._dbname); } }
        public virtual SQLRepository<ModelFileDownloadMain> ModelFileDownloadMain { get { return new SQLRepository<ModelFileDownloadMain>(base._dbname); } }
        public virtual SQLRepository<MessageUnitSetting> MessageUnitSetting { get { return new SQLRepository<MessageUnitSetting>(base._dbname); } }
        public virtual SQLRepository<ModelMessageMain> ModelMessageMain { get { return new SQLRepository<ModelMessageMain>(base._dbname); } }
        public virtual SQLRepository<ModelPageEditMain> ModelPageEditMain { get { return new SQLRepository<ModelPageEditMain>(base._dbname); } }
        public virtual SQLRepository<GroupActive> GroupActive { get { return new SQLRepository<GroupActive>(base._dbname); } }
        public virtual SQLRepository<GroupFileDownload> GroupFileDownload { get { return new SQLRepository<GroupFileDownload>(base._dbname); } }
        public virtual SQLRepository<GroupMessage> GroupMessage { get { return new SQLRepository<GroupMessage>(base._dbname); } }
        public virtual SQLRepository<GroupUser> GroupUser { get { return new SQLRepository<GroupUser>(base._dbname); } }
        public virtual SQLRepository<Img> Img { get { return new SQLRepository<Img>(base._dbname); } }
        public virtual SQLRepository<PageIndexItem> PageIndexItem { get { return new SQLRepository<PageIndexItem>(base._dbname); } }
        public virtual SQLRepository<PageLayout> PageLayout { get { return new SQLRepository<PageLayout>(base._dbname); } }
        public virtual SQLRepository<PageUnitSetting> PageUnitSetting { get { return new SQLRepository<PageUnitSetting>(base._dbname); } }
        public virtual SQLRepository<SiteConfig> SiteConfig { get { return new SQLRepository<SiteConfig>(base._dbname); } }
        public virtual SQLRepository<SiteFlow> SiteFlow { get { return new SQLRepository<SiteFlow>(base._dbname); } }
        public virtual SQLRepository<SEO> SEO { get { return new SQLRepository<SEO>(base._dbname); } }
        public virtual SQLRepository<Users> Users { get { return new SQLRepository<Users>(base._dbname); } }
        public virtual SQLRepository<VerifyData> VerifyData { get { return new SQLRepository<VerifyData>(base._dbname); } }
        public virtual SQLRepository<ModelEventListMain> ModelEventListMain { get { return new SQLRepository<ModelEventListMain>(base._dbname); } }
        public virtual SQLRepository<EventListItem> EventListItem { get { return new SQLRepository<EventListItem>(base._dbname); } }
        public virtual SQLRepository<ModelWebsiteMapMain> ModelWebsiteMapMain { get { return new SQLRepository<ModelWebsiteMapMain>(base._dbname); } }
        public virtual SQLRepository<ModelLangMain> ModelLangMain { get { return new SQLRepository<ModelLangMain>(base._dbname); } }
        public virtual SQLRepository<MenuUrl> MenuUrl { get { return new SQLRepository<MenuUrl>(base._dbname); } }
        public virtual SQLRepository<ModelFormMain> ModelFormMain { get { return new SQLRepository<ModelFormMain>(base._dbname); } }
        public virtual SQLRepository<Lang> Lang { get { return new SQLRepository<Lang>(base._dbname); } }
        public virtual SQLRepository<StudentFormSetting> StudentFormSetting { get { return new SQLRepository<StudentFormSetting>(base._dbname); } }
        public virtual SQLRepository<Student> Student { get { return new SQLRepository<Student>(base._dbname); } }
        public virtual SQLRepository<FormUnitSetting> FormUnitSetting { get { return new SQLRepository<FormUnitSetting>(base._dbname); } }
        public virtual SQLRepository<FormSelItem> FormSelItem { get { return new SQLRepository<FormSelItem>(base._dbname); } }
        public virtual SQLRepository<FormSetting> FormSetting { get { return new SQLRepository<FormSetting>(base._dbname); } }
        public virtual SQLRepository<FormInput> FormInput { get { return new SQLRepository<FormInput>(base._dbname); } }
        public virtual SQLRepository<FormInputNote> FormInputNote { get { return new SQLRepository<FormInputNote>(base._dbname); } }
        public virtual SQLRepository<ModelVideoMain> ModelVideoMain { get { return new SQLRepository<ModelVideoMain>(base._dbname); } }
        public virtual SQLRepository<GroupVideo> GroupVideo { get { return new SQLRepository<GroupVideo>(base._dbname); } }
        public virtual SQLRepository<VideoItem> VideoItem { get { return new SQLRepository<VideoItem>(base._dbname); } }
        public virtual SQLRepository<VideoUnitSetting> VideoUnitSetting { get { return new SQLRepository<VideoUnitSetting>(base._dbname); } }
        public virtual SQLRepository<WebSiteMapInfo> WebSiteMapInfo { get { return new SQLRepository<WebSiteMapInfo>(base._dbname); } }
        public virtual SQLRepository<ModelPatentMain> ModelPatentMain { get { return new SQLRepository<ModelPatentMain>(base._dbname); } }
        public virtual SQLRepository<PatentItem> PatentItem { get { return new SQLRepository<PatentItem>(base._dbname); } }
        public virtual SQLRepository<PatentUnitSetting> PatentUnitSetting { get { return new SQLRepository<PatentUnitSetting>(base._dbname); } }
        public virtual SQLRepository<GroupPatent> GroupPatent { get { return new SQLRepository<GroupPatent>(base._dbname); } }
        public virtual SQLRepository<PatentDetail> PatentDetail { get { return new SQLRepository<PatentDetail>(base._dbname); } }
        public virtual SQLRepository<SiteLayout> SiteLayout { get { return new SQLRepository<SiteLayout>(base._dbname); } }
        public virtual SQLRepository<LangKey> LangKey { get { return new SQLRepository<LangKey>(base._dbname); } }
        public virtual SQLRepository<LangInputText> LangInputText { get { return new SQLRepository<LangInputText>(base._dbname); } }
        public virtual SQLRepository<PageLayoutOP1> PageLayoutOP1 { get { return new SQLRepository<PageLayoutOP1>(base._dbname); } }
        public virtual SQLRepository<PageLayoutOP2> PageLayoutOP2 { get { return new SQLRepository<PageLayoutOP2>(base._dbname); } }
        public virtual SQLRepository<PageLayoutOP3> PageLayoutOP3 { get { return new SQLRepository<PageLayoutOP3>(base._dbname); } }
        public virtual SQLRepository<LinkItem> LinkItem { get { return new SQLRepository<LinkItem>(base._dbname); } }
        public virtual SQLRepository<PageLayoutActivity> PageLayoutActivity { get { return new SQLRepository<PageLayoutActivity>(base._dbname); } }
        
    }
}
