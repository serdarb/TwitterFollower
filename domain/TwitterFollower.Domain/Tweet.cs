using System;
using MongoDB.Bson;

namespace TwitterFollower.Domain
{
    public class Tweet
    {
        public ObjectId Id{ get; set; }

        public string Who { get; set; }
        public string Image { get; set; }
        public string What { get; set; }
        public string When { get; set; }
        public string Where { get; set; }
    }
}