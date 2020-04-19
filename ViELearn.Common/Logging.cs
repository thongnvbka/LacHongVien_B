using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;

namespace ViELearn.Common
{
    public static class Logging
    {
        /// <summary>
        /// Ghi log lai, co the dung de recovery
        /// </summary>
        /// <param name="type">Loại logs, vd: log_diem, log_access, log_ips, log_nhanxet, log_sms,..</param>
        /// <param name="modifier_guid">GUID của tài khoản thực hiện hành động</param>
        /// <param name="modifier_id">ID dạng số của tài khoản thực hiện hành động</param>
        /// <param name="newer_value">Giá trị mới</param>
        /// <param name="older_value">Giá trị trước đây</param>
        /// <param name="table_name_of_value_modified">Tên của bảng trong CSDL lưu giá trị</param>
        /// <param name="column_name_of_value_modified">Tên của cột trong CSDL lưu giá trị</param>
        /// <param name="id_of_row_modified">ID dạng số của giá trị thay đổi</param>
        /// <param name="time">Thời điểm xảy ra sự thay đổi (0: thoi diem hien tai)</param>
        public static void SetLogs(string type, string modifier_guid, string modifier_id, string newer_value, string older_value = "", string table_name_of_value_modified = "", string column_name_of_value_modified = "", string id_of_row_modified = "", int time = 0, string client_ip = "")
        {
            #region Set client_ip
            if (client_ip == "")
            {
                var context = HttpContext.Current;
                var ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(ipAddress)) client_ip = context.Request.ServerVariables["REMOTE_ADDR"];
                else
                {
                    var addresses = ipAddress.Split(',');
                    client_ip = addresses.Length != 0 ? addresses[0] : context.Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            #endregion
            try
            {
                if (time == 0) time = CLibs.DatetimeToTimestamp(DateTime.Now);

                var args = new Hashtable
                {
                    {"type", type},
                    {"modifier_guid", modifier_guid},
                    {"modifier_id", modifier_id},
                    {"newer_value", newer_value},
                    {"older_value", older_value},
                    {"table_name", table_name_of_value_modified},
                    {"column_name", column_name_of_value_modified},
                    {"id_of_row", id_of_row_modified},
                    {"timestamp", time},
                    {"client_ip", client_ip}
                };
                DBLibs.ExecuteStoreProcedure("Proc_SetLogs", args, ConfigurationManager.ConnectionStrings["LogsConnection"].ConnectionString);
            }
            catch
            {
                // ignored
            }
        }

        public static DataTable RecoveryFromLogs(string type, int start_logtime, int end_logtime, string table_name_of_value_modified, string column_name_of_value_modified, string id_of_row_modified, int top = 10)
        {
            var args = new Hashtable
            {
                {"type", type},
                {"modifier_guid", ""},
                {"modifier_id", 0},
                {"table_name", table_name_of_value_modified},
                {"column_name", column_name_of_value_modified},
                {"id_of_row", id_of_row_modified},
                {"start_time", start_logtime},
                {"end_time", end_logtime},
                {"top", top}
            };
            return DBLibs.ExecuteStoreProcedure_Select("Proc_GetLogs", args, ConfigurationManager.ConnectionStrings["LogsConnection"].ConnectionString);
        }

    }
}