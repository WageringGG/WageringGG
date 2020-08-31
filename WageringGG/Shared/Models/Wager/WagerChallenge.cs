using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

#nullable disable
namespace WageringGG.Shared.Models
{
    public class WagerChallenge : Approvable
    {
        public int Id { get; set; }

        public int WagerId { get; set; }
        public Wager Wager { get; set; }

        [Required]
        public DateTime Date { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,7)")]
        public decimal Amount { get; set; }
        public bool IsAccepted { get; set; }
    }
}
