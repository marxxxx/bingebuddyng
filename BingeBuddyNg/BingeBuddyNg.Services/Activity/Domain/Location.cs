namespace BingeBuddyNg.Services.Activity
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool IsValid()
        {
            return Latitude != 0 && Longitude != 0;
        }

        public Location()
        {

        }

        public Location(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public override string ToString()
        {
            return $"{nameof(Latitude)}: {Latitude}, {nameof(Longitude)}: {Longitude}";
        }
    }
}
