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
        private IEventsService _eventsService;
        private EmailsCampaignService _emailsCampaignService;

        private IList<Event> _events;

        public EmailsCampaignServiceTests()
        {
            _customersRepository = Substitute.For<ICustomersRepository>();
            _eventsRepository = Substitute.For<IEventsRepository>();
            _emailsSender = Substitute.For<IEmailsSender>();
            _citiesService = Substitute.For<ICitiesService>();
            _eventsService = Substitute.For<IEventsService>();
            _emailsCampaignService = new EmailsCampaignService(_customersRepository, _eventsRepository, _emailsSender, _citiesService, _eventsService);

            _events = new List<Event>
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
            };

            _eventsRepository.FindAll().Returns(_events);
        }

        [Fact]
        public void Should_Send_Campaign_Emails_For_Events_In_Same_CityA()
        {
            //given
            var customerMrFake = new Customer { Name = "Mr. Fake", City = "New York" };
            _customersRepository.FindAll().Returns(new List<Customer>
            {
                customerMrFake
            });

            //when
            _emailsCampaignService.SendCampaignEmailsA();

            //then
            _emailsSender.Received().AddToEmail(customerMrFake, Arg.Is<Event>(x => x.Name == "Phantom of the Opera" && x.City == "New York"));
            _emailsSender.Received().AddToEmail(customerMrFake, Arg.Is<Event>(x => x.Name == "Metallica" && x.City == "New York"));
            _emailsSender.Received().AddToEmail(customerMrFake, Arg.Is<Event>(x => x.Name == "LadyGaGa" && x.City == "New York"));
        }

        [Fact]
        public void Should_Send_Campaign_Emails_For_Events_In_Same_CityB()
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
            _emailsCampaignService.SendCampaignEmailsB();

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

        [Fact]
        public void Should_Send_Campain_Emaisl_From_Near_CitiesB()
        {
            //given
            var customerMrFake = new Customer { Name = "Mr. Fake", City = "New York" };
            _customersRepository.FindAll().Returns(new List<Customer>
            {
                customerMrFake
            });
            var preDefinedLimit = 5;
            _eventsService.GetNearestEvents(customerMrFake.City, preDefinedLimit).Returns(new List<Event>
            {
                _events[1], _events[3], _events[5], _events[6], _events[7]
            });

            //when
            _emailsCampaignService.SendCampaignNearCitiesB();

            //then
            _emailsSender.Received().AddToEmail(customerMrFake, Arg.Is<IList<Event>>(x => x.Contains(_events[1])));
            _emailsSender.Received().AddToEmail(customerMrFake, Arg.Is<IList<Event>>(x => x.Contains(_events[3])));
            _emailsSender.Received().AddToEmail(customerMrFake, Arg.Is<IList<Event>>(x => x.Contains(_events[5])));
            _emailsSender.Received().AddToEmail(customerMrFake, Arg.Is<IList<Event>>(x => x.Contains(_events[6])));
            _emailsSender.Received().AddToEmail(customerMrFake, Arg.Is<IList<Event>>(x => x.Contains(_events[7])));
        }
    }
}
