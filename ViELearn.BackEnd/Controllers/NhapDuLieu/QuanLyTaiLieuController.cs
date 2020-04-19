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
using ViELearn.BackEnd.App_Code;

namespace ViELearn.BackEnd.Controllers
{
    public class QuanLyTaiLieuController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            var sql = $@"SELECT * FROM dbo.TaiLieu ORDER BY PublishedAt DESC, UpdatedAt DESC, CreatedAt DESC, id DESC";
            var dtBaiViet = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsBaiViet = dtBaiViet;

            sql = $@"SELECT * FROM dbo.Tags";
            var dtTags = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsTags = dtTags;

            return View();
        }
        public ActionResult Create(int type = 0)
        {
            var dtDsTags = DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM Tags", _cnn);
            ViewBag.DsTags = dtDsTags != null ? dtDsTags : new DataTable();

            return View();
        }
        public ActionResult Edit(int id)
        {
            var sql = $@"SELECT * FROM dbo.Tags";
            var dtTags = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsTags = dtTags;

            sql = $@"SELECT id, title FROM TaiLieu WHERE id <> {id} AND id NOT IN (SELECT related_news FROM TaiLieu WHERE id = {id}) AND PublishAt BETWEEN {CLibs.DatetimeToTimestampOrgin(DateTime.Now.AddMonths(-1))} AND {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}";
            var dtDsBaiViet = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsBaiViet = dtDsBaiViet;

            sql = $@"SELECT id, title FROM TaiLieu WHERE id IN (SELECT related_news FROM TaiLieu WHERE id = {id})";
            var dtRelatedNews = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsBaiVietLienQuan = dtRelatedNews;

            #region Get item infos
            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM TaiLieu WHERE id = {id}", _cnn);
            ViewBag.Infos = dtInfos.Rows[0];
            #endregion

            ViewBag.Images = $"{Server.MapPath(@"\")}UserData\\{SysBaseInfor.GetIdNguoiDung()}\\{id}";
            ViewBag.Files = $"{Server.MapPath(@"\")}TaiLieu\\{id}";
            ViewBag.Cates = Request["_cates"];
            ViewBag.Id = id;
            return View();
        }
        public JsonResult Delete(int id)
        {
            var stt = false;
            var msg = "";

            #region Kiểm tra quyền được xóa (chỉ admin & người tạo mới xóa được)
            if (
                SysBaseInfor.GetCurrentUserName().StartsWith("admin@") ||
                int.Parse(DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM TaiLieu WHERE id = {id} AND CreatedBy = '{SysBaseInfor.GetCurrentUserId()}'", _cnn).ToString()) > 0)
            {
                #region Xóa các file ảnh
                try
                {
                    var path_imgs = Server.MapPath("~") + $"UserData/{SysBaseInfor.GetIdNguoiDung()}/{id}/";
                    var di = new DirectoryInfo(path_imgs);
                    foreach (FileInfo file in di.GetFiles()) file.Delete();
                    foreach (DirectoryInfo dir in di.GetDirectories()) dir.Delete(true);
                }
                catch { }
                #endregion

                #region Xóa các file tài liệu
                try
                {
                    var path_docs = Server.MapPath("~") + $"TaiLieu/{id}/";
                    var di = new DirectoryInfo(path_docs);
                    foreach (FileInfo file in di.GetFiles()) file.Delete();
                    foreach (DirectoryInfo dir in di.GetDirectories()) dir.Delete(true);
                }
                catch { }
                #endregion

                var eff = DBLibs.ExecuteNonQuery($"DELETE TaiLieu WHERE id = {id}", _cnn);
                if (eff < 1)
                    msg = "Lỗi không xác định, không xóa được tài liệu";
                else
                    stt = true;
            }
            else
                msg = "Không có quyền xóa tài liệu";
            #endregion

            return Json(new
            {
                status = stt,
                message = msg
            }, JsonRequestBehavior.AllowGet);
        }
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

        [ValidateInput(false)]
        public JsonResult Save(
            int id = 0,             int _status = 0,            int _show_athome = 0,       int _price = 0,         int _price_src = 0,     int _chapter = 0,           int _page = 0,                      int _view_temp = 0,
            string _title = "",     string _author = "",        string _slug = "",          string _sapo = "",      string _content = "",
            string _cates = "",     string _cates_slug = "",    string _tags = "",          string _tags_slug = "", string _related = "",   string _meta_keywords = "", string _meta_description = "",
            int _cate_id = 0,
            string _thumbnail = "", string _preview_urls = "",  string _loai_file = "",     int _type = 0,          int _top_level = 0,     int _order = 0,             int _hot_kind = 0,                  int _luot_mua = 0
            )
        {
            var stt = false;
            var msg = "";

            #region Phân tích request/submit form (nếu có)
            if (_title == null || _title.Trim() == "")
                msg = "Tên không được để trống";
            if (_slug == null || _slug.Trim() == "")
                msg = "Slug không được để trống";
            else
            {
                #region Update/Insert tags
                var tags = _tags.Trim().Split(',');
                var tags_slug = new string[tags.Length];// _tags.ToLower().Trim().Split(',');
                for (int i = 0; i < tags.Length; i++)
                    tags_slug[i] = PLibs.GenerateSlug(tags[i].ToLower());
                
                var cates = (_cates.Trim() == "0" || _cates.Trim() == "" ? "" : _cates.Trim()).Split(',');
                //var cates_slug = (_cates_slug.Trim() == "0" || _cates_slug.Trim() == "" ? "" : _cates_slug.ToLower().Trim()).Split(',');
                var cates_slug = new string[cates.Length];// _tags.ToLower().Trim().Split(',');
                for (int i = 0; i < cates.Length; i++)
                    cates_slug[i] = PLibs.GenerateSlug(cates[i].ToLower());

                for (int i = 0; i < tags.Length; i++)
                {
                    var t_slug = tags_slug[i].Trim(" -".ToCharArray()).ToLower().Trim();
                    if (t_slug == "") continue;
                    DBLibs.ExecuteNonQuery($@"
                    BEGIN TRAN
                       UPDATE Tags WITH (SERIALIZABLE) SET [used] = ([used] + 1) WHERE slug = '{t_slug}'

                       IF @@rowcount = 0
                       BEGIN
                          INSERT INTO Tags ([name], [slug], created_at) VALUES (N'{tags[i].Trim(" -".ToCharArray()).Trim()}', '{t_slug}', {CLibs.DatetimeToTimestampOrgin(DateTime.Now)})
                       END
                    COMMIT TRAN
                    ", _cnn);
                }

                for (int i = 0; i < cates.Length; i++)
                {
                    var c_slug = cates_slug[i].Trim(" -".ToCharArray()).ToLower().Trim();
                    if (c_slug == "") continue;
                    DBLibs.ExecuteNonQuery($@"
                    BEGIN TRAN
                       UPDATE Tags WITH (SERIALIZABLE) SET [used] = ([used] + 1) WHERE slug = '{c_slug}'

                       IF @@rowcount = 0
                       BEGIN
                          INSERT INTO Tags ([name], [slug], created_at) VALUES (N'{cates[i].Trim(" -".ToCharArray()).Trim()}', '{c_slug}', {CLibs.DatetimeToTimestampOrgin(DateTime.Now)})
                       END
                    COMMIT TRAN
                    ", _cnn);
                }
                #endregion

                //#region Tính toán lại cateid
                //if (_cate_id == 0 && _cates_slug.Length > 0)
                //{
                //    var sql = $@"SELECT id FROM Tags WHERE slug = '{cates_slug[0].Trim()}'";
                //    var tmp = DBLibs.ExecuteScalar(sql, _cnn);
                //    if (tmp != null)
                //        int.TryParse(tmp.ToString(), out _cate_id);
                //}
                //#endregion

                if (id < 1)
                {
                    #region Thêm tài liệu
                    var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM TaiLieu WHERE slug = N'{_slug.Replace("'", "''").Trim()}' OR Title = N'{_title.Replace("'", "''").Trim()}'", _cnn);
                    if (exited != null && int.Parse(exited.ToString()) > 0)
                    {
                        return Json(new
                        {
                            status = false,
                            message = "Đã có tài liệu có cùng tiêu đề hoặc url!"
                        });
                    }
                    var sql = $@"
                    INSERT INTO dbo.TaiLieu
                    (
	                    Slug ,			    UnitId ,			    TacGia ,			Title ,
	                    Thumbnail ,		    Summary ,				FullContent ,		Tags ,				Price ,
	                    Price_Src ,		    Price_Sale_Percent ,	[Status] ,          CreatedAt ,			UpdatedAt ,
                        CreatedBy ,			UpdatedBy ,             show_athome ,	    SoChapter ,		    SoPage ,
                        meta_keywords ,	    meta_descriptions ,     view_temp
                    ) VALUES (
                        '{_slug.Replace("'", "''")}' , -- Slug - varchar(250)
	                    {SysBaseInfor.GetCurrentUnitId()} , -- UnitId - int
	                    N'{_author.Replace("'", "''")}' , -- TacGia - nvarchar(500)
	                    N'{_title.Replace("'", "''")}' , -- Title - nvarchar(200)
	                    N'' , -- Thumbnail - nvarchar(500)
	                    N'{_sapo.Replace("'", "''")}' , -- Summary - nvarchar(1000)
	                    N'{_content.Replace("'", "''")}' , -- FullContent - ntext
	                    N'{_tags.Replace("'", "''")}' , -- Tags - nvarchar(1000)
	                    {_price} , -- Price - bigint
	                    {_price_src} , -- Price_Src - bigint
	                    {(_price == 0 ? 0 : (_price_src * 100 / _price))} , -- Price_Sale_Percent - tinyint
	                    {_status} , -- Status - tinyint
	                    GETDATE() , -- CreatedAt - datetime
	                    GETDATE() , -- UpdatedAt - datetime
	                    N'{(string.IsNullOrEmpty(SysBaseInfor.GetCurrentUserId()) ? "0" : SysBaseInfor.GetCurrentUserId())}' , -- CreatedBy - nvarchar(128)
	                    N'{(string.IsNullOrEmpty(SysBaseInfor.GetCurrentUserId()) ? "0" : SysBaseInfor.GetCurrentUserId())}' , -- UpdatedBy - nvarchar(128)
	                    {(_show_athome > 0 ? 1 : 0)} , -- show_athome - bit
	                    {_chapter} , -- SoChapter - smallint
	                    {_page} , -- SoPage - smallint
	                    N'{_meta_keywords.Replace("'", "''")}' , -- meta_keywords - nvarchar(250)
	                    N'{_meta_description.Replace("'", "''")}' , -- meta_descriptions - nvarchar(250)
	                    {_view_temp} -- view_temp - int
                    )  SELECT SCOPE_IDENTITY() ";
                    var newid = DBLibs.ExecuteScalar(sql, _cnn);
                    int.TryParse(newid == null ? "0" : newid.ToString(), out id);
                    if (newid != null) stt = true;
                    else
                        msg = "Không thêm dữ liệu vào được!";
                    #endregion
                }
                else
                {
                    #region Update tài liệu
                    var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM TaiLieu WHERE id <> {id} AND (slug = N'{_slug.Replace("'", "''").Trim()}' OR title = N'{_title.Replace("'", "''").Trim()}')", _cnn);
                    if (exited != null && int.Parse(exited.ToString()) > 0)
                    {
                        return Json(new
                        {
                            status = false,
                            message = "Đã có tài liệu có cùng tiêu đề hoặc url!"
                        });
                    }
                    var sql = $@"
                    UPDATE TaiLieu 
                    SET 
                        Slug = '{_slug.Replace("'", "''")}' , -- Slug - varchar(250)
	                    --CategoryID = {_cate_id} , -- CategoryID - int
	                    TacGia = N'{_author.Trim().Replace("'", "''")}' , -- TacGia - nvarchar(500)
	                    Title = N'{_title.Trim().Replace("'", "''")}' , -- Title - nvarchar(200)
	                    Thumbnail = N'{_thumbnail.Trim().Replace("'", "''")}' , -- Thumbnail - nvarchar(500)
	                    Summary = N'{_sapo.Trim().Replace("'", "''")}' , -- Summary - nvarchar(1000)
	                    FullContent = N'{_content.Trim().Replace("'", "''")}' , -- FullContent - ntext
	                    Tags = N'{_tags.Trim().Replace("'", "''")}' , -- Tags - nvarchar(1000)
	                    Price = {_price} , -- Price - bigint
	                    Price_Src = {_price_src} , -- Price_Src - bigint
	                    Price_Sale_Percent = {(_price == 0 ? 0 : (_price_src * 100 / _price))} , -- Price_Sale_Percent - tinyint
	                    PreviewLinks = N'{_preview_urls.Trim().Replace("'", "''")}' , -- PreviewLinks - nvarchar(max)
	                    Type = {_type} , -- Type - tinyint
	                    Status = {_status} , -- Status - tinyint
	                    UpdatedAt = GETDATE() , -- UpdatedAt - datetime
	                    UpdatedBy = N'{(string.IsNullOrEmpty(SysBaseInfor.GetCurrentUserId()) ? "0" : SysBaseInfor.GetCurrentUserId())}' , -- UpdatedBy - nvarchar(128)
	                    show_athome = {(_show_athome > 0 ? 1 : 0)} , -- show_athome - bit
	                    TopLevel = {_top_level} , -- TopLevel - tinyint
	                    --Order = {_order} , -- Order - int
	                    HotKind = {_hot_kind} , -- HotKind - tinyint
	                    SoChapter = {_chapter} , -- SoChapter - smallint
	                    SoPage = {_page} , -- SoPage - smallint
	                    LuotMua = {_luot_mua} , -- LuotMua - int
	                    LoaiFile = '{_loai_file.Replace("'", "''")}' , -- LoaiFile - varchar(4)
	                    meta_keywords = N'{_meta_keywords.Replace("'", "''")}' , -- meta_keywords - nvarchar(250)
	                    meta_descriptions = N'{_meta_description.Replace("'", "''")}' , -- meta_descriptions - nvarchar(250)
	                    view_temp = {_view_temp} -- view_temp - int
                    WHERE
                        id = {id}";

                    var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                    if (eff > 0) stt = true;
                    else
                        msg = "Không cập nhật dữ liệu được!";
                    #endregion
                }
            }
            #endregion

            return Json(new
            {
                newid = id,
                status = stt,
                message = msg
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveThumb(int id = 0, string _thumbnail = "")
        {
            var stt = false;
            var msg = "";

            var sql = $"UPDATE TaiLieu SET Thumbnail = N'{_thumbnail.Replace("'", "''").Trim()}' WHERE id = {id}";
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
        public JsonResult UpdateConfig(int id = 0, int _status = 0, int _show_athome = 0)
        {
            var stt = false;
            var msg = "";

            var sql = $"UPDATE TaiLieu SET [Status] = {_status}, show_athome = {_show_athome} WHERE id = {id}";
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

            var sql = $"UPDATE TaiLieu SET view_total = ISNULL(view_total,0) + {_view_temp}, view_temp = 0, last_sync_time = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} WHERE id = {id}";
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

        public JsonResult UpdateOneCol(int id = 0, string _col_name = "", string _value = "")
        {
            var stt = false;
            var msg = "";

            var sql = "";
            if (int.TryParse(_value, out int r))
                sql = $"UPDATE TaiLieu SET {_col_name} = {_value} WHERE id = {id}";
            else
                sql = $"UPDATE TaiLieu SET {_col_name} = '{_value.Replace("'", "''")}' WHERE id = {id}";

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">0: Images; 1: Documents</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UploadFile(int type = 0, int id = 0)
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
                        var path = "";
                        if (type == 0) // images
                            path = Server.MapPath("~") + $"UserData/{SysBaseInfor.GetIdNguoiDung()}/{id}/";
                        else if (type == 1) // documents
                            path = Server.MapPath("~") + $"TaiLieu/{id}/";

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

        public void XoaFile(string file_path)
        {
            if (System.IO.File.Exists(Server.MapPath("~") + file_path))
            {
                try { System.IO.File.Delete(Server.MapPath("~") + file_path); }
                catch { }
            }
        }
    }
}