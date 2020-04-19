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

namespace ViELearn.BackEnd.Controllers
{
    public class QuanLyTrangChuController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            var sql = $@"SELECT TOP 8 n.*, t.slug cates_slug FROM dbo.News n LEFT OUTER JOIN Tags t ON n.Cates = t.name WHERE n.Status = 1 AND n.show_athome = 1 ORDER BY n.PublishAt DESC, n.UpdatedAt DESC, n.CreatedAt DESC, n.id DESC";
            var dtBaiVietTop = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            var dtBvTop1 = dtBaiVietTop.Copy();
            var dtBvTop2 = dtBaiVietTop.Copy();
            //if (dtBaiVietTop != null && dtBaiVietTop.Rows.Count > 3)
            //{
                for (int i = 0; i < dtBaiVietTop.Rows.Count; i++)
                {
                    if (i >= dtBaiVietTop.Rows.Count - 4)
                        dtBvTop2.ImportRow(dtBaiVietTop.Rows[i]);
                    else
                        dtBvTop1.ImportRow(dtBaiVietTop.Rows[i]);
                }
                ViewBag.DsBaiVietTop1 = dtBvTop1;
                ViewBag.DsBaiVietTop2 = dtBvTop2; // Chỉ có 3 bài viết
            //}
            //else
            //{

            //}

            //sql = $@"SELECT * FROM dbo.Tags";
            //var dtTags = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            //ViewBag.DsTags = dtTags;

