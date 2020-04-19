using PetaPoco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace ViELearn.DAL
{
    public abstract class DALBaseClass<T> where T : class
    {
        public string ConnectionStringName;
        public Database _db; 
        #region CraeteItem
        public bool CreateItem(T obj)
        {
            try
            {
                _db.Insert(obj);
                return true;
            }
            catch
            {
                return false;
            }
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

        public List<T> GetListItemsOrderBy(string name,string value)
        {
            var sql = String.Format("Select * From {0} Order by {1} {2}", typeof(T).Name, name, value); 
            var result = _db.Query<T>(sql);
            return result.ToList();
        }

        public List<T> GetListItems(List<KeyValuePair<String, String>> lstKeyValue)
        {
            var sql = Sql.Builder.Append("SELECT * FROM " + typeof(T).Name)
                                 .Append("WHERE @0=@1", lstKeyValue.First().Key, lstKeyValue.First().Value);
            int i = 0;
            foreach (KeyValuePair<String, String> item in lstKeyValue)
            {
                if (i>0) sql = sql.Append("AND @0=@1", lstKeyValue.First().Key, lstKeyValue.First().Value);
                i++;                
            }
            var result = _db.Query<T>(sql);
            return result.ToList();
        }

        public T GetItemByID(string idName,int ID)
        {
            T result = _db.SingleOrDefault<T>(String.Format("SELECT * FROM {0} WHERE {1}={2}", typeof(T).Name,idName, ID)); 
            return result;
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

        public void UpdateListItem(List<T> lstObj)
        {
            foreach (T item in lstObj)
            {
                _db.Update(item);
            }
        }

        public void UpdateItemByCommand(List<KeyValuePair<String,String>> lstKeyValue)
        {

            foreach (KeyValuePair<String,String> keyValue in lstKeyValue)
            {
                _db.Update<T>("Where @0 = @1 ", keyValue.Key, keyValue.Value);
            }
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

        public void DeleteItemByValues(string colName, string value)
        {            
            _db.Delete<T>(String.Format("Where {0} = '{1}' ", colName, value));
        }

        public void DeleteItemByValues(string colName, int value)
        {
            _db.Delete<T>(String.Format("Where {0} = {1} ", colName, value));
        }
        #region Others

        public void CallSPReader(string spName,Object[] args)
        {
        }

        public void CallSPReaderNoReturn(string spName, Object[] args)
        {
        }

        //VinhNX: Convert List to datatable
        //public static DataTable ToDataTable<T>(List<T> items)
        //{
        //    DataTable dataTable = new DataTable(typeof(T).Name);
        //    PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    foreach (PropertyInfo prop in Props)
        //    {
        //        dataTable.Columns.Add(prop.Name);
        //    }
        //    foreach (T item in items)
        //    {
        //        var values = new object[Props.Length];
        //        for (int i = 0; i < Props.Length; i++)
        //        {
        //            values[i] = Props[i].GetValue(item, null);
        //        }
        //        dataTable.Rows.Add(values);
        //    }
        //    return dataTable;
        //}
        #endregion

    }
}