using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAppMVC.Models
{
    public class Grade
    {
        public int StudentId { get; set; }

        [Display(Name = "Grade")]
        public string CourceMark { get; set; }
        public int CourceNumber { get; set; }

    }
}