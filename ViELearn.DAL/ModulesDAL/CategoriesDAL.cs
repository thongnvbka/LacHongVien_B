using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class CategoriesDAL : DALBaseClass<NewsCategories>
    {
        public CategoriesDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public CategoriesDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public int GetTotal()
        {
            try
            {
                var sql = string.Format("SELECT Total_Rows= SUM(st.row_count) FROM sys.dm_db_partition_stats st WHERE object_name(object_id) = '{0}' AND (index_id < 2)", "NewsCategories");
                return _db.SingleOrDefault<int>(sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<NewsCategories> GetLstAll(int idUnit)
        {
            try
            {
                var sql = string.Format(@"SELECT nc.*,sp.Name as TypeName
                                          FROM NewsCategories nc
                                          INNER JOIN SystemParameters sp ON sp.code='CategoryType' AND sp.Value = nc.Type
                                          WHERE nc.UnitID = {0}
                                          ORDER BY nc.OrderView,nc.Type ASC ", idUnit);

                return _db.Fetch<NewsCategories>(sql).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<NewsCategories> GetLstByUnit(int idUnit )
        {
            try
            {
                var sql = string.Format(@"SELECT nc.*,sp.Name as TypeName
                                          FROM NewsCategories nc
                                          INNER JOIN SystemParameters sp ON sp.code='CategoryType' AND sp.Value = nc.Type
                                          WHERE nc.UnitID = {0} AND nc.Active = 1 
                                          ORDER BY nc.OrderView,nc.Type ASC ", idUnit);

                return _db.Fetch<NewsCategories>(sql).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<NewsCategories> GetLstByType(int idUnit,int type)
        {
            try
            {
                var sql = string.Format(@"SELECT nc.*,sp.Name as TypeName
                                          FROM NewsCategories nc
                                          INNER JOIN SystemParameters sp ON sp.code='CategoryType' AND sp.Value = nc.Type
                                          WHERE nc.UnitID = {0} AND nc.Type = {1} And nc.Active = 1 
                                          ORDER BY nc.Type,nc.OrderView ASC ", idUnit, type);

                return _db.Fetch<NewsCategories>(sql).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int GetTotalNewsInCategory(int categoryId)
        {
            var sql = String.Format(@"SELECT Count(Id)
                                      FROM News                                        
                                      WHERE CategoryId = {0} AND Status = 3", categoryId);
            return _db.SingleOrDefault<int>(sql);
        }
    }
}
