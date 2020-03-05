using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLModel.Models;
using System.Web.Mvc;
using ViewModels;
using SQLModel;
using Utilities;
using System.Web;
using AutoMapper;

namespace Services.Manager
{
    public class MasterPageManager : IMasterPageManager
    {
        ISiteLayoutManager _ISiteLayoutManager;
        IModelManager _IModelManager;
        readonly SQLRepository<ADMain> _adminsqlrepository;
        readonly SQLRepository<ADSet> _adsetsqlrepository;
        readonly SQLRepository<Menu> _menusqlrepository;
        readonly SQLRepository<MenuUrl> _menuurlsqlrepository;
        readonly SQLRepository<ModelLangMain> _ModelLangMainsqlrepository;
        readonly SQLRepository<Lang> _LangMainsqlrepository;
        readonly SQLRepository<PageIndexItem> _PageIndexItemsqlrepository;
        readonly SQLRepository<MessageItem> _MessageItemsqlrepository;
        readonly SQLRepository<AdminFunctionAuth> _functioninputsqlrepository;
        readonly SQLRepository<PageLayout> _pagelayoutsqlrepository;
        readonly SQLRepository<VideoItem> _videosqlrepository;
        readonly SQLRepository<AdminFunction> _adfunctionrepository;
        readonly SQLRepository<SEO> _seosqlrepository;
        readonly SQLRepository<GroupMessage> _GroupMessagesqlrepository;
        readonly SQLRepository<GroupActive> _GroupActivesqlrepository;
        readonly SQLRepository<CountSearchKey> _SKrepository;
        readonly SQLRepository<ActiveItem> _ActiveItemsqlrepository;
        readonly SQLRepository<PatentItem> _patentsqlrepository;
        
        string _hosturl = "";
        string _langid = "";
        Dictionary<string, string> _langdict;
        public MasterPageManager(string connectionStr, string langid, Dictionary<string, string> langdict)
        {
            _langdict = langdict;
            _langid = langid;
               _ISiteLayoutManager = new SiteLayoutManager(new ViewModels.DBModels.SQLRepositoryInstances(connectionStr));
            _adsetsqlrepository = new SQLRepository<ADSet>(connectionStr);
            _adminsqlrepository = new SQLRepository<ADMain>(connectionStr);
            _menusqlrepository = new SQLRepository<Menu>(connectionStr);
            _menuurlsqlrepository = new SQLRepository<MenuUrl>(connectionStr);
            _videosqlrepository = new SQLRepository<VideoItem>(connectionStr);
            _patentsqlrepository = new SQLRepository<PatentItem>(connectionStr);
            
            _ModelLangMainsqlrepository = new SQLRepository<ModelLangMain>(connectionStr);
            _LangMainsqlrepository = new SQLRepository<Lang>(connectionStr);
            _PageIndexItemsqlrepository = new SQLRepository<PageIndexItem>(connectionStr);
            _MessageItemsqlrepository = new SQLRepository<MessageItem>(connectionStr);
            _ActiveItemsqlrepository= new SQLRepository<ActiveItem>(connectionStr);
            _functioninputsqlrepository = new SQLRepository<AdminFunctionAuth>(connectionStr);
            _hosturl = string.Format("{0}://{1}",
                  System.Web.HttpContext.Current.Request.Url.Scheme,
                  System.Web.HttpContext.Current.Request.Url.Authority);
            //_adset = _adsetsqlrepository.GetByWhere("Lang_ID=@1", new object[] { _langid }).ToList();
            _pagelayoutsqlrepository = new SQLRepository<PageLayout>(connectionStr);
            _adfunctionrepository=new SQLRepository<AdminFunction>(connectionStr);
            _seosqlrepository = new SQLRepository<SEO>(connectionStr);
            _IModelManager = new ModelManager(connectionStr);
            _GroupMessagesqlrepository= new SQLRepository<GroupMessage>(connectionStr);
            _GroupActivesqlrepository=new SQLRepository<GroupActive>(connectionStr);
            _SKrepository=new SQLRepository<CountSearchKey>(connectionStr);
        }
        #region SetModel2
        public void SetModel<T>(ref T model, string stype, string langid,string menuid) where T: MasterPageModel 
        {
            var tempmodel = GetModel(stype, langid, menuid);
            MapperConfiguration _configuration = new MapperConfiguration(cnf =>
            {
                cnf.CreateMap<MasterPageModel, T>();
            });
            var mapper = new Mapper(_configuration);
            mapper.DefaultContext.Mapper.Map(tempmodel, model);
        }
        #endregion

        #region GetModel
        public MasterPageModel GetModel(string stype, string langid,string mid)
        {
            var MasterPageModel = new MasterPageModel();
            
            //讀取logo圖片
            var layoutdata = _ISiteLayoutManager.GetSiteLayout(stype, langid);
            MasterPageModel.LogoUrl = layoutdata.LogoImageUrlThumb;
            MasterPageModel.InnerLogoUrl = layoutdata.InnerLogoImageUrlThumb;
            MasterPageModel.PublishContent = layoutdata.PublishContent;
            MasterPageModel.PrintContent = layoutdata.PrintHtmlContent;
            MasterPageModel.MasterMainTitle = layoutdata.FowardHtmlContent;
            //MasterPageModel.MasterMainTitle = _langdict.ContainsKey("國家高速網路與計算中心") ? _langdict["國家高速網路與計算中心"]: "" ;
            //if (MasterPageModel.MasterMainTitle == "") { MasterPageModel.MasterMainTitle = langid == "1" ? "國家高速網路與計算中心" : "National Center for High-performance Computing"; }
            MasterPageModel.SEOTitle = MasterPageModel.MasterMainTitle;
            var adset = _adsetsqlrepository.GetByWhere("Lang_ID=@1 and SType=@2", new object[] { langid, stype });
            MasterPageModel.FooterString = layoutdata.HtmlContent;
            var menuurl = _menuurlsqlrepository.GetAll().ToDictionary(v => v.MenuName, v => v.MenuPath);
            MasterPageModel.PrintImageUrl = layoutdata.PrintImageUrl;
            //if (langid == "1")
            //{
            //    MasterPageModel.TopMenu = MasterPageModel.TopMenu.Replace(">網站導覽<", "").Replace(">回首頁<", "");
            //}
            //else
            //{
            //    int i = MasterPageModel.TopMenu.LastIndexOf(">Home");
            //    if (i >= 0)
            //        MasterPageModel.TopMenu = MasterPageModel.TopMenu.Substring(0, i+1) + MasterPageModel.TopMenu.Substring(i + "Home".Length);
            //    MasterPageModel.TopMenu = MasterPageModel.TopMenu.Replace(">SiteMap<", "><");
            //}

            MasterPageModel.TopMenu = GetTopString(langid, "2", menuurl);
            MasterPageModel.TopMobileMenu = GetTopStringWithDisplayName(langid, "2", menuurl);
            
            MasterPageModel.MainMenu = GetMainString(langid, "1", menuurl);
            var langdict = new Dictionary<string, string>();
            MasterPageModel.LeftMenu = GetSubString(langid, mid, _langdict, menuurl);
            MasterPageModel.FooterMenu = GetFooterString(langid, menuurl);
            MasterPageModel.BannerImage = string.IsNullOrEmpty(layoutdata.InsidePageImgNameOri) ? "" : layoutdata.InsidePageImageUrl;
            MasterPageModel.LangId = langid;
            if (HttpContext.Current.Session["LangID"] == null) { HttpContext.Current.Session["LangID"] = langid; } else {
                if (HttpContext.Current.Session["LangID"].ToString() != langid) { HttpContext.Current.Session["LangID"] = langid; }
            }
            MasterPageModel.Fontsize = HttpContext.Current.Session["fontsize"]==null?"": HttpContext.Current.Session["fontsize"].ToString();
            //FirstPageModel.AdminHost = _hosturl;
            //FirstPageModel.ADDown = GetADDown(stype, langid);

            ////取得上方選單
            //if (stype == "M")
            //{
            //    FirstPageModel.MainMenu = GetMainString(langid,"4", menuurl);
            //    FirstPageModel.TopMenu = GetTopString(langid,"5", menuurl);
            //}
            //else {
            //   
            //    
            //}
            //
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var SK = _SKrepository.GetByWhere(" LangID=" + langid + " Order By Count Desc", null, "Top 3 SearchKey");
            MasterPageModel.SearchKey = string.Join("、", SK.Select(v => "<a href='"+ helper.Action("Search","Search",new { key = v.SearchKey })+"' key='" + v.SearchKey + "' class='topserch' title='" + v.SearchKey+"'> " + v.SearchKey + "</a>").ToArray());
            //FirstPageModel.SEOScript = new string[] { "", "", "" };
            //var indexmodel = _IModelManager.GetPageIndexSettingModel(langid);
            //FirstPageModel.IsShowSearch = indexmodel.IsInPage ? "Y" : "N";
            //FirstPageModel.ShowModel = 1;
            //FirstPageModel.Device = stype;
            //
            return MasterPageModel;
        }
        #endregion

        #region GetAdminMenuString
        public string GetAdminMenuString(string langid, string menutype, string groupid, string role, string openmenuid)
        {
           
            var auth = _functioninputsqlrepository.GetByWhere("GroupID=@1 and Type=1 Order by Type,ItemID", new object[] { groupid }).Select(v => v.ItemID).ToList();
            var sb = new StringBuilder();
            var allmenu = _menusqlrepository.GetByWhere("LangID=@1  and MenuType=@2 order by MenuLevel,Sort", new object[] { langid, menutype });
            var openlevel1id = -1;
            var openlevel2id = -1;
            try
            {
                if (openmenuid.IsNullorEmpty() == false)
                {
                    var menu = _menusqlrepository.GetByWhere("ID=@1", new object[] { openmenuid });
                    if (menu.First().MenuLevel == 2)
                    {
                        openlevel1id = menu.First().ParentID.Value;
                    }
                    else if (menu.First().MenuLevel == 3)
                    {
                        openlevel2id = menu.First().ParentID.Value;
                        var menu2 = _menusqlrepository.GetByWhere("ID=@1", new object[] { menu.First().ParentID.Value });
                        openlevel1id = menu2.First().ParentID.Value;
                    }

                }

            }
            catch (Exception ex) {
                NLogManagement.SystemLogInfo("KenTest=langid=" + langid + " menutype=" + menutype + " groupid=" + groupid + " role=" + role + " openmenuid=" + openmenuid+
                    " trace="+ ex.StackTrace);
                if (openmenuid.IsNullorEmpty() == false)
                {
                    var menu = _menusqlrepository.GetByWhere("ID=@1", new object[] { openmenuid });
                    if (menu.First().MenuLevel == 2)
                    {
                        openlevel1id = menu.First().ParentID.Value;
                    }
                    else if (menu.First().MenuLevel == 3)
                    {
                        openlevel2id = menu.First().ParentID.Value;
                        var menu2 = _menusqlrepository.GetByWhere("ID=@1", new object[] { menu.First().ParentID.Value });
                        openlevel1id = menu2.First().ParentID.Value;
                    }

                }
            }
          
            if (role != "admin") {
                allmenu = allmenu.Where(v => auth.Any(x => x == v.ID.Value));
            }
            foreach (var m in allmenu.Where(v => v.MenuLevel == 1))
            {
                if (m.ID == openlevel1id)
                {
                    sb.Append("<li class='nav-item letter open'>");
                }
                else {
                    sb.Append("<li class='nav-item letter'>");
                }

                var l2 = allmenu.Where(v => v.ParentID == m.ID);
                if (l2.Count() > 0)
                {
                    if (m.ID == openlevel1id)
                    {
                        sb.Append("<a href='#' class='nav-link nav-toggle' title='" + m.MenuName+ "'><i class='fa fa-dot-circle-o' aria-hidden='true'></i><span class='title' index='" + m.ID + "'>" + (m.DisplayName.IsNullorEmpty()? m.MenuName : m.DisplayName) + "</span><span class='arrow open'></span></a>");
                        sb.Append("<ul class='sub-menu' style='display:block'>");
                    }
                    else
                    {
                        sb.Append("<a href='#' class='nav-link nav-toggle' title='"+ m.MenuName+ "'><i class='fa fa-dot-circle-o' aria-hidden='true'></i><span class='title' index='" + m.ID + "'>" + (m.DisplayName.IsNullorEmpty() ? m.MenuName : m.DisplayName) + "</span><span class='arrow'></span></a>");
                        sb.Append("<ul class='sub-menu' >");
                    }


                    foreach (var m2 in l2)
                    {
                        var l3 = allmenu.Where(v => v.ParentID == m2.ID);
                        if (l3.Count() > 0)
                        {
                            if (m2.ID == openlevel2id)
                            {
                                sb.Append("<li class='nav-item open'><a href='#' class='nav-link nav-toggle' title='" + m2.MenuName+"'><span class='title' index='" + m2.ID + "'>" + (m2.DisplayName.IsNullorEmpty() ? m2.MenuName : m2.DisplayName) + "</span><span class='arrow open'></span></a>");
                                sb.Append("<ul class='sub-menu' style='display:block'>");
                            }
                            else {
                                sb.Append("<li class='nav-item'><a href='#' class='nav-link nav-toggle' title='"+ m2.MenuName +"'><span class='title' index='" + m2.ID + "'>" + (m2.DisplayName.IsNullorEmpty() ? m2.MenuName : m2.DisplayName) + "</span><span class='arrow'></span></a>");
                                sb.Append("<ul class='sub-menu'>");
                            }
                            foreach (var m3 in l3)
                            {
                                sb.Append("<li class='nav-visited'><a href='#' class='nav-link title' index='" + m3.ID + "' title='"+ m3.MenuName+"'>" + (m3.DisplayName.IsNullorEmpty() ? m3.MenuName : m3.DisplayName) + "</a></li>");
                            }
                            sb.Append("</ul>");
                        }
                        else
                        {
                            sb.Append("<li class='nav-item'><a href='#' class='nav-link' title='"+ m2.MenuName+"'><span class='title' index='" + m2.ID + "'>" + (m2.DisplayName.IsNullorEmpty() ? m2.MenuName : m2.DisplayName) + "</span></a>");
                        }
                        sb.Append("</li>");
                    }
                    sb.Append("</ul>");
                }
                else
                {
                    sb.Append("<a href='#' class='nav-link nav-toggle' title='"+ m.MenuName+ "'><i class='fa fa-dot-circle-o' aria-hidden='true'></i><span class='title' index='" + m.ID + "'>" + (m.DisplayName.IsNullorEmpty() ? m.MenuName : m.DisplayName) + "</span></span></a>");
                }
                sb.Append("</li>");
            }

            return sb.ToString();
        }
        #endregion

