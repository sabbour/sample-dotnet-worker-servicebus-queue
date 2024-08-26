using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Keda.Samples.DotNet.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRazorPages();
            services.AddSignalR();

            services.AddOptions();
            var orderQueueSection = Configuration.GetSection("OrderQueue");
            services.Configure<OrderQueueSettings>(orderQueueSection);

            services.AddSwagger();
            services.AddScoped<ServiceBusSender>(serviceProvider =>
            {

                var serviceBusClient = AuthenticateToAzureServiceBus();
                var messageSender = serviceBusClient.CreateSender("orders");
                return messageSender;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(swaggerUiOptions =>
            {
                swaggerUiOptions.SwaggerEndpoint("v1/swagger.json", "Keda.Samples.Dotnet.API");
                swaggerUiOptions.DocumentTitle = "KEDA API";
            });
        }

        private ServiceBusClient AuthenticateToAzureServiceBus()
        {
            var authenticationMode = Configuration.GetValue<AuthenticationMode>("KEDA_SERVICEBUS_AUTH_MODE");

            ServiceBusClient serviceBusClient;

            switch (authenticationMode)
            {
                case AuthenticationMode.ConnectionString:
                    serviceBusClient = ServiceBusClientFactory.CreateWithConnectionStringAuthentication(Configuration);
                    break;
                case AuthenticationMode.ServicePrinciple:
                    serviceBusClient = ServiceBusClientFactory.CreateWithServicePrincipleAuthentication(Configuration);
                    break;
                case AuthenticationMode.WorkloadIdentity:
                    serviceBusClient = ServiceBusClientFactory.CreateWithWorkloadIdentityAuthentication(Configuration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return serviceBusClient;
        }
    }
}
