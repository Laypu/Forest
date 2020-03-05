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
using ViewModels.DBModels;

namespace Services.Manager
{
    public class ModelFileDownloadManager : IModelFileDownloadManager
    {
        readonly SQLRepository<ModelFileDownloadMain> _sqlrepository;
        readonly SQLRepository<GroupFileDownload> _groupsqlrepository;
        readonly SQLRepository<SEO> _seosqlrepository;
        readonly SQLRepository<FileDownloadItem> _itemsqlrepository;
        readonly SQLRepository<FileDownloadUnitSetting> _unitsqlitemrepository;
        readonly SQLRepository<ColumnSetting> _columnsqlrepository;
        readonly SQLRepository<Menu> _menusqlrepository;
        readonly SQLRepository<PageLayout> _pagesqlrepository;
        readonly SQLRepository<ClickCountTable> _clicktablesqlitemrepository;
        readonly SQLRepository<VerifyData> _verifydatasqlrepository;
        readonly SQLRepository<Users> _Usersqlrepository;
        public ModelFileDownloadManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.ModelFileDownloadMain;
            _seosqlrepository = sqlinstance.SEO ;
            _itemsqlrepository = sqlinstance.FileDownloadItem;
            _groupsqlrepository = sqlinstance.GroupFileDownload;
            _unitsqlitemrepository = sqlinstance.FileDownloadUnitSetting;
            _columnsqlrepository = sqlinstance.ColumnSetting;
            _menusqlrepository = sqlinstance.Menu;
            _pagesqlrepository = sqlinstance.PageLayout;
            _clicktablesqlitemrepository = sqlinstance.ClickCountTable;
            _verifydatasqlrepository = sqlinstance.VerifyData;
            _Usersqlrepository = sqlinstance.Users;
        }

        #region GetAll
        public IEnumerable<ModelFileDownloadMain> GetAll()
        {
            return _sqlrepository.GetAll();
        }
        #endregion

