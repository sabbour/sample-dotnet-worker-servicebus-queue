using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Keda.Samples.Dotnet.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Configuration;

namespace Keda.Samples.DotNet.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class QueueController : ControllerBase
    {
        protected IConfiguration Configuration { get; }

        public QueueController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private ServiceBusAdministrationClient AuthenticateToAzureServiceBus()
        {
            var authenticationMode = Configuration.GetValue<AuthenticationMode>("KEDA_SERVICEBUS_AUTH_MODE");
            Console.WriteLine("Authentication mode: " + authenticationMode);

            ServiceBusAdministrationClient serviceBusClient;

            switch (authenticationMode)
            {
                case AuthenticationMode.ConnectionString:
                    //Logger.LogInformation($"Authentication by using connection string");
                    serviceBusClient = ServiceBusAdministrationClientFactory.CreateWithConnectionStringAuthentication(Configuration);
                    break;
                case AuthenticationMode.ServicePrinciple:
                    //Logger.LogInformation("Authentication by using service principle");
                    serviceBusClient = ServiceBusAdministrationClientFactory.CreateWithServicePrincipleAuthentication(Configuration);
                    break;
                case AuthenticationMode.WorkloadIdentity:
                    //Logger.LogInformation("Authentication by using workload identity");
                    serviceBusClient = ServiceBusAdministrationClientFactory.CreateWithWorkloadIdentityAuthentication(Configuration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return serviceBusClient;
        }


        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<QueueStatus> Get()
        {
             // Check current queue length
            var serviceBusClient = AuthenticateToAzureServiceBus();
            var queueInfo = await serviceBusClient.GetQueueRuntimePropertiesAsync("orders");

            return new QueueStatus
            {
                MessageCount = queueInfo.Value.TotalMessageCount
            };
        }
    }
}