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
    public class CongCuDAL
    {
        public DataTable get_tonghop_dksolienlacdientu(int id_truong)
        {
            Hashtable args = new Hashtable();
            args.Add("id_truong", id_truong);
            DataTable dt = DBLibs.ExecuteStoreProcedure_Select("sp_tonghop_dk_slldt_truong", args, ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            return dt;
        }

        public DataTable get_tonghop_statushocsinh(int id_truong)
        {
            Hashtable args = new Hashtable();
            args.Add("id_truong", id_truong);
            DataTable dt = DBLibs.ExecuteStoreProcedure_Select("sp_tonghop_statushocsinh", args, ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            return dt;
        }
        public DataTable get_danhsach_giaovien(int id_truong)
        {
            Hashtable args = new Hashtable();
            args.Add("id_truong", id_truong);
            DataTable dt = DBLibs.ExecuteStoreProcedure_Select("sp_get_danhsach_giaovien_chatting", args, ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            return dt;
        }
        public DataTable get_danhsach_giaovien_avaiable(int id_truong)
        {
            Hashtable args = new Hashtable();
            args.Add("id_truong", id_truong);
            DataTable dt = DBLibs.ExecuteStoreProcedure_Select("sp_get_danhsach_giaovien_chatting_avaiable", args, ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            return dt;
        }
    }
}