using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TeamMVCProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Players()
        {
            ViewBag.Title = "Player";
            ViewBag.Message = "Your players page.";

            return View();
        }

        public ActionResult Teams()
        {
            ViewBag.Title = "Teams";
            ViewBag.Message = "Your teams page.";

            return View();
        }
    }
}