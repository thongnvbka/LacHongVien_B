using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViCms.DAL.ModulesDAL;
using ViCms.Models.ProjectModels;
using QuanLyVuChay.Ultilities;

namespace QuanLyVuChay.Controllers
{
    public class GalereyaImage
    {
        public string lowsrc;
        public string fullsrc;
        public string description;
        public string category;
    }
    
    public class PictureManageController : Controller
    {
        public static string userPicturePath;
        public static string userPictureNewPath;
        public static string userPictureThumbnailPath;
        public static string userPictureUploadedPath;
        public static int pageSize = 10;
        //public static string userpictureOtherPath;

        [ProjectCustomFilter(UserPermissions.QuyenXem)]
        public ActionResult Index()
        {
            CheckDefaultDirectory();            
            return View();
        }
        public PartialViewResult IndexModal(string returnLink)
        {
            CheckDefaultDirectory();
            ViewBag.returnLink = returnLink;
            return PartialView();
        }
        public void CheckDefaultDirectory()
        {
            userPicturePath = Server.MapPath(string.Format("~/UserImages/{0}/{1}/",SysBaseInfor.GetCurrentUnitId(), SysBaseInfor.GetCurrentUserId()));            
            userPictureNewPath= Server.MapPath(string.Format("~/UserImages/{0}/{1}/{2}/", SysBaseInfor.GetCurrentUnitId(), SysBaseInfor.GetCurrentUserId(),SysBaseDirectory.pictureNews));
            userPictureThumbnailPath = Server.MapPath(string.Format("~/UserImages/{0}/{1}/{2}/", SysBaseInfor.GetCurrentUnitId(), SysBaseInfor.GetCurrentUserId(), SysBaseDirectory.pictureThumbnail));
            userPictureUploadedPath = Server.MapPath(string.Format("~/UserImages/{0}/{1}/{2}/", SysBaseInfor.GetCurrentUnitId(), SysBaseInfor.GetCurrentUserId(), SysBaseDirectory.pictureUploaded));
            //userpictureOtherPath = Server.MapPath(string.Format("~/UserImages/{0}/{1}/{2}/", SysBaseInfor.GetCurrentUnitId(), SysBaseInfor.GetCurrentUserId(), SysBaseDirectory.pictureOther));

            if (!Directory.Exists(userPicturePath)) Directory.CreateDirectory(userPicturePath);
            if (!Directory.Exists(userPictureNewPath)) Directory.CreateDirectory(userPictureNewPath);
            if (!Directory.Exists(userPictureThumbnailPath)) Directory.CreateDirectory(userPictureThumbnailPath);
            if (!Directory.Exists(userPictureUploadedPath)) Directory.CreateDirectory(userPictureUploadedPath);
        }
        [HttpGet]
        public PartialViewResult UploadZoneView(int? idAlbum)
        {
            if (idAlbum != null && idAlbum.GetValueOrDefault()>0)
            {
                ViewBag.idAlbum = idAlbum.GetValueOrDefault();
                return PartialView("~/Views/PictureManage/_PictureUpload.cshtml");
            }
            else
            {
                return PartialView("~/Views/Shared/_blank.cshtml");
            }            
        }
        [HttpGet]
        public PartialViewResult TabDefaultGalleryView(string dirSelect)
        {
            if (!string.IsNullOrEmpty(dirSelect))
            {
                int idAlbum = 0;
                if (int.TryParse(dirSelect, out idAlbum))
                {
                    var ctrlUI = new PictureUserDAL();
                    var lstImg = ctrlUI.GetLstByUnitAndUser(SysBaseInfor.GetCurrentUnitId().ToInt32(), SysBaseInfor.GetCurrentUserId(), idAlbum).Select(item => new GalereyaImage
                    {
                        lowsrc = item.PictureUrl,
                        fullsrc = item.PictureUrl,
                        description = "Ngày tải: " + item.CreatedAt.ToString("dd/MM/yyyy"),
                        category = item.IdAlbum.ToString()
                    }).ToArray();
                    ViewBag.lstImgTabDefault = lstImg;
                    ViewBag.dirSelect = dirSelect;
                    return PartialView("~/Views/PictureManage/_PictureDefaultGallery.cshtml");
                }
                else
                {
                    var uriImage = string.Format("/UserImages/{0}/{1}/{2}/", SysBaseInfor.GetCurrentUnitId(), SysBaseInfor.GetCurrentUserId(), dirSelect);
                    DirectoryInfo currentDir = new DirectoryInfo(userPicturePath + "/" + dirSelect + "/");
                    var lstImg = currentDir.GetFiles().OrderBy(fi => fi.CreationTime).Select(fi => new GalereyaImage
                    {
                        lowsrc = Url.Content(uriImage + Path.GetFileName(fi.Name)),
                        fullsrc = Url.Content(uriImage + Path.GetFileName(fi.Name)),
                        description = "Ngày tải: " + fi.CreationTime.ToString("dd/MM/yyyy"),
                        category = dirSelect
                    }).ToArray();
                    ViewBag.lstImgTabDefault = lstImg;
                    ViewBag.dirSelect = dirSelect;
                    return PartialView("~/Views/PictureManage/_PictureDefaultGallery.cshtml");
                }
            }
            else
            {
                return PartialView("~/Views/Shared/_blank.cshtml");
            }
        }

