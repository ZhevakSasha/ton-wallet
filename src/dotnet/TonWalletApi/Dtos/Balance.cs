using Newtonsoft.Json;

namespace TonWalletApi.Dtos
{
    public class Balance
    {
        [JsonProperty("balance")]
        public string BalanceAmount { get; set; }
        [JsonProperty("price")]
        public PriceInfo Price { get; set; }
        [JsonProperty("wallet_address")]
        public WalletAddress WalletAddress { get; set; }
        public Jetton Jetton { get; set; }
    }

    public class PriceRates
    {
        public PriceInfo TON { get; set; }
    }


    public class PriceInfo
    {
        [JsonProperty("prices")]
        public Prices Prices { get; set; }
        [JsonProperty("diff_24h")]
        public Diff Diff24h { get; set; }
        [JsonProperty("diff_7d")]
        public Diff Diff7d { get; set; }
        [JsonProperty("diff_30d")]
        public Diff Diff30d { get; set; }
    }

    public class Prices
    {
        [JsonProperty("USD")]
        public decimal USD { get; set; }

        [JsonProperty("UAH")]
        public decimal UAH { get; set; }
    }

    public class Diff
    {
        [JsonProperty("USD")]
        public string USD { get; set; }
        [JsonProperty("UAH")]
        public string UAH { get; set; }
    }

    public class WalletAddress
    {
        public string Address { get; set; }
    }

    public class Jetton
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int Decimals { get; set; }
        public string Image { get; set; }
    }

    public class Transaction
    {
        public string Hash { get; set; }
        public long Lt { get; set; }
        public Account Account { get; set; }
        public bool Success { get; set; }
        public long Utime { get; set; }
        public string OrigStatus { get; set; }
        public string EndStatus { get; set; }
        public long TotalFees { get; set; }
        public long EndBalance { get; set; }
        public string TransactionType { get; set; }


        [JsonProperty("in_msg")]
        public InMessage InMsg { get; set; }
        [JsonProperty("out_msgs")]
        public List<OutMessage> OutMsgs { get; set; }
    }

    public class Account
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public bool IsScam { get; set; }
        public string Icon { get; set; }
        public bool IsWallet { get; set; }
    }

    public class InMessage
    {
        [JsonProperty("msg_type")]
        public string MsgType { get; set; }
        public long Value { get; set; }
        [JsonProperty("decoded_body")]
        public DecodedBody DecodedBody { get; set; }
    }

    public class OutMessage
    {
        public string MsgType { get; set; }
        public long Value { get; set; }
        [JsonProperty("decoded_op_name")]
        public string DecodedOpName { get; set; }
        [JsonProperty("decoded_body")]
        public DecodedBody DecodedBody { get; set; }

    }

    public class DecodedBody
    {
        [JsonProperty("query_id")]
        public long QueryId { get; set; }
        public long Amount { get; set; }
    }
}
