using Microsoft.AspNet.Identity.Owin;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using ViElearn.Services;
using ViELearn.DAL.ModulesDAL;
using ViELearn.Models.ProjectModels;
using ViELearn.BackEnd.Ultilities;
using System.Configuration;
using System.Data;
using System.IO;

namespace ViELearn.BackEnd.Controllers
{

    public class DanhMucChungController : Controller
    {

        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            ViewBag.Title = "Danh mục ";
            var dtLoaiDm = DsLoaiDanhMuc();
            //foreach (DataRow dr in dtLoaiDm.Rows)
            //{
            //    if (Request["t"].ToString() == dr["id"].ToString())
            //    {
            //        ViewBag.Title += dr["tieuDe"];
            //        break;
            //    }
            //}
            ViewBag.DsLoaiDanhMuc = dtLoaiDm;
            var model = DanhMucChungServices.Instance.GetDanhMucChung(Request["t"].MapInt());
            ViewBag.Type = Request["t"];
            return View(model);
        }
        public ActionResult IndexSort()
        {
            ViewBag.Title = "Sắp xếp ";
            var dtLoaiDm = DsLoaiDanhMuc();
            //foreach (DataRow dr in dtLoaiDm.Rows)
            //{
            //    if (Request["t"].ToString() == dr["id"].ToString())
            //    {
            //        ViewBag.Title += dr["tieuDe"];
            //        break;
            //    }
            //}
            ViewBag.DsLoaiDanhMuc = dtLoaiDm;
            var model = DanhMucChungServices.Instance.GetDanhMucChung(Request["t"].MapInt());
            ViewBag.Type = Request["t"];
            return View(model);
        }
        public ActionResult IndexSortChild(int parentId = 0)
        {
            var sql = $@"SELECT TOP 1 id,TenDanhMuc,MaDanhMuc,SoThuTu,TrangThai FROM dbo.DanhMucChung Where Type <> 0 AND id = {parentId} AND LoaiDanhMuc=1";
            var dtCat = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            var  DsCat = dtCat.Rows[0]["TenDanhMuc"];
            ViewBag.Title = DsCat;
            var dtLoaiDm = DsLoaiDanhMuc();
            ViewBag.DsLoaiDanhMuc = dtLoaiDm;
            var loai = Request["t"].MapInt();
            var model = DanhMucChungServices.Instance.GetDanhMucChung(loai);
            // ViewBag.Category= DanhMucChungServices.Instance.
            ViewBag.Type = Request["t"];
            ViewBag.parentId = Request["parentId"];
            return View(model);
        }
        public ActionResult Create(int type = 0)
        {
            ViewBag.Title = "Thêm danh mục ";
            var dtLoaiDm = DsLoaiDanhMuc();
            foreach (DataRow dr in dtLoaiDm.Rows)
            {
                if (Request["t"].ToString() == dr["id"].ToString())
                {
                    ViewBag.Title += dr["tieuDe"];
                    break;
                }
            }
            var model = DanhMucChungServices.Instance.GetListDanhMucChung();
            ViewBag.DsLoaiDanhMuc = dtLoaiDm;
            //var dt = DsDanhMucCha();
            //ViewBag.DsDanhMucCha = dt;
            var sql = $@"SELECT * FROM dbo.DanhMucChung Where Type <> 0 AND LoaiDanhMuc=1 ORDER BY TenDanhMuc";
            var dtCat = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsCat = dtCat;
            ViewBag.Images = $"{Server.MapPath(@"\")}UserData\\0\\ImgCat";
            return View(model);
        }
        [HttpPost]
        public ActionResult Search(int cat, int status = 2, string title = "")
        {

            Hashtable args2 = new Hashtable();
            args2.Add("title", title);
            args2.Add("catId", cat);
            args2.Add("status", status);
            var dt = DBLibs.ExecuteStoreProcedure_Select("be_GetNewsSearch", args2, _cnn);
            ViewBag.Data = dt;
            return PartialView("Search");

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="_loaidanhmuc"></param>
        /// <param name="_danhmuccha"></param>
        /// <param name="_tendanhmuc"></param>
        /// <param name="_slug"></param>
        /// <param name="_sothutu"></param>
        /// <param name="_trangthai"></param>
        /// <param name="_type"></param> type = 1 hien thi dang img, type = 2 hien thi theo icon. 
        /// <param name="_path">Đường dẫn ảnh danh mục</param>
        /// <param name="_thumbnail">đường dẫn icon</param>
        /// <returns></returns>
        public JsonResult Save(int id = 0, int _loaidanhmuc = 0, int _danhmuccha = 0, string _tendanhmuc = "", string _slug = "", int _sothutu = 0, int _trangthai = 1, int _type = 1, string _path = "", string _thumbnail = "", string _url = "",string _template = "")
         {
            var stt = false;
            var msg = "";
            #region Phân tích request/submit form (nếu có)
            if (_tendanhmuc == null || _tendanhmuc.Trim() == "")
                msg = "Tên danh mục không được để trống";

            else
            {
                if (id < 1)
                {
                    #region Thêm danh mục
                    var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM DanhMucChung WHERE TenDanhMuc = N'{_tendanhmuc.Replace("'", "''").Trim()}'", _cnn);
                    if (exited != null && int.Parse(exited.ToString()) > 0)
                    {
                        return Json(new
                        {
                            status = false,
                            message = "Tên danh mục đã tồn tại!"
                        });
                    }
                    var exited1 = DBLibs.ExecuteScalar($"SELECT COUNT(*) FROM DanhMucChung WHERE MaDanhMuc = N'{_slug.Replace("'", "''").Trim()}'", _cnn);
                    if (exited1 != null && int.Parse(exited.ToString()) > 0)
                    {
                        return Json(new
                        {
                            status = false,
                            message = "Slug  đã tồn tại!"
                        });
                    }
                    var eff = DBLibs.ExecuteNonQuery($@"
                    INSERT INTO dbo.DanhMucChung
                    (
                        TenDanhMuc ,
                        MaDanhMuc,
                        LoaiDanhMuc ,
                        SoThuTu ,
                        TrangThai ,
                         Path,
                        idDanhMucCha ,
                        Type,                    
                        created_at,
                        Thumbnail,
                        Url,
                        Template
                    )
                    VALUES  
                    (
                        N'{_tendanhmuc.Replace("'", "''").Trim()}' , -- TenDanhMuc - nvarchar(500)
                        N'{_slug.Replace("'", "''").Trim()}' ,
                        {_loaidanhmuc} , -- LoaiDanhMuc - smallint
                        {_sothutu} , -- SoThuTu - smallint
                        {_trangthai} , -- TrangThai - bit
                        N'{_path.Replace("'", "''").Trim()}' , -- ThongTinThem - nvarchar(1500)
                        {_danhmuccha} , -- idDanhMucCha - bigint
                        {_type} , -- SoThuTu - smallint
                        {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}, -- created_at - int
                        N'{_thumbnail.Replace("'", "''").Trim()}', -- Thumbnail - nvarchar(1500)
                         N'{_url.Replace("'", "''").Trim()}',
                        N'{_template.Replace("'", "''").Trim()}'
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
                    UPDATE DanhMucChung 
                    SET 
                        TenDanhMuc = N'{_tendanhmuc.Replace("'", "''").Trim()}',
                        MaDanhMuc =N'{_slug.Replace("'", "''").Trim()}',
                        LoaiDanhMuc = {_loaidanhmuc} ,
                        SoThuTu = {_sothutu} ,
                        TrangThai = {_trangthai} ,
                        Path = N'{_path.Replace("'", "''").Trim()}' ,
                        idDanhMucCha = {_danhmuccha} ,
                        Type = {_type},
                        updated_at = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)},
                        Thumbnail = N'{_thumbnail.Replace("'", "''").Trim()}' ,
                        Url =  N'{_url.Replace("'", "''").Trim()}',
                        Template =  N'{_template.Replace("'", "''").Trim()}'
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

        public JsonResult UpdateTenDanhMuc(string tenDm = "", int id = 0)
       {
            var stt = false;
            var msg = "";
            #region Update danh muc
            var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE DanhMucChung 
                    SET 
                        TenDanhMuc = N'{tenDm.Replace("'", "''").Trim()}'
                   
                    WHERE
                        id = {id}", _cnn);
            if (eff > 0) stt = true;
            else
                msg = "Không cập nhật dữ liệu được!";
            #endregion
            return Json(new
            {
                status = stt,
                message = msg
            });
        }
        public ActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Sửa danh mục";
            var dtLoaiDm = DsLoaiDanhMuc();
            foreach (DataRow dr in dtLoaiDm.Rows)
            {
                if (id.ToString() == dr["id"].ToString())
                {
                    ViewBag.Title += dr["tieuDe"];
                    break;
                }
            }

            var sql = $@"SELECT * FROM dbo.DanhMucChung Where Type <> 0 AND LoaiDanhMuc=1 ORDER BY TenDanhMuc";
            var dtCat = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsCat = dtCat;
            ViewBag.DsLoaiDanhMuc = dtLoaiDm;
            #region Get item infos
            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM DanhMucChung WHERE id = {id}", _cnn);
            ViewBag.ItemInfos = dtInfos;
            #endregion

            ViewBag.Type = Request["t"];
            ViewBag.dmId = id;
            var model = DanhMucChungServices.Instance.GetDanhMucChung(Request["t"].MapInt());
            //ViewBag.DsDanhMucCha = DsDanhMucCha();
            ViewBag.Images = $"{Server.MapPath(@"\")}UserData\\0\\ImgCat";
            return View(model);
        }
        private DataTable DsDanhMuc(string t = "")
        {
            var args = new Hashtable
            {
                { "loai", t=Request["t"] }
            };
            var dt = DBLibs.ExecuteStoreProcedure_Select("GetDanhMuc", args, _cnn);
            return dt;
        }

        private DataTable DsLoaiDanhMuc()
        {
            return DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM LoaiDanhMuc ", _cnn);
        }

        private DataTable DsDanhMucCha()
        {
            var where = " WHERE 1=1 ";
            if (Request["t"] == "2")
                where += " AND LoaiDanhMuc = 1";
            if (Request["t"] == "4")
                where += " AND LoaiDanhMuc = 2";
            var dt = DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM DanhMucChung  {where} AND Type <> 0  ORDER BY LoaiDanhMuc, SoThuTu", _cnn);
            var cnt = dt.Rows.Count;
            return dt;
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
                        var path = Server.MapPath("~") + $"UserData/0/ImgCat/";

                        var ext = Path.GetExtension(file.FileName);
                        fName = file.FileName;// "FileDiemChuanBiDongBo" + ext;
                        fName = string.Format("{0}{1}{2}{3}_{4}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Millisecond, fName);
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
        public JsonResult CopyFile(string fName = "")
        {


            var msg = ""; var newPath = "";
            try
            {

                string fullPath = Server.MapPath("~") + fName;
                //string oldPath = string.Format("UserData/{0}/", type);
                newPath = fName.Replace("UserData/0/ImgCat", "UserData/Share/Images");
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
        public JsonResult ChangeStatus(int id = 0, int _status = 0)
        {
            var status = false;
            var message = "";
            if (_status > 1)
            {
                message = "Show chỉ có thể là 0 hoặc 1";
                return Json(new
                {
                    status,
                    message
                }, JsonRequestBehavior.AllowGet);
            }

            var dtLoaiDm = DsLoaiDanhMuc();
            foreach (DataRow dr in dtLoaiDm.Rows)
            {
                if (id.ToString() == dr["id"].ToString())
                {
                    ViewBag.Title += dr["tieuDe"];
                    break;
                }
            }

            if (id > 0)
            {
                var sql = $@"UPDATE dbo.DanhMucChung SET ShowHome = {_status} WHERE id = {id}";
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                if (eff > 0) status = true;
            }
            ViewBag.Type = Request["t"];
            return Json(new
            {
                status,
                message
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ShowMenu(int id = 0, int t = 0)
        {
            var status = false;
            var message = "";
            if (t > 1)
            {
                message = "Show chỉ có thể là 0 hoặc 1";
                return Json(new
                {
                    status,
                    message
                }, JsonRequestBehavior.AllowGet);
            }

            var dtLoaiDm = DsLoaiDanhMuc();
            foreach (DataRow dr in dtLoaiDm.Rows)
            {
                if (id.ToString() == dr["id"].ToString())
                {
                    ViewBag.Title += dr["tieuDe"];
                    break;
                }
            }

            if (id > 0)
            {
                var sql = $@"UPDATE dbo.DanhMucChung SET TrangThai = {t} WHERE id = {id}";
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                if (eff > 0) status = true;
            }
            ViewBag.Type = Request["t"];
            return Json(new
            {
                status,
                message
            }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// thay đổi type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult Delete(string id = "0")
        {
            var dtLoaiDm = DsLoaiDanhMuc();
            foreach (DataRow dr in dtLoaiDm.Rows)
            {
                if (id.ToString() == dr["id"].ToString())
                {
                    ViewBag.Title += dr["tieuDe"];
                    break;
                }
            }

            if (id != "" && id != "0" && int.Parse(id) > 0)
            {
                var sql = $@"Update dbo.DanhMucChung Set Type = 0  WHERE id = {id}";
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);

            }
            ViewBag.Type = Request["t"];
            return Json(new
            {
                message = ""
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
        [ValidateInput(false)]
        public JsonResult SaveSort(string values)
        {
            values = values.TrimEnd(';');
            string[] arrId = values.Split(';');
            foreach (var item in arrId)
            {
                int id = Convert.ToInt32(item.Split(':')[0]);
                int stt = Convert.ToInt32(item.Split(':')[1]);
                if (id > 0)
                {
                    var sql = $@"
                UPDATE dbo.DanhMucChung
                SET
	                SoThuTu = {stt} 
                WHERE
                    id = {id}";
                    var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                }
            }



            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }
    }
}