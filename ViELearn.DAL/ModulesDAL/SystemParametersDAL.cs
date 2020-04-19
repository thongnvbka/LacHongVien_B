using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class SystemParametersDAL : DALBaseClass<SystemParameters>
    {
        public SystemParametersDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public SystemParametersDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<SystemParameters> GetLstByCode(string codeSP)
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM SystemParameters
                                        WHERE Code='{0}' 
                                        Order By OrderView ASC",codeSP);

                return _db.Fetch<SystemParameters>(sql);
            }
            catch (Exception ex)
            {
                return new List<SystemParameters>();
            }
        }
        public int GetTotalSysParameter()
        {
            try
            {
                var sql = string.Format("SELECT Total_Rows= SUM(st.row_count) FROM sys.dm_db_partition_stats st WHERE object_name(object_id) = '{0}' AND (index_id < 2)", "SystemParameters");
                return _db.SingleOrDefault<int>(sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
