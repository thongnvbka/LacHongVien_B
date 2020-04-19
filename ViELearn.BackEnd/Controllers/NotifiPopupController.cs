using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ViELearn.BackEnd.Ultilities;

namespace ViELearn.BackEnd.Controllers
{
    public class NotifiPopupController : Controller
    {
        // GET: NotifiPopup
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var sql = $@"SELECT * FROM dbo.NotifiPopup ";
            var dtBaiViet = DBLibs.GetDataBy_DataAdapter(sql, _cnn);

            var dtNews = dtBaiViet.AsEnumerable();

            var dtNewsInfo = dtNews.ToPagedList(page, pageSize);

            ViewBag.DsBaiViet = dtNewsInfo;





            return View();
        }
        public ActionResult Create(int type = 0)
        {

            return View();
        }
        public ActionResult Edit(int id)
        {


            #region Get item infos
            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM NotifiPopup WHERE id = {id}", _cnn);
            ViewBag.Infos = dtInfos.Rows[0];
            #endregion


            ViewBag.Cates = Request["_cates"];
            ViewBag.Id = id;
            return View();
        }

        [ValidateInput(false)]
        public JsonResult Save(
            int id = 0,

            string _title = "",
            string _DateShow = "",
            string _DateHide = "",
            string _content = "",
           
            int status = 0)

