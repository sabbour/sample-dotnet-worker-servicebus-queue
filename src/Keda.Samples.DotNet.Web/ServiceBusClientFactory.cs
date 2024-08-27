using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Keda.Samples.DotNet.Web
{
    public static class ServiceBusClientFactory
    {
        public static ServiceBusClient CreateWithWorkloadIdentityAuthentication(IConfiguration configuration)
        {
            var hostname = configuration.GetValue<string>("SERVICEBUS_HOST_NAME");
            var appIdentityId = configuration.GetValue<string>("SERVICEBUS_IDENTITY_CLIENTID");

            return new ServiceBusClient(hostname, new DefaultAzureCredential(
                 new DefaultAzureCredentialOptions
                 {
                     ManagedIdentityClientId = appIdentityId
                 }
             ));
        }

        public static ServiceBusClient CreateWithServicePrincipleAuthentication(IConfiguration configuration)
        {
            var hostname = configuration.GetValue<string>("SERVICEBUS_HOST_NAME");
            var tenantId = configuration.GetValue<string>("SERVICEBUS_TENANT_ID");
            var appIdentityId = configuration.GetValue<string>("SERVICEBUS_IDENTITY_CLIENTID");
            var appIdentitySecret = configuration.GetValue<string>("SERVICEBUS_IDENTITY_SECRET");

            return new ServiceBusClient(hostname, new ClientSecretCredential(tenantId, appIdentityId, appIdentitySecret));
        }

        public static ServiceBusClient CreateWithConnectionStringAuthentication(IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("SERVICEBUS_QUEUE_CONNECTIONSTRING");
            return new ServiceBusClient(connectionString);
        }
    }
}
