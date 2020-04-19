using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Web;

namespace ViELearn.BackEnd.Ultilities
{

    public class JsonNetFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is JsonResult == false)
            {
                return;
            }

            filterContext.Result = new JsonNetResult(
                (JsonResult)filterContext.Result);
        }

        private class JsonNetResult : JsonResult
        {
            public JsonNetResult(JsonResult jsonResult)
            {
                this.ContentEncoding = jsonResult.ContentEncoding;
                this.ContentType = jsonResult.ContentType;
                this.Data = jsonResult.Data;
                this.JsonRequestBehavior = jsonResult.JsonRequestBehavior;
                this.MaxJsonLength = jsonResult.MaxJsonLength;
                this.RecursionLimit = jsonResult.RecursionLimit;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                var isMethodGet = string.Equals(
                    context.HttpContext.Request.HttpMethod,
                    "GET",
                    StringComparison.OrdinalIgnoreCase);

                if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet
                    && isMethodGet)
                {
                    throw new InvalidOperationException(
                        "GET not allowed! Change JsonRequestBehavior to AllowGet.");
                }

                var response = context.HttpContext.Response;

                response.ContentType = string.IsNullOrEmpty(this.ContentType)
                    ? "application/json"
                    : this.ContentType;

                if (this.ContentEncoding != null)
                {
                    response.ContentEncoding = this.ContentEncoding;
                }

                if (this.Data != null)
                {
                    response.Write(JsonConvert.SerializeObject(this.Data));
                }
            }
        }
    }
    public static class ProjectTools
    {
        #region "Convert"
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
        public static string ConvertVNtoEN(string strText)
        {
            const string findText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
            const string replText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
            int index;
            char[] arrChar = findText.ToCharArray();
            while ((index = strText.IndexOfAny(arrChar)) != -1)
            {
                int index2 = findText.IndexOf(strText[index]);
                strText = strText.Replace(strText[index], replText[index2]);
            }
            return strText;
        }
        #endregion
    }


    public static class Extension
    {
        static Extension()
        {
            ImageTypes = new Dictionary<string, string>();
            ImageTypes.Add("FFD8", "jpg");
            ImageTypes.Add("424D", "bmp");
            ImageTypes.Add("474946", "gif");
            ImageTypes.Add("89504E", "png");
            //ImageTypes.Add("89504E470D0A1A0A", "png");
        }

        /// <summary>
        ///     <para> Registers a hexadecimal value used for a given image type </para>
        ///     <param name="imageType"> The type of image, example: "png" </param>
        ///     <param name="uniqueHeaderAsHex"> The type of image, example: "89504E470D0A1A0A" </param>
        /// </summary>
        public static void RegisterImageHeaderSignature(string imageType, string uniqueHeaderAsHex)
        {
            Regex validator = new Regex(@"^[A-F0-9]+$", RegexOptions.CultureInvariant);

            uniqueHeaderAsHex = uniqueHeaderAsHex.Replace(" ", "");

            if (string.IsNullOrWhiteSpace(imageType)) throw new ArgumentNullException("imageType");
            if (string.IsNullOrWhiteSpace(uniqueHeaderAsHex)) throw new ArgumentNullException("uniqueHeaderAsHex");
            if (uniqueHeaderAsHex.Length % 2 != 0) throw new ArgumentException("Hexadecimal value is invalid");
            if (!validator.IsMatch(uniqueHeaderAsHex)) throw new ArgumentException("Hexadecimal value is invalid");

            ImageTypes.Add(uniqueHeaderAsHex, imageType);
        }

        private static Dictionary<string, string> ImageTypes;

        public static bool IsImage(this Stream stream)
        {
            string imageType;
            return stream.IsImage(out imageType);
        }

        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static bool IsImage(this Stream stream, out string imageType)
        {
            stream.Seek(0, SeekOrigin.Begin);
            StringBuilder builder = new StringBuilder();
            int largestByteHeader = ImageTypes.Max(img => img.Value.Length);

            for (int i = 0; i < largestByteHeader; i++)
            {
                string bit = stream.ReadByte().ToString("X2");
                builder.Append(bit);

                string builtHex = builder.ToString();
                bool isImage = ImageTypes.Keys.Any(img => img == builtHex);
                if (isImage)
                {
                    imageType = ImageTypes[builder.ToString()];
                    return true;
                }
            }
            imageType = null;
            return false;
        }

        public static string GenerateSlug(this string phrase)
        {
            string str = phrase.RemoveAccent().ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        private static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}