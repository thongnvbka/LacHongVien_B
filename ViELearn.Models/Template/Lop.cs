//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ViELearn.Models.Template
{
    using System;
    using System.Collections.Generic;
    
    public partial class LopNew
    {
        public int ID_lop { get; set; }
        public int ID_truong { get; set; }
        public string Nam_hoc { get; set; }
        public string Ma_lop { get; set; }
        public string Ten_lop { get; set; }
        public int ID_khoi { get; set; }
        public Nullable<int> ID_gv_cn { get; set; }
        public Nullable<int> ID_nguoi_dung { get; set; }
        public Nullable<System.DateTime> Ngay { get; set; }
        public Nullable<System.DateTime> Updated_at { get; set; }
    }
}
