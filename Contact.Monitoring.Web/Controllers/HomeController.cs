using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Contact.Monitoring.Web.Models;
using Contact.Monitoring.Web.ViewModel;

namespace Contact.Monitoring.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Monitoring()
        {
            ViewBag.Message = "Monitoring page.";

            return View();
        }

        public ActionResult GetSystemUpTime()
        {
            var context = new MonitoringContext();
            var result = context.SystemUpTimes.Select(s => new SystemUpTimeViewModel
            {
                Service = s.Service,
                LastBootUpTime = s.LastBootUpTime,
                MachineName = s.MachineName
            }).ToList();
            return Json(result,JsonRequestBehavior.AllowGet);
        }
    }
}
