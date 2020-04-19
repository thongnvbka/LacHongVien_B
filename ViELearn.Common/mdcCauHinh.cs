using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;


namespace ViELearn.Common
{
    public static class mdcCauHinh
    {
        public static string CHHT_LayGiaTri(int ID_truong, string Ma_setting)
        {
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@ID_truong", ID_truong);
                param[1] = new SqlParameter("@Ma_setting", Ma_setting.ToUpper());
                DataTable dt = Connect.SelectTableSP("Proc_CauHinhHeThong_LayGiaTri", param);
                return dt.Rows[0][0].MapStr();
            }
            catch 
            {
                return "";
            }
        }
    }
}