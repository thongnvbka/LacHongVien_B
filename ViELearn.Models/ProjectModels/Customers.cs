using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViELearn.Models.ProjectModels
{
    [TableName("Customers")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class Customers
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Service { get; set; }
        public int CreatedAt { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
    }
}