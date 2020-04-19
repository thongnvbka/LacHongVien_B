using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using ViELearn.BackEnd.Ultilities;
using ViELearn.BackEnd.Models;
using System;

namespace ViELearn.BackEnd.Controllers
{    
    public class RolesAdminController : Controller
    {
        public RolesAdminController()
        {
        }

        public RolesAdminController(ApplicationUserManager userManager,ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //[ProjectCustomFilter(UserPermissions.QuyenXem)]
        public ActionResult Index()
        {            
            return View();
        }

        [HttpPost]
        public JsonResult GetListRole()
        {
            try
            {
                var lstResult = RoleManager.GetListRole(SysBaseInfor.GetCurrentUnitCode().ToLower() + SysBaseInfor.SignProject);
                return Json(lstResult);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public async Task<ActionResult> Details(string roleId)
        {
            if (roleId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            int unitId = SysBaseInfor.GetCurrentUnitId().ToInt32();
            var usersInUnit = userManager.Users.Where(x => x.UnitId == unitId && x.Type != 0 && x.Type != 1);
            ViewBag.lstAllUser = new SelectList(usersInUnit.ToList(), "Id", "UserName");
            var role = await RoleManager.FindByIdAsync(roleId);
            RoleViewModel roleModel = new RoleViewModel();
            roleModel.Id = role.Id;
            var sign = "_" + SysBaseInfor.GetCurrentUnitCode().ToLower() + SysBaseInfor.SignProject;
            roleModel.Name = role.Name.Replace(sign, "");
            roleModel.unitId = role.UnitId;
            roleModel.roleType = role.RoleType;
            return PartialView(roleModel);
        }

        [HttpPost]
        public JsonResult GetListUserInRole(string roleId)
        {
            try
            {
                var result = RoleManager.GetListUsersInRole(roleId);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult AddUserInRole(string userId, string roleId)
        {
            try
            {
                var role = RoleManager.FindById(roleId);
                var users = new List<ApplicationUser>();
                var result = UserManager.AddToRole(userId, role.Name);
                if (!result.Succeeded) return Json(result.Errors.First());
                return Json("Success");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult DeleteUserFromRole(string userId, string roleId)
        {
            try
            {
                var role = RoleManager.FindById(roleId);
                var users = new List<ApplicationUser>();
                var result = UserManager.RemoveFromRoles(userId, role.Name);
                if (!result.Succeeded) return Json(result.Errors.First());
                return Json("Success");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        public ActionResult Create()
        {
            return PartialView();
        }
        [HttpPost]
        public async Task<JsonResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var sign = "_" + SysBaseInfor.GetCurrentUnitCode().ToLower() + SysBaseInfor.SignProject;
                var role = new ApplicationRole(roleViewModel.Name+sign);
                role.RoleType = 2;
                role.UnitId = SysBaseInfor.GetCurrentUnitId().ToInt32();
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First());
                    return Json("Có lỗi xảy ra, vui lòng thử lại");
                }
                return Json("Success");                
            }
            return Json("Có lỗi xảy ra, vui lòng thử lại");
        }
        public async Task<ActionResult> Edit(string roleId)
        {
            if (roleId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return HttpNotFound();
            }
            RoleViewModel roleModel = new RoleViewModel { Id = role.Id, Name = role.Name.Replace("_"+SysBaseInfor.GetCurrentUnitCode().ToLower()+SysBaseInfor.SignProject, ""),roleType=role.RoleType,unitId=role.UnitId };
            return PartialView("~/Views/Users/EditRole.cshtml",roleModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit([Bind(Include = "Name,Id")] RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name+ "_" + SysBaseInfor.GetCurrentUnitCode().ToLower() + SysBaseInfor.SignProject;
                await RoleManager.UpdateAsync(role);
                return Json("Success");
            }
            return Json("Có lỗi xảy ra, vui lòng thử lại");
        }
        // Xóa nhóm người dùng;
        [HttpPost]        
        public async Task<JsonResult> DeleteConfirmed(string roleId, string deleteUser)
        {
            string mess = "";
             bool status = false;
            if (roleId == null) return Json("Có lỗi xảy ra, vui lòng thử lại");
            var role = await RoleManager.FindByIdAsync(roleId);
            if (role == null) return Json("Có lỗi xảy ra, vui lòng thử lại");
            var listUsers = RoleManager.GetListUsersInRole(roleId);
            IdentityResult result;
            if (listUsers.Count() > 0)
            {
                mess = "Yêu cầu xóa hết người dùng trong nhóm";
                status = false;
                return Json(new
                {
                    status = status,
                    message = mess
                });
            }
            else
            {
                result = await RoleManager.DeleteAsync(role);
                mess = "Xóa nhóm người dùng thành công!";
                status = true;
                if (!result.Succeeded) return Json("Có lỗi xảy ra, vui lòng thử lại");
                return Json(new
                {
                    status = status,
                    message = mess
                });
            }
                   
        }
    }
}