        #region GetLinkString
        public string GetLinkString(string langid, string menutype, string groupid, string role, string openmenuid)
        {
            if (openmenuid.IsNullorEmpty()) { return ""; }
            var menu = _menusqlrepository.GetByWhere("ID=@1", new object[] { openmenuid });
            var sb = new StringBuilder();
            sb.Append("<a href='#' title='Home'>Home</a><i class='fa fa-circle' aria-hidden='true'></i>");
            if (menutype == "1") { sb.Append("<a href = '#' title='主要選單'>主要選單 </a>"); }
            if (menutype == "2") { sb.Append("<a href = '#' title='上方選單'>上方選單 </a>"); }
            if (menutype == "3") { sb.Append("<a href = '#' title='下方選單'>下方選單 </a>"); }
            if (menutype == "4") { sb.Append("<a href = '#' title='手機版選單(主要選單)'>手機版選單(主要選單) </a>"); }
            if (menutype == "5") { sb.Append("<a href = '#' title='手機版選單(上方選單)'>手機版選單(上方選單) </a>"); }
            sb.Append("<i class='fa fa-circle' aria-hidden='true'></i>");
            if (menu.First().MenuLevel == 1)
            {
                sb.Append("<a href='#' title='" + menu.First().MenuName +"> " + menu.First().MenuName + "</a><i class='fa fa-circle' aria-hidden='true'></i>");
            }
            else if (menu.First().MenuLevel == 2)
            {
                var menu2 = _menusqlrepository.GetByWhere("ID=@1", new object[] { menu.First().ParentID.Value });
                sb.Append("<a href='#' title='" + menu2.First().MenuName + ">" + menu2.First().MenuName + "</a><i class='fa fa-circle' aria-hidden='true'></i>");
                sb.Append("<a href='#' title='" + menu.First().MenuName + ">" + menu.First().MenuName + "</a><i class='fa fa-circle' aria-hidden='true'></i>");
            }
            else if (menu.First().MenuLevel == 3)
            {
                var menu2 = _menusqlrepository.GetByWhere("ID=@1", new object[] { menu.First().ParentID.Value });
                var menu1 = _menusqlrepository.GetByWhere("ID=@1", new object[] { menu2.First().ParentID.Value });

                sb.Append("<a href='#' title='" + menu1.First().MenuName + ">" + menu1.First().MenuName + "</a><i class='fa fa-circle' aria-hidden='true'></i>");
                sb.Append("<a href='#' title='" + menu2.First().MenuName + ">" + menu2.First().MenuName + "</a><i class='fa fa-circle' aria-hidden='true'></i>");
                sb.Append("<a href='#' title='" + menu.First().MenuName + ">" + menu.First().MenuName + "</a><i class='fa fa-circle' aria-hidden='true'></i>");
            }
            return sb.ToString();
        }
        #endregion

        #region GetTopString
        public string GetTopString(string langid,string type, Dictionary<string, string> langdict, IDictionary<string, string> menuurl = null)
        {
            if (menuurl == null)
            {
                menuurl = _menuurlsqlrepository.GetAll().ToDictionary(v => v.MenuName, v => v.MenuPath);
            }
            var allmenu = _menusqlrepository.GetByWhere("LangID=@1 and  Status=@2 and MenuType=@3 order by MenuLevel,Sort", new object[] { langid, true, type });
            var sb = new StringBuilder();
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            foreach (var m in allmenu)
            {
                //if (m.ModelID == 6) {
                //    m.MenuName = m.DisplayName;
                //}
                sb.Append("<li>" + GetPageUrl(menuurl, m, helper, m.MenuLevel) + "</li>");
            }
            return sb.ToString();
        }
        #endregion

        #region GetTopDisplayNameString
        public string GetTopStringWithDisplayName(string langid, string type, Dictionary<string, string> langdict, IDictionary<string, string> menuurl = null)
        {
            if (menuurl == null)
            {
                menuurl = _menuurlsqlrepository.GetAll().ToDictionary(v => v.MenuName, v => v.MenuPath);
            }
            var allmenu = _menusqlrepository.GetByWhere("LangID=@1 and  Status=@2 and MenuType=@3 order by MenuLevel,Sort", new object[] { langid, true, type });
            var sb = new StringBuilder();
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            foreach (var m in allmenu)
            {
                if (m.MenuName == "EN")
                {
                    m.DisplayName = m.MenuName;
                    m.MenuName = m.MenuName+ "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + m.MenuName ;
                
                }
                else if (m.MenuName == "中文")
                {
                    m.DisplayName = m.MenuName;
                    m.MenuName = m.MenuName + "&nbsp;&nbsp;" + m.MenuName;

                }
                else {
                    m.ICon = m.ICon + m.MenuName;
                }
             
                sb.Append("<li>" + GetPageUrl(menuurl, m, helper, m.MenuLevel) + "</li>");
            }
            return sb.ToString();
        }
        #endregion

        #region GetMainString
        public string GetMainString(string langid,string type, Dictionary<string, string> langdict, IDictionary<string, string> menuurl = null)
        {
            if (menuurl == null)
            {
                menuurl = _menuurlsqlrepository.GetAll().ToDictionary(v => v.MenuName, v => v.MenuPath);
            }
            var allmenu = _menusqlrepository.GetByWhere("LangID=@1 and  Status=@2 and MenuType=@3 order by MenuLevel,Sort", new object[] { langid, true, type });
            var sb = new StringBuilder();
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var M = allmenu.Where(v => v.ID == 4);
            var x = 1;
            foreach (var m in allmenu.Where(v => v.MenuLevel == 1))
            {
                sb.Append($"<li class='dropdown' id='menu{x}'>");

                var l2 = allmenu.Where(v => v.ParentID == m.ID);
                if (l2.Count() > 0)
                {
                    if (m.OpenMode == 1)
                    {
                        var l2f = l2.First();
                        if (l2f.LinkMode == 1) {
                            var l3f = allmenu.Where(v => v.ParentID == l2f.ID);
                            if (l3f.Count() > 0)
                            {
                                Menu tempmenu = new Menu()
                                {
                                    ID = l3f.First().ID,
                                    DisplayName = m.DisplayName,
                                    LangID = l3f.First().LangID,
                                    LinkMode = l3f.First().LinkMode,
                                    LinkUrl = l3f.First().LinkUrl,
                                    MenuLevel = m.MenuLevel,
                                    MenuName = m.MenuName,
                                    MenuUrl = l3f.First().MenuUrl,
                                    OpenMode = l3f.First().OpenMode,
                                    MenuType = l3f.First().MenuType,
                                    ModelID = l3f.First().ModelID,
                                    ModelItemID = l3f.First().ModelItemID,
                                    ParentID = l3f.First().ParentID,
                                };
                                sb.Append(GetMainPageUrl(menuurl, tempmenu, helper, m.MenuLevel));
                            }
                            else { sb.Append(GetMainPageUrl(menuurl, m, helper, m.MenuLevel)); }
                        } else {
                            Menu tempmenu = new Menu()
                            {
                                ID = l2f.ID,
                                DisplayName = m.DisplayName,
                                LangID = l2f.LangID,
                                LinkMode = l2f.LinkMode,
                                LinkUrl = l2f.LinkUrl,
                                MenuLevel = m.MenuLevel,
                                MenuName = m.MenuName,
                                MenuUrl = l2f.MenuUrl,
                                OpenMode = l2f.OpenMode,
                                MenuType = l2f.MenuType,
                                ModelID = l2f.ModelID,
                                ModelItemID = l2f.ModelItemID,
                                ParentID = l2f.ParentID,
                                ICon= l2f.ICon
                            };
                            sb.Append(GetMainPageUrl(menuurl, tempmenu, helper, m.MenuLevel));
                        }
                    }
                    else {
                        sb.Append(GetMainPageUrl(menuurl, m, helper, m.MenuLevel));
                    }
                    sb.Append("<ul class='dropdown-menu'>");
                    foreach (var m2 in l2)
                    {
                        var l3 = allmenu.Where(v => v.ParentID == m2.ID);
                        if (l3.Count() > 0)
                        {
                            var l2menustr = "";
                            if (m2.LinkMode == 1)
                            {
                                var l3f = l3.First();
                                Menu tempmenu = new Menu()
                                {
                                    ID = l3f.ID,
                                    DisplayName = m2.DisplayName,
                                    LangID = l3f.LangID,
                                    LinkMode = l3f.LinkMode,
                                    LinkUrl = l3f.LinkUrl,
                                    MenuLevel = m2.MenuLevel,
                                    MenuName = m2.MenuName,
                                    MenuUrl = l3f.MenuUrl,
                                    OpenMode = l3f.OpenMode,
                                    MenuType = l3f.MenuType,
                                    ModelID = l3f.ModelID,
                                    ModelItemID = l3f.ModelItemID,
                                    ParentID = l3f.ParentID,
                                    ICon = l3f.ICon
                                };
                                l2menustr = GetPageUrl(menuurl, tempmenu, helper, 2);
                            }
                            else {
                                l2menustr = GetPageUrl(menuurl, m2, helper, 2);
                            }
                            l2menustr=l2menustr.Insert(2, " data-toggle='dropdown' ");
                            sb.Append("<li class='dropdown-submenu'>" + l2menustr);
                            sb.Append("<ul class='dropdown-menu'>");
                            foreach (var m3 in l3)
                            {
                                sb.Append("<li>" + GetPageUrl(menuurl, m3, helper,3) + "</li>");
                            }
                            sb.Append("</ul>");
                            sb.Append("</li>");
                        }
                        else {
                            sb.Append("<li>" + GetPageUrl(menuurl, m2, helper,2) + "</li>");
                        }
                    }
                    sb.Append("</ul>");
                }
                else
                {
                    var sbstr = GetMainPageUrl(menuurl, m, helper,m.MenuLevel);
                    sbstr = sbstr.Replace("class='dropdown-toggle tomenu'", "");
                    sbstr = sbstr.Replace("data-toggle='dropdown'", "");
                    sb.Append(sbstr);
                }
                sb.Append("</li>");
                
            }

            return sb.ToString();
        }
        #endregion

        #region GetFooterString
        public string GetFooterString(string langid, Dictionary<string, string> langdict, IDictionary<string, string> menuurl = null)
        {
            if (menuurl == null)
            {
                menuurl = _menuurlsqlrepository.GetAll().ToDictionary(v => v.MenuName, v => v.MenuPath);
            }
            var allmenu = _menusqlrepository.GetByWhere("LangID=@1 and  Status=@2 and MenuType=3 order by MenuLevel,Sort", new object[] { langid, true });
            var sb = new StringBuilder();
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            foreach (var m in allmenu)
            {
                var tempname = m.MenuName;
                sb.Append("<li data-sr='enter botom over 1.5s'>" + GetPageUrl(menuurl, m, helper, m.MenuLevel) +"</li>");
            }
            return sb.ToString();
        }
        #endregion

