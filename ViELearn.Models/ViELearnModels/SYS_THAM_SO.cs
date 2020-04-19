namespace ViELearn.Models.ViELearnModels
{
    using System;
    using PetaPoco;

    [TableName("SYS_THAM_SO")]
    [PrimaryKey("ID_TS", AutoIncrement = true)]

    public partial class SYS_THAM_SO
    {
        public short ID_TS { get; set; }
        public string NHOM_TS { get; set; }
        public string MA_TS { get; set; }
        public string TEN_TS { get; set; }
        public byte GIA_TRI_TS { get; set; }
        public string MO_TA_TS { get; set; }
        public string THONG_TIN_TS { get; set; }
        public System.DateTime NGAY { get; set; }
        public System.DateTime NBD { get; set; }
        public Nullable<System.DateTime> NKT { get; set; }
    }
}
