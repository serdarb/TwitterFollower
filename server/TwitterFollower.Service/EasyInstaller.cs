using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace TwitterFollower.Service
{
    [RunInstaller(true)]
    public partial class EasyInstaller : Installer
    {
        public EasyInstaller()
        {
            InitializeComponent();

            var serviceProcess = new ServiceProcessInstaller { Account = ServiceAccount.NetworkService };
            var serviceInstaller = new ServiceInstaller
            {
                ServiceName = "TwitterFollowerService",
                DisplayName = "Twitter Follower Service",
                Description = "Twitter Follower Windows Service ...",
                StartType = ServiceStartMode.Automatic
            };
            Installers.Add(serviceProcess);
            Installers.Add(serviceInstaller);
        }
    }
}
