using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("DmTinh")]
    [PrimaryKey("ID_tinh", AutoIncrement = true)]

    public partial class DmTinh
    {
        public int ID_tinh { get; set; }
        public string Ma_tinh { get; set; }
        public string Ten_tinh { get; set; }
        public Nullable<int> ID_mien { get; set; }
        public string Ten_so_giao_duc { get; set; }
    }
}
