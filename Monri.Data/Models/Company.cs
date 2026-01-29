namespace Monri.Data.Models
{
    public class Company : BaseModel<int>
    {
        public string Name { get; set; }
        public string CatchPhrase { get; set; }
        public string Bs { get; set; }
    }
}
