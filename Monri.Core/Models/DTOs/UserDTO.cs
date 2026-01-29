namespace Monri.Core.Models.DTOs
{
    public class UserDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public AddressDTO? Address { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public CompanyDTO? Company { get; set; }
    }
}
