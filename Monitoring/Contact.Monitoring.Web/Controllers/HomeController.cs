using System.Web.Mvc;
using Contact.Monitoring.Services;

namespace Contact.Monitoring.Web.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Monitoring()
        {
            ViewBag.Message = "Monitoring page.";

            return View();
        }

        public ActionResult Overview()
        {
            ViewBag.Message = "Contact Service Overview.";
            Response.AddHeader("Refresh", "30");
            var overviewService = new OverviewService();
            var overviewModel = overviewService.GetOverview();
            return View(overviewModel);
        }

    }
}
