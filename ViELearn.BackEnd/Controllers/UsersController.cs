using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViELearn.BackEnd.Models;
using ViELearn.BackEnd.Ultilities;

namespace ViELearn.BackEnd.Controllers
{
    public class UsersController : Controller
    {
        int userId = int.Parse(SysBaseInfor.GetIdNguoiDung());
        private string uId = SysBaseInfor.GetCurrentUserId();
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private ApplicationUserManager _userManager;
        public UsersController()
        {
           // UserManager = userManager;
        }
        // private ApplicationDbContext context = new ApplicationDbContext();
        public UsersController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
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
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        // GET: Admin/ManageUser
        [Authorize(Roles = "HostAdmin")]
        public ActionResult Index()
        {
            var countNews = DBLibs.ExecuteScalar($"SELECT COUNT(*) AS count FROM  dbo.News Where CreatedBy = '{SysBaseInfor.GetCurrentUserId()}' OR UpdatedBy = '{SysBaseInfor.GetCurrentUserId()}' ", _cnn);
            ViewBag.countNews = countNews;
            var sql = $@"SELECT TOP 1000 [Id]
                 ,[Name]
                 ,[UnitId]
                 ,[RoleType]
                 ,[Discriminator]
                  FROM [dbo].[AspNetRoles]";
            var dtRole = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.ListRole = dtRole;
            return View();

        }
        // GET: Users
        //public ActionResult Index()
        //{
        //    var sql = $@"SELECT * FROM dbo.AspNetUsers";
        //    var dtUser = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
        //    ViewBag.User = dtUser;
        //    return View();
        //}

        public ActionResult GetUserSearch(string name = "",int trangthai = 0)
        {
            Hashtable args2 = new Hashtable();
            args2.Add("uId", uId);
            args2.Add("trangthai", trangthai);
            args2.Add("name", name);
            var dt = DBLibs.ExecuteStoreProcedure_Select("be_GetAllUserSearch", args2, _cnn);
            ViewBag.User = dt;
            ViewBag.status = trangthai;
            return PartialView("_UsersPartial");
        }
        public ActionResult GetUserbyRole(string id = "")
        {
            Hashtable args2 = new Hashtable();
           // args2.Add("uId", uId);
            args2.Add("id",id);
            var dt = DBLibs.ExecuteStoreProcedure_Select("be_GetUserbyRole", args2, _cnn);
            ViewBag.User = dt;
            return PartialView("_UsersPartial");
        }

        public ActionResult Create(int type = 0)
        {
            if (Request["t"] == "1")
                ViewBag.Title = " ";
            else if (Request["t"] == "2")
                ViewBag.Title = " ";

            var dtTruong = DBLibs.GetDataBy_DataAdapter($@"SELECT ID, TenDanhMuc FROM DanhMucChung WHERE TrangThai = 1 AND LoaiDanhMuc = 10 ORDER BY TenDanhMuc", _cnn);
            ViewBag.DsTruong = dtTruong;

            return View();
        }

        public JsonResult Save(
        string id = "",
        string _DisplayName = "",
        string _Email = "",
        string _PhoneNumber = ""

      )
        {
            var stt = false;
            var msg = "";

            //var PasswordHash = UserManager.PasswordHasher.HashPassword(_Password);

            #region Update thông tin tài khoản
            var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE AspNetUsers 
                    SET 
	                    DisplayName = N'{_DisplayName.Replace("'", "''").Trim()}', 
	                    Email = N'{_Email.Replace("'", "''").Trim()}', 
	                    PhoneNumber = N'{_PhoneNumber.Replace("'", "''").Trim()}'
                    WHERE
                        id = '{id}'", _cnn);



            #endregion
            if (eff > 0) stt = true;
            else
                msg = "Không cập nhật dữ liệu được!";
          

            return Json(new
            {
                status = stt,
                message = msg
            });
        }

