using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("SystemParameters")]
    [PrimaryKey("Id", AutoIncrement = true)]

    public partial class SystemParameters
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }
        public byte OrderView { get; set; }
    }
}