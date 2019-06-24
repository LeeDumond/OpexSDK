using OpexSDK.Enumerations;

namespace OpexSDK.Models
{
    /// <summary>
    ///     Contains information about a single Optical Mark Detect read.
    ///     Optical Mark Detection can be used to look for Change of Address (CHAD), checkboxes, etc.
    /// </summary>
    public class MarkDetect
    {
        internal MarkDetect()
        {
        }

        /// <summary>
        ///     The index of the Mark read, as configured in the job. Values are 1-indexed. (i.e., "1" is the first index.)
        /// </summary>
        public int? Index { get; internal set; }

        /// <summary>
        ///     Side on which the Mark Detect read was performed.
        /// </summary>
        public Side? Side { get; internal set; }

        /// <summary>
        ///     Indicates the outcome of the Mark detection. True if a mark was found; otherwise false.
        /// </summary>
        public bool? Result { get; internal set; }

        /// <summary>
        ///     Reiterates the index of the Mark read, i.e. if Index is x, this value will be 'MARK x'.
        /// </summary>
        public string Name { get; internal set; }
    }
}