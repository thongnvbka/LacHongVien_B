using System.Collections.Generic;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{ 
    public partial class FrontendMenuDAL : DALBaseClass<FrontendMenu>
    {
        public FrontendMenuDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public FrontendMenuDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public string GetNameByID(int id)
        {
            var sql = string.Format("Select Name from FrontendMenu where id = {0}",id);
            return _db.SingleOrDefault<string>(sql);
        }

        public int ResetDefaultMainPage(int idUnit)
        {
            var sql = string.Format("Update FrontendMenu SET Status = 1 WHERE UnitId = {0} AND Status = 2", idUnit);
            return _db.Execute(sql);
        }

        public List<FrontendMenu> GetLstByUnit(int idUnit)
        {
            var sql = string.Format("SELECT * FROM FrontendMenu WHERE UnitId = {0} AND Status > 0", idUnit);
            return _db.Fetch<FrontendMenu>(sql);
        }

        public List<FrontendMenu> GetLstByUnitAndUser(int unitId,string userId)
        {
            var sql = string.Format(@"SELECT *
                                        FROM dbo.FrontendMenu fm
                                        WHERE
                                            fm.UnitId = {0}
                                        AND
                                            fm.Status > 0
                                        AND
                                            (fm.AccessType = 0
                                                OR fm.Id IN (SELECT DISTINCT fmr.FrontendMenuId
                                                                FROM dbo.AspNetUserRoles anur
                                                                INNER JOIN FrontendMenuRole fmr ON anur.RoleId = fmr.RoleId WHERE UserId = '{1}'))", unitId,userId);
            return _db.Fetch<FrontendMenu>(sql);
        }
    }
}