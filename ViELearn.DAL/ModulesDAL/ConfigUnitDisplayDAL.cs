using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class ConfigUnitDisplayDAL : DALBaseClass<ConfigUnitDisplay>
    {
        public ConfigUnitDisplayDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public ConfigUnitDisplayDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public bool CheckExist(int idUnit)
        {
            try
            {
                return _db.Exists<ConfigUnitDisplay>(idUnit);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
