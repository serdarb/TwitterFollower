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
                Console.WriteLine("Service is starting!");
                Business.DoWork();
                Console.WriteLine("Service started!");
                Console.ReadLine();
            }
            else
            {
                ServiceBase.Run(new ServiceBase[] { new TwitterFollowerWindowsService() });
            }
        }
    }
}
