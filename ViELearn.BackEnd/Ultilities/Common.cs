using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ViELearn.BackEnd.Ultilities
{
    public class Common
    {
        public static string GetUrlFrontend()
        {
            return ConfigurationManager.AppSettings["domain_fontend"];
        }
        public static string GetByKey(string key)
        {
            return ConfigurationManager.AppSettings[key].ToString();
        }
        public const string uniChars =
         "àáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệđìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆĐÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴÂĂĐÔƠƯ";

        public const string KoDauChars =
                "aaaaaaaaaaaaaaaaaeeeeeeeeeeediiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAAEEEEEEEEEEEDIIIOOOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYYAADOOU";

        public static string UnicodeToKoDau(string s)
        {
            string retVal = String.Empty;
            int pos;
            for (int i = 0; i < s.Length; i++)
            {
                pos = uniChars.IndexOf(s[i].ToString());
                if (pos >= 0)
                    retVal += KoDauChars[pos];
                else
                    retVal += s[i];
            }
            return retVal;
        }
        public static string UnicodeToKoDauAndGach(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return s;
            string strChar = "abcdefghijklmnopqrstxyzuvxw0123456789 ";
            s = UnicodeToKoDau(s.ToLower().Trim());
            string sReturn = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (strChar.IndexOf(s[i]) > -1)
                {
                    if (s[i] != ' ')
                        sReturn += s[i];
                    else if (i > 0 && s[i - 1] != ' ' && s[i - 1] != '_')
                        sReturn += "_";
                }
            }
            return sReturn.Replace("--", "_");

        }
        public static string TimeAgo(DateTime dt)
        {
            if (dt > DateTime.Now) return "vừa xong";
            TimeSpan span = DateTime.Now - dt;

            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("about {0} năm trước", years);
            }

            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format("{0} tháng trước", months);
            }

            if (span.Days > 0) return String.Format("{0} ngày trước", span.Days);
            if (span.Hours > 0) return String.Format("{0} giờ trước", span.Hours);
            if (span.Minutes > 0) return String.Format("{0} phút trước", span.Minutes);
            if (span.Seconds > 5) return String.Format("{0} giây trước", span.Seconds);
            if (span.Seconds <= 5) return "vừa xong";

            return string.Empty;
        }
        public static string TimeRange(DateTime dt)
        {
            if (dt < DateTime.Now) return "Đã publish";
            TimeSpan span = dt - DateTime.Now  ;

            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("Còn {0} năm ", years);
            }

            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format(" {0} tháng nữa", months);
            }

            if (span.Days > 0) return String.Format("{0} ngày nữa", span.Days);
            if (span.Hours > 0) return String.Format("{0} giờ nữa", span.Hours);
            if (span.Minutes > 0) return String.Format("{0} phút nữa", span.Minutes);
            if (span.Seconds > 5) return String.Format("{0} giây nữa", span.Seconds);
            if (span.Seconds <= 5) return "Đã publish";

            return string.Empty;
        }
        public static string Obj2String(object obj)
        {
            if (obj == null)
                return string.Empty;
            try
            {
                return obj.ToString();
            }
            catch { return string.Empty; }
        }
        public static long Obj2Long(object obj)
        {
            if (obj == null)
                return 0;
            try
            {
                return Convert.ToInt64(obj.ToString().Replace(",", string.Empty), CultureInfo.InvariantCulture);
            }
            catch { return 0; }
        }
        public static int Obj2Int(object obj)
        {
            if (obj == null)
                return 0;
            try
            {
                return Convert.ToInt32(obj.ToString().Replace(",", string.Empty), CultureInfo.InvariantCulture);
            }
            catch { return 0; }
        }
        public static double Obj2Double(object obj)
        {
            if (obj == null)
                return 0;
            try
            {
                return Convert.ToDouble(obj.ToString().Replace(",", string.Empty), CultureInfo.InvariantCulture);
            }
            catch { return 0; }
        }
        public static string Obj2Money(object obj)
        {
            if (obj == null)
                return "";
            try
            {
                var info = System.Globalization.CultureInfo.GetCultureInfo("vi-VN");  // try with "en-US"
                return String.Format(info, "{0:c}", obj);
            }
            catch { return ""; }
        }
        public static string Long2DateTime(object obj)
        {
            if (obj == null)
                return "";
            try
            {
                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime date = start.AddMilliseconds(Obj2Double(obj)).ToLocalTime();
                return date.ToString("dd/MM/yyyy");
            }
            catch { return ""; }
        }
        public static string Long2DateTimeToString(DateTime obj)
        {
            if (obj == null)
                return "";
            try
            {
                if (obj != DateTime.MinValue)
                    return obj.ToString("HH:mm dd/MM/yyyy");
                else
                    return "";

            }
            catch { return ""; }
        }
        public static DateTime ObjectToDataTime(object value)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                dt = Convert.ToDateTime(value, CultureInfo.CreateSpecificCulture("fr-FR"));
            }
            catch { dt = DateTime.MinValue; }
            return dt;
        }
        public static string Sapo(object value, int words, bool noMore = false)
        {
            string sapo = Obj2String(value);
            if (string.IsNullOrEmpty(sapo) || sapo.Split(' ').Length < words)
                return sapo;

            string result = string.Empty;
            string[] arrWord = sapo.Split(' ');
            for (int i = 0; i < words; i++)
                result += arrWord[i] + " ";

            result = result.TrimEnd(' ') + (noMore ? "" : "...");

            return result;
        }
    }
}