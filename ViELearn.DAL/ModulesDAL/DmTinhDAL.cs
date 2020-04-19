using System;
using System.Collections.Generic;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class DmTinhDAL : DALBaseClass<DmTinh>
    {
        public DmTinhDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }
        public DmTinhDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<DmTinh> GetLstByOrderView()
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM DmTinh
                                        Order By MaDanhMuc,ThuTu DESC");

                return _db.Fetch<DmTinh>(sql);
            }
            catch (Exception ex)
            {
                return new List<DmTinh>();
            }
        }

        public List<DmTinh> GetLstByCode(string maDanhMuc)
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM DmTinh
                                        WHERE MaDanhMuc='{0}'
                                        Order By ThuTu ASC", maDanhMuc);

                return _db.Fetch<DmTinh>(sql);
            }
            catch (Exception ex)
            {
                return new List<DmTinh>();
            }
        }
        public int GetTotalDmTinh()
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
