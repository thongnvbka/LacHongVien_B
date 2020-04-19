using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ViELearn.DAL.ModulesDAL;
using ViELearn.Models.ProjectModels;
using ViELearn.BackEnd.Ultilities;
using System.Globalization;
using System.Threading;

namespace ViELearn.BackEnd.Controllers
{
    [Authorize]
    public class BaseNewsCtrl : Controller
    {
        protected static bool isChief = false;
        protected static string blankViewUrl = "~/Views/Shared/_Blank.cshtml";
        protected string[] listDocExtension = { ".doc", ".xlsx", ".zip", ".pdf", ".xls", ".rar" };

        public PartialViewResult GetListImg(string inDir)
        {
            if (!string.IsNullOrEmpty(inDir))
            {
                var RootDir = Server.MapPath(string.Format("~/UserImages/{0}/{1}/{2}/{3}/", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.pictureNews, inDir, SysBaseInfor.GetCurrentUserId()));
                var uriImage = string.Format("~/UserImages/{0}/{1}/{2}/{3}/", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.pictureNews, inDir, SysBaseInfor.GetCurrentUserId());                
                var images = Directory.GetFiles(RootDir).Select(x => new ImageUploadView
                {
                    Url = Url.Content(uriImage + Path.GetFileName(x))
                });
                return PartialView("~/Views/NewsManage/UploadPartial/_ListImageShow.cshtml", images);
            }
            return PartialView(blankViewUrl);
        }
        public PartialViewResult GetListFileAttachment(string inDir)
        {
            if (!string.IsNullOrEmpty(inDir))
            {
                var RootDir = Server.MapPath(string.Format("~/UserImages/{0}/{1}/{2}/{3}/", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.newsAttachment, inDir, SysBaseInfor.GetCurrentUserId()));
                var uriImage = string.Format("~/UserImages/{0}/{1}/{2}/{3}/", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.newsAttachment, inDir, SysBaseInfor.GetCurrentUserId());
                var files = Directory.GetFiles(RootDir).Select(x => new FileUploadView
                {
                    Url = Url.Content(uriImage + Path.GetFileName(x)),
                    Name = Path.GetFileName(x)
                });
                return PartialView("~/Views/NewsManage/UploadPartial/_ListFileShow.cshtml", files);
            }
            return PartialView(blankViewUrl);
        }
        public PartialViewResult ImageUploadBrowser(int key)
        {
            try
            {                
                var RootDir = Server.MapPath(string.Format("~/UserImages/{0}/{1}/", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.pictureNews));
                if (!Directory.Exists(RootDir)) Directory.CreateDirectory(RootDir);

                var subDirectories = Directory.GetDirectories(RootDir);
                var lstDateTimeDir = new List<string>();
                foreach (var tmp in subDirectories) lstDateTimeDir.Add(tmp.Split('\\').LastOrDefault());
                ViewBag.lstDateTimeDir = lstDateTimeDir;
                ViewBag.lastDir = lstDateTimeDir.Last();
                return PartialView("~/Views/NewsManage/UploadPartial/ImageUploadBrowserView.cshtml");                                
            }
            catch (Exception ex)
            {
                return PartialView(blankViewUrl);
            }
        }

        public PartialViewResult FileUploadBrowser(int key)
        {
            try
            {
                var RootDir = Server.MapPath(string.Format("~/UserImages/{0}/{1}/", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.newsAttachment));
                if (!Directory.Exists(RootDir)) Directory.CreateDirectory(RootDir);

                var subDirectories = Directory.GetDirectories(RootDir);
                var lstDateTimeDir = new List<string>();
                foreach (var tmp in subDirectories) lstDateTimeDir.Add(tmp.Split('\\').LastOrDefault());
                ViewBag.lstDateTimeDir = lstDateTimeDir;
                ViewBag.lastDir = lstDateTimeDir.Last();
                return PartialView("~/Views/NewsManage/UploadPartial/FileUploadBrowserView.cshtml");
            }
            catch (Exception ex)
            {
                return PartialView(blankViewUrl);
            }
        }

