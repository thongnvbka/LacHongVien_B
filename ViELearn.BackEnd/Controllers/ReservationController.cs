using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ViELearn.BackEnd.Controllers
{
    public class ReservationController : Controller
    {
        private readonly String _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: Reservation
        public ActionResult Index()
        {
            var dt = DBLibs.GetDataBy_DataAdapter("SELECT * FROM dbo.Reservation ORDER BY  STT ASC", _cnn);

            ViewBag.dt = dt;

            return View();
        }
        [HttpPost]
        public ActionResult Items(string name, int status, string fromDate, string toDate)
        {

            string where = " Where Status=" + status;

            if (!string.IsNullOrEmpty(name))
                where += $" And Name like N'%{name}%'";
            if (!string.IsNullOrEmpty(fromDate))
                where += $" And FromDate >= '{fromDate}'";
            if (!string.IsNullOrEmpty(toDate))
                where += $" And FromDate <= '{toDate}'";

            var sql = $@"SELECT * FROM dbo.Reservation {where} Order By STT ASC";

            var data = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.Data = data;

            return PartialView("Items");

        }
        /// <summary>
        /// Lấy dữ liệu chuyển đổi sang Json
        /// </summary>
        /// <returns></returns>
        public ActionResult CalendarResult ()
        {

            JsonResult result = new JsonResult();
            try
            {
                // Loading.  
                var data = this.GetCalendarn();

        // Processing.  
        result = this.Json(data, JsonRequestBehavior.AllowGet);
    }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            // Return info.  
            return result;
        }

        public ActionResult CalendarReservation()
        {
            return View();
        }


        public string GetCalendarn()
        {
            var dt = DBLibs.GetDataBy_DataAdapter("SELECT * FROM dbo.Reservation ORDER BY  STT ASC", _cnn);
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                var setTime = dr["TimeSet"];
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    string dateTime = string.Format("MM/dd/yyyy {0} tt", setTime);
                    if(col.ColumnName.Equals("FromDate"))
                        row.Add(col.ColumnName,Convert.ToDateTime( dr[col]).ToString(dateTime));
                    else
                        row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }
        public JsonResult SaveSort(
           string values
        )
        {
            values = values.TrimEnd(';');
            string[] arrId = values.Split(';');
            foreach(var item in arrId)
            {
                int id = Convert.ToInt32(item.Split(':')[0]);
                int stt = Convert.ToInt32(item.Split(':')[1]);
                if (id > 0)
                {
                    var sql = $@"
                UPDATE dbo.Reservation
                SET
	                STT = {stt} 
                WHERE
                    id = {id}";
                    var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                }
            }

           

            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save(
            int id ,
            string date_from ,
            string set_time,
         string note ,
         int status
         )
        {
            var stt = false;
            var msg = "";

            if (id > 0)
            {
                var sql = $@"
                UPDATE dbo.Reservation
                SET
                       TimeSet = '{set_time.Replace("'", "''").Trim()}',
                       FromDate = '{Convert.ToDateTime(date_from)}',
	                Status = {status} , 
	                Note = N'{note.Replace("'", "''").Trim()}' 
                WHERE
                    id = {id}";
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

        public JsonResult Delete(string id = "0")
        {
            var stt = false;
            var msg = "";

            if (id != "" && id != "0" && int.Parse(id) > 0)
            {
                var sql = $"DELETE dbo.Reservation WHERE id = {id}";
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