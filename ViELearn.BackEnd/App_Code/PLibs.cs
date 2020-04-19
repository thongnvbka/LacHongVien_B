using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace ViELearn.BackEnd.App_Code
{
    public class PLibs
    {
        public static string GetAdminUrl()
        {
            return ConfigurationManager.AppSettings["backend_url"];
        }
    
        public static string GetDhtnUrl()
        {
            return ConfigurationManager.AppSettings["qlvbdhtn_url"];
        }
        public static string CkImg404(string img)
        {
            if (img == null || img.Trim() == "")
                return "UITheme/assets/global/img/no-image.png";
            else return img.Trim();
        }


        public static string GenerateSlug(string phrase, bool tolower = false)
        {
            var str = (tolower) ? phrase.ToLower() : phrase;
            var from = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴàáäâèéëêìíïîòóöôùúüûñç·/_:;";
            var to = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyaaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyaaaaeeeeiiiioooouuuunc-----";
            for (var i = 0; i < from.Length; i++)
            {
                str = str.Replace(from[i], to[i]);
            }

            //string str = RemoveAccent(phrase).ToLower();
            // invalid chars           
            //if (tolower)
            //    str = Regex.Replace(str, @"[^a-z0-9\s- ]", "");
            //else
                str = Regex.Replace(str, @"[^a-zA-Z0-9\s- ]", "");

            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = Regex.Replace(str, @"-+", " ").Trim();
            while (str.Contains(" ")) str = str.Replace(" ", "-");
            while (str.Contains("--")) str = str.Replace("--", "-");
            str = str.Replace("\"", "");
            return str;
        }
    }
}