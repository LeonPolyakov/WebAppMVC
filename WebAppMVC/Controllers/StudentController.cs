using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WebAppMVC.Models;

namespace WebAppMVC.Controllers
{
    public class StudentController : Controller
    {

        // return View(); // Index view
        IList<Student> studentList = new List<Student>{
                            new Student() { StudentId = 1, StudentName = "John", Age = 18 , StudentGender = Gender.Male} ,
                            new Student() { StudentId = 2, StudentName = "Steve",  Age = 21, StudentGender = Gender.Male } ,
                            new Student() { StudentId = 3, StudentName = "Bill",  Age = 25, StudentGender = Gender.Male } ,
                            new Student() { StudentId = 4, StudentName = "Ram" , Age = 20, StudentGender = Gender.Male } ,
                            new Student() { StudentId = 5, StudentName = "Ron" , Age = 31, StudentGender = Gender.Male } ,
                            new Student() { StudentId = 6, StudentName = "Amy" , Age = 18, StudentGender = Gender.Female } ,
                            new Student() { StudentId = 10, StudentName = "Rob" , Age = 19, StudentGender = Gender.Male }
                             
                        };

        // GET: Student
        public ActionResult Index() // Index action method
        {

            return View(studentList); // Index view
        }

        [ActionName("Find")]
        public ActionResult GetById(int Id = 0)
        {
            // make sure that Id is in the list if student Ids, if not return the 1st Id in the list

            var studentId = (from student in studentList
                            where student.StudentId == Id
                            select student.StudentId).ToList();


            List<SelectListItem> StudentIDs = new List<SelectListItem>(); // create a list of Student IDs
            studentList.ForEach(i => StudentIDs.Add(new SelectListItem // use the list of student IDs in 
            {
                Value = i.StudentId.ToString(),
                Text = i.StudentId.ToString()
            }));

            ViewBag.List = StudentIDs;
            if (studentId.Count > 0)
            {
                // get student from the database 
                var std = studentList.Where(s => s.StudentId == Id).FirstOrDefault();
                return View(std);

            }
            else {

                int minStudentId = (from student in studentList                                
                                 select student.StudentId).ToArray().Min();

                // get student from the database 
                var std = studentList.Where(s => s.StudentId == minStudentId).FirstOrDefault();
                return View(std);

            }
            
        }
        
        public ActionResult Edit(int Id)  //This is  Edit action method that is paired with the 'Edit' view. This method sends the std model to the Edit view
        {
            //Get the student from studentList sample collection for demo purpose.
            //Get the student from the database in the real application
            var std = studentList.Where(s => s.StudentId == Id).FirstOrDefault();

            return View(std);
        }

        [HttpPost] // the other Edit mthod must be "HttpGet" because this one is HttpPost. 
        public ActionResult Edit([Bind(Exclude = "Age")]Student std)
        {
            //write code to update student 
            var Id = std.StudentId;
            var name = std.StudentName;
            var age = std.Age;
            return RedirectToAction("Index");
        }


        //[HttpPost]
        //public ActionResult Edit(FormCollection values)
        //{
        //    //write code to update student 
        //    var name = values["StudentName"];
        //    var age = values["Age"];
        //    return RedirectToAction("Index");
        //}


        public ActionResult Delete(int Id)
        {
            // delete student from the database whose id matches with specified id

            return RedirectToAction("Index");
        }

        public ActionResult Details(int Id)
        {
            // delete student from the database whose id matches with specified id

            return RedirectToAction("Index");
        }

    }
}