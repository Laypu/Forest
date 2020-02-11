using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLModel.Models;
using System.Web;
using ViewModels;
using SQLModel;
using System.IO;
using Utilities;
using System.Net;
using System.Xml.Linq;
using ViewModels.DBModels;
using System.Reflection;
using System.Web.Mvc;

namespace Services.Manager
{
    public class SiteLayoutManager : ISiteLayoutManager
    {
        readonly SQLRepository<SiteLayout> _sqlrepository;
        readonly SQLRepository<PageLayout> _pagelayoutsqlrepository;
        readonly SQLRepository<LangKey> _langkeysqlrepository;
        readonly SQLRepository<LangInputText> _langinputsqlrepository;
        readonly SQLRepository<Img> _imgsqlrepository;
        readonly SQLRepository<PageLayoutOP1> _pageLayoutop1qlrepository;
        readonly SQLRepository<PageLayoutOP2> _pageLayoutop2qlrepository;
        readonly SQLRepository<PageLayoutOP3> _pageLayoutop3qlrepository;
        readonly SQLRepository<PageLayoutActivity> _pageLayoutActivityqlrepository;
        public SiteLayoutManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.SiteLayout;
            _imgsqlrepository = sqlinstance.Img;
            _pagelayoutsqlrepository = sqlinstance.PageLayout;
            _langkeysqlrepository = sqlinstance.LangKey;
            _langinputsqlrepository = sqlinstance.LangInputText;
            _pageLayoutop1qlrepository = sqlinstance.PageLayoutOP1;
            _pageLayoutop2qlrepository = sqlinstance.PageLayoutOP2;
            _pageLayoutop3qlrepository = sqlinstance.PageLayoutOP3;
            _pageLayoutActivityqlrepository = sqlinstance.PageLayoutActivity;
        }
        #region PagingMain
        public Paging<PageLayout> PagingMain(SearchModelBase model)
        {
            var Paging = new Paging<PageLayout>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("Stype=@1");
            whereobj.Add(model.Key);
            wherestr.Add("LangID=@2");
            whereobj.Add(model.LangId);
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("Name like @3");
                    whereobj.Add("%" + model.Search + "%");
                }
            }
            var str = string.Join(" and ", wherestr);
            Paging.rows = _pagelayoutsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            Paging.total = _pagelayoutsqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region GetSiteLayout
        public SiteLayoutModel GetSiteLayout(string stype, string langid)
        {
            var data = _sqlrepository.GetByWhere("SType=@1 and LangID=@2", new object[] { stype, langid });
            if (data.Count() > 0)
            {
                var model = new SiteLayoutModel()
                {
                    FirstPageImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().FirstPageImgNameOri),
                    FirstPageImgNameThumb = data.First().FirstPageImgNameThumb,
                    FirstPageImgNameOri = data.First().FirstPageImgNameOri,
                    FirstPageImgShowName = data.First().FirstPageImgShowName,
                    HtmlContent = data.First().HtmlContent,
                    ID = data.First().ID,
                    InsidePageImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().InsidePageImgNameOri),
                    InsidePageImgNameOri = data.First().InsidePageImgNameOri,
                    InsidePageImgNameThumb = data.First().InsidePageImgNameThumb,
                    InsidePageImgShowName = data.First().InsidePageImgShowName,
                    LogoImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().LogoImgNameOri),
                    LogoImageUrlThumb = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().LogoImgNameThumb),

                    LogoImgNameOri = data.First().LogoImgNameOri,
                    LogoImgNameThumb = data.First().LogoImgNameThumb,
                    LogoImgShowName = data.First().LogoImgShowName,

                    FowardImgNameOri = data.First().FowardImgNameOri,
                    FowardImgNameThumb = data.First().FowardImgNameThumb,
                    FowardImgShowName = data.First().FowardImgShowName,
                    FowardImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().FowardImgNameOri),
                    FowardHtmlContent = data.First().FowardHtmlContent,

                    PrintImgNameOri = data.First().PrintImgNameOri,
                    PrintImgNameThumb = data.First().PrintImgNameThumb,
                    PrintImgShowName = data.First().PrintImgShowName,
                    PrintImageUrl = data.First().PrintImgNameOri.IsNullorEmpty() ? "" : VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().PrintImgNameOri),
                    PrintHtmlContent = data.First().PrintHtmlContent,
                    Page404HtmlContent = data.First().Page404HtmlContent,
                    Page404Title = data.First().Page404Title,
                    SType = stype,
                    PublishContent = data.First().PublishContent,
                    InnerLogoImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().InnerLogoImgNameOri),
                    InnerLogoImageUrlThumb = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().InnerLogoImgNameThumb),
                    InnerLogoImgNameOri = data.First().InnerLogoImgNameOri,
                    InnerLogoImgNameThumb = data.First().InnerLogoImgNameThumb,
                    InnerLogoImgShowName = data.First().InnerLogoImgShowName,
                };
                return model;
            }
            else
            {
                return new SiteLayoutModel();
            }
        }
        #endregion

        #region EditSiteLayout
        public string EditSiteLayout(SiteLayoutModel model)
        {

            var SiteLayout = new SiteLayout()
            {
                //FirstPageImgHeight = model.FirstPageImgHeight,
                ID = model.ID,
                LangID = int.Parse(model.LangID),
                FirstPageImgNameOri = model.FirstPageImgNameOri == null ? "" : model.FirstPageImgNameOri,
                FirstPageImgNameThumb = model.FirstPageImgNameThumb,
                FirstPageImgShowName = model.FirstPageImgShowName,
                HtmlContent = model.HtmlContent == null ? "" : model.HtmlContent,
                //InsidePageImgHeight = model.InsidePageImgHeight,
                InsidePageImgNameThumb = model.InsidePageImgNameThumb,
                InsidePageImgShowName = model.InsidePageImgShowName,
                InsidePageImgNameOri = model.InsidePageImgNameOri == null ? "" : model.InsidePageImgNameOri,
                //LogoImgHeight = model.LogoImgHeight,
                LogoImgNameOri = model.LogoImgNameOri == null ? "" : model.LogoImgNameOri,
                LogoImgNameThumb = model.LogoImgNameThumb,
                LogoImgShowName = model.LogoImgShowName,
                InnerLogoImgNameOri = model.InnerLogoImgNameOri == null ? "" : model.InnerLogoImgNameOri,
                InnerLogoImgNameThumb = model.InnerLogoImgNameThumb,
                InnerLogoImgShowName = model.InnerLogoImgShowName,
                PublishContent = model.PublishContent,
                SType = model.SType
            };
            var olddata = _sqlrepository.GetByWhere("ID=@1 and LangID=@2", new object[] { model.ID, model.LangID });
            var r = 0;
            if (olddata.Count() > 0)
            {

                r = _sqlrepository.Update(SiteLayout);
                if (r > 0)
                {
                    var oldfilename = olddata.First().LogoImgNameThumb;
                    var oldfileoriname = olddata.First().LogoImgNameOri;
                    var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                        "\\UploadImage\\SiteLayout\\" + oldfilename;
                    var oldoriroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                      "\\UploadImage\\SiteLayout\\" + oldfileoriname;
                    if (model.LogoImgNameOri.IsNullorEmpty() || model.LogoImgNameOri != oldfileoriname)
                    {
                        //刪除舊檔案
                        if (System.IO.File.Exists(oldroot))
                        {
                            System.IO.File.Delete(oldroot);
                        }
                        if (System.IO.File.Exists(oldoriroot))
                        {
                            System.IO.File.Delete(oldoriroot);
                        }
                    }

                    oldfilename = olddata.First().InnerLogoImgNameThumb;
                    oldfileoriname = olddata.First().InnerLogoImgNameOri;
                    oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                       "\\UploadImage\\SiteLayout\\" + oldfilename;
                    oldoriroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                     "\\UploadImage\\SiteLayout\\" + oldfileoriname;
                    if (model.InnerLogoImgNameOri.IsNullorEmpty() || model.InnerLogoImgNameOri != oldfileoriname)
                    {
                        //刪除舊檔案
                        if (System.IO.File.Exists(oldroot))
                        {
                            System.IO.File.Delete(oldroot);
                        }
                        if (System.IO.File.Exists(oldoriroot))
                        {
                            System.IO.File.Delete(oldoriroot);
                        }
                    }
                }
            }
            else
            {
                r = _sqlrepository.Create(SiteLayout);
            }
            if (r > 0)
            {
                return "設定成功";
            }
            else
            {
                return "設定失敗";
            }
        }
        #endregion

        #region GetModel
        public PageLayoutModel GetModel(string title, string stype, string langid)
        {
            var model = new PageLayoutModel();
            model.SType = stype;
            var data = _pagelayoutsqlrepository.GetByWhere("Title=@1 and LangID=@2", new object[] { title, langid });
            if (data.Count() > 0)
            {
                model = new PageLayoutModel()
                {
                    BlockWidth = data.First().BlockWidth.ToString(),
                    Title = data.First().Title,
                    ID = data.First().ID,
                    ImageDesc = data.First().ImageDesc,
                    ImgNameOri = data.First().ImgNameOri,
                    ImgShowName = data.First().ImgShowName,
                    LangID = data.First().LangID,
                    LinkMode = data.First().LinkMode,
                    MenuItem = data.First().MenuItem,
                    MenuLevel1 = data.First().MenuLevel1,
                    MenuLevel2 = data.First().MenuLevel2,
                    MenuLevel3 = data.First().MenuLevel3,
                    ModelID = data.First().ModelID,
                    ModelItemID = data.First().ModelItemID,
                    OpenMode = data.First().OpenMode,
                    OpenModeCust = data.First().OpenModeCust,
                    SType = stype,
                    ModelItemList = data.First().ModelItemList
                };
            }
            model.ImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + model.ImgNameOri);
            return model;
        }
        #endregion

        #region Create
        public string Create(PageLayoutModel model, string langid, string account, string username)
        {

            //1.create message
            var datetime = DateTime.Now;
            model.SType = "P";
            //先將所有index+1
            var menucnt = _pagelayoutsqlrepository.GetByWhere("Stype=@1 and LangID=@2", new object[] { model.SType, langid }).Max(v => v.Sort);
            if (menucnt < 0 || menucnt == null) { menucnt = 0; }
            var sort = menucnt + 1;
            var maxid = _pagelayoutsqlrepository.GetDataCaculate("Max(ID)") + 1;
            var PageLayout = new PageLayout()
            {
                ID = maxid,
                ImageDesc = model.ImageDesc,
                ImageUrl = model.ImageUrl,
                ImgNameOri = model.ImgNameOri,
                ImgShowName = model.ImgShowName,
                LangID = model.LangID,
                LinkMode = 1,
                MenuItem = model.MenuItem,
                MenuLevel1 = model.MenuLevel1,
                MenuLevel2 = model.MenuLevel2,
                MenuLevel3 = model.MenuLevel3,
                ModelID = model.ModelID,
                ModelItemID = model.ModelItemID,
                OpenMode = model.OpenMode,
                OpenModeCust = model.OpenModeCust,
                Sort = sort,
                Status = true,
                Title = model.Title,
                UpdateDatetime = datetime,
                UpdateUser = account,
                SType = model.SType,
                ModelItemList = model.ModelItemList == null ? "" : model.ModelItemList
            };
            if (model.BlockWidth.IsNullorEmpty() == false)
            {
                PageLayout.BlockWidth = int.Parse(model.BlockWidth);
            }
            var r = _pagelayoutsqlrepository.Create(PageLayout);
            if (r > 0)
            {
                return "新增成功";
            }
            else
            {
                return "新增失敗";
            }

        }
        #endregion

        #region Update
        public string Update(PageLayoutModel model, string account, string username)
        {
            var olddata = _pagelayoutsqlrepository.GetByWhere("ID=@1", new object[] { model.ID });
            var oldfileoriname = olddata.First().ImgNameOri;
            var oldoriroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
              "\\UploadImage\\SiteLayout\\" + oldfileoriname;
            if (model.ImgNameOri.IsNullorEmpty())
            {
                model.ImgNameOri = "";
                model.ImgShowName = "";
            }
            //1.create message
            var datetime = DateTime.Now;
            //先將所有index+1
            var tr = _pagelayoutsqlrepository.Update("MenuLevel1=@1,MenuLevel2=@2,MenuLevel3=@3,ModelID=@4,ModelItemID=@5",
            "ID=@6", new object[] { DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, model.ID });

            var PageLayout = new PageLayout()
            {
                ID = model.ID,
                ImageDesc = model.ImageDesc,
                ImageUrl = model.ImageUrl,
                ImgNameOri = model.ImgNameOri,
                ImgShowName = model.ImgShowName,
                LangID = model.LangID,
                LinkMode = 1,
                MenuItem = model.MenuItem,
                MenuLevel1 = model.MenuLevel1,
                MenuLevel2 = model.MenuLevel2,
                MenuLevel3 = model.MenuLevel3,
                ModelID = model.ModelID,
                ModelItemID = model.ModelItemID,
                OpenMode = model.OpenMode,
                OpenModeCust = model.OpenModeCust == null ? "" : model.OpenModeCust,
                Title = model.Title = model.Title == null ? "" : model.Title,
                UpdateDatetime = datetime,
                UpdateUser = account,
                ModelItemList = model.ModelItemList == null ? "" : model.ModelItemList
            };

            var r = _pagelayoutsqlrepository.Update(PageLayout);
            if (r > 0)
            {
                if (model.ImgNameOri.IsNullorEmpty() || olddata.First().ImgNameOri != model.ImgNameOri)
                {
                    //刪除舊檔案
                    if (System.IO.File.Exists(oldoriroot))
                    {
                        System.IO.File.Delete(oldoriroot);
                    }
                }
                return "修改成功";
            }
            else
            {
                return "修改失敗";
            }

        }

        #endregion

        #region UpdateSeq
        public string UpdateSeq(int id, int seq, string langid, string type, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _pagelayoutsqlrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";
                var stype = oldmodel.First().SType;
                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<PageLayout> mtseqdata = null;
                    mtseqdata = _pagelayoutsqlrepository.GetByWhere("Sort>@1 and Stype=@2 and LangID=@3", new object[] { seq, stype, langid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _pagelayoutsqlrepository.GetCountUseWhere("Stype=@1 and LangID=@2", new object[] { stype, langid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<PageLayout> ltseqdata = null;
                    ltseqdata = _pagelayoutsqlrepository.GetByWhere("Sort<=@1 and Stype=@2 and LangID=@3", new object[] { qseq, stype, langid }).OrderBy(v => v.Sort).ToArray();
                    //更新seq+1
                    var sidx = 0;
                    for (var idx = 1; idx <= ltseqdata.Count(); idx++)
                    {
                        if (idx == seq && seq < oldmodel.First().Sort)
                        {
                            sidx += 1;
                        }
                        var tempmodel = ltseqdata[idx - 1];
                        if (tempmodel.ID == id)
                        {
                            continue;
                        }
                        else
                        {
                            sidx += 1;
                        }
                        tempmodel.Sort = sidx;
                        _pagelayoutsqlrepository.Update("Sort=@1", "ID=@2", new object[] { tempmodel.Sort, tempmodel.ID });
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _pagelayoutsqlrepository.Update("Sort=@1", "ID=@2", new object[] { oldmodel.First().Sort, oldmodel.First().ID });
                if (r > 0)
                {
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("更新訊息管理排序失敗:" + " error:" + ex.Message);
                return "更新訊息管理排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region Delete
        public string Delete(string[] idlist, string delaccount, string langid, string account, string username)
        {
            try
            {
                var r = 0;
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\SiteLayout\\";
                var stype = "";
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var oldddata = _pagelayoutsqlrepository.GetByWhere("ID=@1", new object[] { idlist[idx] });
                    stype = oldddata.First().SType;
                    var entity = new PageLayout();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _pagelayoutsqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除SiteLayout失敗:ID=" + idlist[idx]);
                    }
                    else
                    {
                        if (System.IO.File.Exists(oldroot + "\\" + oldddata.First().ImgNameOri))
                        {
                            System.IO.File.Delete(oldroot + "\\" + oldddata.First().ImgNameOri);
                        }
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    rstr = "刪除成功";
                }
                else
                {
                    NLogManagement.SystemLogInfo("刪除SiteLayout失敗:" + delaccount);
                    rstr = "刪除失敗";
                }
                var alldata = _pagelayoutsqlrepository.GetByWhere("SType=@1 and LangID=@2", new object[] { stype, langid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    tempmodel.Sort = idx;
                    _pagelayoutsqlrepository.Update("Sort=@1", "ID=@2", new object[] { idx, tempmodel.ID });
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除訊息管理單元失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region EditFowardSiteLayout
        public string EditFowardSiteLayout(SiteLayoutModel model)
        {

            var r = 0;
            if (model.ID != -1)
            {
                var olddata = _sqlrepository.GetByWhere("ID=@1", new object[] { model.ID });
                var updatedata = olddata.First();
                var oldfilename = updatedata.FowardImgNameThumb;
                var oldfileoriname = updatedata.FowardImgNameOri;
                updatedata.FowardHtmlContent = model.FowardHtmlContent == null ? "" : model.FowardHtmlContent;
                if (string.IsNullOrEmpty(model.FowardImgNameOri))
                {
                    updatedata.FowardImgNameThumb = "";
                    updatedata.FowardImgShowName = "";
                    updatedata.FowardImgNameOri = "";
                }
                else
                {
                    if (updatedata.FowardImgNameOri != model.FowardImgNameOri)
                    {
                        updatedata.FowardImgNameOri = model.FowardImgNameOri;
                        updatedata.FowardImgNameThumb = model.FowardImgNameThumb;
                        updatedata.FowardImgShowName = model.FowardImgShowName;
                    }
                }
                r = _sqlrepository.Update(updatedata);
                if (r > 0)
                {

                    var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                        "\\UploadImage\\SiteLayout\\" + oldfilename;
                    var oldoriroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                      "\\UploadImage\\SiteLayout\\" + oldfileoriname;
                    if (model.FowardImgNameOri.IsNullorEmpty() || model.FowardImgNameOri != oldfileoriname)
                    {
                        //刪除舊檔案
                        if (System.IO.File.Exists(oldroot))
                        {
                            System.IO.File.Delete(oldroot);
                        }
                        if (System.IO.File.Exists(oldoriroot))
                        {
                            System.IO.File.Delete(oldoriroot);
                        }
                    }
                }
            }
            else
            {
                var SiteLayout = new SiteLayout()
                {
                    ID = model.ID,
                    FowardHtmlContent = model.FowardHtmlContent,
                    FowardImgNameOri = model.FowardImgNameOri,
                    FowardImgNameThumb = model.FowardImgNameThumb,
                    FowardImgShowName = model.FowardImgShowName
                };
                r = _sqlrepository.Create(SiteLayout);
            }
            if (r > 0)
            {
                return "設定成功";
            }
            else
            {
                return "設定失敗";
            }
        }
        #endregion

        #region EditPrintSiteLayout
        public string EditPrintSiteLayout(SiteLayoutModel model)
        {
            var r = 0;
            if (model.ID != -1)
            {
                var olddata = _sqlrepository.GetByWhere("ID=@1", new object[] { model.ID });
                var updatedata = olddata.First();
                var oldfilename = updatedata.PrintImgNameThumb;
                var oldfileoriname = updatedata.PrintImgNameOri;
                updatedata.PrintHtmlContent = model.PrintHtmlContent == null ? "" : model.PrintHtmlContent;
                if (string.IsNullOrEmpty(model.PrintImgNameOri))
                {
                    updatedata.PrintImgNameThumb = "";
                    updatedata.PrintImgShowName = "";
                    updatedata.PrintImgNameOri = "";
                }
                else
                {
                    if (updatedata.PrintImgNameOri != model.PrintImgNameOri)
                    {
                        updatedata.PrintImgNameOri = model.PrintImgNameOri;
                        updatedata.PrintImgNameThumb = model.PrintImgNameThumb;
                        updatedata.PrintImgShowName = model.PrintImgShowName;
                    }
                }
                r = _sqlrepository.Update(updatedata);
                if (r > 0)
                {

                    var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                        "\\UploadImage\\SiteLayout\\" + oldfilename;
                    var oldoriroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                      "\\UploadImage\\SiteLayout\\" + oldfileoriname;
                    if (model.PrintImgNameOri.IsNullorEmpty() || model.PrintImgNameOri != oldfileoriname)
                    {
                        //刪除舊檔案
                        if (System.IO.File.Exists(oldroot))
                        {
                            System.IO.File.Delete(oldroot);
                        }
                        if (System.IO.File.Exists(oldoriroot))
                        {
                            System.IO.File.Delete(oldoriroot);
                        }
                    }
                }
            }
            else
            {
                var SiteLayout = new SiteLayout()
                {
                    ID = model.ID,
                    PrintHtmlContent = model.PrintHtmlContent,
                    PrintImgNameOri = model.PrintImgNameOri,
                    PrintImgNameThumb = model.PrintImgNameThumb,
                    PrintImgShowName = model.PrintImgShowName
                };
                r = _sqlrepository.Create(SiteLayout);
            }
            if (r > 0)
            {
                return "儲存成功";
            }
            else
            {
                return "儲存失敗";
            }
        }
        #endregion

        #region EditPage404SiteLayout
        public string EditPage404SiteLayout(SiteLayoutModel model)
        {
            var r = 0;
            if (model.ID != -1)
            {
                var olddata = _sqlrepository.GetByWhere("ID=@1", new object[] { model.ID });
                var updatedata = olddata.First();
                updatedata.Page404Title = model.Page404Title == null ? "" : model.Page404Title;
                updatedata.Page404HtmlContent = model.Page404HtmlContent == null ? "" : model.Page404HtmlContent;
                r = _sqlrepository.Update(updatedata);

            }
            else
            {
                var SiteLayout = new SiteLayout()
                {
                    ID = model.ID,
                    Page404HtmlContent = model.Page404HtmlContent,
                    Page404Title = model.Page404Title
                };
                r = _sqlrepository.Create(SiteLayout);
            }
            if (r > 0)
            {
                return "儲存成功";
            }
            else
            {
                return "儲存失敗";
            }
        }
        #endregion

        #region GetSiteLangText
        public SiteLangTextModel GetSiteLangText(string langid)
        {
            var input = _langinputsqlrepository.GetByWhere("LangID=@1", new object[] { langid }).ToDictionary(v => v.LangTextID, v => v.Text);
            var data = _langkeysqlrepository.GetByWhere("GroupName=@1 and Used=1", new object[] { "BasicSetting" });
            if (data.Count() > 0)
            {
                var model = new SiteLangTextModel();
                model.GroupKey1 = data.Where(v => v.SubGroup == 1).OrderBy(v => v.Sort).ToDictionary(k => k.ID.Value.ToString(), i => i.Item);
                foreach (var key in data.Where(v => v.SubGroup == 1).OrderBy(v => v.Sort).Select(v => v.ID).ToArray())
                {
                    if (input.ContainsKey(key.Value))
                    {
                        model.Group1.Add(key.ToString(), input[key.Value]);
                    }
                    else
                    {
                        model.Group1.Add(key.ToString(), "");
                    }
                }

                model.GroupKey2 = data.Where(v => v.SubGroup == 2).OrderBy(v => v.Sort).ToDictionary(k => k.ID.Value.ToString(), i => i.Item);
                foreach (var key in data.Where(v => v.SubGroup == 2).OrderBy(v => v.Sort).Select(v => v.ID).ToArray())
                {
                    if (input.ContainsKey(key.Value))
                    {
                        model.Group2.Add(key.ToString(), input[key.Value]);
                    }
                    else
                    {
                        model.Group2.Add(key.ToString(), "");
                    }
                }

                model.GroupKey3 = data.Where(v => v.SubGroup == 3).OrderBy(v => v.Sort).ToDictionary(k => k.ID.Value.ToString(), i => i.Item);
                foreach (var key in data.Where(v => v.SubGroup == 3).OrderBy(v => v.Sort).Select(v => v.ID).ToArray())
                {
                    if (input.ContainsKey(key.Value))
                    {
                        model.Group3.Add(key.ToString(), input[key.Value]);
                    }
                    else
                    {
                        model.Group3.Add(key.ToString(), "");
                    }
                }

                model.GroupKey4 = data.Where(v => v.SubGroup == 4).OrderBy(v => v.Sort).ToDictionary(k => k.ID.Value.ToString(), i => i.Item);
                foreach (var key in data.Where(v => v.SubGroup == 4).OrderBy(v => v.Sort).Select(v => v.ID).ToArray())
                {
                    if (input.ContainsKey(key.Value))
                    {
                        model.Group4.Add(key.ToString(), input[key.Value]);
                    }
                    else
                    {
                        model.Group4.Add(key.ToString(), "");
                    }
                }

                model.GroupKey5 = data.Where(v => v.SubGroup == 5).OrderBy(v => v.Sort).ToDictionary(k => k.ID.Value.ToString(), i => i.Item);
                foreach (var key in data.Where(v => v.SubGroup == 5).OrderBy(v => v.Sort).Select(v => v.ID).ToArray())
                {
                    if (input.ContainsKey(key.Value))
                    {
                        model.Group5.Add(key.ToString(), input[key.Value]);
                    }
                    else
                    {
                        model.Group5.Add(key.ToString(), "");
                    }
                }
                model.GroupKey6 = data.Where(v => v.SubGroup == 6).OrderBy(v => v.Sort).ToDictionary(k => k.ID.Value.ToString(), i => i.Item);
                foreach (var key in data.Where(v => v.SubGroup == 6).OrderBy(v => v.Sort).Select(v => v.ID).ToArray())
                {
                    if (input.ContainsKey(key.Value))
                    {
                        model.Group6.Add(key.ToString(), input[key.Value]);
                    }
                    else
                    {
                        model.Group6.Add(key.ToString(), "");
                    }
                }
                return model;
            }
            else
            {
                return new SiteLangTextModel();
            }

        }
        #endregion

        #region SaveSiteLangText
        public string SaveSiteLangText(SiteLangTextModel model, string langid)
        {
            var input = _langinputsqlrepository.GetByWhere("LangID=@1", new object[] { langid }).ToDictionary(v => v.LangTextID.ToString(), v => v.Text);
            var r = 0;
            foreach (var g in model.Group1.Keys)
            {
                if (input.ContainsKey(g.ToString()))
                {
                    r = _langinputsqlrepository.Update("Text=@1", "LangID=@2 and LangTextID=@3", new object[] { model.Group1[g], langid, g });
                }
                else
                {
                    r = _langinputsqlrepository.Create(new LangInputText()
                    {
                        LangID = int.Parse(langid),
                        LangTextID = int.Parse(g),
                        Text = model.Group1[g]
                    });
                }
            }

            foreach (var g in model.Group2.Keys)
            {
                if (input.ContainsKey(g.ToString()))
                {
                    r = _langinputsqlrepository.Update("Text=@1", "LangID=@2 and LangTextID=@3", new object[] { model.Group2[g], langid, g });
                }
                else
                {
                    r = _langinputsqlrepository.Create(new LangInputText()
                    {
                        LangID = int.Parse(langid),
                        LangTextID = int.Parse(g),
                        Text = model.Group2[g]
                    });
                }
            }

            foreach (var g in model.Group3.Keys)
            {
                if (input.ContainsKey(g.ToString()))
                {
                    r = _langinputsqlrepository.Update("Text=@1", "LangID=@2 and LangTextID=@3", new object[] { model.Group3[g], langid, g });
                }
                else
                {
                    r = _langinputsqlrepository.Create(new LangInputText()
                    {
                        LangID = int.Parse(langid),
                        LangTextID = int.Parse(g),
                        Text = model.Group3[g]
                    });
                }
            }

            foreach (var g in model.Group4.Keys)
            {
                if (input.ContainsKey(g.ToString()))
                {
                    r = _langinputsqlrepository.Update("Text=@1", "LangID=@2 and LangTextID=@3", new object[] { model.Group4[g], langid, g });
                }
                else
                {
                    r = _langinputsqlrepository.Create(new LangInputText()
                    {
                        LangID = int.Parse(langid),
                        LangTextID = int.Parse(g),
                        Text = model.Group4[g]
                    });
                }
            }
            foreach (var g in model.Group5.Keys)
            {
                if (input.ContainsKey(g.ToString()))
                {
                    r = _langinputsqlrepository.Update("Text=@1", "LangID=@2 and LangTextID=@3", new object[] { model.Group5[g], langid, g });
                }
                else
                {
                    r = _langinputsqlrepository.Create(new LangInputText()
                    {
                        LangID = int.Parse(langid),
                        LangTextID = int.Parse(g),
                        Text = model.Group5[g]
                    });
                }
            }
            foreach (var g in model.Group6.Keys)
            {
                if (input.ContainsKey(g.ToString()))
                {
                    r = _langinputsqlrepository.Update("Text=@1", "LangID=@2 and LangTextID=@3", new object[] { model.Group6[g], langid, g });
                }
                else
                {
                    r = _langinputsqlrepository.Create(new LangInputText()
                    {
                        LangID = int.Parse(langid),
                        LangTextID = int.Parse(g),
                        Text = model.Group6[g]
                    });
                }
            }
            if (r > 0) { return ""; }
            else
            {
                return "設定失敗";
            }
        }

        #endregion

        #region GetTrainingSiteData
        public string GetTrainingSiteData(string opennewstr)
        {
            string urlAddress = "https://edu.nchc.org.tw/course_latest.aspx";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    }
                    string data = readStream.ReadToEnd();
                    var xmldata = XDocument.Parse(data);
                    response.Close();
                    readStream.Close();
                    var cdata = xmldata.Descendants("course").OrderBy(v => v.Descendants("start_date").First().Value);
                    var idx = 1;
                    var sb = new System.Text.StringBuilder();

                    foreach (var node in cdata)
                    {
                        if (idx > 3) { continue; }
                        var sdate = node.Descendants("start_date");
                        var place = node.Descendants("place");
                        var title = node.Descendants("course_name");
                        var url = node.Descendants("url");
                        if (DateTime.Parse(sdate.First().Value) < DateTime.Now)
                        {
                            continue;
                        }
                        sb.Append("<li data-sr='enter bottom over 1.5s'>");
                        sb.Append("<div class='date_map'>" + sdate.First().Value + "<span class='map'>" + place.First().Value.Split('-').First().Trim() + "</span></div>");
                        sb.Append(" <div class='title'><a href='" + url.First().Value + "' target='_blank' title='" + title.First().Value + "("+ opennewstr+")'>" + title.First().Value + "</a></div></li>");
                        idx++;
                    }
                    return sb.ToString();

                }
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("讀取教育訓練網站失敗:" + ex.Message);
            }

            return "";
        }

        #endregion

        #region SavePageLayoutOP1Edit
        public string SavePageLayoutOP1Edit(PageLayoutOP1Model model)
        {
            var basepath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\PageLayout\\";
            if (System.IO.Directory.Exists(basepath) == false) { System.IO.Directory.CreateDirectory(basepath); }
            var olddata = _pageLayoutop1qlrepository.GetByWhere("LangID=@1", new object[] { model.LangID });
            var updateobj = new PageLayoutOP1()
            {
                Introduction = model.Introduction,
                IsShow = model.IsShow,
                LangID = model.LangID,
                RightLink = model.RightLink,
                RightTitle = model.RightTitle,
                LeftTitle = model.LeftTitle,

                LeftItem1Desc = model.LeftItem.Length > 0 ? model.LeftItem[0].Desc == null ? "" : model.LeftItem[0].Desc : "",
                LeftItem2Desc = model.LeftItem.Length > 1 ? model.LeftItem[1].Desc == null ? "" : model.LeftItem[1].Desc : "",
                LeftItem3Desc = model.LeftItem.Length > 2 ? model.LeftItem[2].Desc == null ? "" : model.LeftItem[2].Desc : "",
                LeftItem4Desc = model.LeftItem.Length > 3 ? model.LeftItem[3].Desc == null ? "" : model.LeftItem[3].Desc : "",
                RightItem1Desc = model.RightItem.Length > 0 ? model.RightItem[0].Desc == null ? "" : model.RightItem[0].Desc : "",
                RightItem2Desc = model.RightItem.Length > 1 ? model.RightItem[1].Desc == null ? "" : model.RightItem[1].Desc : "",
                RightItem3Desc = model.RightItem.Length > 2 ? model.RightItem[2].Desc == null ? "" : model.RightItem[2].Desc : "",
                LeftItem1Link = model.LeftItem.Length > 0 ? model.LeftItem[0].Link == null ? "" : model.LeftItem[0].Link : "",
                LeftItem2Link = model.LeftItem.Length > 1 ? model.LeftItem[1].Link == null ? "" : model.LeftItem[1].Link : "",
                LeftItem3Link = model.LeftItem.Length > 2 ? model.LeftItem[2].Link == null ? "" : model.LeftItem[2].Link : "",
                LeftItem4Link = model.LeftItem.Length > 3 ? model.LeftItem[3].Link == null ? "" : model.LeftItem[3].Link : "",
                RightItem1Link = model.RightItem.Length > 0 ? model.RightItem[0].Link == null ? "" : model.RightItem[0].Link : "",
                RightItem2Link = model.RightItem.Length > 1 ? model.RightItem[1].Link == null ? "" : model.RightItem[1].Link : "",
                RightItem3Link = model.RightItem.Length > 2 ? model.RightItem[2].Link == null ? "" : model.RightItem[2].Link : "",
                LeftItem1Title = model.LeftItem.Length > 0 ? model.LeftItem[0].Title == null ? "" : model.LeftItem[0].Title : "",
                LeftItem2Title = model.LeftItem.Length > 1 ? model.LeftItem[1].Title == null ? "" : model.LeftItem[1].Title : "",
                LeftItem3Title = model.LeftItem.Length > 2 ? model.LeftItem[2].Title == null ? "" : model.LeftItem[2].Title : "",
                LeftItem4Title = model.LeftItem.Length > 3 ? model.LeftItem[3].Title == null ? "" : model.LeftItem[3].Title : "",
                RightItem1Title = model.RightItem.Length > 0 ? model.RightItem[0].Title == null ? "" : model.RightItem[0].Title : "",
                RightItem2Title = model.RightItem.Length > 1 ? model.RightItem[1].Title == null ? "" : model.RightItem[1].Title : "",
                RightItem3Title = model.RightItem.Length > 2 ? model.RightItem[2].Title == null ? "" : model.RightItem[2].Title : "",
                LeftItem1LinkMode = model.LeftItem.Length > 0 ? model.LeftItem[0].Link_Mode == 0 ? "1" : model.LeftItem[0].Link_Mode.ToString() : "1",
                LeftItem2LinkMode = model.LeftItem.Length > 1 ? model.LeftItem[1].Link_Mode == 0 ? "1" : model.LeftItem[1].Link_Mode.ToString() : "1",
                LeftItem3LinkMode = model.LeftItem.Length > 2 ? model.LeftItem[2].Link_Mode == 0 ? "1" : model.LeftItem[2].Link_Mode.ToString() : "1",
                LeftItem4LinkMode = model.LeftItem.Length > 3 ? model.LeftItem[3].Link_Mode == 0 ? "1" : model.LeftItem[3].Link_Mode.ToString() : "1",
                RightItem1LinkMode = model.RightItem.Length > 0 ? model.RightItem[0].Link_Mode == 0 ? "1" : model.RightItem[0].Link_Mode.ToString() : "1",
                RightItem2LinkMode = model.RightItem.Length > 1 ? model.RightItem[1].Link_Mode == 0 ? "1" : model.RightItem[1].Link_Mode.ToString() : "1",
                RightItem3LinkMode = model.RightItem.Length > 2 ? model.RightItem[2].Link_Mode == 0 ? "1" : model.RightItem[2].Link_Mode.ToString() : "1",
                RightLinkMode = model.RightLinkMode == 0 ? "1" : model.RightLinkMode.ToString()
            };
            if (model.LeftItem.Length > 0 && model.LeftItem[0].ImageFile != null)
            {
                if (olddata.Count() > 0)
                {
                    if (File.Exists(basepath + olddata.First().LeftItem1ImageNamePath))
                    { File.Delete(basepath + olddata.First().LeftItem1ImageNamePath); }
                }
                var newfilename = DateTime.Now.Ticks + "_" + model.LeftItem[0].ImageFile.FileName.Split('\\').Last();
                var path = basepath + newfilename;
                updateobj.LeftItem1ImageNamePath = newfilename;
                updateobj.LeftItem1ImageName = model.LeftItem[0].ImageFile.FileName.Split('\\').Last();
                model.LeftItem[0].ImageFile.SaveAs(path);
            }
            else
            {
                if (model.LeftItem[0].FileName.IsNullorEmpty() == false)
                {
                    updateobj.LeftItem1ImageNamePath = olddata.Count() == 0 ? "" : olddata.First().LeftItem1ImageNamePath;
                    updateobj.LeftItem1ImageName = olddata.Count() == 0 ? "" : olddata.First().LeftItem1ImageName;
                }
                else
                {
                    updateobj.LeftItem1ImageNamePath = "";
                    updateobj.LeftItem1ImageName = "";
                    //刪除舊檔案
                    if (olddata.Count() > 0)
                    {
                        if (File.Exists(basepath + olddata.First().LeftItem1ImageNamePath))
                        {
                            File.Delete(basepath + olddata.First().LeftItem1ImageNamePath);
                        }
                    }
                }
            }

            if (model.LeftItem.Length > 1 && model.LeftItem[1].ImageFile != null)
            {
                if (olddata.Count() > 0)
                {
                    if (File.Exists(basepath + olddata.First().LeftItem2ImageNamePath))
                    {
                        File.Delete(basepath + olddata.First().LeftItem2ImageNamePath);
                    }
                }
                var newfilename = DateTime.Now.Ticks + "_" + model.LeftItem[1].ImageFile.FileName.Split('\\').Last();
                var path = basepath + newfilename;
                updateobj.LeftItem2ImageNamePath = newfilename;
                updateobj.LeftItem2ImageName = model.LeftItem[1].ImageFile.FileName.Split('\\').Last();
                model.LeftItem[1].ImageFile.SaveAs(path);
            }
            else
            {
                if (model.LeftItem[1].FileName.IsNullorEmpty() == false)
                {
                    updateobj.LeftItem2ImageNamePath = olddata.Count() == 0 ? "" : olddata.First().LeftItem2ImageNamePath;
                    updateobj.LeftItem2ImageName = olddata.Count() == 0 ? "" : olddata.First().LeftItem2ImageName;
                }
                else
                {
                    updateobj.LeftItem2ImageNamePath = "";
                    updateobj.LeftItem2ImageName = "";
                    if (olddata.Count() > 0)
                    {
                        if (File.Exists(basepath + olddata.First().LeftItem2ImageNamePath))
                        {
                            File.Delete(basepath + olddata.First().LeftItem2ImageNamePath);
                        }
                    }
                }
            }

            if (model.LeftItem.Length > 2 && model.LeftItem[2].ImageFile != null)
            {
                if (olddata.Count() > 0)
                {
                    if (File.Exists(basepath + olddata.First().LeftItem3ImageNamePath))
                    {
                        File.Delete(basepath + olddata.First().LeftItem3ImageNamePath);
                    }
                }
                var newfilename = DateTime.Now.Ticks + "_" + model.LeftItem[2].ImageFile.FileName.Split('\\').Last();
                var path = basepath + newfilename;
                updateobj.LeftItem3ImageNamePath = newfilename; updateobj.LeftItem3ImageName = model.LeftItem[2].ImageFile.FileName.Split('\\').Last(); model.LeftItem[2].ImageFile.SaveAs(path);
            }
            else
            {
                if (model.LeftItem[2].FileName.IsNullorEmpty() == false)
                {
                    updateobj.LeftItem3ImageNamePath = olddata.Count() == 0 ? "" : olddata.First().LeftItem3ImageNamePath; updateobj.LeftItem3ImageName = olddata.Count() == 0 ? "" : olddata.First().LeftItem3ImageName;
                }
                else
                {
                    updateobj.LeftItem3ImageNamePath = ""; updateobj.LeftItem3ImageName = "";
                    if (olddata.Count() > 0)
                    {
                        if (File.Exists(basepath + olddata.First().LeftItem3ImageNamePath)) { File.Delete(basepath + olddata.First().LeftItem3ImageNamePath); }
                    }
                }
            }

            if (model.LeftItem.Length > 3 && model.LeftItem[3].ImageFile != null)
            {
                if (olddata.Count() > 0)
                {
                    if (File.Exists(basepath + olddata.First().LeftItem4ImageNamePath))
                    {
                        File.Delete(basepath + olddata.First().LeftItem4ImageNamePath);
                    }
                }
                var newfilename = DateTime.Now.Ticks + "_" + model.LeftItem[3].ImageFile.FileName.Split('\\').Last();
                var path = basepath + newfilename;
                updateobj.LeftItem4ImageNamePath = newfilename;
                updateobj.LeftItem4ImageName = model.LeftItem[3].ImageFile.FileName.Split('\\').Last();
                model.LeftItem[3].ImageFile.SaveAs(path);
            }
            else
            {
                if (model.LeftItem[3].FileName.IsNullorEmpty() == false)
                {
                    updateobj.LeftItem4ImageNamePath = olddata.Count() == 0 ? "" : olddata.First().LeftItem4ImageNamePath; updateobj.LeftItem4ImageName = olddata.Count() == 0 ? "" : olddata.First().LeftItem4ImageName;
                }
                else
                {
                    updateobj.LeftItem4ImageNamePath = ""; updateobj.LeftItem4ImageName = "";
                    if (olddata.Count() > 0)
                    {
                        if (File.Exists(basepath + olddata.First().LeftItem4ImageNamePath)) { File.Delete(basepath + olddata.First().LeftItem4ImageNamePath); }
                    }
                }
            }

            //==
            if (model.RightItem.Length > 0 && model.RightItem[0].ImageFile != null)
            {
                if (olddata.Count() > 0)
                {
                    if (File.Exists(basepath + olddata.First().RightItem1ImageNamePath))
                    { File.Delete(basepath + olddata.First().RightItem1ImageNamePath); }
                }
                var newfilename = DateTime.Now.Ticks + "_" + model.RightItem[0].ImageFile.FileName.Split('\\').Last();
                var path = basepath + newfilename;
                updateobj.RightItem1ImageNamePath = newfilename;
                updateobj.RightItem1ImageName = model.RightItem[0].ImageFile.FileName.Split('\\').Last();
                model.RightItem[0].ImageFile.SaveAs(path);
            }
            else
            {
                if (model.RightItem[0].FileName.IsNullorEmpty() == false)
                {
                    updateobj.RightItem1ImageNamePath = olddata.Count() == 0 ? "" : olddata.First().RightItem1ImageNamePath;
                    updateobj.RightItem1ImageName = olddata.Count() == 0 ? "" : olddata.First().RightItem1ImageName;
                }
                else
                {
                    updateobj.RightItem1ImageNamePath = "";
                    updateobj.RightItem1ImageName = "";
                    if (olddata.Count() > 0)
                    {
                        if (File.Exists(basepath + olddata.First().RightItem1ImageNamePath))
                        {
                            File.Delete(basepath + olddata.First().RightItem1ImageNamePath);
                        }
                    }
                }
            }

            if (model.RightItem.Length > 1 && model.RightItem[1].ImageFile != null)
            {
                if (olddata.Count() > 0)
                {
                    if (File.Exists(basepath + olddata.First().RightItem2ImageNamePath))
                    {
                        File.Delete(basepath + olddata.First().RightItem2ImageNamePath);
                    }
                }
                var newfilename = DateTime.Now.Ticks + "_" + model.RightItem[1].ImageFile.FileName.Split('\\').Last();
                var path = basepath + newfilename;
                updateobj.RightItem2ImageNamePath = newfilename;
                updateobj.RightItem2ImageName = model.RightItem[1].ImageFile.FileName.Split('\\').Last();
                model.RightItem[1].ImageFile.SaveAs(path);
            }
            else
            {
                if (model.RightItem[1].FileName.IsNullorEmpty() == false)
                {
                    updateobj.RightItem2ImageNamePath = olddata.Count() == 0 ? "" : olddata.First().RightItem2ImageNamePath;
                    updateobj.RightItem2ImageName = olddata.Count() == 0 ? "" : olddata.First().RightItem2ImageName;
                }
                else
                {
                    updateobj.RightItem2ImageNamePath = "";
                    updateobj.RightItem2ImageName = "";
                    if (olddata.Count() > 0)
                    {
                        if (File.Exists(basepath + olddata.First().RightItem2ImageNamePath))
                        {
                            File.Delete(basepath + olddata.First().RightItem2ImageNamePath);
                        }
                    }
                }
            }

            if (model.RightItem.Length > 2 && model.RightItem[2].ImageFile != null)
            {
                if (olddata.Count() > 0)
                {
                    if (File.Exists(basepath + olddata.First().RightItem3ImageNamePath))
                    {
                        File.Delete(basepath + olddata.First().RightItem3ImageNamePath);
                    }
                }
                var newfilename = DateTime.Now.Ticks + "_" + model.RightItem[2].ImageFile.FileName.Split('\\').Last();
                var path = basepath + newfilename;
                updateobj.RightItem3ImageNamePath = newfilename; updateobj.RightItem3ImageName = model.RightItem[2].ImageFile.FileName.Split('\\').Last(); model.RightItem[2].ImageFile.SaveAs(path);
            }
            else
            {
                if (model.RightItem[2].FileName.IsNullorEmpty() == false)
                {
                    updateobj.RightItem3ImageNamePath = olddata.Count() == 0 ? "" : olddata.First().RightItem3ImageNamePath; updateobj.RightItem3ImageName = olddata.Count() == 0 ? "" : olddata.First().RightItem3ImageName;
                }
                else
                {
                    updateobj.RightItem3ImageNamePath = ""; updateobj.RightItem3ImageName = "";
                    if (olddata.Count() > 0)
                    {
                        if (File.Exists(basepath + olddata.First().RightItem3ImageNamePath)) { File.Delete(basepath + olddata.First().RightItem3ImageNamePath); }
                    }
                }
            }

            var r = 0;
            if (olddata.Count() == 0)
            {
                r = _pageLayoutop1qlrepository.Create(updateobj);
            }
            else
            {
                r = _pageLayoutop1qlrepository.Update(updateobj);
            }
            if (r > 0)
            {
                return "";
            }
            else
            {
                return "設定失敗";
            }
        }

        #endregion

        #region SavePageLayoutOP2Edit
        public string SavePageLayoutOP2Edit(PageLayoutOP2Model model)
        {
            var basepath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\PageLayout\\";
            if (System.IO.Directory.Exists(basepath) == false) { System.IO.Directory.CreateDirectory(basepath); }
            var olddata = _pageLayoutop2qlrepository.GetByWhere("LangID=@1", new object[] { model.LangID });
            var updateobj = new PageLayoutOP2()
            {
                Introduction = model.Introduction,
                IsShow = model.IsShow,
                LangID = model.LangID,
                Link = model.Link == null ? "" : model.Link,
                Title = model.Title == null ? "" : model.Title,
                LinkMode = model.LinkMode == 0 ? "1" : model.LinkMode.ToString()
            };

            if (model.ImageFile != null)
            {
                if (olddata.Count() > 0)
                {
                    if (File.Exists(basepath + olddata.First().ImageNamePath))
                    { File.Delete(basepath + olddata.First().ImageNamePath); }
                }
                var newfilename = DateTime.Now.Ticks + "_" + model.ImageFile.FileName.Split('\\').Last();
                var path = basepath + newfilename;
                updateobj.ImageNamePath = newfilename;
                updateobj.ImageName = model.ImageFile.FileName.Split('\\').Last();
                model.ImageFile.SaveAs(path);
            }
            else
            {
                if (model.FileName.IsNullorEmpty() == false)
                {
                    updateobj.ImageNamePath = olddata.Count() == 0 ? "" : olddata.First().ImageNamePath;
                    updateobj.ImageName = olddata.Count() == 0 ? "" : olddata.First().ImageName;
                }
                else
                {
                    updateobj.ImageNamePath = "";
                    updateobj.ImageName = "";
                    //刪除舊檔案
                    if (olddata.Count() > 0)
                    {
                        if (File.Exists(basepath + olddata.First().ImageNamePath))
                        {
                            File.Delete(basepath + olddata.First().ImageNamePath);
                        }
                    }
                }
            }
            var r = 0;
            if (olddata.Count() == 0)
            {
                r = _pageLayoutop2qlrepository.Create(updateobj);
            }
            else
            {
                r = _pageLayoutop2qlrepository.Update(updateobj);
            }
            if (r > 0)
            {
                return "";
            }
            else
            {
                return "設定失敗";
            }
        }

        #endregion

        #region SavePageLayoutOP3Edit
        public string SavePageLayoutOP3Edit(PageLayoutOP3Model model)
        {
            var olddata = _pageLayoutop3qlrepository.GetByWhere("LangID=@1", new object[] { model.LangID });
            var updateobj = new PageLayoutOP3()
            {
                Introduction = model.Introduction,
                IsShow = model.IsShow,
                LangID = model.LangID,
                Link = model.Link == null ? "" : model.Link,
                Title = model.Title == null ? "" : model.Title,
                VideoLink = model.VideoLink == null ? "" : model.VideoLink,
                LinkMode = model.LinkMode == 0 ? "1" : model.LinkMode.ToString()
            };

            var r = 0;
            if (olddata.Count() == 0)
            {
                r = _pageLayoutop3qlrepository.Create(updateobj);
            }
            else
            {
                r = _pageLayoutop3qlrepository.Update(updateobj);
            }
            if (r > 0)
            {
                return "";
            }
            else
            {
                return "設定失敗";
            }
        }

        #endregion

        #region SavePageActivity
        public string SavePageActivity(PageLayoutActivityModel model)
        {
            var basepath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\PageLayout\\";
            if (System.IO.Directory.Exists(basepath) == false) { System.IO.Directory.CreateDirectory(basepath); }
            var olddata = _pageLayoutActivityqlrepository.GetByWhere("LangID=@1", new object[] { model.LangID });
            var updateobj = new PageLayoutActivity()
            {
                MoreLinkUrl = model.Link,
                LangID = model.LangID,
                //Item1Desc = model.Items.Length > 0 ? model.Items[0].Desc == null ? "" : model.Items[0].Desc : "",
                //Item2Desc = model.Items.Length > 1 ? model.Items[1].Desc == null ? "" : model.Items[1].Desc : "",
                //Item3Desc = model.Items.Length > 2 ? model.Items[2].Desc == null ? "" : model.Items[2].Desc : "",
                //Item4Desc = model.Items.Length > 3 ? model.Items[3].Desc == null ? "" : model.Items[3].Desc : "",
                //Item5Desc = model.Items.Length > 4 ? model.Items[4].Desc == null ? "" : model.Items[4].Desc : "",
                //Item6Desc = model.Items.Length > 5 ? model.Items[5].Desc == null ? "" : model.Items[5].Desc : "",
                //Item7Desc = model.Items.Length > 6 ? model.Items[6].Desc == null ? "" : model.Items[6].Desc : "",
                Item1Desc = "",
                Item2Desc = "",
                Item3Desc = "",
                Item4Desc = "",
                Item5Desc = "",
                Item6Desc = "",
                Item7Desc = "",
                Item1Link = model.Items.Length > 0 ? model.Items[0].Link == null ? "" : model.Items[0].Link : "",
                Item2Link = model.Items.Length > 1 ? model.Items[1].Link == null ? "" : model.Items[1].Link : "",
                Item3Link = model.Items.Length > 2 ? model.Items[2].Link == null ? "" : model.Items[2].Link : "",
                Item4Link = model.Items.Length > 3 ? model.Items[3].Link == null ? "" : model.Items[3].Link : "",
                Item5Link = model.Items.Length > 4 ? model.Items[4].Link == null ? "" : model.Items[4].Link : "",
                Item6Link = model.Items.Length > 5 ? model.Items[5].Link == null ? "" : model.Items[5].Link : "",
                Item7Link = model.Items.Length > 6 ? model.Items[6].Link == null ? "" : model.Items[6].Link : "",
                Item1Title = model.Items.Length > 0 ? model.Items[0].Title == null ? "" : model.Items[0].Title : "",
                Item2Title = model.Items.Length > 1 ? model.Items[1].Title == null ? "" : model.Items[1].Title : "",
                Item3Title = model.Items.Length > 2 ? model.Items[2].Title == null ? "" : model.Items[2].Title : "",
                Item4Title = model.Items.Length > 3 ? model.Items[3].Title == null ? "" : model.Items[3].Title : "",
                Item5Title = model.Items.Length > 4 ? model.Items[4].Title == null ? "" : model.Items[4].Title : "",
                Item6Title = model.Items.Length > 5 ? model.Items[5].Title == null ? "" : model.Items[5].Title : "",
                Item7Title = model.Items.Length > 6 ? model.Items[6].Title == null ? "" : model.Items[6].Title : "",
            };

            var classtoFrom = typeof(PageLayoutActivity);

            if (model.Items != null)
            {
                for (var idx = 0; idx < model.Items.Length; idx++)
                {
                    PropertyInfo proppath = classtoFrom.GetProperty("Item" + (idx + 1) + "ImageNamePath");
                    PropertyInfo propname = classtoFrom.GetProperty("Item" + (idx + 1) + "ImageName");
                    if (model.Items[idx].ImageFile != null)
                    {
                        var classTypeOldFrom = typeof(PageLayoutActivity);
                        if (olddata.Count() > 0)
                        {
                            var imagepath = classTypeOldFrom.GetProperty("Item" + (idx + 1) + "ImageNamePath").GetValue(olddata.First());
                            if (File.Exists(basepath + imagepath))
                            { File.Delete(basepath + imagepath); }
                        }
                        var newfilename = DateTime.Now.Ticks + "_" + model.Items[idx].ImageFile.FileName.Split('\\').Last();
                        var path = basepath + newfilename;
                        proppath.SetValue(updateobj, newfilename);
                        propname.SetValue(updateobj, model.Items[idx].ImageFile.FileName.Split('\\').Last());

                        model.Items[idx].ImageFile.SaveAs(path);
                    }
                    else
                    {
                        proppath.SetValue(updateobj, "");
                        propname.SetValue(updateobj, "");
                        if (model.Items[idx].FileName.IsNullorEmpty() == false)
                        {
                            var classTypeOldFrom = typeof(PageLayoutActivity);
                            if (olddata.Count() > 0)
                            {
                                var imagepath = classTypeOldFrom.GetProperty("Item" + (idx + 1) + "ImageNamePath").GetValue(olddata.First());
                                var imagename = classTypeOldFrom.GetProperty("Item" + (idx + 1) + "ImageName").GetValue(olddata.First());
                                proppath = classtoFrom.GetProperty("Item" + (idx + 1) + "ImageNamePath");
                                proppath.SetValue(updateobj, imagepath.ToString());
                                propname = classtoFrom.GetProperty("Item" + (idx + 1) + "ImageName");
                                propname.SetValue(updateobj, imagename.ToString());
                            }
                        }
                        else
                        {
                            //刪除舊檔案
                            if (olddata.Count() > 0)
                            {
                                var classTypeOldFrom = typeof(PageLayoutActivity);
                                var imagepath = classTypeOldFrom.GetProperty("Item" + (idx + 1) + "ImageNamePath").GetValue(olddata.First());
                                if (File.Exists(basepath + imagepath))
                                { File.Delete(basepath + imagepath); }
                            }
                        }
                    }
                }
            }

            var r = 0;
            if (olddata.Count() == 0)
            {
                r = _pageLayoutActivityqlrepository.Create(updateobj);
            }
            else
            {
                r = _pageLayoutActivityqlrepository.Update(updateobj);
            }
            if (r > 0)
            {
                return "";
            }
            else
            {
                return "設定失敗";
            }
        }

        #endregion


        #region PageLayoutOP1Model
        public PageLayoutOP1Model GetPageLayoutOP1Edit(string langid)
        {
            var data = _pageLayoutop1qlrepository.GetByWhere("LangID=@1", new object[] { langid });
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            if (data.Count() == 0)
            {
                var model = new PageLayoutOP1Model();

                model.LeftItem = Enumerable.Range(0, 4).Select(_ => new PageLayoutOP1ModelItem()).ToArray();
                model.RightItem = Enumerable.Range(0, 3).Select(_ => new PageLayoutOP1ModelItem()).ToArray();
                return model;
            }
            else
            {
                var model = new PageLayoutOP1Model()
                {
                    Introduction = data.First().Introduction == null ? "" : data.First().Introduction,
                    IsShow = data.First().IsShow,
                    LangID = data.First().LangID,
                    RightLinkMode = data.First().RightLinkMode.IsNullorEmpty() ? 1 : int.Parse(data.First().RightLinkMode),
                    LeftItem = new PageLayoutOP1ModelItem[] {
                              new PageLayoutOP1ModelItem(){  Desc=data.First().LeftItem1Desc, Index=1, Link =data.First().LeftItem1Link, FileName=data.First().LeftItem1ImageName,FilePath=helper.Content("~/UploadImage/PageLayout/" + data.First().LeftItem1ImageNamePath)  ,
                                  Title =data.First().LeftItem1Title, Link_Mode=data.First().LeftItem1LinkMode.IsNullorEmpty()?1:int.Parse(data.First().LeftItem1LinkMode)},
                              new PageLayoutOP1ModelItem(){  Desc=data.First().LeftItem2Desc, Index=2, Link =data.First().LeftItem2Link, FileName=data.First().LeftItem2ImageName,FilePath= helper.Content("~/UploadImage/PageLayout/" + data.First().LeftItem2ImageNamePath) ,
                                  Title =data.First().LeftItem2Title, Link_Mode=data.First().LeftItem2LinkMode.IsNullorEmpty()?1:int.Parse(data.First().LeftItem2LinkMode)},
                              new PageLayoutOP1ModelItem(){  Desc=data.First().LeftItem3Desc, Index=3, Link =data.First().LeftItem3Link, FileName=data.First().LeftItem3ImageName,FilePath=    helper.Content("~/UploadImage/PageLayout/" + data.First().LeftItem3ImageNamePath) ,
                                  Title =data.First().LeftItem3Title, Link_Mode=data.First().LeftItem3LinkMode.IsNullorEmpty()?1:int.Parse(data.First().LeftItem3LinkMode)},
                              new PageLayoutOP1ModelItem(){  Desc=data.First().LeftItem4Desc, Index=4, Link =data.First().LeftItem4Link, FileName=data.First().LeftItem4ImageName,FilePath=  helper.Content("~/UploadImage/PageLayout/" + data.First().LeftItem4ImageNamePath) ,
                                  Title =data.First().LeftItem4Title, Link_Mode=data.First().LeftItem4LinkMode.IsNullorEmpty()?1:int.Parse(data.First().LeftItem4LinkMode) }},
                    LeftTitle = data.First().LeftTitle == null ? "" : data.First().LeftTitle,
                    RightLink = data.First().RightLink == null ? "" : data.First().RightLink,
                    RightTitle = data.First().RightTitle == null ? "" : data.First().RightTitle,
                    RightItem = new PageLayoutOP1ModelItem[] {
                              new PageLayoutOP1ModelItem(){  Desc=data.First().RightItem1Desc, Index=1, Link =data.First().RightItem1Link, FileName=data.First().RightItem1ImageName ,FilePath= helper.Content("~/UploadImage/PageLayout/" + data.First().RightItem1ImageNamePath),
                                  Title =data.First().RightItem1Title, Link_Mode=data.First().RightItem1LinkMode.IsNullorEmpty()?1:int.Parse(data.First().RightItem1LinkMode)},
                              new PageLayoutOP1ModelItem(){  Desc=data.First().RightItem2Desc, Index=2, Link =data.First().RightItem2Link, FileName=data.First().RightItem2ImageName,FilePath=  helper.Content("~/UploadImage/PageLayout/" + data.First().RightItem2ImageNamePath),
                                  Title =data.First().RightItem2Title, Link_Mode=data.First().RightItem2LinkMode.IsNullorEmpty()?1:int.Parse(data.First().RightItem2LinkMode)},
                              new PageLayoutOP1ModelItem(){  Desc=data.First().RightItem3Desc, Index=3, Link =data.First().RightItem3Link, FileName=data.First().RightItem3ImageName,FilePath=  helper.Content("~/UploadImage/PageLayout/" + data.First().RightItem3ImageNamePath),
                                  Title =data.First().RightItem3Title , Link_Mode=data.First().RightItem3LinkMode.IsNullorEmpty()?1:int.Parse(data.First().RightItem3LinkMode)},
                         }
                };
                return model;
            }
        }
        #endregion

        #region PageLayoutOP2Model
        public PageLayoutOP2Model GetPageLayoutOP2Edit(string langid)
        {
            var data = _pageLayoutop2qlrepository.GetByWhere("LangID=@1", new object[] { langid });
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            if (data.Count() == 0)
            {
                var model = new PageLayoutOP2Model();
                return model;
            }
            else
            {
                var model = new PageLayoutOP2Model()
                {
                    Introduction = data.First().Introduction == null ? "" : data.First().Introduction,
                    IsShow = data.First().IsShow,
                    LangID = data.First().LangID,
                    Title = data.First().Title == null ? "" : data.First().Title,
                    Link = data.First().Link == null ? "" : data.First().Link,
                    FileName = data.First().ImageName == null ? "" : data.First().ImageName,
                    FilePath = data.First().ImageNamePath == null ? "" : helper.Content("~/UploadImage/PageLayout/" + data.First().ImageNamePath),
                    LinkMode = data.First().LinkMode.IsNullorEmpty() ? 1 : int.Parse(data.First().LinkMode)
                };
                return model;
            }
        }
        #endregion

        #region PageLayoutOP3Model
        public PageLayoutOP3Model GetPageLayoutOP3Edit(string langid)
        {
            var data = _pageLayoutop3qlrepository.GetByWhere("LangID=@1", new object[] { langid });
            if (data.Count() == 0)
            {
                var model = new PageLayoutOP3Model();
                return model;
            }
            else
            {
                var model = new PageLayoutOP3Model()
                {
                    Introduction = data.First().Introduction == null ? "" : data.First().Introduction,
                    IsShow = data.First().IsShow,
                    LangID = data.First().LangID,
                    Title = data.First().Title == null ? "" : data.First().Title,
                    Link = data.First().Link == null ? "" : data.First().Link,
                    VideoLink = data.First().VideoLink == null ? "" : data.First().VideoLink,
                    LinkMode = data.First().LinkMode.IsNullorEmpty() ? 1 : int.Parse(data.First().LinkMode)
                };
                return model;
            }
        }
        #endregion

        #region PageLayoutActivity
        public PageLayoutActivityModel PageLayoutActivity(string langid)
        {
            var data = _pageLayoutActivityqlrepository.GetByWhere("LangID=@1", new object[] { langid });
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            if (data.Count() == 0)
            {
                var model = new PageLayoutActivityModel();
                model.Items = Enumerable.Range(0, 7).Select(_ => new PageLayoutModelItem()).ToArray();
                return model;
            }
            else
            {
                var model = new PageLayoutActivityModel()
                {
                    Link = data.First().MoreLinkUrl,
                    LangID = data.First().LangID,
                    Items = new PageLayoutModelItem[] {
                       new PageLayoutModelItem(){  Desc=data.First().Item1Desc, Index=1, Link =data.First().Item1Link, FileName=data.First().Item1ImageName,FilePath=helper.Content("~/UploadImage/PageLayout/" + data.First().Item1ImageNamePath) , Title=data.First().Item1Title},
                       new PageLayoutModelItem(){  Desc=data.First().Item2Desc, Index=2, Link =data.First().Item2Link, FileName=data.First().Item2ImageName,FilePath= helper.Content("~/UploadImage/PageLayout/" + data.First().Item2ImageNamePath), Title=data.First().Item2Title},
                       new PageLayoutModelItem(){  Desc=data.First().Item3Desc, Index=3, Link =data.First().Item3Link, FileName=data.First().Item3ImageName,FilePath=  helper.Content("~/UploadImage/PageLayout/" + data.First().Item3ImageNamePath), Title=data.First().Item3Title},
                       new PageLayoutModelItem(){  Desc=data.First().Item4Desc, Index=4, Link =data.First().Item4Link, FileName=data.First().Item4ImageName,FilePath= helper.Content("~/UploadImage/PageLayout/" + data.First().Item4ImageNamePath), Title=data.First().Item4Title},
                       new PageLayoutModelItem(){  Desc=data.First().Item5Desc, Index=5, Link =data.First().Item5Link, FileName=data.First().Item5ImageName,FilePath= helper.Content("~/UploadImage/PageLayout/" + data.First().Item5ImageNamePath), Title=data.First().Item5Title},
                       new PageLayoutModelItem(){  Desc=data.First().Item6Desc, Index=6, Link =data.First().Item6Link, FileName=data.First().Item6ImageName,FilePath= helper.Content("~/UploadImage/PageLayout/" + data.First().Item6ImageNamePath), Title=data.First().Item6Title},
                       new PageLayoutModelItem(){  Desc=data.First().Item7Desc, Index=7, Link =data.First().Item7Link, FileName=data.First().Item7ImageName,FilePath= helper.Content("~/UploadImage/PageLayout/" + data.First().Item7ImageNamePath), Title=data.First().Item7Title}}
                };
                return model;
            }
        }
        #endregion
    }
}
