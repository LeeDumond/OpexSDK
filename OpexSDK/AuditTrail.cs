using OpexSDK.Enumerations;

namespace OpexSDK
{
    /// <summary>
    /// Contains information about a single audit trail. Audit trails are lines of text, often containing dates, batch numbers, sequence numbers, etc. They can be physically printed onto a page (i.e., by an inkjet printer) or can be electronically embedded into an image.
    /// </summary>
    public class AuditTrail
    {
        /// <summary>
        /// The type of audit trail performed.
        /// </summary>
        public AuditTrailType Type { get; set; }

        /// <summary>
        /// Side onto which the Audit Trail was added.
        /// </summary>
        public Side Side { get; set; }

        /// <summary>
        /// Indicates whether or not the software directed the scanner to imprint the audit trail on the paper/image. (Some operations, such as RescanNoPrint, prepare the audit trail but do not apply it.) Note that mechanical failures or ink shortages can result in physical imprinting not occurring even if Apply is true.
        /// </summary>
        public bool Apply { get; set; }

        /// <summary>
        /// The actual text string that was prepared by the system. Check the "Apply" property to determine whether or not the text was actually imprinted on the document/image.
        /// </summary>
        public string Text { get; set; }
    }
}