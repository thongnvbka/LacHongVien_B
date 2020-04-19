using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class SpecificationParametersDAL : DALBaseClass<SpecificationParameters>
    {
        public SpecificationParametersDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }
        public SpecificationParametersDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<SpecificationParameters> GetLstByOrderView()
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM SpecificationParameters
                                        Order By MaDanhMuc,ThuTu DESC");

                return _db.Fetch<SpecificationParameters>(sql);
            }
            catch (Exception ex)
            {
                return new List<SpecificationParameters>();
            }
        }

        public List<SpecificationParameters> GetLstByCode(string maDanhMuc)
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM SpecificationParameters
                                        WHERE MaDanhMuc='{0}'
                                        Order By ThuTu ASC", maDanhMuc);

                return _db.Fetch<SpecificationParameters>(sql);
            }
            catch (Exception ex)
            {
                return new List<SpecificationParameters>();
            }
        }

        public List<SpecificationParameters> GetLstByParrentId(int parrentId)
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM SpecificationParameters
                                        WHERE DanhMucChaId='{0}'
                                        Order By ThuTu ASC", parrentId);

                return _db.Fetch<SpecificationParameters>(sql);
            }
            catch (Exception ex)
            {
                return new List<SpecificationParameters>();
            }
        }

        public List<SpecificationParameters> GetLstByParrentId(int parrentId,int type)
        {
            try
            {
                var sql = "";
                if (type == 0)
                {
                    sql = string.Format(@"SELECT *
                                        FROM SpecificationParameters
                                        WHERE DanhMucChaId='{0}' AND Thutu < 100
                                        Order By ThuTu ASC", parrentId);
                }                    
                else
                {
                    sql = string.Format(@"SELECT *
                                        FROM SpecificationParameters
                                        WHERE DanhMucChaId='{0}' AND Thutu >= 100
                                        Order By ThuTu ASC", parrentId);
                }
                return _db.Fetch<SpecificationParameters>(sql);
            }
            catch (Exception ex)
            {
                return new List<SpecificationParameters>();
            }
        }


        public List<SpecificationParameters> GetLstMaDanhMuc()
        {
            try
            {
                var sql = string.Format(@"SELECT DISTINCT MaDanhMuc
                                        FROM SpecificationParameters
                                        ORDER BY MaDanhMuc ASC");

                return _db.Fetch<SpecificationParameters>(sql);
            }
            catch (Exception ex)
            {
                return new List<SpecificationParameters>();
            }
        }
        public int GetTotalSpecificationParameters()
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
