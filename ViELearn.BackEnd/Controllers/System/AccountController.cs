using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ViELearn.BackEnd.Models;
using ViELearn.BackEnd.Ultilities;
using Newtonsoft.Json;
using ViELearn.DAL.SystemDAL;
using ViELearn.DAL.ModulesDAL;
using ViELearn.Models.ProjectModels;
using ViELearn.Models.SystemModels;
using System.Collections.Generic;
using System.Configuration;
using System.Collections;
using Google.Authenticator;
using System.Threading;

namespace ViELearn.BackEnd.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        // GET: /Account/Login
        //[AllowAnonymous]
        //public async Task<JsonResult> Process()
        //{
        //    Session["flag"] = "On";
        //    while (Session["flag"].ToString()=="On")
        //    {
        //        Thread.Sleep(1000);

        //    }
        //    return Json(new{message="sucess" },JsonRequestBehavior.AllowGet);
        //}
        //[AllowAnonymous]
        //public JsonResult Stop()
        //{
        //    Session["flag"] = "Off";

        //    return Json(new{ },JsonRequestBehavior.AllowGet);
        //}

        private const string Key = "aaaabbbbccccdddd";
        public AccountController()
        {
        }
        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private ApplicationUserManager _userManager;
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

        public ActionResult Index()
        {
            var sql = $@"SELECT * FROM dbo.AspNetUsers";
            var dtUser = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.User = dtUser;
            return View();
           
        }
        public ActionResult GetUserSearch(string name = "")
        {
            Hashtable args2 = new Hashtable();
            args2.Add("name", name);
            var dt = DBLibs.ExecuteStoreProcedure_Select("be_GetAllUserSearch", args2, _cnn);
            ViewBag.User = dt;
            return PartialView("_UsersPartial");
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        private SignInHelper _helper;
        private SignInHelper SignInHelper
        {
            get
            {
                if (_helper == null)
                {
                    _helper = new SignInHelper(UserManager, AuthenticationManager);
                }
                return _helper;
            }
        }
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            if (model.UnitName == null) model.UnitName = SysBaseInfor.DefaultUnit;
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            UnitsDAL unitCtrl = new UnitsDAL();
            Units unitObj = unitCtrl.GetUnitByCode(model.UnitName);

            var result = SignInStatus.Failure;
            
            if ((model.UserName.ToLower() == SysBaseInfor.SuperAdmin.ToLower()) || (unitObj != null && (unitObj.Active == true)))
                result = await SignInHelper.PassworCheck(unitObj, model.UserName.ToLower(), model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:

                    // gan session
                    Session["userLogin"] = model.UserName;
                    Session["unitLogin"] = model.UnitName;
                    await OnLogin();
                    return Redirect("/");

                    //return RedirectToAction("OuthenGoogle", new { ReturnUrl = returnUrl }); bat authen qrcode

                case SignInStatus.LockedOut:
                    ModelState.AddModelError("", "Tài khoản bị khóa!");
                    return View(model);
                case SignInStatus.RequiresTwoFactorAuthentication:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl});
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Sai thông tin tài khoản.");
                    return View(model);
            }
        }
        public async Task<bool> OnLogin()
        {
            string userNameForauthen = Session["userLogin"].ToString();
            string unitNameForauthen = Session["unitLogin"].ToString();
            UnitsDAL unitCtrl = new UnitsDAL();
            Units unitObj = unitCtrl.GetUnitByCode(unitNameForauthen);
            try
            {
                var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var roleManager = System.Web.HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
                ApplicationUser user = userManager.FindByName(userNameForauthen);


                if (user != null)
                {

                    SysBaseInfor.SetCurrentUserName(user.UserName);
                    SysBaseInfor.SetCurrentUserDisplayName(user.DisplayName);
                    SysBaseInfor.SetCurrentUserId(user.Id);
                    SysBaseInfor.SetCurrentUserType(user.Type);
                    SysBaseInfor.SetCurrentUserTypeInfo(user.TypeInfo);
                    SysBaseInfor.SetIdNguoiDung(user.TypeInfo.ToString());

                    SysBaseInfor.SetCurrentUnitId(unitObj.Id);
                    SysBaseInfor.SetCurrentUnitCode(unitObj.Code);
                    SysBaseInfor.SetCurrentUnitName(unitObj.Name);
                    SysBaseInfor.SetCurrentMediaURL(unitObj.MediaUrl);
                    SysBaseInfor.SetCurrentUnitLogoUrl(unitObj.LogoUrl);

                    var lstRoleNames = userManager.GetRoles(user.Id);
                    if (lstRoleNames.Count > 0)
                    {
                        SYS_MENU_ROLE_CTRL smrCtrl = new SYS_MENU_ROLE_CTRL();
                        var tmpLstRoles = new List<SYS_MENU_ROLE>();
                        foreach (string tmpRoleName in lstRoleNames)
                            if (!string.IsNullOrEmpty(tmpRoleName))
                            {
                                var tmpRole = roleManager.FindByName(tmpRoleName);
                                var lstRoles = smrCtrl.GetLstSysMenuRoleWithView(tmpRole.Id);
                                foreach (SYS_MENU_ROLE tmpSmr in lstRoles)
                                    if (tmpSmr.ID > 0)
                                    {
                                        if (tmpLstRoles.All(r => r.ID_SYS_MENU != tmpSmr.ID_SYS_MENU)) tmpLstRoles.Add(tmpSmr);
                                        else
                                        {
                                            var index = tmpLstRoles.FindIndex(r => r.ID_SYS_MENU == tmpSmr.ID_SYS_MENU);
                                            var tmpAccessOld = tmpLstRoles[index].ACCESS_RIGHT;
                                            var tmpAccessNew = tmpSmr.ACCESS_RIGHT;
                                            var tmpAccessJoin = "";
                                            if (tmpAccessOld != tmpAccessNew)
                                            {
                                                for (int i = 0; i < tmpAccessOld.Length; i++)
                                                {
                                                    tmpAccessJoin += (tmpAccessOld[i] > tmpAccessNew[i]) ? tmpAccessOld[i] : tmpAccessNew[i];
                                                }
                                                tmpLstRoles[index].ACCESS_RIGHT = tmpAccessJoin;
                                            }
                                        }
                                    }
                            }
                        var jsonLstRoles = JsonConvert.SerializeObject(tmpLstRoles);
                        SysBaseInfor.SetCurrentUserListMenu(jsonLstRoles);
                    }

                    var sign = await SignInHelper.SignInOrTwoFactor(user, true);

                }

            }
            catch (Exception ex)
            {
                //AuthenticationManager.SignOut();
                //Session.Abandon();
                //ModelState.AddModelError("", ex);
                //return View(model);
                return false;
            }
                return true;
        }
        [AllowAnonymous]
        public ActionResult OuthenGoogle(string returnUrl)
        {
           var  message = "Sử dụng Ứng dụng Google Authenticator để quét mã QR:";
            //if ( Session["IsValid2FA"] == null || !(bool)Session["IsValid2FA"])
            //{
            //    return RedirectToAction("Login");
            //}
            var authenticator = new TwoFactorAuthenticator();
            var result = authenticator.GenerateSetupCode("Sao Ha Thanh", "SHT Login", Key, 300, 300);
            ViewBag.BarcodeImageUrl = result.QrCodeSetupImageUrl;
            ViewBag.SetupCode = result.ManualEntryKey;
            ViewBag.Message = message;
            ViewBag.returnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>  Verify2Fa(string returnUrl="/")
        {
            int count = Session["Count"] != null ? Session["Count"].MapInt():0;
            //var count = 0;
            var message = "Sử dụng ứng dụng Google Authenticator để quét mã QR:";
            var token = Request["passcode"];
            var authenticator = new TwoFactorAuthenticator();
            var isValid = authenticator.ValidateTwoFactorPIN(Key, token);
            if (isValid)
            {
                var result = await  OnLogin();
                Session["IsValid2FA"] = true;

                return Redirect(returnUrl);
            }
            else
            {
                var countLimit = Ultilities.Common.GetByKey("countInputError").MapInt();
                count++;
                if (count >= countLimit)
                {
                    return LogOff();
                }
                var authenticator1 = new TwoFactorAuthenticator();
                var result = authenticator1.GenerateSetupCode("Sao Ha Thanh", "SHT Login", Key, 300, 300);
                ViewBag.BarcodeImageUrl = result.QrCodeSetupImageUrl;
                ViewBag.Message = message;
                ViewBag.messError = string.Format("Mã code nhập sai, xin vui lòng nhập lại!");
                Session["Count"] = count;
                return View("OuthenGoogle");

            }
        }
        public ActionResult ImgQrCode()
        {
            var message = "Sử dụng ứng dụng Google Authenticator để quét mã QR:";
            var authenticator = new TwoFactorAuthenticator();
            var result = authenticator.GenerateSetupCode("Sao Ha Thanh", "SHT Login", Key, 300, 300);
            ViewBag.BarcodeImageUrl = result.QrCodeSetupImageUrl;
            ViewBag.SetupCode = result.ManualEntryKey;
            ViewBag.Message = message;
            return View();
        }
        public ActionResult SignOut()
        {
            return LogOff();
        }
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInHelper.HasBeenVerified())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInHelper.GetVerifiedUserIdAsync());
            if (user != null)
            {
                // To exercise the flow without actually sending codes, uncomment the following line
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        }
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInHelper.TwoFactorSignIn(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Sai mã code.");
                    return View(model);
            }
        }
        // GET: /Account/Register
        [Authorize(Roles = "HostAdmin,UnitsAdmin")]
        public ActionResult Register()
        {            
            return PartialView();
        }
        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = "HostAdmin,UnitsAdmin")]
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

        public ActionResult EditUser(string id = "")
        {
            var sql = $@"SELECT TOP 1 * FROM dbo.AspNetUsers Where id = '{id}'";
            var dtUser = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            ViewBag.User = dtUser;
            return View();
        }
        public JsonResult Save(
         string id = "",
         string _DisplayName = "",
         string _Email = "",
         string _Password = "",
         string _PhoneNumber = ""
         
       )
        {
            var stt = false;
            var msg = "";

            #region Phân tích request/submit form (nếu có)
    
                    #region Update thông tin tài khoản
                    var eff = DBLibs.ExecuteNonQuery($@"
                    UPDATE AspNetUsers 
                    SET 
	                    DisplayName = N'{_DisplayName.Replace("'", "''").Trim()}' , 
	                    Email = N'{_Email.Replace("'", "''").Trim()} , -- idTruong - int
	                    PasswordHash = N'{_Email.Replace("'", "''").Trim()},
	                    PhoneNumber = {_PhoneNumber} -- GioiTinh - bit
                    WHERE
                        id = '{id}'", _cnn);

                   
                 
                    #endregion
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
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                result = userManager.SetLockoutEnabled(userId, false);
                return View("ConfirmEmail");
            }
            AddErrors(result);
            return View();
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
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
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
        // POST: /Account/ResetPassword
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
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await SignInHelper.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        }
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            // Generate the token and send it
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!await SignInHelper.SendTwoFactorCode(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
        }
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInHelper.ExternalSignIn(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresTwoFactorAuthentication:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInHelper.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}