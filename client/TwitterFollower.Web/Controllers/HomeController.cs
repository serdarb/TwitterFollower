using System.Collections.Generic;
using System.Web.Mvc;
using TwitterFollower.Domain;

namespace TwitterFollower.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var list = new List<Tweet>();



            return View(list);
        }
    }
}