        #region GetSubString
        public string GetSubString(string langid, string mid, Dictionary<string, string> langkey, IDictionary<string, string> menuurl = null)
        {
            if (mid.IsNullorEmpty()) { return ""; }
            if (menuurl == null)
            {
                menuurl = _menuurlsqlrepository.GetAll().ToDictionary(v => v.MenuName, v => v.MenuPath);
            }
            var allmenu = _menusqlrepository.GetByWhere("LangID=@1 and  Status=@2  order by MenuLevel,Sort", new object[] { langid, true });
            var nowmid = allmenu.Where(v => v.ID == int.Parse(mid));
            var firstmid = mid;
            var menutype = -1;
            if (nowmid.Count() > 0)
            {
                menutype = nowmid.First().MenuType.Value;
                if (nowmid.First().ParentID != -1)
                {
                    firstmid = nowmid.First().ParentID.Value.ToString();
                    nowmid = allmenu.Where(v => v.ID == int.Parse(firstmid));
                    if (nowmid.Count() > 0)
                    {
                        if (nowmid.First().ParentID != -1) { firstmid = nowmid.First().ParentID.Value.ToString(); }
                    }
                }
            }
            allmenu = allmenu.Where(v => v.MenuType == menutype && (v.MenuLevel == 1 && v.ID == int.Parse(firstmid)) || v.MenuLevel >= 2);
            var sb = new StringBuilder();
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var midint = int.Parse(mid);
            var rightareastr = langkey.ContainsKey("右側選單") ? langkey["右側選單"] : "右側選單";
            foreach (var m in allmenu.Where(V => V.MenuLevel == 1))
            {
                sb.Append("<div class='menu_title'><a href='#;return false;' title='"+ rightareastr+"' accesskey='R'>:::</a>" + m.MenuName +"</div>");
                var l2 = allmenu.Where(v => v.ParentID == m.ID);
                if (l2.Count() > 0)
                {
                    sb.Append("<nav class='navbar sidebar right_menu' role='navigation'>");
                    sb.Append("<div class='navbar-collapse'><ul class='nav navbar-nav'>");
                    foreach (var m2 in l2)
                    {
                        var l3 = allmenu.Where(v => v.ParentID == m2.ID);
                        if (l3.Count() > 0)
                        {
                            if (l3.Any(v => v.ID == midint))
                            {
                                sb.Append("<li class='dropdown-submenu open'>" + GetSubPageUrl2(menuurl, m2, helper,true));
                            }
                            else
                            {
                                sb.Append("<li class='dropdown-submenu'>" + GetSubPageUrl2(menuurl, m2, helper,false));
                            }

                            sb.Append("<ul class='dropdown-menu' role='menu'>");
                            foreach (var m3 in l3)
                            {
                                Menu tempmenu = new Menu()
                                {
                                    ID = m3.ID,
                                    DisplayName = m3.DisplayName,
                                    LangID = m3.LangID,
                                    LinkMode = m3.LinkMode,
                                    LinkUrl = m3.LinkUrl,
                                    MenuLevel = 1,
                                    MenuName = m3.MenuName,
                                    MenuUrl = m3.MenuUrl,
                                    OpenMode = m3.OpenMode,
                                    MenuType = m3.MenuType,
                                    ModelID = m3.ModelID,
                                    ModelItemID = m3.ModelItemID,
                                    ParentID = m3.ParentID,
                                    ICon = m3.ICon,
                                };
                                sb.Append("<li>" + GetPageUrl(menuurl, tempmenu, helper,1));
                            }
                            sb.Append("</ul>");
                        }
                        else
                        {
                            Menu tempmenu = new Menu()
                            {
                                ID = m2.ID,
                                DisplayName = m2.DisplayName,
                                LangID = m2.LangID,
                                LinkMode = m2.LinkMode,
                                LinkUrl = m2.LinkUrl,
                                MenuLevel = 1,
                                MenuName = m2.MenuName,
                                MenuUrl = m2.MenuUrl,
                                OpenMode = m2.OpenMode,
                                MenuType = m2.MenuType,
                                ModelID = m2.ModelID,
                                ModelItemID = m2.ModelItemID,
                                ParentID = m2.ParentID,
                                ICon = m2.ICon,
                            };
                            sb.Append("<li>" + GetPageUrl(menuurl, tempmenu, helper,1));
                        }
                        sb.Append("</li>");
                    }
                    sb.Append("</ul></div>  </nav>");
                }
            }
            return sb.ToString();
        }
        #endregion

        #region GetSubString
        public string GetSubString(string sitemenuid)
        {
            var pagelayout = _pagelayoutsqlrepository.GetByWhere("ID=@1", new object[] { sitemenuid });
            var sb = new StringBuilder();
            sb.Append("<div class='left_title'>" + (pagelayout.Count() == 0 ? "" : pagelayout.First().Title) + "</div>");
            return sb.ToString();
        }
        #endregion

        #region GetSiteMenuLinkString
        public string GetSiteMenuLinkString(string sitemenuid)
        {
            var sb = new StringBuilder();
            //if (openmenuid.IsNullorEmpty()) { return ""; }
            //  <a href="@Url.Action("Index","Home")"><i class="fa fa-home"></i>首頁</a><i class="fa fa-angle-right"></i>@ViewBag.modelname
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            sb.Append("<a href='" + helper.Action("Index", "Home") + "' title='Home'><i class='fa fa-home' aria-hidden='true'></i>Home</a><i class='fa fa-angle-right' aria-hidden='true'></i>");
            var pagelayout = _pagelayoutsqlrepository.GetByWhere("ID=@1", new object[] { sitemenuid });

            sb.Append("<a href='#' title='"+ pagelayout.First().Title+"'> " + pagelayout.First().Title + "</a>");

            return sb.ToString();
        }
        #endregion

        #region GetLogo
        public string GetLogo(string langid, IDictionary<string, string> menuurl = null)
        {
            if (menuurl == null)
            {
                menuurl = _menuurlsqlrepository.GetAll().ToDictionary(v => v.MenuName, v => v.MenuPath);
            }
            var allmenu = _menusqlrepository.GetByWhere("LangID=@1 and  Status=@2 order by MenuLevel,Sort", new object[] { langid, true });
            var sb = new StringBuilder();
            foreach (var m in allmenu.Where(v => v.MenuType == 2))
            {
                if (m.LinkMode.Value == 3)
                {
                    if (m.LinkUrl.IsNullorEmpty() == false)
                    {
                        if (m.LinkUrl.ToLower().IndexOf("http") == 0)
                        {
                            sb.Append("<li><a href='" + m.LinkUrl + "' title='"+ m.MenuName+"'>" + m.MenuName + "</a></li>");
                        }
                        else if (m.LinkUrl.ToLower().IndexOf("~") == 0)
                        {
                            sb.Append("<li><a href='" + VirtualPathUtility.ToAbsolute(m.LinkUrl) + "' title='"+ m.MenuName+"'>" + m.MenuName + "</a></li>");
                        }
                        else
                        {
                            //helper
                            var area = HttpContext.Current.Request.RequestContext.RouteData.DataTokens["area"]; var linkarea = "";
                            if (area == null) { linkarea = ""; } else { linkarea = area.ToString(); }
                            var hostUrl = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority);
                            if (linkarea!="" &&m.LinkUrl.IndexOf(linkarea) >= 0)
                            {
                                sb.Append("<li><a href='" + hostUrl + "/" + linkarea + "/" + m.LinkUrl + "' title='"+ m.MenuName +"'>" + m.MenuName + "</a></li>");
                            }
                            else
                            {
                                sb.Append("<li><a href='" + hostUrl + "/" + m.LinkUrl + "' title='"+ m.MenuName +"'>" + m.MenuName + "</a></li>");
                            }
                        }
                    }
                    else
                    {
                        if (menuurl.ContainsKey(m.MenuName))
                        {
                            sb.Append("<li><a href='" + VirtualPathUtility.ToAbsolute(menuurl[m.MenuName]) + "' title='"+ m.MenuName +"'>" + m.MenuName + "</a></li>");
                        }
                        else
                        {
                            sb.Append("<li><a href='#' title='"+ m.MenuName+"'>" + m.MenuName + "</a></li>");
                        }
                    }
                }
                else if (m.LinkMode.Value == 2)
                {
                    if (m.ModelID == 6)
                    {
                        //搜尋設定
                        var lang = _ModelLangMainsqlrepository.GetByWhere("ID=@1", new object[] { m.ModelItemID });
                        if (lang.Count() > 0)
                        {
                            if (lang.First().UseType.Value == 1)
                            {
                                sb.Append("<li><a href='#' class='langchange' langid='" + lang.First().UseLangID + "' title='"+ m.MenuName+"'>" + m.MenuName + "</a></li>");
                            }
                            else
                            {
                                var alllang = _LangMainsqlrepository.GetByWhere("Enabled=@1 and Deleted=@2 and Published=@3", new object[] { true, false, true });
                                sb.Append("<li><a href='#' class='float_left' title='"+ m.MenuName +"> " + m.MenuName + "</a>");
                                sb.Append("<select class='form-control_1 sellang'>");
                                foreach (var l in alllang)
                                {
                                    sb.Append("<option value='" + l.ID + "'>" + l.Disp_Name + "</option>");
                                }
                                sb.Append("</select></li>");
                            }
                        }
                        else
                        {
                            sb.Append("<li><a href='#' title='"+ m.MenuName +"'>" + m.MenuName + "</a></li>");
                        }
                    }
                    else if (m.ModelID == 5)
                    {
                        if (menuurl.ContainsKey(m.MenuName))
                        {
                            sb.Append("<li><a href='" + VirtualPathUtility.ToAbsolute(menuurl[m.MenuName]) + "?itemid=" + m.ModelItemID + "' title='"+ m.MenuName+"'>" + m.MenuName + "</a></li>");
                        }
                        else { sb.Append("<li><a href='#' title='"+ m.MenuName+"'>" + m.MenuName + "</a></li>"); }

                    }

                }
                if (m.LinkMode.Value ==4)
                {
                    sb.Append("<li><a href='#' title='"+ m.MenuName+"'>" + m.MenuName + "</a></li>");
                }
                else
                {
                    sb.Append("<li><a href='#' title='"+ m.MenuName+"'>" + m.MenuName + "</a></li>");
                }
            }
            return sb.ToString();
        }
        #endregion

        #region GetADMain
        public IList<ADBase> GetADMain(string stype,string langid,int? site_id)
        {
            var admainmax  = _adsetsqlrepository.GetByWhere("Lang_ID=@1 and Type_ID='main' and SType=@2", new object[] { langid, stype }).ToList();
            var admain = _adminsqlrepository.GetByWhere("Lang_ID=@1 and Enabled=@2  and Stype=@3 Order By Sort", new object[] { langid, true, stype });
            if (admain.Count() == 0) { return new List<ADBase>(); }
            var maxnum = admainmax.Count()==0 ? 4 : admainmax.First().Max_Num.Value;
            var retuernList = new List<ADBase>();
            if (admain.Count() > 0)
            {
                admain = admain.Where(v => (v.StDate == null || v.StDate <= DateTime.Now) && (v.EdDate == null || v.EdDate.Value.AddDays(1) >= DateTime.Now));
                //var fixlist = admain.Where(v => v.Fixed == true).OrderBy(v => v.Sort).Take(maxnum);
                var fixlist = admain.Where(v => v.Fixed == true && v.Site_ID == site_id).OrderBy(v => v.Sort).Take(maxnum);

                foreach (var v in fixlist) { retuernList.Add(v); }
                if (retuernList.Count() < maxnum)
                {
                    //var nofix = admain.Where(v => v.Fixed == false).OrderBy(v => v.Sort).Take(maxnum).ToList();
                    var nofix = admain.Where(v => v.Fixed == false).OrderBy(v => v.Sort).ToList();
                    var random = new Random();
                    while (retuernList.Count() < maxnum)
                    {
                        if (nofix.Count() == 0) { break; }
                        int index = random.Next(nofix.Count());
                        retuernList.Add(nofix[index]);
                        nofix.RemoveAt(index);
                    }
                }
            }
            return retuernList;
        }
        #endregion

