namespace CSharpAssessment
{
    public interface ICitiesService
    {
        int GetDistance(string fromCity, string toCity);
        string[] GetClosestCities(string city, int limit);
    }
}
