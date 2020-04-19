using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using ViELearn.BackEnd.Ultilities;
using System.Collections;

namespace ViELearn.BackEnd.Controllers
{
    public class QuanLyAnhController : Controller
    {
        private readonly String _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        // GET: QuanLyAnh
        public ActionResult Index(int type = 1, int id = 0, int cat = 0)
        {
            
            string sql = $@"SELECT * FROM dbo.DanhMucChung Where LoaiDanhMuc=2 ";
            var dtAlbum = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsAlbum = dtAlbum;
            string query = $@"SELECT * FROM dbo.DanhMucChung Where LoaiDanhMuc=3";
            var dtVideo = DBLibs.GetDataBy_DataAdapter(query, _cnn);
            ViewBag.dsVideo = dtVideo;
            //ViewBag.Type = Request["type"];
            var qr = $@"SELECT n.id,n.page_kind_appear,n.position_inpage,n.type,n.order_inpage,n.main_content,n.alter_content,n.url,n.tooltip,n.size,n.width,
              n.height,n.AlbumId,n.GroupAlbumID, dbo.GetCountAlbumid(n.id)TenDanhMuc FROM dbo.Images n  Where page_kind_appear =" + type;
            var dtImages = DBLibs.GetDataBy_DataAdapter(qr, _cnn);
            if (dtImages == null || dtImages.Rows.Count < 1)
            {
                ViewBag.ImageHomeC = new DataTable();
            }
            else
            {
                Hashtable args = new Hashtable();
                args.Add("type", type);
                args.Add("albumId", cat);
                if (cat > 0)
                dtImages = DBLibs.ExecuteStoreProcedure_Select("be_Image_byCat", args, _cnn);
                ViewBag.dsImg = dtImages;
            }
            ViewBag.Images = $"{Server.MapPath(@"\")}UserData\\{type}\\Images";
           // var dt = DBLibs.GetDataBy_DataAdapter("SELECT id,page_kind_appear,position_inpage,type,order_inpage,main_content,alter_content,url,tooltip,size,width,height, AlbumId FROM dbo.News ORDER BY  Id desc", _cnn);
            ViewBag.alBumID = cat;
            ViewBag.Type = Request["type"];
            return View();
        }
        public ActionResult Demo(int type = 1, int id = 0, int cat = 0)
        {
            ViewBag.cat = Request["cat"].MapInt();
            string sql = $@"SELECT * FROM dbo.DanhMucChung Where LoaiDanhMuc=2 ORDER BY SoThuTu";
            var dtAlbum = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsAlbum = dtAlbum;
            string query = $@"SELECT * FROM dbo.DanhMucChung Where LoaiDanhMuc=3 ORDER BY SoThuTu";
            var dtVideo = DBLibs.GetDataBy_DataAdapter(query, _cnn);
            //ViewBag.Type = Request["type"];
            var qr = $@"SELECT n.id,n.page_kind_appear,n.position_inpage,n.type,n.order_inpage,n.main_content,n.alter_content,n.url,n.tooltip,n.size,n.width,
              n.height,n.AlbumId,n.GroupAlbumID, dbo.GetCountAlbumid(n.id)TenDanhMuc FROM dbo.Images n  Where page_kind_appear =" + type;
            var dtImages = DBLibs.GetDataBy_DataAdapter(qr, _cnn);
            if (dtImages == null || dtImages.Rows.Count < 1)
            {
                ViewBag.ImageHomeC = new DataTable();

            }
            else
            {
                Hashtable args = new Hashtable();
                args.Add("type", type);
                args.Add("albumId", cat);
                if (cat > 0)
                    dtImages = DBLibs.ExecuteStoreProcedure_Select("be_Image_byCat", args, _cnn);
                ViewBag.dsImg = dtImages;
            }
            ViewBag.Images = $"{Server.MapPath(@"\")}UserData\\{type}\\Images";
            // var dt = DBLibs.GetDataBy_DataAdapter("SELECT id,page_kind_appear,position_inpage,type,order_inpage,main_content,alter_content,url,tooltip,size,width,height, AlbumId FROM dbo.News ORDER BY  Id desc", _cnn);


            ViewBag.alBumID = cat;
            ViewBag.Type = Request["type"];

            return View();
        }
        /// <summary>
        /// Sửa nhanh tên
        /// </summary>
        /// <param name="title"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult UpdateTooltip(string tooltip = "", int id = 0)
        {
            var stt = false;
            var msg = "";
            #region Update Tên
            var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE Images
                    SET 
                        tooltip = N'{tooltip.Replace("'", "''").Trim()}'
                   
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
            int hour_off = 0,
            string groupAlbumid = "",
            double size = 0,
            int width = 0,
            int height = 0
            )
        {
            var stt = false;
            var msg = "";
            var alBumId = groupAlbumid.Split(',');
            DBLibs.ExecuteNonQuery($"Delete From GroupAlbumImg WHERE ImagId = {id}", _cnn);
            if (id <= 0)
            {
               
                    var sql = $@" INSERT INTO dbo.Images
				         ( page_kind_appear ,
				           position_inpage ,
				           order_inpage ,
                           type,
				           main_content ,
				           alter_content ,
				           url ,
				           tooltip ,
                            size,
                            width,
                            height,
				           display_weekday ,
				           display_time_on ,
				           display_time_off ,
				           created_at ,
				           updated_at,
                            GroupAlbumID,
                            AlbumId
				         )
				 VALUES  ( {page_kind} , 
				           {col_kind} ,
				           {order} , 
                           {type}, 
				           N'{main_content.Replace("'", "''").Trim()}' ,
				           N'{alter_content.Replace("'", "''").Trim()}' ,
				           N'{url.Replace("'", "''").Trim()}' ,
				           N'{title.Replace("'", "''").Trim()}' , 
                            {size},
                            {width},
                             {height},
				           '{weekdays.Replace("'", "''").Trim()}' ,
				           {hour_on} , 
				           {hour_off} ,
				           {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , 
				           {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}, 
				           N'{groupAlbumid.Replace("'", "''").Trim()}' ,
                            0
				            ) SELECT SCOPE_IDENTITY()";
                        var newid = DBLibs.ExecuteScalar(sql, _cnn);
                        int.TryParse(newid == null ? "0" : newid.ToString(), out id);
                        if (newid != null) stt = true;
                        else
                            msg = "Không thêm dữ liệu vào được!";
                for (int i = 0; i < alBumId.Length; i++)
                {
                    var qr = $@"INSERT INTO GroupAlbumImg (ImagId,AlbumId) VALUES ({id},{alBumId[i]})";
                    var eff1 = DBLibs.ExecuteScalar(qr, _cnn);
                }
           
            }
            else
            {
               
                    var sql = $@"
                UPDATE dbo.Images
                SET
                    page_kind_appear = {page_kind} ,
	                position_inpage = {col_kind} , 
	                order_inpage = {order} , 
	                type = {type} ,
	                main_content = N'{main_content.Replace("'", "''").Trim()}' ,
	                alter_content = N'{alter_content.Replace("'", "''").Trim()}' , 
	                url = N'{url.Replace("'", "''").Trim()}' , 
	                tooltip = N'{title.Replace("'", "''").Trim()}' ,
	                display_weekday = '{weekdays.Replace("'", "''").Trim()}' ,
	                display_time_on = {hour_on} ,
	                display_time_off = {hour_off} , 
	                created_at = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)} , 
	                updated_at = {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}, 
	                GroupAlbumID = N'{groupAlbumid.Replace("'", "''").Trim()}' ,
                    AlbumId = 0
                WHERE
                    id = {id}";
               
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                if (eff > 0) stt = true;
                else
                    msg = "Không cập nhật dữ liệu được!";
                for (int i = 0; i < alBumId.Length; i++)
                {
                    var qr = $@"INSERT INTO GroupAlbumImg (ImagId,AlbumId) VALUES ({id},{alBumId[i]})";
                    var eff1 = DBLibs.ExecuteScalar(qr, _cnn);
                }
            }
            return Json(new
            {
                status = stt,
                message = msg
            }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult AddEditDanhMucChungPartial(int id = 0)
        {
            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT TOP 1 * FROM DanhMucChung WHERE id = {id}", _cnn);
            ViewBag.ItemInfos = dtInfos;
            ViewBag.DsLoaiDanhMuc = DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM LoaiDanhMuc ", _cnn);
            var dtCat = DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM DanhMucChung WHERE  LoaiDanhMuc = 2 ORDER BY LoaiDanhMuc, SoThuTu", _cnn); ;
            ViewBag.DsDanhMucCha = dtCat;
            //home.listCatetop = HomeService.Instance.GetListcateNews();

            return PartialView();
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
        /// <summary>
        /// Lưu thay đổi vị trí
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public JsonResult SaveSort(
         string values
  )
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
        public JsonResult Delete(string id = "0",int albumId = 0)
        {
            var stt = false;
            var msg = "";

            if (id != "" && id != "0" && int.Parse(id) > 0)
            {
                string strDelete = $"DELETE dbo.GroupAlbumImg WHERE ImagId = {id}";
                var efd = DBLibs.ExecuteNonQuery(strDelete, _cnn);
                var sql = $"DELETE dbo.Images WHERE id = {id}";
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
        public JsonResult DeleteAlbum(string id = "0")
        {
            var stt = false;
            var msg = "";
            var exited = DBLibs.ExecuteScalar($"SELECT COUNT(*)  FROM dbo.DanhMucChung d JOIN dbo.GroupAlbumImg gi ON d.id = gi.AlbumId JOIN dbo.Images n ON gi.ImagId = n.id WHERE d.id ="+id, _cnn);
            if (int.Parse(exited.ToString()) == 0)
            {
                if (id != "" && id != "0" && int.Parse(id) > 0)
                {

                    var sql = $"DELETE dbo.DanhMucChung WHERE id = {id}";
                    var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                    if (eff > 0) stt = true;
                    else
                        msg = "Không cập nhật dữ liệu được!";
                }
                else
                {
                    msg = "ID rỗng hoặc = 0";
                }
            }
            else
            {
                msg = "Đã có ảnh trong Album, nếu muốn xóa album bạn phải xóa hết ảnh trong album này!";
            }
        
            return Json(new
            {
                status = stt,
                message = msg
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReFreshData()
        {
            var stt = false;
            var msg = "";
            string qr = $@"SELECT * FROM dbo.Images Where page_kind_appear = 1 OR page_kind_appear = 3";
            var dtImg = DBLibs.GetDataBy_DataAdapter(qr, _cnn);

            try
            {
                foreach (DataRow dr in dtImg.Rows)
                {
                    var url = dr["main_content"].ToString();
                    int id = dr["id"].MapInt();
                    string fullDes = Server.MapPath("~") + url;
                    var path = string.Format(fullDes);
                    FileInfo fileInfo = new FileInfo(fullDes);
                    double fileSizeKB = fileInfo.Length;
                    Image img = Image.FromFile(fullDes);
                    var width = img.Width;
                    var height = img.Height;
                    var size = fileSizeKB;
                    var sql = $@"
                UPDATE dbo.Images
                SET
            
                    size = {size},
                    width = {width},
                    height = {height}
	           Where
                    id = {id}";
                    var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                }

            }
            catch (Exception ex)
            {
            }
            stt = true;
            msg = "Cập nhật dữ liệu thành công";
            return Json(new
            {
                status = stt,
                mess = msg
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UploadFile(int type = 0)
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
                    fName = file.FileName;
                    var subfName = fName.Substring(fName.LastIndexOf(".") + 1);
                    if (file != null && file.ContentLength > 0 && subfName.Contains("jpg") || subfName.Contains("png") || subfName.Contains("jpeg")|| subfName.Contains("gif"))
                    {
                        //FileInfo fileInfo = new FileInfo(Server.MapPath($"{Server.MapPath(@"\")}UserData\\{type}\\Images"));
                        //var path = $"{Server.MapPath(@"\")}SyncFiles\\{SysBaseInfor.GetIDTruong()}\\Diem";
                        var path = Server.MapPath("~") + $"UserData/{type}/Images/";

                        var ext = Path.GetExtension(file.FileName);
                   //     fName = file.FileName;

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
                return Json(new
                {
                    Message = fName,
                });
            else
                return Json(new { Message = msg });
        }
        public JsonResult CopyFile(string fName = "", int type = 0)
        {


            var msg = ""; var newPath = ""; var size = ""; var width = ""; var height = "";
            try
            {
                string fullPath = Server.MapPath("~") + fName;
                string oldPath = string.Format("UserData/{0}/", type);
                newPath = fName.Replace(oldPath, "UserData/Share/");
                newPath = newPath.Replace(Path.GetFileName(newPath), string.Format("{0}{1}{2}{3}_{4}", DateTime.Now.Year, DateTime.Now.Month,
                    DateTime.Now.Day, DateTime.Now.Millisecond, Path.GetFileNameWithoutExtension(newPath) + Path.GetExtension(newPath)));

                string fullDes = Server.MapPath("~") + newPath;

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Copy(fullPath, fullDes);
                    //System.IO.Directory.CreateDirectory(fullDes);

                    FileInfo fileInfo = new FileInfo(fullDes);
                    double fileSizeKB = fileInfo.Length;
                    Image img = Image.FromFile(fullDes);
                    width = img.Width.ToString();
                    height = img.Height.ToString();
                    size = fileSizeKB.ToString();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new
            {
                size = size,
                width = width,
                height = height,
                newpath = newPath,
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
    }
}
