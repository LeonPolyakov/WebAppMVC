using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAppMVC.Models
{
    [Serializable]
    public class Student
    {

       
        [Display(Name = "Student Id")]
        public int StudentId { get; set; }
        
        [Required][StringLength(30)]
        [Display(Name = "First Name")]
        public string StudentFName { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Last Name")]
        public string StudentLname { get; set; }

        [Range(13, 75)]
        public int StudentAge { get; set; }
        
        [Display(Name = "Gender")]
        public Gender StudentGender { get; set; }
       


    }

    public enum Gender
    {
        Male,
        Female
    }

}