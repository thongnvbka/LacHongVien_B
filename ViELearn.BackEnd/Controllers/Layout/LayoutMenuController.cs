using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ViELearn.Models.SystemModels;
using ViELearn.BackEnd.Ultilities;

namespace ViELearn.BackEnd.Controllers
{
    public class LayoutMenuController : Controller
    {
        [AllowAnonymous]
        public ActionResult MobileMenu()
        {
            return PartialView("~/Views/Shared/Shop/_MobileMenu.cshtml");
        }
        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult LeftMenu()
        {
            try
            {
                var results = new List<SYS_MENU_ROLE>();
                var tmpLstRoles = SysBaseInfor.GetListSysMenu();
                if (!String.IsNullOrEmpty(tmpLstRoles))
                {
                    results = JsonConvert.DeserializeObject<List<SYS_MENU_ROLE>>(tmpLstRoles);
                }
                return PartialView("~/Views/Shared/_LeftMenu.cshtml", results);
            }
            catch (Exception ex)
            {
                return PartialView("~/Views/Shared/_Error.cshtml", ex);
            }
        }
        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult HeaderTopMenu()
        {
            try
            {
                var results = new List<SYS_MENU_ROLE>();
                var tmpLstRoles = SysBaseInfor.GetListSysMenu();
                if (!String.IsNullOrEmpty(tmpLstRoles))
                {
                    results = JsonConvert.DeserializeObject<List<SYS_MENU_ROLE>>(tmpLstRoles);
                }
                return PartialView("~/Views/Shared/_HeaderTopMenu.cshtml", results);
            }
            catch (Exception ex)
            {
                return PartialView("~/Views/Shared/_Error.cshtml", ex);
            }
        }        

        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult FooterMenu()
        {
            return PartialView("~/Views/Shared/_FooterMenu.cshtml");
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public PartialViewResult TopMenu()
        {
            @ViewBag.TenTruong = "";
            @ViewBag.TenNguoiDung = "";
            try
            {
                if (SysBaseInfor.GetCurrentRoleName().ToLower() == "hostadmin")
                {
                    @ViewBag.TenTruong = "Quản trị viên MDC Group";
                }
                else
                {
                    @ViewBag.TenTruong = SysBaseInfor.GetCurrentUnitName();
                }
                @ViewBag.TenNguoiDung = SysBaseInfor.GetCurrentUserName();
            }
            catch (Exception ex)
            {
            }
            return PartialView("~/Views/Shared/_TopMenu.cshtml");
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public PartialViewResult TitleLink()
        {
            return PartialView("~/Views/Shared/_TitleLink.cshtml");
        }
    }
}