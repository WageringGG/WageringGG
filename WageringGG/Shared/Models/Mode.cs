using System;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace WageringGG.Shared.Models
{
    public abstract class Mode : Approvable
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Title { get; set; }
        [Required]
        [StringLength(250, MinimumLength = 5)]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }
        public bool IsPrivate { get; set; }

        public abstract string[] HostIds();
        public abstract string[] ClientIds();
        public abstract string[] AllIds();
    }
}