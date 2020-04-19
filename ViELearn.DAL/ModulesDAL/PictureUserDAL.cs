using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class PictureUserDAL : DALBaseClass<PictureUser>
    {
        public PictureUserDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public PictureUserDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<PictureUser> GetLstPaginationByUnitAndUser(int idUnit, string idUser,int idAlbum, int offset, int limit)
        {
            var sql = String.Format(@"Select * FROM PictureUser WHERE IdUnit = {0} And CreatedBy = '{1}' AND IdAlbum = {2} ORDER BY CreatedAt DESC
                                      OFFSET {3} ROWS FETCH NEXT {4} ROWS ONLY", idUnit, idUser, idAlbum, offset, limit);
            return _db.Fetch<PictureUser>(sql).ToList();
        }

        public List<PictureUser> GetLstByUnitAndUser(int idUnit, string idUser, int idAlbum)
        {
            var sql = String.Format(@"Select * FROM PictureUser WHERE IdUnit = {0} And CreatedBy = '{1}' AND IdAlbum = {2} ORDER BY CreatedAt DESC", idUnit, idUser, idAlbum);
            return _db.Fetch<PictureUser>(sql).ToList();
        }
    }
}
