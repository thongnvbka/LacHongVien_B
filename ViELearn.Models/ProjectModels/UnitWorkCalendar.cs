using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("UnitWorkCalendar")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public partial class UnitWorkCalendar
    {
        public int Id { get; set; }
        public int UnitId { get; set; }
        public DateTime DayWorkCalendar { get; set; }
        public short Status { get; set; }
        public string MorningNote { get; set; }
        public string AfternoonNote { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string PublishedBy { get; set; }
        public DateTime? PublishedAt { get; set; }

        [ResultColumn]
        public string CreatedByName { get; set; }
        [ResultColumn]
        public string PublishedByName { get; set; }
    }
}
