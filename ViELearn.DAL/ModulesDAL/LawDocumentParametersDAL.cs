using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class LawDocumentParametersDAL : DALBaseClass<LawDocumentParameters>
    {
        public LawDocumentParametersDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public LawDocumentParametersDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<LawDocumentParameters> GetLstByGroupName(string groupName)
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM LawDocumentParameters
                                        WHERE GroupName='{0}' 
                                        Order By OrderView ASC", groupName);

                return _db.Fetch<LawDocumentParameters>(sql);
            }
            catch (Exception ex)
            {
                return new List<LawDocumentParameters>();
            }
        }

        public List<LawDocumentParameters> GetLst(int unitId)
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM LawDocumentParameters
                                        WHERE UnitId='{0}' 
                                        Order By OrderView ASC", unitId);

                return _db.Fetch<LawDocumentParameters>(sql);
            }
            catch (Exception ex)
            {
                return new List<LawDocumentParameters>();
            }
        }
    }
}
