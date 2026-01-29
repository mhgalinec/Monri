namespace Monri.Core.Models.DTOs
{
    public class AddressDTO
    {
        public string Street { get; set; }
        public string Suite { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public GeolocationDTO Geo { get; set; }
    }
}
