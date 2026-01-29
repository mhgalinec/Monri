namespace Monri.Data.Models
{
    public class User : BaseModel<int>
    {
        //FirstName and LastName refer to the Form fields, Name refers to the api results
        public string Name { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public Company Company { get; set; }
    }
}
