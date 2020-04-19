using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("ModuleTypes")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class ModuleTypes
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
    }
}