        #region GetPageUrl
        public string GetPageUrl(IDictionary<string, string> menuurl, Menu _menu, UrlHelper helper,int reallevel)
        {
            // var openmodestr  = "openmode='" + _menu.OpenMode + "'";
            var levelclass = "";
            if (reallevel == 2)
            {
                levelclass = "2nd";
            }
            if (reallevel == 3)
            {
                levelclass = "3rd";
            }
            var openmodestr = "";
            var opennewwindowstr = "另開新視窗";
            if (_langid == "2") {
                opennewwindowstr = "New Window";
            }
            if (_langdict != null && _langdict.ContainsKey("另開新視窗")) { opennewwindowstr = _langdict["另開新視窗"]; }
            var isopennewwinstr = "";
            var link = "";
            //var showmenun = _menu.ICon.IsNullorEmpty() ? _menu.MenuName : _menu.ICon;
            var showmenun = _menu.ICon.IsNullorEmpty() ? _menu.MenuName : _menu.MenuName;

            if (_menu.OpenMode == 2) { openmodestr = "target='_blank' class='" + levelclass + "'"; isopennewwinstr = "(" + opennewwindowstr + ")"; }
            else if (_menu.OpenMode == 3)
            {
                var constr = new List<string>();
                if (_menu.WindowWidth != null) { constr.Add("width=" + _menu.WindowWidth.Value); }
                if (_menu.WindowHeight != null)
                {
                    constr.Add("height=" + +_menu.WindowHeight.Value);
                }
                if (constr.Count() > 0)
                {
                    openmodestr = "class='newopen " + levelclass + "'";
                    if (_menu.WindowWidth != null)
                    {
                        openmodestr = openmodestr + " owidth='" + _menu.WindowWidth + "' class='" + levelclass + "'";
                    }
                    if (_menu.WindowHeight != null)
                    {
                        openmodestr = openmodestr + " oheight='" + _menu.WindowHeight + "' class='" + levelclass + "'";
                    }
                }
                else { openmodestr = "target='_blank' class='" + levelclass + "'"; _menu.OpenMode = 2; }
            }
            else {
                openmodestr = "class='" + levelclass + "'";
            }
            if (_menu.LinkMode.Value == 3)
            {
                if (_menu.LinkUrl.IsNullorEmpty() == false)
                {
                    if (_menu.LinkUrl.ToLower().IndexOf("http") >= 0)
                    {
                        link = _menu.LinkUrl;
                        return "<a "+ openmodestr+ "  href='" +(_menu.OpenMode == 3?"#": link)  + "'  style='cursor:pointer' type='3'"+
                            ( _menu.OpenMode != 3?"":" path='"+link+"'") + " title='"+ _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + "> " + showmenun + "</a>";
                    }
                    else if (_menu.LinkUrl.ToLower().IndexOf("~") == 0)
                    {
                        if (_menu.LinkUrl.IndexOf("mid=") >= 0)
                        {
                            link = helper.Content(_menu.LinkUrl);
                        }
                        else {
                            link = helper.Content(_menu.LinkUrl) + "?mid=" + _menu.ID;
                        }
                     
                        return "<a " + openmodestr + " href='" + (_menu.OpenMode == 3 ? "#" : link) + "'  style='cursor:pointer' type='3'" +
                            (_menu.OpenMode != 3 ? "" : " path='" + link + "'") + " title='" + _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
                    }
                    else
                    {
                        //helper
                        if (_menu.LinkUrl.IndexOf("mid=") >= 0)
                        {
                            link = VirtualPathUtility.ToAbsolute("~/")+ _menu.LinkUrl;
                        }
                        else
                        {
                            link = VirtualPathUtility.ToAbsolute("~/") + _menu.LinkUrl + "?mid=" + _menu.ID;
                        }
                        return "<a " + openmodestr + " href='" + (_menu.OpenMode == 3 ? "#" : link) + "'  style='cursor:pointer' type='3'" + 
                            (_menu.OpenMode != 3 ? "" : " path='" + link + "'") + " title='" + _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
                        //var area = HttpContext.Current.Request.RequestContext.RouteData.DataTokens["area"]; var linkarea = "";
                        //if (area == null) { linkarea = ""; } else { linkarea = area.ToString(); }
                        //var hostUrl = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority);
                        //if (linkarea != "" && _menu.LinkUrl.IndexOf(linkarea) >= 0)
                        //{
                        //    if (_menu.LinkUrl.IndexOf("mid=") >= 0)
                        //    {
                        //        link = hostUrl + "/" + linkarea + "/" + _menu.LinkUrl;
                        //    }
                        //    else
                        //    {
                        //        link = hostUrl + "/" + linkarea + "/" + _menu.LinkUrl + "?mid=" + _menu.ID;
                        //    }

                        //    return "<a " + openmodestr + "  href='" + (_menu.OpenMode == 3 ? "#" : link) + "'  style='cursor:pointer' type='3'" + (_menu.OpenMode != 3 ? "" : " path='" + link + "'") + " title='"+ _menu.MenuName  + isopennewwinstr + "'>" + _menu.MenuName + "</a>";
                        //}
                        //else
                        //{

                        //    if (_menu.LinkUrl.IndexOf("mid=") >= 0)
                        //    {
                        //        link = hostUrl + "/" + linkarea + "/" + _menu.LinkUrl;
                        //    }
                        //    else
                        //    {
                        //        link = hostUrl + "/" + linkarea + "/" + _menu.LinkUrl + "?mid=" + _menu.ID;
                        //    }
                        //    return "<a " + openmodestr + "  href='" + (_menu.OpenMode == 3 ? "#" : link) + "'  style='cursor:pointer' type='3'" +
                        //        (_menu.OpenMode != 3 ? "" : " path='" + link + "'") + " title='"+ _menu.MenuName  + isopennewwinstr + "'>" + _menu.MenuName + "</a>";
                        //}

                    }
                }
                else
                {
                    if (menuurl.ContainsKey(_menu.MenuName))
                    {
                        link = helper.Content(menuurl[_menu.MenuName]) + "?mid=" + _menu.ID;
                        return "<a " + openmodestr + "  href='" + (_menu.OpenMode == 3 ? "#" : link) + "'  style='cursor:pointer' type='3'" + 
                            (_menu.OpenMode != 3 ? "" : " path='" + link + "'") + " title='"+ _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
                    }
                    else
                    {
                        return "<a  class='"+ levelclass+"' href = '#' title='" + _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
                    }
                }
            }
            else if (_menu.LinkMode.Value == 4)
            {
                link = helper.Action("FileDownLoad", "Home", new { mid = _menu.ID });
                return "<a " + openmodestr + " href='" + (_menu.OpenMode == 3 ? "#" : link) + "'  style='cursor:pointer' type='4'" +
                    (_menu.OpenMode != 3 ? "" : " path='" + link + "'") + " title='"+ _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
            }
            else if (_menu.LinkMode.Value == 2)
            {
                if (_menu.ModelID == 6)
                {
                    var lang = _ModelLangMainsqlrepository.GetByWhere("ID=@1", new object[] { _menu.ModelItemID });
                    if (lang.Count() > 0)
                    {
                        if (lang.First().UseType.Value == 1)
                        {
                            return "<a href='#' class='langchange " + levelclass + "' langid='" + lang.First().UseLangID + "' title='" + _menu.MenuName + isopennewwinstr +
                                "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
                        }
                        else
                        {
                            var alllang = _LangMainsqlrepository.GetByWhere("Enabled=@1 and Deleted=@2 and Published=@3", new object[] { true, false, true });
                            var sb = new StringBuilder();
                            sb.Append("<a href='#' class='float_left " + levelclass + "' title='" + _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") +
                                ">" + showmenun + "</a><select class='form-control_1 sellang'>");
                            foreach (var l in alllang)
                            {
                                sb.Append("<option value='" + l.ID + "'>" + l.Disp_Name + "</option>");
                            }
                            sb.Append("</select>");
                            return sb.ToString();
                        }
                    }
                    else
                    {
                        return "<a class='"+ levelclass+"' href = '#' title='"+ _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
                    }

                }
                else
                {
                    var path = "Home";
                    if (_menu.ModelID == 1) { path = "Page"; }
                    if (_menu.ModelID == 2) { path = "Message"; }
                    if (_menu.ModelID == 3) { path = "Active"; }
                    if (_menu.ModelID == 4) { path = "Download"; }
                    if (_menu.ModelID == 5) { path = "Map"; }
                    if (_menu.ModelID == 7) { path = "Article"; }
                    if (_menu.ModelID == 8) { path = "Knowledge"; }
                    if (_menu.ModelID == 9) { path = "Forum"; }
                    if (_menu.ModelID == 10) { path = "Industry"; }
                    if (_menu.ModelID == 11) { path = "Form"; }
                    if (_menu.ModelID == 12) { path = "Course"; }
                    if (_menu.ModelID == 13) { path = "Product"; }
                    if (_menu.ModelID == 14) { path = "Cart"; }
                    if (_menu.ModelID == 15) { path = "BaseData"; }
                    if (_menu.ModelID == 16) { path = "FAQ"; }
                    if (_menu.ModelID == 17) { path = "Event"; }
                    if (_menu.ModelID == 18) { path = "Videos"; }
                    if (_menu.ModelID == 19) { path = "Patent"; }
                    if (_menu.ModelID == 14)
                    {
                        link = helper.Action("Index", path, new { itemid = _menu.ModelItemID, mid = _menu.ID });
                        return "<a " + openmodestr + " href='" + (_menu.OpenMode == 3 ? "#" : link) + "' style='cursor:pointer' type='2'" +
                            (_menu.OpenMode != 3 ? "" : " path='" + link + "'") + " title='" + _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">"
                            + showmenun + "<span id='cartnum'></span></a>";
                    } else if (_menu.ModelID ==3) { 
                    link = helper.Action("Index", path, new { itemid = _menu.ModelItemID, mid = _menu.ID });
                    return "<a " + openmodestr + "  href='" + (_menu.OpenMode == 3 ? "#" : link) + "' style='cursor:pointer' type='2'" +
                            (_menu.OpenMode != 3 ? "" : " path='" + link + "'") + " title='" + _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
                }

                    else
                    {
                        link = helper.Action("Index", path, new { itemid = _menu.ModelItemID, mid = _menu.ID });
                        return "<a " + openmodestr + "  href='" + (_menu.OpenMode == 3 ? "#" : link) + "' style='cursor:pointer' type='2'" + (_menu.OpenMode != 3 ? "" : " path='" + link + "'") 
                            + " title='" + _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>"; }

                  }
            }
            else
            {
                return "<a class='"+ levelclass+"' href = '#' title='" + _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
            }

        }
        #endregion

        #region GetSubPageUrl2
        public string GetSubPageUrl2(IDictionary<string, string> menuurl, Menu _menu, UrlHelper helper,bool linkopen)
        {
            var linkstr = "";
            var showmenun = _menu.ICon.IsNullorEmpty() ? _menu.MenuName : _menu.ICon;
            var opennewwindowstr = "另開新視窗";
            if (_langid == "2")
            {
                opennewwindowstr = "New Window";
            }
            var isopennewwinstr = "";
            if (_langdict != null && _langdict.ContainsKey("另開新視窗")) { isopennewwinstr = "(" + opennewwindowstr + ")"; }
            if (_menu.LinkMode.Value == 3)
            {
                if (_menu.LinkUrl.IsNullorEmpty() == false)
                {
                    if (_menu.LinkUrl.ToLower().IndexOf("http") == 0)
                    {
                        return "<a href='" + _menu.LinkUrl  + "'  style='cursor:pointer' type='3' title='" + _menu.MenuName + "'>" + showmenun + "</a>";
                    }
                    else if (_menu.LinkUrl.ToLower().IndexOf("~") == 0)
                    {
                        return "<a href='" + VirtualPathUtility.ToAbsolute(_menu.LinkUrl) + "?mid=" + _menu.ID + "'  style='cursor:pointer' type='3' title='"+ _menu.MenuName + isopennewwinstr + "'>" + showmenun + "</a>";
                    }
                    else
                    {
                        var area = HttpContext.Current.Request.RequestContext.RouteData.DataTokens["area"]; var linkarea = "";
                        if (area == null) { linkarea = ""; } else { linkarea = area.ToString(); }
                        var hostUrl = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority);
                        if (linkarea!=""&&_menu.LinkUrl.IndexOf(linkarea) >= 0)
                        {
                            return "<a href='" + hostUrl + "/" + linkarea + "/" + _menu.LinkUrl + "?mid=" + _menu.ID + "'  style='cursor:pointer' type='3' title='"+ _menu.MenuName + isopennewwinstr + "'>" + showmenun + "</a>";
                        }
                        else
                        {
                            return "<a href='" + hostUrl + "/" + _menu.LinkUrl + "?mid=" + _menu.ID + "'  style='cursor:pointer' type='3' title="+ _menu.MenuName + isopennewwinstr + "'>" + showmenun + "</a>";
                        }
                    }
                }
                else
                {
                    if (menuurl.ContainsKey(_menu.MenuName))
                    {
                        return "<a href='" + VirtualPathUtility.ToAbsolute(menuurl[_menu.MenuName]) + "?mid=" + _menu.ID + "' class='dropdown-toggle' data-toggle='dropdown' title='"+
                            _menu.MenuName + isopennewwinstr + "'>" + showmenun + "<i class='fa fa-caret-down' aria-hidden='true'></i></a>";
                    }
                    else
                    {
                        return "<a href='#' class='dropdown-toggle' data-toggle='dropdown' title='" + _menu.MenuName + isopennewwinstr + "'>" + showmenun + "<i class='fa fa-caret-down' aria-hidden='true'></i></a>";
                    }
                }
            }
            else if (_menu.LinkMode.Value == 4)
            {
                return "<a href='"+helper.Action("FileDownLoad","Home",new { mid= _menu.ID })+ "'  style='cursor:pointer' class='dropdown-toggle' data-toggle='dropdown' title='"+
                    _menu.MenuName + isopennewwinstr + "'> " + showmenun + "</a>";
            }
            else if (_menu.LinkMode.Value == 2)
            {
                if (_menu.ModelID == 6)
                {
                    //搜尋設定
                    var lang = _ModelLangMainsqlrepository.GetByWhere("ID=@1", new object[] { _menu.ModelItemID });
                    if (lang.Count() > 0)
                    {
                        if (lang.First().UseType.Value == 1)
                        {
                            return "<a href='#' class='langchange' langid='" + lang.First().UseLangID + "' class='dropdown-toggle' data-toggle='dropdown' title='" + _menu.MenuName + isopennewwinstr + "'>" 
                                + showmenun + "<i class='fa fa-caret-down' aria-hidden='true'></i></a>";
                        }
                        else
                        {
                            var alllang = _LangMainsqlrepository.GetByWhere("Enabled=@1 and Deleted=@2 and Published=@3", new object[] { true, false, true });
                            var sb = new StringBuilder();
                            sb.Append("<a href='#' class='dropdown-toggle' data-toggle='dropdown' title='"+ _menu.MenuName + isopennewwinstr + "'>" + showmenun + "</a><select class='form-control_1 sellang'>");
                            foreach (var l in alllang)
                            {
                                sb.Append("<option value='" + l.ID + "'>" + l.Disp_Name + "</option>");
                            }
                            sb.Append("</select>");
                            return sb.ToString();
                        }
                    }
                    else
                    {
                        return "<a href='#' class='dropdown-toggle' data-toggle='dropdown' title='"+ _menu.MenuName + isopennewwinstr + "'>" + showmenun + "<i class='fa fa-caret-down' aria-hidden='true'></i></a>";
                    }
                }
                else
                {
                    var path = "Home";
                    if (_menu.ModelID == 1) { path = "Page"; }
                    if (_menu.ModelID == 2) { path = "Message"; }
                    if (_menu.ModelID == 3) { path = "Active"; }
                    if (_menu.ModelID == 4) { path = "Download"; }
                    if (_menu.ModelID == 5) { path = "Map"; }
                    if (_menu.ModelID == 7) { path = "Article"; }
                    if (_menu.ModelID == 8) { path = "Knowledge"; }
                    if (_menu.ModelID == 9) { path = "Forum"; }
                    if (_menu.ModelID == 10) { path = "Industry"; }
                    if (_menu.ModelID == 11) { path = "Form"; }
                    if (_menu.ModelID == 12) { path = "Course"; }
                    if (_menu.ModelID == 13) { path = "Product"; }
                    if (_menu.ModelID == 14) { path = "Cart"; }
                    if (_menu.ModelID == 15) { path = "BaseData"; }
                    if (_menu.ModelID == 16) { path = "FAQ"; }
                    if (_menu.ModelID == 17) { path = "Event"; }
                    if (_menu.ModelID == 18) { path = "Videos"; }
                    if (_menu.ModelID == 19) { path = "Patent"; }
                    return "<a href='" + helper.Action("Index", path) + "?itemid=" + _menu.ModelItemID + "&mid=" + _menu.ID + "' class='dropdown-toggle' data-toggle='dropdown' title='"
                        + _menu.MenuName + isopennewwinstr + "'>" + showmenun + "<i class='fa fa-caret-down' aria-hidden='true'></i></a>";
                }
            }
            else
            {
                // return "<a href='#' class='dropdown-toggle' data-toggle='dropdown' aria-expanded='true'>" + _menu.MenuName + "<i class='fa fa-caret-down' aria-hidden='true'></i></a>";
                return "<a href='#' class='dropdown-toggle' data-toggle='dropdown' title='" + _menu.MenuName + isopennewwinstr + "'>" + showmenun + "<i class='fa fa-caret-down' aria-hidden='true'></i></a>";
            }

        }
        #endregion

