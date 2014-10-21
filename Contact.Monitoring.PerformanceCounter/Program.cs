using System;
using Tesco.Services.Contact.Mechanisms.Process;
using Topshelf.Runtime;

namespace Contact.Monitoring.PerformanceMonitor
{
    class Program : ServiceApplication
    {
        public static int Main(string[] args)
        {
            var program = new Program();

            return program.Bootstrap(args);
        }

        protected override IServiceBuilder CreateServiceBuilder()
        {
            throw new NotSupportedException();
        }

        protected override ServiceIdentity Identity
        {
            get { return new ServiceIdentity("Tesco Contact Service - PerformanceMonitor", "Tesco-Contact-Service-PerformanceMonitor"); }
        }

        protected override IService OnConstruction(HostSettings hostSettings)
        {
            return new Service(hostSettings.InstanceName);
        }
    }
}
