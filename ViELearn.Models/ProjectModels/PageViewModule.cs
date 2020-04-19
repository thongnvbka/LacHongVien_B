using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("PageViewModule")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public partial class PageViewModule
    {
        public int Id { get; set; }
        public int IdPage { get; set; }
        public int IdModule { get; set; }        
        public int IdUnit { get; set; }
        public string JsonData { get; set; }
        public string CssClass { get; set; }
        public Nullable<short> Position { get; set; }
        public Nullable<short> OrderView { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public string Note { get; set; }
        [ResultColumn]
        public string PageName { get; set; }
        [ResultColumn]
        public string ModuleName { get; set; }
        [ResultColumn]
        public string ModuleCode { get; set; }
    }
}