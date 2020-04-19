using FluentData;
using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class DanhMucChungDAL : DALBaseClass<DanhMucChung>
    {
        public IDbContext MainDB()
        {
            return new DbContext().ConnectionStringName("DefaultConnection",
                    new SqlServerProvider());
        }
        public DanhMucChungDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }
        public DanhMucChungDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<DanhMucChung> GetLstByOrderView()
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM DanhMucChung
                                        Order By MaDanhMuc,SoThuTu DESC");

                return _db.Fetch<DanhMucChung>(sql);
            }
            catch (Exception ex)
            {
                return new List<DanhMucChung>();
            }
        }

        public List<DanhMucChung> GetLstByCode(string maDanhMuc)
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM DanhMucChung
                                        WHERE MaDanhMuc='{0}'
                                        Order By SoThuTu ASC", maDanhMuc);

                return _db.Fetch<DanhMucChung>(sql);
            }
            catch (Exception ex)
            {
                return new List<DanhMucChung>();
            }
        }

        public List<DanhMucChung> GetLstByParrentId(int parrentId)
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM DanhMucChung
                                        WHERE idDanhMucCha='{0}'
                                        Order By SoThuTu ASC", parrentId);

                return _db.Fetch<DanhMucChung>(sql);
            }
            catch (Exception ex)
            {
                return new List<DanhMucChung>();
            }
        }

        public List<DanhMucChung> GetLstByParrentId(int parrentId,int type)
        {
            try
            {
                var sql = "";
                if (type == 0)
                {
                    sql = string.Format(@"SELECT *
                                        FROM DanhMucChung
                                        WHERE idDanhMucCha='{0}' AND SoThutu < 100
                                        Order By SoThuTu ASC", parrentId);
                }                    
                else
                {
                    sql = string.Format(@"SELECT *
                                        FROM DanhMucChung
                                        WHERE idDanhMucCha='{0}' AND SoThutu >= 100
                                        Order By SoThuTu ASC", parrentId);
                }
                return _db.Fetch<DanhMucChung>(sql);
            }
            catch (Exception ex)
            {
                return new List<DanhMucChung>();
            }
        }


        public List<DanhMucChung> GetLstMaDanhMuc()
        {
            try
            {
                var sql = string.Format(@"SELECT DISTINCT MaDanhMuc
                                        FROM DanhMucChung
                                        ORDER BY MaDanhMuc ASC");

                return _db.Fetch<DanhMucChung>(sql);
            }
            catch (Exception ex)
            {
                return new List<DanhMucChung>();
            }
        }
        public int GetTotalDanhMucChung()
        {
            try
            {
                var sql = string.Format("SELECT Total_Rows= SUM(st.row_count) FROM sys.dm_db_partition_stats st WHERE object_name(object_id) = '{0}' AND (index_id < 2)", "DanhMucChung");
                return _db.SingleOrDefault<int>(sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<DanhMucChung> GetDanhMucChung(int loai)
        {
            using (var context = MainDB())
            {
                return context.StoredProcedure("GetDanhMuc")
                    .Parameter("loai", loai)
                    .QueryMany<DanhMucChung>();
            }
        }
        public List<DanhMucChung> GetlistDanhMucChung()
        {
            using (var context = MainDB())
            {
                return context.Sql(@"SELECT c.id,c.TenDanhMuc,c.MaDanhMuc,c.LoaiDanhMuc,c.SoThuTu,c.TrangThai,c.idDanhMucCha,dbo.GetCountNewsByCatId(c.id)'TongSoTin',
            c.Type,c.Path,c.Thumbnail,c.ShowHome,c.Url, ISNULL(p.TenDanhMuc, '') tenCha FROM DanhMucChung c LEFT JOIN DanhMucChung p ON c.idDanhMucCha = p.id  WHERE c.LoaiDanhMuc = 1 ORDER BY p.SoThuTu, c.SoThuTu")
                    .QueryMany<DanhMucChung>();
            }
        }
        public List<DanhMucChung> GetChildDanhMucChung(int loai,int parentId)
        {
            using (var context = MainDB())
            {
                return context.StoredProcedure("GetChildDanhMucbyId")
                    .Parameter("loai", loai)
                    .Parameter("parentId", parentId)
                    .QueryMany<DanhMucChung>();
            }
        }


    }
}
