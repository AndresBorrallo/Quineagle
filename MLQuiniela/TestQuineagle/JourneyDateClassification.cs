using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestQuineagle
{
    public class JourneyDateClassification
    {
        public int journey { get; set; }
        public int season { get; set; }

        public class DistinctJourneyDateClassificationComparer : IEqualityComparer<JourneyDateClassification>
        {
            public bool Equals(JourneyDateClassification x, JourneyDateClassification y)
            {
                return x.journey == y.journey && x.season == y.season;
            }

            public int GetHashCode(JourneyDateClassification obj)
            {
                return obj.journey.GetHashCode() ^ obj.season.GetHashCode();
            }
        }
    }
   
}
