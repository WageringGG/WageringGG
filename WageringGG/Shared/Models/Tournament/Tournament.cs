using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WageringGG.Shared.Models
{
    public class Tournament : Mode
    {
        [Column(TypeName = "decimal(18,7)")]
        public decimal Entry { get; set; }

        public override bool IsApproved()
        {
            throw new System.NotImplementedException();
        }

        public override string GroupName
        {
            get
            {
                return GetGroupName.Tournament(Id);
            }
        }
        public override IEnumerable<string> HostIds()
        {
            return new List<string>();
        }
        public override IEnumerable<string> ClientIds()
        {
            return new List<string>();
        }
        public override IEnumerable<string> AllIds()
        {
            return new List<string>();
        }
    }
}