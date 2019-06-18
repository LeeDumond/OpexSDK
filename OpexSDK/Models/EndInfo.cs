using System;

namespace OpexSDK.Models
{
    /// <summary>
    /// Contains information about the batch that is determined once the batch is ended or closed.
    /// </summary>
    public class EndInfo
    {
        internal EndInfo()
        {
            
        }

        /// <summary>
        /// Date and time the batch was ended / closed. Uses date set in OPEX Scanning Device PC.
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// The total number of pages in the batch, including batch tickets.
        /// This information is redundant in that it can be derived from the Count of the Pages collection in the Batch. It is provided primarily as a convenience.
        /// </summary>
        public int? NumPages { get; set; }

        /// <summary>
        /// The total number of groups in the batch, including batch tickets.
        /// This information is redundant in that it can be derived from the Count of the Groups collection in the Batch. It is provided primarily as a convenience.
        /// </summary>
        public int? NumGroups { get; set; }

        /// <summary>
        /// The total number of transactions in the batch, EXCLUDING batch tickets.
        /// This value may differ from the count of the Transactions collection in the Batch. It is provided primarily as a convenience.
        /// </summary>
        public int? NumTransactions { get; set; }

        /// <summary>
        /// Indicates whether or not the batch was modified (by the AS3600 Batch Editor, for example) after scanning was completed.
        /// True if the batch was modified; otherwise false.
        /// </summary>
        public bool? IsModified { get; set; }
    }
}