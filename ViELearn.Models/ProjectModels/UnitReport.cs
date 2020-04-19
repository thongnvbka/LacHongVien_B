using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("UnitReport")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public partial class UnitReport
    {
        public int Id { get; set; }
        public DateTime DayReport { get; set; }
        public int TotalPeople { get; set; }
        public int PeopleInMisson { get; set; }
        public int PeopleInStudy { get; set; }
        public int PeopleInVacation { get; set; }
        public int PeopleInSick { get; set; }
        public int PeopleInOtherReason { get; set; }
        public string ReasonNote { get; set; }
        public string Title { get; set; }
        public int UnitId { get; set; }
        public string GroupId { get; set; }
        public short Status { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string PublishedBy { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}
