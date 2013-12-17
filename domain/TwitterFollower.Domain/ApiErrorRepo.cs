using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace TwitterFollower.Domain
{
    public class ApiErrorRepo
    {
        private readonly MongoCollection<ApiError> _collection;

        public ApiErrorRepo()
        {
            var mongoCnnStr = ConfigurationManager.AppSettings["MongoCnnStr"] ?? "mongodb://localhost";
            var dbName = ConfigurationManager.AppSettings["MongoDBName"] ?? "TwitterFollower";
            var concern = new WriteConcern { Journal = true, W = 1 };

            var mongoDatabase = new MongoClient(mongoCnnStr).GetServer().GetDatabase(dbName);

            _collection = mongoDatabase.GetCollection<ApiError>("ApiError", concern);
        }

        public MongoCursor<ApiError> FindAll()
        {
            return _collection.FindAllAs<ApiError>();
        }

        public List<ApiError> FindLastTweets(int count)
        {
            return _collection.AsQueryable().OrderByDescending(x => x.Id).Skip(0).Take(count).ToList();
        }

        public IQueryable<ApiError> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public IQueryable<ApiError> AsOrderedQueryable()
        {
            return AsQueryable().OrderByDescending(x => x.Id);
        }

        public WriteConcernResult Add(ApiError entity)
        {
            return _collection.Insert(entity);
        }

        public void AddBulk(IEnumerable<ApiError> entities)
        {
            _collection.InsertBatch(entities);
        }

        public WriteConcernResult DeleteAll()
        {
            return _collection.RemoveAll();
        }
    }
}