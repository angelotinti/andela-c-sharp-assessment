using System;

namespace CSharpAssessment
{
    internal class EmailsCampaignService : IEmailsCampaignService
    {
        private readonly ICustomersRepository _customersRepository;
        private readonly IEventsRepository _eventsRepository;
        private readonly IEmailsSender _emailsSender;

        public EmailsCampaignService(
            ICustomersRepository customersRepository,
            IEventsRepository eventsRepository,
            IEmailsSender emailsSender)
        {
            _customersRepository = customersRepository;
            _eventsRepository = eventsRepository;
            _emailsSender = emailsSender;
        }

        public void SendCampaignEmails()
        {
            var customers = _customersRepository.FindCustomers();
            foreach (var customer in customers)
            {
                var customerCityEvents = _eventsRepository.FindEventsByCity(customer.City);
                foreach (var customerCityEvent in customerCityEvents)
                {
                    _emailsSender.AddToEmail(customer, customerCityEvent);
                }
            }
        }
    }
}
