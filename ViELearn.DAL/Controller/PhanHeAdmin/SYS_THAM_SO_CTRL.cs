using System;
using System.Collections.Generic;
using ViELearn.Models.ViELearnModels;

namespace ViELearn.DAL.Controller
{
    public class SYS_THAM_SO_CTRL : DALBaseClass<SYS_THAM_SO>
    {
        public SYS_THAM_SO_CTRL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public SYS_THAM_SO_CTRL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public void DeleteItemById(int id)
        {
            _db.Delete("SYS_THAM_SO", "ID_TS", null,id);
        }

        public IEnumerable<SYS_THAM_SO> GetListSysParameterByName(string name)
        {
            var results = _db.Query<SYS_THAM_SO>("Select GIA_TRI_TS,TEN_TS From SYS_THAM_SO Where NHOM_TS = @0", name);
            return results;
        }
    }
}