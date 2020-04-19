using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using QLVC.DAL.ModulesDAL;
using QLVC.Models.ProjectModels;
using QuanLyVuChay.Ultilities;
using System.Configuration;
using System.Data;

namespace ViElearn.BackEnd.Controllers
{
    public class TagsManagerController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            ViewBag.Title = "Danh mục ";
            var dtLoaiDm = DsLoaiDanhMuc();
            foreach(DataRow dr in dtLoaiDm.Rows)
            {
                if (Request["t"].ToString() == dr["id"].ToString())
                {
                    ViewBag.Title += dr["tieuDe"];
                    break;
                }
            }
            ViewBag.DsLoaiDanhMuc = dtLoaiDm;
            ViewBag.DsDanhMuc = DsDanhMuc();
            ViewBag.Type = Request["t"];
            return View();
        }
        public ActionResult Create(int type = 0)
        {
            ViewBag.Title = "Thêm danh mục ";
            var dtLoaiDm = DsLoaiDanhMuc();
            foreach (DataRow dr in dtLoaiDm.Rows)
            {
                if (Request["t"].ToString() == dr["id"].ToString())
                {
                    ViewBag.Title += dr["tieuDe"];
                    break;
                }
            }

            ViewBag.DsLoaiDanhMuc = dtLoaiDm;
            ViewBag.DsDanhMucCha = DsDanhMucCha();
            return View();
        }
        public JsonResult Save(
            int id = 0, 
            int _loaidanhmuc = 0, 
            int _danhmuccha = 0, 
            string _tendanhmuc = "", 
            string _dstendanhmuc = "",
            int _sothutu = 0, 
            int _trangthai = 1, 
            string _thongtinthem = "")
        {
            var stt = false;
            var msg = "";
            #region Phân tích request/submit form (nếu có)
            if (_tendanhmuc.Trim() == "" && _dstendanhmuc.Trim() == "")
                msg = "Tên danh mục không được để trống";
            else
            {
                if (id < 1)
                {
                    if (_dstendanhmuc == "")
                    {
                        #region Thêm danh mục
                        // Bỏ phần check trùng tên bên dưới do có những danh mục khác loại
                        // vd: Thiết bị chữa cháy & thiết bị ngoài ngành
                        //var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM DanhMucChung WHERE TenDanhMuc = N'{_tendanhmuc.Replace("'", "''").Trim()}'", _cnn);
                        //if (exited != null && int.Parse(exited.ToString()) > 0)
                        //{
                        //    return Json(new
                        //    {
                        //        status = false,
                        //        message = "Tên danh mục đã tồn tại!"
                        //    });
                        //}
                        var eff = DBLibs.ExecuteNonQuery($@"
                        INSERT INTO dbo.DanhMucChung
                        (
                            TenDanhMuc ,
                            LoaiDanhMuc ,
                            SoThuTu ,
                            TrangThai ,
                            ThongTinThem ,
                            idDanhMucCha ,
                            created_at
                        )
                        VALUES  
                        (
                            N'{_tendanhmuc.Replace("'", "''").Trim()}' , -- TenDanhMuc - nvarchar(500)
                            {_loaidanhmuc} , -- LoaiDanhMuc - smallint
                            {_sothutu} , -- SoThuTu - smallint
                            {_trangthai} , -- TrangThai - bit
                            N'{_thongtinthem.Replace("'", "''").Trim()}' , -- ThongTinThem - nvarchar(1500)
                            {_danhmuccha} , -- idDanhMucCha - bigint
                            {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} -- created_at - int
                        )", _cnn);
                        if (eff > 0) stt = true;
                        else
                            msg = "Không thêm dữ liệu vào được!";
                    }
                    else
                    {
                        var stt_ = _sothutu;
                        foreach (string tdm in _dstendanhmuc.Split('\n'))
                        {
                            if (tdm.Trim() == "") continue;
                            DBLibs.ExecuteNonQuery($@"
                            INSERT INTO dbo.DanhMucChung
                            (
                                TenDanhMuc ,
                                LoaiDanhMuc ,
                                SoThuTu ,
                                TrangThai ,
                                ThongTinThem ,
                                idDanhMucCha ,
                                created_at
                            )
                            VALUES  
                            (
                                N'{tdm.Replace("'", "''").Trim()}' , -- TenDanhMuc - nvarchar(500)
                                {_loaidanhmuc} , -- LoaiDanhMuc - smallint
                                {stt_} , -- SoThuTu - smallint
                                {_trangthai} , -- TrangThai - bit
                                N'{_thongtinthem.Replace("'", "''").Trim()}' , -- ThongTinThem - nvarchar(1500)
                                {_danhmuccha} , -- idDanhMucCha - bigint
                                {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} -- created_at - int
                            )", _cnn);
                            stt_++;
                        }
                        stt = true;
                    }
                    #endregion
                }
                else
                {
                    #region Update danh muc
                    var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE DanhMucChung 
                    SET 
                        TenDanhMuc = N'{_tendanhmuc.Replace("'", "''").Trim()}',
                        LoaiDanhMuc = {_loaidanhmuc} ,
                        SoThuTu = {_sothutu} ,
                        TrangThai = {_trangthai} ,
                        ThongTinThem = N'{_thongtinthem.Replace("'", "''").Trim()}' ,
                        idDanhMucCha = {_danhmuccha} ,
                        updated_at = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}
                    WHERE
                        id = {Request["id"]}", _cnn);
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
                message = msg
            });
        }
        public ActionResult Edit(int id)
        {
            ViewBag.Title = "Sửa danh mục";
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