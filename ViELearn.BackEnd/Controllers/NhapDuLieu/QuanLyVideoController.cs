using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ViELearn.BackEnd.Controllers.NhapDuLieu
{
    public class QuanLyVideoController : Controller
    {
        private readonly String _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: QuanLyVideo
        public ActionResult Index()
        {
            var sql = $@"SELECT * FROM Videos WHERE display_weekday = ',1,2,3,4,5,6,7,' AND display_time_on = 0 AND display_time_off = 0 ORDER BY order_inpage";
            var dtVideos = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            DataTable dt_default = new DataTable();
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

            #region Tách ra riêng ở trang chủ, nằm ở cột giữa - position_inpage = 6 & page_kind_appear in (1,9)
            try
            {
                var dtVideos_at_Center_Home = dtVideos.AsEnumerable()
                    .Where(x => x.Field<byte>("page_kind_appear") == (byte)1)
                    .Where(x => x.Field<Int16>("position_inpage") == 6)
                    .CopyToDataTable();
                ViewBag.VideoHomeC = dtVideos_at_Center_Home;
            }
            catch { ViewBag.VideoHomeC = dt_default; }
            #endregion


            #region tách videos nằm ở trang chủ (page_kind_appear = 1), cột bên phải (position_inpage=7)
            try
            {
                var dtVideo_at_Right_Home = dtVideos.AsEnumerable()
                    .Where(x => x.Field<byte>("page_kind_appear") == 1)
                .Where(x => x.Field<Int16>("position_inpage") == 7)
                .CopyToDataTable();
                ViewBag.VideoHomeR = dtVideo_at_Right_Home;
            }
            catch 
            {
                ViewBag.VideoHomeR = dt_default;
            }
            #endregion tách videos nằm ở trang chủ (page_kind_appear = 1), cột bên phải (position_inpage=11)

            #region Tách videos riêng ở trang chuyên mục, nằm ở cột giữa - position_inpage = 6 & page_kind_appear in (2,9)
            try
            {
                var dtVideos_at_Center_Cate = dtVideos.AsEnumerable()
                    .Where(x => x.Field<byte>("page_kind_appear") == (byte)2 || x.Field<byte>("page_kind_appear") == (byte)9)
                    .Where(x => x.Field<Int16>("position_inpage") == 6)
                    .CopyToDataTable();
                ViewBag.VideoCateC = dtVideos_at_Center_Cate;
            }
            catch { ViewBag.VideoCateC = dt_default; }
            #endregion

            #region Tách videos riêng ở trang chuyên mục, nằm ở cột phải - position_inpage = 7 & page_kind_appear in (2,9)
            try
            {
                var dtVideos_at_Right_Cate = dtVideos.AsEnumerable()
                    .Where(x => x.Field<byte>("page_kind_appear") == (byte)2 || x.Field<byte>("page_kind_appear") == (byte)9)
                    .Where(x => x.Field<Int16>("position_inpage") == 7)
                    .CopyToDataTable();
                ViewBag.VideoCateR = dtVideos_at_Right_Cate;
            }
            catch { ViewBag.VideoCateR = dt_default; }
            #endregion

            #region Tách videos riêng ở trang chi tiết, nằm ở cột giữa - position_inpage = 6 & page_kind_appear in (3,9)
            try
            {
                var dtVideos_at_Center_Detail = dtVideos.AsEnumerable()
                    .Where(x => x.Field<byte>("page_kind_appear") == (byte)3 || x.Field<byte>("page_kind_appear") == (byte)9)
                    .Where(x => x.Field<Int16>("position_inpage") == 6)
                    .CopyToDataTable();
                ViewBag.VideoDetailC = dtVideos_at_Center_Detail;
            }
            catch { ViewBag.VideoDetailC = dt_default; }
            #endregion

            #region Tách video riêng ở trang chi tiết, nằm ở cột phải - position_inpage = 7 & page_kind_appear in (3,9)
            try
            {
                var dtVideos_at_Right_Detail = dtVideos.AsEnumerable()
                    .Where(x => x.Field<byte>("page_kind_appear") == (byte)3 || x.Field<byte>("page_kind_appear") == (byte)9)
                    .Where(x => x.Field<Int16>("position_inpage") == 7)
                    .CopyToDataTable();
                ViewBag.VideoDetailR = dtVideos_at_Right_Detail;
            }
            catch { ViewBag.VideoDetailR = dt_default; }
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
                #region Thêm Video
                string sql = $@"
                INSERT INTO Videos
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
	                updated_by
                ) VALUES (
	                {page_kind} , 
	                {col_kind} , 
	                {order} , 
	                {type} , 
	                N'{main_content.Replace("'", "''").Trim()}' ,
	                N'{alter_content.Replace("'", "''").Trim()}' ,
	                N'{url.Replace("'", "''").Trim()}' , 
	                N'{title.Replace("'", "''").Trim()}' , 
	                '{weekdays.Replace("'", "''").Trim()}' ,
	                {hour_on} , 
	                {hour_off} ,
	                {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , 
	                {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} 
                )
                SELECT SCOPE_IDENTITY() ";
                var newid = DBLibs.ExecuteScalar(sql, _cnn);
                int.TryParse(newid == null ? "0" : newid.ToString(), out id);
                if (newid != null) stt = true;
                else
                    msg = "Không thêm dữ liệu vào được!";
                #endregion
            }
            else
            {
                #region Update Video
                var sql = $@"
                UPDATE Videos 
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
	                updated_by = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}  -- updated_at - int
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
                var sql = $"DELETE Videos WHERE id = {id}";
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
    }
}