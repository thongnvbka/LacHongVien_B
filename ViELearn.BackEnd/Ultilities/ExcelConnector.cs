using System;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Collections;
using System.Configuration;
using System.Data.OleDb;

public class ExcelConnector
{
    #region Con
    public string Role;
    public int SearhBy = 0, doituong;
    //sql connent

    public OleDbConnection cnn;
    //connecton String

    string Strcnn;
    //get connection string
    public string strcnn
    {
        set
        {
            Strcnn = value;
        }
        get
        {
            return Strcnn;
        }
    }
    //Dung Trong dong bo
    public object khoa = new object();
    //Get connection
    public OleDbConnection getCNN()
    {
        try
        {
            if (cnn == null)
            {
                cnn = new OleDbConnection(Strcnn);
                cnn.Open();
            }
            else
                if (cnn.State == ConnectionState.Closed)
                {
                    cnn = new OleDbConnection(Strcnn);
                    cnn.Open();
                }
            return cnn;
        }
        catch (OleDbException ex)
        {
            throw ex;
        }
    }
    //dong connect
    public void close()
    {
        if (cnn != null)
        {
            if (cnn.State == ConnectionState.Open)
                cnn.Close();
            cnn.Dispose();
        }
    }
    #endregion

    #region getDataSet
    public DataSet getDS(OleDbCommand cmd)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                if (cmd.Connection == null)
                    cmd.Connection = getCNN();
                using (DataSet db = new DataSet())
                {
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        da.Fill(db);
                        return db;
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }
    public DataSet getDS(OleDbCommand cmd, params OleDbParameter[] p)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                if (cmd.Connection == null)
                    cmd.Connection = getCNN();
                using (DataSet db = new DataSet())
                {
                    if (cmd.CommandType == null)
                        cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(p);
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        da.Fill(db);
                        return db;
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }

    public DataSet getDS(string strcmd)
    {
        try
        {
            return getDS(strcmd, CommandType.Text);
        }
        catch (OleDbException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataSet getDS(string strcmd, CommandType ct)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                using (OleDbCommand cmd = new OleDbCommand(strcmd))
                {
                    cmd.Connection = getCNN();
                    cmd.CommandType = ct;
                    using (DataSet db = new DataSet())
                    {
                        using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                        {
                            da.Fill(db);
                            return db;
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }

    public DataSet getDS(string strcmd, CommandType ct, params OleDbParameter[] p)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                using (OleDbCommand cmd = new OleDbCommand(strcmd))
                {
                    cmd.Connection = getCNN();
                    cmd.CommandType = ct;
                    using (DataSet db = new DataSet())
                    {
                        cmd.Parameters.AddRange(p);
                        using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                        {
                            da.Fill(db);
                            return db;
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }
    #endregion

    #region getDataTable
    public DataTable getDB(OleDbCommand cmd)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                if (cmd.Connection == null)
                    cmd.Connection = getCNN();
                using (DataTable db = new DataTable())
                {
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        da.Fill(db);
                        return db;
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }

    public DataTable getDB(OleDbCommand cmd, params OleDbParameter[] p)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                if (cmd.Connection == null)
                    cmd.Connection = getCNN();
                using (DataTable db = new DataTable())
                {
                    if (cmd.CommandType == null)
                        cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(p);
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        da.Fill(db);
                        return db;
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }

    public DataTable getDB(string strcmd)
    {
        try
        {
            return getDB(strcmd, CommandType.Text);
        }
        catch (OleDbException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable getDB(string strcmd, CommandType ct)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                using (OleDbCommand cmd = new OleDbCommand(strcmd))
                {
                    cmd.Connection = getCNN();
                    cmd.CommandType = ct;
                    using (DataTable db = new DataTable())
                    {
                        using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                        {
                            da.Fill(db);
                            return db;
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }

    public DataTable getDB(string strcmd, CommandType ct, params OleDbParameter[] p)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                using (OleDbCommand cmd = new OleDbCommand(strcmd))
                {
                    cmd.Connection = getCNN();
                    cmd.CommandType = ct;
                    using (DataTable db = new DataTable())
                    {
                        cmd.Parameters.AddRange(p);
                        using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                        {
                            da.Fill(db);
                            return db;
                        }
                    }
                }
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }
    #endregion

    #region getOleDbDataReader
    public OleDbDataReader getSDR(OleDbCommand cmd)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                if (cmd.Connection == null)
                    cmd.Connection = getCNN();
                return cmd.ExecuteReader();
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }

    public OleDbDataReader getSDR(OleDbCommand cmd, params OleDbParameter[] p)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                if (cmd.Connection == null)
                    cmd.Connection = getCNN();
                if (cmd.CommandType == null)
                    cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(p);
                return cmd.ExecuteReader();
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }
    public OleDbDataReader getSDR(string strcmd)
    {
        try
        {
            return getSDR(strcmd, CommandType.Text);
        }
        catch (OleDbException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public OleDbDataReader getSDR(string strcmd, CommandType ct)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                using (OleDbCommand cmd = new OleDbCommand(strcmd, getCNN()))
                {
                    cmd.CommandType = ct;
                    return cmd.ExecuteReader();
                }
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }

    public OleDbDataReader getSDR(string strcmd, CommandType ct, params OleDbParameter[] p)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                using (OleDbCommand cmd = new OleDbCommand(strcmd, getCNN()))
                {
                    cmd.CommandType = ct;
                    cmd.Parameters.AddRange(p);
                    return cmd.ExecuteReader();
                }
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }
    #endregion

    #region ExecuteNonQuery
    public int ExecuteNonQuery(OleDbCommand cmd)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                if (cmd.Connection == null)
                    cmd.Connection = getCNN();
                return cmd.ExecuteNonQuery();
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }

    public int ExecuteNonQuery(OleDbCommand cmd, params OleDbParameter[] p)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                if (cmd.Connection == null)
                    cmd.Connection = getCNN();
                if (cmd.CommandType == null)
                    cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(p);
                return cmd.ExecuteNonQuery();
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }

    public int ExecuteNonQuery(string strcmd)
    {
        try
        {
            return ExecuteNonQuery(strcmd, CommandType.Text);
        }
        catch (OleDbException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public int ExecuteNonQuery(string strcmd, CommandType ct)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                using (OleDbCommand cmd = new OleDbCommand(strcmd, getCNN()))
                {
                    cmd.CommandType = ct;
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }

    public int ExecuteNonQuery(string strcmd, CommandType ct, params OleDbParameter[] p)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                using (OleDbCommand cmd = new OleDbCommand(strcmd, getCNN()))
                {
                    cmd.CommandType = ct;
                    cmd.Parameters.AddRange(p);
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }
    #endregion

    #region ExcuteScalar
    public object ExecuteScalar(OleDbCommand cmd)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                if (cmd.Connection == null)
                    cmd.Connection = getCNN();
                return cmd.ExecuteScalar();
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }

    public object ExecuteScalar(OleDbCommand cmd, params OleDbParameter[] p)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                if (cmd.Connection == null)
                    cmd.Connection = getCNN();
                if (cmd.CommandType == null)
                    cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(p);
                return cmd.ExecuteScalar();
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }

    public object ExecuteScalar(string strcmd)
    {
        try
        {
            return ExecuteScalar(strcmd, CommandType.Text);
        }
        catch (OleDbException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public object ExecuteScalar(string strcmd, CommandType ct)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                using (OleDbCommand cmd = new OleDbCommand(strcmd, getCNN()))
                {
                    cmd.CommandType = ct;
                    return cmd.ExecuteScalar();
                }
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }

    public object ExecuteScalar(string strcmd, CommandType ct, params OleDbParameter[] p)
    {
        lock (khoa)
        {
            try
            {
                Monitor.Enter(khoa);
                using (OleDbCommand cmd = new OleDbCommand(strcmd, getCNN()))
                {
                    cmd.CommandType = ct;
                    cmd.Parameters.AddRange(p);
                    return cmd.ExecuteScalar();
                }
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Monitor.Exit(khoa);
            }
        }
    }
    #endregion

    #region MaHoa
    public string Encrypt(string text, string key)
    {
        byte[] encodedkey;
        byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        byte[] bytes;

        encodedkey = Encoding.UTF8.GetBytes(key);
        DESCryptoServiceProvider csp = new DESCryptoServiceProvider();

        bytes = Encoding.UTF8.GetBytes(text);
        MemoryStream ms = new MemoryStream();

        try
        {
            CryptoStream cs = new CryptoStream(ms, csp.CreateEncryptor(encodedkey, iv), CryptoStreamMode.Write);

            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
        }
        catch (Exception ex)
        {
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string text, string key)
    {
        byte[] encodedkey;
        byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        byte[] bytes;

        encodedkey = Encoding.UTF8.GetBytes(key);
        DESCryptoServiceProvider csp = new DESCryptoServiceProvider();

        bytes = Convert.FromBase64String(text);

        MemoryStream ms = new MemoryStream();

        try
        {
            CryptoStream cs = new CryptoStream(ms, csp.CreateDecryptor(encodedkey, iv), CryptoStreamMode.Write);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
        }
        catch (Exception ex)
        {
        }

        return Encoding.UTF8.GetString(ms.ToArray());
    }
    #endregion
}