        #region Paging
        public Paging<ModelFileDownloadMain> Paging(SearchModelBase model)
        {
            var Paging = new Paging<ModelFileDownloadMain>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("Lang_ID=@1");
            whereobj.Add(model.LangId);
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("Name like @2");
                    whereobj.Add("%" + model.Search + "%");
                }
            }
            var str = string.Join(" and ", wherestr);
            Paging.rows = _sqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            Paging.total = _sqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region Where
        public IEnumerable<ModelFileDownloadMain> Where(ModelFileDownloadMain model)
        {
            return _sqlrepository.GetByWhere(model);
        }
        #endregion

        #region AddUnit
        public string AddUnit(string name, string langid, string account, ref int newid)
        {
            var alldata = _sqlrepository.GetByWhere("Lang_ID=@1 Order By Sort", new object[] { langid });
            var idx = 2;
            foreach (var mdata in alldata)
            {
                mdata.Sort = idx;
                _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { mdata.Sort, mdata.ID });
                idx += 1;
            }
            var Model = new ModelFileDownloadMain();
            Model.Lang_ID = int.Parse(langid);
            Model.ModelID = 4;
            Model.Name = name;
            Model.CreateDate = DateTime.Now;
            Model.UpdateDate = DateTime.Now;
            Model.CreateUser = account;
            Model.UpdateUser = account;
            Model.Sort = 1;
            Model.Status = true;
            var r = _sqlrepository.Create(Model);
            newid = Model.ID;
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

        #region UpdateUnit
        public string UpdateUnit(string name, string id, string account)
        {
            var r = _sqlrepository.Update("Name=@1", "ID=@2", new object[] { name, id });
            if (r > 0) { return "修改成功"; } else { return "修改失敗"; }
        }
        #endregion

        #region UpdateSeq
        public string UpdateSeq(int id, int seq, string langid, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _sqlrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<ModelFileDownloadMain> mtseqdata = null;
                    mtseqdata = _sqlrepository.GetByWhere("Sort>@1 and Lang_ID=@2", new object[] { seq, langid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlrepository.GetCountUseWhere("Lang_ID=@1", new object[] { langid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ModelFileDownloadMain> ltseqdata = null;
                    ltseqdata = _sqlrepository.GetByWhere("Sort<=@1   and Lang_ID=@2", new object[] { qseq, langid }).OrderBy(v => v.Sort).ToArray();
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
                        _sqlrepository.Update(tempmodel);
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _sqlrepository.Update(oldmodel.First());
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
                NLogManagement.SystemLogInfo("更新部落客文章管理排序失敗:" + " error:" + ex.Message);
                return "更新部落客文章管理排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region Delete
        public string Delete(string[] idlist, string delaccount, string langid, string account, string username)
        {
            try
            {
                var r = 0;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var m = _menusqlrepository.GetByWhere("ModelID=4 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (m.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                    var p = _pagesqlrepository.GetByWhere("ModelID=4 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (p.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                }
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\FileDownloadItem\\";
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new ModelFileDownloadMain();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _sqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除ModelFileDownloadMain單元失敗:ID=" + idlist[idx]);
                    }
                    else
                    {
                        _verifydatasqlrepository.DelDataUseWhere("ModelID=4 and ModelMainID=@1", new object[] { idlist[idx] });
                        var allitems = _itemsqlrepository.GetByWhere("ModelID=@1", new object[] { idlist[idx] });
                        _seosqlrepository.DelDataUseWhere("TypeName='FileDownload' and TypeID=@1", new object[] { idlist[idx] });
                        _groupsqlrepository.DelDataUseWhere("Main_ID=@1", new object[] { idlist[idx] });
                        _unitsqlitemrepository.DelDataUseWhere("MainID=@1", new object[] { idlist[idx] });
                        var olditem = _itemsqlrepository.GetByWhere("ModelID=@1", new object[] { idlist[idx] });
                        foreach (var items in olditem)
                        {
                            _itemsqlrepository.Delete(items);
                            _seosqlrepository.DelDataUseWhere("TypeName='FileDownloadItem' and TypeID=@1", new object[] { items.ItemID });
                            if (items.UploadFileName.IsNullorEmpty() == false)
                            {
                                if (System.IO.File.Exists(items.UploadFilePath))
                                {
                                    System.IO.File.Delete(items.UploadFilePath);
                                }
                            }
                            if (items.ImageFileName.IsNullorEmpty() == false)
                            {

                                if (System.IO.File.Exists(oldroot + "\\" + items.ImageFileName))
                                {
                                    System.IO.File.Delete(oldroot + "\\" + items.ImageFileName);
                                }
                            }
                        }
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除ModelFileDownloadMain單元失敗:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _sqlrepository.GetByWhere("Lang_ID=@1", new object[] { langid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    tempmodel.Sort = idx;
                    _sqlrepository.Update(tempmodel);
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除ModelFileDownloadMain單元失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        //group

        #region GetGroupSelectList
        public IList<SelectListItem> GetGroupSelectList(string mainid, bool enabled)
        {
            var list = _groupsqlrepository.GetByWhere("Main_ID=@1 Order By Sort", new object[] { mainid });
            if (enabled)
            {
                list = list.Where(v => v.Enabled == true);
            }
            IList<System.Web.Mvc.SelectListItem> item = new List<System.Web.Mvc.SelectListItem>();
            item.Add(new System.Web.Mvc.SelectListItem() { Text = "全部", Value = "" });
            foreach (var l in list)
            {
                item.Add(new System.Web.Mvc.SelectListItem() { Text = l.Group_Name, Value = l.ID.ToString() });
            }
            return item;
        }
        #endregion

        #region PagingGroup
        public Paging<GroupFileDownload> PagingGroup(SearchModelBase model)
        {
            var Paging = new Paging<GroupFileDownload>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("Main_ID=" + model.ModelID);
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("Group_Name like @1");
                    whereobj.Add("%" + model.Search + "%");
                }
            }
            var str = string.Join(" and ", wherestr);
            Paging.rows = _groupsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);

            Paging.total = _groupsqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region EditGroup
        public string EditGroup(string name, string id, string mainid, string account)
        {
            var r = 0;
            if (id == "-1" || id.IsNullorEmpty())
            {
                var alldata = _groupsqlrepository.GetByWhere("Main_ID=@1", new object[] { mainid });
                if (alldata.Any(v => v.Group_Name == name))
                {
                    return "類別名稱已經存在";
                }
                var maxsort = _groupsqlrepository.GetDataCaculate("Max(Sort)", "Main_ID=@1", new object[] { mainid });
                var group = new GroupFileDownload();
                group.Group_Name = name;
                group.Enabled = true;
                group.Readonly = false;
                group.Seo_Manage = false;
                group.Main_ID = int.Parse(mainid);
                group.Sort = maxsort + 1;
                group.UpdateDatetime = DateTime.Now;
                group.UpdateUser = account;
                r = _groupsqlrepository.Create(group);
                if (r > 0)
                {
                    return "新增成功";
                }
                else
                {
                    return "新增失敗";
                }
            }
            else
            {
                var checkdata = _groupsqlrepository.GetByWhere("Group_Name=@1 and ID!=@2 AND Main_ID=@3", new object[] { name, id, mainid });
                if (checkdata.Count() > 0)
                {
                    return "類別名稱已經存在";
                }
                var group = new GroupFileDownload();
                group.ID = int.Parse(id);
                group.Group_Name = name;
                group.UpdateDatetime = DateTime.Now;
                group.UpdateUser = account;
                r = _groupsqlrepository.Update(group);
                if (r > 0)
                {
                    return "修改成功";
                }
                else
                {
                    return "修改失敗";
                }

            }
        }
        #endregion

        #region DeleteGroup
        public string DeleteGroup(string[] idlist, string delaccount, string account, string username)
        {
            try
            {
                var r = 0;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var users = _itemsqlrepository.GetByWhere("GroupID=@1", new object[] { idlist[idx] });
                    if (users.Count() > 0)
                    {
                        var gname = _groupsqlrepository.GetByWhere("ID=@1", new object[] { idlist[idx] }).First().Group_Name;
                        return "群組名稱:" + gname + " 目前已被使用,無法刪除";
                    }
                    var entity = new GroupFileDownload();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _groupsqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除群組失敗:ID=" + idlist[idx]);
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除群組:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _groupsqlrepository.GetAll().OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    tempmodel.Sort = idx;
                    _groupsqlrepository.Update(tempmodel);
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除群組失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region UpdateGroupStatus
        public string UpdateGroupStatus(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new GroupFileDownload();
                entity.Enabled = status ? true : false;
                entity.UpdateDatetime = DateTime.Now;
                entity.ID = int.Parse(id);
                entity.UpdateUser = account;
                var r = _groupsqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改GroupFileDownload顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改GroupFileDownload顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region UpdateGroupSeq
        public string UpdateGroupSeq(int id, int seq, string mainid, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _groupsqlrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<GroupFileDownload> mtseqdata = null;
                    mtseqdata = _groupsqlrepository.GetByWhere("Sort>@1 and Main_ID=@2", new object[] { seq, mainid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _groupsqlrepository.GetCountUseWhere("Main_ID=@1", new object[] { mainid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<GroupFileDownload> ltseqdata = null;
                    ltseqdata = _groupsqlrepository.GetByWhere("Sort<=@1  and Main_ID=@2", new object[] { qseq, mainid }).OrderBy(v => v.Sort).ToArray();
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
                        _groupsqlrepository.Update(tempmodel);
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _groupsqlrepository.Update(oldmodel.First());
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
                NLogManagement.SystemLogInfo("更新部落客文章管理排序失敗:" + " error:" + ex.Message);
                return "更新部落客文章管理排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        //seo
        #region GetSEO
        public SEOViewModel GetSEO(string mainid)
        {
            var seodata = _seosqlrepository.GetByWhere("TypeName=@1 and TypeID=@2", new object[] { "FileDownload", mainid });
            var model = new SEOViewModel();
            if (seodata.Count() > 0)
            {
                model.ID = seodata.First().ID;
                model.TypeID = seodata.First().TypeID.ToString();
                model.Description = seodata.First().Description;
                model.WebsiteTitle = seodata.First().Title;
                model.Keywords = seodata.Count() == 0 ? new string[10] : new string[] {
                        seodata.First().Keywords1,seodata.First().Keywords2,seodata.First().Keywords3,seodata.First().Keywords4,seodata.First().Keywords5
                    ,seodata.First().Keywords6,seodata.First().Keywords7,seodata.First().Keywords8,seodata.First().Keywords9,seodata.First().Keywords10};
            }
            else
            {
                model.Keywords = new string[10];
                model.TypeID = mainid;
            }
            return model;
        }
        #endregion

        #region SaveSEO
        public string SaveSEO(SEOViewModel model, string LangID)
        {
            var seomodel = new SEO()
            {
                Description = model.Description == null ? "" : model.Description,
                Keywords1 = model.Keywords[0],
                Keywords2 = model.Keywords[1],
                Keywords3 = model.Keywords[2],
                Keywords4 = model.Keywords[3],
                Keywords5 = model.Keywords[4],
                Keywords6 = model.Keywords[5],
                Keywords7 = model.Keywords[6],
                Keywords8 = model.Keywords[7],
                Keywords9 = model.Keywords[8],
                Keywords10 = model.Keywords[9],
                Title = model.WebsiteTitle == null ? "" : model.WebsiteTitle,
                TypeName = "FileDownload",
                TypeID = int.Parse(model.TypeID),
                Lang_ID = int.Parse(LangID)
            };
            var r = 0;
            if (model.ID == -1)
            {
                r = _seosqlrepository.Create(seomodel);
            }
            else
            {
                seomodel.ID = model.ID;
                r = _seosqlrepository.Update(seomodel);
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

        //unit
        #region GetUnitModel
        public FileDownloadUnitSettingModel GetUnitModel(string mainid)
        {
            var data = _unitsqlitemrepository.GetByWhere("MainID=@1", new object[] { mainid });
            var model = new FileDownloadUnitSettingModel();
            model.MainID = int.Parse(mainid);
            if (data.Count() > 0)
            {
                model = new FileDownloadUnitSettingModel()
                {
                    ClassOverview = data.First().ClassOverview,
                    Column1 = data.First().Column1,
                    Column2 = data.First().Column2,
                    Column3 = data.First().Column3,
                    Column4 = data.First().Column4,
                    Column5 = data.First().Column5,
                    Column6 = data.First().Column6,
                    Column7 = data.First().Column7,
                    Column8 = data.First().Column8,
                    Column9 = data.First().Column9,
                    Column10 = data.First().Column10,
                    Column11 = data.First().Column11,
                    Column12 = data.First().Column12,
                    Column13 = data.First().Column13,
                    Column14 = data.First().Column14,
                    Column15 = data.First().Column15,
                    Column16 = data.First().Column16,
                    Column17 = data.First().Column17,
                    Column18 = data.First().Column18,
                    Column19 = data.First().Column19,
                    Column20 = data.First().Column20,
                    IsForward = data.First().IsForward,
                    IsPrint = data.First().IsPrint,
                    IsRSS = data.First().IsRSS,
                    IsShare = data.First().IsShare,
                    MemberAuth = data.First().MemberAuth,
                    MainID = data.First().MainID,
                    ShowCount = data.First().ShowCount,
                    ID = data.First().ID,
                    EMailAuth = data.First().EMailAuth,
                    VIPAuth = data.First().VIPAuth,
                    EnterpriceStudentAuth = data.First().EnterpriceStudentAuth,
                    GeneralStudentAuth = data.First().GeneralStudentAuth,
                    Summary= data.First().Summary
                };
                var cs = data.First().ColumnSetting;
                if (cs.IsNullorEmpty() == false)
                {
                    var csarr = cs.Split('@');
                    var cname = csarr[0].Split(',');
                    var cuse = csarr[1].Split(',');
                    for (var v = 0; v < cname.Length; v++)
                    {
                        model.UnitSettingColumnList.Add(new UnitSettingColumn()
                        {
                            Name = cname[v],
                            Sellected = int.Parse(cuse[v])
                        });
                    }
                }
            }

            if (model.UnitSettingColumnList.Count() == 0)
            {
                var columnlist = _columnsqlrepository.GetByWhere("Type='FileDownload'", null).OrderBy(v => v.Sort);
                foreach (var c in columnlist)
                {
                    model.UnitSettingColumnList.Add(new UnitSettingColumn()
                    {
                        Name = c.ColumnName,
                        Sellected = 1
                    });
                }
            }
            model.ColumnNameMapping = new Dictionary<string, string>();
            model.ColumnNameMapping.Add("代表圖", model.Column1.IsNullorEmpty() ? "代表圖" : model.Column1);
            model.ColumnNameMapping.Add("發佈日期", model.Column2.IsNullorEmpty() ? "發佈日期" : model.Column2);
            model.ColumnNameMapping.Add("標題", model.Column3.IsNullorEmpty() ? "標題" : model.Column3);
            model.ColumnNameMapping.Add("類別", model.Column4.IsNullorEmpty() ? "類別" : model.Column4);
            model.ColumnNameMapping.Add("檔案下載", model.Column5.IsNullorEmpty() ? "檔案下載" : model.Column5);
            return model;
        }
        #endregion

        #region ColumnPaging
        public Paging<ColumnSetting> ColumnPaging(SearchModelBase model)
        {
            var Paging = new Paging<ColumnSetting>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("Type='FileDownload'");
            whereobj.Add(model.LangId);
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("Name like @2");
                    whereobj.Add("%" + model.Search + "%");
                }
            }
            var str = string.Join(" and ", wherestr);
            Paging.rows = _columnsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            Paging.total = _columnsqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region UpdateColumnStatus
        public string UpdateColumnStatus(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new ColumnSetting();
                entity.Show = status ? true : false;
                entity.ID = int.Parse(id);
                var r = _columnsqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改ColumnSetting顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改ColumnSetting顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region UpdateColumnSeq
        public string UpdateColumnSeq(int id, int seq, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _columnsqlrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<ColumnSetting> mtseqdata = null;
                    mtseqdata = _columnsqlrepository.GetByWhere("Sort>@1 and Type=@2", new object[] { seq, "FileDownload" }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _columnsqlrepository.GetCountUseWhere("Type=@1", new object[] { "FileDownload" });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ColumnSetting> ltseqdata = null;
                    ltseqdata = _columnsqlrepository.GetByWhere("Sort<=@1  and Type=@2", new object[] { qseq, "FileDownload" }).OrderBy(v => v.Sort).ToArray();
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
                        _columnsqlrepository.Update(tempmodel);
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _columnsqlrepository.Update(oldmodel.First());
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
                NLogManagement.SystemLogInfo("更新ColumnSetting排序失敗:" + " error:" + ex.Message);
                return "更新ColumnSetting排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region SetUnitModel
        public string SetUnitModel(FileDownloadUnitSettingModel model, string account)
        {
            var newmodel = new FileDownloadUnitSetting();
            newmodel.UpdateDatetime = DateTime.Now;
            newmodel.UpdateUser = account;
            var r = 0;
            var columnsetting = "";
            if (model.ColumnName != null) {
                 columnsetting = string.Join(",", model.ColumnName) + "@" + string.Join(",", model.ColumnUse);
            }
            if (model.ID == -1)
            {
                newmodel.MainID = model.MainID.Value;
                newmodel.ClassOverview = model.ClassOverview;
                newmodel.Column1 = model.Column1;
                newmodel.Column2 = model.Column2;
                newmodel.Column3 = model.Column3;
                newmodel.Column4 = model.Column4;
                newmodel.Column5 = model.Column5;
                newmodel.Column6 = model.Column6;
                newmodel.Column7 = model.Column7;
                newmodel.Column8 = model.Column8;
                newmodel.Column9 = model.Column9;
                newmodel.Column10 = model.Column10;
                newmodel.Column11 = model.Column11;
                newmodel.Column12 = model.Column12;
                newmodel.Column13 = model.Column13;
                newmodel.Column14 = model.Column14;
                newmodel.Column15 = model.Column15;
                newmodel.Column16 = model.Column16;
                newmodel.Column17 = model.Column17;
                newmodel.Column18 = model.Column18;
                newmodel.Column19 = model.Column19;
                newmodel.Column20 = model.Column20;
                newmodel.IsRSS = model.IsRSS;
                newmodel.IsShare = model.IsShare;
                newmodel.IsPrint = model.IsPrint;
                newmodel.IsForward = model.IsForward;
                newmodel.MemberAuth = model.MemberAuth;
                newmodel.ShowCount = model.ShowCount;
                newmodel.VIPAuth = model.VIPAuth;
                newmodel.EMailAuth = model.EMailAuth;
                newmodel.EnterpriceStudentAuth = model.EnterpriceStudentAuth;
                newmodel.GeneralStudentAuth = model.GeneralStudentAuth;
                newmodel.ColumnSetting = columnsetting;
                newmodel.Summary = model.Summary==null?"" : model.Summary;
                r = _unitsqlitemrepository.Create(newmodel);
            }
            else
            {
                newmodel.ID = model.ID.Value;
                newmodel.MainID = model.MainID.Value;
                newmodel.ClassOverview = model.ClassOverview;
                newmodel.Column1 = model.Column1 == null ? "" : model.Column1;
                newmodel.Column2 = model.Column2 == null ? "" : model.Column2;
                newmodel.Column3 = model.Column3 == null ? "" : model.Column3;
                newmodel.Column4 = model.Column4 == null ? "" : model.Column4;
                newmodel.Column5 = model.Column5 == null ? "" : model.Column5;
                newmodel.Column6 = model.Column6 == null ? "" : model.Column6;
                newmodel.Column7 = model.Column7 == null ? "" : model.Column7;
                newmodel.Column8 = model.Column8 == null ? "" : model.Column8;
                newmodel.Column9 = model.Column9 == null ? "" : model.Column9;
                newmodel.Column10 = model.Column10 == null ? "" : model.Column10;
                newmodel.Column11 = model.Column11 == null ? "" : model.Column11;
                newmodel.Column12 = model.Column12 == null ? "" : model.Column12;
                newmodel.Column13 = model.Column13 == null ? "" : model.Column13;
                newmodel.Column14 = model.Column14 == null ? "" : model.Column14;
                newmodel.Column15 = model.Column15 == null ? "" : model.Column15;
                newmodel.Column16 = model.Column16 == null ? "" : model.Column16;
                newmodel.Column17 = model.Column17 == null ? "" : model.Column17;
                newmodel.Column18 = model.Column18 == null ? "" : model.Column18;
                newmodel.Column19 = model.Column19 == null ? "" : model.Column19;
                newmodel.Column20 = model.Column20 == null ? "" : model.Column20;
                newmodel.IsRSS = model.IsRSS;
                newmodel.IsShare = model.IsShare;
                newmodel.IsPrint = model.IsPrint;
                newmodel.IsForward = model.IsForward;
                newmodel.MemberAuth = model.MemberAuth;
                newmodel.ShowCount = model.ShowCount;
                newmodel.VIPAuth = model.VIPAuth;
                newmodel.EMailAuth = model.EMailAuth;
                newmodel.EnterpriceStudentAuth = model.EnterpriceStudentAuth;
                newmodel.GeneralStudentAuth = model.GeneralStudentAuth;
                newmodel.ColumnSetting = columnsetting;
                newmodel.Summary = model.Summary == null ? "" : model.Summary;
                r = _unitsqlitemrepository.Update(newmodel);
            }
            if (r > 0)
            {
                return "修改成功";
            }
            else
            {
                return "修改失敗";
            }

        }
        #endregion

        //edit
        #region GetModelByID
        public FileDownloadEditModel GetModelByID(string modelid, string itemid)
        {
            var data = _itemsqlrepository.GetByWhere("ItemID=@1", new object[] { itemid });
            var maindata = _sqlrepository.GetByWhere("ID=@1", new object[] { modelid });
            if (data.Count() > 0)
            {
                var fdata = data.First();
                var seodata = _seosqlrepository.GetByWhere("TypeName='FileDownloadItem' and TypeID=@1", new object[] { itemid });
                var model = new FileDownloadEditModel()
                {
                    ItemID = fdata.ItemID,
                    ModelName = maindata.Count() == 0 ? "" : maindata.First().Name,
                    Description = seodata.Count() > 0 ? seodata.First().Description : "",
                    ImageFileName = fdata.ImageFileName,
                    ImageFileOrgName = fdata.ImageFileOrgName,
                    UploadFileDesc = fdata.UploadFileDesc,
                    UploadFileName = fdata.UploadFileName,
                    UploadFilePath = fdata.UploadFilePath,
                    WebsiteTitle = seodata.Count() > 0 ? seodata.First().Title : "",
                    Keywords = seodata.Count() == 0 ? new string[10] : new string[] {
                        seodata.First().Keywords1,seodata.First().Keywords2,seodata.First().Keywords3,seodata.First().Keywords4,seodata.First().Keywords5
                    ,seodata.First().Keywords6,seodata.First().Keywords7,seodata.First().Keywords8,seodata.First().Keywords9,seodata.First().Keywords10},
                    HtmlContent = fdata.HtmlContent,
                    ImageFileDesc = fdata.ImageFileDesc,
                    ImageFileLocation = fdata.ImageFileLocation,
                    LinkUrl = fdata.LinkUrl,
                    ModelID = fdata.ModelID.Value,
                    ImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/FileDownloadItem/" + fdata.ImageFileName),
                    EdDate = fdata.EdDate,
                    EdDateStr = fdata.EdDate == null ? "" : fdata.EdDate.Value.ToString("yyyy/MM/dd"),
                    Group_ID = fdata.GroupID == null ? -1 : fdata.GroupID.Value,
                    Link_Mode = fdata.Link_Mode,
                    PublicshStr = fdata.PublicshDate,
                    StDate = fdata.StDate,
                    StDateStr = fdata.StDate == null ? "" : fdata.StDate.Value.ToString("yyyy/MM/dd"),
                    Title = fdata.Title,
                       RelateImageFileOrgName = fdata.RelateImageFileOrgName,
                    RelateImageName = fdata.RelateImageFileName,
                    RelateImagelUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/FileDownloadItem/" + fdata.RelateImageFileName),
                    CreateDatetime = fdata.CreateDatetime == null ? "" : fdata.CreateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    CreateUser = fdata.CreateName,
                    UpdateDatetime = fdata.UpdateDatetime == null ? "" : fdata.UpdateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    UpdateUser = fdata.UpdateName,
                };
                var hasvdata = _verifydatasqlrepository.GetByWhere("ModelID=@1 and ModelMainID=@2 and ModelItemID=@3", new object[] {
                     4, fdata.ModelID.Value,fdata.ItemID
                });
                if (hasvdata.Count() > 0)
                {
                    model.VerifyStatus = hasvdata.First().VerifyStatus == 0 ? "審核中" : (hasvdata.First().VerifyStatus == 1 ? "已通過" : "未通過");
                    model.VerifyDateTime = hasvdata.First().VerifyDateTime == null ? "" : hasvdata.First().VerifyDateTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
                    model.VerifyUser = hasvdata.First().VerifyName;
                }
                return model;
            }
            else
            {
                FileDownloadEditModel model = new FileDownloadEditModel();
                model.Keywords = new string[10];
                model.ModelID = int.Parse(modelid);
                model.ModelName = maindata.Count() == 0 ? "" : maindata.First().Name;
                model.PublicshStr = DateTime.Now.ToString("yyyy/MM/dd");
                return model;
            }

        }
        #endregion

        #region CreateItem
        public string CreateItem(FileDownloadEditModel model, string LangId, string account)
        {

            var olddata = _itemsqlrepository.GetByWhere("ModelID=@1", new object[] { model.ModelID });
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var savemodel = new FileDownloadItem()
            {
                HtmlContent = model.HtmlContent,
                ImageFileDesc = model.ImageFileDesc,
                ImageFileLocation = model.ImageFileLocation,
                ModelID = model.ModelID,
                ImageFileOrgName = model.ImageFileOrgName,
                LinkUrl = model.LinkUrl,
                UploadFileDesc = model.UploadFileDesc,
                UploadFileName = model.UploadFileName,
                UploadFilePath = model.UploadFilePath,
                ImageFileName = model.ImageFileName,
                Sort = 1,
                ClickCnt = 0,
                GroupID = model.Group_ID,
                Lang_ID = int.Parse(LangId),
                Title = model.Title,
                PublicshDate = model.PublicshStr,
                UpdateDatetime = DateTime.Now,
                UpdateUser = account,
                Enabled = true,
                Link_Mode = model.Link_Mode,
                RelateImageFileName = model.RelateImageName,
                RelateImageFileOrgName = model.RelateImageFileOrgName,
                CreateDatetime = DateTime.Now,
                CreateUser = account,
                CreateName = admin.Count() == 0 ? "" : admin.First().User_Name,
                IsVerift = false,
            };
            if (model.EdDateStr.IsNullorEmpty() == false)
            {
                savemodel.EdDate = DateTime.Parse(model.EdDateStr);
            }
            if (model.EdDateStr.IsNullorEmpty() == false)
            {
                savemodel.EdDate = DateTime.Parse(model.EdDateStr);
            }
            if (model.StDateStr.IsNullorEmpty() == false)
            {
                savemodel.StDate = DateTime.Parse(model.StDateStr);
            }
            var r = _itemsqlrepository.Create(savemodel);
            if (r > 0)
            {
                _verifydatasqlrepository.DelDataUseWhere("ModelID=@1 and ModelMainID=@2 and ModelItemID=@3", new object[] { 4, savemodel.ModelID.Value, savemodel.ItemID });
                _verifydatasqlrepository.Create(new VerifyData()
                {
                    ModelID = 4,
                    ModelItemID = savemodel.ItemID,
                    ModelName = savemodel.Title,
                    ModelMainID = savemodel.ModelID.Value,
                    VerifyStatus = 0,
                    ModelStatus = 1,
                    UpdateDateTime = DateTime.Now,
                    UpdateUser = admin.Count() == 0 ? "" : admin.First().User_Name,
                    UpdateAccount = account,
                    LangID = int.Parse(LangId)
                });
                foreach (var odata in olddata)
                {
                    _itemsqlrepository.Update("Sort=@1", "ItemID=@2", new object[] { odata.Sort + 1, odata.ItemID });
                }
                return "新增成功";
            }
            else
            {
                return "新增失敗";
            }

        }
        #endregion

        #region UpdateItem
        public string UpdateItem(FileDownloadEditModel model, string LangId, string account)
        {
            var olddata = _itemsqlrepository.GetByWhere("ItemID=@1", new object[] { model.ItemID });
            var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\FileDownloadItem\\";
            //刪除舊檔案
            if (model.UploadFileName.IsNullorEmpty())
            {
                if (System.IO.File.Exists(olddata.First().UploadFilePath))
                {
                    System.IO.File.Delete(olddata.First().UploadFilePath);
                }
                model.UploadFilePath = "";
                model.UploadFileName = "";
            }
            else
            {
                if (olddata.First().UploadFileName != model.UploadFileName)
                {
                    if (System.IO.File.Exists(olddata.First().UploadFilePath))
                    {
                        System.IO.File.Delete(olddata.First().UploadFilePath);
                    }
                }
            }
          if (model.RelateImageName.IsNullorEmpty())
            {

                if (System.IO.File.Exists(oldroot + "\\" + olddata.First().RelateImageFileName))
                {
                    System.IO.File.Delete(oldroot + "\\" + olddata.First().RelateImageFileName);
                }
                model.RelateImageFileOrgName = "";
                model.RelateImageName = "";
            }
            else
            {

                if (olddata.First().RelateImageFileName != model.RelateImageName)
                {
                    if (System.IO.File.Exists(oldroot + "\\" + olddata.First().RelateImageFileName))
                    {
                        System.IO.File.Delete(oldroot + "\\" + olddata.First().RelateImageFileName);
                    }
                }
            }

            //刪除seo
            _seosqlrepository.DelDataUseWhere("TypeName='FileDownloadItem' and TypeID=@1", new object[] { model.ItemID });
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var savemodel = new FileDownloadItem()
            {
                ItemID = model.ItemID,
                HtmlContent = model.HtmlContent == null ? "" : model.HtmlContent,
                ImageFileDesc = model.ImageFileDesc == null ? "" : model.ImageFileDesc,
                ImageFileLocation = model.ImageFileLocation = model.ImageFileLocation,
                ModelID = model.ModelID,
                ImageFileOrgName = model.ImageFileOrgName,
                LinkUrl = model.LinkUrl == null ? "" : model.LinkUrl,
                UploadFileDesc = model.UploadFileDesc == null ? "" : model.UploadFileDesc,
                UploadFileName = model.UploadFileName,
                UploadFilePath = model.UploadFilePath == null ? "" : model.UploadFilePath,
                ImageFileName = model.ImageFileName,
                GroupID = model.Group_ID,
                Lang_ID = int.Parse(LangId),
                Title = model.Title,
                PublicshDate = model.PublicshStr == null ? "" : model.PublicshStr,
                UpdateDatetime = DateTime.Now,
                UpdateUser = account,
                Enabled = true,
                Link_Mode = model.Link_Mode,
                       RelateImageFileName = model.RelateImageName,
                RelateImageFileOrgName = model.RelateImageFileOrgName,
                UpdateName = admin.Count() == 0 ? "" : admin.First().User_Name,
                IsVerift = false
            };
            if (model.EdDateStr.IsNullorEmpty() == false)
            {
                savemodel.EdDate = DateTime.Parse(model.EdDateStr);
            }
            if (model.EdDateStr.IsNullorEmpty() == false)
            {
                savemodel.EdDate = DateTime.Parse(model.EdDateStr);
            }
            if (model.StDateStr.IsNullorEmpty() == false)
            {
                savemodel.StDate = DateTime.Parse(model.StDateStr);
            }

            var r = _itemsqlrepository.Update(savemodel);
            if (r > 0)
            {
                var hasvdata = _verifydatasqlrepository.GetByWhere("ModelID=@1 and ModelMainID=@2 and ModelItemID=@3", new object[] {
                     4, savemodel.ModelID.Value,savemodel.ItemID
                });
                if (hasvdata.Count() == 0)
                {
                    _verifydatasqlrepository.Create(new VerifyData()
                    {
                        ModelID = 4,
                        ModelItemID = savemodel.ItemID,
                        ModelName = savemodel.Title,
                        ModelMainID = savemodel.ModelID.Value,
                        VerifyStatus = 0,
                        ModelStatus = 2,
                        UpdateDateTime = DateTime.Now,
                        UpdateUser = admin.Count() == 0 ? "" : admin.First().User_Name,
                        UpdateAccount = account,
                        LangID = int.Parse(LangId)
                    });
                }
                else
                {
                    _verifydatasqlrepository.Update("VerifyStatus=0,ModelStatus=2,VerifyDateTime=Null,VerifyUser='',VerifyName='',ModelName=@1,UpdateDateTime=@2,UpdateUser=@3,UpdateAccount=@4",
                        "ModelID=@5 and ModelMainID=@6 and ModelItemID=@7", new object[] {
                           savemodel.Title,DateTime.Now, (admin.Count() == 0 ? "" : admin.First().User_Name),account,4 , savemodel.ModelID.Value, savemodel.ItemID
                    });
                }
                return "修改成功";
            }
            else
            {
                return "修改失敗";
            }
        }

        #endregion

        #region PagingItem
        public Paging<FileDownloadItemResult> PagingItem(string modelid, FileDownloadSearchModel model)
        {
            var Paging = new Paging<FileDownloadItemResult>();
            var data = new List<FileDownloadItem>();

            var whereobj = new List<object>();
            var wherestr = new List<string>();
            var idx = 1;
            wherestr.Add("ModelID=@1");
            whereobj.Add(modelid);
            if (model.GroupId != null)
            {
                idx += 1;
                wherestr.Add("GroupID=@" + idx);
                whereobj.Add(model.GroupId.Value.ToString());
            }
            if (string.IsNullOrEmpty(model.Title) == false)
            {
                idx += 1;
                wherestr.Add("(Title like @" + idx + ")");
                whereobj.Add("%" + model.Title + "%");
            }

            if (model.Enabled != null)
            {
                idx += 1;
                wherestr.Add("Enabled=@" + idx);
                whereobj.Add(model.Enabled);
            }
            if (string.IsNullOrEmpty(model.PublicshDateFrom) == false)
            {
                idx += 1;
                wherestr.Add("(PublicshDate >=@" + idx + " or PublicshDate IS Null)");
                whereobj.Add(model.PublicshDateFrom);
            }
            if (string.IsNullOrEmpty(model.PublicshDateTo) == false)
            {
                idx += 1;
                wherestr.Add("(PublicshDate <=@" + idx + " or  PublicshDate IS Null)");
                whereobj.Add(model.PublicshDateTo);
            }
            if (string.IsNullOrEmpty(model.DisplayFrom) == false)
            {
                idx += 1;
                wherestr.Add("(EdDate IS NULL OR EdDate >=@" + idx + ")");
                whereobj.Add(model.DisplayFrom);
            }
            if (string.IsNullOrEmpty(model.DisplayTo) == false)
            {
                idx += 1;
                wherestr.Add("(StDate IS NULL OR StDate <=@" + idx + ")");
                whereobj.Add(model.DisplayTo);
            }

            if (string.IsNullOrEmpty(model.IsRange) == false)
            {
                idx += 1;
                if (model.IsRange == "1")
                {
                    wherestr.Add("((StDate <=@" + idx + " or StDate is null or StDate='') and (EdDate >=@" + idx + " or EdDate is null or EdDate=''))");
                    whereobj.Add(DateTime.Now);
                }
                else
                {
                    wherestr.Add("((StDate >@" + idx + ") or (EdDate <@" + idx + "))");
                    whereobj.Add(DateTime.Now);
                }
            }

            if (wherestr.Count() > 0)
            {
                var str = string.Join(" and ", wherestr);
                data = _itemsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
                Paging.total = _itemsqlrepository.GetCountUseWhere(str, whereobj.ToArray());

            }
            else
            {
                Paging.total = _itemsqlrepository.GetAllDataCount();
                data = _itemsqlrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort).ToList();
            }

            var ALLGroup = _groupsqlrepository.GetAll();
            foreach (var d in data)
            {
                var isrange = false;
                if (d.StDate == null && d.EdDate == null) { isrange = true; }
                else if (d.StDate != null && d.EdDate == null)
                {
                    if (DateTime.Now >= d.StDate.Value)
                    {
                        isrange = true;
                    }
                }
                else if (d.StDate == null && d.EdDate != null)
                {
                    if (DateTime.Now <= d.EdDate.Value.AddDays(1))
                    {
                        isrange = true;
                    }
                }
                else if (d.StDate != null && d.EdDate != null)
                {
                    if (DateTime.Now >= d.StDate.Value && DateTime.Now <= d.EdDate.Value.AddDays(1))
                    {
                        isrange = true;
                    }
                }
                var modelverifydata = _verifydatasqlrepository.GetByWhere("ModelID=4 and ModelMainID=@1", new object[] { modelid });
                var gname = ALLGroup.Where(v => v.ID == d.GroupID);
                var vdata = modelverifydata.Where(v => v.ModelItemID == d.ItemID);
                Paging.rows.Add(new FileDownloadItemResult()
                {
                    ItemID = d.ItemID,
                    Title = d.Title,
                    ClickCount = d.ClickCnt == null ? "0" : d.ClickCnt.Value.ToString(),
                    Enabled = d.Enabled,
                    IsRange = isrange == true ? "是" : "否",
                    GroupName = gname.Count() > 0 ? gname.First().Group_Name : "無分類",
                    PublicshDate = d.PublicshDate,
                    Sort = d.Sort.Value,
                    VerifyStr = vdata.Count() == 0 ? (d.IsVerift.Value ? "已通過" : "審核中") : vdata.First().VerifyStatus == 0 ? "審核中" : (vdata.First().VerifyStatus == 1 ? "已通過" : "未通過"),
                });

            }
            return Paging;
        }
        #endregion

        #region UpdateItemSeq
        public string UpdateItemSeq(int modelid, int id, int seq, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _itemsqlrepository.GetByWhere("ItemID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<FileDownloadItem> mtseqdata = null;
                    mtseqdata = _itemsqlrepository.GetByWhere("Sort>@1 and ModelID=@2", new object[] { seq, modelid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _itemsqlrepository.GetCountUseWhere("ModelID=@1", new object[] { modelid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<FileDownloadItem> ltseqdata = null;
                    ltseqdata = _itemsqlrepository.GetByWhere("Sort<=@1 and ModelID=@2", new object[] { qseq, modelid }).OrderBy(v => v.Sort).ToArray();
                    //更新seq+1
                    var sidx = 0;
                    for (var idx = 1; idx <= ltseqdata.Count(); idx++)
                    {
                        if (idx == seq && seq < oldmodel.First().Sort)
                        {
                            sidx += 1;
                        }
                        var tempmodel = ltseqdata[idx - 1];
                        if (tempmodel.ItemID == id)
                        {
                            continue;
                        }
                        else
                        {
                            sidx += 1;
                        }
                        tempmodel.Sort = sidx;
                        _itemsqlrepository.Update(tempmodel);
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _itemsqlrepository.Update(oldmodel.First());
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
                NLogManagement.SystemLogInfo("更新ActiveItem排序失敗:" + " error:" + ex.Message);
                return "更新ActiveItem排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region DeleteItem
        public string DeleteItem(string[] idlist, string delaccount, string account, string username)
        {
            try
            {
                var r = 0;
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\FileDownloadItem\\";
                var modelid = -1;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new FileDownloadItem();
                    var olditem = _itemsqlrepository.GetByWhere("ItemID=@1", new object[] { idlist[idx] });
                    modelid = olditem.First().ModelID.Value;
                    entity.ItemID = int.Parse(idlist[idx]);
                    r = _itemsqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除群組失敗:ID=" + idlist[idx]);
                    }
                    else
                    {
                        _verifydatasqlrepository.DelDataUseWhere("ModelID=4 and ModelMainID=@1 and ModelItemID=@2", new object[] { modelid, idlist[idx] });
                        _seosqlrepository.DelDataUseWhere("TypeName='FileDownloadItem' and TypeID=@1", new object[] { idlist[idx] });
                        if (olditem.First().UploadFileName.IsNullorEmpty() == false)
                        {
                            if (System.IO.File.Exists(olditem.First().UploadFilePath))
                            {
                                System.IO.File.Delete(olditem.First().UploadFilePath);
                            }
                        }
                        if (olditem.First().ImageFileName.IsNullorEmpty() == false)
                        {

                            if (System.IO.File.Exists(oldroot + "\\" + olditem.First().ImageFileName))
                            {
                                System.IO.File.Delete(oldroot + "\\" + olditem.First().ImageFileName);
                            }
                        }
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除群組:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _itemsqlrepository.GetByWhere("ModelID=@1", new object[] { modelid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    _itemsqlrepository.Update("Sort=@1", "ItemID=@2", new object[] { idx, tempmodel.ItemID });
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除群組失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region SetItemStatus
        public string SetItemStatus(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new FileDownloadItem();
                entity.Enabled = status ? true : false;
                entity.ItemID = int.Parse(id);
                var r = _itemsqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改FileDownloadItem顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改FileDownloadItem顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region PagingItemForWebSite
        public Paging<FileDownloadItemResult> PagingItemForWebSite(string modelid, FileDownloadSearchModel model, string nogroupstr)
        {
            var Paging = new Paging<FileDownloadItemResult>();
            var data = new List<FileDownloadItem>();

            var whereobj = new List<object>();
            var wherestr = new List<string>();
            var idx = 1;
            wherestr.Add("ModelID=@1");
            whereobj.Add(modelid);
            var nowdate = DateTime.Now.ToString("yyyy/MM/dd");
            idx += 1;
            wherestr.Add("PublicshDate <=@" + idx);
            whereobj.Add(nowdate);

            idx += 1;
            wherestr.Add("Enabled=@" + idx);
            whereobj.Add(true);

            idx += 1;
            wherestr.Add("(StDate <=@" + idx + " or StDate is null or StDate='')");
            whereobj.Add(nowdate);

            idx += 1;
            wherestr.Add("(EdDate >=@" + idx + " or EdDate is null or EdDate='')");
            whereobj.Add(nowdate);

            wherestr.Add("IsVerift=1");

            if (string.IsNullOrEmpty(model.DisplayTo) == false)
            {
                idx += 1;
                wherestr.Add("(PublicshDate <=@" + idx + " or PublicshDate is null or PublicshDate='')");
                whereobj.Add(model.DisplayTo);
            }
            if (string.IsNullOrEmpty(model.DisplayFrom) == false)
            {
                idx += 1;
                wherestr.Add("(PublicshDate >=@" + idx + " or PublicshDate is null or PublicshDate='')");
                whereobj.Add(model.DisplayFrom);
            }

            if (string.IsNullOrEmpty(model.Title) == false)
            {
                idx += 1;
                wherestr.Add("(Title like @" + idx + " or HtmlContent like @" + idx + ")");
                whereobj.Add("%" + model.Title + "%");
            }

            var ALLGroup = _groupsqlrepository.GetByWhere("Main_ID=@1 and Enabled=@2", new object[] { modelid, 1 });
            if (model.GroupId != null)
            {
                idx += 1;
                wherestr.Add("GroupID=@" + idx);
                whereobj.Add(model.GroupId.Value.ToString());
            }
            else
            {
                if (ALLGroup.Count() > 0)
                {
                    var enabled = ALLGroup.Where(v => v.Enabled == true).Select(v => v.ID.ToString());
                    var instr = string.Join(",", enabled);
                    wherestr.Add("GroupID In(" + instr + ",0)");
                }
                else
                {
                    wherestr.Add("GroupID=0");
                }
            }


            if (wherestr.Count() > 0)
            {
                var str = string.Join(" and ", wherestr);
                if (model.Limit == -1)
                {
                    data = _itemsqlrepository.GetByWhere(str + " order by " + model.Sort, whereobj.ToArray()).ToList();
                }
                else
                {
                    data = _itemsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
                }
                Paging.total = _itemsqlrepository.GetCountUseWhere(str, whereobj.ToArray());

            }
            else
            {
                if (model.Limit == -1)
                {
                    data = _itemsqlrepository.GetAll("*", model.Sort).ToList();
                }
                else
                {
                    data = _itemsqlrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort).ToList();
                }
                Paging.total = _itemsqlrepository.GetAllDataCount();
            }
            var sidx = model.Offset;
            foreach (var d in data)
            {

                var gname = ALLGroup.Where(v => v.ID == d.GroupID);

                sidx += 1;
                Paging.rows.Add(new FileDownloadItemResult()
                {
                    ItemID = d.ItemID,
                    Title = d.Title,
                    ClickCount = d.ClickCnt == null ? "0" : d.ClickCnt.Value.ToString(),
                    Enabled = d.Enabled,
                    GroupName = gname.Count() > 0 ? gname.First().Group_Name : nogroupstr,
                    PublicshDate = d.PublicshDate == null ? "" : d.PublicshDate,
                    Sort = sidx,
                    UploadFileName = d.UploadFileName == null ? "" : d.UploadFileName,
                    RelatceImageFileName = d.RelateImageFileName == null ? "" : d.RelateImageFileName,
                    UploadFilePath=d.UploadFilePath,
                     UploadFileDesc = d.UploadFileDesc == null ? "" : d.UploadFileDesc,
                });

            }
            return Paging;
        }
        #endregion

        #region GetModelItem
        public FileDownloadItem GetModelItem(string itemid)
        {
            var data = _itemsqlrepository.GetByWhere("ItemID=@1", new object[] { itemid });
            if (data.Count() > 0)
            {
                return data.First();
            }
            else
            {
                return new FileDownloadItem();
            }
        }
        #endregion

        #region UpdateClickCount
        public void UpdateClickCount(string itemid)
        {
            string tClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            var HTTP_VIA = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"];
            if (HTTP_VIA.IsNullorEmpty() == false)
            {
                var viaip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (viaip.IsNullorEmpty() == false)
                {
                    tClientIP = viaip;
                }
            }
            _clicktablesqlitemrepository.DelDataUseWhere("Type='FileDownload' and LastClickDatetime<@1", new object[] { DateTime.Now.AddDays(-1) });
            var hasdata = _clicktablesqlitemrepository.GetByWhere("Type='FileDownload' and IP=@1 and ID=@2", new object[] { tClientIP, itemid });
            if (hasdata.Count() == 0)
            {
                _itemsqlrepository.Update("ClickCnt=ClickCnt+1", "ItemID=@1", new object[] { itemid });
                _clicktablesqlitemrepository.Create(new ClickCountTable()
                {
                    IP = tClientIP,
                    LastClickDatetime = DateTime.Now,
                    Type = "FileDownload",
                    ID = itemid
                });
            }

        }
        #endregion

        #region GetAllGroupSelectList
        public IList<SelectListItem> GetAllGroupSelectList(string mainid)
        {
            var list = _groupsqlrepository.GetByWhere("Main_ID=@1 Order By Sort", new object[] { mainid });
            IList<System.Web.Mvc.SelectListItem> item = new List<System.Web.Mvc.SelectListItem>();
            item.Add(new System.Web.Mvc.SelectListItem() { Text = "無分類", Value = "0" });
            foreach (var l in list)
            {
                item.Add(new System.Web.Mvc.SelectListItem() { Text = l.Group_Name, Value = l.ID.ToString() });
            }
            return item;
        }
        #endregion
    }
}
