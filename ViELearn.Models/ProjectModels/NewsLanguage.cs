using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViELearn.Models.ProjectModels
{
    [TableName("NewsLanguage")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class NewsLanguage
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Summary { get; set; }
        public string FullContent { get; set; }
    }
}