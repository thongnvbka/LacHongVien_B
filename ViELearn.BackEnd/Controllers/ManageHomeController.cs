using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ViELearn.BackEnd.Controllers
{
    public class ManageHomeController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: ManageHome
        public ActionResult Index()
        {
            var sql = $@"SELECT * FROM Banners WHERE display_weekday = ',1,2,3,4,5,6,7,' AND display_time_on = 0 AND display_time_off = 0 ORDER BY order_inpage";
            var dt = DBLibs.GetDataBy_DataAdapter(sql, _cnn);


            #region Tạo dữ liệu mặc định
            var dt_default = new DataTable();
            dt_default.Columns.Add("id");
            dt_default.Columns.Add("page_kind_appear");
            dt_default.Columns.Add("position_inpage");
            dt_default.Columns.Add("order_inpage");
            dt_default.Columns.Add("type");
            dt_default.Columns.Add("main_content");
            dt_default.Columns.Add("alter_content");
            dt_default.Columns.Add("url");
            dt_default.Columns.Add("tooltip");
            dt_default.Columns.Add("display_weekday");
            dt_default.Columns.Add("display_time_on");
            dt_default.Columns.Add("display_time_off");
            dt_default.Columns.Add("created_at");
            dt_default.Columns.Add("updated_at");
            var dr_default = dt_default.NewRow();
            dr_default["id"] = "0";
            dr_default["page_kind_appear"] = "0";
            dr_default["position_inpage"] = "0";
            dr_default["order_inpage"] = "0";
            dr_default["type"] = "0";
            dr_default["main_content"] = "";
            dr_default["alter_content"] = "";
            dr_default["url"] = "";
            dr_default["tooltip"] = "";
            dr_default["display_weekday"] = "";
            dr_default["display_time_on"] = "0";
            dr_default["display_time_off"] = "0";
            dr_default["created_at"] = "0";
            dr_default["updated_at"] = "0";
            dt_default.Rows.Add(dr_default);
            #endregion


            #region Tách ra Logo nằm ở top  - position_inpage = 3
            try
            {
                var dtLogotop = dt.AsEnumerable()
                    .Where(r => r.Field<byte>("page_kind_appear") == (byte)9)
                    .Where(r => r.Field<Int16>("position_inpage") == 1)
                    .CopyToDataTable();
                ViewBag.LogoTop = dtLogotop;
            }
            catch { ViewBag.LogoTop = dt_default; }
            #endregion


            //phần Benner top

            #region Tách ra riêng ở trang chủ, hiển thị trên cùng - position_inpage = 6 & page_kind_appear in (1,9)
            try
            {
                var dtBanners_at_Top_Home = dt.AsEnumerable()
                    .Where(r => r.Field<byte>("page_kind_appear") == (byte)1 || r.Field<byte>("page_kind_appear") == (byte)9)
                    .Where(r => r.Field<Int16>("position_inpage") == 1)
                    .CopyToDataTable();
                ViewBag.BannerTopC = dtBanners_at_Top_Home;
            }
            catch { ViewBag.BannerTopC = dt_default; }




            // chia cột trang chủ

            var dt_Column = DBLibs.GetDataBy_DataAdapter($@" SELECT * FROM ConfigColumns", _cnn);

       


            ViewBag.ColumnInfo = dt_Column;

            #endregion

            #region Lấy danh sách các ảnh trong thư mục banners (mà người dùng upload lên)
            ViewBag.Images = $"{Server.MapPath(@"\")}UserData\\0\\Banners";
            #endregion
            return View();
        }



     


        [ValidateInput(false)]
        public JsonResult Save(
           int id = 0,
           int page_kind = 0,
           int col_kind = 0,
           int order = 0,
           int type = 0,
           string title = "",
           string main_content = "",
           string alter_content = "",
           string url = "",
           string weekdays = "",
           int hour_on = 0,
           int hour_off = 0)
        {
            var stt = false;
            var msg = "";
            #region Phân tích request/submit form (nếu có)
            if (id < 1)
            {
                #region Thêm logo
                var newid = DBLibs.ExecuteScalar($@"
                INSERT INTO Banners
                (
	                page_kind_appear ,
	                position_inpage ,
	                order_inpage ,
	                type ,
	                main_content ,
	                alter_content ,
	                url ,
	                tooltip ,
	                display_weekday ,
	                display_time_on ,
	                display_time_off ,
	                created_at ,
	                updated_at
                ) VALUES (
	                {page_kind} , -- page_kind_appear - tinyint
	                {col_kind} , -- position_inpage - smallint
	                {order} , -- order_inpage - smallint
	                {type} , -- type - smallint
	                N'{main_content.Replace("'", "''").Trim()}' , -- main_content - nvarchar(2500)
	                N'{alter_content.Replace("'", "''").Trim()}' , -- alter_content - nvarchar(2500)
	                N'{url.Replace("'", "''").Trim()}' , -- url - nvarchar(2500)
	                N'{title.Replace("'", "''").Trim()}' , -- tooltip - nvarchar(450)
	                '{weekdays.Replace("'", "''").Trim()}' , -- display_weekday - varchar(16)
	                {hour_on} , -- display_time_on - smallint
	                {hour_off} , -- display_time_off - smallint
	                {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , -- created_at - int
	                {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}  -- updated_at - int
                )
                SELECT SCOPE_IDENTITY() ", _cnn);
                int.TryParse(newid == null ? "0" : newid.ToString(), out id);
                if (newid != null) stt = true;
                else
                    msg = "Không thêm dữ liệu vào được!";
                #endregion
            }
            else
            {
                #region Update Banners
                var sql = $@"
                UPDATE Banners 
                SET 
                    page_kind_appear = {page_kind} , -- page_kind_appear - tinyint
	                position_inpage = {col_kind} , -- position_inpage - smallint
	                order_inpage = {order} , -- order_inpage - smallint
	                type = {type} , -- type - smallint
	                main_content = N'{main_content.Replace("'", "''").Trim()}' , -- main_content - nvarchar(2500)
	                alter_content = N'{alter_content.Replace("'", "''").Trim()}' , -- alter_content - nvarchar(2500)
	                url = N'{url.Replace("'", "''").Trim()}' , -- url - nvarchar(2500)
	                tooltip = N'{title.Replace("'", "''").Trim()}' , -- tooltip - nvarchar(450)
	                display_weekday = '{weekdays.Replace("'", "''").Trim()}' , -- display_weekday - varchar(16)
	                display_time_on = {hour_on} , -- display_time_on - smallint
	                display_time_off = {hour_off} , -- display_time_off - smallint
	                created_at = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , -- created_at - int
	                updated_at = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}  -- updated_at - int
                WHERE
                    id = {id}";
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
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(string id = "0")
        {
            var stt = false;
            var msg = "";

            if (id != "" && id != "0" && int.Parse(id) > 0)
            {
                var sql = $"DELETE Banners WHERE id = {id}";
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                if (eff > 0) stt = true;
                else
                    msg = "Không cập nhật dữ liệu được!";
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

        public ActionResult UploadFile()
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
                        var path = Server.MapPath("~") + $"UserData/0/Banners/";

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

        [ValidateInput(false)]
        public JsonResult ChangeCols(
            int id = 0
        
            )
        {
            var st1 = false;
            var msg = "không thay đổi được";
            try { 
            var sql = DBLibs.ExecuteNonQuery($@"

            UPDATE dbo.ConfigColumns SET status = 0
            UPDATE dbo.ConfigColumns
             SET

               status = 1;


            WHERE id = {id}", _cnn);
            }
            catch(Exception ex)
            {
                msg = ex.Message;
            }

            return Json(new
            {
                status = st1,
                mess = msg
            },JsonRequestBehavior.AllowGet);
        }
    }
}