using System;
using System.Collections.Generic;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class UnitWorkCalendarDAL : DALBaseClass<UnitWorkCalendar>
    {
        public UnitWorkCalendarDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public UnitWorkCalendarDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<UnitWorkCalendar> GetListWorkCalendarByDay(int unitId, DateTime day)
        {
            try
            {
                var sql = string.Format(@"SELECT  uwc.* ,
                                                anu1.UserName AS CreatedByName ,
                                                anu2.UserName AS PublishedByName
                                          FROM dbo.UnitWorkCalendar uwc
                                                    INNER JOIN dbo.AspNetUsers anu1 ON anu1.Id = uwc.CreatedBy
		                                            LEFT JOIN dbo.AspNetUsers anu2 ON anu2.Id = uwc.PublishedBy
                                          WHERE uwc.UnitId = {0} AND uwc.DayWorkCalendar = '{1}'", unitId, day.Date);
                return _db.Fetch<UnitWorkCalendar>(sql);
            }
            catch (Exception ex)
            {
                return new List<UnitWorkCalendar>();
            }
        }

        public List<UnitWorkCalendar> GetListWorkCalendarByWeek(int unitId, DateTime dayStart, DateTime dayEnd)
        {
            try
            {
                var sql = string.Format(@"SELECT  uwc.* ,
                                                anu1.UserName AS CreatedByName ,
                                                anu2.UserName AS PublishedByName
                                          FROM dbo.UnitWorkCalendar uwc
                                                    INNER JOIN dbo.AspNetUsers anu1 ON anu1.Id = uwc.CreatedBy
		                                            LEFT JOIN dbo.AspNetUsers anu2 ON anu2.Id = uwc.PublishedBy
                                        WHERE   uwc.UnitId = {0}
                                                AND uwc.DayWorkCalendar >= '{1}'
                                                AND uwc.DayWorkCalendar <= '{2}'", unitId, dayStart, dayEnd);
                return _db.Fetch<UnitWorkCalendar>(sql);
            }
            catch (Exception ex)
            {
                return new List<UnitWorkCalendar>();
            }
        }
    }
}