        public JsonResult UploadImageCkeditor(int key, HttpPostedFileWrapper upload)
        {
            try
            {
                var userPath = Server.MapPath(string.Format("~/UserImages/{0}/{1}/{2}/{3}/", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.pictureNews, DateTime.Now.ToString("dd-MM-yyyy"), SysBaseInfor.GetCurrentUserId()));                
                if (!Directory.Exists(userPath)) Directory.CreateDirectory(userPath);
                if (upload != null)
                {
                    string imgType = "";
                    var isImages = upload.InputStream.IsImage(out imgType);
                    if (isImages && !string.IsNullOrEmpty(imgType))
                    {
                        string ImageName = Guid.NewGuid().ToString() + "." + imgType;
                        string path = Path.Combine(userPath, ImageName);                        
                        upload.SaveAs(path);
                        var uriImage = string.Format("/UserImages/{0}/{1}/{2}/{3}/{4}", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.pictureNews, DateTime.Now.ToString("dd-MM-yyyy"), SysBaseInfor.GetCurrentUserId(), ImageName);
                        return Json(uriImage);
                    }
                    return Json("Chỉ được tải lên file hình ảnh!");
                };
                return Json("Không tìm thấy file!");
            }
            catch (Exception ex)
            {
                return Json("Có lỗi xảy ra. Xin vui lòng thử lại!");
            }
        }

        public JsonResult UploadFileCkeditor(int key, HttpPostedFileWrapper upload)
        {
            try
            {                
                var userPath = Server.MapPath(string.Format("~/UserImages/{0}/{1}/{2}/{3}/", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.newsAttachment, DateTime.Now.ToString("dd-MM-yyyy"),SysBaseInfor.GetCurrentUserId()));                
                if (!Directory.Exists(userPath)) Directory.CreateDirectory(userPath);

                if (upload != null)
                {
                    string fileName = Guid.NewGuid().ToString()+"_"+upload.FileName;
                    if (Array.IndexOf(listDocExtension, Path.GetExtension(fileName)) > -1)
                    {                       
                        string path = Path.Combine(userPath, fileName);
                        upload.SaveAs(path);
                        var uriFile = string.Format("/UserImages/{0}/{1}/{2}/{3}/{4}", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.newsAttachment, DateTime.Now.ToString("dd-MM-yyyy"), SysBaseInfor.GetCurrentUserId(), fileName);
                        return Json(uriFile);
                    }
                    return Json("Chỉ được tải lên file word,excel,pdf hoặc file nén zip,rar");
                };
                return Json("Không tìm thấy file!");
            }
            catch (Exception ex)
            {
                return Json("Có lỗi xảy ra!");
            }
        }

        public ActionResult UploadDropZone()
        {
            string fName = "";
            string urlImage = "";

            try
            {
                var userPath = Server.MapPath(string.Format("~/UserImages/{0}/{1}/{2}/{3}/", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.pictureThumbnail, DateTime.Now.ToString("dd-MM-yyyy"), SysBaseInfor.GetCurrentUserId()));
                if (!Directory.Exists(userPath)) Directory.CreateDirectory(userPath);
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    string imgType = "";
                    var isImages = file.InputStream.IsImage(out imgType);
                    if (isImages && !string.IsNullOrEmpty(imgType))
                    {
                        fName = Guid.NewGuid().ToString() + "." + imgType;
                        if (file != null && file.ContentLength > 0)
                        {
                            string path = Path.Combine(userPath, fName);
                            file.SaveAs(path);
                            urlImage = string.Format("/UserImages/{0}/{1}/{2}/{3}/{4}", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.pictureThumbnail, DateTime.Now.ToString("dd-MM-yyyy"), SysBaseInfor.GetCurrentUserId(), fName);
                            return Json(new { urlImage = urlImage });
                        }
                    }
                }
                return Json("Error");
            }
            catch (Exception ex)
            {
                return Json("Error");
            }
        }

