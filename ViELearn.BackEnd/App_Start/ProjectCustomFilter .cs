using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
//using ViELearn.Models.SystemModels;
using ViELearn.Models.ViELearnModels;
using ViELearn.BackEnd.Ultilities;

namespace ViELearn.BackEnd
{
    public class ProjectCustomFilter : AuthorizeAttribute
    {
        public IEnumerable<UserPermissions> PermissionsRequired { get; set; }

        public ProjectCustomFilter()
        {
        }

        public ProjectCustomFilter(params UserPermissions[] permissionsRequired)
        {
            PermissionsRequired = permissionsRequired;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                var controllerName = filterContext.RouteData.Values["controller"];
                var actionName = filterContext.RouteData.Values["action"];
                var tmpAuth = PermissionsRequired;
                var tmpLstRoles = SysBaseInfor.GetListSysMenu();
                var lstConvert = JsonConvert.DeserializeObject<List<SYS_MENU_ROLE>>(tmpLstRoles);
                var tmpSysMenu = lstConvert.Find(x => x.Controller == controllerName.ToString());
                bool userOk = true;

                if (tmpSysMenu != null)
                {
                    foreach (UserPermissions tmpPermission in tmpAuth)
                    {
                        int noR = (int)tmpPermission;
                        if (tmpSysMenu.ACCESS_RIGHT[noR] == '0')
                        {
                            userOk = false;
                            break;
                        }
                    }
                }

                if (tmpSysMenu != null && userOk)
                {
                    base.OnAuthorization(filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "WarningAndError" }, { "action", "NeedPermissionWarning" } });                    
                }
            }
            catch (Exception ex)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "WarningAndError" }, { "action", "NeedPermissionWarning" } });
            }
            
            base.OnAuthorization(filterContext);
        }
    }
}