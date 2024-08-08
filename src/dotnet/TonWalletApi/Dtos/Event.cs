using Newtonsoft.Json;

namespace TonWalletApi.Dtos
{
    public class SimplePreview
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        [JsonProperty("value_image")]
        public string Image { get; set; }
    }

    public class EventAction
    {
        [JsonProperty("simple_preview")]
        public SimplePreview SimplePreview { get; set; }
        [JsonProperty("JettonTransfer")]
        public JettonTransfer JettonTransfer { get; set; }
    }

    public class JettonTransfer
    {
        public Sender Sender { get; set; }
        public Recipient Recipient { get; set; }
        public string Amount { get; set; }

    }

    public class Sender 
    {
        public string Address { get; set; }
    }

    public class Recipient 
    {
        public string Address { get; set; }
    }

    public class Event
    {
        public long Timestamp { get; set; }
        public List<EventAction> Actions { get; set; }
        public Account Account { get; set; }
    }

    public class EventResponse
    {
        public List<Event> Events { get; set; }
        [JsonProperty("next_from")]
        public long NextFrom { get; set; }
    }
}
