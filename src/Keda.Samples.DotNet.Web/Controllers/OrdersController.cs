using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using GuardNet;
using Keda.Samples.Dotnet.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Keda.Samples.DotNet.Web.Controllers
{
    /// <summary>
    ///     API endpoint to manage orders
    /// </summary>
    [ApiController]
    [Route("api/v1/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly ServiceBusSender _queueClient;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="queueClient">Client to send messages to queue with</param>
        public OrdersController(ServiceBusSender queueClient)
        {
            Guard.NotNull(queueClient, nameof(queueClient));

            _queueClient = queueClient;
        }

        /// <summary>
        ///     Create Order
        /// </summary>
        [HttpPost(Name = "Order_Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody, Required] Order order)
        {
            var rawOrder = JsonConvert.SerializeObject(order);
            var orderMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(rawOrder));
            await _queueClient.SendMessageAsync(orderMessage);

            return Accepted();
        }
    }
}