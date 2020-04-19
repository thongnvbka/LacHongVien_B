using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace ViELearn.Common
{
    public static class Connect
    {
        private static readonly string Cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private static SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        public enum connectionState { open, close };
        public static void setStateConnection(connectionState state)
        {
            if (state == connectionState.open)
            {
                con.Open();
            }
            else
            {
                con.Close();
            }
        }
        public static int Excute(string sql)
        {
            var result = 0;
            try
            {
                return DBLibs.ExecuteNonQuery(sql, Cnn);
                //SqlCommand cmd = new SqlCommand(sql, con);
                //cmd.Connection = con;
                //if (con.State == ConnectionState.Closed) { con.Open(); }
                //cmd.CommandType = CommandType.Text;
                //cmd.CommandText = sql;
                //result = cmd.ExecuteScalar().MapInt();
                //return result;
            }
            catch (Exception)
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }

        public static int ExcuteSP(string storeProcedure)
        {
            try
            {
                return DBLibs.ExecuteStoreProcedure(storeProcedure, new Hashtable(), Cnn);
                //var result = 0;
                //SqlCommand cmd = new SqlCommand(storeProcedure, con);
                //cmd.Connection = con;
                //if (con.State == ConnectionState.Closed) { con.Open(); }
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.CommandText = storeProcedure;
                //result = cmd.ExecuteScalar().MapInt();
                //return result;
            }
            catch { return 0; }
            finally
            {
                con.Close();
            }
        }

        public static int ExcuteSP(string StoreProcName, SqlParameter[] param = null)
        {
            //SqlCommand cmd = null;
            try
            {
                var args = new Hashtable();
                if (param == null) return DBLibs.ExecuteStoreProcedure(StoreProcName, args, Cnn);
                foreach (var p in param)
                {
                    if (p.DbType.ToString() == "DateTime")
                    {
                        var timeValue = DateTime
                            .ParseExact(p.Value.ToString(), "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)
                            .ToString("yyyy-MM-dd HH:mm:ss.fff");
                        args.Add(p.ParameterName.Replace("@", ""), timeValue);
                    }
                    else
                    {
                        args.Add(p.ParameterName.Replace("@", ""), p.Value.ToString());
                    }
                }
                return DBLibs.ExecuteStoreProcedure(StoreProcName, args, Cnn);

                //cmd = new SqlCommand(StoreProcName);
                //cmd.Connection = con;
                //if (con.State == ConnectionState.Closed) { con.Open(); }
                //cmd.CommandType = CommandType.StoredProcedure;
                //if (param != null)
                //{
                //    for (int i = 0; i < param.Length; i++)
                //    {
                //        cmd.Parameters.Add(param[i]);
                //    }
                //}
                //return cmd.ExecuteScalar().MapInt();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                con.Close();
            }
        }
        public static object ExecuteScalar(string sql)
        {
            //object result;
            try
            {
                return DBLibs.ExecuteScalar(sql, Cnn);
                //SqlCommand cmd = new SqlCommand(sql, con);
                //cmd.Connection = con;
                //if (con.State == ConnectionState.Closed) { con.Open(); }
                //cmd.CommandType = CommandType.Text;
                //cmd.CommandText = sql;
                //result = cmd.ExecuteScalar();
                //return result;
            }
            catch { return null; }

            finally
            {
                con.Close();
            }
        }

        public static object ExecuteScalarSP(string storeProcedure)
        {
            //object result;
            try
            {
                return DBLibs.ExcuteStoreProcedure_Scalar(storeProcedure, new Hashtable(), Cnn);

                //SqlCommand cmd = new SqlCommand(storeProcedure, con);
                //cmd.Connection = con;
                //if (con.State == ConnectionState.Closed) { con.Open(); }
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.CommandText = storeProcedure;
                //result = cmd.ExecuteScalar();
                //return result;
            }
            catch { return null; }

            finally
            {
                con.Close();
            }
        }

        public static object ExecuteScalarSP(string StoreProcName, SqlParameter[] param = null)
        {
            try
            {
                var args = new Hashtable();
                if (param == null) return DBLibs.ExcuteStoreProcedure_Scalar(StoreProcName, args, Cnn);
                foreach (var p in param)
                {
                    if (p.DbType.ToString() == "DateTime")
                    {
                        var timeValue = DateTime
                            .ParseExact(p.Value.ToString(), "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)
                            .ToString("yyyy-MM-dd HH:mm:ss.fff");
                        args.Add(p.ParameterName.Replace("@", ""), timeValue);
                    }
                    else
                    {
                        args.Add(p.ParameterName.Replace("@", ""), p.Value.ToString());
                    }
                }
                return DBLibs.ExcuteStoreProcedure_Scalar(StoreProcName, args, Cnn);
                //SqlCommand cmd = new SqlCommand(StoreProcName);
                //cmd.Connection = con;
                //if (con.State == ConnectionState.Closed) { con.Open(); }
                //cmd.CommandType = CommandType.StoredProcedure;
                //if (param != null)
                //{
                //    for (int i = 0; i < param.Length; i++)
                //    {
                //        cmd.Parameters.Add(param[i]);
                //    }
                //}
                //return cmd.ExecuteScalar();
            }
            catch (Exception)
            {
                return null;
            }

            finally
            {
                con.Close();
            }
        }

        public static DataTable SelectTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                return DBLibs.GetDataBy_DataAdapter(sql, Cnn);

                //SqlCommand cmd = new SqlCommand(sql, con);
                //cmd.Connection = con;
                //SqlDataAdapter da = new SqlDataAdapter(cmd);
                //if (con.State == ConnectionState.Closed) { con.Open(); }
                //DataSet ds = new DataSet();
                //da.Fill(ds);
                //dt = ds.Tables[0];
                //return dt;
            }
            catch (Exception)
            {
                return dt;
            }

            finally
            {
                con.Close();
            }
        }

        public static DataTable SelectTableSP(string storeProcedure)
        {
            DataTable dt = new DataTable();
            try
            {
                return DBLibs.ExecuteStoreProcedure_Select(storeProcedure, new Hashtable(), Cnn);
                //SqlCommand cmd = new SqlCommand(storeProcedure, con);
                //cmd.Connection = con;
                //if (con.State == ConnectionState.Closed) { con.Open(); }
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.CommandText = storeProcedure;
                //SqlDataAdapter da = new SqlDataAdapter(cmd);
                //DataSet ds = new DataSet();
                //da.Fill(ds);
                //dt = ds.Tables[0];
                //return dt;
            }
            catch (Exception)
            {
                return dt;
            }

            finally
            {
                con.Close();
            }
        }
        public static DataTable SelectTableSP(string StoreProcName, SqlParameter[] param = null)
        {
            DataTable dt = new DataTable();
            try
            {
                var args = new Hashtable();
                if (param == null) return DBLibs.ExecuteStoreProcedure_Select(StoreProcName, args, Cnn);
                foreach (var p in param)
                {
                    if (p.DbType.ToString() == "DateTime")
                    {
                        var timeValue = DateTime
                            .ParseExact(p.Value.ToString(), "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)
                            .ToString("yyyy-MM-dd HH:mm:ss.fff");
                        args.Add(p.ParameterName.Replace("@", ""), timeValue);
                    }
                    else
                    {
                        args.Add(p.ParameterName.Replace("@", ""), p.Value.ToString());
                    }
                }
                return DBLibs.ExecuteStoreProcedure_Select(StoreProcName, args, Cnn);
                //            SqlCommand cmd = new SqlCommand(StoreProcName);
                //            cmd.Connection = con;
                //if (con.State == ConnectionState.Closed) { con.Open(); }
                //cmd.CommandType = CommandType.StoredProcedure;
                //            if (param != null)
                //            {
                //                for (int i = 0; i < param.Length; i++)
                //                {
                //                    cmd.Parameters.Add(param[i]);
                //                }
                //            }
                //            SqlDataAdapter da = new SqlDataAdapter(cmd);
                //            DataSet ds = new DataSet();
                //            da.Fill(ds);
                //            dt = ds.Tables[0];
                //            return dt;
            }
            catch (Exception)
            {
                return dt;
            }

            finally
            {
                con.Close();
            }
        }
        public static int GetIDENTITY(String tableName)
        {
            int idx = 0;
            string sql = "Select * FROM " + tableName;
            DataTable dt = SelectTable(sql);
            if (dt.Rows.Count > 0)
            {
                String sql2 = "Select COLUMN_NAME " +
                "FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE " +
                "WHERE TABLE_NAME = '" + tableName + "'";
                String colID = "";
                DataTable dt2 = SelectTable(sql2);
                if (dt2.Rows.Count > 0)
                {
                    colID = dt2.Rows[0][0].ToString();
                }
                if (colID != "")
                {
                    idx = dt.Rows[dt.Rows.Count - 1][colID].ToInt32();
                }
                else
                {
                    idx = dt.Rows[dt.Rows.Count - 1][0].ToInt32();
                }
            }
            return idx;
        }

        //public static int LogUser(int UserID, )
        //{

        //}
    }
}