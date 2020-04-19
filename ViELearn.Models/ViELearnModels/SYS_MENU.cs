namespace ViELearn.Models.ViELearnModels
{
    using System;
    using PetaPoco;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    [TableName("SYS_MENU")]
    [PrimaryKey("ID_SYS_MENU", AutoIncrement = true)]
    public partial class SYS_MENU
    {
        public int ID_SYS_MENU { get; set; }
        public string TENDANHMUC { get; set; }
        public int CAPDO { get; set; }
        public int DANHMUCCHA { get; set; }
        public int DANHMUCCON { get; set; }
        public string CHITIET { get; set; }
        public string GHICHU { get; set; }
        public string CLASS { get; set; }
        public string CLASS_MAIN { get; set; }
        public Nullable<System.DateTime> CREATEDATE { get; set; }
        public Nullable<System.DateTime> UPDATEAT { get; set; }
        public string ACTION { get; set; }
        public string CONTROLLER { get; set; }
        public int ORDERVIEW { get; set; }

        [ResultColumn]
        public string ACCESS_RIGHT { get; set; }
    }
}
