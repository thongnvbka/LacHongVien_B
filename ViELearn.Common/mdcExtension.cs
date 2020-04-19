using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViELearn.Common
{
    public static class mdcExtension
    {
        

        #region "Convert"
        // To - throw khi gặp exception

        //public static int Gia_tri_int(string Ma_settting)
        //{
        //    try
        //    {
        //        DataTable dtThamSo = iThuocConnect.SelectTable(string.Format("SELECT * from tblThamSoHeThong WHERE Ma_settting = N'{0}'", Ma_settting.ToLower().Trim()));
        //        return dtThamSo.Rows[0]["Gia_tri"].MapInt();
        //    }
        //    catch (Exception)
        //    {
        //        return 0;
        //    }

        //}

        //public static string Gia_tri_string(string Ma_settting)
        //{
        //    try
        //    {
        //        DataTable dtThamSo = iThuocConnect.SelectTable(string.Format("SELECT * from tblThamSoHeThong WHERE Ma_settting = N'{0}'", Ma_settting.ToLower().Trim()));
        //        return dtThamSo.Rows[0]["Gia_tri"].MapStr();
        //    }
        //    catch (Exception)
        //    {

        //        return "";
        //    }
        //}

        public static string ToDateRFC3987(this object obj)
        {
            try
            {
                DateTime dt = obj.ToDate();
                if (dt == DateTime.MinValue)
                {
                    return "";
                }
                else
                {
                    String _mm = dt.Month > 9 ? dt.Month.MapStr() : ("0" + dt.Month.MapStr());
                    String _dd = dt.Day > 9 ? dt.Day.MapStr() : ("0" + dt.Day.MapStr());
                    return dt.Year + "-" + _mm + "-" + _dd;
                }
            }
            catch
            {
                return String.Empty;
            }
        }
        public static string ToDateVietNam(this object obj)
        {
            try
            {
                DateTime dt = obj.ToDate();
                return dt == DateTime.MinValue ? "" : dt.Date.ToString("dd/MM/yyyy");
            }
            catch
            {
                return String.Empty;
            }
        }

        public static string ToDateTimeVietNam(this object obj)
        {
            try
            {
                DateTime dt = DateTime.Parse(obj.ToString());
                return dt == DateTime.MinValue ? "" : dt.ToString("yyyy-MM-dd'T'HH:mm:ss.fff");
            }
            catch
            {
                return String.Empty;
            }
        }
        public static int ToInt32(this object obj)
        {
            if (obj == null) return 0;
            int a = 0;
            int.TryParse(obj.ToString(), out a);
            return a;
            //try
            //{
            //    return Int32.Parse(obj.ToString());
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
        }
        public static Double ToDouble(this object obj)
        {
            try
            {
                return Double.Parse(obj.ToString());
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static float ToFloat(this object obj)
        {
            try
            {
                return float.Parse(obj.ToString());
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static DateTime ToDate(this object obj)
        {
            try
            {
                return DateTime.Parse(obj.ToString()).Date;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static Boolean ToBoolean(this object obj)
        {
            try
            {
                return (obj.ToString().ToUpper().Trim().Equals("TRUE") || obj.ToString().Trim().Equals("1")) ? true : false;
            }
            catch (Exception)
            {

                throw;
            }

        }
        // Map - luôn trả về éo quan tâm ngoại lệ cl gì hết
        public static int MapInt(this object obj)
        {
            try
            {
                return obj.ToInt32();
            }
            catch
            {
                return 0;
            }
        }
        public static string MapStr(this object obj, bool tienTe = false)
        {
            try
            {
                String result = "";
                result = obj.ToString();
                if (tienTe)
                {
                    result = string.Format("{0:0,0}", result.MapDouble());
                }
                return result;
            }
            catch
            {

                return String.Empty;
            }
        }
        public static Double MapDouble(this object obj, int lenght = 2)
        {
            try
            {
                return Math.Round(obj.ToDouble(), lenght);
            }
            catch
            {

                return 0;
            }
        }
        public static DateTime MapDate(this object obj)
        {
            try
            {
                return obj.ToDate();
            }
            catch
            {

                return DateTime.MinValue;
            }
        }
        public static string MapDateTime(this object obj)
        {
            try
            {
                return obj.ToDateTimeVietNam();
            }
            catch
            {

                return DateTime.MinValue.ToString("HH:mm:ss dd/MM/yyyy");
            }
        }
        public static Boolean MapBool(this object obj)
        {
            try
            {
                return obj.ToBoolean();
            }
            catch
            {

                return false;
            }
        }
        public static float MapFloat(this object obj)
        {
            try
            {
                return obj.ToFloat();
            }
            catch
            {
                return 0;
            }
        }

        //public static int GetGV()
        //{
        //    return 1;
        //}

        //public static int GetUserDangNhap()
        //{
        //    return 1;
        //}

        //public static int GetTruongId()
        //{
        //    return 1;
        //}

        //public static int GetDiaDanhId()
        //{
        //    return 777;
        //}

        //public static short GetCurrentNamHocId()
        //{
        //    return 7;
        //}

        //public static string GetNamHocHienTai()
        //{
        //    return "2016-2017";
        //}

        //public static int getHocKy()
        //{
        //    return 1;
        //}

        //public static int GetTruongHienTai()
        //{
        //    return 1;
        //}

        //public static int GetDiemTruongHienTai()
        //{
        //    return 1;
        //}

        public static string ConvertVNtoEN(string strText)
        {
            return RemoveUnicode(strText);
            //const string findTextUnicodeDungSan = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
            //const string findTextUnicodeToHop1 =  "áàảãạấầẩẫậắằẳẵặéèẻẽẹếềểễệíìỉĩịóòỏõọốồổỗộớờởỡợúùủũụứừửữựýỳỷỹỵÁÀẢÃẠẤẦẨẪẬẮẰẲẴẶÉÈẺẼẸẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌỐỒỔỖỘỚỜỞỠỢÚÙỦŨỤỨỪỬỮỰÝỲỶỸỴ";
            //const string findTextDau = "áàảãạ";
            //const string findTextUnicodeToHop2 =  "âăđêôơưÂĂĐÊÔƠƯ";
            //const string replText =               "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
            //const string replText1 =              "aaaaaaaaaaaaaaaeeeeeeeeeeiiiiiooooooooooooooouuuuuuuuuuyyyyyAAAAAAAAAAAAAAAEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOUUUUUUUUUUYYYYY";
            //const string replText2 =              "aadeoouAADEOOU";
            //// Unicode tổ hợp
            //{
            //    int index;
            //    char[] arrChar = findTextUnicodeToHop1.ToCharArray();
            //    while ((index = strText.IndexOfAny(arrChar)) != -1)
            //    {
            //        int index2 = findTextUnicodeToHop1.IndexOf(strText[index]);
            //        var f = strText.Substring(0, strText[index]) + replText1[index2 / 2] +
            //                strText.Substring(strText[index] + 2);
            //        strText = f;
            //    }
            //}
            //// Unicode dựng sẵn
            //{
            //    int index;
            //    char[] arrChar = findTextUnicodeDungSan.ToCharArray();
            //    while ((index = strText.IndexOfAny(arrChar)) != -1)
            //    {
            //        int index2 = findTextUnicodeDungSan.IndexOf(strText[index]);
            //        strText = strText.Replace(strText[index], replText[index2]);
            //    }
            //}
            //return strText;
        }

        public static string RemoveUnicode(string text)
        {
            string[] arr_tohop = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                "đ",
                "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                "í","ì","ỉ","ĩ","ị",
                "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                "ý","ỳ","ỷ","ỹ","ỵ"};
            string[] arr_dungsan = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                "đ",
                "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                "í","ì","ỉ","ĩ","ị",
                "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                "ý","ỳ","ỷ","ỹ","ỵ"};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                "d",
                "e","e","e","e","e","e","e","e","e","e","e",
                "i","i","i","i","i",
                "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                "u","u","u","u","u","u","u","u","u","u","u",
                "y","y","y","y","y"};
            for (int i = 0; i < arr_tohop.Length; i++)
            {
                text = text.Replace(arr_tohop[i], arr2[i]);
                text = text.Replace(arr_tohop[i].ToUpper(), arr2[i].ToUpper());
            }
            for (int i = 0; i < arr_dungsan.Length; i++)
            {
                text = text.Replace(arr_dungsan[i], arr2[i]);
                text = text.Replace(arr_dungsan[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }

        public static int getIDTruong(string Ma_truong)
        {
            int ID_truong = 0;
            try
            {
                return Connect.ExecuteScalar("SELECT ID_truong FROM Truong WHERE Ma_truong = N'" + Ma_truong.ToUpper() + "'").MapInt();
            }
            catch
            {
                return ID_truong;
            }
        }

        public static int Hoc_ky(DateTime Ngay)
        {
            int Ky = 0;
            if (Ngay.Month >= 9 && Ngay.Month <= 12)
            { Ky = 1; }
            else { Ky = 2; };
            return Ky;
        }

        public static string Nam_hoc(DateTime Ngay)
        {
            string Nam = "";
            if (Ngay.Month >= 9 && Ngay.Month <= 12)
            {
                Nam = Ngay.Year + "-" + (Ngay.Year + 1);
            }
            else
            {
                Nam = (Ngay.Year - 1) + "-" + Ngay.Year;
            }
            return Nam;
        }
        #endregion
    }
}