        #region GetMainPageUrl
        public string GetMainPageUrl(IDictionary<string, string> menuurl, Menu _menu, UrlHelper helper,int reallevel)
        {
            var openmodestr = "";
            var link = "";
            var showmenun = _menu.ICon.IsNullorEmpty() ? _menu.MenuName : _menu.ICon;
            var opennewwindowstr = "另開新視窗";
            if (_langid == "2")
            {
                opennewwindowstr = "New Window";
            }
            var isopennewwinstr = "";
            var levelclass = "";
            if (reallevel == 2) {
                levelclass = "2nd";
            }
            if (reallevel == 3)
            {
                levelclass = "3rd";
            }
            if (_langdict != null && _langdict.ContainsKey("另開新視窗")) { opennewwindowstr = _langdict["另開新視窗"]; }
            if (_menu.OpenMode == 2) { openmodestr = "target='_blank'"; isopennewwinstr = "(" + opennewwindowstr + ")"; }
            else if (_menu.OpenMode == 3)
            {
                var constr = new List<string>();
                if (_menu.WindowWidth != null) { constr.Add("width=" + _menu.WindowWidth.Value); }
                if (_menu.WindowHeight != null)
                {
                    constr.Add("height=" + +_menu.WindowHeight.Value);
                }
                if (constr.Count() > 0)
                {
                    openmodestr = "class='newopen'";
                    if (_menu.WindowWidth != null)
                    {
                        openmodestr = openmodestr + " owidth='" + _menu.WindowWidth + "'";
                    }
                    if (_menu.WindowHeight != null)
                    {
                        openmodestr = openmodestr + " oheight='" + _menu.WindowHeight + "'";
                    }
                }
                else { openmodestr = "target='_blank'"; _menu.OpenMode = 2; }
            }

            if (_menu.LinkMode.Value == 3)
            {
                if (_menu.LinkUrl.IsNullorEmpty() == false)
                {
                    //HttpContext.Current.Request.Url.Authority
                    if (_menu.LinkUrl.ToLower().IndexOf("http") == 0)
                    {
                        link = _menu.LinkUrl;
                        return "<a "+ openmodestr+" href = '" + (_menu.OpenMode == 3 ? "#" : link) + "' class='dropdown-toggle tomenu "+ levelclass+"' data-toggle='dropdown' " +
                            (_menu.OpenMode != 3 ? "" : " path='" + link + "'") + " title='"+ _menu.MenuName + isopennewwinstr + "'"+(_menu.MenuLevel>1? " tabindex='-1'":"")+">" + showmenun + "</a>";
                    }
                    else if (_menu.LinkUrl.ToLower().IndexOf("~") == 0)
                    {
                        link = VirtualPathUtility.ToAbsolute(_menu.LinkUrl) + "?mid=" + _menu.ID;
                        return "<a " + openmodestr + "    href='" + (_menu.OpenMode == 3 ? "#" : link) + "'  class='dropdown-toggle tomenu " + levelclass + "' data-toggle='dropdown' " +
                            (_menu.OpenMode != 3 ? "" : " path='" + link + "'") + " title='"+ _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
                    }
                    else
                    {
                        var area = HttpContext.Current.Request.RequestContext.RouteData.DataTokens["area"];
                        var linkarea = "";
                        if (area == null) { linkarea = ""; } else { linkarea = area.ToString(); }
                        var hostUrl = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority);
                        if (linkarea!=""&&_menu.LinkUrl.IndexOf(linkarea) >= 0)
                        {
                            link = hostUrl + "/" + linkarea + "/" + _menu.LinkUrl + "?mid=" + _menu.ID;
                            return "<a " + openmodestr + "   href='" + (_menu.OpenMode == 3 ? "#" : link) + "'  class='dropdown-toggle tomenu " + levelclass + "' data-toggle='dropdown' " +
                                (_menu.OpenMode != 3 ? "" : " path='" + link + "'") + " title='"+ _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
                        }
                        else
                        {
                            link = hostUrl + "/" + _menu.LinkUrl + "?mid=" + _menu.ID;
                            return "<a " + openmodestr + "   href = '" + (_menu.OpenMode == 3 ? "#" : link) + "'  class='dropdown-toggle tomenu " + levelclass + "' data-toggle='dropdown' " +
                                (_menu.OpenMode != 3 ? "" : " path='" + link + "'") + " title='" + _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
                        }

                    }
                }
                else
                {
                    if (menuurl.ContainsKey(_menu.MenuName))
                    {
                        link = VirtualPathUtility.ToAbsolute(menuurl[_menu.MenuName]) + "?mid=" + _menu.ID;
                        return "<a " + openmodestr + "   href='" + (_menu.OpenMode == 3 ? "#" : link) + "' class='dropdown-toggle tomenu " + levelclass + "' data-toggle='dropdown' " +
                            (_menu.OpenMode != 3 ? "" : " path='" + link + "'") + " title='"+ _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
                    }
                    else
                    {
                        return "<a href='#' class='dropdown-toggle " + levelclass + "' data-toggle='dropdown'  title='" + _menu.MenuName + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
                    }
                }
            }
            else if (_menu.LinkMode.Value == 4)
            {
                link = helper.Action("FileDownLoad", "Home", new { mid = _menu.ID });
                return "<a href = '" + helper.Action("FileDownLoad", "Home", new { mid = _menu.ID })+
                    "'  style='cursor:pointer' type='4'  class='dropdown-toggle tomenu " + levelclass + "' data-toggle='dropdown'  title='" + _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + "> " + showmenun + "</a>";
            }
            else if (_menu.LinkMode.Value == 2)
            {
                if (_menu.ModelID == 6)
                {
                    var lang = _ModelLangMainsqlrepository.GetByWhere("ID=@1", new object[] { _menu.ModelItemID });
                    if (lang.Count() > 0)
                    {
                        if (lang.First().UseType.Value == 1)
                        {
                            return "<a href='#' class='dropdown-toggle langchange " + levelclass + "' data-toggle='dropdown'  langid='" + lang.First().UseLangID + 
                                "' title='"+ _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
                        }
                        else
                        {
                            var alllang = _LangMainsqlrepository.GetByWhere("Enabled=@1 and Deleted=@2 and Published=@3", new object[] { true, false, true });
                            var sb = new StringBuilder();
                            sb.Append("<a href='#' class='float_left " + levelclass + "' title='" + _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a><select class='form-control_1 sellang'>");
                            foreach (var l in alllang)
                            {
                                sb.Append("<option value='" + l.ID + "'>" + l.Disp_Name + "</option>");
                            }
                            sb.Append("</select>");
                            return sb.ToString();
                        }
                    }
                    else
                    {
                        return "<a href='#' title=" + _menu.MenuName + isopennewwinstr + "> " + showmenun + "</a>";
                    }
                }
                else
                {
                    var path = "Home";
                    if (_menu.ModelID == 1) { path = "Page"; }
                    if (_menu.ModelID == 2) { path = "Message"; }
                    if (_menu.ModelID == 3) { path = "Active"; }
                    if (_menu.ModelID == 4) { path = "Download"; }
                    if (_menu.ModelID == 5) { path = "Map"; }
                    if (_menu.ModelID == 7) { path = "Article"; }
                    if (_menu.ModelID == 8) { path = "Knowledge"; }
                    if (_menu.ModelID == 9) { path = "Forum"; }
                    if (_menu.ModelID == 10) { path = "Industry"; }
                    if (_menu.ModelID == 11) { path = "Form"; }
                    if (_menu.ModelID == 12) { path = "Course"; }
                    if (_menu.ModelID == 13) { path = "Product"; }
                    if (_menu.ModelID == 14) { path = "Cart"; }
                    if (_menu.ModelID == 15) { path = "BaseData"; }
                    if (_menu.ModelID == 16) { path = "FAQ"; }
                    if (_menu.ModelID == 17) { path = "Event"; }
                    if (_menu.ModelID == 18) { path = "Videos"; }
                    if (_menu.ModelID == 19) { path = "Patent"; }
                    link = helper.Action("Index", path) + "?itemid=" + _menu.ModelItemID + "&mid=" + _menu.ID;
                    return "<a " + openmodestr + " href='" + (_menu.OpenMode == 3 ? "#" : link) + "' class='dropdown-toggle tomenu " + levelclass + "' data-toggle='dropdown' " + (_menu.OpenMode != 3 ? "" : " path='" + link + "'") 
                        + " title='"+ _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + ">" + showmenun + "</a>";
                }
            }
            else
            {
                return "<a href='#' class='dropdown-toggle " + levelclass + "' data-toggle='dropdown' title='" + _menu.MenuName + isopennewwinstr + "'" + (_menu.MenuLevel > 1 ? " tabindex='-1'" : "") + "> " + showmenun + "</a>";
            }

        }
        #endregion

