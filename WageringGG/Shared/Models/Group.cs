using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace WageringGG.Shared.Models
{
    public abstract class Group : Approvable
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }
        public bool IsPrivate { get; set; }

        public abstract string GroupName { get; }

        public abstract IEnumerable<string> HostIds();
        public abstract IEnumerable<string> ClientIds();
        public abstract IEnumerable<string> AllIds();
    }
}