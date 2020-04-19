using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;

namespace ViElearn.BackEnd.Controllers
{
    public class TagTypesManagerController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            var dtLoaiDm = DsLoaiDanhMuc();
            if (dtLoaiDm == null)
            {
                dtLoaiDm = new DataTable();
                dtLoaiDm.Columns.Add("id");
                dtLoaiDm.Columns.Add("name");
                dtLoaiDm.Columns.Add("used");
            }

            ViewBag.DsLoaiDanhMuc = dtLoaiDm;
            return View();
        }
        public ActionResult Create(int type = 0)
        {
            return View();
        }
        public ActionResult Edit(int id)
        {
            #region Get item infos
            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM TagTypes WHERE id = {id}", _cnn);
            ViewBag.Infos = (dtInfos == null) ? new DataTable().Rows : dtInfos.Rows;
            #endregion

            ViewBag.Id = id;
            return View();
        }
        public JsonResult Save(int id = 0, string _name = "")
        {
            var stt = false;
            var msg = "";
            #region Phân tích request/submit form (nếu có)
            if (_name.Trim() == "" && _name.Trim() == "")
                msg = "Tên không được để trống";
            else
            {
                if (id < 1)
                {
                    #region Thêm danh mục
                    var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM TagTypes WHERE name = N'{_name.Replace("'", "''").Trim()}'", _cnn);
                    if (exited != null && int.Parse(exited.ToString()) > 0)
                    {
                        return Json(new
                        {
                            status = false,
                            message = "Loại danh mục đã tồn tại!"
                        });
                    }
                    var eff = DBLibs.ExecuteNonQuery($@"
                        INSERT INTO dbo.TagTypes ( name ) VALUES  
                        (
                            N'{_name.Replace("'", "''").Trim()}'  -- name - nvarchar(150)
                        )", _cnn);
                    if (eff > 0) stt = true;
                    else
                        msg = "Không thêm dữ liệu vào được!";
                    #endregion
                }
                else
                {
                    #region Update danh muc
                    var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM TagTypes WHERE id <> {id} AND name = N'{_name.Replace("'", "''").Trim()}'", _cnn);
                    if (exited != null && int.Parse(exited.ToString()) > 0)
                    {
                        return Json(new
                        {
                            status = false,
                            message = "Trùng tên với loại khác!"
                        });
                    }
                    var eff = DBLibs.ExecuteNonQuery($@"
                        UPDATE TagTypes 
                        SET 
                            name = N'{_name.Trim().Replace("'", "''").Trim()}'
                        WHERE
                            id = {id}", _cnn);
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
        private DataTable DsLoaiDanhMuc()
        {
            return DBLibs.GetDataBy_DataAdapter($@"
            SELECT
	            tt.id, tt.name, COUNT(t.id) used
            FROM dbo.TagTypes tt
	            LEFT OUTER JOIN dbo.Tags t ON tt.id = t.type
            GROUP BY
	            tt.id, tt.name", _cnn);
        }
    }
}