using System;
using PetaPoco;

namespace ViELearn.Models.SystemModels
{
    [TableName("SYS_MENU")]
    [PrimaryKey("ID_SYS_MENU", AutoIncrement = true)]
    public partial class SYS_MENU
    {
        public int ID_SYS_MENU { get; set; }
        public string TENDANHMUC { get; set; }
        public byte? CAPDO { get; set; }
        public int? DANHMUCCHA { get; set; }
        public byte? DANHMUCCON { get; set; }
        public string CHITIET { get; set; }
        public string GHICHU { get; set; }
        public string CLASS { get; set; }
        public string CLASS_MAIN { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public DateTime? UPDATEAT { get; set; }
        public string ACTION { get; set; }
        public string CONTROLLER { get; set; }
        public Nullable<byte> ORDERVIEW { get; set; }
        public bool Status { get; set; }

        [ResultColumn]
        public string ACCESS_RIGHT { get; set; }
    }
}
