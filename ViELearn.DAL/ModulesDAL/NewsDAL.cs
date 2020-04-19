using FluentData;
using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class NewsDAL : DALBaseClass<News>
    {
        public IDbContext MainDB()
        {
            return new DbContext().ConnectionStringName("DefaultConnection",
                    new SqlServerProvider());
        }
        public NewsDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public NewsDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<News> GetLst(int unitId)
        {
            var sql = String.Format(@"SELECT n.*,nc.Name as CategoryName,nc.Type as CategoryType,anu.DisplayName as UserCreatedName
                                      FROM News n 
                                      INNER JOIN NewsCategories nc ON nc.Id = n.CategoryID AND nc.UnitID = {0}
                                      INNER JOIN AspNetUsers anu ON n.CreatedBy = anu.Id 
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC", unitId);

            return _db.Fetch<News>(sql).ToList();
        }

        public List<News> GetLstPagination(int unitId, int offset, int limit)
        {
            var sql = String.Format(@"SELECT n.*,nc.Name as CategoryName,nc.Type as CategoryType,anu.DisplayName as UserCreatedName 
                                      FROM News n 
                                      INNER JOIN NewsCategories nc ON nc.Id = n.CategoryID AND nc.UnitID = {0} 
                                      INNER JOIN AspNetUsers anu ON n.CreatedBy = anu.Id 
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC
                                      OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY", unitId, offset, limit);
            return _db.Fetch<News>(sql).ToList();
        }

        public List<News> GetLstByCategoryType(int unitId, int type)
        {
            var sql = String.Format(@"SELECT n.*,nc.Name as CategoryName,nc.Type as CategoryType,anu.DisplayName as UserCreatedName  
                                      FROM News n 
                                      INNER JOIN NewsCategories nc ON nc.Id = n.CategoryID AND nc.UnitID = {0} AND nc.Type = {1} 
                                      INNER JOIN AspNetUsers anu ON n.CreatedBy = anu.Id 
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC", unitId, type);
            return _db.Fetch<News>(sql).ToList();
        }

        public List<News> GetLstByStatus(int unitId, int status)
        {
            var sql = String.Format(@"SELECT n.*,nc.Name as CategoryName,nc.Type as CategoryType,anu.DisplayName as UserCreatedName  
                                      FROM News n 
                                      INNER JOIN NewsCategories nc ON nc.Id = n.CategoryID AND nc.UnitID = {0}
                                      INNER JOIN AspNetUsers anu ON n.CreatedBy = anu.Id 
                                      WHERE n.Status = {1} 
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC", unitId, status);
            return _db.Fetch<News>(sql).ToList();
        }

        public List<News> GetLstByUser(int unitId, string userId)
        {
            var sql = String.Format(@"SELECT n.*,nc.Name as CategoryName,nc.Type as CategoryType,anu.DisplayName as UserCreatedName   
                                      FROM News n 
                                      INNER JOIN NewsCategories nc ON nc.Id = n.CategoryID AND nc.UnitID = {0} AND n.CreatedBy = '{1}' 
                                      INNER JOIN AspNetUsers anu ON n.CreatedBy = anu.Id 
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC", unitId, userId);
            return _db.Fetch<News>(sql).ToList();
        }

        public List<News> GetLstPaginationByUser(int unitId, string userId, int offset, int limit)
        {
            var sql = String.Format(@"SELECT n.*,nc.Name as CategoryName,nc.Type as CategoryType,anu.DisplayName as UserCreatedName   
                                      FROM News n 
                                      INNER JOIN NewsCategories nc ON nc.Id = n.CategoryID AND nc.UnitID = {0} AND n.CreatedBy = '{2}' 
                                      INNER JOIN AspNetUsers anu ON n.CreatedBy = anu.Id 
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC
                                      OFFSET {2} ROWS FETCH NEXT {3} ROWS ONLY", unitId, userId, offset, limit);
            return _db.Fetch<News>(sql).ToList();
        }

        public List<News> GetLstByUserAndType(int unitId, int type, string userId)
        {
            var sql = String.Format(@"SELECT n.*,nc.Name as CategoryName,nc.Type as CategoryType,anu.DisplayName as UserCreatedName   
                                      FROM News n 
                                      INNER JOIN NewsCategories nc ON nc.Id = n.CategoryID AND nc.UnitID = {0} AND nc.Type = {1} AND n.CreatedBy = '{2}' 
                                      INNER JOIN AspNetUsers anu ON n.CreatedBy = anu.Id 
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC", unitId, type, userId);
            return _db.Fetch<News>(sql).ToList();
        }

        public List<News> GetLstInCategoryWithCount(int total, int categoryId)
        {
            var sql = String.Format(@"SELECT TOP {0} * 
                                      FROM News Where 
                                      CategoryID  = {1} And Status = {2}
                                      ORDER BY UpdatedAt DESC,CreatedAt DESC", total, categoryId, 3);
            return _db.Fetch<News>(sql).ToList();
        }

        public List<News> GetLstInCategoryWithCount(int categoryId, int offset, int limit)
        {
            var sql = String.Format(@"SELECT * 
                                      FROM News Where 
                                      CategoryID  = {0} And Status = {1}
                                      ORDER BY UpdatedAt DESC,CreatedAt DESC
                                      OFFSET {2} ROWS FETCH NEXT {3} ROWS ONLY", categoryId, 3,offset,limit);
            return _db.Fetch<News>(sql).ToList();
        }

        public News GetTotalNews(int categoryId)
        {
            var sql = String.Format(@"SELECT Count(Id)
                                      FROM News                                        
                                      WHERE CategoryId = {0}", categoryId);
            return _db.SingleOrDefault<News>(sql);
        }

        public List<News> GetLstByCategoryTypeWithCount(int total, int categoryType, int unitId)
        {
            var sql = String.Format(@"SELECT TOP {0} n.* 
                                      FROM News n 
                                      INNER JOIN NewsCategories nc ON nc.Id = n.CategoryID AND nc.Type = {1} AND nc.UnitID = {2} 
                                      WHERE n.Status = {3} 
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC", total, categoryType, unitId, 3);
            return _db.Fetch<News>(sql).ToList();
        }

        public List<News> GetLstByNewsTypeWithCount(int total, int newsType, int unitId)
        {
            var sql = String.Format(@"SELECT TOP {0} n.* 
                                      FROM News n 
                                      INNER JOIN NewsCategories nc ON nc.Id = n.CategoryID AND nc.UnitID = {1} 
                                      WHERE n.Type = {2} AND n.Status = {3}
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC", total, unitId, newsType, 3);
            return _db.Fetch<News>(sql).ToList();
        }

        public List<News> GetLstByCounter(int total, int unitId)
        {
            var sql = String.Format(@"SELECT TOP {0} * 
                                      FROM News 
                                      WHERE UnitId = {1} AND Status = {2}
                                      ORDER BY ReadCounter DESC,CreatedAt DESC", total, unitId, 3);
            return _db.Fetch<News>(sql).ToList();
        }

        public News GetOneNews(int newsId)
        {
            var sql = String.Format(@"SELECT n.*,nc.Name as CategoryName
                                      FROM News n 
                                      INNER JOIN NewsCategories nc ON nc.Id = n.CategoryID
                                      WHERE n.Id = {0}
                                      ORDER BY n.UpdatedAt DESC,n.CreatedAt DESC", newsId);
            return _db.SingleOrDefault<News>(sql);
        }
        public List<News> GetListNewsByCateStatus(int cateID, int status )
        {

            using (var context = MainDB())
            {
                return context.StoredProcedure("be_GetNewsbyCate_andStatus")
                    .Parameter("CatId", cateID)
                    .Parameter("Status", status)
                    .QueryMany<News>();
            }
        }
        public List<News> GetListNews()
        {

            using (var context = MainDB())
            {
                return context.StoredProcedure("be_GetNews")
                   
                    .QueryMany<News>();
            }
        }
    }
}
