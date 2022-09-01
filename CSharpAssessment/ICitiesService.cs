namespace CSharpAssessment
{
    public interface ICitiesService
    {
        int GetDistance(string fromCity, string toCity);
        int GetClosestCities(string city, int limit);
    }
}
