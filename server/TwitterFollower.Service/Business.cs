using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using LinqToTwitter;
using Newtonsoft.Json;
using TwitterFollower.Domain;

namespace TwitterFollower.Service
{
    internal class Business
    {
        public static void DoWork()
        {
            var context = GetTwitterContext();

            var repo = new TweetRepo();
            GetUsersLastTweets(context, repo, new List<string> { "hserdarb", "cmylmz", "DemetAkalin", "isilrecber", "esraceydaersoy", "armanayse", "Niltakipte", "eceerken", "Serdarortacs", "ahmethc" });

            var stream = context.UserStream.Where(x => x.Type == UserStreamType.User)
                .StreamingCallback(x =>
                {
                    if (x.Status == TwitterErrorStatus.RequestProcessingException)
                    {
                        var wex = x.Error as WebException;
                        if (wex != null
                            && wex.Status == WebExceptionStatus.ConnectFailure)
                        {
                            Console.WriteLine(wex.Message + " You might want to reconnect. " + DateTime.Now.ToLongDateString());
                        }

                        Console.WriteLine(x.Error.ToString());
                        return;
                    }

                    dynamic obj = JsonConvert.DeserializeObject(x.Content);
                    if (obj == null || obj.user == null) return;

                    string statusId = obj.id_str;
                    if (repo.AsQueryable().Any(y => y.StatusID == statusId)) return;

                    string text = obj.text;
                    string userName = obj.user.screen_name;
                    string userImgUrl = obj.user.profile_image_url_https;
                    var time = DateTime.ParseExact((string)obj.created_at, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture);
                    var time2 = time.ToString("dd MMMM dddd - HH:mm", new CultureInfo("tr-TR"));

                    repo.Add(new Tweet
                    {
                        Link = string.Format("https://twitter.com/{0}/status/{1}", userName, statusId),
                        StatusID = statusId,
                        Who = userName,
                        What = text,
                        Image = userImgUrl,
                        When = time2,
                        Where = string.Empty
                    });

                    Console.WriteLine(text + "\n");
                })
                .SingleOrDefault();
        }

        private static void GetUsersLastTweets(TwitterContext context, TweetRepo repo, IEnumerable<string> usernames)
        {
            foreach (var username in usernames)
            {
                var tweets = context.Status.Where(x => x.Type == StatusType.User && x.ScreenName == username)
                                           .ToList();

                foreach (var tweet in tweets)
                {
                    SaveTweet(repo, tweet);
                }
            }
        }

        private static void SaveTweet(TweetRepo repo, Status tweet)
        {
            if (repo.AsQueryable().Any(x => x.StatusID == tweet.StatusID)) return;

            var where = string.Empty;
            if (tweet.Coordinates.Latitude > double.MinValue
                && tweet.Coordinates.Longitude > double.MinValue)
            {
                where = string.Format("{0},{1}", tweet.Coordinates.Latitude, tweet.Coordinates.Longitude);
            }

            repo.Add(new Tweet
            {
                Link = string.Format("https://twitter.com/{0}/status/{1}", tweet.ScreenName, tweet.StatusID),
                StatusID = tweet.StatusID,
                Who = tweet.ScreenName,
                What = tweet.Text,
                Image = tweet.User.ProfileImageUrlHttps,
                When = tweet.CreatedAt.ToString("dd MMMM dddd - HH:mm", new CultureInfo("tr-TR")),
                Where = where
            });
        }

        private static TwitterContext GetTwitterContext()
        {
            var auth = new SingleUserAuthorizer
            {
                Credentials =
                    new SingleUserInMemoryCredentials
                    {
                        ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                        ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"],
                        TwitterAccessToken = ConfigurationManager.AppSettings["twitterTwitterAccessToken"],
                        TwitterAccessTokenSecret = ConfigurationManager.AppSettings["twitterTwitterAccessTokenSecret"]
                    }
            };
            return new TwitterContext(auth);
        }
    }
}