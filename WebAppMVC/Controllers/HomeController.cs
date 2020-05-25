using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAppMVC.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// The Index action mathod of the Home Controller is the default entry point into this MVC application
        /// From here HTTP requests are routed to the various Controllers and Action methods 
        /// </summary>
        public ActionResult Index()
        {
            return View(); //Display the Index view of the Home Controller. 
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

       
        public ActionResult Contact()
        {
           ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}