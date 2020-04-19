using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using ViELearn.DAL.ModulesDAL;
using ViELearn.Models.ProjectModels;
using ViELearn.BackEnd.Ultilities;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using ViElearn.Services;

namespace ViELearn.BackEnd.Controllers.NhapDuLieu
{
    public class CustomerController : Controller
    {
        private string message = string.Empty;
        private bool status = false;
        // GET: Customer
        public ActionResult Index()
        {
            //List<Customers> model = CustomerService.Instance.GetAllLisstCustomer();
            return View();
        }
        public ActionResult ResultSearchCustomer(string name, string phone, string fromDate, string toDate)
        {
            int dateFrom = 0; int dateTo = 0;
            if (fromDate != "" && fromDate != null)
            {
                dateFrom = CLibs.DatetimeToTimestamp(DateTime.Parse(fromDate, new CultureInfo("fr-FR")));
            }
            if (toDate != "" && toDate != null)
            {
                dateTo = CLibs.DatetimeToTimestamp(DateTime.Parse(toDate, new CultureInfo("fr-FR")).AddDays(1));
            }
            var result = CustomerService.Instance.SearhDataCustomer(name, phone, dateFrom, dateTo);
            return PartialView(result);
        }

        public JsonResult DeleteCus(int id = 0)
        {
            try
            {
                CustomerService.Instance.DeleteCus(id);
                status = true;
                message = "Bạn đã xóa khách hàng thành công";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }


            return Json(new
            {
                status = status,
                message = message
            }, JsonRequestBehavior.AllowGet);

        }
    }
}