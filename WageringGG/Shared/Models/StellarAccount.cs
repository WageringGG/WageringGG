namespace WageringGG.Shared.Models
{
    public class StellarAccount
    {
        public int Id { get; set; }
        public string PublicKey { get; set; }
        public string SecretSeed { get; set; }
        public decimal Balance { get; set; }
    }
}
