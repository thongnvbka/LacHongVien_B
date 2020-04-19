using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("DanhMucChung")]
    [PrimaryKey("id", AutoIncrement = true)]

    public partial class DanhMucChung
    {
        public long id { get; set; }
        public string TenDanhMuc { get; set; }
        public string MaDanhMuc { get; set; }
        public short LoaiDanhMuc { get; set; }
        public short SoThuTu { get; set; }
        public bool TrangThai { get; set; }
        public Nullable<long> idDanhMucCha { get; set; }
        public Nullable<int> created_at { get; set; }
        public Nullable<int> updated_at { get; set; }

        public short Type { set; get; }
        public string Path { set; get; }
        public string Thumbnail { set; get; }

        public bool ShowHome { get; set; }
        public string Url { set; get; }

        public int TongSoTin { set; get; }

        public string tenCha { set; get; }

    }
}
