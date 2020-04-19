using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ViELearn.Models.Objects;
using System.Data;
using ViELearn.Common;
using ViELearn.DAL;
using System.Data.SqlClient;
using System.Configuration;

namespace ViELearn.DAL
{
    public class TraceLogsDAL
    {
        private readonly string _cnn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public DataTable get_log_by_user_guid(string userGuid, int startTimestame = 0, int endTimestame = 0, int limit = 1000)
        {
            if (startTimestame == 0)
                startTimestame = CLibs.DatetimeToTimestamp(DateTime.Now.AddMonths(-6));
            if (endTimestame == 0)
                endTimestame = CLibs.DatetimeToTimestamp(DateTime.Now);

            var args = new Hashtable
            {
                { "guid", userGuid },
                { "start_time", startTimestame },
                { "end_time", endTimestame },
                { "top", limit }
            };
            var dt = DBLibs.ExecuteStoreProcedure_Select("sp_get_tracelogs_byguid", args, _cnn);
            return dt;
        }

        public DataTable get_truong_alllogs(int idTruong, int startTimestame = 0, int endTimestame = 0, int limit = 1000)
        {
            if (startTimestame == 0)
                startTimestame = CLibs.DatetimeToTimestamp(DateTime.Now.AddMonths(-6));
            if (endTimestame == 0)
                endTimestame = CLibs.DatetimeToTimestamp(DateTime.Now);

            var args = new Hashtable
            {
                { "start_time", startTimestame },
                { "end_time", endTimestame },
                { "top", limit },
                { "id_truong", idTruong }
            };
            var dt = DBLibs.ExecuteStoreProcedure_Select("sp_get_tracelogs_truong_all", args, _cnn);
            return dt;
        }
        public DataTable get_alllogs(int startTimestame = 0, int endTimestame = 0, int limit = 1000)
        {
            if (startTimestame == 0)
                startTimestame = CLibs.DatetimeToTimestamp(DateTime.Now.AddMonths(-6));
            if (endTimestame == 0)
                endTimestame = CLibs.DatetimeToTimestamp(DateTime.Now);

            var args = new Hashtable
            {
                { "start_time", startTimestame },
                { "end_time", endTimestame },
                { "top", limit }
            };
            var dt = DBLibs.ExecuteStoreProcedure_Select("sp_get_tracelogs_all", args, _cnn);
            return dt;
        }

        public void delete_by_id(int id)
        {
            var args = new Hashtable {{"id", id}};
            DBLibs.ExecuteStoreProcedure("sp_del_tracelogs_byid", args, _cnn);
        }

        //public DataTable get_tonghop_dksolienlacdientu(int id_truong)
        //{
        //    Hashtable args = new Hashtable();
        //    args.Add("id_truong", id_truong);
        //    DataTable dt = DBLibs.ExecuteStoreProcedure_Select("sp_tonghop_dk_slldt_truong", args, ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        //    return dt;
        //}

        //public DataTable get_tonghop_statushocsinh(int id_truong)
        //{
        //    Hashtable args = new Hashtable();
        //    args.Add("id_truong", id_truong);
        //    DataTable dt = DBLibs.ExecuteStoreProcedure_Select("sp_tonghop_statushocsinh", args, ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        //    return dt;
        //}
        //public DataTable get_danhsach_giaovien(int id_truong)
        //{
        //    Hashtable args = new Hashtable();
        //    args.Add("id_truong", id_truong);
        //    DataTable dt = DBLibs.ExecuteStoreProcedure_Select("sp_get_danhsach_giaovien_chatting", args, ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        //    return dt;
        //}
        //public DataTable get_danhsach_giaovien_avaiable(int id_truong)
        //{
        //    Hashtable args = new Hashtable();
        //    args.Add("id_truong", id_truong);
        //    DataTable dt = DBLibs.ExecuteStoreProcedure_Select("sp_get_danhsach_giaovien_chatting_avaiable", args, ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        //    return dt;
        //}
    }
}