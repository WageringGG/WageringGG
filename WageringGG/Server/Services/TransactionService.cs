using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Shared.Models;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;

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
        /// <param name="context">Database context</param>
        /// <param name="source">Keys to the user sending funds.</param>
        /// <param name="receipt">The transaction being processed.</param>
        /// <returns>True if the transaction is successful.</returns>
        public async Task<bool> ReceiveFunds(ApplicationDbContext context, KeyPair source, TransactionReceipt receipt)
        {
            KeyPair server = KeyPair.FromAccountId(_config["Stellar:PublicKey"]);
            AccountResponse account = await _server.Accounts.Account(source.AccountId);
            Asset asset = new AssetTypeNative();
            string amount = receipt.Amount.ToString();
            PaymentOperation payment = new PaymentOperation.Builder(server, asset, amount).Build();
            Transaction transaction = new TransactionBuilder(account)
                .AddOperation(payment).AddMemo(new MemoText("wagering.gg")).Build();
            transaction.Sign(source);
            SubmitTransactionResponse transactionResponse = await _server.SubmitTransaction(transaction);
            //log result
            if (transactionResponse.IsSuccess())
            {
                context.Transactions.Add(receipt);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sends funds from the server to a destination account.
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="destination">Keys to the user receiving funds.</param>
        /// <param name="receipt">The transaction being processed.</param>
        /// <returns>True if the trnasaction is successful.</returns>
        public async Task<bool> RefundFunds(ApplicationDbContext context, KeyPair destination, TransactionReceipt receipt)
        {
            KeyPair server = KeyPair.FromAccountId(_config["Stellar:SecretSeed"]);
            AccountResponse account = await _server.Accounts.Account(server.AccountId);
            Asset asset = new AssetTypeNative();
            string amount = receipt.Amount.ToString();
            PaymentOperation payment = new PaymentOperation.Builder(destination, asset, amount).Build();
            Transaction transaction = new TransactionBuilder(account)
                .AddOperation(payment).AddMemo(new MemoText("wagering.gg")).Build();
            transaction.Sign(server);
            SubmitTransactionResponse transactionResponse = await _server.SubmitTransaction(transaction);
            //log result
            if (transactionResponse.IsSuccess())
            {
                context.Transactions.Add(receipt);
                return true;
            }
            return false;
        }
    }
}
