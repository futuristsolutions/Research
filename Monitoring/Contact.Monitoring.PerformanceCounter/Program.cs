using System;
using Tesco.Services.Contact.Mechanisms.Configuration;
using Tesco.Services.Contact.Mechanisms.Constraints;
using Tesco.Services.Contact.Mechanisms.DomainSpecific;
using Tesco.Services.Contact.Mechanisms.IO;
using Tesco.Services.Contact.Mechanisms.Logging;
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
            Log.ApplicationLog = CreateGlobalLoggerForService(hostSettings.InstanceName);
            return new Service(hostSettings.InstanceName);
        }

        private ILogger CreateGlobalLoggerForService(string instanceName)
        {
            Constraint.MustNotBeNull(Configuration, "Configuration");

            var logFilePath = Configuration.GetStringValue(GlobalSettings.Keys.LogFilePath);
            // ReSharper disable once AssignNullToNotNullAttribute
            logFilePath = string.Format(logFilePath, string.Format(".{0}", instanceName));
            logFilePath = PathHelp.RootPathIfRelative(logFilePath, AppDomain.CurrentDomain.BaseDirectory);
            return Log4NetLogger.GetLogger(ExeAppSettings.FilePath,
                                           Appenders.Trace(),
                                           Appenders.ColouredConsole(),
                                           Appenders.EventLog(EventLogSource),
                                           Appenders.RollingFile(logFilePath, true));
        }
    }
}