        [HttpPost, ValidateInput(false)]
        [MultipleButton(Name = "action", Argument = "CreateNews")]
        [ProjectCustomFilter(UserPermissions.QuyenThemMoi)]
        public ActionResult CreateNews(string chkType, string chkAutoUpload, News newDetail)
        {
            try
            {
                if (string.IsNullOrEmpty(newDetail.Title)) return Json(new { msg = "Bạn phải nhập tiêu đề" });
                if (string.IsNullOrEmpty(newDetail.Summary)) return Json(new { msg = "Bạn phải nhập tóm tắt" });
                if (newDetail.CategoryID <= 0) return Json(new { msg = "Bạn phải chọn chuyên mục" });
                if (newDetail.Id == 0)
                {
                    CategoriesDAL categoryCtrl = new CategoriesDAL();
                    var category = categoryCtrl.GetItemByID("Id", newDetail.CategoryID);

                    NewsDAL objCtrl = new NewsDAL();
                    newDetail.Type = !string.IsNullOrEmpty(chkType) ? (byte)1 : (byte)0;
                    newDetail.Active = 1;
                    newDetail.UnitId = SysBaseInfor.GetCurrentUnitId().ToInt32();
                    newDetail.CreatedAt = DateTime.Now;
                    newDetail.CreatedBy = SysBaseInfor.GetCurrentUserId();
                    newDetail.Status = isChief ? (byte)1 : (byte)0;
                    objCtrl.CreateItem(newDetail);

                    if (newDetail.Id > 0 && !string.IsNullOrEmpty(chkAutoUpload))
                    {
                        var tmpContent = UploadAllImageInNewsToServer(newDetail.FullContent, newDetail.Id);
                        if (!string.IsNullOrEmpty(tmpContent)) newDetail.FullContent = tmpContent;
                        objCtrl.UpdateItem(newDetail);
                    }
                    return Json(new { msg = "Success", idNews = newDetail.Id, typeAction = "Create" });
                }
                else
                    return Json(new { msg = "Thao tác không thực hiện được" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message });
            }
        }

        [HttpPost, ValidateInput(false)]
        [MultipleButton(Name = "action", Argument = "UpdateNews")]
        [ProjectCustomFilter(UserPermissions.QuyenSua)]
        public ActionResult UpdateNews(string chkType, string chkAutoUpload, News newDetail)
        {
            try
            {
                if (newDetail.Id > 0)
                {
                    if (isChief || (newDetail.CreatedBy == SysBaseInfor.GetCurrentUserId() && (newDetail.Status == 0 || newDetail.Status == 4)))
                    {
                        NewsDAL objCtrl = new NewsDAL();
                        newDetail.Type = !string.IsNullOrEmpty(chkType) ? (byte)1 : (byte)0;
                        newDetail.UpdatedAt = DateTime.Now;
                        newDetail.UpdatedBy = SysBaseInfor.GetCurrentUserId();
                        newDetail.UnitId = SysBaseInfor.GetCurrentUnitId().ToInt32();

                        if (!string.IsNullOrEmpty(chkAutoUpload))
                        {
                            var tmpContent = UploadAllImageInNewsToServer(newDetail.FullContent, newDetail.Id);
                            if (!string.IsNullOrEmpty(tmpContent)) newDetail.FullContent = tmpContent;
                        }

                        objCtrl.UpdateItem(newDetail);
                        return Json(new { msg = "Success", idNews = newDetail.Id, typeAction = "Update" });
                    }
                }
                return Json(new { msg = "Thao tác không thực hiện được" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message });
            }
        }

