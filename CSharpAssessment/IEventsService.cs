using System.Collections.Generic;

namespace CSharpAssessment
{
    public interface IEventsService
    {
        IList<Event> GetNearestEvents(string city, int limit);
    }
}
