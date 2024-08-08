using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Numerics;
using TonSdk;
using TonSdk.Modules;
using TonWallet.Domain.Repositories;
using TonWalletApi.Dtos;

namespace TonWalletApi.Services
{
    public class TonService : ITonService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserRepository _userRepository;

        public TonService(IHttpClientFactory httpClientFactory, IUserRepository userRepository)
        {
            _httpClientFactory = httpClientFactory;
            _userRepository = userRepository;
        }

        public async Task<List<Balance>> GetJettonsAsync(int userId)
        {
            var walletAddress =  await _userRepository.GetUserWalletAddress(userId);
            var client = _httpClientFactory.CreateClient();
            var url = $"https://tonapi.io/v2/accounts/{walletAddress}/jettons?currencies=uah,usd";

            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, List<Balance>>>(responseString);
            var balances = jsonObject["balances"];

            url = $"https://tonapi.io/v2/accounts/{walletAddress}";
            response = await client.GetAsync(url);
            responseString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseString);
            var balance = json["balance"]?.ToObject<string>();

            url = $"https://tonapi.io/v2/rates?tokens=ton&currencies=usd,uah";
                 response = await client.GetAsync(url);
                 responseString = await response.Content.ReadAsStringAsync();
                 var rates = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, PriceInfo>>>(responseString);
                 var tonRate = rates["rates"]["TON"];

            balances.Insert(0,
                new Balance
                {

                    Price = tonRate,
                    BalanceAmount = balance,
                    Jetton =
                    new Jetton
                    {
                        Name = "Toncoin",
                        Symbol = "TON",
                        Decimals = 9,
                        Image = "https://s2.coinmarketcap.com/static/img/coins/64x64/11419.png"
                    }
                });

             return balances;
        }

        public async Task<List<Dtos.Point>> GetJettonChartAsync(string jettonAddress, long startDate)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://tonapi.io/v2/rates/chart?token={jettonAddress}&currency=usd&start_date={startDate}&end_date={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}&points_count=200";
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            var chartResponse = JsonConvert.DeserializeObject<ChartResponse>(responseString);

            var points = new List<Dtos.Point>();
            foreach (var pointArray in chartResponse.Points)
            {
                if (pointArray.Count == 2)
                {
                    points.Add(new Dtos.Point
                    {
                        Utime = (long)pointArray[0],
                        Value = pointArray[1]
                    });
                }
            }

            return points;
        }

        public async Task<List<TransactionHistory>> GetTonHistoryAsync(int userId)
        {
            var walletAddress = await _userRepository.GetUserWalletAddress(userId);
            var client = _httpClientFactory.CreateClient();
            var url = $"https://tonapi.io/v2/blockchain/accounts/{walletAddress}/transactions?limit=100&sort_order=desc";

            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, List<Transaction>>>(responseString);
            var transactions = jsonResponse["transactions"];

            var filteredTransactions = transactions
                .Where(t => (t.InMsg != null && t.InMsg.Value > 0) ||
                    (t.OutMsgs != null && t.OutMsgs.Any(outMsg => outMsg.Value > 0)))
                .ToList();

            var transactionsDictionary = TransactionProcessor.ProcessTransactions(filteredTransactions);

            var mergedTransactions = TransactionMerger.MergeTransactions(transactionsDictionary);

            var sortedTransactions = mergedTransactions.OrderByDescending(t => t.Utime).ToList();

            return sortedTransactions;
        }

        public async Task<List<TransactionHistory>> GetJettonHistoryAsync(int userId, string jettonAddress)
        {
            var walletAddress = await _userRepository.GetUserWalletAddress(userId);
            var client = _httpClientFactory.CreateClient();
            var url = $"https://tonapi.io/v2/accounts/{walletAddress}/jettons/{jettonAddress}/history?limit=100&start_date=1668436763&end_date={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<EventResponse>(responseString);
            var events = jsonResponse.Events;

            var transactionsHistory = events.Select(e => new TransactionHistory() { 
                Value = Convert.ToDecimal(e.Actions.FirstOrDefault()?.JettonTransfer.Amount), 
                Utime = e.Timestamp,
                TransactionType = (e.Actions.FirstOrDefault()?.JettonTransfer.Recipient.Address == e.Account.Address) ? TransactionType.Received : TransactionType.Sent
            })
            .ToList();

            return transactionsHistory;
        }

        public static class TransactionProcessor
        {
            public static Dictionary<long, List<Transaction>> ProcessTransactions(List<Transaction> transactions)
            {
                var dictionary = new Dictionary<long, List<Transaction>>();

                foreach (var transaction in transactions)
                {
                    long queryId = transaction.InMsg?.DecodedBody?.QueryId ?? 0;
                    if (queryId == 0 && transaction.OutMsgs != null && transaction.OutMsgs.Any())
                    {
                        queryId = transaction.OutMsgs.First().DecodedBody?.QueryId ?? 0;
                    }

                    if (!dictionary.ContainsKey(queryId))
                    {
                        dictionary[queryId] = new List<Transaction>();
                    }
                    dictionary[queryId].Add(transaction);
                }

                return dictionary;
            }
        }

        public static class TransactionMerger
        {
            public static List<TransactionHistory> MergeTransactions(Dictionary<long, List<Transaction>> transactionsDictionary)
            {
                var mergedTransactions = new List<TransactionHistory>();

                foreach (var kvp in transactionsDictionary)
                {
                    long queryId = kvp.Key;
                    List<Transaction> transactions = kvp.Value;

                    if (queryId == 0)
                    {
                        foreach (var transaction in transactions)
                        {
                            if (transaction.InMsg.Value > 0)
                            {
                                mergedTransactions.Add(new TransactionHistory
                                {
                                    Value = transaction.InMsg.Value,
                                    TransactionType = TransactionType.Received,
                                    Commission = 0,
                                    Utime = transaction.Utime
                                });
                            } else if (transaction.OutMsgs != null && transaction.OutMsgs.Any())
                                {
                                    mergedTransactions.Add(new TransactionHistory
                                    {
                                        Value = transaction.OutMsgs.FirstOrDefault().Value,
                                        TransactionType = TransactionType.Sent,
                                        Commission = 0,
                                        Utime = transaction.Utime
                                    });
                                }
                        }

                        continue;
                    }

                    var mergedTransaction = new TransactionHistory();

                    mergedTransaction.TransactionType = TransactionType.Exchange;

                    foreach (var transaction in transactions)
                    {
                        if (transaction.InMsg.Value > 0)
                        {
                            mergedTransaction.Commission += transaction.InMsg.Value;    
                        }

                        if (transaction.OutMsgs != null && transaction.OutMsgs.Any())
                        {
                            var value = (transaction.OutMsgs.FirstOrDefault().DecodedBody.Amount != 0) ? 
                                transaction.OutMsgs.FirstOrDefault().DecodedBody.Amount : transaction.OutMsgs.FirstOrDefault().Value - mergedTransaction.Commission;

                            mergedTransaction.Value = value;
                            mergedTransaction.Commission = transaction.OutMsgs.FirstOrDefault().Value - value - mergedTransaction.Commission;
                            mergedTransaction.Utime = transaction.Utime;
                        } 
                    }

                    mergedTransactions.Add(mergedTransaction);
                }

                return mergedTransactions;
            }
        }

    }  
}
