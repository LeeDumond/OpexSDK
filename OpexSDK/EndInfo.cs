using System;

namespace OpexSDK
{
    public class EndInfo
    {
        public DateTime EndTime { get; set; }
        public int NumPages { get; set; }
        public int NumGroups { get; set; }
        public int NumTransactions { get; set; }
        public bool IsModified { get; set; }
    }
}