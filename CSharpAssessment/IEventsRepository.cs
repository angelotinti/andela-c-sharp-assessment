using System.Collections.Generic;

namespace CSharpAssessment
{
    public interface IEventsRepository
    {
        IList<Event> FindEvents();
        IList<Event> FindEventsByCity(string city);
    }
}
