using System.ComponentModel.DataAnnotations.Schema;

namespace WageringGG.Shared.Models
{
    public class Tournament : Mode
    {
        [Column(TypeName = "decimal(18,7)")]
        public decimal Entry { get; set; }
    }
}