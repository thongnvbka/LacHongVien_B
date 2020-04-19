using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using ViELearn.BackEnd.Ultilities;
using System.Collections;

namespace ViElearn.BackEnd.Controllers
{
    public class TagsManagerController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index(int type = 1, int show = 0)
        {
            ViewBag.Title = "Danh sách Tag ";
            ViewBag.type = type;
            var dtLoaiDm = DsLoaiDanhMuc();
            //foreach(DataRow dr in dtLoaiDm.Rows)
            //{
            //    if (Request["t"] != null && Request["t"].ToString() == dr["id"].ToString())
            //    {
            //        ViewBag.Title += dr["name"];
            //        break;
            //    }
            //}
            //var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM dbo.TagNews tnJOIN dbo.News nn ON tn.NewsId = nn.Id",_cnn);
            ViewBag.DsLoaiDanhMuc = dtLoaiDm;
            
            return View();
        }
        [HttpPost]
        public ActionResult FilterTag(int type = 1, int show = 0, string name = "")
        {

            Hashtable args2 = new Hashtable();
            args2.Add("type", type);
            args2.Add("show_athome", show);
            args2.Add("name", name);
            var dt = DBLibs.ExecuteStoreProcedure_Select("be_GetFilterTag", args2, _cnn);
            ViewBag.Data = dt;
            return PartialView("FilterTag");

        }
        public ActionResult Create(int type = 0)
        {
            ViewBag.Title = "Thêm Tag ";
            var dtLoaiDm = DsLoaiDanhMuc();
            foreach (DataRow dr in dtLoaiDm.Rows)
            {
                if (Request["t"].ToString() == dr["id"].ToString())
                {
                    ViewBag.Title += dr["name"];
                    break;
                }
            }

            ViewBag.DsLoaiDanhMuc = dtLoaiDm;
            ViewBag.DsDanhMucCha = DsDanhMucCha(0);
            return View();
        }
        public ActionResult Edit(int id = 0, int t = 0)
        {
            ViewBag.Title = "Sửa Tag ";
            var dtLoaiDm = DsLoaiDanhMuc();
            foreach (DataRow dr in dtLoaiDm.Rows)
            {
                if (t.ToString() == dr["id"].ToString())
                {
                    ViewBag.Title += dr["name"];
                    break;
                }
            }
            ViewBag.DsLoaiDanhMuc = dtLoaiDm;
            #region Get item infos
            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM Tags WHERE id = {id}", _cnn);
            ViewBag.Infos = dtInfos.Rows[0];
            #endregion

            #region Get Top 50 Newest
            var sql = $@"
            SELECT TOP 50 * 
            FROM dbo.News 
            WHERE 
                Tags = N'{dtInfos.Rows[0]["name"].MapStr().Replace("'", "''")}' OR 
                Tags LIKE N'%{dtInfos.Rows[0]["name"].MapStr().Replace("'", "''")},%' OR 
                Tags LIKE N'%,{dtInfos.Rows[0]["name"].MapStr().Replace("'", "''")}%' OR 
                Tags LIKE N'%,{dtInfos.Rows[0]["name"].MapStr().Replace("'", "''")},%' 

                
            ORDER BY UpdatedAt DESC";
            ViewBag.Sql = sql;
            var dtTopNewest = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.TopNewest = dtTopNewest.Rows;
            var dtTopHome = DBLibs.GetDataBy_DataAdapter($@"
            SELECT TOP 10 * 
            FROM dbo.News 
            WHERE 
                show_athome = 1 AND 
                (
                    Tags = N'{dtInfos.Rows[0]["name"].MapStr().Replace("'", "''")}' OR 
                    Tags LIKE N'%{dtInfos.Rows[0]["name"].MapStr().Replace("'", "''")},%' OR 
                    Tags LIKE N'%,{dtInfos.Rows[0]["name"].MapStr().Replace("'", "''")}%' OR 
                    Tags LIKE N'%,{dtInfos.Rows[0]["name"].MapStr().Replace("'", "''")},%' 

                    
                ) 
            ORDER BY UpdatedAt DESC", _cnn);
            ViewBag.TopHome = dtTopHome.Rows;
            #endregion

            ViewBag.Type = Request["t"];
            ViewBag.Id = id;
            ViewBag.DsDanhMucCha = DsDanhMucCha();
            return View();
        }
        public JsonResult Save(
            int id = 0,
            int _loaidanhmuc = 0,
            int _danhmuccha = 0,
            string _tendanhmuc = "",
            string _dstendanhmuc = "",
            string _slug = "",
            int _home_ishow = 0,
            int _home_index = 0,
            int _cot_trangchu = 0)
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
                        #region Thêm danh mục đơn lẻ
                        var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM Tags WHERE name = N'{_tendanhmuc.Replace("'", "''").Trim()}'", _cnn);
                        if (exited != null && int.Parse(exited.ToString()) > 0)
                        {
                            return Json(new
                            {
                                status = false,
                                message = "Tên danh mục đã tồn tại!"
                            });
                        }
                        var eff = DBLibs.ExecuteNonQuery($@"
                        INSERT INTO dbo.Tags
                        (
                            name ,
                            slug ,
                            type ,
                            col_athome,
                            index_athome,
                            show_athome,
                            parent_id ,
                            created_at
                        )
                        VALUES  
                        (
                            N'{_tendanhmuc.Replace("'", "''").Trim()}' , -- TenDanhMuc - nvarchar(150)
                            N'{_slug.Replace("'", "''").Trim()}' , -- slug - varchar(150)
                            {_loaidanhmuc} , -- type - smallint
                            {_cot_trangchu} , -- col_athome - tinyint
                            {_home_index} , -- index_athome - tinyint
                            {_home_ishow} , -- show_athome - bit
                            {_danhmuccha} , -- parent_id - bigint
                            {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} -- created_at - int
                        )", _cnn);
                        if (eff > 0) stt = true;
                        else
                            msg = "Không thêm dữ liệu vào được!";
                        #endregion
                    }
                    else
                    {
                        // Thêm danh mục hàng loạt
                        foreach (string tdm in _dstendanhmuc.Split('\n'))
                        {
                            if (tdm.Trim() == "") continue;
                            var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM Tags WHERE name = N'{tdm.Replace("'", "''").Trim()}'", _cnn);
                            if (exited != null && int.Parse(exited.ToString()) > 0) continue;
                            var eff = DBLibs.ExecuteNonQuery($@"
                            INSERT INTO dbo.Tags
                            (
                                name ,
                                slug ,
                                type ,
                                col_athome ,
                                index_athome,
                                show_athome,
                                parent_id ,
                                created_at
                            )
                            VALUES  
                            (
                                N'{tdm.Replace("'", "''").Trim()}' , -- name - nvarchar(150)
                                N'{Extension.GenerateSlug(tdm.Trim()).Replace("'", "''").Trim()}' , -- slug - varchar(150)
                                {_loaidanhmuc} , -- type - smallint
                                {_cot_trangchu} , -- col_athome - tinyint
                                {_home_index} , -- index_athome - tinyint
                                {_home_ishow} , -- show_athome - bit
                                {_danhmuccha} , -- parent_id - bigint
                                {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} -- created_at - int
                            )", _cnn);
                        }
                        stt = true;
                    }
                }
                else
                {
                    #region Update danh muc
                    var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE dbo.Tags 
                    SET 
                        name = N'{_tendanhmuc.Replace("'", "''").Trim()}' , -- name - nvarchar(150)
                        slug = N'{_slug.Trim().Replace("'", "''").Trim()}' , -- slug - varchar(150)
                        type = {_loaidanhmuc} , -- type - smallint
                        col_athome = {_cot_trangchu} , -- col_athome - tinyint
                        index_athome = {_home_index} , -- index_athome - tinyint
                        show_athome = {_home_ishow} , -- show_athome - bit
                        parent_id = {_danhmuccha} , -- parent_id - bigint
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
        private DataTable DsDanhMuc()
        {
            var where = "";
            if (Request["t"] != null && Request["t"] != "") where += "WHERE c.type = " + Request["t"];
            return DBLibs.GetDataBy_DataAdapter($@"SELECT c.*, ISNULL(p.name, '') tenCha FROM Tags c LEFT JOIN Tags p ON c.parent_id = p.id {where} ORDER BY c.type, c.col_athome, c.index_athome, c.name", _cnn);
        }
        private DataTable DsLoaiDanhMuc()
        {
            return DBLibs.GetDataBy_DataAdapter($@"SELECT id, name FROM TagTypes", _cnn);
        }

        private DataTable DsDanhMucCha(int id = 0)
        {
            return DBLibs.GetDataBy_DataAdapter($@"SELECT id, name FROM Tags WHERE id <> {id} {(Request["t"] != "" ? "AND type = " + Request["t"] : "")} ORDER BY name", _cnn);
        }
        public JsonResult Delete(string id = "0")
        {
            var stt = false;
            var msg = "";

            if (id != "" && id != "0" && int.Parse(id) > 0)
            {
                var sql = $"DELETE dbo.Tags WHERE id = {id} DELETE dbo.TagNews WHERE TagsId = {id}";
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