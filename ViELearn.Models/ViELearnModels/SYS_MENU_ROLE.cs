using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViELearn.Models.ViELearnModels
{
    [TableName("SYS_MENU_ROLE")]
    [PrimaryKey("ID", AutoIncrement = true)]
    public class SYS_MENU_ROLE
    {
        public int ID { get; set; }
        public int ID_SYS_MENU { get; set; }
        public string ID_ROLE { get; set; }
        public string ACCESS_RIGHT { get; set; }
        [ResultColumn]
        public string TenDanhMuc { get; set; }
        [ResultColumn]
        public string Action { get; set; }
        [ResultColumn]
        public string Controller { get; set; }
        [ResultColumn]
        public string Class { get; set; }
        [ResultColumn]
        public string CLASS_MAIN { get; set; }
        [ResultColumn]
        public int CAPDO { get; set; }
        [ResultColumn]
        public int DANHMUCCHA { get; set; }
        [ResultColumn]
        public int DANHMUCCON { get; set; }
        public int Order { get; set; }
    }
}