using System.Web.Mvc;

namespace TwitterFollower.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}