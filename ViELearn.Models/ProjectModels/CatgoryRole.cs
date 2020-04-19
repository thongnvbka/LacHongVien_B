using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("CatgoryRole")]
    [PrimaryKey("Id", AutoIncrement = true)]

    public partial class CatgoryRole
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string RoleId { get; set; }
        public string AccessRight { get; set; }
    }
}