using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using ViELearn.DAL.SystemDAL;
using ViELearn.Models.SystemModels;
using ViELearn.BackEnd.Models;
using ViELearn.BackEnd.Ultilities;
using System.Configuration;

namespace ViELearn.BackEnd.Controllers
{    
    public class SysMenuManageController : Controller
    {
        protected static string blankViewUrl = "~/Views/Shared/_Blank.cshtml";
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //[ProjectCustomFilter(UserPermissions.QuyenXem)]
        public ActionResult Index()
        {
            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            var GetCurrentUserName = SysBaseInfor.GetCurrentUserName().ToLower();
            if (SysBaseInfor.GetCurrentUserName().ToLower() == SysBaseInfor.SuperAdmin)
            {
                //var lstRoles = roleManager.Roles;
                //ViewBag.SelectLstRoles = new SelectList(lstRoles, "ID", "Name", "RoleType", 0);
                var sql = "SELECT * FROM AspNetRoles ORDER BY RoleType";
                ViewBag.SelectLstRoles = DBLibs.GetDataBy_DataAdapter(sql, _cnn);
            }
            else
            {
                var lstRoles = roleManager.GetListRoleByUnitId(SysBaseInfor.GetCurrentUnitCode().ToLower() + SysBaseInfor.SignProject, SysBaseInfor.GetCurrentUnitId().ToInt32());
                ViewBag.SelectLstRoles = new SelectList(lstRoles, "ID", "Name", "RoleType", 0);
            }
            return View();
        }

