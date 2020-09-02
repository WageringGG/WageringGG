using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WageringGG.Shared.Models
{
    public class StellarAccount
    {
        [Key]
        public string Id { get; set; }
        [Column(TypeName = "decimal(18,7)")]
        public decimal Balance { get; set; }
        [JsonIgnore]
        public string SecretSeed { get; set; }
    }
}
