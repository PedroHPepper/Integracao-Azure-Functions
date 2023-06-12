using System;
using Azure.Messaging.ServiceBus;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pepper.Service.Repositories.Contracts;
using Pepper.Models.Entities;
using Pepper.Service.Repositories;

namespace Pepper.SqlTimer
{
    public class ContactTimer
    {
        private readonly IContactService _contactService;
        private readonly ILogger _logger;
        public ContactTimer(IContactService contactService, ILogger<ContactTimer> logger)
        {
            _contactService = contactService;
            _logger = logger ?? throw new ArgumentNullException("logger");
        }
        [FunctionName("ContactTimer")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            List<Contact> contacts = (List<Contact>)_contactService.GetAll();
            foreach (Contact contact in contacts)
            {
                string contactBody = JsonConvert.SerializeObject(contact);
                string ConnString = Environment.GetEnvironmentVariable("SBconnString");
                string queue = Environment.GetEnvironmentVariable("queueContact");

                ServiceBusClient client;
                ServiceBusSender sender;

                client = new ServiceBusClient(ConnString);
                sender = client.CreateSender(queue);

                ServiceBusMessage messsage = new ServiceBusMessage(contactBody);

                sender.SendMessageAsync(messsage);
            }
        }
    }
}
