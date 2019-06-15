using OpexSDK.Enumerations;

namespace OpexSDK.Models
{
    /// <summary>
    /// Contains information about a single barcode read.
    /// </summary>
    public class Barcode
    {
        /// <summary>
        /// The index of the Barcode read, as configured in the job. Values are 1-indexed. (i.e., "1" is the first index.)
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Description of the type of barcode, as configured in the job. Note that not all transports support all barcode types.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Side on which the Barcode read was performed.
        /// </summary>
        public Side Side { get; set; }

        /// <summary>
        /// Barcode read result. Data can include alpha-numeric digits and special characters. If null, indicates that the barcode algorithm was run, but no valid barcode was found.
        /// </summary>
        public string Value { get; set; }
    }
}