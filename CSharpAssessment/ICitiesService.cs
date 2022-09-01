using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpAssessment
{
    internal interface ICitiesService
    {
        int GetDistance(string fromCity, string toCity);
    }
}
