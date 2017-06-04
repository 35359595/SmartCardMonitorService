using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace SCBlocker
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(serviceConfig =>
            {
                serviceConfig.Service<SCManager>(serviceInstance =>
                {
                    serviceInstance.ConstructUsing(() => new SCManager());
                    serviceInstance.WhenStarted(s => s.StartMonitoring());
                    serviceInstance.WhenStopped(s => s.StopMonitoring());
                });
                serviceConfig.SetServiceName("scRemoveLock");
                serviceConfig.StartManually();
            });
        }
    }
}
