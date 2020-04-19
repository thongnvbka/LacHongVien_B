using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class VuViecDAL : DALBaseClass<VuViec>
    {
        public VuViecDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public VuViecDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<VuViec> GetLst(int MaDonVi)
        {
            var sql = String.Format(@"SELECT n.*,anu.DisplayName as TenNguoiTao
                                      FROM VuViec n                        
                                      INNER JOIN AspNetUsers anu ON n.NguoiTao = anu.Id 
                                      WHERE n.MaDonVi = {0}  
                                      ORDER BY n.NgayBaoCao DESC", MaDonVi);

            return _db.Fetch<VuViec>(sql).ToList();
        }

        public List<VuViec> GetLstPagination(int unitId, int offset, int limit)
        {
            var sql = String.Format(@"SELECT n.*,nc.Name as CategoryName,nc.Type as CategoryType,anu.DisplayName as UserCreatedName 
                                      FROM VuViec n 
                                      INNER JOIN VuViecCategories nc ON nc.Id = n.CategoryID AND nc.UnitID = {0} 
                                      INNER JOIN AspNetUsers anu ON n.CreatedBy = anu.Id 
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC
                                      OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY", unitId, offset, limit);
            return _db.Fetch<VuViec>(sql).ToList();
        }

        public List<VuViec> GetLstByCategoryType(int unitId, int type)
        {
            var sql = String.Format(@"SELECT n.*,nc.Name as CategoryName,nc.Type as CategoryType,anu.DisplayName as UserCreatedName  
                                      FROM VuViec n 
                                      INNER JOIN VuViecCategories nc ON nc.Id = n.CategoryID AND nc.UnitID = {0} AND nc.Type = {1} 
                                      INNER JOIN AspNetUsers anu ON n.CreatedBy = anu.Id 
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC", unitId, type);
            return _db.Fetch<VuViec>(sql).ToList();
        }

        public List<VuViec> GetLstByStatus(int unitId, int status)
        {
            var sql = String.Format(@"SELECT n.*,nc.Name as CategoryName,nc.Type as CategoryType,anu.DisplayName as UserCreatedName  
                                      FROM VuViec n 
                                      INNER JOIN VuViecCategories nc ON nc.Id = n.CategoryID AND nc.UnitID = {0}
                                      INNER JOIN AspNetUsers anu ON n.CreatedBy = anu.Id 
                                      WHERE n.Status = {1} 
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC", unitId, status);
            return _db.Fetch<VuViec>(sql).ToList();
        }

        public List<VuViec> GetLstByUser(int unitId, string userId)
        {
            var sql = String.Format(@"SELECT n.*,nc.Name as CategoryName,nc.Type as CategoryType,anu.DisplayName as UserCreatedName   
                                      FROM VuViec n 
                                      INNER JOIN VuViecCategories nc ON nc.Id = n.CategoryID AND nc.UnitID = {0} AND n.CreatedBy = '{1}' 
                                      INNER JOIN AspNetUsers anu ON n.CreatedBy = anu.Id 
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC", unitId, userId);
            return _db.Fetch<VuViec>(sql).ToList();
        }

        public List<VuViec> GetLstPaginationByUser(int unitId, string userId, int offset, int limit)
        {
            var sql = String.Format(@"SELECT n.*,nc.Name as CategoryName,nc.Type as CategoryType,anu.DisplayName as UserCreatedName   
                                      FROM VuViec n 
                                      INNER JOIN VuViecCategories nc ON nc.Id = n.CategoryID AND nc.UnitID = {0} AND n.CreatedBy = '{2}' 
                                      INNER JOIN AspNetUsers anu ON n.CreatedBy = anu.Id 
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC
                                      OFFSET {2} ROWS FETCH NEXT {3} ROWS ONLY", unitId, userId, offset, limit);
            return _db.Fetch<VuViec>(sql).ToList();
        }

        public List<VuViec> GetLstByUserAndType(int unitId, int type, string userId)
        {
            var sql = String.Format(@"SELECT n.*,nc.Name as CategoryName,nc.Type as CategoryType,anu.DisplayName as UserCreatedName   
                                      FROM VuViec n 
                                      INNER JOIN VuViecCategories nc ON nc.Id = n.CategoryID AND nc.UnitID = {0} AND nc.Type = {1} AND n.CreatedBy = '{2}' 
                                      INNER JOIN AspNetUsers anu ON n.CreatedBy = anu.Id 
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC", unitId, type, userId);
            return _db.Fetch<VuViec>(sql).ToList();
        }
        

        public VuViec GetTotalVuViec(int categoryId)
        {
            var sql = String.Format(@"SELECT Count(Id)
                                      FROM VuViec                                        
                                      WHERE CategoryId = {0}", categoryId);
            return _db.SingleOrDefault<VuViec>(sql);
        }        
    }
}