        [HttpPost, ValidateInput(false)]
        [MultipleButton(Name = "action", Argument = "UpdateAndCreate")]
        [ProjectCustomFilter(UserPermissions.QuyenThemMoi, UserPermissions.QuyenSua)]
        public ActionResult UpdateAndCreate(string chkType, string chkAutoUpload, News newDetail)
        {
            try
            {
                if (newDetail.Id > 0)
                {
                    if (isChief || (newDetail.CreatedBy == SysBaseInfor.GetCurrentUserId() && (newDetail.Status == 0 || newDetail.Status == 4)))
                    {
                        NewsDAL objCtrl = new NewsDAL();
                        newDetail.Type = !string.IsNullOrEmpty(chkType) ? (byte)1 : (byte)0;
                        newDetail.UpdatedAt = DateTime.Now;
                        newDetail.UpdatedBy = SysBaseInfor.GetCurrentUserId();
                        newDetail.UnitId = SysBaseInfor.GetCurrentUnitId().ToInt32();
                        if (!string.IsNullOrEmpty(chkAutoUpload))
                        {
                            var tmpContent = UploadAllImageInNewsToServer(newDetail.FullContent, newDetail.Id);
                            if (!string.IsNullOrEmpty(tmpContent)) newDetail.FullContent = tmpContent;
                        }
                        objCtrl.UpdateItem(newDetail);
                        return Json(new { msg = "Success", idNews = newDetail.Id, typeAction = "UpdateAndCreate" });
                    }
                }
                return Json(new { msg = "Thao tác không thực hiện được" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message });
            }
        }

        [HttpPost, ValidateInput(false)]
        [MultipleButton(Name = "action", Argument = "ForwardNews")]
        [ProjectCustomFilter(UserPermissions.QuyenChuyen)]
        public ActionResult ForwardNews(News newDetail)
        {
            try
            {
                //if (newDetail.Id > 0 && (newDetail.Status == 0 || newDetail.Status == 4))
                //{
                //    NewsDAL objCtrl = new NewsDAL();
                //    newDetail.Status = 1;
                //    newDetail.UpdatedAt = DateTime.Now;
                //    newDetail.UpdatedBy = SysBaseInfor.GetCurrentUserId();
                //    objCtrl.UpdateItem(newDetail, new string[] { "Status", "UpdatedAt", "UpdatedBy" });
                //    return Json(new { msg = "Success", idNews = newDetail.Id, typeAction = "Forward" });
                //}
                //else
                    return Json(new { msg = "Thao tác không thực hiện được" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message });
            }
        }

        [HttpPost, ValidateInput(false)]
        [MultipleButton(Name = "action", Argument = "AcceptNews")]
        [ProjectCustomFilter(UserPermissions.QuyenDuyet)]
        public ActionResult AcceptNews(News newDetail)
        {
            try
            {
                //if (newDetail.Id > 0 && newDetail.Status == 1)
                //{
                //    NewsDAL objCtrl = new NewsDAL();
                //    newDetail.Status = 2;
                //    newDetail.UpdatedAt = DateTime.Now;
                //    newDetail.UpdatedBy = SysBaseInfor.GetCurrentUserId();
                //    objCtrl.UpdateItem(newDetail, new string[] { "Status", "UpdatedAt", "UpdatedBy" });
                //    return Json(new { msg = "Success", idNews = newDetail.Id, typeAction = "Accept" });
                //}
                //else
                    return Json(new { msg = "Thao tác không thực hiện được" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message });
            }
        }

