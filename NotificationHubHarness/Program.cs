using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Notifications;

namespace NotificationHubHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var registrations = GetAllRegisteredDevicesAsync("Endpoint=sb://contactservice-demo.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=EZA5Q88Tt9XjdxCi8GgePjBc+9Ah+Xeg1Zol+f+ul4k=",
                "contactservice-demo-hub");

            int index = 0;
            foreach (var registration in registrations)
            {
                Console.WriteLine("{0}",++index);
                Console.WriteLine(registration.RegistrationId);
                if (registration is GcmRegistrationDescription)
                {
                    Console.WriteLine((registration as GcmRegistrationDescription).GcmRegistrationId);
                }
                if (registration is AppleRegistrationDescription)
                {
                    Console.WriteLine((registration as AppleRegistrationDescription).DeviceToken);
                }
                    foreach (var tag in registration.Tags)
                {
                    Console.WriteLine(tag);
                }
            }
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
            }

            return registrationDescriptionsList;
        }
    }
}
