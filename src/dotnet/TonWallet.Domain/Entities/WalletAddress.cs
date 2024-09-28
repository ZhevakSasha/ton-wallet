using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TonWallet.Domain.Entities
{
    public class WalletAddress
    {
        public int Id { get; set; }
        public string RawForm { get; set; }
        public AddressInfo Bounceable { get; set; }
        public AddressInfo NonBounceable { get; set; }
    }

    public class AddressInfo
    {
        public string B64 { get; set; }
        public string B64url { get; set; }
    }
}
