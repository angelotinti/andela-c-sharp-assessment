using System.Collections.Generic;

namespace CSharpAssessment
{
    public interface IEmailsSender
    {
        void AddToEmail(Customer customer, Event @event);
        void AddToEmail(Customer customer, IList<Event> events);
    }
}
