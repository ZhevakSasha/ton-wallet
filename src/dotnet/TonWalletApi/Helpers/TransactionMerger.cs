using TonWalletApi.Dtos;

namespace TonWalletApi.Helpers
{
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
                        }
                        else if (transaction.OutMsgs != null && transaction.OutMsgs.Any())
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
