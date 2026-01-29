namespace Monri.Data.Models
{
    public class Geolocation : BaseModel<int>
    {
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}
