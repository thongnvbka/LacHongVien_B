using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class PhuongTienChuaChay_BaoDuongDAL : DALBaseClass<PhuongTienChuaChay_BaoDuong>
    {
        public PhuongTienChuaChay_BaoDuongDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }
        public PhuongTienChuaChay_BaoDuongDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<PhuongTienChuaChay_BaoDuong> GetLstByLoaiSuaChua(int phuongTienChuaChayId,int loaiSuaChua)
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM PhuongTienChuaChay_BaoDuong
                                        WHERE PhuongTienChuaChayId = {0} AND LoaiSuaChua = {1}
                                        Order By Stt ASC", phuongTienChuaChayId, loaiSuaChua);

                return _db.Fetch<PhuongTienChuaChay_BaoDuong>(sql);
            }
            catch (Exception ex)
            {
                return new List<PhuongTienChuaChay_BaoDuong>();
            }
        }

        public int GetTotalPhuongTienChuaChay_BaoDuong()
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
