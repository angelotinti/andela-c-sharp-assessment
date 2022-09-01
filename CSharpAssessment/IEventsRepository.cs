using System.Collections.Generic;

namespace CSharpAssessment
{
    public interface IEventsRepository
    {
        IList<Event> FindAll();
        IList<Event> FindEventsByCity(string city);
        IList<Event> FindEventsByCities(string[] cities);
    }
}
