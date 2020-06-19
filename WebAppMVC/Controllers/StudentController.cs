using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebAppMVC.Models;
using NLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http.Headers;

namespace WebAppMVC.Controllers
{

    public class StudentController : Controller
    {
               
      //  static HttpClient client = new HttpClient();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static List<Student> studentList = new List<Student>();
        bool getStudentData = retrieveStudentInfo();
        /// <summary>
        /// Index (default) action method of the Student controller. This action method returns linked list of Student model objects to the Index view 
        /// </summary>
        public ActionResult Index() // Index action method  // GET: Student
        {           
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
                var std = studentList.Where(s => s.StudentId == Id).FirstOrDefault(); //select the student from the  database who's Id matches the variable in the URI sent to the controller 
                logger.Log(LogLevel.Info, "Editing Student with ID = " + std.StudentId.ToString());
                return View(std); // send the Student data model of the selected student to the Edit view 

            }
            else // could not find a valid student Id so will default to the 1st one in the list
            { 
                //int minStudentId = (from student in studentList      // select the 1st student Id from the list of student Ids                          
                //                    select student.StudentId).ToArray().Min();
               int minStudentId = (from student in studentList // select the 1st student Id from the list of student Ids   
                                        orderby student.StudentId descending
                                        select student).First().StudentId;


                UpdateModelWithTempData();   //    apply TempData values to the mock studentList database 
                var std = studentList.Where(s => s.StudentId == minStudentId).FirstOrDefault(); //select the student from the   database whose Id matches the smallest one
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
                logger.Log(LogLevel.Info, "Updating student info");
                if (updateStudentInfo(std) == true)
                {
                    TempData["StudentId"] = std.StudentId;
                    TempData["StudentFName"] = std.StudentFName;
                    TempData["StudentLname"] = std.StudentLname;
                    TempData["StudentAge"] = std.StudentAge;
                    TempData["StudentGender"] = std.StudentGender;
                    logger.Log(LogLevel.Info, "Edited Student with ID = " + std.StudentId.ToString());
                    logger.Log(LogLevel.Info, "Added to TempData Student object " + std.StudentId.ToString());
                    logger.Log(LogLevel.Info, "Successfully saved student info" + std.StudentId.ToString());
                }
                else
                {
                    logger.Log(LogLevel.Error, "Failed to save student info" + std.StudentId.ToString());
                }
                
                return RedirectToAction("Index");              
            }        
        }

        public ActionResult Create()
        {
            return View();
            
        }

        [HttpPost]
        public ActionResult Create(Student std)
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
                logger.Log(LogLevel.Info, "Updating student info");
                Student new_student = new Student();
                int highest_student_ID = 0;
                if (studentList.Count > 0)
                {
                    highest_student_ID = studentList.OrderByDescending(s => s.StudentId).First().StudentId; // assign this new student the next highest ID
                    new_student.StudentId = highest_student_ID + 1;
                }
                else // the studentList linked list is empty
                {
                    new_student.StudentId = 0;
                }
                 new_student.StudentFName = std.StudentFName;
                 new_student.StudentLname = std.StudentLname;
                 new_student.StudentAge = std.StudentAge;
                 new_student.StudentGender = std.StudentGender;
                

                if (addNewStudent(new_student) == true)
                {                   
                    studentList.Add(new_student);
                }
                else
                {
                    logger.Log(LogLevel.Error, "Failed to save student info" + std.StudentId.ToString());
                }

