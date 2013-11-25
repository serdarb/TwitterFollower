using System.ServiceModel;

namespace TwitterFollower.Service.Domain
{
    [ServiceContract]
    public interface ITweetService
    {
        [OperationContract]
        string SaveTweet(string who, string what, string when, string where, string image);
    }
}