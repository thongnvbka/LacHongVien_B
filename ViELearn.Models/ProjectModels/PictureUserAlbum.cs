using System;
using System.Collections.Generic;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("PictureUserAlbum")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public partial class PictureUserAlbum
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IdUnit { get; set; }
        public string IdUser { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? OrderView { get; set; }
        public bool? Active { get; set; }
    }

}
