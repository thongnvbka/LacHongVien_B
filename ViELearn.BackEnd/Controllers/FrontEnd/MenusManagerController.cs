using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using QLVC.DAL.SystemDAL;
using QLVC.Models.SystemModels;
using QuanLyVuChay.Ultilities;

namespace ViELearn.BackEnd.Controllers
{
    [Authorize(Roles ="HostAdmin")]
    public class MenusManagerController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var model = CreateTreeData();
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult ViewThongTinChiTietDanhMuc(int IdDanhMuc)
        {
            Sys_MenuController ctrlSysMenu = new Sys_MenuController();
            SYS_MENU objSysMenu = ctrlSysMenu.GetItemByID("ID_SYS_MENU",IdDanhMuc);
            return PartialView("~/Views/TreeMenuManage/_ThongTinChiTietDanhMuc.cshtml", objSysMenu);
        }
        [HttpGet]
        public ActionResult ViewTaoMoiDanhMuc(int IdDanhMuc)
        {
            Sys_MenuController ctrlSysMenu = new Sys_MenuController();
            SYS_MENU objSysMenu = ctrlSysMenu.GetItemByID("ID_SYS_MENU", IdDanhMuc);
            objSysMenu.CAPDO += 1;
            return PartialView("~/Views/TreeMenuManage/_TaoMoiDanhMuc.cshtml", objSysMenu);
        }
        [HttpPost]
        public ActionResult SaveThongTinChiTietDanhMuc(SYS_MENU objSysMenu)
        {
            Sys_MenuController ctrlSysMenu = new Sys_MenuController();
            objSysMenu.UPDATEAT = DateTime.Now;
            ctrlSysMenu.UpdateItem(objSysMenu);
            return PartialView("~/Views/TreeMenuManage/_ThongTinChiTietDanhMuc.cshtml", objSysMenu);
        }
        [HttpPost]
        public ActionResult SaveTaoMoiDanhMuc(SYS_MENU objSysMenu)
        {
            Sys_MenuController ctrlSysMenu = new Sys_MenuController();
            objSysMenu.CREATEDATE = DateTime.Now;
            objSysMenu.Status = true;
            ctrlSysMenu.CreateItem(objSysMenu);

            var newSysMenu = new SYS_MENU();
            newSysMenu.CAPDO = objSysMenu.CAPDO;
            newSysMenu.DANHMUCCHA = objSysMenu.DANHMUCCHA;
            return PartialView("~/Views/TreeMenuManage/_TaoMoiDanhMuc.cshtml",newSysMenu);
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
                    if (model.DANHMUCCON == 1) tmpNode.children = AddNodeTree(lstDanhMuc, level + 1, model.ID_SYS_MENU);
                    lstJsTree.Add(tmpNode);
                }
            }
            return lstJsTree;
        }
        private List<JsTreeModel> CreateTreeData()
        {
            Sys_MenuController sysMenuCtrl = new Sys_MenuController();
            var lstDanhMuc = sysMenuCtrl.GetListItemsOrderBy("OrderView", "asc");
            List<JsTreeModel> tree = new List<JsTreeModel>();
            tree = AddNodeTree(lstDanhMuc, 0, 0);
            return tree;
        }
    }
}