using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpexSDK.Models
{
    /// <summary>
    ///     Groups together a collection of Group elements into a logical group. A transaction is envisioned to consist of the
    ///     contents of a folder of pages.
    /// </summary>
    public class Transaction
    {
        internal Transaction()
        {
            _groups = new List<Group>();
        }

        private readonly IList<Group> _groups;

        /// <summary>
        ///     Transaction in batch. Batch tickets always have a Transaction Number of zero. All pages in a transaction have the
        ///     same Transaction Number. The Transaction Number of the first non-batch ticket transaction may be 1 or another
        ///     larger number as defined in the job file.
        /// </summary>
        public int? TransactionId { get; set; }

        /// <summary>
        ///     One for each group within the transaction.
        /// </summary>
        public ReadOnlyCollection<Group> Groups => new ReadOnlyCollection<Group>(_groups);

        internal void Add(Group group)
        {
            _groups.Add(group);
        }
    }
}