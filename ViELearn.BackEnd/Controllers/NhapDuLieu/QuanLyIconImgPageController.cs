using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ViELearn.BackEnd.Ultilities;

namespace ViELearn.BackEnd.Controllers.NhapDuLieu
{
    public class QuanLyIconImgPageController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            var sql = $@"SELECT * FROM IconImgPage ORDER BY order_inpage";
            var dtIconImgPage = DBLibs.GetDataBy_DataAdapter(sql, _cnn);

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

            #endregion Tạo dữ liệu mặc định

            //#region Tách ra banner nằm ở top (cạnh logo) - position_inpage = 3
            //try
            //{
            //    var dtIconImgPager_logotop = dtIconImgPage.AsEnumerable()
            //        .Where(r => r.Field<byte>("page_kind_appear") == (byte)9)
            //        .Where(r => r.Field<Int16>("position_inpage") == 3)
            //        .CopyToDataTable();
            //    ViewBag.BannerTopR = dtIconImgPager_logotop.Rows[0];
            //}
            //catch { ViewBag.BannerTopR = dt_default; }
            //#endregion

            #region Tách ra riêng ở trang chủ, dịch vụ nổi bật - position_inpage = 1 & page_kind_appear = 1
            try
            {
                var dtIconImgPage_at_dv_noibat = FilterPage(dtIconImgPage, 1, 1);
                ViewBag.DVnoibatHome = dtIconImgPage_at_dv_noibat;
            }
            catch { ViewBag.DVnoibatHome = dt_default; }

            #endregion 

            #region Tách ra riêng ở trang chủ, người nổi tiếng - position_inpage = 2 & page_kind_appear = 1

            try
            {
                var dtIconImgPage_at_nguoinoitieng = FilterPage(dtIconImgPage, 1, 2);
                ViewBag.NguoiNoiTieng = dtIconImgPage_at_nguoinoitieng;
            }
            catch { ViewBag.NguoiNoiTieng = dt_default; }

            #endregion 

            #region Tách ra riêng ở trang chủ, Ảnh trước sau - position_inpage = 4 & page_kind_appear =1

            try
            {
                var dtIconImgPage_at_AnhTruocSau = FilterPage(dtIconImgPage, 1, 4);
                ViewBag.AnhTruocSau = dtIconImgPage_at_AnhTruocSau;
            }
            catch { ViewBag.AnhTruocSau = dt_default; }
        
            #endregion 

            #region Tách ra riêng ở trang giới thiệu, Sứ mệnh của SHT - position_inpage = 6 & page_kind_appear = 2

            try
            {
                var dtIconImgPage_at_SuMenh = FilterPage(dtIconImgPage, 2, 6);
                ViewBag.GioiThieu_Sumenh = dtIconImgPage_at_SuMenh;
            }
            catch { ViewBag.GioiThieu_Sumenh = dt_default; }

            #endregion 

            #region Tách ra riêng ở trang giới thiệu, Dịch vụ nổi bật - position_inpage = 7 & page_kind_appear = 2

            try
            {
                var dtIconImgPage_at_DVnoibat = FilterPage(dtIconImgPage, 2, 7);
                ViewBag.DVnoibat = dtIconImgPage_at_DVnoibat;
            }
            catch { ViewBag.DVnoibat = dt_default; }

            #endregion 

            #region Tách ra riêng ở trang Ngực, Vì sao lại chọn - position_inpage = 10 & page_kind_appear = 3

            try
            {
                var dtIconImgPage_at_ViSaoLaiChon = FilterPage(dtIconImgPage, 3, 10);
                ViewBag.ViSaoLaiChon = dtIconImgPage_at_ViSaoLaiChon;
            }
            catch { ViewBag.ViSaoLaiChon = dt_default; }

            #endregion 

            #region Tách ra riêng ở trang ngực, Quy trình nâng ngực - position_inpage = 11 & page_kind_appear = 3

            try
            {
                var dtIconImgPage_at_QuyTrinh = FilterPage(dtIconImgPage, 3, 11);
                ViewBag.QuyTrinh = dtIconImgPage_at_QuyTrinh;
            }
            catch { ViewBag.QuyTrinh = dt_default; }

            #endregion 



            #region Lấy danh sách các ảnh trong thư mục IconImgPage (mà người dùng upload lên)

            ViewBag.Images = $"{Server.MapPath(@"\")}UserData\\0\\IconImgPage";

            #endregion 

            return View();
        }
        


        // Lọc điều kiện ảnh trên trang và vị trí
        public DataTable FilterPage(DataTable dt,int page,int pos)
        {
            DataTable _dt = new DataTable();
            _dt = dt.AsEnumerable()
             .Where(r => r.Field<byte>("page_kind_appear") == (byte)(page))
             .Where(r => r.Field<Int16>("position_inpage") == pos)
             .CopyToDataTable();
            return _dt;
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

            #endregion Get item infos

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
                #region Thêm banner

                var newid = DBLibs.ExecuteScalar($@"
                INSERT INTO dbo.IconImgPage
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

                #endregion Thêm banner
            }
            else
            {
                #region Update bài viết

                var sql = $@"
                UPDATE IconImgPage
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

                #endregion Update bài viết
            }

            #endregion Phân tích request/submit form (nếu có)

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
                var sql = $"DELETE IconImgPage WHERE id = {id}";
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

        public JsonResult DeleteFile(string fName = "")
        {
            var msg = "";
            try
            {
                //Save file content goes here
                //fName = file.FileName;
                //var path = $"{Server.MapPath(@"\")}SyncFiles\\{SysBaseInfor.GetIDTruong()}\\Diem";
                //var path = Server.MapPath("~") + $"UserData/0/Images/";

                //var ext = Path.GetExtension(file.FileName);
                //fName = file.FileName;// "FileDiemChuanBiDongBo" + ext;
                string fullPath = Server.MapPath("~") + fName;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return Json(new
            {
                message = msg
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CopyFile(string fName = "")
        {
            var msg = ""; var newPath = "";
            try
            {
                string fullPath = Server.MapPath("~") + fName;
                newPath = fName.Replace("UserData/0/", "UserData/Share/");
                //fullDes = fullDes.Replace(Path.GetFileName(fullDes), string.Format("{0}{1}{2}{3}_{4}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Millisecond,
                //    Path.GetFileNameWithoutExtension(fullDes) + Path.GetExtension(fullDes)));
                newPath = newPath.Replace(Path.GetFileName(newPath), string.Format("{0}{1}{2}{3}_{4}", DateTime.Now.Year, DateTime.Now.Month,
                 DateTime.Now.Day, DateTime.Now.Millisecond, Path.GetFileNameWithoutExtension(newPath) + Path.GetExtension(newPath)));
                string fullDes = Server.MapPath("~") + newPath;

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Copy(fullPath, fullDes);
                    //System.IO.Directory.CreateDirectory(fullDes);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return Json(new
            {
                newpath = newPath,
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
                        var path = Server.MapPath("~") + $"UserData/0/IconImgPage/";

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