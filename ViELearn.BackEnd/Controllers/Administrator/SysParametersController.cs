using ViELearn.DAL.ModulesDAL;
using ViELearn.Models.ProjectModels;
using ViELearn.BackEnd.Ultilities;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace ViELearn.BackEnd.Controllers.Administrator
{
    public class SysParametersController : Controller
    {
        protected static string urlView = "~/Views/SysParameters/";
        //[ProjectCustomFilter(UserPermissions.QuyenXem)]
        public ActionResult Index()
        {
            return View(urlView + "Index.cshtml");
        }

        [HttpPost]
        public JsonResult GetListSysParameters(byte? IPS, int? limit, int? offset)
        {
            try
            {
                SystemParametersDAL SystemParametersCtrl = new SystemParametersDAL();
                var lstResult = new List<SystemParameters>();
                if (IPS == 1)
                {
                    lstResult = SystemParametersCtrl.GetListItems();
                    return Json(lstResult);
                }
                else
                {
                    lstResult = SystemParametersCtrl.GetListItems();
                    return Json(new { total = SystemParametersCtrl.GetTotalSysParameter(), rows = lstResult });
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        public ActionResult ShowDetailSysParameter(int IdSysParameter)
        {
            try
            {
                SystemParametersDAL SystemParametersCtrl = new SystemParametersDAL();

                var result = new SystemParameters();
                if (IdSysParameter > 0)
                    result = SystemParametersCtrl.GetItemByID("Id", IdSysParameter);

                return PartialView(urlView + "_DetailSysParameter.cshtml", result); 
            }
            catch (Exception ex)
            {
                return PartialView(urlView + "_DetailSysParameter.cshtml", ex);
            }
        }

        public ActionResult SaveSysParameter(SystemParameters objSysParameter)
        {
            try
            {
                SystemParametersDAL objCtrl = new SystemParametersDAL();                
                if (objSysParameter.Id > 0)
                {                    
                    objCtrl.UpdateItem(objSysParameter);
                }
                else
                {                    
                    objCtrl.CreateItem(objSysParameter);
                }

                return PartialView(urlView + "_DetailSysParameter.cshtml", objSysParameter);
            }
            catch (Exception ex)
            {
                return PartialView(urlView + "_DetailSysParameter.cshtml", new Modules());
            }
        }

        [HttpPost]
        public ActionResult DeleteSysParameter(int idSysParameter)
        {
            //try
            //{
            //    SystemParametersDAL objCtrl = new SystemParametersDAL();
            //    var tmpResult = objCtrl.DeleteItemByValues("Id", idSysParameter);
            //    return Content("Success");
            //}
            //catch (Exception ex)
            //{
                return Content("Không thể xóa! Vui lòng kiểm tra lại.");
            //}
        }        
    }
}