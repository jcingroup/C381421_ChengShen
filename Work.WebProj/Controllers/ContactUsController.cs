using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotWeb.WebApp.Controllers
{
    public class ContactUsController : WebFrontController
    {
        // GET: ContactUs
        public ActionResult Index()
        {
            ViewBag.BodyClass = "ContactUs";
            return View("ContactUs"); 
        }
    }
}