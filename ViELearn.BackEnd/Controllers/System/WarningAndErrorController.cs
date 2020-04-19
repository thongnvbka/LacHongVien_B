using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ViELearn.BackEnd.Controllers
{
    public class WarningAndErrorController : Controller
    {
        // GET: WarningAndError
        public ActionResult NeedPermissionWarning()
        {
            return PartialView("~/Views/Shared/NeedPermission.cshtml");
        }

        public ActionResult NotFound()
        {
            return View();
        }
    }
}