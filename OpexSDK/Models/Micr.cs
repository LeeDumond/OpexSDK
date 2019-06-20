using OpexSDK.Enumerations;

namespace OpexSDK.Models
{
    /// <summary>
    /// Contains information about a single MICR read. NOTE: Different OPEX products can use Magnetic readers, Optical readers, or a combination of both, when reading a MICR line. There is no indication as to which type(s) of reader was used.
    /// </summary>
    public class Micr
    {
        internal Micr()
        {

        }

        /// <summary>
        /// Indicates the outcome of the MICR read.
        /// </summary>
        public MicrStatus Status { get; set; }

        /// <summary>
        /// Indicates the outcome of determining the check routing and transit field.
        /// </summary>
        public RtStatus RtStatus { get; set; }

        /// <summary>
        /// Indicates the type of check and the country of origin.
        /// </summary>
        public CheckType CheckType { get; set; }

        /// <summary>
        /// Indicates the side on which the MICR read was performed. Normally, this will be Front.
        /// </summary>
        public Side Side { get; set; }

        /// <summary>
        /// The final result of the MICR read. Data includes numeric digits plus the following special symbol designators: 'c' – on us 'd' – routing/transit 'b' – amount '-' – dash '!' – rejected or unknown character.
        /// </summary>
        public string Value { get; set; }
    }
}