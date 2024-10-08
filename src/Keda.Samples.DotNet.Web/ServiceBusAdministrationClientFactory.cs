﻿using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Keda.Samples.DotNet.Web
{
    public static class ServiceBusAdministrationClientFactory
    {

        public static ServiceBusAdministrationClient CreateWithWorkloadIdentityAuthentication(IConfiguration configuration)
        {
            var hostname = configuration.GetValue<string>("SERVICEBUS_HOST_NAME");
            var appIdentityId = configuration.GetValue<string>("SERVICEBUS_IDENTITY_CLIENTID");

            return new ServiceBusAdministrationClient(hostname, new DefaultAzureCredential(
                 new DefaultAzureCredentialOptions
                 {
                     ManagedIdentityClientId = appIdentityId
                 }
             ));
        }

        public static ServiceBusAdministrationClient CreateWithServicePrincipleAuthentication(IConfiguration configuration)
        {
            var hostname = configuration.GetValue<string>("SERVICEBUS_HOST_NAME");
            var tenantId = configuration.GetValue<string>("SERVICEBUS_TENANT_ID");
            var appIdentityId = configuration.GetValue<string>("SERVICEBUS_IDENTITY_CLIENTID");
            var appIdentitySecret = configuration.GetValue<string>("SERVICEBUS_IDENTITY_SECRET");

            return new ServiceBusAdministrationClient(hostname, new ClientSecretCredential(tenantId, appIdentityId, appIdentitySecret));
        }

        public static ServiceBusAdministrationClient CreateWithConnectionStringAuthentication(IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("SERVICEBUS_QUEUE_CONNECTIONSTRING");
            return new ServiceBusAdministrationClient(connectionString);
        }
    }
}
