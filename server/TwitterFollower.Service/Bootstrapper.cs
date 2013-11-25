using System;
using System.Linq;
using System.ServiceModel;
using Castle.Facilities.Logging;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using TwitterFollower.Service.Domain;

namespace TwitterFollower.Service
{
    internal class Bootstrapper
    {
        public static IWindsorContainer Container { get; private set; }
        public static void Initialize()
        {
            Container = new WindsorContainer();
            Container.AddFacility<WcfFacility>();
            Container.AddFacility<LoggingFacility>(f => f.UseNLog());

            var netNamedPipeBinding = new NetNamedPipeBinding
            {
                MaxBufferSize = 67108864,
                MaxReceivedMessageSize = 67108864,
                TransferMode = TransferMode.Streamed,
                ReceiveTimeout = new TimeSpan(0, 30, 0),
                SendTimeout = new TimeSpan(0, 30, 0)
            };

            Container.Register(
                Component.For<ExceptionInterceptor>(),
                Component.For<ITweetRepo>().ImplementedBy<TweetRepo>().LifestyleTransient(),
                Component.For<ITweetService>().ImplementedBy<TweetService>().LifestyleSingleton()
                         .AsWcfService(new DefaultServiceModel().AddEndpoints(WcfEndpoint.BoundTo(netNamedPipeBinding).At("net.pipe://localhost/TweetService"))
                         .PublishMetadata()));

        }
    }
}