        #region GetSiteLayout
        public List<HomePageLayoutModel> GetSiteLayout(List<PageLayout> sitemenu,string title,string langid)
        {
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var rootpath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
            var idx = 1;
            List<PageLayout> tempsitemenu = sitemenu;
            if (title != "") {  tempsitemenu = sitemenu.Where(v => v.Title == title && v.LangID==int.Parse(langid)).ToList(); }
            var pagelayout = new List<HomePageLayoutModel>();
            foreach (var m in tempsitemenu)
            {
                var usemodel = 0;
                var usemodelitem = 0;
                var HomePageLayout = new HomePageLayoutModel();
                var menuid =-1;
                HomePageLayout.Name = m.Title;
                if (m.LinkMode == 1)
                {
                    var menuitem = m.MenuItem;
                    var l1id = m.MenuLevel1;
                    var l2id = m.MenuLevel2;
                    var l3id = m.MenuLevel3;
                    var lastmenuid = l2id == null ? l1id : l2id;
                    lastmenuid = l3id == null ? lastmenuid : l3id;
                    if (lastmenuid != null)
                    {
                        var menu = _menusqlrepository.GetByWhere("ID=@1", new object[] { lastmenuid });
                        if (menu.Count() > 0) {
                            usemodel = menu.First().ModelID;
                            usemodelitem = menu.First().ModelItemID;
                            menuid = menu.First().ID.Value;
                        }
                    }
                }
                else
                {
                    usemodel = m.ModelID.Value;
                    usemodelitem = m.ModelItemID.Value;
                }
                if (usemodel == 1)
                {
                    var pdata = _PageIndexItemsqlrepository.GetByWhere("ModelID=@1 And Enabled=1 and IsVerift=1 Order By Sort ", new object[] { usemodelitem });
                    if (pdata.Count() > 0)
                    {
                        var arrpdata = pdata.Take(5).ToArray();
                        for (var pidx = 0; pidx < arrpdata.Count(); pidx++)
                        {
                            HomePageLayout.LinkUrl.Add(helper.Action("Index", "Page") + "?itemid=" + usemodelitem + "&pageitemid=" + arrpdata[pidx].ItemID 
                                + "&menutype=0&sitemenuid=" + m.ID+"&mid="+ menuid);
                            if (arrpdata[pidx].ImageFileName.IsNullorEmpty() == false)
                            {
                                HomePageLayout.LinkImageSrc.Add(helper.Content("~/UploadImage/SiteLayout/" + arrpdata[pidx].ImageFileName));
                            }
                            else {
                                HomePageLayout.LinkImageSrc.Add("");
                            }
                            HomePageLayout.ModelGroup.Add("");
                            HomePageLayout.PublicshDate.Add(arrpdata[pidx].UpdateDatetime.Value.ToString("yyyy.MM.dd")) ;
                            HomePageLayout.Title.Add(arrpdata[pidx].ItemName);
                            HomePageLayout.JustView.Add(false);
                            if (pidx == 0)
                            {
                                var content = arrpdata[pidx].HtmlContent == null ? "" : arrpdata[pidx].HtmlContent.TrimgHtmlTag();
                                if (content.Length >= 100) { content = content.Substring(0, 100); }
                                HomePageLayout.HtmlContent.Add(content);
                            }else { HomePageLayout.HtmlContent.Add(""); }
                        }
                        if (m.OpenMode != 2)
                        {
                            if (m.OpenMode == 3)
                            {
                                HomePageLayout.MoreLink = m.OpenModeCust + "?itemid=" + usemodelitem;
                            }
                            else
                            {
                                HomePageLayout.MoreLink = helper.Action("Index", "Page") + "?itemid=" + usemodelitem+(menuid==-1?"":"&mid="+ menuid);
                            }
                        }
                    }
                }
                else if (usemodel == 2)
                {
                    var nowdate = DateTime.Now.ToString("yyyy/MM/dd");
                    var modelitem = m.ModelItemList.IsNullorEmpty() ? "" : m.ModelItemList;
                    IList<MessageItem> pdata;
                    if (modelitem .IsNullorEmpty())
                    {
                        pdata = _MessageItemsqlrepository.GetByWhere("ModelID=@1 AND (EdDate IS NULL OR EdDate >=@2)  AND (StDate IS NULL OR StDate <=@2)" +
                       " AND Enabled=1  and IsVerift=1 Order By Sort", new object[] { usemodelitem, nowdate }).ToList();
                    }
                    else {
                        pdata = _MessageItemsqlrepository.GetByWhere("ItemID In (" + modelitem + ") and IsVerift=1", new object[0]).ToList();
                    }
               
                    if (pdata.Count() > 0)
                    {
                        var arrpdata = pdata.Take(5).ToArray();
                        for (var pidx = 0; pidx < arrpdata.Count(); pidx++)
                        {
                            HomePageLayout.LinkUrl.Add(helper.Action("MessageView", "Message") + "?id=" + arrpdata[pidx].ItemID + "&menutype=0&sitemenuid=" +
                                m.ID + "&mid=" + menuid);
                            var pgroup = _GroupMessagesqlrepository.GetByWhere("ID=@1", new object[] { arrpdata[pidx].GroupID });
                            if (pgroup.Count() > 0) { HomePageLayout.ModelGroup.Add(pgroup.First().Group_Name); } else { HomePageLayout.ModelGroup.Add(""); }
                            if (arrpdata[pidx].RelateImageFileName.IsNullorEmpty() == false)
                            {
                                HomePageLayout.LinkImageSrc.Add(helper.Content("~/UploadImage/MessageItem/" + arrpdata[pidx].RelateImageFileName)); }
                            else {
                                var baseimg = helper.Content("~/ContentWebsite/image/logo_400x300.jpg");
                                HomePageLayout.LinkImageSrc.Add(baseimg);
                            }
                            HomePageLayout.PublicshDate.Add(arrpdata[pidx].PublicshDate.Value.ToString("yyyy/MM/dd"));
                            HomePageLayout.JustView.Add(arrpdata[pidx].Link_Mode == 1 ? false : true);
                            var content = arrpdata[pidx].Introduction == null ? "" : arrpdata[pidx].Introduction.TrimgHtmlTag();
                            if (content.Length >= 1000) { content = content.Substring(0, 1000); } 
                            HomePageLayout.HtmlContent.Add(content);
                            var titlestr = arrpdata[pidx].Title == null ? "" : arrpdata[pidx].Title;
                            if (titlestr.Length > 30) { titlestr = titlestr.Substring(0, 30); }
                            HomePageLayout.Title.Add(titlestr);
                        }
                        if (m.OpenMode != 2)
                        {
                            if (m.OpenMode == 3)
                            {
                                HomePageLayout.MoreLink = m.OpenModeCust + "?itemid=" + usemodelitem;
                            }
                            else
                            {
                                HomePageLayout.MoreLink = helper.Action("Index", "Message") + "?itemid=" + usemodelitem + (menuid == -1 ? "" : "&mid=" + menuid);
                            }
                        }
                    }
                }
                else if (usemodel == 3)
                {
                    var nowdate = DateTime.Now.ToString("yyyy/MM/dd");
                    var modelitem = m.ModelItemList.IsNullorEmpty() ? "" : m.ModelItemList;
                    IList<ActiveItem> pdata;
                    if (modelitem.IsNullorEmpty())
                    {
                        pdata = _ActiveItemsqlrepository.GetByWhere("ModelID=@1 AND (EdDate IS NULL OR EdDate >=@2)  AND (StDate IS NULL OR StDate <=@2)" +
                       " AND Enabled=1  and IsVerift=1 Order By Sort", new object[] { usemodelitem, nowdate }).ToList();
                    }
                    else
                    {
                        pdata = _ActiveItemsqlrepository.GetByWhere("ItemID In (" + modelitem + ") and IsVerift=1", new object[0]).ToList();
                    }

                    if (pdata.Count() > 0)
                    {
                        var arrpdata = pdata.Take(5).ToArray();
                        for (var pidx = 0; pidx < arrpdata.Count(); pidx++)
                        {
                            HomePageLayout.LinkUrl.Add(helper.Action("ActiveView", "Active") + "?id=" + arrpdata[pidx].ItemID + "&menutype=0&sitemenuid=" +
                                m.ID + "&mid=" + menuid);
                            var pgroup = _GroupActivesqlrepository.GetByWhere("ID=@1", new object[] { arrpdata[pidx].GroupID });
                            if (pgroup.Count() > 0) { HomePageLayout.ModelGroup.Add(pgroup.First().Group_Name); } else { HomePageLayout.ModelGroup.Add(""); }
                            if (arrpdata[pidx].RelateImageFileName.IsNullorEmpty() == false)
                            {
                                HomePageLayout.LinkImageSrc.Add(helper.Content("~/UploadImage/ActiveItem/" + arrpdata[pidx].RelateImageFileName));
                            }
                            else
                            {
                                var baseimg = helper.Content("~/ContentWebsite/image/logo_400x300.jpg");
                                HomePageLayout.LinkImageSrc.Add(baseimg);
                            }
                            HomePageLayout.PublicshDate.Add(arrpdata[pidx].PublicshDate.Value.ToString("yyyy/MM/dd"));
                            HomePageLayout.JustView.Add(arrpdata[pidx].Link_Mode == 1 ? false : true);
                            var content = arrpdata[pidx].Introduction == null ? "" : arrpdata[pidx].Introduction.TrimgHtmlTag();
                            if (content.Length >= 1000) { content = content.Substring(0, 1000); }
                            HomePageLayout.HtmlContent.Add(content);
                            var titlestr = arrpdata[pidx].Title == null ? "" : arrpdata[pidx].Title;
                            if (titlestr.Length > 30) { titlestr = titlestr.Substring(0, 30); }
                            HomePageLayout.Title.Add(titlestr);
                        }
                        if (m.OpenMode != 2)
                        {
                            if (m.OpenMode == 3)
                            {
                                HomePageLayout.MoreLink = m.OpenModeCust + "?itemid=" + usemodelitem;
                            }
                            else
                            {
                                HomePageLayout.MoreLink = helper.Action("Index", "Active") + "?itemid=" + usemodelitem + (menuid == -1 ? "" : "&mid=" + menuid);
                            }
                        }
                    }
                }
                else if (usemodel == 18)
                {
                    var nowdate = DateTime.Now.ToString("yyyy/MM/dd");
                    var modelitem = m.ModelItemList.IsNullorEmpty() ? "" : m.ModelItemList;
                    IList<VideoItem> pdata;
                    if (modelitem.IsNullorEmpty())
                    {
                        pdata = _videosqlrepository.GetByWhere("ModelID=@1 AND (EdDate IS NULL OR EdDate >=@2)  AND (StDate IS NULL OR StDate <=@2)" +
                       " AND Enabled=1  and IsVerift=1 Order By Sort", new object[] { usemodelitem, nowdate }).ToList();
                    }
                    else
                    {
                        pdata = _videosqlrepository.GetByWhere("ItemID In (" + modelitem + ") and IsVerift=1", new object[0]).ToList();
                    }

                    if (pdata.Count() > 0)
                    {
                        var arrpdata = pdata.Take(5).ToArray();
                        for (var pidx = 0; pidx < arrpdata.Count(); pidx++)
                        {
                            HomePageLayout.LinkUrl.Add(helper.Action("VideoView", "Videos") + "?id=" + arrpdata[pidx].ItemID + "&menutype=0&sitemenuid=" +
                                m.ID + "&mid=" + menuid);
                            var pgroup = _GroupActivesqlrepository.GetByWhere("ID=@1", new object[] { arrpdata[pidx].GroupID });
                            if (pgroup.Count() > 0) { HomePageLayout.ModelGroup.Add(pgroup.First().Group_Name); } else { HomePageLayout.ModelGroup.Add(""); }
                            if (arrpdata[pidx].RelateImageFileName.IsNullorEmpty() == false)
                            {
                                HomePageLayout.LinkImageSrc.Add(helper.Content("~/UploadImage/VideoItem/" + arrpdata[pidx].RelateImageFileName));
                            }
                            else
                            {
                                var baseimg = helper.Content("~/ContentWebsite/image/logo_400x300.jpg");
                                HomePageLayout.LinkImageSrc.Add(baseimg);
                            }
                            HomePageLayout.PublicshDate.Add(arrpdata[pidx].PublicshDate.Value.ToString("yyyy/MM/dd"));
                            HomePageLayout.JustView.Add(arrpdata[pidx].Link_Mode == 1 ? false : true);
                            var content = arrpdata[pidx].Introduction == null ? "" : arrpdata[pidx].Introduction.TrimgHtmlTag();
                            if (content.Length >= 1000) { content = content.Substring(0, 1000); }
                            HomePageLayout.HtmlContent.Add(content);
                            var titlestr = arrpdata[pidx].Title == null ? "" : arrpdata[pidx].Title;
                            if (titlestr.Length > 30) { titlestr = titlestr.Substring(0, 30); }
                            HomePageLayout.Title.Add(titlestr);
                        }
                        if (m.OpenMode != 2)
                        {
                            if (m.OpenMode == 3)
                            {
                                HomePageLayout.MoreLink = m.OpenModeCust + "?itemid=" + usemodelitem;
                            }
                            else
                            {
                                HomePageLayout.MoreLink = helper.Action("Index", "Videos") + "?itemid=" + usemodelitem + (menuid == -1 ? "" : "&mid=" + menuid);
                            }
                        }
                    }
                }
                else if (usemodel == 19)
                {
                    var nowdate = DateTime.Now.ToString("yyyy/MM/dd");
                    var modelitem = m.ModelItemList.IsNullorEmpty() ? "" : m.ModelItemList;
                    IList<PatentItem> pdata;
                    if (modelitem.IsNullorEmpty())
                    {
                        pdata = _patentsqlrepository.GetByWhere("ModelID=@1 AND (EdDate IS NULL OR EdDate >=@2)  AND (StDate IS NULL OR StDate <=@2)" +
                       " AND Enabled=1  and IsVerift=1 Order By Sort", new object[] { usemodelitem, nowdate }).ToList();
                    }
                    else
                    {
                        pdata = _patentsqlrepository.GetByWhere("ItemID In (" + modelitem + ") and IsVerift=1", new object[0]).ToList();
                    }

                    if (pdata.Count() > 0)
                    {
                        var arrpdata = pdata.Take(5).ToArray();
                        for (var pidx = 0; pidx < arrpdata.Count(); pidx++)
                        {
                            HomePageLayout.LinkUrl.Add(helper.Action("PatentView", "Patent") + "?id=" + arrpdata[pidx].ItemID + "&menutype=0&sitemenuid=" +
                                m.ID + "&mid=" + menuid);
                            var pgroup = _GroupActivesqlrepository.GetByWhere("ID=@1", new object[] { arrpdata[pidx].GroupID });
                            if (pgroup.Count() > 0) { HomePageLayout.ModelGroup.Add(pgroup.First().Group_Name); } else { HomePageLayout.ModelGroup.Add(""); }
                            HomePageLayout.PublicshDate.Add(arrpdata[pidx].PublicshDate.Value.ToString("yyyy/MM/dd"));
                            HomePageLayout.JustView.Add(arrpdata[pidx].Link_Mode == 1 ? false : true);
                            var content = arrpdata[pidx].HtmlContent == null ? "" : arrpdata[pidx].HtmlContent.TrimgHtmlTag();
                            if (content.Length >= 1000) { content = content.Substring(0, 1000); }
                            HomePageLayout.HtmlContent.Add(content);
                            var titlestr = arrpdata[pidx].Title == null ? "" : arrpdata[pidx].Title;
                            if (titlestr.Length > 30) { titlestr = titlestr.Substring(0, 30); }
                            HomePageLayout.Title.Add(titlestr);
                        }
                        if (m.OpenMode != 2)
                        {
                            if (m.OpenMode == 3)
                            {
                                HomePageLayout.MoreLink = m.OpenModeCust + "?itemid=" + usemodelitem;
                            }
                            else
                            {
                                HomePageLayout.MoreLink = helper.Action("Index", "Patent") + "?itemid=" + usemodelitem + (menuid == -1 ? "" : "&mid=" + menuid);
                            }
                        }
                    }
                }
                pagelayout.Add(HomePageLayout);
                idx += 1;
            }
            return pagelayout;
        }
        #endregion

