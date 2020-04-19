using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViELearn.DAL.BaseDAL
{
    public class DataBaseProcessClass
    {
        public string ConnectionStringName;
        public Database _db;
        public DataBaseProcessClass()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public DataBaseProcessClass(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public string GetDisplayNameByUserName(string userName,int type)
        {
            try
            {
                string sql = "";
                switch (type)
                {
                    case 1:
                        sql = string.Format(@"SELECT TOP 1 hs.Ten_hs FROM HocSinh hs INNER JOIN AspNetUsers anu ON anu.Type_info = hs.ID_hs AND hs.Ma_hs = '{0}'", userName);
                        break;
                    case 2:
                        sql = string.Format(@"SELECT TOP 1 gv.Ten_gv FROM GiaoVien gv INNER JOIN AspNetUsers anu ON anu.Type_info = gv.ID_gv AND gv.Ma_gv = '{0}'", userName);
                        break;
                    case 3:
                        return "Quản trị viên";
                    case 4:
                        return "Administrator";
                    default:
                        return "Administrator";
                }
                return _db.SingleOrDefault<string>(sql).ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}