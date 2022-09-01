using System.Collections.Generic;

namespace CSharpAssessment
{
    public interface ICustomersRepository
    {
        IList<Customer> FindCustomers();
    }
}