        #region GetFrontLinkString
        public string GetFrontLinkString(string itemid, string mid,string itemname,string sitemenuid)
        {
            var sb = new StringBuilder();
            //if (openmenuid.IsNullorEmpty()) { return ""; }
            //  <a href="@Url.Action("Index","Home")"><i class="fa fa-home"></i>首頁</a><i class="fa fa-angle-right"></i>@ViewBag.modelname
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            //   <a><i class='fas fa-home'></i></a><i class='fa fa-angle-right'></i>關於我們<i class="fa fa-angle-right"></i><span class="active">@Html.Raw(Model.Title)</span>
            var homestr = "Home";
            if (_langdict.ContainsKey("回首頁")) { homestr = _langdict["回首頁"]; }
            sb.Append("<a title='"+ homestr+"' href='"+ helper.Action("Index","Home")+ "'><i class='fas fa-home' aria-hidden='true'></i>"+
                "<span class='sr-only'>"+ homestr +"</span></a>");
            if (mid.IsNullorEmpty() == false)
            {
                var menu = _menusqlrepository.GetByWhere("ID=@1", new object[] { mid });

                if (menu.Count() > 0)
                {  
                    //var thislevel =new List<Menu>();
                    //var levelfirstpath = "";
                    //var path = GetPath(menu.First().ModelID);
                    var tempmenu = menu.First();
                    if (menu.First().MenuLevel == 1)
                    {
                        if (tempmenu.MenuType == 2)
                        {
                            tempmenu.MenuName = tempmenu.DisplayName.IsNullorEmpty()? tempmenu.MenuName.TrimgHtmlTag(): tempmenu.DisplayName;
                        }
                        sb.Append("<i class='fa fa-angle-right' aria-hidden='true'></i><span class='active'>" + tempmenu.MenuName + "</span>");
                        //if (menu.First().LinkMode == 2)
                        //{ sb.Append("<li><a href='#' path='" + helper.Action("Index", path) + "' itemid='" + menu.First().ModelItemID + "' mid='" + menu.First().ID + "'> " + menu.First().MenuName + "</a></li>");}
                        //else if (menu.First().LinkMode == 3)
                        //{sb.Append("<li><a href='" + menu.First().MenuUrl + "'> " + menu.First().MenuName + "</a></li>"); }
                        //else if (menu.First().LinkMode == 4)
                        //{sb.Append("<li><a href='" + helper.Action("FileDownLoad", "Home", new { mid = menu.First().ID }) + "'> " + menu.First().MenuName + "</a></li>");}
                        //else{ sb.Append("<li><a href='#'> " + menu.First().MenuName + "</a></li>");}
                    }
                    else if (menu.First().MenuLevel == 2)
                    {
                        var menu2 = _menusqlrepository.GetByWhere("ID=@1", new object[] { menu.First().ParentID.Value });
                        sb.Append("<i class='fa fa-angle-right' aria-hidden='true'></i>" + menu2.First().MenuName);
                        sb.Append("<i class='fa fa-angle-right' aria-hidden='true'></i><span class='active'>" + menu.First().MenuName + "</span>");
                    }
                    else if (menu.First().MenuLevel == 3)
                    {
                        var menu2 = _menusqlrepository.GetByWhere("ID=@1", new object[] { menu.First().ParentID.Value });
                        var menu1 = _menusqlrepository.GetByWhere("ID=@1", new object[] { menu2.First().ParentID.Value });
                        sb.Append("<i class='fa fa-angle-right' aria-hidden='true'></i>" + menu1.First().MenuName);
                        sb.Append("<i class='fa fa-angle-right' aria-hidden='true'></i>" + menu2.First().MenuName);
                        sb.Append("<i class='fa fa-angle-right' aria-hidden='true'></i><span class='active'>" + menu.First().MenuName + "</span>");
                    }
                }
            }
            else {
                if (sitemenuid.IsNullorEmpty() || sitemenuid == "-1")
                {
                    sb.Append("<i class='fa fa-angle-right' aria-hidden='true'></i>" + itemname);
                }
                else {
                    var pagelayout = _pagelayoutsqlrepository.GetByWhere("ID=@1", new object[] { sitemenuid });
                    sb.Append("<i class='fa fa-angle-right' aria-hidden='true'></i>" + pagelayout.First().Title);
                }
            }
            return sb.ToString();
        }
        #endregion

