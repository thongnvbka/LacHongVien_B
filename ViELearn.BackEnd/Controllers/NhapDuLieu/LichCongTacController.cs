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
    public class LichCongTacController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index(int t = 0)
        {
            if (t == 0)
            {
                int diff = (7 + (DateTime.Now.DayOfWeek - DayOfWeek.Monday)) % 7;
                var monday = DateTime.Now.AddDays(-1 * diff).Date;
                Response.Redirect("/LichCongTac?t=" + CLibs.DatetimeToTimestampOrgin(monday));
            }
            var sql = $@"SELECT TOP 1 * FROM LichCongTac WHERE T2DauTuan = {t}";
            var dtCalendarInfo = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            if (dtCalendarInfo != null && dtCalendarInfo.Rows.Count > 0)
                ViewBag.CalInfo = dtCalendarInfo.Rows[0];


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

            sql = $@"SELECT id, title FROM News WHERE id <> {id} AND id NOT IN (SELECT related_news FROM News WHERE id = {id}) AND PublishAt BETWEEN {CLibs.DatetimeToTimestampOrgin(DateTime.Now.AddMonths(-1))} AND {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}";
            var dtDsBaiViet = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsBaiViet = dtDsBaiViet;

            sql = $@"SELECT id, title FROM News WHERE id IN (SELECT related_news FROM News WHERE id = {id})";
            var dtRelatedNews = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsBaiVietLienQuan = dtRelatedNews;

            #region Get item infos
            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM News WHERE id = {id}", _cnn);
            ViewBag.Infos = dtInfos.Rows[0];
            #endregion

            ViewBag.Images = $"{Server.MapPath(@"\")}UserData\\{SysBaseInfor.GetIdNguoiDung()}\\{id}";
            ViewBag.Cates = Request["_cates"];
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
            int t2dautuan = 0,
            string _t2sang = "",            string _t2chieu = "",            string _t3sang = "",            string _t3chieu = "",
            string _t4sang = "",            string _t4chieu = "",            string _t5sang = "",            string _t5chieu = "",
            string _t6sang = "",            string _t6chieu = "",            string _t7sang = "",            string _t7chieu = "",
            string _cnsang = "",            string _cnchieu = "")
        {
            var stt = false;
            var msg = "";
            #region Phân tích request/submit form (nếu có)
            if (t2dautuan == 0)
                msg = "Sai thông tin";
            else
            {
                #region Thêm / cập nhật lịch công tác
                var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM LichCongTac WHERE T2DauTuan = {t2dautuan}", _cnn);
                if (exited != null && int.Parse(exited.ToString()) > 0)
                {
                    var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE LichCongTac 
                    SET 
	                    T2Sang = N'{_t2sang.Replace("'", "''")}' , -- T2Sang - ntext
	                    T2Chieu = N'{_t2chieu.Replace("'", "''")}' , -- T2Chieu - ntext
	                    T3Sang = N'{_t3sang.Replace("'", "''")}' , -- T3Sang - ntext
	                    T3Chieu = N'{_t3chieu.Replace("'", "''")}' , -- T3Chieu - ntext
	                    T4Sang = N'{_t4sang.Replace("'", "''")}' , -- T4Sang - ntext
	                    T4Chieu = N'{_t4chieu.Replace("'", "''")}' , -- T4Chieu - ntext
	                    T5Sang = N'{_t5sang.Replace("'", "''")}' , -- T5Sang - ntext
	                    T5Chieu = N'{_t5chieu.Replace("'", "''")}' , -- T5Chieu - ntext
	                    T6Sang = N'{_t6sang.Replace("'", "''")}' , -- T6Sang - ntext
	                    T6Chieu = N'{_t6chieu.Replace("'", "''")}' , -- T6Chieu - ntext
	                    T7Sang = N'{_t7sang.Replace("'", "''")}' , -- T7Sang - ntext
	                    T7Chieu = N'{_t7chieu.Replace("'", "''")}' , -- T7Chieu - ntext
	                    CnSang = N'{_cnsang.Replace("'", "''")}' , -- CnSang - ntext
	                    CnChieu = N'{_cnchieu.Replace("'", "''")}' , -- CnChieu - ntext
                        update_at = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}  -- update_at - int
                    WHERE
                        T2DauTuan = {t2dautuan}", _cnn);
                    if (eff > 0) stt = true;
                    else
                        msg = "Không cập nhật dữ liệu vào được!";
                }
                else
                {
                    var eff = DBLibs.ExecuteNonQuery($@"
                    INSERT INTO dbo.LichCongTac
                    (
	                    T2DauTuan ,
	                    T2Sang ,	                    T2Chieu ,
	                    T3Sang ,	                    T3Chieu ,
	                    T4Sang ,	                    T4Chieu ,
	                    T5Sang ,	                    T5Chieu ,
	                    T6Sang ,	                    T6Chieu ,
	                    T7Sang ,	                    T7Chieu ,
	                    CnSang ,	                    CnChieu ,
                        created_at ,                    update_at
                    )
                    VALUES  
                    (
	                    {t2dautuan} , -- T2DauTuan - int
	                    N'{_t2sang.Replace("'", "''")}' , -- T2Sang - ntext
	                    N'{_t2chieu.Replace("'", "''")}' , -- T2Chieu - ntext
	                    N'{_t3sang.Replace("'", "''")}' , -- T3Sang - ntext
	                    N'{_t3chieu.Replace("'", "''")}' , -- T3Chieu - ntext
	                    N'{_t4sang.Replace("'", "''")}' , -- T4Sang - ntext
	                    N'{_t4chieu.Replace("'", "''")}' , -- T4Chieu - ntext
	                    N'{_t5sang.Replace("'", "''")}' , -- T5Sang - ntext
	                    N'{_t5chieu.Replace("'", "''")}' , -- T5Chieu - ntext
	                    N'{_t6sang.Replace("'", "''")}' , -- T6Sang - ntext
	                    N'{_t6chieu.Replace("'", "''")}' , -- T6Chieu - ntext
	                    N'{_t7sang.Replace("'", "''")}' , -- T7Sang - ntext
	                    N'{_t7chieu.Replace("'", "''")}' , -- T7Chieu - ntext
	                    N'{_cnsang.Replace("'", "''")}' , -- CnSang - ntext
	                    N'{_cnchieu.Replace("'", "''")}' , -- CnChieu - ntext
	                    {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , -- created_at - int
	                    {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}  -- update_at - int
                    )", _cnn);
                    if (eff > 0) stt = true;
                    else
                        msg = "Không thêm dữ liệu vào được!";
                }
                #endregion
            }
            #endregion

            return Json(new
            {
                status = stt,
                message = msg
            }, JsonRequestBehavior.AllowGet);
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
                        //var path = $"{Server.MapPath(@"\")}SyncFiles\\{SysBaseInfor.GetIDTruong()}\\Diem";
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
    }
}