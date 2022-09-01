using System.Linq;

namespace CSharpAssessment
{
    internal class EmailsCampaignService : IEmailsCampaignService
    {
        private readonly ICustomersRepository _customersRepository;
        private readonly IEventsRepository _eventsRepository;
        private readonly IEmailsSender _emailsSender;
        private readonly ICitiesService _citiesService;
        private readonly IEventsService _eventsService;

        public EmailsCampaignService(
            ICustomersRepository customersRepository,
            IEventsRepository eventsRepository,
            IEmailsSender emailsSender,
            ICitiesService citiesService,
            IEventsService eventsService)
        {
            _customersRepository = customersRepository;
            _eventsRepository = eventsRepository;
            _emailsSender = emailsSender;
            _citiesService = citiesService;
            _eventsService = eventsService;
        }

        public void SendCampaignEmailsA()
        {
            var customers = _customersRepository.FindAll();
            foreach (var customer in customers)
            {
                var events = _eventsRepository.FindAll();
                foreach (var @event in events)
                {
                    if (@event.City == customer.City)
                    {
                        _emailsSender.AddToEmail(customer, @event);
                    }
                }
            }
        }

        public void SendCampaignEmailsB()
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

        public void SendCampaignNearCitiesB()
        {
            var customers = _customersRepository.FindAll();
            foreach (var customer in customers)
            {
                var nearestEvents = _eventsService.GetNearestEvents(customer.City, 5);
                _emailsSender.AddToEmail(customer, nearestEvents);
            }
        }
    }
}

