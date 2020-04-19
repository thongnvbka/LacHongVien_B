using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("SpecificationParameters")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public partial class SpecificationParameters
    {
        public int Id { get; set; }
        public string MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public int ThuTu { get; set; }
        public byte TrangThai { get; set; }
        public int DanhMucChaId { get; set; }
    }
}