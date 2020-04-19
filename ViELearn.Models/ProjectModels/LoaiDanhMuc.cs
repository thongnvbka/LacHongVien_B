using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("LoaiDanhMuc")]
    [PrimaryKey("id", AutoIncrement = true)]

    public partial class LoaiDanhMuc
    {
        public int id { get; set; }
        public string maLoaiDm { get; set; }
        public string tenLoaiDm { get; set; }
        public string tenNganMenu { get; set; }
        public Nullable<int> soThuTu { get; set; }
        public string moTa { get; set; }
        public string tieuDe { get; set; }
        public Nullable<byte> status { get; set; }
    }
}
