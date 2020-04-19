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

namespace ViELearn.BackEnd.Controllers
{
    public class QuanLyVanBanController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        [AllowAnonymous]
        public ActionResult Public(
             int coquan = 0,
            int loaivb = 0,
            int linhvuc = 0,
            int tinhtrang = 1,
            string trichyeu = "",
            string sovb = "",
            string kyhieu = "",
            string nbh_s = "",
            string nbh_e = "",
            int thutu = 0
            )
        {
            var where = $" n.[Type] = 1 AND n.vb_tinhtrang = {tinhtrang} ";
            var order = " n.id DESC ";

            #region Tạo câu lệnh query
            if (coquan > 0)
                where += $" AND n.vb_coquan = {coquan} ";
            if (loaivb > 0)
                where += $" AND n.vb_loaivb = {loaivb} ";
            if (linhvuc > 0)
                where += $" AND n.vb_linhvuc = {linhvuc} ";

            if (trichyeu != "")
                where += $" AND n.Title LIKE N'%{trichyeu.Replace("'", "''")}%' ";
            if (sovb != "")
                where += $" AND n.vb_sovb LIKE N'%{sovb.Replace("'", "''")}%' ";
            if (kyhieu != "")
                where += $" AND n.vb_kyhieu LIKE N'%{kyhieu.Replace("'", "''")}%' ";

            if (nbh_s != "" && nbh_e != "")
                where += $" AND n.vb_ngaybanhanh BETWEEN '{CLibs.DatetimeToTimestampOrgin(DateTime.ParseExact(nbh_s.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture))}' AND '{CLibs.DatetimeToTimestampOrgin(DateTime.ParseExact(nbh_e.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1))}' ";

            // Thu tu:
            // 0: ngay xuat ban - desc
            // 1: ngay ban hanh - desc
            // 2: ngay het hieu luc - desc (ko dùng)
            // 3: trich yeu - asc
            if (thutu == 0)
                order = " n.CreatedAt DESC ";
            else if (thutu == 1)
                order = " n.vb_ngaybanhanh DESC ";
            else if (thutu == 2)
                order = " n.Title ";
            #endregion

            var sql = $@"
            SELECT
                n.* ,
                cq.TenDanhMuc title_coquan ,
                l.TenDanhMuc title_loaivb ,
                lv.TenDanhMuc title_linhvuc
            FROM dbo.News n 
                LEFT OUTER JOIN DanhMucChung cq ON cq.id = n.vb_coquan 
                LEFT OUTER JOIN DanhMucChung l ON l.id = n.vb_loaivb 
                LEFT OUTER JOIN DanhMucChung lv ON lv.id = n.vb_linhvuc 
            WHERE {where}
            ORDER BY {order}";
            var dtBaiViet = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsBaiViet = dtBaiViet;

            #region Lấy ra các danh mục cho vào box chọn
            sql = "SELECT * FROM DanhMucChung WHERE LoaiDanhMuc IN (3,4,5)";
            var dtDanhMuc = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            var dtCoQuan = dtDanhMuc.Clone();
            var dtLoaiVb = dtDanhMuc.Clone();
            var dtLinhVuc = dtDanhMuc.Clone();

            foreach (DataRow dr in dtDanhMuc.Rows)
            {
                if (dr["LoaiDanhMuc"].ToString() == "3")
                    dtCoQuan.ImportRow(dr);
                else if (dr["LoaiDanhMuc"].ToString() == "4")
                    dtLoaiVb.ImportRow(dr);
                else if (dr["LoaiDanhMuc"].ToString() == "5")
                    dtLinhVuc.ImportRow(dr);
            }

            ViewBag.DsCoQuanBh = dtCoQuan;
            ViewBag.DsLoaiVanBan = dtLoaiVb;
            ViewBag.DsLinhVuc = dtLinhVuc;
            #endregion

            return View();
        }
        public ActionResult Index(
            int coquan = 0,
            int loaivb = 0,
            int linhvuc = 0,
            int tinhtrang = 1,
            string trichyeu = "",
            string sovb = "",
            string kyhieu = "",
            string nbh_s = "",
            string nbh_e = "",
            int thutu = 0
            )
        {
            var where = $" n.[Type] = 1 AND n.vb_tinhtrang = {tinhtrang} ";
            var order = " n.id DESC ";

            #region Tạo câu lệnh query
            if (coquan > 0)
                where += $" AND n.vb_coquan = {coquan} ";
            if (loaivb > 0)
                where += $" AND n.vb_loaivb = {loaivb} ";
            if (linhvuc > 0)
                where += $" AND n.vb_linhvuc = {linhvuc} ";

            if (trichyeu != "")
                where += $" AND n.Title LIKE N'%{trichyeu.Replace("'", "''")}%' ";
            if (sovb != "")
                where += $" AND n.vb_sovb LIKE N'%{sovb.Replace("'", "''")}%' ";
            if (kyhieu != "")
                where += $" AND n.vb_kyhieu LIKE N'%{kyhieu.Replace("'", "''")}%' ";

            if (nbh_s != "" && nbh_e != "")
                where += $" AND n.vb_ngaybanhanh BETWEEN '{CLibs.DatetimeToTimestampOrgin(DateTime.ParseExact(nbh_s.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture))}' AND '{CLibs.DatetimeToTimestampOrgin(DateTime.ParseExact(nbh_e.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1))}' ";

            // Thu tu:
            // 0: ngay xuat ban - desc
            // 1: ngay ban hanh - desc
            // 2: ngay het hieu luc - desc (ko dùng)
            // 3: trich yeu - asc
            if (thutu == 0)
                order = " n.CreatedAt DESC ";
            else if (thutu == 1)
                order = " n.vb_ngaybanhanh DESC ";
            else if (thutu == 2)
                order = " n.Title ";
            #endregion

            var sql = $@"
            SELECT
                n.* ,
                cq.TenDanhMuc title_coquan ,
                l.TenDanhMuc title_loaivb ,
                lv.TenDanhMuc title_linhvuc
            FROM dbo.News n 
                LEFT OUTER JOIN DanhMucChung cq ON cq.id = n.vb_coquan 
                LEFT OUTER JOIN DanhMucChung l ON l.id = n.vb_loaivb 
                LEFT OUTER JOIN DanhMucChung lv ON lv.id = n.vb_linhvuc 
            WHERE {where}
            ORDER BY {order}";
            var dtBaiViet = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsBaiViet = dtBaiViet;

            #region Lấy ra các danh mục cho vào box chọn
            sql = "SELECT * FROM DanhMucChung WHERE LoaiDanhMuc IN (3,4,5)";
            var dtDanhMuc = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            var dtCoQuan = dtDanhMuc.Clone();
            var dtLoaiVb = dtDanhMuc.Clone();
            var dtLinhVuc = dtDanhMuc.Clone();

            foreach (DataRow dr in dtDanhMuc.Rows)
            {
                if (dr["LoaiDanhMuc"].ToString() == "3")
                    dtCoQuan.ImportRow(dr);
                else if (dr["LoaiDanhMuc"].ToString() == "4")
                    dtLoaiVb.ImportRow(dr);
                else if (dr["LoaiDanhMuc"].ToString() == "5")
                    dtLinhVuc.ImportRow(dr);
            }

            ViewBag.DsCoQuanBh = dtCoQuan;
            ViewBag.DsLoaiVanBan = dtLoaiVb;
            ViewBag.DsLinhVuc = dtLinhVuc;
            #endregion

            return View();
        }
        public ActionResult Create(int type = 0)
        {
            #region Lấy ra các danh mục cho vào box chọn
            var sql = "SELECT * FROM DanhMucChung WHERE LoaiDanhMuc IN (3,4,5)";
            var dtDanhMuc = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            var dtCoQuan = dtDanhMuc.Clone();
            var dtLoaiVb = dtDanhMuc.Clone();
            var dtLinhVuc = dtDanhMuc.Clone();

            foreach (DataRow dr in dtDanhMuc.Rows)
            {
                if (dr["LoaiDanhMuc"].ToString() == "3")
                    dtCoQuan.ImportRow(dr);
                else if (dr["LoaiDanhMuc"].ToString() == "4")
                    dtLoaiVb.ImportRow(dr);
                else if (dr["LoaiDanhMuc"].ToString() == "5")
                    dtLinhVuc.ImportRow(dr);
            }

            ViewBag.DsCoQuanBh = dtCoQuan;
            ViewBag.DsLoaiVanBan = dtLoaiVb;
            ViewBag.DsLinhVuc = dtLinhVuc;
            #endregion

            return View();
        }
        public ActionResult Edit(int id)
        {
            #region Lấy ra các danh mục cho vào box chọn
            var sql = "SELECT * FROM DanhMucChung WHERE LoaiDanhMuc IN (3,4,5)";
            var dtDanhMuc = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            var dtCoQuan = dtDanhMuc.Clone();
            var dtLoaiVb = dtDanhMuc.Clone();
            var dtLinhVuc = dtDanhMuc.Clone();

            foreach (DataRow dr in dtDanhMuc.Rows)
            {
                if (dr["LoaiDanhMuc"].ToString() == "3")
                    dtCoQuan.ImportRow(dr);
                else if (dr["LoaiDanhMuc"].ToString() == "4")
                    dtLoaiVb.ImportRow(dr);
                else if (dr["LoaiDanhMuc"].ToString() == "5")
                    dtLinhVuc.ImportRow(dr);
            }

            ViewBag.DsCoQuanBh = dtCoQuan;
            ViewBag.DsLoaiVanBan = dtLoaiVb;
            ViewBag.DsLinhVuc = dtLinhVuc;
            #endregion

            #region Get item infos
            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM News WHERE id = {id}", _cnn);
            ViewBag.Infos = dtInfos.Rows[0];
            #endregion

            ViewBag.Files = $"{Server.MapPath(@"\")}UserData\\{SysBaseInfor.GetIdNguoiDung()}\\{id}";
            ViewBag.Id = id;
            return View();
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
            int id = 0,
            int _coquan = 0,
            int _status = 1,
            int _loaivb = 0,
            int _linhvuc = 0,
            int _tinhtrang = 0,
            string _trichyeu = "",
            string _summary = "",
            string _trichyeu_slug = "",
            string _sovb = "",
            string _kyhieu = "",
            string _ngaybanhanh = "")
        {
            var stt = false;
            var msg = "";
            var new_id = 0;

            #region Phân tích request/submit form (nếu có)
            if (_trichyeu == null || _trichyeu.Trim() == "" || _trichyeu_slug == null || _trichyeu_slug.Trim() == "")
                msg = "Nội dung trích yếu cho văn bản không được để trống";
            else
            {
                if (_ngaybanhanh == "") _ngaybanhanh = DateTime.Now.ToString("dd/MM/yyyy");
                var tmp = DateTime.Now;
                if (!DateTime.TryParseExact(_ngaybanhanh, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmp))
                    _ngaybanhanh = DateTime.Now.ToString("dd/MM/yyyy");
                if (id < 1)
                {
                    #region Thêm thông tin văn bản
                    //var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM News WHERE slug = N'{_slug.Replace("'", "''").Trim()}' OR title = N'{_title.Replace("'", "''").Trim()}'", _cnn);
                    //if (exited != null && int.Parse(exited.ToString()) > 0)
                    //{
                    //    return Json(new
                    //    {
                    //        status = false,
                    //        message = "Đã có văn bản có cùng nội dung trích yếu hoặc url!"
                    //    });
                    //}
                    var newid = DBLibs.ExecuteScalar($@"
                    INSERT INTO dbo.News
                    (
                        Slug ,
                        UnitId ,
                        Title ,
                        Summary ,
                        Cates ,
                        Type ,
                        Status ,
                        CreatedAt ,
                        CreatedBy ,
                        PublishAt ,

                        vb_coquan ,
                        vb_loaivb ,
                        vb_linhvuc ,
                        vb_tinhtrang ,
                        vb_sovb ,
                        vb_kyhieu ,
                        vb_ngaybanhanh 
                    )
                    VALUES
                    (
                        '{_trichyeu_slug.Replace("'", "''").Trim()}' , -- Slug - varchar(250)
                        {SysBaseInfor.GetCurrentUnitId()} , -- UnitId - int
                        N'{_trichyeu.Replace("'", "''").Trim()}' , -- Title - nvarchar(200)
                        N'{_summary.Replace("'", "''").Trim()}' , -- Summary - nvarchar(1000)
                        N'Văn bản' , -- Cates - nvarchar(1000)
                        1 , -- Type - tinyint
                        1 , -- Status - tinyint
                        {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , -- CreatedAt - int
                        {SysBaseInfor.GetIdNguoiDung()} , -- CreatedBy - int
                        {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , -- PublishAt - int

                        {_coquan} , -- vb_coquan - int
                        {_loaivb} , -- vb_loaivb - int
                        {_linhvuc} , -- vb_linhvuc - int
                        {_tinhtrang} , -- vb_tinhtrang - int
                        N'{_sovb.Replace("'", "''").Trim()}' , -- vb_sovb - nvarchar(200)
                        N'{_kyhieu.Replace("'", "''").Trim()}' , -- vb_kyhieu - nvarchar(200)
                        {CLibs.DatetimeToTimestampOrgin(DateTime.ParseExact(_ngaybanhanh, "dd/MM/yyyy", CultureInfo.InvariantCulture))}  -- vb_ngaybanhanh - int
                    )  SELECT SCOPE_IDENTITY() ", _cnn);
                    int.TryParse(newid == null ? "0" : newid.ToString(), out id);
                    if (newid != null) { stt = true; new_id = id; }
                    else
                        msg = "Không thêm dữ liệu vào được!";
                    #endregion
                }
                else
                {
                    #region Update thông tin văn bản
                    //var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM News WHERE id <> {id} AND (slug = N'{_slug.Replace("'", "''").Trim()}' OR title = N'{_title.Replace("'", "''").Trim()}')", _cnn);
                    //if (exited != null && int.Parse(exited.ToString()) > 0)
                    //{
                    //    return Json(new
                    //    {
                    //        status = false,
                    //        message = "Đã có bài viết có cùng tiêu đề hoặc url!"
                    //    });
                    //}
                    var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE News 
                    SET 
                        Slug = '{_trichyeu_slug.Replace("'", "''").Trim()}' , -- Slug - varchar(250)
                        Title = N'{_trichyeu.Replace("'", "''").Trim()}' , -- Title - nvarchar(200)
                        Summary = N'{_summary.Replace("'", "''").Trim()}' , -- Summary - nvarchar(1000)
                        Status = {_status} , -- Status - tinyint
                        UpdatedAt = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , -- UpdatedAt - int
                        PublishAt = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , -- PublishAt - int
                        UpdatedBy = {SysBaseInfor.GetIdNguoiDung()} , -- UpdatedBy - int

                        vb_coquan = {_coquan} , -- vb_coquan - int
                        vb_loaivb = {_loaivb} , -- vb_loaivb - int
                        vb_linhvuc = {_linhvuc} , -- vb_linhvuc - int
                        vb_tinhtrang = {_tinhtrang} , -- vb_tinhtrang - int
                        vb_sovb = N'{_sovb.Replace("'", "''").Trim()}' , -- vb_sovb - nvarchar(200)
                        vb_kyhieu = N'{_kyhieu.Replace("'", "''").Trim()}' , -- vb_kyhieu - nvarchar(200)
                        vb_ngaybanhanh = {CLibs.DatetimeToTimestampOrgin(DateTime.ParseExact(_ngaybanhanh, "dd/MM/yyyy", CultureInfo.InvariantCulture))}  -- vb_ngaybanhanh - int
                    WHERE
                        id = {id}", _cnn);
                    if (eff > 0) stt = true;
                    else
                        msg = "Không cập nhật dữ liệu được!";
                    #endregion
                }
            }
            #endregion

            return Json(new
            {
                status = stt,
                newid = new_id,
                message = msg
            });
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
                        var path = Server.MapPath("~") + $"UserData/{SysBaseInfor.GetIdNguoiDung()}/{id}/";

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