        #region GetMapPageUrl
        public static string GetMapPageUrl(IDictionary<string, string> menuurl, Menu _menu, UrlHelper helper)
        {
            if (_menu.LinkMode.Value == 3)
            {
                if (_menu.LinkUrl.IsNullorEmpty() == false)
                {
                    if (_menu.LinkUrl.ToLower().IndexOf("http") == 0)
                    {
                        return _menu.LinkUrl;
                    }
                    else if (_menu.LinkUrl.ToLower().IndexOf("~") == 0)
                    {
                        return VirtualPathUtility.ToAbsolute(_menu.LinkUrl) + "?mid=" + _menu.ID;
                    }
                    else
                    {
                        //helper
                        var area = HttpContext.Current.Request.RequestContext.RouteData.DataTokens["area"];
                        var linkarea = "";
                        if (area == null) { linkarea = ""; } else { linkarea = area.ToString(); }
                        var hostUrl = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority);
                        if (linkarea!=""&&_menu.LinkUrl.IndexOf(linkarea) >= 0)
                        {
                            return hostUrl + "/" + linkarea + "/" + _menu.LinkUrl + "?mid=" + _menu.ID;
                        }
                        else
                        {
                            return hostUrl + "/" + _menu.LinkUrl + "?mid=" + _menu.ID;
                        }
                    }
                }
                else
                {
                    if (menuurl.ContainsKey(_menu.MenuName))
                    {
                        return VirtualPathUtility.ToAbsolute(menuurl[_menu.MenuName]) + "?mid=" + _menu.ID;
                    }
                    else
                    {
                        return "#";
                    }
                }
                //if (menuurl.ContainsKey(_menu.MenuName))
                //{
                //    return VirtualPathUtility.ToAbsolute(menuurl[_menu.MenuName]) + "?mid=" + _menu.ID;
                //}
                //else
                //{
                //    return "#";
                //}
            }
            else if (_menu.LinkMode.Value == 2)
            {
                var path = "Home";
                if (_menu.ModelID == 1) { path = "Page"; }
                if (_menu.ModelID == 2) { path = "Message"; }
                if (_menu.ModelID == 3) { path = "Active"; }
                if (_menu.ModelID == 4) { path = "Download"; }
                if (_menu.ModelID == 5) { path = "Map"; }
                if (_menu.ModelID == 6) { path = "#"; }
                if (_menu.ModelID == 7) { path = "Article"; }
                if (_menu.ModelID == 8) { path = "Knowledge"; }
                if (_menu.ModelID == 9) { path = "Forum"; }
                if (_menu.ModelID == 10) { path = "Industry"; }
                if (_menu.ModelID == 11) { path = "Form"; }
                if (_menu.ModelID == 12) { path = "Course"; }
                if (_menu.ModelID == 13) { path = "Product"; }
                if (_menu.ModelID == 14) { path = "Cart"; }
                if (_menu.ModelID == 15) { path = "BaseData"; }
                if (_menu.ModelID == 16) { path = "FAQ"; }
                if (_menu.ModelID == 17) { path = "Event"; }
                if (_menu.ModelID == 18) { path = "Videos"; }
                if (_menu.ModelID == 19) { path = "Patent"; }
                return helper.Action("Index", path) + "?itemid=" + _menu.ModelItemID + "&mid=" + _menu.ID;
            }
            else if (_menu.LinkMode.Value ==4)
            {
                return helper.Action("DownloadFile", "Home") + "?itemid=" + _menu.ModelItemID + "&mid=" + _menu.ID;
            }
            else
            {
                return "#";
            }

        }
        #endregion

        #region GetSEO
        //List
        public string[] GetSEOData(string type, string typeid, string langid,string title="",bool leve2to3=false)
        {
            title = title.TrimgHtmlTag();
            var SEO = new SEO();
            var seodata = _seosqlrepository.GetByWhere("TypeName=@1 and Lang_ID=@2", new object[] { "Main", langid });
            var model = new SEOViewModel();
            var finaltitle = "";
            var finaldesc = "";
            var orgseo = "";
            string[] finalkeyword = new string[0];
            if (seodata.Count() > 0)
            {
                finaldesc = seodata.First().Description.IsNullorEmpty()? "" : seodata.First().Description;
                finaltitle = seodata.First().Title.IsNullorEmpty() ? "" : seodata.First().Title;
                orgseo= seodata.First().Title.IsNullorEmpty() ? "" : seodata.First().Title;
                finalkeyword = new string[] {
                        seodata.First().Keywords1,seodata.First().Keywords2,seodata.First().Keywords3,seodata.First().Keywords4,seodata.First().Keywords5
                    ,seodata.First().Keywords6,seodata.First().Keywords7,seodata.First().Keywords8,seodata.First().Keywords9,seodata.First().Keywords10};
                finalkeyword = finalkeyword.Where(v => v != "").ToArray();
            }

            var pageseo = _seosqlrepository.GetByWhere("TypeName=@1 and Lang_ID=@2 and TypeID=@3", new object[] { type, langid, typeid });
            if (pageseo.Count() > 0)
            {
                model.ID = pageseo.First().ID;
                model.Description = pageseo.First().Description == null?"": pageseo.First().Description;
                model.WebsiteTitle = pageseo.First().Title == null ? "" : pageseo.First().Title;
                model.Keywords = new string[] {
                        pageseo.First().Keywords1,pageseo.First().Keywords2,pageseo.First().Keywords3,pageseo.First().Keywords4,pageseo.First().Keywords5
                    ,pageseo.First().Keywords6,pageseo.First().Keywords7,pageseo.First().Keywords8,pageseo.First().Keywords9,pageseo.First().Keywords10};
                model.Keywords = model.Keywords.Where(v => v != "").ToArray();
                if (model.Description == "" && model.WebsiteTitle == "" && model.Keywords.Length == 0)
                {
                    _seosqlrepository.DelDataUseWhere("ID=@1", new object[] { model.ID });
                }
            }
            else { model.Keywords = new string[0];}

            if (leve2to3 == false)
            {
                if (finaltitle == "" && model.WebsiteTitle.IsNullorEmpty()) { finaltitle = title; }
                else if (finaltitle != "" && model.WebsiteTitle.IsNullorEmpty())
                { finaltitle = (title.IsNullorEmpty() ? "" : title + "-") + finaltitle; finaltitle = finaltitle.TrimStart('-'); }
                else if (finaltitle != "" && model.WebsiteTitle.IsNullorEmpty() == false)
                { finaltitle = model.WebsiteTitle + "-" + finaltitle; }
                else { finaltitle = model.WebsiteTitle; }
                if (finaldesc == "" && model.Description.IsNullorEmpty()) { finaldesc = title; }
                else if (finaldesc != "" && model.Description.IsNullorEmpty())
                {
                    //finaldesc = (finaldesc.IsNullorEmpty() ? "" : title + "-") + finaldesc; finaldesc = finaldesc.TrimStart('-');
                }
                else if (model.Description.IsNullorEmpty() == false) { finaldesc = model.Description; }
            }
            else {

                if (finaltitle == "" && model.WebsiteTitle.IsNullorEmpty()) { finaltitle = title; }
                else if (finaltitle != "" && model.WebsiteTitle.IsNullorEmpty())
                { finaltitle = (title.IsNullorEmpty() ? "" : title + "-") + finaltitle; finaltitle = finaltitle.TrimStart('-'); }
                else if (finaltitle != "" && model.WebsiteTitle.IsNullorEmpty() == false)
                { finaltitle = model.WebsiteTitle +"-"+ title+ "-" + finaltitle; }
                else if (finaltitle == "" && model.WebsiteTitle.IsNullorEmpty() == false)
                { finaltitle = model.WebsiteTitle + "-" + title; }
                else { finaltitle = model.WebsiteTitle; }

                if (finaldesc == "" && model.Description.IsNullorEmpty()) { finaldesc = title; }
                else if (finaldesc != "" && model.Description.IsNullorEmpty()){  }
                else if (model.Description.IsNullorEmpty() == false) { finaldesc = model.Description; }
            }
            if (finaltitle.IsNullorEmpty())
            {
                if (langid == "1")
                {
                    finaltitle = "國家高速網路與計算中心";
                }
                else
                {
                    finaltitle = "National Center for High-performance Computing";
                }
            }
            if (orgseo == "") { orgseo = title; }
             if (model.Keywords.Count() > 0) { finalkeyword = model.Keywords; }
            var key = (finalkeyword == null || finalkeyword.Count() == 0) ? "" : "<meta name='Keywords' content='" + string.Join(",", finalkeyword) + "'>";
            return new string[] { finaldesc == null?"": finaldesc, key, finaltitle==null?"": finaltitle , orgseo };
        }
        #endregion

        #region GetSEOData
        public string[] GetSEOData(string type, string type2, string typeid, string typeid2, string langid,  string title = "")
        {
            title = title.TrimgHtmlTag();
            var SEO = new SEO();
            var seodata = _seosqlrepository.GetByWhere("TypeName=@1 and Lang_ID=@2", new object[] { "Main", langid });
            var model = new SEOViewModel();
            var finaltitle = "";
            var finaldesc = "";
            string[] finalkeyword = new string[0];
            if (seodata.Count() > 0)
            {
                finaldesc = seodata.First().Description.IsNullorEmpty() ? "" : seodata.First().Description;
                finaltitle = seodata.First().Title.IsNullorEmpty() ? "" : seodata.First().Title;
                finalkeyword = new string[] {
                        seodata.First().Keywords1,seodata.First().Keywords2,seodata.First().Keywords3,seodata.First().Keywords4,seodata.First().Keywords5
                    ,seodata.First().Keywords6,seodata.First().Keywords7,seodata.First().Keywords8,seodata.First().Keywords9,seodata.First().Keywords10};
                finalkeyword = finalkeyword.Where(v => v != "").ToArray();
            }

            var pageseo = _seosqlrepository.GetByWhere("TypeName=@1 and Lang_ID=@2 and TypeID=@3", new object[] { type, langid, typeid });
            var finaltitle2 = "";
            var finaldesc2 = "";
            string[] finalkeyword2 = new string[0];

            if (pageseo.Count() > 0)
            {
                finaldesc2 = pageseo.First().Description == null ? "" : pageseo.First().Description;
                finaltitle2 = pageseo.First().Title == null ? "" : pageseo.First().Title;
                finalkeyword2 = new string[] {
                        pageseo.First().Keywords1,pageseo.First().Keywords2,pageseo.First().Keywords3,pageseo.First().Keywords4,pageseo.First().Keywords5
                    ,pageseo.First().Keywords6,pageseo.First().Keywords7,pageseo.First().Keywords8,pageseo.First().Keywords9,pageseo.First().Keywords10};
                finalkeyword2 = finalkeyword2.Where(v => v != "").ToArray();
                //if (finaldesc2 == "" && finaltitle2 == "" && finalkeyword2.Length == 0)
                //{
                //    _seosqlrepository.DelDataUseWhere("ID=@1", new object[] { pageseo.First().ID });
                //}
            }

             pageseo = _seosqlrepository.GetByWhere("TypeName=@1  and TypeID=@2", new object[] { type2, typeid2 });
            var finaltitle3 = "";
            var finaldesc3 = "";
            string[] finalkeyword3 = new string[0];
            if (pageseo.Count() > 0)
            {
                finaldesc3 = pageseo.First().Description;
                finaltitle3 = pageseo.First().Title;
                finalkeyword3 = pageseo.Count() == 0 ? new string[10] : new string[] {
                        pageseo.First().Keywords1,pageseo.First().Keywords2,pageseo.First().Keywords3,pageseo.First().Keywords4,pageseo.First().Keywords5
                    ,pageseo.First().Keywords6,pageseo.First().Keywords7,pageseo.First().Keywords8,pageseo.First().Keywords9,pageseo.First().Keywords10};
                finalkeyword3 = finalkeyword3.Where(v => v != "").ToArray();
                //if (finaldesc3 == "" && finaltitle3 == "" && finalkeyword3.Length == 0)
                //{
                //    _seosqlrepository.DelDataUseWhere("ID=@1", new object[] { pageseo.First().ID });
                //}
            }

            if (finaltitle == "" && finaltitle2 == "" && finaltitle3 == "") { finaltitle = title; }
            else if (finaltitle != "" && finaltitle2 == "" && finaltitle3 == "") { finaltitle = title + "-"+ finaltitle; }
            else if (finaltitle != "" && finaltitle2 != "" && finaltitle3 == "") { finaltitle = title + "-" + finaltitle2+"-"+ finaltitle; }
            else if (finaltitle != "" && finaltitle2 != "" && finaltitle3 != "") { finaltitle = finaltitle3 + "-" + finaltitle2 + "-" + finaltitle; }
            else if (finaltitle == "" && finaltitle2 != "" && finaltitle3 != "") { finaltitle = finaltitle3 + "-" + finaltitle2; }
            else if (finaltitle == "" && finaltitle2 == "" && finaltitle3 != "") { finaltitle = finaltitle3 ; }
            else if (finaltitle != "" && finaltitle2 == "" && finaltitle3 != "") { finaltitle = finaltitle3 +"-" + finaltitle; }

            if (finaldesc == "" && finaldesc2 == "" && finaldesc3 == "") { finaldesc = title; }
            //else if (finaldesc != "" && finaldesc2 == "" && finaldesc3 == "") { finaldesc = title + "-" + finaldesc; }
            //else if (finaldesc != "" && finaldesc2 != "" && finaldesc3 == "") { finaldesc = title + "-" + finaldesc2; }
            //else if ( finaldesc3 != "") { finaldesc =  finaldesc3; }

            if (finalkeyword2.Count() > 0 && finalkeyword3.Count() ==0) { finalkeyword = finalkeyword2; }
            else if (finalkeyword3.Count() > 0 ) { finalkeyword = finalkeyword3; }

            var key = (finalkeyword == null || finalkeyword.Count() == 0) ? "" : "<meta name='Keywords' content='" + string.Join(",", finalkeyword) + "'>";
            return new string[] { finaldesc == null ? "" : finaldesc, key, finaltitle == null ? "" : finaltitle };
        }
        #endregion

        #region GetMenuShowModel
        public int GetMenuShowModel(string menuid )
        {
            if (menuid.IsNullorEmpty()) { return 1; }
            var menu = _menusqlrepository.GetByWhere("ID=@1", new object[] { menuid });
            if (menu.Count() > 0)
            {
                return menu.First().ShowMode.Value;
            }
            else { return 1; }
        }
        #endregion

        #region CheckPagrAuth
        public string CheckPagrAuth(bool EnterpriceStudentAuth, bool GeneralStudentAuth, bool VIPAuth, bool EMailAuth, string UserID, string connectionMemberStr, Dictionary<string, string> langdict)
        {
            return "";
        }

        #endregion

        #region CheckLangID
        public string CheckLangID(string mid)
        {
            var langid = HttpContext.Current.Session["LangID"].ToString();
            var requestlangid = HttpContext.Current.Request.Form["lang"];
            if (requestlangid.IsNullorEmpty() == false) {
                if (requestlangid == "1" || requestlangid == "2" || requestlangid == "3") {
                    HttpContext.Current.Session["LangID"] = requestlangid;
                    return requestlangid;
                }
            }
            if (mid.IsNullorEmpty() == false)
            {
                var menu = _menusqlrepository.GetByWhere("ID=@1", new object[] { mid });
                if (menu.Count() > 0)
                {
                    if (langid != menu.First().LangID.ToString())
                    {
                        HttpContext.Current.Session["LangID"] = menu.First().LangID.ToString();
                        return menu.First().LangID.ToString();
                    }
                }
            }
            return langid;
        }
        #endregion

        #region GetPath
        private string GetPath(int modelid)
        {
            var path = "Home";
            if (modelid == 1) { path = "Page"; }
            if (modelid == 2) { path = "Message"; }
            if (modelid == 3) { path = "Active"; }
            if (modelid == 4) { path = "Download"; }
            if (modelid == 5) { path = "Map"; }
            if (modelid == 7) { path = "Article"; }
            if (modelid == 8) { path = "Knowledge"; }
            if (modelid == 9) { path = "Forum"; }
            if (modelid == 10) { path = "Industry"; }
            if (modelid == 10) { path = "Form"; }
            if (modelid == 12) { path = "Course"; }
            if (modelid == 13) { path = "Product"; }
            if (modelid == 14) { path = "Cart"; }
            if (modelid == 15) { path = "BaseData"; }
            if (modelid == 16) { path = "FAQ"; }
            if (modelid == 17) { path = "Event"; }
            if (modelid == 18) { path = "Videos"; }
            if (modelid == 19) { path = "Patent"; }
            return path;
        }
        #endregion

        #region GetLangName
        public string GetLangName(string id)
        {
            var lang = _LangMainsqlrepository.GetByWhere("ID="+id,null);
            return lang.First().Lang_Name;
        }

        #endregion
 
    }
}
