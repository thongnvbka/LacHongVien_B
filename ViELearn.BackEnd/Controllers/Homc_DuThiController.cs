using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ViELearn.BackEnd.Controllers
{
    public class Homc_DuThiController : Controller
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: Homc_DuThi
        public ActionResult Index(int page = 1, int pageSize = 20)
        {
            var dt = DBLibs.GetDataBy_DataAdapter("  SELECT TOP 1000 Id, Name, Phone,Email, Fax, City, [File] from dbo.homc_DuThi ORDER BY Id DESC", _cnn);

            var dtNews = dt.AsEnumerable();

            var dtNewsInfo = dtNews.ToPagedList(page, pageSize);

            ViewBag.dsDuthi = dtNewsInfo;

            return View();
        }

        public JsonResult Delete(string id = "0")
        {
            var stt = false;
            var msg = "";

            if (id != "" && id != "0" && int.Parse(id) > 0)
            {
                var sql = $"DELETE dbo.homc_DuThi WHERE id = {id}";
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
    }
}