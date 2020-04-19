using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("DmTinhThanh")]
    [PrimaryKey("Id", AutoIncrement = true)]

    public partial class DmTinhThanh
    {    
        public int Id { get; set; }
        public string TenTinhThanh { get; set; }
        public int? ThuTu { get; set; }
        public int? KichHoat { get; set; }    
    }
}
