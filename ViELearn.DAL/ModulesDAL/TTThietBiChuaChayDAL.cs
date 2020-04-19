using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class TTThietBiChuaChayDAL : DALBaseClass<TTThietBiChuaChay>
    {
        public TTThietBiChuaChayDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }
        public TTThietBiChuaChayDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<TTThietBiChuaChay> GetLstByOrderView()
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM TTThietBiChuaChay
                                        Order By NgayTao DESC");

                return _db.Fetch<TTThietBiChuaChay>(sql);
            }
            catch (Exception ex)
            {
                return new List<TTThietBiChuaChay>();
            }
        }
        public int GetTotalTTThietBiChuaChay()
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
