using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{ 
    public partial class PageViewModuleDAL : DALBaseClass<PageViewModule>
    {
        public PageViewModuleDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public PageViewModuleDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<PageViewModule> GetListByPageIdAndPosition(int idPage,byte position)
        {
            var sql = string.Format(@"SELECT pwm.*,m.Name as ModuleName,fm.Name as PageName,m.Code as ModuleCode FROM PageViewModule pwm INNER JOIN  Modules m ON m.Id  = pwm.IdModule 
																			  INNER JOIN  FrontendMenu fm ON fm.Id = pwm.IdPage
																			  WHERE pwm.IdPage = {0}  AND pwm.Position = {1} ORDER BY  pwm.ORDERVIEW ASC", idPage, position);
            return _db.Fetch<PageViewModule>(sql).ToList();
        }

        public PageViewModule GetPageViewModuleById(int idPVM)
        {
            var sql = string.Format(@"SELECT pwm.*,m.Name as ModuleName,fm.Name as PageName,m.Code as ModuleCode FROM PageViewModule pwm INNER JOIN  Modules m ON m.Id  = pwm.IdModule 
																			  INNER JOIN  FrontendMenu fm ON fm.Id = pwm.IdPage
																			  WHERE pwm.Id = {0} ORDER BY  pwm.ORDERVIEW ASC", idPVM);
            return _db.SingleOrDefault<PageViewModule>(sql);
        }
    }
}