using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("Themes")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public partial class Themes
    {
        public int Id { get; set; }
        public string CssName { get; set; }
        public string Name { get; set; }
        public string ImagePreview { get; set; }
        public bool Active { get; set; }
        public string LayoutView { get; set; }
    }
}