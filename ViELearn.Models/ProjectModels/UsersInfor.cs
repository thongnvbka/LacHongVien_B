using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("UsersInfor")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public partial class UsersInfor
    {
        public int Id { get; set; }
        public string IdAcc { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string Position { get; set; }
        public Nullable<bool> IsRoot { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}