        {
            var stt = false;
            var msg = "";
            #region Phân tích request/submit form (nếu có)
            if (_title == null || _title.Trim() == "")
                msg = "Tên tiêu đề không được để trống";

            else
            {
                if (id < 1)
                {
                    #region Thêm bài viết
                
                    var newid = DBLibs.ExecuteScalar($@"
                    INSERT INTO dbo.NotifiPopup
                    (
                       
                        Title ,
                       DateShow ,
                        DateHide ,
                        FullContent ,
                        Status ,
                    
                    )
                    VALUES
                    (
                        N'{_title.Replace("'", "''").Trim()}' , 
                   
                        '{ _DateShow.MapDate().ToString("yyyy-dd-MM")}' , 
                         '{_DateHide.MapDate().ToString("yyyy-dd-MM")}' , 
                       
                        N'{_content.Replace("'", "''").Trim()}' ,
                       
                        {status}
                    
                    )  SELECT SCOPE_IDENTITY() ", _cnn);
                    int.TryParse(newid == null ? "0" : newid.ToString(), out id);
                    if (newid != null) stt = true;
                    else
                        msg = "Không thêm dữ liệu vào được!";
                    #endregion
                }
                else
                {
                    #region Update bài viết
                    var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM News WHERE id <> {id} AND (title = N'{_title.Replace("'", "''").Trim()}')", _cnn);
                    if (exited != null && int.Parse(exited.ToString()) > 0)
                    {
                        return Json(new
                        {
                            status = false,
                            message = "Đã có bài viết có cùng tiêu đề !"
                        });
                    }
                    var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE NotifiPopup 
                    SET 
                        Title = N'{_title.Replace("'", "''").Trim()}' , 
                        DateShow = '{_DateShow.MapDate().ToString("yyyy-dd-MM")}' , -- Title - nvarchar(200)
                        DateHide = '{_DateHide.MapDate().ToString("yyyy-dd-MM")}' , -- Thumbnail - nvarchar(500)
                        
                        FullContent = N'{_content.Replace("'", "''").Trim()}' , -- FullContent - ntext
                        Status = {status}
            
                    WHERE
                        id = {id}", _cnn);
                    if (eff > 0) stt = true;
                    else
                        msg = "Không cập nhật dữ liệu được!";
                    #endregion
                }

                #region Cache lại ảnh trong bài viết
                var h = new WebLibs.HttpRequest();

                foreach (Match m in Regex.Matches(_content, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    string src = m.Groups[1].Value.Trim();
                    if (src.StartsWith("/UserData/")) continue;

                    if (!src.ToLower().StartsWith(Request.Url.Host.ToLower()))
                    {
                        var path = Server.MapPath("~") + $"UserData\\{SysBaseInfor.GetIdNguoiDung()}\\{id}\\";
                        var fullpath = path + CLibs.GenPassCode(false, 12);
                        fullpath += (src.Contains(".jpg") || src.Contains(".jpeg") || src.Contains(".gif") || src.Contains(".png") ? src.Substring(src.LastIndexOf('.')).Trim() : ".jpg");
                        if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

                        var base64Data = Regex.Match(src.Replace("&amp;", "&"), @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                        var binData = Convert.FromBase64String(base64Data);
                        using (var stream = new MemoryStream(binData))
                        {
                            Bitmap bitmap = new Bitmap(stream);
                            bitmap.Save(fullpath);
                        }

                        var dest = fullpath.Replace(Server.MapPath("~"), "\\").Replace("\\", "/");
                        _content = _content.Replace(src, dest);
                    }
                }

                foreach (Match m in Regex.Matches(_content, "<input.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    string src = m.Groups[1].Value.Trim();
                    if (src.StartsWith("/UserData/")) continue;

                    if (!src.ToLower().StartsWith(Request.Url.Host.ToLower()))
                    {
                        var path = Server.MapPath("~") + $"UserData\\{SysBaseInfor.GetIdNguoiDung()}\\{id}\\";
                        var fullpath = path + CLibs.GenPassCode(false, 12);
                        fullpath += (src.Contains(".jpg") || src.Contains(".jpeg") || src.Contains(".gif") || src.Contains(".png") ? src.Substring(src.LastIndexOf('.')).Trim() : ".jpg");
                        if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

                        var base64Data = Regex.Match(src.Replace("&amp;", "&"), @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                        var binData = Convert.FromBase64String(base64Data);
                        using (var stream = new MemoryStream(binData))
                        {
                            Bitmap bitmap = new Bitmap(stream);
                            bitmap.Save(fullpath);
                        }
                        var dest = fullpath.Replace(Server.MapPath("~"), "\\").Replace("\\", "/");
                        _content = _content.Replace(src, dest);
                    }
                }
                foreach (Match m in Regex.Matches(_content, "<img.+?rel=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    string src = m.Groups[1].Value.Trim();
                    if (src.StartsWith("/UserData/")) continue;

                    if (!src.ToLower().StartsWith(Request.Url.Host.ToLower()))
                    {
                        var path = Server.MapPath("~") + $"UserData/{SysBaseInfor.GetIdNguoiDung()}/{id}/";
                        var fullpath = path + CLibs.GenPassCode(false, 12);
                        fullpath += (src.Contains(".jpg") || src.Contains(".jpeg") || src.Contains(".gif") || src.Contains(".png") ? src.Substring(src.LastIndexOf('.')).Trim() : ".jpg");
                        if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

                        var base64Data = Regex.Match(src.Replace("&amp;", "&"), @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                        var binData = Convert.FromBase64String(base64Data);
                        using (var stream = new MemoryStream(binData))
                        {
                            Bitmap bitmap = new Bitmap(stream);
                            bitmap.Save(fullpath);
                        }
                        var dest = fullpath.Replace(Server.MapPath("~"), "\\").Replace("\\", "/");
                        _content = _content.Replace(src, dest);
                    }
                }
                foreach (Match m in Regex.Matches(_content, "<img.+?data-original=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    string src = m.Groups[1].Value.Trim();
                    if (src.StartsWith("/UserData/")) continue;

                    if (!src.ToLower().StartsWith(Request.Url.Host.ToLower()))
                    {
                        var path = Server.MapPath("~") + $"UserData/{SysBaseInfor.GetIdNguoiDung()}/{id}/";
                        var fullpath = path + CLibs.GenPassCode(false, 12);
                        fullpath += (src.Contains(".jpg") || src.Contains(".jpeg") || src.Contains(".gif") || src.Contains(".png") ? src.Substring(src.LastIndexOf('.')).Trim() : ".jpg");
                        if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

                        var base64Data = Regex.Match(src.Replace("&amp;", "&"), @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                        var binData = Convert.FromBase64String(base64Data);
                        using (var stream = new MemoryStream(binData))
                        {
                            Bitmap bitmap = new Bitmap(stream);
                            bitmap.Save(fullpath);
                        }
                        var dest = fullpath.Replace(Server.MapPath("~"), "\\").Replace("\\", "/");
                        _content = _content.Replace(src, dest);
                    }
                }
                DBLibs.ExecuteNonQuery($"UPDATE News SET FullContent = N'{_content.Replace("'", "''").Trim()}' WHERE id = {id}", _cnn);
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
        public JsonResult Delete(string id = "0")
        {
            var stt = false;
            var msg = "";

            if (id != "" && id != "0" && int.Parse(id) > 0)
            {
                var sql = $"DELETE dbo.News WHERE id = {id}";
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