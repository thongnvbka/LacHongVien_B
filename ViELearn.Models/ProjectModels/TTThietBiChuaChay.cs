using PetaPoco;
using System;

namespace ViELearn.Models.ProjectModels
{
    [TableName("TTThietBiChuaChay")]
    [PrimaryKey("Id", AutoIncrement = true)]

    public partial class TTThietBiChuaChay
    {
        public int Id { get; set; }
        public int LoaiPhuongTien { get; set; }
        public string TenPhuongTien { get; set; }
        public int PhanLoaiChiTiet { get; set; }
        public Nullable<int> NuocSanXuat { get; set; }
        public string MaHieu { get; set; }
        public string DacTinhhKyThuat { get; set; }
        public string TTSD1_DonVi { get; set; }
        public int TTSD1_TongSo { get; set; }
        public int TTSD1_Tot { get; set; }
        public int TTSD1_TrungBinh { get; set; }
        public int TTSD1_Kem { get; set; }
        public int TTSD1_HuHong { get; set; }
        public string TTSD2_DonVi { get; set; }
        public int TTSD2_TongSo { get; set; }
        public int TTSD2_Tot { get; set; }
        public int TTSD2_TrungBinh { get; set; }
        public int TTSD2_Kem { get; set; }
        public int TTSD2_HuHong { get; set; }
        public int DonVi { get; set; }
        public int Phong { get; set; }
        public int Doi { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<System.DateTime> NgaySuaDoi { get; set; }
        public string NguoiTao { get; set; }
        public string NguoiSua { get; set; }
    }
}
