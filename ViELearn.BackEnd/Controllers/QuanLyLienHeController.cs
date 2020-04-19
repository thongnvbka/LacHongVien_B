using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViELearn.BackEnd.Ultilities;

namespace ViELearn.BackEnd.Controllers
{
    public class QuanLyLienHeController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: QuanLyLienHe
        public ActionResult Index()
        {

            var sql = $@"SELECT * FROM dbo.Contact WHERE status= 1  ORDER BY Id";
            var dtContact = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.DsLienhe = dtContact;
            return View();
        }
        public ActionResult Create(int type = 0)
        {

            return View();
        }
        public ActionResult Edit(int id)
        {


            //var sql = $@"SELECT id, title FROM News WHERE id <> {id} AND id NOT IN (SELECT related_news FROM News WHERE id = {id}) AND PublishAt BETWEEN {CLibs.DatetimeToTimestampOrgin(DateTime.Now.AddMonths(-1))} AND {CLibs.DatetimeToTimestampOrgin(DateTime.Now)}";
            //var dtDsBaiViet = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            //ViewBag.DsBaiViet = dtDsBaiViet;
            #region Get item infos
            var dtInfos = DBLibs.GetDataBy_DataAdapter($@"SELECT * FROM Contact WHERE id = {id}", _cnn);
            ViewBag.Infos = dtInfos.Rows[0];
            #endregion
            ViewBag.Id = id;
            ViewBag.Logo = $"{Server.MapPath(@"\")}UserData\\0\\Logo";
            return View();
        }
        [ValidateInput(false)]
        public JsonResult Save(
            int id = 0,
            string _name = "",
            string _phone = "",
            string _email = "",
            string _address = "",
            string _ggmap = "",
            string _description = "",
            string _metaTitle = "",
            string _metakeyword = "",
            string _metaDescription = "",
            string _OgUrl = "",
            string _OgTitle = "",
            string _OgDescription = "",
            string _OgImage = "",
            int _OgImgWidth = 0,
            int _OgImgheight = 0,
            string _facebook = "",
            string _twitter = "",
            string _instagram = "",
            int _styleMenu = 0
           )
        {
            var stt = false;
            var msg = "";


            if (id < 1)
            {
                #region Thêm Liên hệ

                var newid = DBLibs.ExecuteScalar($@"
                    INSERT INTO dbo.Contact
                    (
                         Name,
                        phone ,
                        Email ,
                        Adderss ,
                        GoogleMap ,
                        Description ,
                        Facebook ,
                        Twitter ,
                        Instagram ,
                      
                    )
                    VALUES
                    (
                        '{_name.Replace("'", "''").Trim()}' , -- Name - varchar(250)
                         '{_phone.Replace("'", "''").Trim()}' , -- phone - nvarchar(200)
                        
                        N'{_email.Replace("'", "''").Trim()}' , -- Email - nvarchar(200)
                        N'{_address.Replace("'", "''").Trim()}' , -- address - nvarchar(500)
                        N'{_ggmap.Replace("'", "''").Trim()}' , -- ggmap - nvarchar(1000)
                        N'{_description.Replace("'", "''").Trim()}' , -- Description - ntext
                        N'{_facebook.Replace("'", "''").Trim()}' , -- Facebook - nvarchar(45)
                        N'{_twitter.Replace("'", "''").Trim()}' , -- Twitter - nvarchar(1000)
                        N'{_instagram.Replace("'", "''").Trim()}' , -- Tags - nvarchar(1000)
                     
              
                    )  SELECT SCOPE_IDENTITY() ", _cnn);
                int.TryParse(newid == null ? "0" : newid.ToString(), out id);
                if (newid != null) stt = true;
                else
                    msg = "Không thêm dữ liệu vào được!";
                #endregion
            }
            else
            {
                #region Update liên hệ

                var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE Contact 
                    SET 
                        Name = N'{_name.Replace("'", "''").Trim()}' , -- Name - varchar(250)
                        phone = N'{_phone.Replace("'", "''").Trim()}' , -- phone - nvarchar(200)
                        Email = N'{_email.Replace("'", "''").Trim()}' , -- Email - nvarchar(200)
                        Address = N'{_address.Replace("'", "''").Trim()}' , -- Address - nvarchar(500)
                        GoogleMap = N'{_ggmap.Replace("'", "''").Trim()}' ,  -- ggmap - nvarchar(1000)
                        MetaTitle = N'{_metaTitle.Replace("'", "''").Trim()}' ,  -- ggmap - nvarchar(1000)
                        Meta_Keyword = N'{_metakeyword.Replace("'", "''").Trim()}' ,  -- ggmap - nvarchar(1000)
                        Meta_Description = N'{_metaDescription.Replace("'", "''").Trim()}' ,  -- ggmap - nvarchar(1000)
                        Description = N'{_description.Replace("'", "''").Trim()}' , 
                        OgUrl = N'{_OgUrl.Replace("'", "''").Trim()}' , 
                        OgTitle = N'{_OgTitle.Replace("'", "''").Trim()}' , 
                        OgDescription = N'{_OgDescription.Replace("'", "''").Trim()}' , 
                        OgImage = N'{_OgImage.Replace("'", "''").Trim()}' , 
                        OgImgWidth = {_OgImgWidth}, 
                        OgImgHeight = {_OgImgheight}, 
                        Facebook = N'{_facebook.Replace("'", "''").Trim()}' , -- Facebook - nvarchar(45)
                        Twitter = N'{_twitter.Replace("'", "''").Trim()}' ,  -- Twitter - nvarchar(1000)
                        Instagram = N'{_instagram.Replace("'", "''").Trim()}', -- Tags - nvarchar(1000)
                        StyleMenu = {_styleMenu} -- Tags - nvarchar(1000)
                    
                       
                    WHERE
                        id = {id}", _cnn);
                if (eff > 0) stt = true;
                else
                    msg = "Không cập nhật dữ liệu được!";
                #endregion
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
                        //FileInfo fileInfo = new FileInfo(Server.MapPath($"{Server.MapPath(@"\")}UserData\\{type}\\Images"));
                        //var path = $"{Server.MapPath(@"\")}SyncFiles\\{SysBaseInfor.GetIDTruong()}\\Diem";
                        var path = Server.MapPath("~") + $"UserData/0/Logo/";

                        var ext = Path.GetExtension(file.FileName);
                        fName = file.FileName;

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
        public JsonResult CopyFile(string fName = "")
        {


            var msg = ""; var newPath = "";
            try
            {
                string fullPath = Server.MapPath("~") + fName;
                string oldPath = "UserData/0/Logo";
                newPath = fName.Replace(oldPath, "UserData/Share");
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
        public JsonResult DeleteFile(string fName = "")
        {
            var msg = "";
            try
            {
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

