using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViELearn.BackEnd.Models;
using ViELearn.BackEnd.Ultilities;

namespace ViELearn.BackEnd.Controllers
{
    public class UnitUserManageController : Controller
    {
        //[ProjectCustomFilter(UserPermissions.QuyenXem)]
        public ActionResult Index()
        {
            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();            
            var lstAllRoles = new List<ApplicationRole>();
            lstAllRoles = roleManager.GetListRoleByUnitIdAndType(SysBaseInfor.GetCurrentUnitCode().ToLower() + SysBaseInfor.SignProject, SysBaseInfor.GetCurrentUnitId().ToInt32(),2);
            ViewBag.SelectLstAllRoles = new MultiSelectList(lstAllRoles, "ID", "Name");
            return View();
        }

        [HttpPost]
        public JsonResult GetListUserInUnit(string roleId)
        {
            try
            {
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                int unitId = SysBaseInfor.GetCurrentUnitId().ToInt32();
                var usersInUnit = userManager.Users.Where(x => x.UnitId == unitId && x.Type != 0 && x.Type != 1).ToList();                
                return Json(usersInUnit);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public async Task<ActionResult> DetailsUser(string userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();

            var lstRoles = new List<ApplicationRole>();
            lstRoles = roleManager.GetListRoleByUnitIdAndType(SysBaseInfor.GetCurrentUnitCode().ToLower() + SysBaseInfor.SignProject, SysBaseInfor.GetCurrentUnitId().ToInt32(),2);            
            var user = await userManager.FindByIdAsync(userId);
            var tmpLstUserRole = new List<string>();
            foreach (var item in user.Roles)
            {
                tmpLstUserRole.Add(item.RoleId);
            }
            ViewBag.SelectLstRoles = new MultiSelectList(lstRoles, "ID", "Name", tmpLstUserRole);
            return PartialView(user);
        }

        public async Task<JsonResult> SaveDetailsUser(List<string> lstUserInRole, ApplicationUser user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Id)) return Json("Không thể sửa thông tin");
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                ApplicationUser modelSave = await userManager.FindByIdAsync(user.Id);
                modelSave.Email = user.Email;
                modelSave.DisplayName = user.DisplayName;
                modelSave.UserName = user.UserName;

                var result = await userManager.UpdateAsync(modelSave);

                if (result.Succeeded)
                {
                    var tmpRemoveRoles = await userManager.GetRolesAsync(user.Id);
                    await userManager.RemoveFromRolesAsync(user.Id, tmpRemoveRoles.ToArray());
                    var tmpLstRoleName = new List<string>();
                    if (lstUserInRole != null && lstUserInRole.Count > 0)
                    {
                        foreach (var item in lstUserInRole)
                        {
                            tmpLstRoleName.Add(HttpContext.GetOwinContext().Get<ApplicationRoleManager>().FindById(item).Name);
                        }
                        await userManager.AddUserToRolesAsync(user.Id, tmpLstRoleName);
                    }

                    return Json("Success");
                }
                return Json(result.Errors.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return Json("Có lỗi xảy ra :" + ex);
            }
        }
        public async Task<JsonResult> SaveManyUserToRole(List<string> lstUserId, List<string> lstAccessSelectedRole)
        {
            try
            {
                if (lstUserId.Count == 0 || lstAccessSelectedRole.Count == 0) return Json(new { Msg = "Phải chọn người dùng và nhóm người dùng" });
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                
                var tmpLstRoleName = new List<string>();
                foreach (var item in lstAccessSelectedRole)
                {
                    tmpLstRoleName.Add(HttpContext.GetOwinContext().Get<ApplicationRoleManager>().FindById(item).Name);
                }
                foreach (string userId in lstUserId)
                {
                    await userManager.AddUserToRolesAsync(userId, tmpLstRoleName);
                }
                return Json(new { Msg = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { Msg = ex.Message });
            }
        }
    }
}