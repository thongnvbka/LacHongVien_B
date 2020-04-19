using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class PictureUserAlbumDAL : DALBaseClass<PictureUserAlbum>
    {
        public PictureUserAlbumDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public PictureUserAlbumDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<PictureUserAlbum> GetListUserAlbumByUnitAndUser(int idUnit,string idUser)
        {
            var sql = string.Format("SELECT * FROM PictureUserAlbum WHERE IdUnit={0} AND IdUser='{1}' AND Active = 1 ORDER BY OrderView",idUnit,idUser);
            var result = _db.Fetch<PictureUserAlbum>(sql);
            return result;
        }

        public List<PictureUserAlbum> GetLstByType(int idUnit)
        {
            try
            {
                var sql = string.Format(@"SELECT * FROM dbo.PictureUserAlbum WHERE IdUnit = {0} AND Active = 1 ", idUnit);
                return _db.Fetch<PictureUserAlbum>(sql).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int DeleteUserAlbumByUnitAndUser(int idUnit, string idUser)
        {
            var sql = string.Format("DELETE FROM PictureUserAlbum WHERE IdUnit={0} AND IdUser='{1}' AND Active = 1", idUnit, idUser);
            return _db.Execute(sql);
        }
    }
}
