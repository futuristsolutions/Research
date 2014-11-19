using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Notifications;

namespace NotificationHubHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            const string outputFileName = "tesco-contactservice-registration-export.csv";

            var registrations = GetAllRegisteredDevicesAsync("Endpoint=sb://tesco-contactservice-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=3t6/c2+zvZBBjLvUwAd9zxslD19rw4esbySZMuQTVL0=",
                "tesco-contactservice");

            //var registrations = GetAllRegisteredDevicesAsync("Endpoint=sb://contactservice-demo.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=EZA5Q88Tt9XjdxCi8GgePjBc+9Ah+Xeg1Zol+f+ul4k=",
            //      "contactservice-demo-hub");

            Console.WriteLine("Writing output to {0}", outputFileName);
            int index = 0;
            var writer  = new StreamWriter(outputFileName);
            writer.WriteLine(string.Format("Id,RegistrationId,Type,Token,Tag"));
            foreach (var registration in registrations)
            {
                ++index;
                string type = string.Empty;
                string token = string.Empty;
                string tag = string.Empty;
                if (registration is GcmRegistrationDescription)
                {
                    type = "GCM";
                    token = (registration as GcmRegistrationDescription).GcmRegistrationId;
                }
                if (registration is AppleRegistrationDescription)
                {
                    type = "APNS";
                    token = (registration as AppleRegistrationDescription).DeviceToken;
                }
                if (registration.Tags != null)
                {
                    tag = string.Join(" ", registration.Tags.ToArray());
                }
                writer.WriteLine("{0},{1},{2},{3},{4}", index, registration.RegistrationId, type, token, tag);
            }
            writer.Close();
            Console.WriteLine("Writing output to {0} completed", outputFileName);
            stopwatch.Stop();
            Console.WriteLine("Total time taken {0}",TimeSpan.FromSeconds(stopwatch.ElapsedMilliseconds/60.0).ToString("c"));
        }

        internal static List<RegistrationDescription> GetAllRegisteredDevicesAsync(string connectionString,
                string notificationPath)
        {
            var hub = NotificationHubClient.CreateClientFromConnectionString(connectionString, notificationPath);

            var allRegistrations = hub.GetAllRegistrationsAsync(0).Result;
            var continuationToken = allRegistrations.ContinuationToken;
            var registrationDescriptionsList = new List<RegistrationDescription>(allRegistrations);
            while (!string.IsNullOrWhiteSpace(continuationToken))
            {
                var otherRegistrations = hub.GetAllRegistrationsAsync(continuationToken, 0).Result;
                registrationDescriptionsList.AddRange(otherRegistrations);
                continuationToken = otherRegistrations.ContinuationToken;
                Console.WriteLine("Read {0} registrations", registrationDescriptionsList.Count);
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }

            return registrationDescriptionsList;
        }
    }
}
