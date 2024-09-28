using Newtonsoft.Json;
using TonWallet.Domain.Entities;

namespace TonWalletApi.Dtos
{
    public class WalletAddressDto
    {
        [JsonProperty("raw_form")]
        public string RawForm { get; set; }

        [JsonProperty("bounceable")]
        public AddressInfo Bounceable { get; set; }

        [JsonProperty("non_bounceable")]
        public AddressInfo NonBounceable { get; set; }

        [JsonProperty("given_type")]
        public string GivenType { get; set; }

        [JsonProperty("test_only")]
        public bool TestOnly { get; set; }
    }

    public class AddressInfo
    {
        [JsonProperty("b64")]
        public string B64 { get; set; }

        [JsonProperty("b64url")]
        public string B64url { get; set; }
    }
}
