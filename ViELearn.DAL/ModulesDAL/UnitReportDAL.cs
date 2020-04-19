using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class UnitReportDAL : DALBaseClass<UnitReport>
    {
        public UnitReportDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public UnitReportDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<UnitReport> GetListReportByWeek(int unitId,DateTime dayStart,DateTime dayEnd)
        {
            try
            {
                var sql = string.Format(@"SELECT  *
                                        FROM    dbo.UnitReport
                                        WHERE   UnitId = {0}
                                                AND DayReport >= '{1}'
                                                AND DayReport <= '{2}'", unitId,dayStart,dayEnd);
                return _db.Fetch<UnitReport>(sql);
            }
            catch (Exception ex)
            {
                return new List<UnitReport>();
            }
        }
    }
}
