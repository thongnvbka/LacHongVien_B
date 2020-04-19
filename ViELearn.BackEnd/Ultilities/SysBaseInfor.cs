using System.Configuration;
using System.Web;

namespace ViELearn.BackEnd.Ultilities
{    
    public enum UserPermissions
    {
        QuyenXem = 0,
        QuyenThemMoi = 1,
        QuyenSua = 2,
        QuyenXoa = 3,
        QuyenChuyen = 4,
        QuyenDuyet = 5,
        QuyenXuatBan = 6
    }
    public enum UserRoleType
    {
        HostAdmin = 0,
        UnitAdmin = 1
    }

    public static class SysBaseDirectory
    {
        public static string pictureNews = "AnhTinBai";
        public static string pictureThumbnail = "AnhThumbnail";
        public static string pictureUploaded = "AnhTaiLen";
        public static string pictureOther = "AnhKhac";
        public static string newsAttachment = "TaiLieuDinhKem";
        public static string LawDocumentAttachment = "VanBanQPPL";
    }

    public static class SysBaseInfor
    {
        public static int sysMenuQuyenXem = 0;
        public static int sysMenuQuyenSua = 1;
        public static int sysMenuQuyenXoa = 2;
        public static int sysMenuQuyenThemMoi = 3;

        //public static string SuperAdmin = ConfigurationManager.AppSettings["sadmin_uname"] +  "@" + ConfigurationManager.AppSettings["sadmin_unit"];
        public static string SuperAdmin = ConfigurationManager.AppSettings["sadmin_uname"] ;
        public static string SuperAdminPass = ConfigurationManager.AppSettings["sadmin_pword"];
        public static string SuperAdminRoleName = "HostAdmin";
        public static string DefaultUnit = ConfigurationManager.AppSettings["default_unit"];
        public static string SignProject = "^&@*#";
        
        public static string GetCurrentUnitName()
        {
            return (string)HttpContext.Current.Session["CurrentUnitName"];
        }
        public static void SetCurrentUnitName(string unitName)
        {
            HttpContext.Current.Session["CurrentUnitName"] = unitName;
        }
        public static string GetCurrentUnitCode()
        {
            return (string)HttpContext.Current.Session["CurrentUnitCode"];
        }
        public static void SetCurrentUnitCode(string unitName)
        {
            HttpContext.Current.Session["CurrentUnitCode"] = unitName;
        }
        public static string GetCurrentUnitId()
        {
            try
            {
                return HttpContext.Current.Session["CurrentUnitId"].ToString();
            }
            catch { return ""; }
        }
        public static void SetCurrentUnitId(int unitId)
        {
            HttpContext.Current.Session["CurrentUnitId"] = unitId;
        }
        public static string GetCurrentUserName()
        {
            return (string)HttpContext.Current.Session["CurrentUserName"];
        }
        public static void SetCurrentUserName(string userName)
        {
            HttpContext.Current.Session["CurrentUserName"] = userName;
        }
        public static string GetCurrentUserDisplayName()
        {
            return (!string.IsNullOrEmpty((string)HttpContext.Current.Session["CurrentUserDisplayName"])? HttpContext.Current.Session["CurrentUserDisplayName"].ToString() : GetCurrentUserName());
        }
        public static void SetCurrentUserDisplayName(string userDisplayName)
        {
            HttpContext.Current.Session["CurrentUserDisplayName"] = userDisplayName;
        }
        public static string GetCurrentUserType()
        {
            return (string)HttpContext.Current.Session["CurrentUserType"];
        }
        public static void SetCurrentUserType(int userType)
        {
            HttpContext.Current.Session["CurrentUserType"] = userType;
        }
        public static string GetCurrentUserTypeInfo()
        {
            return HttpContext.Current.Session["CurrentUserTypeInfo"].ToString();
        }
        public static void SetCurrentUserTypeInfo(int userTypeInfo)
        {
            HttpContext.Current.Session["CurrentUserTypeInfo"] = userTypeInfo;
        }
        public static string GetCurrentUserId()
        {
            return (string)HttpContext.Current.Session["CurrentUserId"];
        }
        public static void SetCurrentUserId(string userID)
        {
            HttpContext.Current.Session["CurrentUserId"] = userID;
        }
        public static void SetCurrentRoleName(string roleName)
        {
            HttpContext.Current.Session["CurrentRoleName_" + SysBaseInfor.GetCurrentUserId()] = roleName;
        }
        public static string GetCurrentRoleName()
        {
            return HttpContext.Current.Session["CurrentRoleName_" + SysBaseInfor.GetCurrentUserId()].ToString();
        }

