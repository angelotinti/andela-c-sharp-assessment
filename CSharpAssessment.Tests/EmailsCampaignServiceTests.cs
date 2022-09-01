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
        private ICitiesService _citiesService;
        private EmailsCampaignService _emailsCampaignService;

        public EmailsCampaignServiceTests()
        {
            _customersRepository = Substitute.For<ICustomersRepository>();
            _eventsRepository = Substitute.For<IEventsRepository>();
            _emailsSender = Substitute.For<IEmailsSender>();
            _citiesService = Substitute.For<ICitiesService>();
            _emailsCampaignService = new EmailsCampaignService(_customersRepository, _eventsRepository, _emailsSender, _citiesService);

            _eventsRepository.FindAll().Returns(new List<Event>
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
        public void Should_Send_Campaign_Emails_For_Events_In_Same_City()
        {
            //given
            var customerMrFake = new Customer { Name = "Mr. Fake", City = "New York" };
            _customersRepository.FindAll().Returns(new List<Customer>
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

        [Fact]
        public void Should_Send_Campain_Emaisl_From_Near_CitiesA()
        {
            //given
            var customerMrFake = new Customer { Name = "Mr. Fake", City = "New York" };
            _customersRepository.FindAll().Returns(new List<Customer>
            {
                customerMrFake
            });
            var cityLosAngeles = "Los Angeles";
            var cityBoston = "Boston";
            var cityChicago = "Chicago";
            var citySanFrancisco = "San Francisco";
            _citiesService.GetDistance(customerMrFake.City, Arg.Any<string>()).Returns(10);
            _citiesService.GetDistance(customerMrFake.City, customerMrFake.City).Returns(0);
            _citiesService.GetDistance(customerMrFake.City, cityLosAngeles).Returns(1);
            _citiesService.GetDistance(customerMrFake.City, cityBoston).Returns(2);
            _citiesService.GetDistance(customerMrFake.City, cityChicago).Returns(3);
            _citiesService.GetDistance(customerMrFake.City, citySanFrancisco).Returns(4);

            //when
            _emailsCampaignService.SendCampaignNearCitiesA();

            //then
            _emailsSender.Received().AddToEmail(customerMrFake, Arg.Is<Event>(x => x.Name == "Metallica" && x.City == cityLosAngeles));
            _emailsSender.Received().AddToEmail(customerMrFake, Arg.Is<Event>(x => x.Name == "Metallica" && x.City == cityBoston));
            _emailsSender.Received().AddToEmail(customerMrFake, Arg.Is<Event>(x => x.Name == "LadyGaGa" && x.City == cityBoston));
            _emailsSender.Received().AddToEmail(customerMrFake, Arg.Is<Event>(x => x.Name == "LadyGaGa" && x.City == cityChicago));
            _emailsSender.Received().AddToEmail(customerMrFake, Arg.Is<Event>(x => x.Name == "LadyGaGa" && x.City == citySanFrancisco));
        }
    }
}
