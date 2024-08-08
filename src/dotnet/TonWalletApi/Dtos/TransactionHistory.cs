using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace TonWalletApi.Dtos
{
    public class TransactionHistory
    {
        public long Utime { get; set; }
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public TransactionType TransactionType { get; set; }
        public long Commission { get; set; }
        public decimal Value { get; set; }
    }
    
    public enum TransactionType 
    {
        Sent = 0,
        Received = 1,
        Exchange = 2,
    }
}
