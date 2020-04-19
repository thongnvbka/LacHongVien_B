using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ViELearn.BackEnd.Models;
using ViELearn.BackEnd.Ultilities;
using Newtonsoft.Json;
using ViELearn.DAL.SystemDAL;
using ViELearn.DAL.ModulesDAL;
using ViELearn.Models.ProjectModels;
using ViELearn.Models.SystemModels;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Security;

namespace ViELearn.BackEnd.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ActionResult Index()
        {
            if (Request["t"] == "1")
                ViewBag.Title = "Danh sách hội đồng chấm";
            else if (Request["t"] == "2")
                ViewBag.Title = "Danh sách giáo viên dự thi";

            var sql = $@"SELECT gv.*, t.TenDanhMuc TenTruong FROM GiaoVien gv LEFT OUTER JOIN DanhMucChung t ON t.id = gv.idTruong AND t.LoaiDanhMuc = 10 WHERE gv.TrangThai = {(Request["t"] == "1" ? "9" : "1")}";
            var dtGiaoVien = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsGiaoVien = dtGiaoVien;
            
            ViewBag.Type = Request["t"];
            return View();
        }
        public ActionResult Create(int type = 0)
        {
            if (Request["t"] == "1")
                ViewBag.Title = "Thêm thành viên hội đồng chấm";
            else if (Request["t"] == "2")
                ViewBag.Title = "Thêm giáo viên dự thi";

            var dtTruong = DBLibs.GetDataBy_DataAdapter($@"SELECT ID, TenDanhMuc FROM DanhMucChung WHERE TrangThai = 1 AND LoaiDanhMuc = 10 ORDER BY TenDanhMuc", _cnn);
            ViewBag.DsTruong = dtTruong;

            return View();
        }
        public JsonResult Save(
            int id = 0, 
            string _roleid = "", 
            string _username = "",
            string _fullname = "", 
            string _email = "",
            string _birthday = "",
            string _phone = "",
            int _truong = 0, 
            int _gender = 0,
            int _trangthai = 1, 
            string _pass = "")
        {
            var stt = false;
            var msg = "";

            #region Phân tích request/submit form (nếu có)
            if (_username == null || _username.Trim() == "")
                msg = "Tên không được để trống";
            else if(_pass != "" && _pass.Length < 6)
                msg = "Mật khẩu phải từ 6 ký tự";
            else
            {
                // roleid kia la cua giam khao
                if (_trangthai == 1 && _roleid == "487548ac-1a6b-4787-aada-c59455082f93") _trangthai = 9;
                if (id < 1)
                {
                    #region Thêm tài khoản
                    var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM GiaoVien WHERE magv = N'{_username.Replace("'", "''").Trim()}'", _cnn);
                    if (exited != null && int.Parse(exited.ToString()) > 0)
                    {
                        return Json(new
                        {
                            status = false,
                            message = "Mã giáo viên đã tồn tại!"
                        });
                    }
                    var eff = DBLibs.ExecuteNonQuery($@"
                    INSERT INTO dbo.GiaoVien
                    (
	                    magv ,
	                    TenGiaoVien ,
	                    idTruong ,
	                    NgaySinh ,
	                    GioiTinh ,
	                    DienThoai ,
	                    TrangThai
                    ) VALUES  (
	                    '{_username.Replace("'", "''").Trim()}' , -- magv - varchar(50)
	                    N'{_fullname.Replace("'", "''").Trim()}' , -- TenGiaoVien - nvarchar(250)
	                    {_truong} , -- idTruong - int
	                    '{(_birthday.Trim() == "" ? "1900-1-1" : DateTime.ParseExact(_birthday, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"))}' , -- NgaySinh - date
	                    {_gender} , -- GioiTinh - bit
	                    '{_phone.Replace("'", "''").Trim()}' , -- DienThoai - varchar(15)
	                    {_trangthai}  -- TrangThai - tinyint
                    )", _cnn);
                    if (eff > 0)
                    {
                        #region Thêm tài khoản & gán vào role
                        var result = Create(_username, _fullname, _pass, _roleid);
                        if (!result)
                        {
                            stt = false;
                            DBLibs.ExecuteNonQuery($"DELETE GiaoVien WHERE magv = N'{_username.Replace("'", "''").Trim()}'", _cnn);
                            DBLibs.ExecuteNonQuery($"DELETE AspNetUsers WHERE UserName = N'{_username.Replace("'", "''").Trim()}'", _cnn);
                            msg = "Không tạo được tài khoản";
                        }
                        else
                            stt = true;
                        #endregion
                    }
                    else
                        msg = "Không thêm dữ liệu vào được!";
                    #endregion
                }
                else
                {
                    #region Update thông tin tài khoản
                    var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE GiaoVien 
                    SET 
	                    TenGiaoVien = N'{_fullname.Replace("'", "''").Trim()}' , -- TenGiaoVien - nvarchar(250)
	                    idTruong = {_truong} , -- idTruong - int
	                    NgaySinh = '{DateTime.ParseExact(_birthday, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")}' , -- NgaySinh - date
	                    GioiTinh = {_gender} , -- GioiTinh - bit
	                    DienThoai = '{_phone.Replace("'", "''").Trim()}' , -- DienThoai - varchar(15)
	                    TrangThai = {_trangthai}  -- TrangThai - tinyint
                    WHERE
                        id = {Request["id"]} AND magv = N'{_username.Replace("'", "''").Trim()}'", _cnn);

                    #region Thay pass (nếu có)
                    if (_pass != "")
                    {
                        ResetPassword(_username, _pass);
                    }
                    #endregion
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
            if (Request["t"] == "1")
                ViewBag.Title = "Sửa thông tin thành viên hội đồng chấm";
            else if (Request["t"] == "2")
                ViewBag.Title = "Sửa thông tin giáo viên dự thi";

            #region Get item infos
            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM GiaoVien WHERE id = {id}", _cnn);
            ViewBag.Infos = dtInfos.Rows;
            #endregion
            
            var dtTruong = DBLibs.GetDataBy_DataAdapter($@"SELECT ID, TenDanhMuc FROM DanhMucChung WHERE TrangThai = 1 AND LoaiDanhMuc = 10 ORDER BY TenDanhMuc", _cnn);
            ViewBag.DsTruong = dtTruong;

            ViewBag.Type = Request["t"];
            ViewBag.IdDm = id;
            return View();
        }

        private bool Create(string username, string displayname, string password, string roleId)
        {
            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {
                    UserName = username,
                    DisplayName = displayname
                };
                var result = UserManager.Create(user, password);
                if (result.Succeeded)
                {
                    user.Type = 2;
                    user.UnitId = SysBaseInfor.GetCurrentUnitId().ToInt32();
                    user.UnitName = SysBaseInfor.GetCurrentUnitCode().ToLower();
                    try { user.TypeInfo = int.Parse(DBLibs.ExecuteScalar($"SELECT TOP 1 id FROM GiaoVien WHERE magv = N'{username.Replace("'", "''")}'", _cnn).ToString()); }
                    catch { }
                    var resultActive = UserManager.SetLockoutEnabled(user.Id, false);
                    if (resultActive.Succeeded)
                    {
                        try
                        {
                            var sql = $@"
                            INSERT INTO dbo.AspNetUserRoles
                            ( UserId, RoleId )
                            VALUES
                            (
	                            N'{user.Id}', -- UserId - nvarchar(128)
	                            N'{roleId}'  -- RoleId - nvarchar(128)
                            )";
                            DBLibs.ExecuteNonQuery(sql, _cnn);
                        }
                        catch { }
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            return false;
        }
        private bool ResetPassword(string username, string new_pass = "Abc@123")
        {
            try
            {
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = UserManager.FindByName(username);
                if (user == null)
                    user = UserManager.FindById(username);
                if (user != null)
                {
                    user.PasswordHash = UserManager.PasswordHasher.HashPassword(new_pass);
                    UserManager.Update(user);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public ActionResult ImgQrCode()
        {
            return View();
        }
    }
}