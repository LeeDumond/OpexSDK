using System.Collections.Generic;

namespace OpexSDK
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        public ICollection<Group> Groups { get; }
    }
}