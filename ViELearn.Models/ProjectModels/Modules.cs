using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("Modules")]
    [PrimaryKey("Id", AutoIncrement = true)]

    public partial class Modules
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string JsonData { get; set; }
        public Nullable<int> UnitID { get; set; }
        public Nullable<bool> Active { get; set; }
        public int Id { get; set; }
    }
}
