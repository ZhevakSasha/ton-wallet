using Newtonsoft.Json;

namespace TonWalletApi.Dtos
{
    public class Point
    {
        public long Utime { get; set; }
        public double Value { get; set; }
    }

    public class ChartResponse
    {
        [JsonProperty("points")]
        public List<List<double>> Points { get; set; }
    }
}
