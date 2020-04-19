using PetaPoco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace ViELearn.DAL.BaseDAL
{
    public abstract class DALBaseClass<T> where T : class
    {
        public string ConnectionStringName;
        public Database _db;
        #region CraeteItem
        public void CreateItem(T obj)
        {
            _db.Insert(obj);
        }

        public int CreateItemWithCheck(T obj)
        {
            if (_db.IsNew(obj))
            {
                _db.Insert(obj);
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void CreateItem(List<T> lstObj)
        {
            foreach (T item in lstObj)
            {
                _db.Insert(item);
            }
        }
        #endregion
        #region GetItems
        public List<T> GetListItems()
        {
            var result = _db.Query<T>("Select * From " + typeof(T).Name);
            return result.ToList();
        }

        public List<T> GetListItemsHaveWhere(string wClause)
        {
            var result = _db.Fetch<T>("Select * From " + typeof(T).Name + " WHERE " + wClause);
            return result.ToList();
        }

        public List<T> GetListItemsOrderBy(string name, string value)
        {
            var sql = String.Format("Select * From {0} Order by {1} {2}", typeof(T).Name, name, value);
            var result = _db.Query<T>(sql);
            return result.ToList();
        }

        public List<T> GetListItems(List<KeyValuePair<String, String>> lstKeyValue)
        {
            var sql = string.Format(@"SELECT *
                                      FROM {0}
                                      WHERE {1}='{2}'", typeof(T).Name, lstKeyValue.First().Key, lstKeyValue.First().Value);
            int i = 0;
            foreach (KeyValuePair<String, String> item in lstKeyValue)
            {
                if (i > 0) sql += string.Format(@" AND {0} = '{1}'", item.Key, item.Value);
                i++;
            }
            var result = _db.Query<T>(sql);
            return result.ToList();
        }

        public T GetItemByID(string idName, int id)
        {
            T result = _db.SingleOrDefault<T>(String.Format("SELECT * FROM {0} WHERE {1}={2}", typeof(T).Name, idName, id));
            return result;
        }

        public List<T> GetListItemByID(string idName, int id)
        {
            var results = _db.Fetch<T>(String.Format("SELECT * FROM {0} WHERE {1}={2}", typeof(T).Name, idName, id));
            return results;
        }

        public List<T> GetListItemByID(List<int> lstID)
        {
            var results = new List<T>();
            foreach (int id in lstID)
            {
                results.Add(_db.SingleOrDefault<T>(id));
            }
            return results;
        }

        public List<T> GetListItemPagination(string orderColumn, string orderType, int offset, int fetch)
        {
            var sql = string.Format(@"Select * From {0} ORDER BY {1} {2}
                                        OFFSET {3} ROWS
                                        FETCH NEXT {4} ROWS ONLY", typeof(T).Name, orderColumn, orderType, offset, fetch);
            return _db.Fetch<T>(sql).ToList();
        }

        public List<T> GetListItemPaginationHaveWhere(string orderColumn, string orderType, int offset, int fetch, string wClause)
        {
            var sql = string.Format(@"Select * From {0} WHERE {5} ORDER BY {1} {2}
                                        OFFSET {3} ROWS
                                        FETCH NEXT {4} ROWS ONLY", typeof(T).Name, orderColumn, orderType, offset, fetch, wClause);
            return _db.Fetch<T>(sql).ToList();
        }

        public T GetItemByValue(string colName, string value)
        {
            T result = _db.SingleOrDefault<T>(String.Format("SELECT * FROM {0} WHERE {1}={2}", typeof(T).Name, colName, value));
            return result;
        }

        public T GetItemHaveWhere(string wClause)
        {
            var result = _db.SingleOrDefault<T>("Select TOP 1 * From " + typeof(T).Name + " WHERE " + wClause);
            return result;
        }
        #endregion
        #region UpdateItems       

        public void CreateNewOrUpdate(T obj)
        {
            _db.Save(obj);
        }

        public void UpdateItem(T obj)
        {
            _db.Update(obj);
        }

        public void UpdateItem(T obj, string[] columnUpdate)
        {
            _db.Update(obj, columnUpdate);
        }

        public void UpdateListItem(List<T> lstObj)
        {
            foreach (T item in lstObj)
            {
                _db.Update(item);
            }
        }

        public void UpdateItemByCommand(List<KeyValuePair<String, String>> lstKeyValue)
        {

            foreach (KeyValuePair<String, String> keyValue in lstKeyValue)
            {
                _db.Update<T>("Where @0 = @1 ", keyValue.Key, keyValue.Value);
            }
        }

        public int UpdateItemByCommand(string keyName, int keyValue, string colName, string colValue)
        {
            var sql = string.Format(@"UPDATE {0} SET {1}=N'{2}' WHERE {3}={4}", typeof(T).Name, colName, colValue.Replace("'", "''"), keyName, keyValue);
            return _db.Execute(sql);
        }

        public int UpdateItemByCommand(string keyName, int keyValue, string colName, int colValue)
        {
            var sql = string.Format(@"UPDATE {0} SET {1}={2} WHERE {3}={4}", typeof(T).Name, colName, colValue, keyName, keyValue);
            return _db.Execute(sql);
        }

        public int UpdateItemByCommand(string keyName, string keyValue, string colName, string colValue)
        {
            var sql = string.Format(@"UPDATE {0} SET {1}=N'{2}' WHERE {3}='{4}'", typeof(T).Name, colName, colValue, keyName, keyValue);
            return _db.Execute(sql);
        }

        public int UpdateItemByCommand(string keyName, string keyValue, string colName, int colValue)
        {
            var sql = string.Format(@"UPDATE {0} SET {1}={2} WHERE {3}='{4}'", typeof(T).Name, colName, colValue, keyName, keyValue);
            return _db.Execute(sql);
        }
        #endregion
        #region DeleteItems

        public void DeleteItem(T obj)
        {
            _db.Delete(obj);
        }

        public void DeleteListItem(List<T> lstObj)
        {
            foreach (T item in lstObj)
            {
                _db.Delete(item);
            }
        }

        public void DeleteItemByCommand(List<KeyValuePair<String, String>> lstKeyValue)
        {
            foreach (KeyValuePair<String, String> keyValue in lstKeyValue)
            {
                _db.Delete<T>("Where @0 = @1 ", keyValue.Key, keyValue.Value);
            }
        }
        #endregion
        public int DeleteItemByValues(string colName, string value)
        {
            return _db.Delete<T>(String.Format("Where {0} = '{1}' ", colName, value));
        }
        public int DeleteItemByValues(string colName, int value)
        {
            return _db.Delete<T>(String.Format("Where {0} = {1} ", colName, value));
        }
        #region Others

        public void CallSPReader(string spName, Object[] args)
        {
        }

        public void CallSPReaderNoReturn(string spName, Object[] args)
        {
        }

        public int CountAllItems()
        {
            try
            {
                var sql = string.Format("SELECT Total_Rows= SUM(st.row_count) FROM sys.dm_db_partition_stats st WHERE object_name(object_id) = '{0}' AND (index_id < 2)", typeof(T).Name);
                return _db.SingleOrDefault<int>(sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion
    }
}