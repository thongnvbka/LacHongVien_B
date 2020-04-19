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
    public class UnitWorkCalendarController : Controller
    {
        protected static string blankViewUrl = "~/Views/Shared/_Blank.cshtml";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UnitWorkCalendarDaily()
        {            
            return View();
        }
        public ActionResult UnitWorkCalendarWeekly()
        {
            DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek) + 1);
            DateTime endOfWeek = startOfWeek.AddDays(6);
            ViewBag.StartOfWeek = startOfWeek;
            ViewBag.EndOfWeek = endOfWeek;
            return View();
        }

        public ActionResult ShowDetailUnitWorkCalendar(int workCalendarId, int? TypeSelect)
        {
            try
            {
                UnitWorkCalendarDAL WorkCalendarCtrl = new UnitWorkCalendarDAL();
                var vnFormat = new CultureInfo("VI-vi");
                var listDayForSelect = new List<SelectListItem>();

                if (TypeSelect > 0)
                {
                    DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek) + 1);
                    DateTime endOfWeek = startOfWeek.AddDays(5);

                    for (int tmpI = (int)DateTime.Today.DayOfWeek; tmpI < 7; tmpI++)
                    {
                        var tmpDow = DateTime.Today.AddDays(tmpI - (int)DateTime.Today.DayOfWeek);
                        listDayForSelect.Add(new SelectListItem() { Text = vnFormat.DateTimeFormat.GetDayName(tmpDow.DayOfWeek) + " " + tmpDow.ToString("dd/MM/yyyy"), Value = tmpDow.ToString() });
                    }
                }
                else
                {
                    listDayForSelect.Add(new SelectListItem() { Text = vnFormat.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek) + " " + DateTime.Now.ToString("dd/MM/yyyy"), Value = DateTime.Now.ToString("MM/dd/yyyy") });
                }

                ViewBag.ListDayForSelect = new SelectList(listDayForSelect, "Value", "Text");

                var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
                ViewBag.ListGroupRoles = new SelectList(roleManager.GetListRoleByUnitIdAndType(SysBaseInfor.GetCurrentUnitCode() + SysBaseInfor.SignProject, SysBaseInfor.GetCurrentUnitId().ToInt32(), 2), "Id", "Name");

                var result = new UnitWorkCalendar();

                if (workCalendarId > 0)
                    result = WorkCalendarCtrl.GetItemByID("Id", workCalendarId);

                return PartialView("~/Views/UnitWorkCalendar/_DetailUnitWorkCalendar.cshtml", result);
            }
            catch (Exception ex)
            {
                return PartialView(blankViewUrl);
            }
        }

        public JsonResult SaveDetailWorkCalendar(UnitWorkCalendar objWorkCalendar)
        {
            try
            {
                UnitWorkCalendarDAL objCtrl = new UnitWorkCalendarDAL();

                if (objWorkCalendar.DayWorkCalendar != null) objWorkCalendar.DayWorkCalendar = objWorkCalendar.DayWorkCalendar.ToLocalTime();

                if (objWorkCalendar.Id > 0)
                {
                    objWorkCalendar.UpdatedBy = SysBaseInfor.GetCurrentUserId();
                    objCtrl.UpdateItem(objWorkCalendar);
                    return Json(new { Msg = "Success", Type = "Edit" });
                }
                else
                {
                    objWorkCalendar.Status = 0;
                    objWorkCalendar.CreatedBy = SysBaseInfor.GetCurrentUserId();
                    objWorkCalendar.UnitId = SysBaseInfor.GetCurrentUnitId().ToInt32();
                    objCtrl.CreateItem(objWorkCalendar);
                    return Json(new { Msg = "Success", Type = "Create" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Msg = "Fail" });
            }
        }

        public JsonResult PublishWorkCalendar(int workCalendarId, int? TypeSelect)
        {
            try
            {
                UnitWorkCalendarDAL objCtrl = new UnitWorkCalendarDAL();
                UnitWorkCalendar obj = objCtrl.GetItemByID("Id", workCalendarId);
                if (obj.Id > 0)
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

        public JsonResult DeleteUnitWorkCalendar(int workCalendarId, int? TypeSelect)
        {
            try
            {
                UnitWorkCalendarDAL objCtrl = new UnitWorkCalendarDAL();
                objCtrl.DeleteItemByValues("Id", workCalendarId);
                return Json(new { Msg = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { Msg = "Fail" });
            }
        }

        [HttpPost]
        public JsonResult GetListUnitWorkCalendarByDay(byte? IPS, int? limit, int? offset)
        {
            try
            {
                UnitWorkCalendarDAL workCalendarCtrl = new UnitWorkCalendarDAL();
                var lstResult = new List<UnitWorkCalendar>();
                if (IPS == 1)
                {
                    //var parameterList = new List<KeyValuePair<string, string>>();
                    //parameterList.Add(new KeyValuePair<string, string>("UnitId", SysBaseInfor.GetCurrentUnitId())); 
                    //parameterList.Add(new KeyValuePair<string, string>("DayWorkCalendar", DateTime.Now.Date.ToString("yyyy-MM-dd")));
                    //lstResult = workCalendarCtrl.GetListItems(parameterList);

                    lstResult = workCalendarCtrl.GetListWorkCalendarByDay(SysBaseInfor.GetCurrentUnitId().ToInt32(), DateTime.Now);
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
        public JsonResult GetListUnitWorkCalendarByWeek(byte? IPS, int? limit, int? offset)
        {
            try
            {
                DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek) + 1);
                DateTime endOfWeek = startOfWeek.AddDays(6);
                UnitWorkCalendarDAL WorkCalendarReportCtrl = new UnitWorkCalendarDAL();
                var lstResult = new List<UnitWorkCalendar>();
                if (IPS == 1)
                {
                    lstResult = WorkCalendarReportCtrl.GetListWorkCalendarByWeek(SysBaseInfor.GetCurrentUnitId().ToInt32(), startOfWeek, endOfWeek);
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