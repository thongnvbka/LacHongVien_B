using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using ViELearn.DAL.ModulesDAL;
using ViELearn.Models.ProjectModels;
using ViELearn.BackEnd.Models;
using ViELearn.BackEnd.Ultilities;

namespace ViELearn.BackEnd.Controllers
{    
    public class UnitsManageController : Controller
    {
        protected static string urlView = "~/Views/UnitsManage/";
        [ProjectCustomFilter(UserPermissions.QuyenXem)]
        public ActionResult Index()
        {
            return View(urlView + "Index.cshtml");
        }

        [HttpPost]
        //public JsonResult GetListUnits(byte? IPS, int? limit, int? offset)
        //{
        //    try
        //    {
        //        UnitsDAL unitsCtrl = new UnitsDAL();
        //        var lstResult = new List<Units>();
        //        if (IPS == 1)
        //        {
        //            if (SysBaseInfor.GetCurrentUserName() == SysBaseInfor.SuperAdmin)
        //            {
        //                lstResult = unitsCtrl.GetListItems();
        //            }
        //            else
        //            {
        //               lstResult.Add(unitsCtrl.GetItemByID("Id",SysBaseInfor.GetCurrentUnitId().ToInt32()));
        //            }                                         
        //            return Json(lstResult);
        //        }
        //        else
        //        {
        //            lstResult = unitsCtrl.GetListItemPagination("CreatedAt", "DESC", offset.GetValueOrDefault(), limit.GetValueOrDefault());
        //            return Json(new { total = unitsCtrl.GetTotalUnit(), rows = lstResult });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message);
        //    }
        //}
        public ActionResult ShowDetailUnit(int IdUnit)
        {
            try
            {
                UnitsDAL unitsCtrl = new UnitsDAL();

                var result = new Units();
                if (IdUnit > 0)
                    result = unitsCtrl.GetItemByID("Id", IdUnit);

                return PartialView(urlView + "_DetailUnit.cshtml", result);
            }
            catch (Exception ex)
            {
                return PartialView(urlView + "_DetailUnit.cshtml", ex);
            }
        }

        public ActionResult SaveUnit(string ChkActive, Units objUnit)
        {
            try
            {
                UnitsDAL objCtrl = new UnitsDAL();

                objUnit.Active = !string.IsNullOrEmpty(ChkActive);

                if (objUnit.Id > 0)
                {
                    objUnit.UpdatedAt = DateTime.Now;
                    objUnit.UpdatedBy = SysBaseInfor.GetCurrentUserId();
                    objCtrl.UpdateItem(objUnit);
                }
                else
                {
                    objUnit.CreatedAt = DateTime.Now;
                    objUnit.CreatedBy = SysBaseInfor.GetCurrentUserId();
                    objCtrl.CreateItem(objUnit);
                    string adminName = "admin@" + objUnit.Code;
                    string adminPass = "123456";
                    string roleName = "UnitsAdmin";
                    InitializeIdentity(HttpContext.GetOwinContext(), adminName, adminPass, roleName, 1, 0, objUnit.Code, objUnit.Id);
                }

                return PartialView(urlView + "_DetailUnit.cshtml", objUnit);
            }
            catch (Exception ex)
            {
                return PartialView(urlView + "_DetailUnit.cshtml", new Modules());
            }
        }

        [HttpPost]
        public ActionResult DeleteUnit(int idUnit)
        {
            try
            {
                //UnitsDAL objCtrl = new UnitsDAL();
                //var tmpResult = objCtrl.DeleteItemByValues("Id", idUnit);
                return Content("Success");
            }
            catch (Exception ex)
            {
                return Content("Không thể xóa! Vui lòng kiểm tra lại.");
            }
        }

        private static void InitializeIdentity(IOwinContext context, string name, string password, string roleName, int type, int typeInfo, string unitCode, int unitId)
        {
            var userManager = context.GetUserManager<ApplicationUserManager>();
            var roleManager = context.Get<ApplicationRoleManager>();

            var role = roleManager.FindByName(roleName);
            if (role != null)
            {
                var user = userManager.FindByName(name);

                if (user == null)
                {
                    user = new ApplicationUser { UserName = name, Email = name };
                    user.Type = type;
                    user.TypeInfo = typeInfo;
                    user.UnitName = unitCode;
                    user.UnitId = unitId;
                    user.DisplayName = "Quản trị đơn vị";
                    var result = userManager.Create(user, password);
                    result = userManager.SetLockoutEnabled(user.Id, false);
                    var rolesForUser = userManager.GetRoles(user.Id);
                    if (rolesForUser == null || !rolesForUser.Contains(role.Name))
                    {
                        result = userManager.AddToRole(user.Id, role.Name);
                    }
                }
            }
        }
    }
}