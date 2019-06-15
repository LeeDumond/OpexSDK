namespace OpexSDK.Models
{
    /// <summary>
    /// The OSD can be configured to prompt the operator for any batch or page specific information. The operator is prompted at the beginning of each job or new batch.
    /// </summary>
    public class ReferenceId
    {
        /// <summary>
        /// The index of the ReferenceID. Values are 1-indexed. (i.e., "1" is the first index.)
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Text string containing the information that the Operator entered for the given item.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// A text-based description of what the Response represents.
        /// </summary>
        public string Name { get; set; }
    }
}