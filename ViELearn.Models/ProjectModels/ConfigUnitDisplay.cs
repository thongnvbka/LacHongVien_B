using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("ConfigUnitDisplay")]
    [PrimaryKey("UnitId", AutoIncrement = false)]    
    public partial class ConfigUnitDisplay
    {
        public int UnitId { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public string ConfigHeaderJson { get; set; }
        public string ConfigFooterJson { get; set; }
        public string ConfigTextRunJson { get; set; }
        public int ThemeId { get; set; }
        public string CssName { get; set; }
    }
}
