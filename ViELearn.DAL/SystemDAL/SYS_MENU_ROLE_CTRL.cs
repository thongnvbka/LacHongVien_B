using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.SystemModels;

namespace ViELearn.DAL.SystemDAL
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
            WHERE
                sm.Status = 1
            ORDER BY sm.OrderView", roleId)).ToList();
        }
    }
}