using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WebAppMVC.Models;
using NLog;

namespace WebAppMVC.Controllers
{   

    public class StudentController : Controller
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();
        // Mock data using the Student Model       
        IList<Student> studentList = new List<Student>{ // in real life applicatoin, fetch List of student objects from the database
                            new Student() { StudentId = 1, StudentName = "John", Age = 18 , StudentGender = Gender.Male} ,
                            new Student() { StudentId = 2, StudentName = "Steve",  Age = 21, StudentGender = Gender.Male } ,
                            new Student() { StudentId = 3, StudentName = "Bill",  Age = 25, StudentGender = Gender.Male } ,
                            new Student() { StudentId = 4, StudentName = "Ram" , Age = 20, StudentGender = Gender.Male } ,
                            new Student() { StudentId = 5, StudentName = "Ron" , Age = 31, StudentGender = Gender.Male } ,
                            new Student() { StudentId = 6, StudentName = "Amy" , Age = 18, StudentGender = Gender.Female } ,
                            new Student() { StudentId = 10, StudentName = "Rob" , Age = 19, StudentGender = Gender.Male }

                        };
        /// <summary>
        /// Index (default) action method of the Student controller. This action method returns linked list of Student model objects to the Index view 
        /// </summary>
        public ActionResult Index() // Index action method  // GET: Student
        {
            logger.Log(LogLevel.Info, "In Index() method of Student Controller");
            UpdateModelWithTempData(); //apply TempData values to the mock studentList database     
            return View(studentList); // send a linked list of Student data model to the Index view
        }


        /// <summary>
        /// Find action method of the Student controller. This action method returns  Student data model of the selected/default student to the Find view 
        /// </summary>
        /// <param name="Id">Student Id with the default = 0</param>
        [ActionName("Find")]
        public ActionResult GetById(int Id = 0)
        {
            // make sure that Id is in the list if student Ids, if not return the 1st Id in the list
            logger.Log(LogLevel.Info, "In Find() Action method of Student Controller");
            var studentId = (from student in studentList
                            where student.StudentId == Id
                            select student.StudentId).ToList();


            List<SelectListItem> StudentIDs = new List<SelectListItem>(); // create a <text,value> pair list of Student IDs
            studentList.ForEach(i => StudentIDs.Add(new SelectListItem // supply this list of student IDs for use in the matching dropdown list that is in the 'Find' View
            {               
                Text = i.StudentId.ToString(),
                Value = i.StudentId.ToString()
            }));

            ViewBag.List = StudentIDs; // use ViewBag to supply the list of student Ids to be used in the dropdown list in 'Find' View
            if (studentId.Count > 0) // make sure that we have a studentId to work with
            {
                // select student from the mock database 
                var std = studentList.Where(s => s.StudentId == Id).FirstOrDefault(); //select the student from the  mock database who's Id matches the variable in the URI sent to the controller 
                logger.Log(LogLevel.Info, "Finding Student with ID = " + std.StudentId.ToString());
                return View(std); // send the Student data model of the selected student to the Find view 

            }
            else { // could not find a valid student Id so will default to the 1st one in the list

                int minStudentId = (from student in studentList      // select the 1st student Id from the list of student Ids                          
                                 select student.StudentId).ToArray().Min();

                // get student from the mock database 
                var std = studentList.Where(s => s.StudentId == minStudentId).FirstOrDefault(); //select the student from the  mock database whose Id matches the smallest one
                logger.Log(LogLevel.Warn, "Could not find valid student Id. Will default to Id = " + std.StudentId.ToString());
                return View(std); // send the Student data model of the selected student to the Find view 
            }
            
        }

        /// <summary>
        /// Edit action method of the Student controller. This action method sends Student data model of the selected/default student to the Edit view 
        /// </summary>
        /// <param name="Id">Student Id</param>
        public ActionResult Edit(int Id=0)  
        {

            logger.Log(LogLevel.Info, "In Edit() Action method of Student Controller");
            // make sure that Id is in the list if student Ids, if not return the 1st Id in the list
            var studentId = (from student in studentList
                             where student.StudentId == Id
                             select student.StudentId).ToList();

            if (studentId.Count > 0) // make sure that we have a studentId to work with
            {
                UpdateModelWithTempData();   //    apply TempData values to the mock studentList database 
                var std = studentList.Where(s => s.StudentId == Id).FirstOrDefault(); //select the student from the  mock database who's Id matches the variable in the URI sent to the controller 
                logger.Log(LogLevel.Info, "Editing Student with ID = " + std.StudentId.ToString());
                return View(std); // send the Student data model of the selected student to the Edit view 

            }
            else // could not find a valid student Id so will default to the 1st one in the list
            { 
                int minStudentId = (from student in studentList      // select the 1st student Id from the list of student Ids                          
                                    select student.StudentId).ToArray().Min();

                UpdateModelWithTempData();   //    apply TempData values to the mock studentList database 
                var std = studentList.Where(s => s.StudentId == minStudentId).FirstOrDefault(); //select the student from the  mock database whose Id matches the smallest one
                logger.Log(LogLevel.Warn, "Could not find valid student Id. Will default to Id = " + std.StudentId.ToString());
                return View(std); // send the Student data model of the selected student to the Edit view 
            }           
        }