                return RedirectToAction("Index");
            }
           
        }

        public ActionResult Delete(int Id)
        {           
            if (deleteStudent(Id) == true) // delete student from the database whose id matches with specified id
            {
                var studentToRemove = studentList.Single(r => r.StudentId == Id);
                studentList.Remove(studentToRemove); // remove Student from studentList linked list 
                TempData.Clear(); // Remove All TempData because we have removed Student info from studentList
                logger.Log(LogLevel.Info, "Successfully deleted student record for student" + Id.ToString());
            }
            else
                logger.Log(LogLevel.Error, "Failed to delete student record for student" + Id.ToString());
            
            return RedirectToAction("Index");
        }

        /// <summary>
        /// This method applies the data saved to TempData in [HttpPost] Edit to the a linked list noode in studentList that mataches the student Id that was edited in Edit view 
        /// </summary>
        void UpdateModelWithTempData()
        {
            logger.Log(LogLevel.Info, "In UpdateModelWithTempData() method of Student Controller");
            string sFname, sLname, sGender, sId, sAge;

            if (TempData.ContainsKey("StudentId") && TempData["StudentId"] != null) //if we have a student Id that is not null
            {
                sId = TempData["StudentId"].ToString(); // extract student Id from TempData
                if (TempData.ContainsKey("StudentFName") && (TempData["StudentFName"] != null)) //if we have a student first name that is not null
                {
                    sFname = TempData["StudentFName"].ToString(); // extract student Name from TempData
                    studentList.Where(s => s.StudentId == Int32.Parse(sId)).First().StudentFName = sFname; //assign TempData student first Name to the matching student object in  studentList
                }

                if (TempData.ContainsKey("StudentLname") && (TempData["StudentLname"] != null)) //if we have a student last name that is not null
                {
                    sLname = TempData["StudentLname"].ToString(); // extract student Name from TempData
                    studentList.Where(s => s.StudentId == Int32.Parse(sId)).First().StudentLname = sLname; //assign TempData student last Name to the matching student object in  studentList
                }

                if (TempData.ContainsKey("StudentAge") && (TempData["StudentAge"] != null))
                {
                    sAge = TempData["StudentAge"].ToString(); // extract student Age from TempData
                    studentList.Where(s => s.StudentId == Int32.Parse(sId)).First().StudentAge = Int32.Parse(sAge); //assign TempData student age to the matching student object in  studentList
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

        private static bool updateStudentInfo(Student std)
        {
            bool success = false;
            logger.Log(LogLevel.Info, "In saveStudentInfo method" + std.StudentId.ToString());
            if (std is null) {                
                logger.Log(LogLevel.Error, "Student object is null" + std.StudentId.ToString());
                throw new ArgumentNullException(nameof(std)); // the std object is null
            }
            else
            {
                try
                {
                    logger.Log(LogLevel.Info, "Updating Student info to a DynamoDB table" + std.StudentId.ToString());
                    var jsonStudent = new // Create my object so that I can serialize Student Info 
                    {
                        std.StudentFName,
                        std.StudentLname,
                        StudentAge = std.StudentAge.ToString(),
                        StudentGender = std.StudentGender.ToString()
                    };

                    Uri u = new Uri("https://zz4k4joszj.execute-api.us-west-2.amazonaws.com/prod/students/" + std.StudentId.ToString());
                    var payload = JsonConvert.SerializeObject(jsonStudent);

                    HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
                    var t = Task.Run(() => PostURI(u, c));
                    t.Wait();
                    var result = t.Result;
                    if (result == "OK")
                    {
                        success = true;
                    }
                    
                }
                catch(Exception ex) {
                    logger.Log(LogLevel.Error, "Error saving student info to DynamoDB" + ex.ToString());

                }
            }
            return success;
        }
        private static bool deleteStudent(int Id)
        {
            bool success = false;
            logger.Log(LogLevel.Info, "In deleteStudentInfo method" + Id.ToString());
            if (Id < 0)
            {
                logger.Log(LogLevel.Error, "Student Id is invalid" + Id.ToString());
                throw new ArgumentNullException(Id.ToString()); // the std object is null
            }
            else
            {
                try
                {
                    logger.Log(LogLevel.Info, "Deleteing Student info in DynamoDB table" + Id.ToString());
                    Uri u = new Uri("https://zz4k4joszj.execute-api.us-west-2.amazonaws.com/prod/students/" + Id.ToString());
                    var t = Task.Run(() => DeleteURI(u));
                    t.Wait();
                    var result = t.Result;
                    if (result == "OK")
                    {
                        success = true;
                    }                    
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, "Error deleteing student on DynamoDB" + ex.ToString());

                }
            }
            return success;

        }

        private static bool addNewStudent(Student std)
        {
            bool success = false;
            logger.Log(LogLevel.Info, "In addNewStudent method" + std.StudentId.ToString());
            if (std is null)
            {
                logger.Log(LogLevel.Error, "Student object is null" + std.StudentId.ToString());
                throw new ArgumentNullException(nameof(std)); // the std object is null
            }
            else
            {
                try
                {
                    logger.Log(LogLevel.Info, "Add Student to a DynamoDB table" + std.StudentId.ToString());
                    Uri u = new Uri("https://zz4k4joszj.execute-api.us-west-2.amazonaws.com/prod/students");
                    var jsonStudentToAdd = new // Create my object so that I can serialize Student Info 
                    {
                        StudentID = std.StudentId,
                        std.StudentFName,
                        std.StudentLname,
                        StudentAge = std.StudentAge.ToString(),
                        StudentGender = std.StudentGender.ToString()                         
                    };
                    var payload = JsonConvert.SerializeObject(jsonStudentToAdd);

                    HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
                    var t = Task.Run(() => PostURI(u, c));
                    t.Wait();
                    var result = t.Result;
                    if (result == "Created")
                    {
                        success = true;
                    }

                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, "Error adding student info to DynamoDB" + ex.ToString());

                }
            }
            return success;

        }


        static async Task<string> DeleteURI(Uri u)
        {
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", "3RM7vABmXc3dyKCfjvR6034BRi3M4TZl5zOSqkEE");// thus API endpoint is protected by this Api Key
                HttpResponseMessage result = await client.DeleteAsync(u);
                if (result.IsSuccessStatusCode)
                {
                    response = result.StatusCode.ToString();
                }
            }
            return response;
        }



        static async Task<string> PostURI(Uri u, HttpContent c)
        {
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", "3RM7vABmXc3dyKCfjvR6034BRi3M4TZl5zOSqkEE");
                HttpResponseMessage result = await client.PostAsync(u, c);
                if (result.IsSuccessStatusCode)
                {
                    response = result.StatusCode.ToString();
                }
            }
            return response;
        }

       
        private static bool retrieveStudentInfo() 
        {
            bool succeeded = false;
            logger.Log(LogLevel.Info, "In retrieveStudentInfo()  method of Student Controller");
            if (studentList.Count < 1)
            {
                string url = string.Format("https://zz4k4joszj.execute-api.us-west-2.amazonaws.com/prod/students");
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("x-api-key", "3RM7vABmXc3dyKCfjvR6034BRi3M4TZl5zOSqkEE");
                try
                {
                    var jsonString = client.GetStringAsync(url).Result; // retrieve the contents of the Dynamo database Students table as a json string
                    JObject o = JObject.Parse(jsonString); // covert json string to object
                    JArray ItemsArray = (JArray)o["Items"]; // parse out the Items array that holds the array of Students

                    foreach (JObject studentArrayItem in ItemsArray.Children<JObject>()) // for each Student in the Items array
                    {
                        Student studentObj = new Student(); // create a new Student object 
                        foreach (JProperty p in studentArrayItem.Properties()) // for each Student retrieve Student info and hydrate the Student object
                        {
                            var name = p.Name.ToString();
                            var value = p.Value.ToString();
                            switch (name)
                            {
                                case "StudentID":
                                    studentObj.StudentId = Int32.Parse(value);
                                    break;
                                case "StudentFName":
                                    studentObj.StudentFName = value;
                                    break;
                                case "StudentLname":
                                    studentObj.StudentLname = value;
                                    break;
                                case "StudentAge":
                                    studentObj.StudentAge = Int32.Parse(value);
                                    break;
                                case "StudentGender":
                                    studentObj.StudentGender = (Gender)Enum.Parse(typeof(Gender), value);
                                    break;
                                default:
                                    break;
                            }
                        }
                        studentList.Add(studentObj); // add the Student object to the studentList linked list
                    }
                    succeeded = true;
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, "Unable to retrieve or parse Student info Json from AWS DynamoDB because of " + ex.ToString());
                }
            }
            else {
                succeeded = true;
            }

            return succeeded;
        }
       
    }
       
}