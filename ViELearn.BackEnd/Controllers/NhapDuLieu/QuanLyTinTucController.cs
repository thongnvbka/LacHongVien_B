using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using ViELearn.DAL.ModulesDAL;
using ViELearn.Models.ProjectModels;
using ViELearn.BackEnd.Ultilities;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using PagedList;
using PagedList.Mvc;
using System.Drawing;
using ViElearn.Services;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json;
using SD = System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ViELearn.BackEnd.Controllers
{
    public class QuanLyTinTucController : Controller
    {
        int userId = int.Parse(SysBaseInfor.GetIdNguoiDung());
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public ActionResult Index(int cat = 0, int status = 0, string title = "")
        {
             DataTable dt = new DataTable();
            if (cat==0 && status==0)
            {
                var sql = $@"SELECT n.*,d.TenDanhMuc,d.MaDanhMuc AS cate_slug,u.DisplayName AS DisplayName  FROM  News n JOIN dbo.DanhMucChung d ON n.CatesId = d.id LEFT JOIN dbo.AspNetUsers u ON u.Id = n.CreatedBy Where n.Status = 2 ORDER BY n.CreatedAt DESC, n.Id desc";
                dt = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            }
            else
            {
                Hashtable args2 = new Hashtable();
                args2.Add("title", title);
                args2.Add("catId", cat);
                args2.Add("status", status);
                dt = DBLibs.ExecuteStoreProcedure_Select("be_GetNewsSearch", args2, _cnn);
            }
            ViewBag.cat = Request["cat"].MapInt();
            ViewBag.CatId = cat;
            ViewBag.status = status;
            ViewBag.DataIndex = dt;
            var model = DanhMucChungServices.Instance.GetListDanhMucChung();
            return View(model);
        }
         public ActionResult SortNews()
        {
            var sql = $@"SELECT *,dbo.GetCountNewsByCat(id)'TongSoTin' FROM dbo.DanhMucChung Where Type <> 0 AND LoaiDanhMuc=1 AND ShowHome = 1 ORDER BY SoThuTu ,id";
            var dtCat = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsCat = dtCat;
            return View();
        }

        public ActionResult Instruction()
        {
            return View();
        }
        #region partial
        public ActionResult GetNewsbyCatPartial(int id = 0)
        {
            var sql = $@"SELECT TOP 1 id FROM dbo.DanhMucChung Where Type <> 0 AND ShowHome = 1 AND SoThuTu = 1 AND LoaiDanhMuc=1";
            var dtCat = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            var dmId = dtCat.Rows[0]["id"];
            Hashtable args2 = new Hashtable();
            args2.Add("id", (id>0)?id:dmId);
            var dt = DBLibs.ExecuteStoreProcedure_Select("be_GetNewsbyCatId", args2, _cnn);
            ViewBag.Data = dt;
            return PartialView("SortNewPartial");
        }

        public ActionResult Home(int cat = 0, int status = 0)
        {

            ViewBag.CatId = cat;
            ViewBag.status = status;

            var sql = $@"SELECT * FROM dbo.DanhMucChung Where Type <> 0 AND LoaiDanhMuc=1 ORDER BY TenDanhMuc";
            var dtCat = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsCat = dtCat;

            return View();
        }
        public ActionResult GetNewsFilterbyCatPartial(int id = 0)
        {
            //var sql = $@"SELECT TOP 1 id FROM dbo.DanhMucChung Where Type <> 0 AND ShowHome = 1 AND SoThuTu = 1 AND LoaiDanhMuc=1";
            //var dtCat = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            //var dmId = dtCat.Rows[0]["id"];
            Hashtable args2 = new Hashtable();
            args2.Add("id",id /*(id > 0) ? id : dmId)*/);
            var dt = DBLibs.ExecuteStoreProcedure_Select("be_GetAllNewsbyCatId", args2, _cnn);
            ViewBag.Data = dt;
            return PartialView("Search");
        }
        public ActionResult HomeSearch(int cat = 0, int status = 0, string title = "")
        {

            DataTable dt = new DataTable();
            if (status == 0)
            {
                Hashtable args2 = new Hashtable();
                args2.Add("title", title);
                args2.Add("catId", cat);
                dt = DBLibs.ExecuteStoreProcedure_Select("be_GetAllNewsSearch", args2, _cnn);
                ViewBag.Data = dt;
            }
            else
            {
                Hashtable args2 = new Hashtable();
                args2.Add("title", title);
                args2.Add("catId", cat);
                args2.Add("status", status);
                dt = DBLibs.ExecuteStoreProcedure_Select("be_GetNewsSearch", args2, _cnn);
                ViewBag.Data = dt;
            }
            return PartialView("HomeSearch");

        }

        public ActionResult Details(int newsId = 0)

        {

            var sql = $@"SELECT TOP 1 * FROM dbo.NewsHistory WHERE NewsId = {newsId}";
            var dt = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.dt = dt.Rows[0];
            return View();

        }
        [HttpPost]
        public ActionResult Search(int cat = 0, int status = 0, string title = "")
        {
            
            DataTable dt = new DataTable();
            if(status == 0)
            {
                Hashtable args2 = new Hashtable();
                args2.Add("title", title);
                args2.Add("catId", cat);
                dt = DBLibs.ExecuteStoreProcedure_Select("be_GetAllNewsSearch", args2, _cnn);
                ViewBag.Data = dt;
            }
            else
            {
                Hashtable args2 = new Hashtable();
                args2.Add("title", title);
                args2.Add("catId", cat);
                args2.Add("status", status);
                dt = DBLibs.ExecuteStoreProcedure_Select("be_GetNewsSearch", args2, _cnn);
                ViewBag.Data = dt;
            }
            return PartialView("Search");

        }
    
        [HttpPost]
        public ActionResult ResultNews(int id = 0,int cat=0, int status = 2, string title = "")
        {
            //bool stt = false; string msg = "";
            DataTable dt = new DataTable();
            if (id > 0)
            {
                Hashtable args2 = new Hashtable();
                args2.Add("id", id);
                args2.Add("title", title);
                args2.Add("catId", cat);
                args2.Add("status", status);
                dt = DBLibs.ExecuteStoreProcedure_Select("be_GetNewsSearch_related", args2, _cnn);
            }
            else
            {
                Hashtable args2 = new Hashtable();
                args2.Add("title", title);
                args2.Add("catId", cat);
                args2.Add("status", status);
                dt = DBLibs.ExecuteStoreProcedure_Select("be_GetNewsSearch", args2, _cnn);
            }
            //ViewBag.Data = dt;
            return PartialView(dt);
       


        }

        #endregion


        public ActionResult PostNewsCalendar()
        {
            return View();
        }

        public ActionResult CalendarResult()
        {

            JsonResult result = new JsonResult();
            try
            {
                // Loading.  
                var data = this.GetCalendarn();

                // Processing.  
                result = this.Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            // Return info.  
            return result;
        }
        public string GetCalendarn()
        {
            var dt = DBLibs.GetDataBy_DataAdapter("SELECT Id,Title,PublishDate FROM dbo.News WHERE Status =2", _cnn);
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                  }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }
        //   [HttpPost]
        //   public JsonResult SaveSort(
        //   string values
        //)
        //   {
        //       values = values.TrimEnd(';');
        //       string[] arrId = values.Split(';');
        //       foreach (var item in arrId)
        //       {
        //           int id = Convert.ToInt32(item.Split(':')[0]);
        //           int stt = Convert.ToInt32(item.Split(':')[1]);
        //           if (id > 0)
        //           {
        //               var sql = $@"
        //           UPDATE dbo.Newslistrelated
        //           SET
        //            SoThuTu = {stt} 
        //           WHERE
        //               id = {id}";
        //               var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
        //           }
        //       }



        //       return Json(new
        //       {
        //       }, JsonRequestBehavior.AllowGet);
        //   }
        [HttpGet]
        public JsonResult LoadData()
        {
            var sql = $@"SELECT * FROM dbo.News ORDER BY  Id desc";
            var dtnews = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            return Json(new
            {
                data = dtnews
            }, JsonRequestBehavior.AllowGet);
        }
       // [Authorize(Roles = "HostAdmin")]
        public ActionResult Create(int catid = 0,int type = 0,bool isHistory = false)
        {
            if(catid > 0)
            {
                ViewBag.catid = catid;

            }
            var model = DanhMucChungServices.Instance.GetListDanhMucChung();


            var sql = $@"SELECT * FROM dbo.Tags WHERE  type = 1";
            var dtTag = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsTag = dtTag;
            ViewBag.userId = userId;
            ViewBag.Images = $"{Server.MapPath(@"\")}UserData\\{userId}\\Images";
     

            return View(model);
        }

        public JsonResult UpdateDatetime(string _datetime = "", int id = 0)
        {
            var stt = false;
            var msg = "";
            #region Update thời gian hiển thị
            DateTime publishDate = DateTime.Now;
            if(_datetime!="")
            {
                DateTime.TryParse(_datetime, out publishDate);
                if (publishDate == DateTime.Now)
                    DateTime.TryParseExact(_datetime, "MM/dd/yyyy HH:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out publishDate);

            }

            var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE News
                    SET 
                        PublishDate = '{publishDate.ToString("yyyy/MM/dd hh:mm tt")}',
                        Status = 2
                    WHERE
                        id = {id}", _cnn);
            DBLibs.ExecuteNonQuery($"Delete From NewsHistory WHERE NewsId = {id}", _cnn);
            if (eff > 0) stt = true;
            else
                msg = "Không cập nhật dữ liệu được!";
            #endregion
            return Json(new
            {
                status = stt,
                message = msg
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateTitle(string title = "", int id = 0)
        {
            var stt = false;
            var msg = "";
            #region Update Tiêu đề
            var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE News
                    SET 
                        Title = N'{title.Replace("'", "''").Trim()}'
                   
                    WHERE
                        id = {id}", _cnn);
            if (eff > 0) stt = true;
            else
                msg = "Không cập nhật dữ liệu được!";
            #endregion
            return Json(new
            {
                status = stt,
                message = msg
            });
        }
        public ActionResult Edit(int id = 0,bool isHistory = false)
        {
            var model = DanhMucChungServices.Instance.GetListDanhMucChung();
            try
            {
                if (id > 0)
                {
         

                    var sql = $@"SELECT id, title FROM News WHERE id <> {id} AND id NOT IN (SELECT related_news FROM News WHERE id = {id}) AND SoThuTu BETWEEN {CLibs.DatetimeToTimestampOrgin(DateTime.Now.AddMonths(-1))} AND {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}";
                    var dtDsBaiViet = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
                    ViewBag.DsBaiViet = dtDsBaiViet;

                    sql = $@"SELECT id, title FROM News WHERE id IN (SELECT related_news FROM News WHERE id = {id})";
                    var dtRelatedNews = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
                    ViewBag.DsBaiVietLienQuan = dtRelatedNews;

                    sql = $@"SELECT TOP 100 r.NewsId,r.RelatedId,r.SoThuTu,n.Title FROM dbo.Newslistrelated r JOIN dbo.News n ON r.RelatedId = n.Id WHERE r.NewsId = {id} ORDER BY r.SoThuTu";
                    var dtrelatedNews = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
                    ViewBag.DsLienQuan = dtrelatedNews;
                    var dtInfosTemp = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM NewsHistory WHERE NewsId = {id}", _cnn);
                    var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 n.*,d.TenDanhMuc, d.MaDanhMuc AS cate_slug FROM  News n JOIN dbo.DanhMucChung d ON n.CatesId = d.id WHERE n.id = {id}", _cnn);
                    #region Get item infos
                    if (isHistory)
                    {
                        if (dtInfosTemp != null && dtInfosTemp.Rows[0] != null)
                            ViewBag.Infos = dtInfosTemp.Rows[0];
                       
                    }
                    else
                    {
                         ViewBag.HasHistory = (dtInfosTemp != null && dtInfosTemp.Rows.Count > 0);
                        if (dtInfos != null && dtInfos.Rows.Count > 0)
                        ViewBag.Infos = dtInfos.Rows[0];
                    }
                       
                    var dtTag = DBLibs.GetDataBy_DataAdapter($@"SELECT  * FROM dbo.Tags t JOIN TagNews tn  ON t.id = tn.TagsId   WHERE tn.NewsId = {id}", _cnn);

                    ViewBag.listTag = dtTag;

                    #endregion

                    ViewBag.Images = $"{Server.MapPath(@"\")}UserData\\{userId}\\{id}";
                    ViewBag.FileAttack = $"{Server.MapPath(@"\")}UserData/attackfile/{id}/";
                    ViewBag.Cates = Request["_cates"];
                    ViewBag.userId = userId;
                    ViewBag.Id = id;
                }
                else
                {
                    return RedirectToAction("Create", "QuanLyTinTuc");
                }

            }
            catch (Exception ex)
            {

            }
            return View(model);
        }

        #region Process
        public ContentResult DanhSachTags()
        {
            var dtDsTags = DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM Tags", _cnn);
            var strJson = "[";
            for (int i = 0; i < dtDsTags.Rows.Count; i++)
            {
                strJson += "\"" + dtDsTags.Rows[i]["name"].ToString().Replace("\"", "").Trim() + "\",";
            }
            strJson = strJson.TrimEnd(',');
            strJson += "]";

            return new ContentResult { Content = strJson, ContentType = "application/json" };
        }
        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };

        [ValidateInput(false)]
        public JsonResult Save(
            int id = 0,
            string _title = "",
            string _slug = "",
            string _thumbnail = "",
            string _sapo = "",
            string _content = "",
            string _related = "",
            int _cates = 0,
           int _status = 1,
            string _tags = "",
            string _tags_slug = "",
            string _thutu = "0",
            string _meta_keywords = "",
            string _meta_description = "",
            int _typeFile = 1,
            string _publishDate ="")
        {
            var stt = false;
            var catesId = "0";
            var msg = "";
            var newid = "0";
            //var publishDate = DateTime.Parse(_publishDate);


            #region Phân tích request/submit form (nếu có)
            //DateTime publishDate = (_publishDate != null && _publishDate != "") ? DateTime.Parse(_publishDate, new CultureInfo("fr-FR")) :  DateTime.Now;
            DateTime publishDate = DateTime.Now;
            if (_publishDate != "")
            {
                DateTime.TryParse(_publishDate, out publishDate);
                if (publishDate == DateTime.Now)
                    DateTime.TryParseExact(_publishDate, "MM/dd/yyyy HH:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out publishDate);
            }

                if (_cates == 0)
                    msg = "Xin vui lòng chọn danh mục";
                else
                {
                    if (id < 1)
                    {
                        #region Thêm bài viết
                        var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM News WHERE Status = 2  slug = N'{_slug.Replace("'", "''").Trim()}' OR title = N'{_title.Replace("'", "''").Trim()}'", _cnn);
                        if (exited != null && int.Parse(exited.ToString()) > 0)
                        {
                            return Json(new
                            {
                                status = false,
                                message = "Đã có bài viết có cùng tiêu đề hoặc url!"
                            });
                        }
                        var eff = DBLibs.ExecuteScalar($@"
                    INSERT INTO dbo.News
                    (
                        Slug ,
                        UnitId ,
                        Title ,
                        Thumbnail ,
                        Summary ,
                        FullContent ,
                        related_news ,
                        CatesId ,
                        Tags ,
                        Status ,
                        CreatedAt ,
                        CreatedBy ,
                        meta_keywords ,
                        meta_descriptions,
                        SoThuTu,
                        PublishDate
                       
                    )
                    VALUES
                    (
                        N'{_slug.Replace("'", "''").Trim()}' , 
                        {SysBaseInfor.GetIdNguoiDung()} , 
                        N'{_title.Replace("'", "''").Trim()}' , 
                        N'{_thumbnail.Replace("'", "''").Trim()}' ,
                        N'{_sapo.Replace("'", "''").Trim()}' ,
                        N'{_content.Replace("'", "''").Trim()}' , 
                        N'{_related.Replace("'", "''").Trim()}' , 
                        {_cates} ,
                        N'{_tags.Replace("'", "''").Trim()}' , 
                        {_status} , 
                        {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} ,
                        N'{SysBaseInfor.GetCurrentUserId()}' , 
                        N'{_meta_keywords.Replace("'", "''").Trim()}' , 
                        N'{_meta_description.Replace("'", "''").Trim()}',
                         {_thutu} ,
                         '{publishDate.ToString("yyyy/MM/dd hh:mm tt")}'
                    )  SELECT SCOPE_IDENTITY() ", _cnn);
                        int.TryParse(newid == null ? "0" : newid.ToString(), out id);
                        if (eff != null && eff.ToString() != "" && int.Parse(eff.ToString()) > 0)
                        { stt = true; newid = eff.ToString(); }
                        else
                            msg = "Không thêm dữ liệu vào được!";
                        #endregion
                    }
                    else
                    {
                        #region Update bài viết
                        var dtNews = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 Slug FROM News WHERE Id = {id}", _cnn);
                        var slugNews = dtNews.Rows[0]["Slug"].ToString();
                        var dtdmId = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 MaDanhMuc FROM DanhMucChung WHERE Id = {_cates}", _cnn);
                        //var slugDM = dtdmId.Rows[0]["MaDanhMuc"].ToString();
                        //var oldUrl = ViELearn.BackEnd.Ultilities.Common.GetUrlFrontend() + "/" + slugDM + "/" + slugNews + ".html";
                        //var dtDm = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM DanhMucChung WHERE Url = N'{oldUrl.Replace("'", "''").Trim()}'", _cnn);
                        //if (dtDm != null && dtDm.Rows.Count > 0)
                        //{
                        //    var dmId = dtDm.Rows[0]["id"].MapInt();
                        //    var urlDm = dtDm.Rows[0]["Url"].ToString();
                        //    if (oldUrl == urlDm)
                        //    {
                        //        var newUrl = ViELearn.BackEnd.Ultilities.Common.GetUrlFrontend() + "/" + slugDM + "/" + _slug + ".html";
                        //        DBLibs.GetDataBy_DataAdapter($@"
                        //    UPDATE DanhMucChung 
                        //SET 
                        //    Url = N'{newUrl.Replace("'", "''").Trim()}'
                        //WHERE
                        //    id = {dmId} ", _cnn);

                        //    }
                        //}
                        // var Url = dtDm.Rows[0]["Url"].ToString();
                        //var dmId = dtDm.Rows[0]["Url"].MapInt();
                        string sql = $@"
                    UPDATE News 
                    SET 
                        Slug = N'{_slug.Replace("'", "''").Trim()}' ,
                        Title = N'{_title.Replace("'", "''").Trim()}' , 
                        Thumbnail = N'{_thumbnail.Replace("'", "''").Trim()}' , 
                        Summary = N'{_sapo.Replace("'", "''").Trim()}' , 
                        FullContent = N'{_content.Replace("'", "''").Trim()}' , 
                        related_news = N'{_related.Replace("'", "''").Trim()}' , 
                        CatesId = {_cates} ,
                        Tags = N'{_tags.Replace("'", "''").Trim()}' ,
                        Status = {_status} , 
                        UpdatedAt = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} ,
                        PublishAt = 0 , -- PublishAt - int
                        UpdatedBy = N'{SysBaseInfor.GetCurrentUserId()}' ,
                        meta_keywords = N'{_meta_keywords.Replace("'", "''").Trim()}' ,
                        meta_descriptions = N'{_meta_description.Replace("'", "''").Trim()}' ,
                        TypeFile = {_typeFile},
                        SoThuTu = {_thutu},
                        PublishDate = '{publishDate.ToString("yyyy/MM/dd hh:mm tt")}'
                    WHERE
                        id = {id}";
                        ViewBag.sql_update = sql;
                        var eff = DBLibs.ExecuteNonQuery(sql, _cnn);


                        if (eff > 0) { stt = true; newid = id.ToString(); }
                        else
                            msg = "Không cập nhật dữ liệu được!";

                        //msg = sql;
                        #endregion
                    }

                

                try
                {
                    #region Cache lại ảnh trong bài viết
                    var h = new WebLibs.HttpRequest();
                    _content = ProcessImgeFinder(id, _content);

                    DBLibs.ExecuteNonQuery($"UPDATE News SET FullContent = N'{_content.Replace("'", "''").Trim()}' WHERE id = {id}", _cnn);
                    #endregion
                }
                catch(Exception ex)
                { }
                  

                    #region Update/Insert tags
                    var tags = _tags.Split(',');
                    var tags_slug = _tags_slug.Split(',');

                    //var cates = (_cates == "0" ? "" : _cates).Split(',');
                    //var cates_slug = (_cates_slug == "0" ? "" : _cates_slug).Split(',');
                    DBLibs.ExecuteNonQuery($"Delete From TagNews WHERE NewsId = {id}", _cnn);

                    for (int i = 0; i < tags.Length; i++)
                    {
                        if (tags_slug[i].Trim(" -".ToCharArray()).ToLower() == "") continue;
                        var sql = $@"
                    BEGIN TRAN
                        UPDATE Tags WITH (SERIALIZABLE) SET [used] = ([used] + 1),[type]=1 WHERE slug = '{tags_slug[i].Replace("'", "''").Trim(" -".ToCharArray()).ToLower()}'

                        IF @@rowcount = 0
                        BEGIN
                            INSERT INTO Tags ([type],[name], [slug], created_at) VALUES (1,N'{tags[i].Replace("'", "''").Trim(" -".ToCharArray()).ToLower()}', '{tags_slug[i].Trim(" -".ToCharArray()).ToLower()}', {CLibs.DatetimeToTimestampOrgin(DateTime.Now)})
                        END
                       
                    COMMIT TRAN
                    ";

                        var eff = DBLibs.ExecuteNonQuery(sql, _cnn);

                        var tagid = DBLibs.ExecuteScalar($"SELECT TOP 1 Id FROM Tags WHERE slug = '{tags_slug[i].Trim(" - ".ToCharArray()).ToLower()}'", _cnn);


                        var qr = $@"
                        BEGIN TRAN
                  
                           BEGIN
                              INSERT INTO TagNews (NewsId,TagsId) VALUES ({id},{tagid})
                           END
                        COMMIT TRAN
                        ";

                        var eff1 = DBLibs.ExecuteNonQuery(qr, _cnn);

                    }

                    #endregion

                    #region Update / Insert NewsRelated
                    DBLibs.ExecuteNonQuery($"Delete From Newslistrelated WHERE NewsId = {id}", _cnn);
                    if (_related != "")
                    {
                        var newsIdlt = (id > 0) ? id : newid.MapInt();
                        string reId = _related.Substring(0, _related.LastIndexOf(","));
                        var relateId = reId.Trim().Split(',');

                        for (int i = 0; i < relateId.Length; i++)
                        {
                            if (relateId[i].Trim(" -".ToCharArray()).ToLower() == "") continue;
                            var sql = $@"
                    INSERT INTO Newslistrelated(NewsId, RelatedId,SoThuTu) VALUES({newsIdlt}, { relateId[i] },{i+1})";
                            DBLibs.ExecuteNonQuery(sql, _cnn);
                        }
                    }
                 //   DBLibs.ExecuteNonQuery($"Delete From NewsHistory WHERE NewsId = {id}", _cnn);
                    #endregion

                }
                #endregion
           
            return Json(new
            {
                catesId = catesId,
                newid = newid,
                status = stt,
                message = msg
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region handel
        public string ProcessContent(long Newsid, string _content)
        {
            foreach (Match m in Regex.Matches(_content, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
            {
                string src = m.Groups[1].Value.Trim();
                if (src.StartsWith("/UserData/")) continue;

                //if (!src.ToLower().StartsWith(Request.Url.Host.ToLower()))
                if (!ImageExtensions.Contains(Path.GetExtension(src).ToUpperInvariant()))
                {
                    var path = Server.MapPath("~") + $"UserData\\{SysBaseInfor.GetIdNguoiDung()}\\{Newsid}\\";
                    var fullpath = path + CLibs.GenPassCode(false, 12);
                    fullpath += (src.Contains(".jpg") || src.Contains(".jpeg") || src.Contains(".gif") || src.Contains(".png") ? src.Substring(src.LastIndexOf('.')).Trim() : ".jpg");
                    if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

                    var base64Data = Regex.Match(src.Replace("&amp;", "&"), @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    var binData = Convert.FromBase64String(base64Data);
                    using (var stream = new MemoryStream(binData))
                    {
                        Bitmap bitmap = new Bitmap(stream);
                        bitmap.Save(fullpath);
                    }

                    var dest = fullpath.Replace(Server.MapPath("~"), "\\").Replace("\\", "/");
                    _content = _content.Replace(src, dest);
                }
            }

            foreach (Match m in Regex.Matches(_content, "<input.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
            {
                string src = m.Groups[1].Value.Trim();
                if (src.StartsWith("/UserData/")) continue;

                if (!ImageExtensions.Contains(Path.GetExtension(src).ToUpperInvariant()))
                {
                    var path = Server.MapPath("~") + $"UserData\\{SysBaseInfor.GetIdNguoiDung()}\\{Newsid}\\";
                    var fullpath = path + CLibs.GenPassCode(false, 12);
                    fullpath += (src.Contains(".jpg") || src.Contains(".jpeg") || src.Contains(".gif") || src.Contains(".png") ? src.Substring(src.LastIndexOf('.')).Trim() : ".jpg");
                    if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

                    var base64Data = Regex.Match(src.Replace("&amp;", "&"), @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    var binData = Convert.FromBase64String(base64Data);
                    using (var stream = new MemoryStream(binData))
                    {
                        Bitmap bitmap = new Bitmap(stream);
                        bitmap.Save(fullpath);
                    }
                    var dest = fullpath.Replace(Server.MapPath("~"), "\\").Replace("\\", "/");
                    _content = _content.Replace(src, dest);
                }
            }
            foreach (Match m in Regex.Matches(_content, "<img.+?rel=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
            {
                string src = m.Groups[1].Value.Trim();
                if (src.StartsWith("/UserData/")) continue;

                if (!ImageExtensions.Contains(Path.GetExtension(src).ToUpperInvariant()))
                {
                    var path = Server.MapPath("~") + $"UserData/{SysBaseInfor.GetIdNguoiDung()}/{Newsid}/";
                    var fullpath = path + CLibs.GenPassCode(false, 12);
                    fullpath += (src.Contains(".jpg") || src.Contains(".jpeg") || src.Contains(".gif") || src.Contains(".png") ? src.Substring(src.LastIndexOf('.')).Trim() : ".jpg");
                    if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

                    var base64Data = Regex.Match(src.Replace("&amp;", "&"), @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    var binData = Convert.FromBase64String(base64Data);
                    using (var stream = new MemoryStream(binData))
                    {
                        Bitmap bitmap = new Bitmap(stream);
                        bitmap.Save(fullpath);
                    }
                    var dest = fullpath.Replace(Server.MapPath("~"), "\\").Replace("\\", "/");
                    _content = _content.Replace(src, dest);
                }
            }
            foreach (Match m in Regex.Matches(_content, "<img.+?data-original=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
            {
                string src = m.Groups[1].Value.Trim();
                if (src.StartsWith("/UserData/")) continue;

                //if (!src.ToLower().StartsWith(Request.Url.Host.ToLower()))
                if (!ImageExtensions.Contains(Path.GetExtension(src).ToUpperInvariant()))

                {
                    var path = Server.MapPath("~") + $"UserData/{SysBaseInfor.GetIdNguoiDung()}/{Newsid}/";
                    var fullpath = path + CLibs.GenPassCode(false, 12);
                    fullpath += (src.Contains(".jpg") || src.Contains(".jpeg") || src.Contains(".gif") || src.Contains(".png") ? src.Substring(src.LastIndexOf('.')).Trim() : ".jpg");
                    if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

                    var base64Data = Regex.Match(src.Replace("&amp;", "&"), @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    var binData = Convert.FromBase64String(base64Data);
                    using (var stream = new MemoryStream(binData))
                    {
                        Bitmap bitmap = new Bitmap(stream);
                        bitmap.Save(fullpath);
                    }
                    var dest = fullpath.Replace(Server.MapPath("~"), "\\").Replace("\\", "/");
                    _content = _content.Replace(src, dest);
                }
            }

            return _content;
        }

        public string ProcessImgeFinder(long Newsid, string _content)
        {
            foreach (Match m in Regex.Matches(_content, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
            {
                string src = m.Groups[1].Value.Trim();
                if (src.StartsWith("/UserData/")|| src.StartsWith("https")) continue;

                //if (!src.ToLower().StartsWith(Request.Url.Host.ToLower()))
                if (ImageExtensions.Contains(Path.GetExtension(src).ToUpperInvariant()))
                {
                    var path = Server.MapPath("~") + $"UserData\\{SysBaseInfor.GetIdNguoiDung()}\\{Newsid}\\";
                    var fullpath = path + CLibs.GenPassCode(false, 12);
                    fullpath += (src.Contains(".jpg") || src.Contains(".jpeg") || src.Contains(".gif") || src.Contains(".png") ? src.Substring(src.LastIndexOf('.')).Trim() : ".jpg");
                    if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

                    //var base64Data = Regex.Match(src.Replace("&amp;", "&"), @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    //var binData = Convert.FromBase64String(base64Data);
                    string newSrc = Server.MapPath("~") + src;
                    Bitmap bitmap = new Bitmap(newSrc);
                    bitmap.Save(fullpath);

                    var dest = fullpath.Replace(Server.MapPath("~"), "\\").Replace("\\", "/");
                    _content = _content.Replace(src, dest);
                }
            }
            return _content;
        }
        public JsonResult SaveThumb(int id = 0, string _thumbnail = "")
        {
            var stt = false;
            var msg = "";

            var sql = $"UPDATE News SET Thumbnail = N'{_thumbnail.Replace("'", "''").Trim()}' WHERE id = {id}";
            var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
            if (eff > 0) stt = true;
            else
                msg = "Không cập nhật dữ liệu được!";

            return Json(new
            {
                status = stt,
                message = msg
            });
        }
        //public ActionResult Delete(string id = "0")
        //{


        //    if (id != "" && id != "0" && int.Parse(id) > 0)
        //    {
        //        var sql = $@"DELETE dbo.News WHERE id = {id}";
        //        var eff = DBLibs.ExecuteNonQuery(sql, _cnn);

        //    }

        //    return RedirectToAction("Index");

        //}
        public JsonResult Delete(string id = "0")
        {
            var stt = false;
            var msg = "";

            if (id != "" && id != "0" && int.Parse(id) > 0)
            {
                var sql = $"DELETE dbo.News WHERE id = {id}";
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                if (eff > 0) stt = true;
                else
                    msg = "Không xóa dữ liệu được!";
            }
            else
            {
                msg = "ID rỗng hoặc = 0";
            }
            return Json(new
            {
                status = stt,
                message = msg
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateConfig(int id = 0, int _status = 0, int _show_athome = 0)
        {
            var stt = false;
            var msg = "";

            var sql = $"UPDATE News SET [Status] = {_status}, show_athome = {_show_athome} WHERE id = {id}";
            var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
            if (eff > 0) stt = true;
            else
                msg = "Không cập nhật dữ liệu được!";

            return Json(new
            {
                status = stt,
                message = msg
            });
        }
        public JsonResult CacheViewTemp(int id = 0, int _view_temp = 0)
        {
            var stt = false;
            var msg = "";

            var sql = $"UPDATE News SET view_total = ISNULL(view_total,0) + {_view_temp}, view_temp = 0, last_sync_time = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} WHERE id = {id}";
            var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
            if (eff > 0) stt = true;
            else
                msg = "Không cập nhật dữ liệu được!";

            return Json(new
            {
                status = stt,
                message = msg
            });
        }

      
        public ActionResult UploadFileAttack(int id = 0)
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            var msg = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    //fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        //var path = $"{Server.MapPath(@"\")}SyncFiles\\{SysBaseInfor.GetIDTruong()}\\Diem";
                        var path = Server.MapPath("~") + $"UserData/attackfile/{id}/";

                        var ext = Path.GetExtension(file.FileName);
                        fName = file.FileName;// "FileDiemChuanBiDongBo" + ext;
                        if (!System.IO.Directory.Exists(path))
                            System.IO.Directory.CreateDirectory(path);
                        if (System.IO.File.Exists(fName)) System.IO.File.Delete(fName);

                        var fullpath = string.Format("{0}\\{1}", path, fName);
                        file.SaveAs(fullpath);
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
                msg = ex.Message;
            }

            if (isSavedSuccessfully)
                return Json(new { Message = fName });
            else
                return Json(new { Message = msg });
        }
        public JsonResult DeleteFileAttack(int id = 0, string fName = "")
        {
            var msg = "";
            try
            {
                //Save file content goes here
                //fName = file.FileName;
                //var path = $"{Server.MapPath(@"\")}SyncFiles\\{SysBaseInfor.GetIDTruong()}\\Diem";
                //var path = Server.MapPath("~") + $"UserData/0/Images/";

                //var ext = Path.GetExtension(file.FileName);
                //fName = file.FileName;// "FileDiemChuanBiDongBo" + ext;
                var path = Server.MapPath("~") + $"UserData\\attackfile\\{id}\\";
                var fullpath = string.Format("{0}\\{1}", path, fName);
                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);

                }
            }
            catch (Exception ex)
            {

                msg = ex.Message;
            }

            return Json(new
            {
                message = msg
            }, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public ActionResult SaveFile(string name, string data, int id = 0)
        {
            string fullpath = "";
            //string BaseURL = "";
            //string path = "";
            //string matchString = "";
            try
            {
                byte[] imageByteArray = Convert.FromBase64String(data.Split(',').ToArray()[1]);
                MemoryStream ms = new MemoryStream(imageByteArray, 0, imageByteArray.Length);
                ms.Write(imageByteArray, 0, imageByteArray.Length);
                string FilePath = Server.MapPath("~") + $"UserData\\{userId}\\{id}\\";
                fullpath = string.Format("{0}\\{1}", FilePath, name);

                using (FileStream file = new FileStream(fullpath, FileMode.Create, FileAccess.Write))
                {
                    ms.WriteTo(file);
                }

            }
            catch (Exception ex)
            {
            }

            return Json(new
            {
                fileName = $"/UserData/{userId}/{id}/{name}",

            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UploadFile(int id = 0)
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            var msg = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    //fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        //var path = $"{Server.MapPath(@"\")}SyncFiles\\{SysBaseInfor.GetIDTruong()}\\Diem";
                        var path = Server.MapPath("~") + $"UserData/{userId}/{id}/";

                        var ext = Path.GetExtension(file.FileName);
                        fName = file.FileName;// "FileDiemChuanBiDongBo" + ext;
                        if (!System.IO.Directory.Exists(path))
                            System.IO.Directory.CreateDirectory(path);
                        if (System.IO.File.Exists(fName)) System.IO.File.Delete(fName);

                        var fullpath = string.Format("{0}\\{1}", path, fName);
                        file.SaveAs(fullpath);
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
                msg = ex.Message;
            }

            if (isSavedSuccessfully)
                return Json(new { Message = fName });
            else
                return Json(new { Message = msg });
        }
        public ActionResult UploadFile1()
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            var msg = "";
            var newid = 0;
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    //fName = file.FileName;
                    newid = DBLibs.ExecuteScalar("SELECT IDENT_CURRENT('News') + 1 AS tmp", _cnn).MapInt();
                    //newid = id.Rows[0]["Id"].MapInt();
                    if (newid > 0)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            //var path = $"{Server.MapPath(@"\")}SyncFiles\\{SysBaseInfor.GetIDTruong()}\\Diem";
                            var path = Server.MapPath("~") + $"UserData/{userId}/{newid}/";

                            var ext = Path.GetExtension(file.FileName);
                            fName = file.FileName;// "FileDiemChuanBiDongBo" + ext;
                            if (!System.IO.Directory.Exists(path))
                                System.IO.Directory.CreateDirectory(path);
                            if (System.IO.File.Exists(fName)) System.IO.File.Delete(fName);
                            var fullpath = string.Format("{0}\\{1}", path, fName);
                            file.SaveAs(fullpath);
                        }
                    }
             
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
                msg = ex.Message;
            }

            if (isSavedSuccessfully)
                return Json(new { Message = fName, id = newid  });
            else
                return Json(new { Message = msg });
        }
        public JsonResult CopyFile(string fName = "", int id = 0)
         {
            var msg = ""; var newPath = "";
            try
            {

                string fullPath = Server.MapPath("~") + fName;  //F:\Working\LacHongVien\LacHongVien_Back\ViELearn.BackEnd\/UserData/0/3/file.364366.jpg

                string oldPath = string.Format("/UserData/{0}/{1}/", userId, id); //UserData/0/3/file.364366.jpg
                newPath = oldPath.Replace(oldPath, "/UserData/Share/Images/"); ///UserData/Share/Images/file.364366.jpg 

                fName = fName.Replace(fName, string.Format("{0}{1}{2}{3}_{4}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Millisecond,
                    Path.GetFileNameWithoutExtension(newPath) + Path.GetExtension(fName)));  ///UserData/Share/Images/2020417523_file.364366.jpg
                var folder = Server.MapPath("~") + newPath;
                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);
                string fullDes = folder + fName;

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Copy(fullPath, fullDes);
                }
            }
            catch (Exception ex)
            {

                msg = ex.Message;
            }

            return Json(new
            {
                newpath = newPath +fName,
                message = msg
            }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult CopyFile1(string fName = "", int id = 0)
        {

            var msg = ""; var newPath = "";
            try
            {
                string fullPath = Server.MapPath("~") + fName;
                string oldPath = string.Format("UserData/{0}/", userId);
                newPath = fName.Replace(oldPath, "UserData/Share/");

                newPath = newPath.Replace(Path.GetFileName(newPath), string.Format("{0}{1}{2}{3}_{4}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Millisecond,
                              Path.GetFileNameWithoutExtension(newPath) + Path.GetExtension(newPath)));
                string fullDes = Server.MapPath("~") + newPath;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Copy(fullPath, fullDes);
                    //System.IO.Directory.CreateDirectory(fullDes);
                }

            }
            catch (Exception ex)
            {

                msg = ex.Message;
            }

            return Json(new
            {
                newpath = newPath,
                message = msg
            }, JsonRequestBehavior.AllowGet);


        }
        #endregion

        public JsonResult DeleteFile(string fName = "")
        {
            var msg = "";
            try
            {
                string fullPath = Server.MapPath("~") + fName;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return Json(new
            {
                message = msg
            }, JsonRequestBehavior.AllowGet);


        }

        #region Modify
        public JsonResult ChangeStatus(int id = 0, int _tophot = 0)
        {
            var status = false;
            var message = "";
            if (_tophot > 1)
            {
                message = "Show chỉ có thể là 0 hoặc 1";
                return Json(new
                {
                    status,
                    message
                }, JsonRequestBehavior.AllowGet);
            }



            if (id > 0)
            {
                var sql = $@"UPDATE dbo.News SET top_hot = {_tophot} WHERE id = {id}";
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                if (eff > 0) status = true;
            }

            return Json(new
            {
                status,
                message
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult Doitrangthai(int id = 0)
        {
            var status = false;
            var message = "";




            if (id > 0)
            {
                var sql = $@"UPDATE dbo.News SET Status = 1 WHERE id = {id}";
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                if (eff > 0) status = true;
            }

            return Json(new
            {
                status,
                message
            }, JsonRequestBehavior.AllowGet);

        }
        [ValidateInput(false)]
        public JsonResult SaveSort(string values)
        {
            values = values.TrimEnd(';');
            string[] arrId = values.Split(';');
            foreach (var item in arrId)
            {
                int id = Convert.ToInt32(item.Split(':')[0]);
                int stt = Convert.ToInt32(item.Split(':')[1]);
                if (id > 0)
                {
                    var sql = $@"
                UPDATE dbo.News
                SET
	                SoThuTu = {stt} 
                WHERE
                    id = {id}";
                    var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                }
            }



            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateSoThuTu(int id = -1000000, int stt = -1000000)
        {
            var status = false;
            var message = "";

            if (id == -1000000) message = "Thiếu dữ liệu truyền vào";
            else if (stt == -1000000) message = "Số thứ tự truyền vào chưa đúng";
            else status = true;

            if (status)
            {
                var sql = $@"UPDATE dbo.News SET SoThuTu = {stt} WHERE id = {id}";
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                if (eff > 0) status = true;
                else { status = false; message = $"Không cập nhật được số thứ tự '{stt}' cho bài viết có id '{id}'"; }
            }

            return Json(new
            {
                status,
                message
            }, JsonRequestBehavior.AllowGet);
        }
        private int ConvertToInt(string value)
        {
            int result = 0;
            decimal d1 = Convert.ToDecimal(value);
            result = Convert.ToInt32(Math.Round(d1, 0));
            return result;
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveImageCrop(int id =0,string path="", string x="", string y="", string w="", string h="")
        {
            try
            {
                int x1 = ConvertToInt(x); int y1 = ConvertToInt(y);int w1 = ConvertToInt(w);int h1 = ConvertToInt(h);
                string newPathCrop = string.Empty;
                string fullPath = Server.MapPath("~") + path;
                byte[] CropImage = Crop(fullPath, w1, h1, x1, y1);
                using (MemoryStream ms = new MemoryStream(CropImage, 0, CropImage.Length))

                {
                    ms.Write(CropImage, 0, CropImage.Length);
                    var ext = Path.GetFileNameWithoutExtension(fullPath) + Path.GetExtension(fullPath);
                    ext = ext.Replace(Path.GetFileName(ext), string.Format("{0}{1}{2}{3}_{4}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Millisecond,
                   Path.GetFileNameWithoutExtension(ext) + Path.GetExtension(ext)));
                    string SaveTo = string.Empty;
                    if (id > 0)
                    {
                         SaveTo = $"/UserData/{userId}/{id}/";
                    }
                    else
                    {
                         SaveTo = $"/UserData/{userId}/Images/";
                    }
                    newPathCrop = SaveTo+ ext;
                    SaveTo = Server.MapPath("~") + SaveTo + ext;
                    
                    using (Image image = Image.FromStream(ms))
                    {
                        Bitmap map = new Bitmap(image);
                        map.Save(SaveTo);
                    }
                }
                return Json(new { path = newPathCrop });
            }
            catch (Exception)
            {

                return Json("Chưa cắt được ảnh");
            }
           
        }
        static byte[] Crop(string Img, int Width, int Height, int X, int Y)
        {
            try
            {
                using (SD.Image OriginalImage = SD.Image.FromFile(Img))
                {
                    using (SD.Bitmap bmp = new SD.Bitmap(Width, Height))
                    {
                        bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
                        using (SD.Graphics Graphic = SD.Graphics.FromImage(bmp))
                        {
                            Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                            Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            Graphic.DrawImage(OriginalImage, new SD.Rectangle(0, 0, Width, Height), X, Y, Width, Height, SD.GraphicsUnit.Pixel);
                            MemoryStream ms = new MemoryStream();
                            bmp.Save(ms, OriginalImage.RawFormat);
                            return ms.GetBuffer();
                        }
                    }
                }
            }

            catch (Exception Ex)
            {
                throw (Ex);
            }

        }
        #endregion
        [HttpPost]
        public ActionResult Crop(string src = "")
        {
            ViewBag.Src = src;
            return PartialView("Crop");
        }
        public ActionResult AddForm()
        {
            return PartialView();
        }
        public ActionResult TempIndex()
        {
            return View();
        }
        public ActionResult AddMucluc()
        {
            return PartialView();
        }
            
    }
}