        [HttpPost]
        public JsonResult GetListImageByDirectory(string dirSelect,int? page)
        {
            try
            {
                var lstImg = new List<GalereyaImage>();
                if (page != null && page.GetValueOrDefault() > 0)
                {
                    var currentPage = page.GetValueOrDefault();
                    var uriImage = string.Format("/UserImages/{0}/{1}/{2}/", SysBaseInfor.GetCurrentUnitId(), SysBaseInfor.GetCurrentUserId(), dirSelect);
                    DirectoryInfo currentDir = new DirectoryInfo(userPicturePath + "/" + dirSelect + "/");
                    lstImg = currentDir.GetFiles().OrderBy(fi => fi.CreationTime).Skip(currentPage*pageSize).Take(pageSize).Select(fi => new GalereyaImage
                    {
                        lowsrc = Url.Content(uriImage + Path.GetFileName(fi.Name)),
                        fullsrc = Url.Content(uriImage + Path.GetFileName(fi.Name)),
                        description = "Ngày tải: " + fi.CreationTime.ToString("dd/MM/yyyy"),
                        category = dirSelect
                    }).ToList();
                }
                return Json(lstImg);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult GetListAlum()
        {
            try
            {
                var ctrlPUA = new PictureUserAlbumDAL();
                var lstAlbum = ctrlPUA.GetListUserAlbumByUnitAndUser(SysBaseInfor.GetCurrentUnitId().ToInt32(),SysBaseInfor.GetCurrentUserId());
                return Json(lstAlbum);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult SaveListAlbum(List<PictureUserAlbum> lstListAlbum)
        {
            try
            {
                var ctrlPUA = new PictureUserAlbumDAL();
                if (lstListAlbum != null && lstListAlbum.Count > 0)
                {
                    foreach (PictureUserAlbum item in lstListAlbum)
                    {
                        item.IdUser = SysBaseInfor.GetCurrentUserId();
                        item.IdUnit = SysBaseInfor.GetCurrentUnitId().ToInt32();
                        item.CreatedAt = DateTime.Now;
                        item.Active = true;
                        ctrlPUA.CreateNewOrUpdate(item);
                    }
                }
                return Json("Success");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult DeleteListAlbum(List<int> lstListDetele)
        {
            try
            {
                var ctrlPUA = new PictureUserAlbumDAL();
                if (lstListDetele != null && lstListDetele.Count > 0)
                {
                    foreach (int id in lstListDetele)
                    {
                        ctrlPUA.UpdateItemByCommand("Id",id,"Active",0);
                    }
                }
                return Json("Success");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        [HttpPost]
        public JsonResult UploadDropZone(int? idAlbum)
        {            
            
            string urlImage = "";
            try
            {
                if (idAlbum != null && idAlbum.GetValueOrDefault() > 0)
                {
                    var ctrlUI = new PictureUserDAL();
                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[fileName];
                        
                        if (file != null && file.ContentLength > 0)
                        {
                            var strDateTime = DateTime.Now.ToString("dd-MM-yyyy");
                            var fName = idAlbum + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
                            urlImage = string.Format("/UserImages/{0}/{1}/{2}/{3}/{4}", SysBaseInfor.GetCurrentUnitId(), SysBaseInfor.GetCurrentUserId(), SysBaseDirectory.pictureUploaded, strDateTime, fName);
                            var userPath = string.Format("{0}{1}/", userPictureUploadedPath, strDateTime);
                            if (!Directory.Exists(userPath)) Directory.CreateDirectory(userPath);
                            string path = Path.Combine(userPath, fName);
                            file.SaveAs(path);

                            var pictureSave = new PictureUser();
                            pictureSave.IdAlbum = idAlbum.GetValueOrDefault();
                            pictureSave.IdUnit = SysBaseInfor.GetCurrentUnitId().ToInt32();
                            pictureSave.CreatedBy = SysBaseInfor.GetCurrentUserId();                            
                            pictureSave.CreatedAt = DateTime.Now;
                            pictureSave.PictureUrl = urlImage;
                            pictureSave.PictureName = fName;
                            ctrlUI.CreateItem(pictureSave);
                        }
                    }
                    return Json(new { urlImage = urlImage });
                }
                else
                {
                    return Json("Có lỗi khi tải lên");
                }
            }
            catch (Exception ex)
            {
                return Json("Có lỗi khi tải lên");
            }
        }
    }
}