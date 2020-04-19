using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class ModulesDAL : DALBaseClass<Modules>
    {
        public ModulesDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public ModulesDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<Modules> GetListModulesByUnitId(int unitId)
        {
            var sql = String.Format("SELECT * FROM Modules WHERE UnitID = {0}", unitId);
            return _db.Fetch<Modules>(sql).ToList();
        }
    }
}
