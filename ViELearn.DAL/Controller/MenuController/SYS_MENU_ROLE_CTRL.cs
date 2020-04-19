using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using ViELearn.Common;
using ViELearn.Models.ViELearnModels;

namespace ViELearn.DAL.Controller.MenuController
{
    public class SYS_MENU_ROLE_CTRL : DALBaseClass<SYS_MENU_ROLE>
    {
        public SYS_MENU_ROLE_CTRL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public SYS_MENU_ROLE_CTRL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);             
        }

        public List<SYS_MENU_ROLE> GetLstSysMenuRoleWithView(string roleId)
        {
            return _db.Fetch<SYS_MENU_ROLE>(String.Format(@"
            SELECT 
                smr.*,
                sm.CapDo,
                sm.DanhMucCha,
                sm.DanhMucCon,
                sm.TenDanhMuc,
                sm.Action,
                sm.Controller,
                sm.Class,
                sm.Class_main 
            FROM SYS_MENU_ROLE smr 
                INNER JOIN SYS_Menu sm ON smr.ID_SYS_MENU = sm.ID_sys_menu AND smr.ID_ROLE = '{0}'
            WHERE sm.Controller IS NOT NULL AND Status = 1
            ORDER BY sm.CapDo, sm.OrderView", roleId)).ToList();

            //DataTable dt = new DataTable();
            //try
            //{
            //    Hashtable args = new Hashtable();
            //    args.Add("role_id", roleId);
            //    dt = DBLibs.ExecuteStoreProcedure_Select("Proc_LayDanhSachMenu", args, ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            //}
            //catch { dt = new DataTable(); }

            //List<SYS_MENU_ROLE> items = dt.AsEnumerable().Select(m => new SYS_MENU_ROLE()
            //{
            //    ACCESS_RIGHT = m["ACCESS_RIGHT"].MapStr(),
            //    Action = m["Action"].MapStr(),
            //    CAPDO = (byte)m["CapDo"].ToInt32(),
            //    Class = m["Class"].MapStr(),
            //    CLASS_MAIN = m["Class_main"].MapStr(),
            //    Controller = m["Controller"].MapStr(),
            //    DANHMUCCHA = (byte)m["DanhMucCha"].ToInt32(),
            //    DANHMUCCON = (byte)m["DanhMucCon"].ToInt32(),
            //    ID = m["ID"].MapInt(),
            //    ID_ROLE = m["ID_ROLE"].MapStr(),
            //    ID_SYS_MENU = m["ID_SYS_MENU"].MapInt(),
            //    TenDanhMuc = m["TenDanhMuc"].MapStr()
            //}).ToList();

            //return items;
        }
    }
}