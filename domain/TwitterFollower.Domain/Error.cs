using System;
using MongoDB.Bson;

namespace TwitterFollower.Domain
{
    public class ApiError
    {
        public ObjectId Id { get; set; }
        public string Message { get; set; }
        public DateTime When { get; set; }
    }
}
