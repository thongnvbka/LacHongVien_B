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
using System.Data;
using System.IO;

namespace ViELearn.BackEnd.Controllers
{
    public class BaiGiangController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private readonly string _diruploadpath = "UserUpload\\BaiGiangDienTu";

        public ActionResult Index()
        {
            var dtBaiGiang = DBLibs.GetDataBy_DataAdapter($@"
            SELECT bg.*, dt.TenDotThi 
            FROM dbo.BaiGiang bg LEFT OUTER JOIN DotThi dt ON dt.id = bg.idDotThi 
            WHERE idGvienChinh = {SysBaseInfor.GetIdNguoiDung()} 
            ORDER BY updated_at DESC", _cnn);
            ViewBag.DsBaiGiang = dtBaiGiang;

            return View();
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult Save(
            int id = 0,
            string _tenbaigiang = "",
            string[] _cacmonhoc = null,
            string _cactacgia = "")
        {
            var stt = false;
            var msg = "";
            var strMon = ";";
            foreach (string m in _cacmonhoc) strMon += m + ";";
            strMon += ";";
            while (strMon.Contains(";;"))
                strMon = strMon.Replace(";;", ";");

            #region Phân tích request/submit form (nếu có)
            if (id < 1)
            {
                #region Thêm
                var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM BaiGiang WHERE TenBaiGiang = N'{_tenbaigiang.Replace("'", "''").Trim()}'", _cnn);
                if (exited != null && int.Parse(exited.ToString()) > 0)
                {
                    return Json(new
                    {
                        status = false,
                        message = "Tên bài giảng đã tồn tại!"
                    });
                }
                var eff = DBLibs.ExecuteNonQuery($@"
                INSERT INTO dbo.BaiGiang
                (
	                TenBaiGiang ,
	                MonHoc ,
	                TacGia ,
                    idGvienChinh ,
	                created_at ,
	                updated_at 
                )
                VALUES
                (
	                N'{_tenbaigiang.Trim().Replace("'", "''")}' , -- TenDotThi - nvarchar(250)
	                N'{strMon.Trim().Replace("'", "''")}' , -- MonHoc - nvarchar(500)
	                N'{_cactacgia.Trim().Replace("'", "''")}' , -- TacGia - nvarchar(500)
	                {SysBaseInfor.GetCurrentUserTypeInfo().ToString()} , -- idGvienChinh - int
	                {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , -- created_at - int
	                {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} -- updated_at - int
                )
                ", _cnn);
                if (eff > 0) stt = true;
                else
                    msg = "Không thêm dữ liệu vào được!";
                #endregion
            }
            else
            {
                #region Kiem tra xem bai giang da duoc nop chua (bai giang nop roi thi ko cho update)
                var submit_at = DBLibs.ExecuteScalar($"SELECT submit_at FROM BaiGiang WHERE id = {id}", _cnn);
                if (submit_at != null)
                {
                    return Json(new
                    {
                        status = false,
                        message = "Bài giảng đã nộp rồi, không được sửa!"
                    });
                }
                #endregion
                #region Update
                var eff = DBLibs.ExecuteNonQuery($@"
                UPDATE BaiGiang 
                SET 
                    TenBaiGiang = N'{_tenbaigiang.Trim().Replace("'", "''")}' , -- TenBaiGiang - nvarchar(250)
	                MonHoc = N'{strMon.Trim().Replace("'", "''")}' , -- MonHoc - nvarchar(500)
	                TacGia = N'{_cactacgia.Trim().Replace("'", "''")}' , -- TacGia - nvarchar(500)
	                updated_at = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} -- updated_at - int
                WHERE
                    id = {Request["id"]}", _cnn);
                if (eff > 0) stt = true;
                else
                    msg = "Không cập nhật dữ liệu được!";
                #endregion
            }
            #endregion

            return Json(new
            {
                status = stt,
                message = msg
            });
        }

        public JsonResult NopBaiGiang(int id = 0, int _dotthi = 0)
        {
            var stt = false;
            var msg = "";

            #region Phân tích request/submit form (nếu có)
            if (id < 1 || _dotthi < 1)
            {
                msg = "Chưa chọn đợt thi hoặc bài giảng!";
            }
            else
            {
                #region Update
                var eff = DBLibs.ExecuteNonQuery($@"
                UPDATE BaiGiang 
                SET 
                    idDotThi = {_dotthi} , -- idDotThi - int
	                submit_at = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} -- submit_at - int
                WHERE
                    id = {Request["id"]}", _cnn);
                if (eff > 0) stt = true;
                else
                    msg = "Không cập nhật dữ liệu được!";
                #endregion
            }
            #endregion

            return Json(new
            {
                status = stt,
                message = msg
            });
        }
        public ActionResult Edit(int id)
        {
            #region Get item infos
            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM BaiGiang WHERE id = {id}", _cnn);
            ViewBag.Infos = dtInfos.Rows[0];
            #endregion

            #region Lấy danh sách các đợt thi còn hạn nộp
            var dtDotThi = DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM DotThi WHERE ThoiHanNop > '{DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")}' ORDER BY ThoiHanNop DESC", _cnn);
            ViewBag.DotThi = dtDotThi;
            #endregion

            #region Lay danh sach file bai giang da duoc upload
            var pathString = Path.Combine(Server.MapPath(@"\"), _diruploadpath.ToString(), SysBaseInfor.GetIdNguoiDung(), id.ToString());
            var dtFiles = new DataTable();
            dtFiles.Columns.Add("TenFile");
            dtFiles.Columns.Add("FilePath");
            if (Directory.Exists(pathString))
            {
                var dir = new DirectoryInfo(pathString);
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo f in files)
                {
                    var dr = dtFiles.NewRow();
                    dr["TenFile"] = f.Name;
                    dr["FilePath"] = f.FullName.Replace(Server.MapPath(@"\"), "/");
                    dtFiles.Rows.Add(dr);
                }
            }
            ViewBag.DsFile = dtFiles;
            #endregion

            ViewBag.IdDm = id;
            return View();
        }

        public ActionResult UploadFiles(int id = 0)
        {
            if (id == 0)
            {
                return Json(new { Message = "File bài giảng phải được gắn kết vào bài giảng cụ thể!" });
            }

            bool isSavedSuccessfully = true;
            var fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        var pathString = Path.Combine(Server.MapPath(@"\"), _diruploadpath.ToString(), SysBaseInfor.GetIdNguoiDung(), id.ToString());
                        if (!Directory.Exists(pathString))
                            Directory.CreateDirectory(pathString);

                        var fullPath = string.Format("{0}\\{1}", pathString, file.FileName);
                        try
                        {
                            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                        }
                        catch { }

                        file.SaveAs(fullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }

            if (isSavedSuccessfully)
                return Json(new { Message = "Đăng thành công bài giảng: " + fName });
            else
                return Json(new { Message = "Không đăng bài giảng lên được!" });
        }

        public JsonResult Xoa_FileBaiGiang(string _filename = "")
        {
            var stt = false;
            var msg = "";

            var fullPath = Path.Combine(Server.MapPath(@"\"), _diruploadpath.ToString(), SysBaseInfor.GetIdNguoiDung(), _filename);
            if (System.IO.File.Exists(fullPath))
            {
                try
                {
                    System.IO.File.Delete(fullPath);
                    stt = true;
                }
                catch (Exception ex) { msg = ex.Message; }
            }

            return Json(new
            {
                status = stt,
                message = msg
            });
        }

        #region Phan danh cho administrator
        public ActionResult BaiGiangDaNop()
        {
            #region danh sach giam thi
            var dtDsGiaoVien = DBLibs.GetDataBy_DataAdapter($@"
            SELECT id, TenGiaoVien text FROM GiaoVien WHERE TrangThai = 9 ORDER BY TenGiaoVien
            ", _cnn);
            ViewBag.DsGiaoVien = CLibs.ConvertDatatableToJson(dtDsGiaoVien);
            #endregion

            var dtBaiGiang = DBLibs.GetDataBy_DataAdapter($@"
            SELECT
                bg.*,
                dt.TenDotThi,
                gv.TenGiaoVien,
                gv1.TenGiaoVien TenGiamKhao1,
                gv2.TenGiaoVien TenGiamKhao2,
                gv1.id idGiamKhao1,
                gv2.id idGiamKhao2
            FROM dbo.BaiGiang bg 
                LEFT OUTER JOIN DotThi dt ON dt.id = bg.idDotThi 
                LEFT OUTER JOIN GiaoVien gv ON gv.id = bg.idGvienChinh 
                LEFT OUTER JOIN GiaoVien gv1 ON gv1.id = bg.idGiamKhao1 
                LEFT OUTER JOIN GiaoVien gv2 ON gv2.id = bg.idGiamKhao2
            WHERE 
                submit_at IS NOT NULL
            ORDER BY dt.TenDotThi, bg.submit_at DESC", _cnn);
            ViewBag.DsBaiGiang = dtBaiGiang;

            return View();
        }
        #endregion

        #region Phần dành cho Tổ trưởng Hội đồng chấm
        public ActionResult BaiGiangDaCham()
        {
            var dtBaiGiang = DBLibs.GetDataBy_DataAdapter($@"
            SELECT
                dt.TenDotThi,
                bg.id,
                bg.TenBaiGiang,
                bg.Diem1,
                bg.NgayChamDiem1,
                bg.Diem2,
                bg.NgayChamDiem2,
                CASE WHEN bg.Diem1 > bg.Diem2 THEN bg.Diem1 - bg.Diem2 ELSE bg.Diem2 - bg.Diem1 END ChenhLech,
                gv.TenGiaoVien,
                gv1.TenGiaoVien TenGiamKhao1,
                gv2.TenGiaoVien TenGiamKhao2,
                gv1.id idGiamKhao1,
                gv2.id idGiamKhao2
            FROM dbo.BaiGiang bg 
                LEFT OUTER JOIN DotThi dt ON dt.id = bg.idDotThi 
                LEFT OUTER JOIN GiaoVien gv ON gv.id = bg.idGvienChinh 
                LEFT OUTER JOIN GiaoVien gv1 ON gv1.id = bg.idGiamKhao1 
                LEFT OUTER JOIN GiaoVien gv2 ON gv2.id = bg.idGiamKhao2
            WHERE 
                submit_at IS NOT NULL
            ORDER BY 
                CASE WHEN bg.Diem1 > bg.Diem2 THEN bg.Diem1 - bg.Diem2 ELSE bg.Diem2 - bg.Diem1 END DESC,
                dt.TenDotThi, bg.submit_at DESC", _cnn);
            ViewBag.DsBaiGiang = dtBaiGiang;

            return View();
        }
        #endregion

        #region Phan danh cho giam khao
        public ActionResult BaiGiangChuaCham()
        {
            var dtBaiGiang = DBLibs.GetDataBy_DataAdapter($@"
            SELECT
                bg.id,
                bg.TenBaiGiang,
                bg.submit_at,
                bg.FilePathBaiGiang,
                CASE WHEN bg.idGiamKhao1 = {SysBaseInfor.GetIdNguoiDung()} THEN bg.Diem1 ELSE CASE WHEN bg.idGiamKhao2 = {SysBaseInfor.GetIdNguoiDung()} THEN bg.Diem2 ELSE '' END END Diem,
                CASE WHEN bg.idGiamKhao1 = {SysBaseInfor.GetIdNguoiDung()} THEN bg.idGiamKhao1 ELSE CASE WHEN bg.idGiamKhao2 = {SysBaseInfor.GetIdNguoiDung()} THEN bg.idGiamKhao2 ELSE '' END END idGiamKhao,
                CASE WHEN bg.idGiamKhao1 = {SysBaseInfor.GetIdNguoiDung()} THEN bg.NgayChamDiem1 ELSE CASE WHEN bg.idGiamKhao2 = {SysBaseInfor.GetIdNguoiDung()} THEN bg.NgayChamDiem2 ELSE '' END END NgayChamDiem,
                dt.TenDotThi 
            FROM dbo.BaiGiang bg 
                LEFT OUTER JOIN DotThi dt ON dt.id = bg.idDotThi 
            WHERE
                bg.idGiamKhao1 = {SysBaseInfor.GetIdNguoiDung()} OR
                bg.idGiamKhao2 = {SysBaseInfor.GetIdNguoiDung()} 
            ORDER BY
                bg.updated_at DESC", _cnn);
            ViewBag.DsBaiGiang = dtBaiGiang;

            return View();
        }
        public JsonResult ChamBaiGiang(int _idbg = 0, int _diem = -1)
        {
            var stt = false;
            var msg = "";

            #region Phân tích request/submit form (nếu có)
            if (_idbg == 0 || _diem < 0 || _diem > 100)
            {
                msg = "Sai ID bài giảng hoặc điểm sai định dạng!";
            }
            else
            {
                #region Update
                var sql = $@"
                UPDATE BaiGiang 
                SET 
                    Diem1 = {_diem} , -- Diem1 - int
                    NgayChamDiem1 = '{CLibs.DatetimeToTimestampOrgin(DateTime.Now)}' -- NgayChamDiem1 - datetime
                WHERE
                    id = {_idbg} AND idGiamKhao1 = {SysBaseInfor.GetIdNguoiDung()}

                UPDATE BaiGiang 
                SET 
                    Diem2 = {_diem} , -- Diem2 - int
                    NgayChamDiem2 = '{CLibs.DatetimeToTimestampOrgin(DateTime.Now)}' -- NgayChamDiem2 - datetime
                WHERE
                    id = {_idbg} AND idGiamKhao2 = {SysBaseInfor.GetIdNguoiDung()}";

                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                if (eff > 0) stt = true;
                else
                    msg = "Không cập nhật dữ liệu được!";
                #endregion
            }
            #endregion

            return Json(new
            {
                status = stt,
                message = msg
            });
        }
        #endregion
    }
}