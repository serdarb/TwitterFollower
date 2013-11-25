using System;
using System.ServiceProcess;

namespace TwitterFollower.Service
{
    class Program
    {
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                Bootstrapper.Initialize();
                Console.WriteLine("Service is ready!");
                Console.ReadLine();
            }
            else
            {
                ServiceBase.Run(new ServiceBase[] { new TwitterFollowerWindowsService() });
            }
        }
    }
}
