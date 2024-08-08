namespace TonWalletApi.Dtos
{
    public class UserJettonInfoDto
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string BalanceAmount { get; set; }
        public Prices Prices { get; set; }
    }
}
