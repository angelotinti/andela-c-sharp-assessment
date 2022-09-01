using CSharpAssessment;
using NSubstitute;
using System.Collections.Generic;
using Xunit;

namespace CSharpAssessmentTests
{
    public class EmailsCampaignServiceTests
    {
        private ICustomersRepository _customersRepository;
        private IEventsRepository _eventsRepository;
        private IEmailsSender _emailsSender;
        private EmailsCampaignService _emailsCampaignService;

        public EmailsCampaignServiceTests()
        {
            _customersRepository = Substitute.For<ICustomersRepository>();
            _eventsRepository = Substitute.For<IEventsRepository>();
            _emailsSender = Substitute.For<IEmailsSender>();
            _emailsCampaignService = new EmailsCampaignService(_customersRepository, _eventsRepository, _emailsSender);

            _eventsRepository.FindEvents().Returns(new List<Event>
            {
                new Event{ Name = "Phantom of the Opera", City = "New York"},
                new Event{ Name = "Metallica", City = "Los Angeles"},
                new Event{ Name = "Metallica", City = "New York"},
                new Event{ Name = "Metallica", City = "Boston"},
                new Event{ Name = "LadyGaGa", City = "New York"},
                new Event{ Name = "LadyGaGa", City = "Boston"},
                new Event{ Name = "LadyGaGa", City = "Chicago"},
                new Event{ Name = "LadyGaGa", City = "San Francisco"},
                new Event{ Name = "LadyGaGa", City = "Washington"}
            });

        }

        [Fact]
        public void Should_Send_Campaign_Emails()
        {
            //given
            var customerMrFake = new Customer { Name = "Mr. Fake", City = "New York" };
            _customersRepository.FindCustomers().Returns(new List<Customer>
            {
                customerMrFake
            });
            var phantomOfTheOperaNewYorkEvent = new Event { Name = "Phantom of the Opera", City = "New York" };
            var metallicaNewYorkEvent = new Event { Name = "Metallica", City = "New York" };
            var ladyGaGaNewYorkEvent = new Event { Name = "LadyGaGa", City = "New York" };
            _eventsRepository.FindEventsByCity(customerMrFake.City).Returns(new List<Event>
            {
                phantomOfTheOperaNewYorkEvent, metallicaNewYorkEvent, ladyGaGaNewYorkEvent
            });

            //when
            _emailsCampaignService.SendCampaignEmails();

            //then
            _emailsSender.Received().AddToEmail(customerMrFake, phantomOfTheOperaNewYorkEvent);
            _emailsSender.Received().AddToEmail(customerMrFake, metallicaNewYorkEvent);
            _emailsSender.Received().AddToEmail(customerMrFake, ladyGaGaNewYorkEvent);
        }
    }
}
