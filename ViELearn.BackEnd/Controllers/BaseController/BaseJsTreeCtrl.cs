using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ViELearn.DAL.ModulesDAL;
using ViELearn.Models.ProjectModels;
using ViELearn.Models.SystemModels;
using ViELearn.BackEnd.Ultilities;

namespace ViELearn.BackEnd.Controllers
{
    public class BaseJsTreeCtrl : Controller
    {
        protected JsTreeModel FindNodeInTree(List<JsTreeModel> tmpTree, int findID)
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
        protected List<JsTreeModel> AddNodeTree(List<FrontendMenu> lstDanhMuc, int level, int idUp)
        {
            List<JsTreeModel> lstJsTree = new List<JsTreeModel>();
            foreach (FrontendMenu model in lstDanhMuc)
            {
                if (model.MenuLevel == level && model.ParrentId == idUp && !String.IsNullOrEmpty(model.Name))
                {
                    JsTreeModel tmpNode = new JsTreeModel();
                    tmpNode.text = model.Name;
                    tmpNode.id = model.Id.ToString();
                    tmpNode.idParrent = model.ParrentId.GetValueOrDefault();
                    if (model.HaveChild == 1) tmpNode.children = AddNodeTree(lstDanhMuc, level + 1, model.Id);
                    lstJsTree.Add(tmpNode);
                }
            }
            return lstJsTree;
        }
        protected List<JsTreeModel> CreateTreeData()
        {
            FrontendMenuDAL frontendMenuCtrl = new FrontendMenuDAL();
            var lstDanhMuc = frontendMenuCtrl.GetListItemsHaveWhere(string.Format("UnitId = {0} AND Status >= {1} ORDER BY OrderView", SysBaseInfor.GetCurrentUnitId(), 1));
            List<JsTreeModel> tree = new List<JsTreeModel>();
            JsTreeModel tmpNode = new JsTreeModel();
            tmpNode.text = "Cây Danh Mục Hiển thị ";
            tmpNode.id = "0";
            tmpNode.idParrent = 0;
            tmpNode.state = new { opened = true };
            tmpNode.children = AddNodeTree(lstDanhMuc, 0, 0);
            tree.Add(tmpNode);
            return tree;
        }
    }
}