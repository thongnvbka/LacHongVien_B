using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("NewsCategories")]
    [PrimaryKey("Id", AutoIncrement = true)]

    public partial class NewsCategories
    {
        public string Name { get; set; }
        public int UnitID { get; set; }
        public Nullable<int> OrderView { get; set; }
        public byte Type { get; set; }
        public bool Active { get; set; }
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte AccessType { get; set; }
        [ResultColumn]
        public string TypeName { get; set; }
    }
}
