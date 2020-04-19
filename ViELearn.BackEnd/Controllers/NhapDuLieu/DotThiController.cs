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

namespace ViELearn.BackEnd.Controllers
{
    public class DotThiController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            var dtDotThi = DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM dbo.DotThi ORDER BY ThoiHanNop DESC, ThoiHanCham DESC", _cnn);
            ViewBag.DsDotThi = dtDotThi;

            return View();
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult Save(
            int id = 0,
            string _tendotthi = "",
            string _thoihannop = "",
            string _thoihancham = "",
            string[] _cacmonthi = null,
            int _diemchenhlech = 0,
            string _thele = "",
            string _gioithieu = "")
        {
            var stt = false;
            var msg = "";
            var strMon = ";";
            foreach(string m in _cacmonthi) strMon += m + ";";
            strMon += ";";
            while (strMon.Contains(";;"))
                strMon = strMon.Replace(";;", ";");

            #region Phân tích request/submit form (nếu có)
            if (id < 1)
            {
                #region Thêm
                var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM DotThi WHERE TenDotThi = N'{_tendotthi.Replace("'", "''").Trim()}'", _cnn);
                if (exited != null && int.Parse(exited.ToString()) > 0)
                {
                    return Json(new
                    {
                        status = false,
                        message = "Tên đợt thi đã tồn tại!"
                    });
                }
                var eff = DBLibs.ExecuteNonQuery($@"
                INSERT INTO dbo.DotThi
                (
	                TenDotThi ,
	                ThoiHanNop ,
	                ThoiHanCham ,
	                CacMonThi ,
	                DiemChenh ,
                    TheLe ,
                    GioiThieu ,
	                created_at 
                )
                VALUES
                (
	                N'{_tendotthi.Trim().Replace("'", "''")}' , -- TenDotThi - nvarchar(250)
	                '{DateTime.ParseExact(_thoihannop, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")}' , -- ThoiHanNop - date
	                '{DateTime.ParseExact(_thoihancham, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")}' , -- ThoiHanCham - date
	                N'{strMon.Trim().Replace("'", "''")}' , -- CacMonThi - nvarchar(500)
	                {_diemchenhlech} , -- DiemChenh - tinyint
	                N'{_thele.Trim().Replace("'", "''")}' , -- TheLe - nvarchar(500)
	                N'{_gioithieu.Trim().Replace("'", "''")}' , -- GioiThieu - nvarchar(500)
	                {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} -- created_at - int
                )
                ", _cnn);
                if (eff > 0) stt = true;
                else
                    msg = "Không thêm dữ liệu vào được!";
                #endregion
            }
            else
            {
                #region Update
                var eff = DBLibs.ExecuteNonQuery($@"
                UPDATE DotThi 
                SET 
                    TenDotThi = N'{_tendotthi.Trim().Replace("'", "''")}' , -- TenDotThi - nvarchar(250)
	                ThoiHanNop = '{DateTime.ParseExact(_thoihannop, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")}' , -- ThoiHanNop - date
	                ThoiHanCham = '{DateTime.ParseExact(_thoihancham, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")}' , -- ThoiHanCham - date
	                CacMonThi = N'{strMon.Trim().Replace("'", "''")}' , -- CacMonThi - nvarchar(500)
	                DiemChenh = {_diemchenhlech} , -- DiemChenh - tinyint
	                TheLe = N'{_thele.Trim().Replace("'", "''")}' , -- TheLe - nvarchar(500)
	                GioiThieu = N'{_gioithieu.Trim().Replace("'", "''")}' , -- GioiThieu - nvarchar(500)
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
        public ActionResult Edit(int id)
        {
            #region Get item infos
            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM DotThi WHERE id = {id}", _cnn);
            ViewBag.Infos = dtInfos.Rows[0];
            #endregion
            
            ViewBag.IdDm = id;
            return View();
        }
    }
}