            return View();
        }
        [ValidateInput(false)]
        public JsonResult Save(
            int id = 0,
            int status = 0,
            string _title = "",
            string _slug = "",
            string _thumbnail = "",
            string _sapo = "",
            string _content = "",
            string _related = "",
            string _cates = "",
            string _cates_slug = "",
            string _tags = "",
            string _tags_slug = "",
            string _meta_keywords = "",
            string _meta_description = "")
        {
            var stt = false;
            var msg = "";
            #region Phân tích request/submit form (nếu có)
            if (_title == null || _title.Trim() == "")
                msg = "Tên gian hàng không được để trống";
            if (_slug == null || _slug.Trim() == "")
                msg = "Slug không được để trống";
            else
            {
                if (id < 1)
                {
                    #region Thêm bài viết
                    var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM News WHERE slug = N'{_slug.Replace("'", "''").Trim()}' OR title = N'{_title.Replace("'", "''").Trim()}'", _cnn);
                    if (exited != null && int.Parse(exited.ToString()) > 0)
                    {
                        return Json(new
                        {
                            status = false,
                            message = "Đã có bài viết có cùng tiêu đề hoặc url!"
                        });
                    }
                    var newid = DBLibs.ExecuteScalar($@"
                    INSERT INTO dbo.News
                    (
                        Slug ,
                        UnitId ,
                        Title ,
                        Thumbnail ,
                        Summary ,
                        FullContent ,
                        related_news ,
                        Cates ,
                        Tags ,
                        Status ,
                        CreatedAt ,
                        CreatedBy ,
                        PublishAt,
                        meta_keywords ,
                        meta_descriptions
                    )
                    VALUES
                    (
                        '{_slug.Replace("'", "''").Trim()}' , -- Slug - varchar(250)
                        {SysBaseInfor.GetCurrentUnitId()} , -- UnitId - int
                        N'{_title.Replace("'", "''").Trim()}' , -- Title - nvarchar(200)
                        N'{_thumbnail.Replace("'", "''").Trim()}' , -- Thumbnail - nvarchar(500)
                        N'{_sapo.Replace("'", "''").Trim()}' , -- Summary - nvarchar(1000)
                        N'{_content.Replace("'", "''").Trim()}' , -- FullContent - ntext
                        N'{_related.Replace("'", "''").Trim()}' , -- related_news - nvarchar(45)
                        N'{_cates.Replace("'", "''").Trim()}' , -- Cates - nvarchar(1000)
                        N'{_tags.Replace("'", "''").Trim()}' , -- Tags - nvarchar(1000)
                        {status} , -- Status - tinyint
                        {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , -- CreatedAt - int
                        {(status == 1 ? CLibs.DatetimeToTimestampOrgin(DateTime.Now) : 0)} , -- PublishAt - int
                        {SysBaseInfor.GetIdNguoiDung()} , -- CreatedBy - int
                        N'{_meta_keywords.Replace("'", "''").Trim()}' , -- meta_keywords - nvarchar(50)
                        N'{_meta_description.Replace("'", "''").Trim()}'  -- meta_descriptions - nvarchar(550)
                    )  SELECT SCOPE_IDENTITY() ", _cnn);
                    int.TryParse(newid == null ? "0" : newid.ToString(), out id);
                    if (newid != null) stt = true;
                    else
                        msg = "Không thêm dữ liệu vào được!";
                    #endregion
                }
                else
                {
                    #region Update bài viết
                    var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM News WHERE id <> {id} AND (slug = N'{_slug.Replace("'", "''").Trim()}' OR title = N'{_title.Replace("'", "''").Trim()}')", _cnn);
                    if (exited != null && int.Parse(exited.ToString()) > 0)
                    {
                        return Json(new
                        {
                            status = false,
                            message = "Đã có bài viết có cùng tiêu đề hoặc url!"
                        });
                    }
                    var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE News 
                    SET 
                        Slug = '{_slug.Replace("'", "''").Trim()}' , -- Slug - varchar(250)
                        Title = N'{_title.Replace("'", "''").Trim()}' , -- Title - nvarchar(200)
                        Thumbnail = N'{_thumbnail.Replace("'", "''").Trim()}' , -- Thumbnail - nvarchar(500)
                        Summary = N'{_sapo.Replace("'", "''").Trim()}' , -- Summary - nvarchar(1000)
                        FullContent = N'{_content.Replace("'", "''").Trim()}' , -- FullContent - ntext
                        related_news = N'{_related.Replace("'", "''").Trim()}' , -- related_news - nvarchar(45)
                        Cates = N'{_cates.Replace("'", "''").Trim()}' , -- Cates - nvarchar(1000)
                        Tags = N'{_tags.Replace("'", "''").Trim()}' , -- Tags - nvarchar(1000)
                        Status = {status} , -- Status - tinyint
                        UpdatedAt = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , -- UpdatedAt - int
                        PublishAt = {(status == 1 ? CLibs.DatetimeToTimestampOrgin(DateTime.Now) : 0)} , -- PublishAt - int
                        UpdatedBy = {SysBaseInfor.GetIdNguoiDung()} , -- UpdatedBy - int
                        meta_keywords = N'{_meta_keywords.Replace("'", "''").Trim()}' , -- meta_keywords - nvarchar(50)
                        meta_descriptions = N'{_meta_description.Replace("'", "''").Trim()}'  -- meta_descriptions - nvarchar(550)
                    WHERE
                        id = {id}", _cnn);
                    if (eff > 0) stt = true;
                    else
                        msg = "Không cập nhật dữ liệu được!";
                    #endregion
                }

                #region Cache lại ảnh trong bài viết
                var h = new WebLibs.HttpRequest();
                foreach (Match m in Regex.Matches(_content, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    string src = m.Groups[1].Value.Trim();
                    if (src.StartsWith("/UserData/")) continue;

                    if (!src.ToLower().StartsWith(Request.Url.Host.ToLower()))
                    {
                        var path = Server.MapPath("~") + $"UserData/{SysBaseInfor.GetIdNguoiDung()}/{id}/";
                        var fullpath = path + CLibs.GenPassCode(false, 12);
                        fullpath += (src.Contains(".jpg") || src.Contains(".jpeg") || src.Contains(".gif") || src.Contains(".png") ? src.Substring(src.LastIndexOf('.')).Trim() : ".jpg");
                        if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
                        h.DownloadFileFullToDisk(src.Replace("&amp;", "&"), fullpath);
                        var dest = fullpath.Replace(Server.MapPath("~"), "/");
                        _content = _content.Replace(src, dest);
                    }
                }
                foreach (Match m in Regex.Matches(_content, "<img.+?rel=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    string src = m.Groups[1].Value.Trim();
                    if (src.StartsWith("/UserData/")) continue;

                    if (!src.ToLower().StartsWith(Request.Url.Host.ToLower()))
                    {
                        var path = Server.MapPath("~") + $"UserData/{SysBaseInfor.GetIdNguoiDung()}/{id}/";
                        var fullpath = path + CLibs.GenPassCode(false, 12);
                        fullpath += (src.Contains(".jpg") || src.Contains(".jpeg") || src.Contains(".gif") || src.Contains(".png") ? src.Substring(src.LastIndexOf('.')).Trim() : ".jpg");
                        if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
                        h.DownloadFileFullToDisk(src.Replace("&amp;", "&"), fullpath);
                        var dest = fullpath.Replace(Server.MapPath("~"), "/");
                        _content = _content.Replace(src, dest);
                    }
                }
                foreach (Match m in Regex.Matches(_content, "<img.+?data-original=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    string src = m.Groups[1].Value.Trim();
                    if (src.StartsWith("/UserData/")) continue;

                    if (!src.ToLower().StartsWith(Request.Url.Host.ToLower()))
                    {
                        var path = Server.MapPath("~") + $"UserData/{SysBaseInfor.GetIdNguoiDung()}/{id}/";
                        var fullpath = path + CLibs.GenPassCode(false, 12);
                        fullpath += (src.Contains(".jpg") || src.Contains(".jpeg") || src.Contains(".gif") || src.Contains(".png") ? src.Substring(src.LastIndexOf('.')).Trim() : ".jpg");
                        if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
                        h.DownloadFileFullToDisk(src.Replace("&amp;", "&"), fullpath);
                        var dest = fullpath.Replace(Server.MapPath("~"), "/");
                        _content = _content.Replace(src, dest);
                    }
                }
                DBLibs.ExecuteNonQuery($"UPDATE News SET FullContent = N'{_content.Replace("'", "''").Trim()}' WHERE id = {id}", _cnn);
                #endregion

                #region Update/Insert tags
                var tags = _tags.Split(',');
                var tags_slug = _tags_slug.Split(',');
                var cates = (_cates == "0" ? "" : _cates).Split(',');
                var cates_slug = (_cates_slug == "0" ? "" : _cates_slug).Split(',');
                for (int i = 0; i < tags.Length; i++)
                {
                    if (tags_slug[i].Trim(" -".ToCharArray()).ToLower() == "") continue;
                    DBLibs.ExecuteNonQuery($@"
                    BEGIN TRAN
                       UPDATE Tags WITH (SERIALIZABLE) SET [used] = ([used] + 1) WHERE slug = '{tags_slug[i].Trim(" -".ToCharArray()).ToLower()}'

                       IF @@rowcount = 0
                       BEGIN
                          INSERT INTO Tags ([name], [slug], created_at) VALUES (N'{tags[i].Trim(" -".ToCharArray()).ToLower()}', '{tags_slug[i].Trim(" -".ToCharArray()).ToLower()}', {CLibs.DatetimeToTimestampOrgin(DateTime.Now)})
                       END
                    COMMIT TRAN
                    ", _cnn);
                }

                for (int i = 0; i < cates.Length; i++)
                {
                    if (cates_slug[i].Trim(" -".ToCharArray()).ToLower() == "") continue;
                    DBLibs.ExecuteNonQuery($@"
                    BEGIN TRAN
                       UPDATE Tags WITH (SERIALIZABLE) SET [used] = ([used] + 1) WHERE slug = '{cates_slug[i].Trim(" -".ToCharArray()).ToLower()}'

                       IF @@rowcount = 0
                       BEGIN
                          INSERT INTO Tags ([name], [slug], created_at) VALUES (N'{cates[i].Trim(" -".ToCharArray()).ToLower()}', '{cates_slug[i].Trim(" -".ToCharArray()).ToLower()}', {CLibs.DatetimeToTimestampOrgin(DateTime.Now)})
                       END
                    COMMIT TRAN
                    ", _cnn);
                }
                #endregion
            }
            #endregion

            return Json(new
            {
                status = stt,
                message = msg
            });
        }

    }
}