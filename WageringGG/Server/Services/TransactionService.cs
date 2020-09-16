using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using stellar_dotnet_sdk;
using System.Threading.Tasks;

namespace WageringGG.Server.Services
{
    public class TransactionService
    {
        private readonly stellar_dotnet_sdk.Server _server;
        private readonly ILogger<TransactionService> _logger;
        private readonly IConfiguration _config;
        public TransactionService(stellar_dotnet_sdk.Server server, IConfiguration config, ILogger<TransactionService> logger)
        {
            _server = server;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// This function sends funds from the source account to the server account.
        /// </summary>
        /// <param name="source">Keys to the user sending funds.</param>
        /// <param name="amount">The amount being transferred.</param>
        /// <returns>True if the transaction is successful.</returns>
        public async Task<bool> ReceiveFunds(KeyPair source, decimal amount)
        {
            KeyPair server = KeyPair.FromAccountId(_config["Stellar:PublicKey"]);

            return false;
        }

        /// <summary>
        /// Sends funds from the server to a destination account.
        /// </summary>
        /// <param name="destination">Keys to the user receiving funds.</param>
        /// <param name="amount">The amount being refunded.</param>
        /// <returns>True if the trnasaction is successful.</returns>
        public async Task<bool> RefundFunds(KeyPair destination, decimal amount)
        {
            KeyPair server = KeyPair.FromAccountId(_config["Stellar:PublicKey"]);

            return false;
        }
    }
}
