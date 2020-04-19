using System;
using System.Collections.Generic;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("PictureUser")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public partial class PictureUser
    {
        public int Id { get; set; }
        public int IdAlbum { get; set; }
        public int IdUnit { get; set; }
        public string PictureUrl { get; set; }
        public string PictureName { get; set; }
        public string PictureInfo { get; set; }      
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
