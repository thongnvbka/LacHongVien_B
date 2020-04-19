using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("DmHuyen")]
    [PrimaryKey("ID_huyen", AutoIncrement = true)]

    public partial class DmHuyen
    {
        public int ID_huyen { get; set; }
        public string Ma_huyen { get; set; }
        public string Ten_huyen { get; set; }
        public int ID_tinh { get; set; }
        public string Ten_phong_giao_duc { get; set; }
    }
}
