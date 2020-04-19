using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("DmXa")]
    [PrimaryKey("ID_xa", AutoIncrement = true)]

    public partial class DmXa
    {
        public int ID_xa { get; set; }
        public string Ma_xa { get; set; }
        public string Ten_xa { get; set; }
        public int ID_huyen { get; set; }
        public Nullable<int> ID_khu_vuc { get; set; }
        public Nullable<int> ID_mien { get; set; }
    }
}
