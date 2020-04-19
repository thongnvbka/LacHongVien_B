using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("FrontendMenuRole")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public partial class FrontendMenuRole
    {
        public int Id { get; set; }
        public int FrontendMenuId { get; set; }
        public string RoleId { get; set; }
        public string AccessRight { get; set; }
    }
}
