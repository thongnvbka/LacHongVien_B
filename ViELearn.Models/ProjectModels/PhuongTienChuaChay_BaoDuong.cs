using PetaPoco;
using System;

namespace ViELearn.Models.ProjectModels
{
    [TableName("PhuongTienChuaChay_BaoDuong")]
    [PrimaryKey("Id", AutoIncrement = true)]
    
    public partial class PhuongTienChuaChay_BaoDuong
    {
        public int Id { get; set; }
        public int PhuongTienChuaChayId { get; set; }
        public int Stt { get; set; }
        public DateTime? ThoiGian { get; set; }
        public string NoiDungSuaChua { get; set; }
        public int LoaiSuaChua { get; set; }
    }
}
