namespace Monri.API.Models
{
    public class AppSettings
    {
        public string JSONPlaceholder { get; set; }
        public EmailSettings EmailSettings { get; set; }
    }
    public class EmailSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
    }
}
