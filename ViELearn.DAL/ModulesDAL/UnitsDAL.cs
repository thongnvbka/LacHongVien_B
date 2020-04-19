using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class UnitsDAL : DALBaseClass<Units>
    {
        public UnitsDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public UnitsDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public Units GetUnitByCode(string code)
        {
            try
            {
                var sql = string.Format("SELECT * FROM Units WHERE Code='{0}'", code);
                return _db.SingleOrDefault<Units>(sql);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Units GetUnitByHost(string host)
        {
            try
            {
                var sql = string.Format(@"SELECT 
                                            un.*,cud.ThemeId,th.LayoutView ,cud.CssName
                                          FROM 
                                            Units un 
                                          INNER JOIN ConfigUnitDisplay cud ON un.Id = cud.UnitId 
                                          INNER JOIN Themes th ON cud.ThemeId = th.Id 
                                          WHERE '{0}'=LOWER(un.Url) OR '{1}'=LOWER(un.SecondUrl)"
                                        , host,host);
                return _db.SingleOrDefault<Units>(sql);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Units GetUnitByHost(List<string> lstHost)
        {
            try
            {
                var innerSql = "";
                foreach (string tmpHost in lstHost)
                {
                    innerSql += string.Format("(CHARINDEX('{0}',un.Url) > 0) OR CHARINDEX('{1}',un.SecondUrl) ", tmpHost);
                }

                var sql = string.Format(@"SELECT 
                                                un.*,th.LayoutView 
                                          FROM 
                                                Units un INNER JOIN Themes th ON th.Id = un.ThemeID 
                                          WHERE {0}"
                                        , innerSql);
                return _db.SingleOrDefault<Units>(sql);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int UpdateMainPage(int mainPageId,int unitId)
        {
            try
            {
                var sql = string.Format("UPDATE Units SET MainPageId = {0} WHERE Id = {1}", mainPageId, unitId);
                return _db.Execute(sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }        

        public int GetTotalUnit()
        {
            try
            {
                var sql = string.Format("SELECT Total_Rows= SUM(st.row_count) FROM sys.dm_db_partition_stats st WHERE object_name(object_id) = '{0}' AND (index_id < 2)","Units");
                return _db.SingleOrDefault<int>(sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
