using System;
using System.Fabric;
using System.Fabric.Health;
using System.Threading;

namespace WarmupWebApi
{
    internal static class BootstrappingControl
    {
        private static Uri serviceUri = new Uri("fabric:/ServiceWarmup/WarmupWebApi");
        private static FabricClient client = new FabricClient();

        private static void RegisterHealthEvent(string property, HealthState state, TimeSpan ttl)
        {

            HealthInformation information = new HealthInformation("X.Warmup", property, state)
            {
                Description = "Concerning service startup",
                RemoveWhenExpired = true,
                TimeToLive = ttl
            };
            HealthReport report = new ServiceHealthReport(serviceUri, information);
            client.HealthManager.ReportHealth(report);
        }

        internal static void StartApp()
        {
            // we are starting, so report the service in error state
            RegisterHealthEvent("Boot", HealthState.Error, TimeSpan.FromSeconds(300));
            //simulate initialization time (e.g. IoC container config, cache loads etc) before moving on
            Thread.Sleep(TimeSpan.FromSeconds(15));
        }

        internal static void AfterServiceTypeRegistered() { }

        internal static void OnStartupCreated()
        {
            //simulate configuration time for the Api
            Thread.Sleep(TimeSpan.FromSeconds(15));
        }

        internal static void OnStartupConfigurationEnd()
        {
            // we are done with setting up the http service, so report the service in OK state
            RegisterHealthEvent("Boot", HealthState.Ok, TimeSpan.FromSeconds(60));
        }
    }
}