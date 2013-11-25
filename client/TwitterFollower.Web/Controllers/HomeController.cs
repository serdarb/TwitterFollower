using System;
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
            try
            {
                var repo = new TweetRepo();
                list = repo.FindLastTweets(100);
            }
            catch (Exception ex)
            {

            }

            return View(list);
        }
    }
}