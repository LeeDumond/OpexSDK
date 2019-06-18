using System.Collections.Generic;

namespace OpexSDK.Models
{
    /// <summary>
    /// Groups together a collection of scanned pages into a logical group. A Group typically consists of the contents of a stapled collection of pages.
    /// </summary>
    public class Group
    {
        internal Group()
        {
            Pages = new List<Page>();
        }

        /// <summary>
        /// Group number within transaction starting with 1, unless a Batch ticket, which always has a Group Number of 0. All pages in a group have the same Group Number. There can be multiple groups in a transaction and multiple pages within a document. In cases where there is one group per transaction, this value is equal to the Transaction Number.
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// One for each page within the group.
        /// </summary>
        public IList<Page> Pages { get; }
    }
}