using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{   
    
    [TableName("News")]
    [PrimaryKey("Id", AutoIncrement = true)]

    public partial class News
    {
        public int Id { get; set; }
        public int CategoryID { get; set; }        
        public int UnitId { get; set; }
        public string Title { get; set; }

        public string Slug { get; set; }
        public string Thumbnail { set; get; }
        public int CatesId { set; get; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string FullContent { get; set; }
        public string Tags { get; set; }
        public Nullable<byte> Type { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<int> ReadCounter { get; set; }
        public Nullable<byte> Active { get; set; }
        public Nullable<DateTime> CreatedAt { get; set; }
        public Nullable<DateTime> UpdatedAt { get; set; }

        public int PublishAt { set;get; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        
        [ResultColumn]
        public string CategoryName { get; set; }
        [ResultColumn]
        public string UserCreatedName { get; set; }
        [ResultColumn]
        public string CategoryType { get; set; }

        public int top_hot { set; get; }



    }
}
