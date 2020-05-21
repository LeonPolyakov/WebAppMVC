﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAppMVC.Models
{
    public class Student
    {

        [Display(Name = "Student Id")]
        public int StudentId { get; set; }

        [Display(Name = "Name")]
        public string StudentName { get; set; }
        public int Age { get; set; }
        
        [Display(Name = "Gender")]
        public Gender StudentGender { get; set; }

    }

    public enum Gender
    {
        Male,
        Female
    }
}