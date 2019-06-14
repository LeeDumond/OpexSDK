using System.Collections.Generic;

namespace OpexSDK
{
    public class Group
    {
        public int GroupId { get; set; }

        public ICollection<Page> Pages { get; }
    }
}