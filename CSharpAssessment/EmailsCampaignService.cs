using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CSharpAssessment
{
    internal class EmailsCampaignService : IEmailsCampaignService
    {
        private readonly ICustomersRepository _customersRepository;
        private readonly IEventsRepository _eventsRepository;
        private readonly IEmailsSender _emailsSender;
        private readonly ICitiesService _citiesService;

        public EmailsCampaignService(
            ICustomersRepository customersRepository,
            IEventsRepository eventsRepository,
            IEmailsSender emailsSender,
            ICitiesService citiesService)
        {
            _customersRepository = customersRepository;
            _eventsRepository = eventsRepository;
            _emailsSender = emailsSender;
            _citiesService = citiesService;
        }

        public void SendCampaignEmails()
        {
            var customers = _customersRepository.FindAll();
            foreach (var customer in customers)
            {
                var customerCityEvents = _eventsRepository.FindEventsByCity(customer.City);
                foreach (var customerCityEvent in customerCityEvents)
                {
                    _emailsSender.AddToEmail(customer, customerCityEvent);
                }
            }
        }

        public void SendCampaignNearCitiesA()
        {
            var customers = _customersRepository.FindAll();
            foreach (var customer in customers)
            {
                var events = _eventsRepository.FindAll();
                var nearCitiesEvents = events
                    .Where(x => _citiesService.GetDistance(customer.City, x.City) != 0)
                    .OrderBy(x => _citiesService.GetDistance(customer.City, x.City))
                    .Take(5);
                foreach (var nearCityEvent in nearCitiesEvents)
                {
                    _emailsSender.AddToEmail(customer, nearCityEvent);
                }
            }
        }
    }
}

