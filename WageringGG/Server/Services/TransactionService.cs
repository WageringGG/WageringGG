using Microsoft.Extensions.Configuration;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using System.Threading.Tasks;
using WageringGG.Server.Data;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Services
{
    public class TransactionService
    {
        private readonly stellar_dotnet_sdk.Server _server;
        private readonly IConfiguration _config;
        public TransactionService(stellar_dotnet_sdk.Server server, IConfiguration config)
        {
            _server = server;
            _config = config;
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
            if (transactionResponse.IsSuccess())
            {
                receipt.ToAddress = server.AccountId;
                receipt.FromAddress = source.AccountId;
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
        /// <returns>True if the transaction is successful.</returns>
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
            if (transactionResponse.IsSuccess())
            {
                receipt.ToAddress = destination.AccountId;
                receipt.FromAddress = server.AccountId;
                context.Transactions.Add(receipt);
                return true;
            }
            return false;
        }
    }
}
