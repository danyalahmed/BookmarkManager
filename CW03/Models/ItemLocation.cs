namespace CW03.Models
{
    public class ItemLocation : Item
    {
        private double _latitude;
        private double _longitude;

        public ItemLocation()
        {
            BookmarkType = Type.LOCATION;
        }

        public ItemLocation(string name, double latitude, double longitude) : base(name)
        {
            BookmarkType = Type.LOCATION;
            Latitude = latitude;
            Longitude = longitude;
            //Content is set based on latitude and longitude.
            Content = GetContent();
        }

        public double Latitude
        {
            get => _latitude;
            set
            {
                _latitude = value;
                UpdateContent();
            }
        }

        public double Longitude
        {
            get => _longitude;
            set
            {
                _longitude = value;
                UpdateContent();
            }
        }

        public string GetContent()
        {
            return "(" + Latitude + "," + Longitude + ")";
        }

        private void UpdateContent()
        {
            Content = "(" + Latitude + "," + Longitude + ")";
        }
    }
}