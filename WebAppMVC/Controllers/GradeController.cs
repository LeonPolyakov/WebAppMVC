using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppMVC.Models;

namespace WebAppMVC.Controllers
{
    public class GradeController : Controller
    {

        IList<Grade> gradeList = new List<Grade>{
                            new Grade() { StudentId = 1, CourceMark = "A", CourceNumber = 1204 } ,
                            new Grade() { StudentId = 2, CourceMark = "B",  CourceNumber = 1204 } ,
                            new Grade() { StudentId = 3, CourceMark = "C",  CourceNumber = 1204 } ,
                            new Grade() { StudentId = 4, CourceMark = "D" , CourceNumber = 1204 } ,
                            new Grade() { StudentId = 5, CourceMark = "F" , CourceNumber = 1204 } ,
                            new Grade() { StudentId = 6, CourceMark = "I" , CourceNumber = 1204 } ,
                            new Grade() { StudentId = 10, CourceMark = "A" , CourceNumber = 1204 }
                        };


        // GET: Grade
        public ActionResult Index()
        {
            return View(gradeList);
        }
    }
}