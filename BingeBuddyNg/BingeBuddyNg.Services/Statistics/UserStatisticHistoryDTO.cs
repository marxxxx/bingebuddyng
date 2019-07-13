using System;

namespace BingeBuddyNg.Api.Dto
{
    public class UserStatisticHistoryDTO
    {
        public UserStatisticHistoryDTO(DateTime timestamp, double alcLevel)
        {
            Timestamp = timestamp;
            AlcLevel = alcLevel;
        }

        public DateTime Timestamp { get; set; }
        public double AlcLevel { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Timestamp)}={Timestamp}, {nameof(AlcLevel)}={AlcLevel}}}";
        }
    }
}
