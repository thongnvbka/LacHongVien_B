using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyVuChay.Ultilities;

namespace QuanLyVuChay.Controllers
{
    public class VideoManageController : Controller
    {
        [ProjectCustomFilter(UserPermissions.QuyenXem)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexModal()
        {
            return View();
        }

        public ActionResult GetListDirectoryLibrary()
        {
            var userLibrary = Server.MapPath(string.Format("~/UserImages/{0}/", SysBaseInfor.GetCurrentUserId()));
            var directories = Directory.GetDirectories(userLibrary);
            return View();
        }
    }
}