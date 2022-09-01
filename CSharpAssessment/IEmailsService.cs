namespace CSharpAssessment
{
    public interface IEmailsSender
    {
        void AddToEmail(Customer customer, Event @event);
    }
}
