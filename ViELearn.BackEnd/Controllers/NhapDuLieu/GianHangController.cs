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

namespace ViELearn.BackEnd.Controllers
{
    public class GianHangController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            var dt = DBLibs.ExecuteStoreProcedure_Select("sp_gianhang_list", new Hashtable(), _cnn);
            ViewBag.DsGianHang = dt;

            return View();
        }
        public ActionResult Create(int type = 0)
        {
            var dtDsTinh = DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM DanhMucChung WHERE TrangThai = 1 AND LoaiDanhMuc = 14", _cnn);
            ViewBag.DsTinh = dtDsTinh;
            var dtDsHuyen = DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM DanhMucChung WHERE TrangThai = 1 AND LoaiDanhMuc = 15", _cnn);
            ViewBag.DsHuyen = dtDsHuyen;
            var dtDsXa = DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM DanhMucChung WHERE TrangThai = 1 AND LoaiDanhMuc = 16", _cnn);
            ViewBag.DsXa = dtDsXa;

            return View();
        }

        [ValidateInput(false)]
        public JsonResult Save(
            int id = 0,
            string _tengianhang = "",
            string _aliasurl = "",
            string _logourl = "",
            string _diachi = "",
            int _tinhtp = 0,
            int _quanhuyen = 0,
            int _phuongxa = 0,
            string _tieudegioithieu = "",
            string _noidunggioithieu = "")
        {
            var stt = false;
            var msg = "";
            #region Phân tích request/submit form (nếu có)
            if (_tengianhang == null || _tengianhang.Trim() == "")
                msg = "Tên gian hàng không được để trống";
            if (_aliasurl == null || _aliasurl.Trim() == "")
                msg = "Đường dẫn url không được để trống";
            else
            {
                if (id < 1)
                {
                    #region Thêm gian hàng
                    var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM GianHang WHERE aliasUrl = N'{_aliasurl.Replace("'", "''").Trim()}' OR tenGianHang = N'{_tengianhang.Replace("'", "''").Trim()}'", _cnn);
                    if (exited != null && int.Parse(exited.ToString()) > 0)
                    {
                        return Json(new
                        {
                            status = false,
                            message = "Tên gian hàng hoặc url cho gian hàng đã có!"
                        });
                    }
                    var eff = DBLibs.ExecuteNonQuery($@"
                    INSERT INTO dbo.dtGianHang
                    (
                        tenGianHang ,
                        aliasUrl ,
                        logoUrl ,
                        idTinhTp ,
                        idQuanHuyen ,
                        idPhuongXa ,
                        diaChi ,
                        tieuDeGioiThieu ,
                        noiDungGioiThieu 
                    ) VALUES (
                        N'{_tengianhang.Trim().Replace("'", "''")}' , -- tenGianHang - nvarchar(250)
                        '{_aliasurl.Trim().Replace("'", "''")}' , -- aliasUrl - varchar(250)
                        N'{_logourl.Trim().Replace("'", "''")}' , -- logoUrl - nvarchar(250)
                        {_tinhtp} , -- idTinhTp - bigint
                        {_quanhuyen} , -- idQuanHuyen - bigint
                        {_phuongxa} , -- idPhuongXa - bigint
                        N'{_diachi.Trim().Replace("'", "''")}' , -- diaChi - nvarchar(50)
                        N'{_tieudegioithieu.Trim().Replace("'", "''")}' , -- tieuDeGioiThieu - nvarchar(250)
                        N'{_noidunggioithieu.Trim().Replace("'", "''")}' -- noiDungGioiThieu - ntext
                    )", _cnn);
                    if (eff > 0) stt = true;
                    else
                        msg = "Không thêm dữ liệu vào được!";
                    #endregion
                }
                else
                {
                    #region Update danh muc
                    //var eff = DBLibs.ExecuteNonQuery($@"
                    //UPDATE DanhMucChung 
                    //SET 
                    //    TenDanhMuc = N'{_tendanhmuc.Replace("'", "''").Trim()}',
                    //    LoaiDanhMuc = {_loaidanhmuc} ,
                    //    SoThuTu = {_sothutu} ,
                    //    TrangThai = {_trangthai} ,
                    //    ThongTinThem = N'{_thongtinthem.Replace("'", "''").Trim()}' ,
                    //    idDanhMucCha = {_danhmuccha} ,
                    //    updated_at = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}
                    //WHERE
                    //    id = {Request["id"]}", _cnn);
                    //if (eff > 0) stt = true;
                    //else
                    //    msg = "Không cập nhật dữ liệu được!";
                    #endregion
                }
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
            ViewBag.Title = "Sửa gian hàng";
            var dtLoaiDm = DsLoaiDanhMuc();
            foreach (DataRow dr in dtLoaiDm.Rows)
            {
                if (id.ToString() == dr["id"].ToString())
                {
                    ViewBag.Title += dr["tieuDe"];
                    break;
                }
            }
            ViewBag.DsLoaiDanhMuc = dtLoaiDm;
            #region Get item infos
            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM DanhMucChung WHERE id = {id}", _cnn);
            ViewBag.ItemInfos = dtInfos;
            #endregion

            ViewBag.Type = Request["t"];
            ViewBag.IdDm = id;
            ViewBag.DsDanhMucCha = DsDanhMucCha();
            return View();
        }
        private DataTable DsDanhMuc()
        {
            var where = "";
            if (Request["t"] != null && Request["t"] != "") where += "WHERE c.LoaiDanhMuc = " + Request["t"];
            return DBLibs.GetDataBy_DataAdapter($@"SELECT c.*, ISNULL(p.TenDanhMuc, '') tenCha FROM DanhMucChung c LEFT JOIN DanhMucChung p ON c.idDanhMucCha = p.id {where} ORDER BY p.SoThuTu, c.LoaiDanhMuc, c.SoThuTu", _cnn);
        }
        private DataTable DsLoaiDanhMuc()
        {
            return DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM LoaiDanhMuc", _cnn);
        }

        private DataTable DsDanhMucCha()
        {
            var where = " WHERE 1=1 ";
            if (Request["t"] == "2")
                where += " AND LoaiDanhMuc = 1";
            if (Request["t"] == "4")
                where += " AND LoaiDanhMuc = 2";
            return DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM DanhMucChung {where} ORDER BY LoaiDanhMuc, SoThuTu", _cnn);
        }
    }
}