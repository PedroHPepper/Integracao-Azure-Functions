using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pepper.Models.Entities;

namespace Pepper.IntegrationServiceBus
{
    public class ContactServiceBus
    {
        private readonly ILogger _logger;
        public ContactServiceBus(ILogger<ContactServiceBus> logger)
        {
            _logger = logger ?? throw new ArgumentNullException("logger");
        }
        [FunctionName("Function1")]
        public void Run([ServiceBusTrigger("contato", Connection = "SBconnString")]string myQueueItem, ILogger log)
        {
            _logger.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            Contact contact = JsonConvert.DeserializeObject<Contact>(myQueueItem);
        }
    }
}
