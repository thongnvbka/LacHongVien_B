using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using ViELearn.DAL.ModulesDAL;
using ViELearn.Models.ProjectModels;
using ViELearn.BackEnd.Ultilities;

namespace ViELearn.BackEnd.Controllers
{
    public class UnitReportController : Controller
    {
        protected static string blankViewUrl = "~/Views/Shared/_Blank.cshtml";
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult UnitReportDaily()
        {
            return View();
        }
        public ActionResult UnitReportWeekly()
        {
            DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek) + 1);
            DateTime endOfWeek = startOfWeek.AddDays(5);
            ViewBag.StartOfWeek = startOfWeek;
            ViewBag.EndOfWeek = endOfWeek;
            return View();
        }

        public ActionResult ShowDetailUnitReport(int reportId, int? TypeSelect)
        {
            try
            {
                UnitReportDAL unitReportCtrl = new UnitReportDAL();                
                var vnFormat = new CultureInfo("VI-vi");
                var listDayForSelect = new List<SelectListItem>();

                if (TypeSelect > 0)
                {
                    DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek) + 1);
                    DateTime endOfWeek = startOfWeek.AddDays(5);
                    for (int tmpI = (int)DateTime.Today.DayOfWeek; tmpI < 7; tmpI++ )
                    {
                        var tmpDow = DateTime.Today.AddDays(tmpI-(int)DateTime.Today.DayOfWeek);
                        listDayForSelect.Add(new SelectListItem() { Text = vnFormat.DateTimeFormat.GetDayName(tmpDow.DayOfWeek) + " " + tmpDow.ToString("dd/MM/yyyy"), Value = tmpDow.ToString() });
                    }
                }
                else
                {
                    listDayForSelect.Add(new SelectListItem() { Text = vnFormat.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek) + " " + DateTime.Now.ToString("dd/MM/yyyy"), Value = DateTime.Now.ToString("dd/MM/yyyy") });
                }
                                
                ViewBag.ListDayForSelect = new SelectList(listDayForSelect, "Value", "Text");

                var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
                ViewBag.ListGroupRoles = new SelectList(roleManager.GetListRoleByUnitIdAndType(SysBaseInfor.GetCurrentUnitCode() + SysBaseInfor.SignProject, SysBaseInfor.GetCurrentUnitId().ToInt32(), 2), "Id", "Name"); 

                var result = new UnitReport();

                if (reportId > 0)
                    result = unitReportCtrl.GetItemByID("Id", reportId);

                return PartialView("~/Views/UnitReport/_DetailUnitReport.cshtml", result);
            }
            catch (Exception ex)
            {
                return PartialView(blankViewUrl);
            }
        }

        public JsonResult SaveDetailUnitReport(UnitReport objReport)
        {
            try
            {
                UnitReportDAL objCtrl = new UnitReportDAL();
                if (objReport.DayReport != null) objReport.DayReport = objReport.DayReport.ToLocalTime();
                if (objReport.Id > 0)
                {                    
                    objReport.UpdatedBy = SysBaseInfor.GetCurrentUserId();
                    objCtrl.UpdateItem(objReport);
                    return Json(new { Msg = "Success",Type = "Edit" });
                }
                else
                {
                    objReport.Status = 0;                    
                    objReport.CreatedBy = SysBaseInfor.GetCurrentUserId();
                    objReport.UnitId = SysBaseInfor.GetCurrentUnitId().ToInt32();
                    objCtrl.CreateItem(objReport);
                    return Json(new { Msg = "Success",Type = "Create" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Msg = "Fail" });
            }
        }

        public JsonResult PublishUnitReport(int reportId, int? TypeSelect)
        {
            try
            {
                UnitReportDAL objCtrl = new UnitReportDAL();
                UnitReport obj = objCtrl.GetItemByID("Id",reportId);
                if (obj.Id>0)
                {
                    obj.PublishedBy = SysBaseInfor.GetCurrentUserId();
                    obj.PublishedAt = DateTime.Now;
                    if (TypeSelect > 0) obj.Status = 0;
                    else
                        obj.Status = 1;
                    objCtrl.UpdateItem(obj);
                }
                return Json(new { Msg = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { Msg = "Fail" });
            }
        }

        public JsonResult DeleteUnitReport(int reportId, int? TypeSelect)
        {
            try
            {
                UnitReportDAL objCtrl = new UnitReportDAL();
                objCtrl.DeleteItemByValues("Id", reportId);
                return Json(new { Msg = "Success" });                
            }
            catch (Exception ex)
            {
                return Json(new { Msg = "Fail" });
            }
        }
        
        [HttpPost]
        public JsonResult GetListReportByDay(byte? IPS, int? limit, int? offset)
        {
            try
            {
                UnitReportDAL UnitReportReportCtrl = new UnitReportDAL();
                var lstResult = new List<UnitReport>();
                if (IPS == 1)
                {
                    var parameterList = new List<KeyValuePair<string, string>>();
                    parameterList.Add(new KeyValuePair<string, string>("UnitId", SysBaseInfor.GetCurrentUnitId()));
                    parameterList.Add(new KeyValuePair<string, string>("DayReport", DateTime.Now.Date.ToString("yyyy-MM-dd")));
                    lstResult = UnitReportReportCtrl.GetListItems(parameterList);
                    return Json(lstResult);
                }
                return Json(lstResult);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult GetListReportByWeek(byte? IPS, int? limit, int? offset)
        {
            try
            {
                DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek)+1);
                DateTime endOfWeek = startOfWeek.AddDays(5);
                UnitReportDAL UnitReportReportCtrl = new UnitReportDAL();
                var lstResult = new List<UnitReport>();
                if (IPS == 1)
                {                    
                    lstResult = UnitReportReportCtrl.GetListReportByWeek(SysBaseInfor.GetCurrentUnitId().ToInt32(),startOfWeek, endOfWeek);
                    return Json(lstResult);
                }
                return Json(lstResult);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}