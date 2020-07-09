using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace WageringGG.Shared.Models
{
    public class StellarAccount
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        [Column(TypeName = "decimal(18,7)")]
        public decimal Balance { get; set; }
        [JsonIgnore]
        public string SecretSeed { get; set; }
    }
}
