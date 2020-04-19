using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using ViELearn.BackEnd.Ultilities;

namespace ViELearn.BackEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //[AllowAnonymous]
       
        public ActionResult Index()
        {

            var countNews = DBLibs.ExecuteScalar($"SELECT COUNT(*) AS count FROM  dbo.News", _cnn);
            ViewBag.countNews = countNews;

            var countDm = DBLibs.ExecuteScalar($"SELECT COUNT(*) AS count FROM  dbo.DanhMucChung WHERE LoaiDanhMuc = 1", _cnn);
            ViewBag.countDm = countDm;

            var countImages = DBLibs.ExecuteScalar($"SELECT COUNT(*) AS count FROM  dbo.Images WHERE page_kind_appear = 3", _cnn);
            ViewBag.countImages = countImages;

            var countVideo = DBLibs.ExecuteScalar($"SELECT COUNT(*) AS count FROM  dbo.Images WHERE page_kind_appear = 5", _cnn);
            ViewBag.countVideo = countVideo;

            var countUser = DBLibs.ExecuteScalar($"SELECT COUNT(*) AS count FROM  dbo.AspNetUsers", _cnn);
            ViewBag.countUser = countUser;

            return View();
        }

       
    }
}