        public static void SetCurrentMediaURL(string roleName)
        {
            HttpContext.Current.Session["CurrentMediaURL_" + SysBaseInfor.GetCurrentUserId()] = roleName;
        }
        public static string GetCurrentMediaURL()
        {
            return (string)HttpContext.Current.Session["CurrentMediaURL_" + SysBaseInfor.GetCurrentUserId()];
        }
        public static void SetCurrentRoleId(string roleID)
        {
            HttpContext.Current.Session["CurrentRoleId_" + SysBaseInfor.GetCurrentUserId()] = roleID;
        }
        public static string GetCurrentRoleId()
        {
            return HttpContext.Current.Session["CurrentRoleId_" + SysBaseInfor.GetCurrentUserId()].ToString();
        }
        public static void SetCurrentUserListMenu(string lstSysMenus)
        {
            HttpContext.Current.Session["CurrentUserListMenu"] = lstSysMenus;
        }
        public static string GetListSysMenu()
        {
            return (string)HttpContext.Current.Session["CurrentUserListMenu"];
        }

        public static void SetCurrentUnitLogoUrl(string unitLogoUrl)
        {
            HttpContext.Current.Session["CurrentUnitLogoUrl"] = unitLogoUrl;
        }
        public static string GetCurrentUnitLogoUrl()
        {
            return (string)HttpContext.Current.Session["CurrentUnitLogoUrl"];
        }

        public static string GetTenNguoiDangNhap()
        {
            string value = "";
            try
            {
                value = HttpContext.Current.Session["Ten_Nguoi_Dang_NNhap"].ToString().ToLower().Trim();
                //if (!value.StartsWith("admin@") && !value.StartsWith("mdc"))
                //    value = value.Replace(GetMaTruong().ToLower().Trim(), "");
            }
            catch { }
            return value;
        }
        public static void SetTenNguoiDangNhap(string userName)
        {
            HttpContext.Current.Session["Ten_Nguoi_Dang_NNhap"] = userName;
        }
        public static string Get_LoggedAccount_Displayname()
        {
            string value = "";
            try
            {
                string v = HttpContext.Current.Session["Ten_Nguoi_Dang_NNhap"].ToString().ToLower().Trim();
                if (v.StartsWith("admin@mdcgroup"))
                    value = "Tổng quản công nghệ";
                else if (v.StartsWith("admin@"))
                    value = "Quản trị nhà trường";
                else if (v.StartsWith("mdc"))
                    value = "Quản trị viên MDC";
                else
                    value = HttpContext.Current.Session["LoggedAccount_Displayname"].ToString().Trim();
            }
            catch { }
            return value;
        }
        public static void Set_LoggedAccount_Displayname(string userName)
        {
            HttpContext.Current.Session["LoggedAccount_Displayname"] = userName;
        }
        public static string Get_LoggedAccount_Avatar()
        {
            string value = "";
            try
            {
                value = HttpContext.Current.Session["LoggedAccount_Avatar"].ToString().Trim();
            }
            catch { }
            if (value == "") value = "/UITheme/assets/layouts/layout/img/avatar_man_3.png";
            return value;
        }
        public static void Set_LoggedAccount_Avatar(string userName)
        {
            HttpContext.Current.Session["LoggedAccount_Avatar"] = userName;
        }
        public static string GetDisplayName_Logged_User()
        {
            return (string)HttpContext.Current.Session["DisplayName_Logged_User"];
        }
        public static void SetDisplayName_Logged_User(string userName)
        {
            HttpContext.Current.Session["DisplayName_Logged_User"] = userName;
        }
        /// <summary>
        /// id dạng string dài (guid)
        /// </summary>
        /// <returns></returns>
        public static string GetIdNguoiDangNhap()
        {
            return (string)HttpContext.Current.Session["ID_Nguoi_Dang_Nhap"];
        }
        public static void SetIdNguoiDangNhap(string userID)
        {
            HttpContext.Current.Session["ID_Nguoi_Dang_Nhap"] = userID;
        }
        /// <summary>
        /// id dạng số int
        /// </summary>
        /// <returns></returns>
        public static string GetIdNguoiDung()
        {
            var id = (string)HttpContext.Current.Session["ID_nguoi_dung"];
            return id;
        }
        public static void SetIdNguoiDung(string user_id)
        {
            HttpContext.Current.Session["ID_nguoi_dung"] = user_id;
        }

        public static string GetQuyenNguoiDangNhap()
        {
            return (string)HttpContext.Current.Session["Quyen_Nguoi_Dang_NNhap"];
        }
        public static void SetQuyenNguoiDangNhap(string typeUser)
        {
            HttpContext.Current.Session["Quyen_Nguoi_Dang_NNhap"] = typeUser;
        }
        public static string GetIdNhomNguoiDUng()
        {
            return (string)HttpContext.Current.Session["DanhSach_Nhom_Nguoi_Dung"];
        }
        public static void SetIdNhomNguoiDUng(string userRole)
        {
            HttpContext.Current.Session["DanhSach_Nhom_Nguoi_Dung"] = userRole;
        }
    }
}