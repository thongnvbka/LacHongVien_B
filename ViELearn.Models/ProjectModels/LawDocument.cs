using System;
using PetaPoco;

namespace ViELearn.Models.ProjectModels
{
    [TableName("LawDocument")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public partial class LawDocument
    {
        public int Id { get; set; }
        public int UnitId { get; set; }
        public int DocNo { get; set; }
        public string DocSymbol { get; set; }
        public DateTime? DocTimeStart { get; set; }
        public DateTime? DocTimeEnd { get; set; }
        public string DocSign { get; set; }
        public string DocSummary { get; set; }
        public int OrganizationId { get; set; }
        public int FieldId { get; set; }
        public int TypeId { get; set; }
        public string DocContent { get; set; }
        public byte Status { get; set; }
    }
}