        [HttpPost, ValidateInput(false)]
        [MultipleButton(Name = "action", Argument = "PublishNews")]
        [ProjectCustomFilter(UserPermissions.QuyenXuatBan)]
        public ActionResult PublishNews(News newDetail)
        {
            try
            {
                //if (newDetail.Id > 0)
                //{
                //    if (newDetail.Status == 2)
                //    {
                //        NewsDAL objCtrl = new NewsDAL();
                //        newDetail.Status = 3;
                //        newDetail.UpdatedAt = DateTime.Now;
                //        newDetail.UpdatedBy = SysBaseInfor.GetCurrentUserId();
                //        objCtrl.UpdateItem(newDetail, new string[] { "Status", "UpdatedAt", "UpdatedBy" });
                //        return Json(new { msg = "Success", idNews = newDetail.Id, typeAction = "Publish" });
                //    }
                //    else
                //    if (newDetail.Status == 3)
                //    {
                //        NewsDAL objCtrl = new NewsDAL();
                //        newDetail.Status = 2;
                //        newDetail.UpdatedAt = DateTime.Now;
                //        newDetail.UpdatedBy = SysBaseInfor.GetCurrentUserId();
                //        objCtrl.UpdateItem(newDetail);
                //        return Json(new { msg = "Success", idNews = newDetail.Id, typeAction = "Publish" });
                //    }
                //}
                return Json(new { msg = "Thao tác không thực hiện được" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message });
            }
        }

        [HttpPost, ValidateInput(false)]
        [MultipleButton(Name = "action", Argument = "ReturnNews")]
        public ActionResult ReturnNews(News newDetail)
        {
            try
            {
                //if (newDetail.Id > 0 && newDetail.Status == 1)
                //{
                //    NewsDAL objCtrl = new NewsDAL();
                //    newDetail.Status = 4;
                //    newDetail.UpdatedAt = DateTime.Now;
                //    newDetail.UpdatedBy = SysBaseInfor.GetCurrentUserId();
                //    objCtrl.UpdateItem(newDetail, new string[] { "Status", "UpdatedAt", "UpdatedBy" });
                //    return Json(new { msg = "Success", idNews = newDetail.Id, typeAction = "Return" });
                //}
                //else
                    return Json(new { msg = "Thao tác không thực hiện được" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message });
            }
        }


