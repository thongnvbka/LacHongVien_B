using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("FrontendMenu")]
    [PrimaryKey("Id", AutoIncrement = true)]

    public partial class FrontendMenu
    {
        public int Id { get; set; }
        public int UnitId { get; set; }
        public string Name { get; set; }
        public byte MenuLevel { get; set; }
        public int? ParrentId { get; set; }
        public byte HaveChild { get; set; }
        public bool IsLeftFromMainPage { get; set; }
        public bool IsRightFromMainPage { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte LengthLeft { get; set; }
        public byte LengthCenter { get; set; }
        public byte LengthRight { get; set; }
        public byte? OrderView { get; set; }
        public byte Status { get; set; }
        public string Type { get; set; }
        public string JsonData { get; set; }
        public byte AccessType { get; set; }
    }
}
