using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViELearn.BackEnd.Models
{
    public class Avatar
    {
        [Display(Name = "Image URL")]
        public string PhotoURL { get; set; }
    }
}