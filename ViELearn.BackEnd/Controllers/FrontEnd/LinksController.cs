using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ViELearn.BackEnd.Controllers.FrontEnd
{
    public class LinksController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: Links
        public ActionResult Index()
        {
            var dt = DBLibs.GetDataBy_DataAdapter("SELECT TOP 1000 Id,Title,Col,Url,Status FROM link ORDER BY Id", _cnn);

            ViewBag.dsLink = dt;

            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(int id = 0)
        {
            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 Id,Title,Col,Url,Status FROM link WHERE Id = {id} ORDER BY Id ", _cnn);
            ViewBag.ItemInfos = dtInfos;
            ViewBag.Id = id;
            return View();
        }

        public JsonResult Save(int id = 0,string _title = "",int _col = 0, string _url = "", int _status = 0)
        {
            var stt = false;
            var msg = "";
            #region Phân tích request/submit form (nếu có)
            if (_title == null || _title.Trim() == "")
                msg = "Tiêu đề không được để trống";
            else
            {
                if (id < 1)
                {
                    #region Thêm tiêu đề
                    
                    var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM Link WHERE TenDanhMuc = N'{_title.Replace("'", "''").Trim()}'", _cnn);
                    if (exited != null && int.Parse(exited.ToString()) > 0)
                    {
                        return Json(new
                        {
                            status = false,
                            message = "Tên tiêu đề đã tồn tại!"
                        });
                    }
                    var eff = DBLibs.ExecuteNonQuery($@"
                    INSERT INTO dbo.Link
                    (
                        Title ,
                        Col,
                        Url ,
                        Status 
                    )
                    VALUES  
                    (
                        N'{_title.Replace("'", "''").Trim()}', -- Tieu De - nvarchar(500)
                        {_col},
                       N'{_url.Replace("'", "''").Trim()}', -- Đường dẫn - smallint
                        {_status} -- Trang thai - smallint
                  
                    )", _cnn);
                    if (eff > 0) stt = true;
                    else
                        msg = "Không thêm dữ liệu vào được!";
                    #endregion
                }
                else
                {
                    #region Update danh muc
                    var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE Link 
                    SET 
                        Title = N'{_title.Replace("'", "''").Trim()}',
                        Col = {_col} ,
                        Url =  N'{_url.Replace("'", "''").Trim()}' ,
                        Status = {_status} 
           
                    WHERE
                        Id = {id}", _cnn);
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

        public JsonResult Delete(string id = "0")
        {
            var stt = false;
            var msg = "";

            if (id != "" && id != "0" && int.Parse(id) > 0)
            {
                var sql = $"DELETE dbo.link WHERE id = {id}";
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                if (eff > 0) stt = true;
                else
                    msg = "Không xóa dữ liệu được!";
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
        public JsonResult ChangeStatus(int id = 0, int _status = 0)
        {
            var status = false;
            var message = "";
            if (_status > 1)
            {
                message = "Status chỉ có thể là 0 hoặc 1";
                return Json(new
                {
                    status,
                    message
                }, JsonRequestBehavior.AllowGet);
            }
            if (id > 0)
            {
                var sql = $@"UPDATE dbo.Link SET Status = {_status} WHERE Id = {id}";
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                if (eff > 0) status = true;
            }
            //ViewBag.Type = Request["t"];
            return Json(new
            {
                status,
                message
            }, JsonRequestBehavior.AllowGet);

        }
    }

}