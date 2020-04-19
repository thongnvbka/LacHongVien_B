using System.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.SystemModels;

namespace ViELearn.DAL.SystemDAL
{ 
    public partial class Sys_MenuController : DALBaseClass<SYS_MENU>
    {
        public Sys_MenuController()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public Sys_MenuController(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<SYS_MENU> GetListSysMenuWithRight(string idRole)
        {
            return _db.Query<SYS_MENU>(String.Format("SELECT sm.*,sr.ACCESS_RIGHT FROM SYS_Menu sm INNER JOIN SYS_MENU_ROLE sr ON sm.ID_sys_menu = sr.ID_SYS_MENU AND sr.ID_ROLE = '{0}' AND Status = 1", idRole)).ToList();
        }

        public List<SYS_MENU> GetLstHaveOrder()
        {
            var sql = "SELECT * from Sys_Menu WHERE Status = 1 ORDER BY OrderView ASC";
            return _db.Fetch<SYS_MENU>(sql).ToList();
        }
    }
}