using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TonWallet.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Userame { get; set; }
        public string RAWWalletAddress { get; set; }
    }
}