        public JsonResult Delete(string id = "")
        {
            var stt = false;
            var msg = "";

            if (id != "" && id != null)
            {
                var sql = $"DELETE dbo.AspNetUsers WHERE id = '{id}'";
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
        public ActionResult Edit(string id="")
        {
            var sql = $@"SELECT TOP 1 * FROM dbo.AspNetUsers Where id = '{id}'";
            var dtUser = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.User = dtUser;
            ViewBag.IdDm = id;
            return View();
        }

        private bool Create(string username, string displayname, string password, string roleId)
        {
            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = username,
                    DisplayName = displayname
                };
                var result = UserManager.Create(user, password);
                if (result.Succeeded)
                {
                    user.Type = 2;
                    user.UnitId = SysBaseInfor.GetCurrentUnitId().ToInt32();
                    user.UnitName = SysBaseInfor.GetCurrentUnitCode().ToLower();
                    try { user.TypeInfo = int.Parse(DBLibs.ExecuteScalar($"SELECT TOP 1 id FROM GiaoVien WHERE magv = N'{username.Replace("'", "''")}'", _cnn).ToString()); }
                    catch { }
                    var resultActive = UserManager.SetLockoutEnabled(user.Id, false);
                    if (resultActive.Succeeded)
                    {
                        try
                        {
                            var sql = $@"
                            INSERT INTO dbo.AspNetUserRoles
                            ( UserId, RoleId )
                            VALUES
                            (
	                            N'{user.Id}', -- UserId - nvarchar(128)
	                            N'{roleId}'  -- RoleId - nvarchar(128)
                            )";
                            DBLibs.ExecuteNonQuery(sql, _cnn);
                        }
                        catch { }
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            return false;
        }

        [Authorize(Roles = "HostAdmin")]
        public ActionResult Register()
        {
            return PartialView();
        }
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Register(RegisterViewModel model)
        {
            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, DisplayName = model.DisplayName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);                        
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Kích hoạt tài khoản", "Để kích hoạt tài khoản,vui lòng bấm vào đường dẫn <a href=\"" + callbackUrl + "\">link</a>");

                    user.Type = 2;
                    user.UnitId = SysBaseInfor.GetCurrentUnitId().ToInt32();
                    user.UnitName = SysBaseInfor.GetCurrentUnitCode().ToLower();
                    var resultActive = UserManager.SetLockoutEnabled(user.Id, false);
                    if (resultActive.Succeeded)
                    {
                        var roleForUserName = user.UserName + "_" + SysBaseInfor.GetCurrentUnitCode().ToLower() + SysBaseInfor.SignProject;
                        var roleForUser = roleManager.FindByName(roleForUserName);
                        if (roleForUser == null)
                        {
                            roleForUser = new ApplicationRole(roleForUserName);
                            roleForUser.RoleType = 3;
                            roleForUser.UnitId = SysBaseInfor.GetCurrentUnitId().ToInt32();
                            var roleResult = roleManager.Create(roleForUser);
                            if (roleResult.Succeeded)
                            {
                                UserManager.AddToRole(user.Id, roleForUserName);
                                return Json("Success");
                            }
                        }
                    }
                    else
                        return Json(resultActive.Errors.FirstOrDefault());
                }
                return Json(result.Errors.FirstOrDefault());
            }
            return Json("Không thể tạo người dùng");
        }
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Users", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            //return code == null ? View("Error") : View();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Users");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Users");
            }
            AddErrors(result);
            return View();
        }
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        // POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}
        // POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await UserManager.FindByNameAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    AddErrors(result);
        //    return View();
        //}
        //private bool ResetPassword(string username, string new_pass = "Abc@123")
        //{
        //    try
        //    {
        //        var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //        var user = UserManager.FindByName(username);
        //        if (user == null)
        //            user = UserManager.FindById(username);
        //        if (user != null)
        //        {
        //            user.PasswordHash = UserManager.PasswordHasher.HashPassword(new_pass);
        //            UserManager.Update(user);
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        public JsonResult Doitrangthai(string id = "",int stt = 0)
        {
            var status = false;
            var message = "";
            if (id != "")
            {
                var sql = $@"UPDATE dbo.AspNetUsers SET LockoutEnabled = {stt} WHERE id = '{id}'";
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                if (eff > 0) status = true;
            }

            return Json(new
            {
                status,
                message
            }, JsonRequestBehavior.AllowGet);

        }
        [Authorize(Roles = "HostAdmin,UnitsAdmin")]
        public async Task<ActionResult> DetailsUser(string userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();

            var lstRoles = new List<ApplicationRole>();
            lstRoles = roleManager.GetListRoleByUnitId(SysBaseInfor.GetCurrentUnitCode().ToLower() + SysBaseInfor.SignProject, SysBaseInfor.GetCurrentUnitId().ToInt32());
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
        public JsonResult UpdateDisplayName(string name = "", string id = "")
        {
            var stt = false;
            var msg = "";
            #region Update Tiêu đề
            var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE AspNetUsers
                    SET 
                        DisplayName = N'{name.Replace("'", "''").Trim()}'
                   
                    WHERE
                        id = {id}", _cnn);
            if (eff > 0) stt = true;
            else
                msg = "Không cập nhật dữ liệu được!";
            #endregion
            return Json(new
            {
                status = stt,
                message = msg
            });
        }

        [Authorize(Roles = "HostAdmin")]
        public async Task<JsonResult> DeleteUserConfirmed(string id="",int count=0)
        {
           // var totalNews = UserManager.GetCountNewsByUsers(id);

            if (count > 0)
            {
                var sql = $@"UPDATE dbo.AspNetUsers SET LockoutEnabled = 1 WHERE id = '{id}'";
                var eff = DBLibs.ExecuteNonQuery(sql, _cnn);
                return Json("Success");
            }
            else
            {
                var user = await UserManager.FindByIdAsync(id);
                var result = await UserManager.DeleteAsync(user);
                return Json("Success");
            }
        }

    }
}