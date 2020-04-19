using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("Units")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public partial class Units
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Url { get; set; }
        public string MediaUrl { get; set; }
        public string SecondUrl { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string LogoUrl { get; set; }
        public Nullable<short> UnitType { get; set; }
        public Nullable<bool> Active { get; set; }
        public int MainPageId { get; set; }
        [ResultColumn]
        public int ThemeId { get; set; }
        [ResultColumn]
        public string LayoutView { get; set; }
        [ResultColumn]
        public string CssName { get; set; }
    }
}
