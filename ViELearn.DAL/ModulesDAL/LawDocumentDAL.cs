using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class LawDocumentDAL : DALBaseClass<LawDocument>
    {
        public LawDocumentDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public LawDocumentDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<LawDocument> GetLst(int unitId)
        {
            var sql = String.Format(@"SELECT *
                                      FROM LawDocument
                                      WHERE UnitId = {0}
                                      ORDER BY Id DESC", unitId);

            return _db.Fetch<LawDocument>(sql).ToList();
        }

        public List<LawDocument> GetLstPagination(int unitId, int offset, int limit)
        {
            var sql = String.Format(@"SELECT *
                                      FROM LawDocument
                                      WHERE UnitId = {0}
                                      ORDER BY Id DESC
                                      OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY", unitId, offset, limit);
            return _db.Fetch<LawDocument>(sql).ToList();
        }
    }
}
