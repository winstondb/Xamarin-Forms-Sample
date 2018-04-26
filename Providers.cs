using SQLite.Net.Attributes;

namespace DiaApp.Models
{
    public class Providers
    {
        [PrimaryKey]
        public string ProviderId { get; set; }
        public string CompanyName { get; set; }
    }
}
