using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("LawDocumentParameters")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public partial class LawDocumentParameters
    {
        public int Id { get; set; }
        public int UnitId { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public Nullable<short> OrderView { get; set; }
    }
}