using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{ 
    [TableName("VuViec")]
    [PrimaryKey("VuViecID", AutoIncrement = true)]
    public partial class VuViec
    {
        public int VuViecID { get; set; }
        public int LoaiVuViecID { get; set; }
        public int LoaiVuViecChiTietID { get; set; }
        public string TenCoSo { get; set; }
        public string NoiPhatSinhChayNo { get; set; }
        public long idSoCanhSat { get; set; }
        public long idPhongCanhSat { get; set; }
        public long idDoiCanhSat { get; set; }
        public int PhuongXa { get; set; }
        public int QuanHuyen { get; set; }
        public int TinhThanhPho { get; set; }
        public string KhoangCach { get; set; }
        public int KhuVucID { get; set; }
        public int NoiXayRaID { get; set; }
        public string SoTang { get; set; }
        public int LoaiPhuongTienID { get; set; }
        public string HoTenNguoiBao { get; set; }
        public string DiaChiNguoiBao { get; set; }
        public string DienThoaiNguoiBao { get; set; }
        public string CoQuanQuanLy { get; set; }
        public int NganhLinhVucID { get; set; }
        public int ThanhPhanKinhTeID { get; set; }
        public int NuocDauTuID { get; set; }
        public string LoaiNhaBiChay { get; set; }
        public string ChatChayChuYeu { get; set; }
        public DateTime? ThoiGianXayRaVuViec { get; set; }
        public DateTime? ThoiGianPhatHien { get; set; }
        public DateTime? ThoiGianNhanTinBao { get; set; }
        public DateTime? ThoiGianXuatXe { get; set; }
        public DateTime? ThoiGianDenHienTruong { get; set; }
        public DateTime? ThoiGianKhongCheDamChay { get; set; }
        public DateTime? ThoiGianDamChayTat { get; set; }
        public string ThoiGianChuaChay { get; set; }
        public DateTime? ThoiGianKetThucVuViec { get; set; }
        public string DienTichDamChay1 { get; set; }
        public string DienTichDamChay2 { get; set; }
        public string DienTichChuaChay { get; set; }
        public int Chet { get; set; }
        public int BiThuong { get; set; }
        public string ThietHaiTaiSan { get; set; }
        public string TaiSanCuThe { get; set; }
        public string AnhHuongKhac { get; set; }
        public int NguyenNhanID { get; set; }
        public int NguyenNhanChiTietID { get; set; }
        public int LoiHanhViID { get; set; }
        public string LoiHanhViChiTiet { get; set; }
        public int SoNguoiDuocCuu { get; set; }
        public int SoNguoiThoatNan { get; set; }
        public int SoXacNanNhan { get; set; }
        public string TaiSanCuuDuoc { get; set; }
        public string HauQuaKhac { get; set; }
        public int XuLyHanhChinh { get; set; }
        public int KhoiToVuAn { get; set; }
        public int CoQuanKhoiToID { get; set; }
        public int SoBiCan { get; set; }
        public string DienBienVuViec { get; set; }
        public int SoNguoiThamGia1 { get; set; }
        public int SoNguoiThamGia2 { get; set; }
        public int SoNguoiThamGia3 { get; set; }
        public int SoNguoiThamGia4 { get; set; }
        public int XeChuaChay1 { get; set; }
        public int XeChuaChay2 { get; set; }
        public int XeChuaChay3 { get; set; }
        public int XeChuaChay4 { get; set; }
        public int XeChoNuoc1 { get; set; }
        public int XeChoNuoc2 { get; set; }
        public int XeChoNuoc3 { get; set; }
        public int XeChoNuoc4 { get; set; }
        public int XeThang1 { get; set; }
        public int XeThang2 { get; set; }
        public int XeThang3 { get; set; }
        public int XeThang4 { get; set; }
        public int XeCuuHo1 { get; set; }
        public int XeCuuHo2 { get; set; }
        public int XeCuuHo3 { get; set; }
        public int XeCuuHo4 { get; set; }
        public int XeChuyenDung1 { get; set; }
        public int XeChuyenDung2 { get; set; }
        public int XeChuyenDung3 { get; set; }
        public int XeChuyenDung4 { get; set; }
        public int MayBom1 { get; set; }
        public int MayBom2 { get; set; }
        public int MayBom3 { get; set; }
        public int MayBom4 { get; set; }
        public int TauChuaChay1 { get; set; }
        public int TauChuaChay2 { get; set; }
        public int TauChuaChay3 { get; set; }
        public int TauChuaChay4 { get; set; }
        public int PhuongTienKhac1 { get; set; }
        public int PhuongTienKhac2 { get; set; }
        public int PhuongTienKhac3 { get; set; }
        public int PhuongTienKhac4 { get; set; }
        public int XuLyID { get; set; }
        public int NoiXayRaChiTietID { get; set; }
        public string DiaChiCoSo { get; set; }
        public string CanhSatPCCC { get; set; }
        public string SoBaoCao { get; set; }
        public DateTime? NgayBaoCao { get; set; }
        public DateTime? NgayChay { get; set; }
        public short? TrangThai { get; set; }
        public string NguoiTao { get; set; }
        public string MaVuViec { get; set; }
        public int MaDonVi { get; set; }

        [ResultColumn]
        public string TenNguoiTao { get; set; }
    }
}
