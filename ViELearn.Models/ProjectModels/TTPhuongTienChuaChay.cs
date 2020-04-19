using PetaPoco;
using System;

namespace ViELearn.Models.ProjectModels
{
    [TableName("TTPhuongTienChuaChay")]
    [PrimaryKey("Id", AutoIncrement = true)]

    public partial class TTPhuongTienChuaChay
    {
        public int Id { get; set; }
        public int DonVi { get; set; }
        public int Phong { get; set; }
        public int Doi { get; set; }
        public string TenPhuongTien { get; set; }
        public int LoaiPhuongTien { get; set; }
        public int PhanLoaiChiTiet { get; set; }
        public string BienKiemSoat { get; set; }
        public string SoKhung { get; set; }
        public string SoMay { get; set; }
        public Nullable<int> TinhTrangSuDung { get; set; }
        public Nullable<int> LoaiNhienLieu { get; set; }
        public Nullable<int> NuocSanXuat { get; set; }
        public string HangSanXuat { get; set; }
        public Nullable<int> NamSanXuat { get; set; }
        public Nullable<int> NamDuaVaoSuDung { get; set; }
        public Nullable<int> NguonCap { get; set; }
        public Nullable<int> DTKT_LoaiXeNen { get; set; }
        public Nullable<int> DTKT_CongSuatDongCo { get; set; }
        public string DTKT_MaHieuHeThong { get; set; }
        public Nullable<int> DTKT_DungTichKetNuoc { get; set; }
        public Nullable<int> DTKT_DungTichKetThuoc { get; set; }
        public Nullable<int> DTKT_LuuLuongBom { get; set; }
        public Nullable<int> DTKT_CotAp { get; set; }
        public Nullable<int> DTKT_TongTrongLuongXe { get; set; }
        public Nullable<int> DTKT_DoCaoThang { get; set; }
        public Nullable<int> DTKT_Dai { get; set; }
        public Nullable<int> DTKT_Rong { get; set; }
        public Nullable<int> DTKT_Cao { get; set; }
        public string TTKT_HeThongMay { get; set; }
        public string TTKT_HeThongChuyenDung { get; set; }
        public string TTKT_VoXeDanDong { get; set; }
        public int ChatLuongPhuongTien { get; set; }
        public string ThongTinKhacCoLienQuan { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiTao { get; set; }
        public string NguoiSua { get; set; }
    }
}