        /// <summary>
        /// Edit action method of the Student Controller. 
        /// 1.This action method recieves Student data model of an edited student from the 'Edit' view 
        /// 2.Then save the updated student data in TempData to be used in the Index view
        /// </summary>
        /// public ActionResult Edit([Bind(Exclude = "Age")]Student std) (a possible Edit method signature)
        /// <param name="std">Student model object holding the data of the edited student</param>
        [HttpPost] // the other Edit method must be "HttpGet" because this one is [HttpPost]         
        public ActionResult Edit(Student std) //another possible Edit method signature is  Edit(FormCollection values)
        {

            logger.Log(LogLevel.Info, "In [HttpPost] Edit() Action method of Student Controller");
            //Save the updated Student data in TempData (write code to update student record in the actual database)
            if (std is null) 
            {
                logger.Log(LogLevel.Error, "Student object is null" + std.StudentId.ToString());
                throw new ArgumentNullException(nameof(std)); // the std object is null
            }
            else // std object is not null
            {
                if (ModelState.IsValid)//  If ModelState is valid then update the student 
                {
                    logger.Log(LogLevel.Info, "ModelState is valid. Updating student info");
                    TempData["StudentId"] = std.StudentId;
                    TempData["StudentName"] = std.StudentName;
                    TempData["age"] = std.Age;
                    TempData["StudentGender"] = std.StudentGender;
                    logger.Log(LogLevel.Info, "Edited Student with ID = " + std.StudentId.ToString());
                    logger.Log(LogLevel.Info, "Added to TempData Student object " + std.StudentId.ToString());
                    return RedirectToAction("Index");
                }
                else
                {
                    logger.Log(LogLevel.Error, "ModelState is NOT valid. student info NOT updated");
                    return View(std); //if not then return Edit without changes
                }
            }        
        }

       
        public ActionResult Delete(int Id)
        {
            // delete student from the database whose id matches with specified id

            return RedirectToAction("Index");
        }

        /// <summary>
        /// This method applies the data saved to TempData in [HttpPost] Edit to the a linked list noode in studentList that mataches the student Id that was edited in Edit view 
        /// </summary>
        void UpdateModelWithTempData()
        {
            logger.Log(LogLevel.Info, "In UpdateModelWithTempData() method of Student Controller");
            string sName, sGender, sId, sAge;

            if (TempData.ContainsKey("StudentId") && TempData["StudentId"] != null) //if we have a student Id that is not null
            {
                sId = TempData["StudentId"].ToString(); // extract student Id from TempData
                if (TempData.ContainsKey("StudentName") && (TempData["StudentName"] != null)) //if we have a student name that is not null
                {
                    sName = TempData["StudentName"].ToString(); // extract student Name from TempData
                    studentList.Where(s => s.StudentId == Int32.Parse(sId)).First().StudentName = sName; //assign TempData studentName to the matching student object in  studentList
                }

                if (TempData.ContainsKey("Age") && (TempData["Age"] != null))
                {
                    sAge = TempData["Age"].ToString(); // extract student Age from TempData
                    studentList.Where(s => s.StudentId == Int32.Parse(sId)).First().Age = Int32.Parse(sAge); //assign TempData student age to the matching student object in  studentList
                }

                if (TempData.ContainsKey("StudentGender") && (TempData["StudentGender"] != null))
                {
                    Gender sGen;
                    sGender = TempData["StudentGender"].ToString(); // extract student gender from TempData
                    if (Enum.TryParse(sGender, true, out sGen)) // convert gender from string to Enum value for later assignemnet to student object, ignore case
                    {
                        logger.Log(LogLevel.Info, "Parsing Student gender = " + sGen.ToString());
                        studentList.Where(s => s.StudentId == Int32.Parse(sId)).First().StudentGender = sGen; //assign TempData student gender to the matching student object in  studentList
                    }
                    else
                    {
                        logger.Log(LogLevel.Warn, "Could not parse Student gender. Defaulting to Female");
                        studentList.Where(s => s.StudentId == Int32.Parse(sId)).First().StudentGender = Models.Gender.Female; //default to Female 

                    }
                }
                TempData.Keep(); // Keep TempData values in a 3rd consecutive request. More info at https://www.tutorialsteacher.com/mvc/tempdata-in-asp.net-mvc
            }
            else 
            {
                logger.Log(LogLevel.Warn, "TempData Not available" );
            }
            
        }

    }
}