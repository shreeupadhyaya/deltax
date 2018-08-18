using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeltaXMovies.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Movie Database";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Reach me";

            return View();
        }

       
    }
}