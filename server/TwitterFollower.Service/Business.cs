using System;
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
        public static int Counter { get; set; }
        public static void DoWork()
        {
            try
            {
                var context = GetTwitterContext();
                var repo = new TweetRepo();
                var repoError = new ApiErrorRepo();

                MyFeed(context, repo, repoError);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message + "\n\n");
            }

        }
        private static void MyFeed(TwitterContext context, TweetRepo repo, ApiErrorRepo repoError)
        {
            var stream = context.UserStream.Where(x => x.Type == UserStreamType.User)
                .StreamingCallback(x =>
                {
                    try
                    {
                        if (x.Status == TwitterErrorStatus.RequestProcessingException)
                        {
                            var wex = x.Error as WebException;
                            if (wex != null
                                && wex.Status == WebExceptionStatus.ConnectFailure)
                            {
                                Console.WriteLine(wex.Message + " You might want to reconnect. " + DateTime.Now.ToLongDateString());
                                repoError.Add(new ApiError
                                {
                                    When = DateTime.Now,
                                    Message = wex.Message + " You might want to reconnect. "
                                });
                            }

                            Console.WriteLine(x.Error.ToString());
                            repoError.Add(new ApiError
                            {
                                When = DateTime.Now,
                                Message = x.Error.ToString()
                            });
                            return;
                        }

                        dynamic obj = JsonConvert.DeserializeObject(x.Content);
                        if (obj == null || obj.user == null) return;

                        string statusId = obj.id_str;
                        if (repo.AsQueryable().Any(y => y.StatusID == statusId)) return;

                        string text = obj.text;
                        string userName = obj.user.screen_name;
                        string userImgUrl = obj.user.profile_image_url_https;
                        var time = DateTime.ParseExact((string)obj.created_at, "ddd MMM dd HH:mm:ss zzz yyyy",
                            CultureInfo.InvariantCulture);
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
                    }
                    catch (Exception ex)
                    {
                        repoError.Add(new ApiError
                        {
                            When = DateTime.Now,
                            Message = ex.Message
                        });

                        if (ex.InnerException != null)
                        {
                            repoError.Add(new ApiError
                            {
                                When = DateTime.Now,
                                Message = ex.InnerException.Message
                            });
                        }
                    }
                })
                .SingleOrDefault();
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