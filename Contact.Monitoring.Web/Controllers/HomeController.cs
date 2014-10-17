using System.Web.Mvc;
using Contact.Monitoring.Services;

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

        public ActionResult Overview()
        {
            ViewBag.Message = "Contact Service Overview.";
            var overviewService = new OverviewService();
            var overviewModel = overviewService.GetOverview();
            return View(overviewModel);
        }

    }
}