        [HttpPost]
        [ProjectCustomFilter(UserPermissions.QuyenXoa)]
        public ActionResult DeleteNew(int idNews)
        {
            try
            {
                NewsDAL objCtrl = new NewsDAL();
                var result = objCtrl.GetItemByID("Id", idNews);
                if (isChief)
                {
                    if (result.Status != 3)
                    {
                        if (result.CreatedBy == SysBaseInfor.GetCurrentUserId())
                        {
                            objCtrl.DeleteItemByValues("Id", idNews);
                            return Content("Success");
                        }
                        else
                        {
                            if (result.Status != 0 || result.Status != 4)
                            {
                                objCtrl.DeleteItemByValues("Id", idNews);
                                return Content("Success");
                            }
                        }
                    }
                }
                else
                {
                    if (result.CreatedBy == SysBaseInfor.GetCurrentUserId() && (result.Status == 0 || result.Status == 4))
                    {
                        objCtrl.DeleteItemByValues("Id", idNews);
                        return Content("Success");
                    }
                }

                return Content("Không thể xóa");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }


        [HttpPost]
        public JsonResult GetDataNews(List<int> lstNewsGetData)
        {
            try
            {
                NewsDAL newCtrl = new NewsDAL();

                foreach (int newsId in lstNewsGetData)
                {
                    News news = newCtrl.GetItemByID("Id", newsId);
                    var tmpContent = UploadAllImageInNewsToServer(news.FullContent, news.Id);
                    if (!string.IsNullOrEmpty(tmpContent))
                    {
                        news.FullContent = tmpContent;
                        newCtrl.UpdateItem(news);
                    }
                }
                return Json(new { Msg = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { Msg = "Fail" });
            }
        }
        public string UploadAllImageInNewsToServer(string contentNews, int newsId)
        {

            try
            {
                UnitsDAL unitCtrl = new UnitsDAL();
                var unitTmp = unitCtrl.GetItemByID("Id", SysBaseInfor.GetCurrentUnitId().ToInt32());
                var mediaUrl = unitTmp.MediaUrl;
                var tmpContentNews = contentNews;
                var linkParser = new Regex(@"\b(?:https?://|www\.)[^ \f\n\r\v\t\""\'\]]+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                string link, linkNew;
                foreach (Match m in linkParser.Matches(contentNews))
                {
                    link = m.Value;
                    if (link.IndexOf(mediaUrl) < 0)
                    {
                        linkNew = UploadImageToServer(HttpUtility.HtmlDecode(link), newsId, mediaUrl);
                        if (!string.IsNullOrEmpty(linkNew)) tmpContentNews = tmpContentNews.Replace(link, linkNew);
                    }
                }
                return tmpContentNews;
            }
            catch (Exception ex)
            {
                return null;
            }
        }        
        public string UploadImageToServer(string url, int newsId, string mediaUrl)
        {
            try
            {
                string fileExtension = Path.GetExtension(url);
                var uri = new Uri(url);

                if (Array.IndexOf(listDocExtension, fileExtension) > -1)
                {
                    var strDateTime = DateTime.Now.ToString("dd-MM-yyyy");
                    var userDocumentPath = Server.MapPath(string.Format("~/UserImages/{0}/{1}/{2}/{3}/", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.newsAttachment, strDateTime, SysBaseInfor.GetCurrentUserId()));
                    if (!Directory.Exists(userDocumentPath)) Directory.CreateDirectory(userDocumentPath);
                    var fileName = Guid.NewGuid().ToString() + fileExtension;                    
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                        webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                        webClient.DownloadFile(uri, Path.Combine(userDocumentPath, fileName));
                    }
                    var urlDocument = string.Format("/UserImages/{0}/{1}/{2}/{3}/{4}", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.newsAttachment, strDateTime, SysBaseInfor.GetCurrentUserId(), fileName);
                    return mediaUrl + urlDocument;
                }
                else
                {
                    var strDateTime = DateTime.Now.ToString("dd-MM-yyyy");
                    var userPictureNewsPath = Server.MapPath(string.Format("~/UserImages/{0}/{1}/{2}/{3}/", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.pictureNews, strDateTime, SysBaseInfor.GetCurrentUserId()));
                    if (!Directory.Exists(userPictureNewsPath)) Directory.CreateDirectory(userPictureNewsPath);
                    var fileName = Path.Combine(userPictureNewsPath, Guid.NewGuid().ToString());

                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                        webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                        webClient.DownloadFile(uri, Path.Combine(userPictureNewsPath, fileName));
                    }

                    var fileStream = new FileStream(fileName, FileMode.Open);

                    string imageType;
                    var isImage = fileStream.IsImage(out imageType);
                    fileStream.Close();
                    fileStream.Dispose();
                    if (isImage && !string.IsNullOrEmpty(imageType))
                    {
                        var fName = newsId + "_" + Guid.NewGuid() + "." + imageType;
                        var urlImage = string.Format("/UserImages/{0}/{1}/{2}/{3}/{4}", SysBaseInfor.GetCurrentUnitId(), SysBaseDirectory.pictureNews, strDateTime, SysBaseInfor.GetCurrentUserId(), fName);
                        string path = Path.Combine(userPictureNewsPath, fName);
                        System.IO.File.Move(fileName, path);
                        return mediaUrl + urlImage;
                    }
                    System.IO.File.Delete(fileName);
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //initilizing culture on controller initialization
        //protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        //{
        //    base.Initialize(requestContext);
        //    if (Session["CurrentCulture"] != null)
        //    {
        //        Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["CurrentCulture"].ToString());
        //        Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["CurrentCulture"].ToString());
        //    }
        //}
        //// changing culture
        //public ActionResult ChangeCulture(string currentCulture, string returnUrl)
        //{
        //    Thread.CurrentThread.CurrentCulture = new CultureInfo(currentCulture);
        //    Thread.CurrentThread.CurrentUICulture = new CultureInfo(currentCulture);

        //    Session["CurrentCulture"] = currentCulture;
        //    return Redirect(returnUrl);
        //}
    }
}