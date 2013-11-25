using System.Diagnostics;
using System.ServiceProcess;

namespace TwitterFollower.Service
{
    partial class TwitterFollowerWindowsService : ServiceBase
    {
        public TwitterFollowerWindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Service Starting", EventLogEntryType.Information);
            Bootstrapper.Initialize();
            EventLog.WriteEntry("Service Started", EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Service Stopped", EventLogEntryType.Information);
        }
    }
}
