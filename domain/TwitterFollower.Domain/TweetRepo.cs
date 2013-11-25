using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace TwitterFollower.Domain
{
    public class TweetRepo
    {
        private readonly MongoCollection<Tweet> _collection;

        public TweetRepo()
        {
            var mongoCnnStr = ConfigurationManager.AppSettings["MongoCnnStr"] ?? "mongodb://localhost";
            var dbName = ConfigurationManager.AppSettings["MongoDBName"] ?? "TwitterFollower";
            var concern = new WriteConcern { Journal = true, W = 1 };

            var mongoDatabase = new MongoClient(mongoCnnStr).GetServer().GetDatabase(dbName);

            _collection = mongoDatabase.GetCollection<Tweet>("Tweet", concern);
        }

        public MongoCursor<Tweet> FindAll()
        {
            return _collection.FindAllAs<Tweet>();
        }

        public List<Tweet> FindLastTweets(int count)
        {
            return _collection.AsQueryable().OrderByDescending(x => x.Id).Skip(0).Take(count).ToList();
        }

        public IQueryable<Tweet> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public IQueryable<Tweet> AsOrderedQueryable()
        {
            return AsQueryable().OrderByDescending(x => x.Id);
        }

        public WriteConcernResult Add(Tweet entity)
        {
            return _collection.Insert(entity);
        }

        public void AddBulk(IEnumerable<Tweet> entities)
        {
            _collection.InsertBatch(entities);
        }

        public WriteConcernResult DeleteAll()
        {
            return _collection.RemoveAll();
        }
    }
}
