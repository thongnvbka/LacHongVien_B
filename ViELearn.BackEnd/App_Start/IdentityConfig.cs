using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using ViELearn.BackEnd.Models;
using ViELearn.BackEnd.Ultilities;
using ViELearn.Models.ProjectModels;

namespace ViELearn.BackEnd
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }
        public List<ApplicationUser> GetCountNewsByUsers(string userId)
        {
            using (var context = new DbContext("DefaultConnection"))
            {
                var sql = String.Format(@"SELECT * FROM dbo.AspNetUsers u JOIN dbo.News n ON n.CreatedBy = u.Id WHERE u.Id = '{0}'", userId);
                var result = context.Database.SqlQuery<ApplicationUser>(sql).ToList();
                return result;
            }
        }
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            // var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));

            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        /// <summary>
        /// Method to add user to multiple roles
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="roles">list of role names</param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> AddUserToRolesAsync(string userId, IList<string> roles)
        {
            var userRoleStore = (IUserRoleStore<ApplicationUser, string>)Store;

            var user = await FindByIdAsync(userId).ConfigureAwait(false);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid user Id");
            }

            var userRoles = await userRoleStore.GetRolesAsync(user).ConfigureAwait(false);
            // Add user to each role using UserRoleStore
            foreach (var role in roles.Where(role => !userRoles.Contains(role)))
            {
                await userRoleStore.AddToRoleAsync(user, role).ConfigureAwait(false);
            }

            // Call update once when all roles are added
            return await UpdateAsync(user).ConfigureAwait(false);
        }

        /// <summary>
        /// Remove user from multiple roles
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="roles">list of role names</param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> RemoveUserFromRolesAsync(string userId, IList<string> roles)
        {
            var userRoleStore = (IUserRoleStore<ApplicationUser, string>)Store;

            var user = await FindByIdAsync(userId).ConfigureAwait(false);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid user Id");
            }

            var userRoles = await userRoleStore.GetRolesAsync(user).ConfigureAwait(false);
            // Remove user to each role using UserRoleStore
            foreach (var role in roles.Where(userRoles.Contains))
            {
                await userRoleStore.RemoveFromRoleAsync(user, role).ConfigureAwait(false);
            }

            // Call update once when all roles are removed
            return await UpdateAsync(user).ConfigureAwait(false);
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
            return manager;
        }
        public List<ApplicationRole> GetListRoleByUnitId(string sign, int unitId)
        {
            using (var context = new DbContext("DefaultConnection"))
            {
                var sql = String.Format(@"Select Id,REPLACE(Name,'_mdcgroup^&@*#', '') as Name,RoleType,UnitId from AspNetRoles ORDER BY Name ASC", "_" + sign);
                var result = context.Database.SqlQuery<ApplicationRole>(sql).ToList();
                return result;
            }
        }
        public List<ApplicationRole> GetListRole(string sign)
        {
            using (var context = new DbContext("DefaultConnection"))
            {
                var sql = String.Format(@"Select Id,REPLACE(Name,'{0}', '') as Name,RoleType,UnitId from AspNetRoles  ORDER BY Name ", "_" + sign);
                var result = context.Database.SqlQuery<ApplicationRole>(sql).ToList();
                return result;
            }
        }

        public List<ApplicationRole> GetListRoleByUnitIdAndType(string sign, int unitId, int roleType)
        {
            using (var context = new DbContext("DefaultConnection"))
            {
                var sql = String.Format(@"Select Id,REPLACE(Name,'{0}', '') as Name,RoleType,UnitId from AspNetRoles where UnitId = {1} and RoleType = {2} ORDER BY RoleType ASC,Name ASC", "_" + sign, unitId, roleType);
                var result = context.Database.SqlQuery<ApplicationRole>(sql).ToList();
                return result;
            }
        }
        //public List<ApplicationRole> GetListRoleByUnitId(string sign, int unitId)
        //{
        //    using (var context = new DbContext("DefaultConnection"))
        //    {
        //        var sql = String.Format(@"Select Id,REPLACE(Name,'{0}', '') as Name,RoleType,UnitId from AspNetRoles where UnitId = {1}  ORDER BY RoleType ASC,Name ASC", "_" + sign, unitId);
        //        var result = context.Database.SqlQuery<ApplicationRole>(sql).ToList();
        //        return result;
        //    }
        //}

        public List<ApplicationUser> GetListUsersInRole(string roleId)
        {
            using (var context = new DbContext("DefaultConnection"))
            {
                var sql = String.Format(@"SELECT  anu.*
                                          FROM    dbo.AspNetUsers anu
                                                  INNER JOIN dbo.AspNetUserRoles anur ON anu.Id = anur.UserId
                                                                                         AND anur.RoleId = '{0}'", roleId);
                var result = context.Database.SqlQuery<ApplicationUser>(sql).ToList();
                return result;
            }
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }


    //This is useful if you do not want to tear down the database each time you run the application.
    public sealed class ApplicationDbInitializer : IDisposable
    {
        private static readonly object ThisLock = new object();
        private static volatile ApplicationDbInitializer _applicationDbInitializer;

        private bool _isInitialized;

        private ApplicationDbInitializer(IOwinContext context)
        {
            Seed(context);
        }

        public static ApplicationDbInitializer Create(IdentityFactoryOptions<ApplicationDbInitializer> options, IOwinContext context)
        {
            if (_applicationDbInitializer != null)
                return _applicationDbInitializer;

            lock (ThisLock)
            {
                if (_applicationDbInitializer != null) return _applicationDbInitializer;
                _applicationDbInitializer = new ApplicationDbInitializer(context);
            }
            return _applicationDbInitializer;
        }

        /// <summary>
        /// Run Once Initialize code blocks.
        /// </summary>
        /// <param name="context"></param>
        private void Seed(IOwinContext context)
        {
            if (_isInitialized) return;
            InitializeDb(context);
            InitializeIdentity(context);
            _isInitialized = true;
        }

        // Verify Db or Tables and Create it, If you need.
        private static void InitializeDb(IOwinContext context)
        {
            // var oracleDatabase = context.Get<ApplicationDbContext>() as OracleDatabase;
            // e.g. Run the DDL if Table is not.
        }

        // Create Sample Admin User: admin@admin.com with password: Admin@123456 in the Admin role
        private static void InitializeIdentity(IOwinContext context)
        {
            var userManager = context.GetUserManager<ApplicationUserManager>();
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                // This disables the validation check on email addresses
                RequireUniqueEmail = false
            };
            var roleManager = context.Get<ApplicationRoleManager>();
            string name = SysBaseInfor.SuperAdmin;
            string password = SysBaseInfor.SuperAdminPass;
            string roleName = SysBaseInfor.SuperAdminRoleName;

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new ApplicationRole(roleName);
                role.RoleType = 0;
                role.UnitId = SysBaseInfor.GetCurrentUnitId().ToInt32();
                var roleresult = roleManager.Create(role);
            }

            var user = userManager.FindByName(name);
            if (user == null)
            {
                user = new ApplicationUser { UserName = name };
                user.Type = 0;
                user.TypeInfo = 0;
                user.UnitId = 0;
                user.UnitName = SysBaseInfor.DefaultUnit;
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }
            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                var result = userManager.AddToRole(user.Id, role.Name);
            }
        }

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }
    }

    public enum SignInStatus
    {
        Success,
        LockedOut,
        RequiresTwoFactorAuthentication,
        Failure
    }

    // These help with sign and two factor (will possibly be moved into identity framework itself)
    public class SignInHelper
    {
        public SignInHelper(ApplicationUserManager userManager, IAuthenticationManager authManager)
        {
            UserManager = userManager;
            AuthenticationManager = authManager;
        }

        public ApplicationUserManager UserManager { get; private set; }
        public IAuthenticationManager AuthenticationManager { get; private set; }

        public async Task SignInAsync(ApplicationUser user, bool isPersistent, bool rememberBrowser)
        {
            // Clear any partial cookies from external or two factor partial sign ins
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            var userIdentity = await user.GenerateUserIdentityAsync(UserManager);
            if (rememberBrowser)
            {
                var rememberBrowserIdentity = AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(user.Id);
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, userIdentity, rememberBrowserIdentity);
            }
            else
            {
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, userIdentity);
            }
        }

        public async Task<bool> SendTwoFactorCode(string provider)
        {
            var userId = await GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return false;
            }

            var token = await UserManager.GenerateTwoFactorTokenAsync(userId, provider);
            // See IdentityConfig.cs to plug in Email/SMS services to actually send the code
            await UserManager.NotifyTwoFactorTokenAsync(userId, provider, token);
            return true;
        }

        public async Task<string> GetVerifiedUserIdAsync()
        {
            var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.TwoFactorCookie);
            if (result != null && result.Identity != null && !String.IsNullOrEmpty(result.Identity.GetUserId()))
            {
                return result.Identity.GetUserId();
            }
            return null;
        }

        public async Task<bool> HasBeenVerified()
        {
            return await GetVerifiedUserIdAsync() != null;
        }

        public async Task<SignInStatus> TwoFactorSignIn(string provider, string code, bool isPersistent, bool rememberBrowser)
        {
            var userId = await GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return SignInStatus.Failure;
            }
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return SignInStatus.Failure;
            }
            if (await UserManager.IsLockedOutAsync(user.Id))
            {
                return SignInStatus.LockedOut;
            }
            if (await UserManager.VerifyTwoFactorTokenAsync(user.Id, provider, code))
            {
                // When token is verified correctly, clear the access failed count used for lockout
                await UserManager.ResetAccessFailedCountAsync(user.Id);
                await SignInAsync(user, isPersistent, rememberBrowser);
                return SignInStatus.Success;
            }
            // If the token is incorrect, record the failure which also may cause the user to be locked out
            await UserManager.AccessFailedAsync(user.Id);
            return SignInStatus.Failure;
        }

        public async Task<SignInStatus> ExternalSignIn(ExternalLoginInfo loginInfo, bool isPersistent)
        {
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user == null)
            {
                return SignInStatus.Failure;
            }
            if (await UserManager.IsLockedOutAsync(user.Id))
            {
                return SignInStatus.LockedOut;
            }
            return await SignInOrTwoFactor(user, isPersistent);
        }

        public async Task<SignInStatus> SignInOrTwoFactor(ApplicationUser user, bool isPersistent)
        {
            if (await UserManager.GetTwoFactorEnabledAsync(user.Id) &&
                !await AuthenticationManager.TwoFactorBrowserRememberedAsync(user.Id))
            {
                var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                AuthenticationManager.SignIn(identity);
                return SignInStatus.RequiresTwoFactorAuthentication;
            }
            await SignInAsync(user, isPersistent, false);
            return SignInStatus.Success;

        }
        public async Task<SignInStatus> PassworCheck(Units unit, string userName, string password, bool isPersistent, bool shouldLockout)
        {
            var user = await UserManager.FindByNameAsync(userName);

            if (user == null)
            {
                return SignInStatus.Failure;
            }

            if (userName != SysBaseInfor.SuperAdmin)
            {
                if (string.IsNullOrEmpty(unit.Code) || user.UnitName.ToUpper() != unit.Code.ToUpper() || unit.Active == false)
                {
                    return SignInStatus.Failure;
                }
            }

            if (user.LockoutEnabled)
            {
                return SignInStatus.LockedOut;
            }

            if (await UserManager.CheckPasswordAsync(user, password))
            {
                return  SignInStatus.Success;
            }

            if (shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await UserManager.AccessFailedAsync(user.Id);
                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    return SignInStatus.LockedOut;
                }
            }

            return SignInStatus.Failure;
        }
        public async Task<SignInStatus> PasswordSignIn(Units unit, string userName, string password, bool isPersistent, bool shouldLockout)
        {
            var user = await UserManager.FindByNameAsync(userName);

            if (user == null)
            {
                return SignInStatus.Failure;
            }

            if (userName != SysBaseInfor.SuperAdmin)
            {
                if (string.IsNullOrEmpty(unit.Code) || user.UnitName.ToUpper() != unit.Code.ToUpper() || unit.Active == false)
                {
                    return SignInStatus.Failure;
                }
            }

            if (user.LockoutEnabled)
            {
                return SignInStatus.LockedOut;
            }

            if (await UserManager.CheckPasswordAsync(user, password))
            {
                return await SignInOrTwoFactor(user, isPersistent);
            }

            if (shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await UserManager.AccessFailedAsync(user.Id);
                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    return SignInStatus.LockedOut;
                }
            }

            return SignInStatus.Failure;
        }
    }
}
