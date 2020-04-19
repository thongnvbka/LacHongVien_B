using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViELearn.Models.ProjectModels
{
    [PetaPoco.TableName("Menus")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class Menus
    {
        public int Id { set; get; }
        public string Name { set; get; }

        public string URL { set; get; }
        public int DisplayOrder { set; get; }
        public int GroupID { set; get; }
        public int Target { set; get; }
        public int Status { set; get; } 

    }
}