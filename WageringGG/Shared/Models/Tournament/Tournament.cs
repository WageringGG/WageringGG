using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WageringGG.Shared.Models
{
    public class Tournament : Mode
    {
        [Column(TypeName = "decimal(18,7)")]
        public decimal Entry { get; set; }

        public override string[] HostIds()
        {
            return null;
        }
        public override string[] ClientIds()
        {
            return null;
        }
        public override string[] AllIds()
        {
            return null;
        }
    }
}