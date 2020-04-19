using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ViELearn.BackEnd.Controllers
{
    public class QuanLyCommentController : Controller
    {
        private readonly String _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: QuanLyComment
        public ActionResult Index()
        {
            var sql = $@"SELECT * FROM dbo.Comment  ORDER BY Id ";
            var data = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.Data = data;
            return View();
           
        }

        public ActionResult Create()
        {
           
         

            return View();
        }

        public ActionResult Edit(int id)
        {
            var sql = $@"SELECT * FROM dbo.Comment WHERE id = {id} ";
            var data = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.Data = data.Rows[0];
            ViewBag.Id = id;
            return View();
        }

        public JsonResult Save(
          int id = 0,
       string fullName = "",
       string email = "",
    
       string content = "",
       int status = 0
       
       )
        {
            var stt = false;
            var msg = "";

            if (id < 1)
            {
                var sql = $@" INSERT INTO dbo.Comment
				         ( FullName ,
				           Email ,
				        
                           Content,
				           Status 
				          
				         )
				 VALUES  (  N'{fullName.Replace("'", "''").Trim()}' , 
				           N'{email.Replace("'", "''").Trim()}' , 
				       
				           N'{content.Replace("'", "''").Trim()}' , 
				          
				           {status}

				         ) SELECT SCOPE_IDENTITY()";
                var newid = DBLibs.ExecuteScalar(sql, _cnn);
                int.TryParse(newid == null ? "0" : newid.ToString(), out id);
                if (newid != null) stt = true;
                else
                    msg = "Không thêm dữ liệu vào được!";
            }
            else
            {
                var sql = $@"
                UPDATE dbo.Comment
                SET
	                FullName = N'{fullName.Replace("'", "''").Trim()}' , 
	                Email = N'{email.Replace("'", "''").Trim()}' , 
	            
	                Content = N'{content.Replace("'", "''").Trim()}' ,
	                Status = {status}
	                
                WHERE
                    Id = {id}";
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                if (eff > 0) stt = true;
                else
                    msg = "Không cập nhật dữ liệu được!";
            }

            return Json(new
            {
                status = stt,
                message = msg
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id = "0")
        {


            if (id != "" && id != "0" && int.Parse(id) > 0)
            {
                var sql = $@"DELETE dbo.Comment WHERE id = {id}";
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);

            }

            return RedirectToAction("Index");

        }
    }
}