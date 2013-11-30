using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using LinqToTwitter;
using MongoDB.Bson;
using Newtonsoft.Json;
using TwitterFollower.Domain;

namespace TwitterFollower.Service
{
    internal class Business
    {
        public static void DoWork()
        {
            var repo = new TweetRepo();
            var streamItems =
                GetTwitterContext().UserStream.Where(x => x.Type == UserStreamType.User && x.Follow == "hserdarb").StreamingCallback(
                    x =>
                    {
                        if (x.Status == TwitterErrorStatus.Success)
                        {
                            try
                            {
                                dynamic obj = JsonConvert.DeserializeObject(x.Content);
                                string text = obj.text;
                                string userName = obj.user.screen_name;
                                string userImgUrl = obj.user.profile_image_url_https;
                                string statusId = obj.id_str;
                                var time = DateTime.ParseExact((string)obj.created_at, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture);
                                var time2 = time.ToString("dd MMMM dddd - HH:mm", CultureInfo.InvariantCulture);

                                repo.Add(new Tweet
                                {
                                    Link = string.Format("https://twitter.com/{0}/status/{1}", userName, statusId),
                                    StatusID = statusId,
                                    What = text,
                                    Image = userImgUrl,
                                    Who = userName,
                                    When = time2
                                });
                            }
                            catch (Exception ex)
                            {
                                
                            }
                        }
                    }).SingleOrDefault();

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