using System;
using System.Collections.Generic;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class DmHuyenDAL : DALBaseClass<DmHuyen>
    {
        public DmHuyenDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }
        public DmHuyenDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<DmHuyen> GetLstByOrderView()
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM DmHuyen
                                        Order By MaDanhMuc,ThuTu DESC");

                return _db.Fetch<DmHuyen>(sql);
            }
            catch (Exception ex)
            {
                return new List<DmHuyen>();
            }
        }

        public List<DmHuyen> GetLstByCode(string maDanhMuc)
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM DmHuyen
                                        WHERE MaDanhMuc='{0}'
                                        Order By ThuTu ASC", maDanhMuc);

                return _db.Fetch<DmHuyen>(sql);
            }
            catch (Exception ex)
            {
                return new List<DmHuyen>();
            }
        }
        public int GetTotalDmHuyen()
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