        [HttpPost]
        public JsonResult SaveRoleAccessRight(JsonTreeData objectJson)
        {
            try
            {
                if (objectJson != null && !String.IsNullOrEmpty(objectJson.NhomNguoiDung))
                {
                    SYS_MENU_ROLE_CTRL smrCtrl = new SYS_MENU_ROLE_CTRL();
                    smrCtrl.DeleteItemByValues("ID_ROLE", objectJson.NhomNguoiDung);
                    if (objectJson.DanhSachQuyen != null)
                    {
                        foreach (JsonNodeList tmpNode in objectJson.DanhSachQuyen)
                        {
                            var tmpRightAccess = tmpNode.data;
                            string strRightAccess = "";

                            foreach (var prop in tmpRightAccess.GetType().GetProperties())
                            {
                                var name = prop.Name;
                                var value = prop.GetValue(tmpRightAccess, null);
                                strRightAccess += (value.ToBoolean() == true) ? "1" : "0";
                            }

                            SYS_MENU_ROLE smrObj = new SYS_MENU_ROLE();
                            smrObj.ACCESS_RIGHT = strRightAccess;
                            smrObj.ID_ROLE = objectJson.NhomNguoiDung;
                            smrObj.ID_SYS_MENU = tmpNode.id.ToInt32();
                            smrCtrl.CreateItem(smrObj);
                        }
                    }

                    if (objectJson.DanhSachUndetermineds != null)
                    {
                        foreach (int tmpI in objectJson.DanhSachUndetermineds)
                        {
                            string strRightAccess = "1000000";
                            SYS_MENU_ROLE smrObj = new SYS_MENU_ROLE();
                            smrObj.ACCESS_RIGHT = strRightAccess;
                            smrObj.ID_ROLE = objectJson.NhomNguoiDung;
                            smrObj.ID_SYS_MENU = tmpI;
                            smrCtrl.CreateItem(smrObj);
                        }
                    }
                }
                return Json(new { Result = String.Format("Fist item in list: '{0}'", 1) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Fail to add" });
            }
        }

        public ActionResult GetRoleAccessRight(string roleId, int? isPopup)
        {

            try
            {
                if (!String.IsNullOrEmpty(roleId))
                {
                    var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
                    ViewBag.NhomNguoiDungKey = roleId;
                    ViewBag.NhomNguoiDungName = roleManager.FindById(roleId).Name.Replace("_" + SysBaseInfor.GetCurrentUnitCode().ToLower() + SysBaseInfor.SignProject, "");
                    var model = CreateTreeData();
                    SYS_MENU_ROLE_CTRL smrCtrl = new SYS_MENU_ROLE_CTRL();
                    var nhomNguoiDungLstRoles = smrCtrl.GetListItemsHaveWhere(String.Format("ID_ROLE = '{0}'", roleId));
                    foreach (SYS_MENU_ROLE tmpSmr in nhomNguoiDungLstRoles)
                    {
                        JsTreeModel tmpNode = FindNodeInTree(model, tmpSmr.ID_SYS_MENU);

                        if (tmpNode != null && (tmpNode.children == null || tmpNode.children.Count == 0))
                        {
                            tmpNode.data = new
                            {
                                quyenXem = (tmpSmr.ACCESS_RIGHT[0] == '1') ? true : false,
                                quyenThemMoi = (tmpSmr.ACCESS_RIGHT[1] == '1') ? true : false,
                                quyenSua = (tmpSmr.ACCESS_RIGHT[2] == '1') ? true : false,
                                quyenXoa = (tmpSmr.ACCESS_RIGHT[3] == '1') ? true : false,
                                quyenChuyen = (tmpSmr.ACCESS_RIGHT[4] == '1') ? true : false,
                                quyenDuyet = (tmpSmr.ACCESS_RIGHT[5] == '1') ? true : false,
                                quyenXuatBan = (tmpSmr.ACCESS_RIGHT[6] == '1') ? true : false
                            };
                            tmpNode.state = new { opened = true, @checked = true };
                        }
                    }
                    if (isPopup > 0)
                        return PartialView("~/Views/SysMenuManage/_ModalTreeRightSelect.cshtml", model);
                    else
                        return PartialView("~/Views/SysMenuManage/_TreeRightSelect.cshtml", model);
                }
                return PartialView(blankViewUrl);
            }
            catch (Exception ex)
            {
                return PartialView(blankViewUrl);
            }
        }

        public ActionResult GetUserAccessRight(string userId, int? isPopup)
        {
            try
            {
                if (!String.IsNullOrEmpty(userId))
                {
                    var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    var user = userManager.FindById(userId);
                    if (user != null)
                    {
                        var roleForUserName = user.UserName + "_" + SysBaseInfor.GetCurrentUnitCode().ToLower() + SysBaseInfor.SignProject;
                        var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
                        var roleForUser = roleManager.FindByName(roleForUserName);
                        if (roleForUser == null)
                        {
                            roleForUser = new ApplicationRole(roleForUserName);
                            roleForUser.RoleType = 3;
                            roleForUser.UnitId = SysBaseInfor.GetCurrentUnitId().ToInt32();
                            var roleResult = roleManager.Create(roleForUser);
                            if (roleResult.Succeeded)
                            {
                                userManager.AddToRole(user.Id, roleForUserName);
                            }
                        }

                        if (!string.IsNullOrEmpty(roleForUser.Id))
                        {
                            var roleId = roleForUser.Id;
                            ViewBag.NhomNguoiDungKey = roleId;
                            ViewBag.NhomNguoiDungName = roleForUserName.Replace("_" + SysBaseInfor.GetCurrentUnitCode().ToLower() + SysBaseInfor.SignProject, "");
                            ViewBag.RoleType = roleForUser.RoleType;
                            var model = CreateTreeData();
                            SYS_MENU_ROLE_CTRL smrCtrl = new SYS_MENU_ROLE_CTRL();
                            var nhomNguoiDungLstRoles = smrCtrl.GetListItemsHaveWhere(String.Format("ID_ROLE = '{0}'", roleId));
                            foreach (SYS_MENU_ROLE tmpSmr in nhomNguoiDungLstRoles)
                            {
                                JsTreeModel tmpNode = FindNodeInTree(model, tmpSmr.ID_SYS_MENU);

                                if (tmpNode != null && (tmpNode.children == null || tmpNode.children.Count == 0))
                                {
                                    tmpNode.data = new
                                    {
                                        quyenXem = (tmpSmr.ACCESS_RIGHT[0] == '1') ? true : false,
                                        quyenThemMoi = (tmpSmr.ACCESS_RIGHT[1] == '1') ? true : false,
                                        quyenSua = (tmpSmr.ACCESS_RIGHT[2] == '1') ? true : false,
                                        quyenXoa = (tmpSmr.ACCESS_RIGHT[3] == '1') ? true : false,
                                        quyenChuyen = (tmpSmr.ACCESS_RIGHT[4] == '1') ? true : false,
                                        quyenDuyet = (tmpSmr.ACCESS_RIGHT[5] == '1') ? true : false,
                                        quyenXuatBan = (tmpSmr.ACCESS_RIGHT[6] == '1') ? true : false
                                    };
                                    tmpNode.state = new { opened = true, @checked = true };
                                }
                            }
                            if (isPopup > 0)
                                return PartialView("~/Views/SysMenuManage/_ModalTreeRightSelect.cshtml", model);
                            else
                                return PartialView("~/Views/SysMenuManage/_TreeRightSelect.cshtml", model);
                        }
                    }

                }
                return PartialView(blankViewUrl);
            }
            catch (Exception ex)
            {
                return PartialView(blankViewUrl);
            }
        }

        private JsTreeModel FindNodeInTree(List<JsTreeModel> tmpTree, int findID)
        {
            foreach (JsTreeModel tmpNode in tmpTree)
            {
                if (tmpNode.id.ToInt32() == findID) return tmpNode;
                else
                {
                    if (tmpNode.children != null)
                    {
                        var tmpResult = FindNodeInTree(tmpNode.children, findID);
                        if (tmpResult != null) return tmpResult;
                    }
                }
            }
            return null;
        }
        private List<JsTreeModel> AddNodeTree(List<SYS_MENU> lstDanhMuc, int level, int idUp)
        {
            List<JsTreeModel> lstJsTree = new List<JsTreeModel>();
            foreach (SYS_MENU model in lstDanhMuc)
            {
                if (model.CAPDO == level && model.DANHMUCCHA == idUp && !String.IsNullOrEmpty(model.TENDANHMUC))
                {
                    JsTreeModel tmpNode = new JsTreeModel();
                    tmpNode.text = model.TENDANHMUC;
                    tmpNode.id = model.ID_SYS_MENU.ToString();
                    tmpNode.idParrent = model.DANHMUCCHA.GetValueOrDefault();
                    if (model.DANHMUCCON == 1) tmpNode.children = AddNodeTree(lstDanhMuc, level + 1, model.ID_SYS_MENU);
                    lstJsTree.Add(tmpNode);
                }
            }
            return lstJsTree;
        }
        private List<JsTreeModel> CreateTreeData()
        {
            Sys_MenuController sysMenuCtrl = new Sys_MenuController();
            var lstDanhMuc = sysMenuCtrl.GetLstHaveOrder();
            List<JsTreeModel> tree = new List<JsTreeModel>();
            tree = AddNodeTree(lstDanhMuc, 0, 0);
            return tree;
        }
    }
}