using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace WageringGG.Shared.Models
{
    //http://math.bu.edu/people/mg/ratings/rs/node1.html could be used for tournaments
    //https://blog.mackie.io/the-elo-algorithm
    public class Rating
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }

        [Column(TypeName = "decimal(9,2)")]
        public decimal Value { get; set; } = Initial;
        public int GamesPlayed { get; set; }

        public const int Initial = 1000;
        public const int K = 750;
        public const int N = 1000;
    }
}
