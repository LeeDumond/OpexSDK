namespace OpexSDK.Enumerations
{
    /// <summary>
    ///     Describes the outcome of a MICR read.
    /// </summary>
    public enum MicrStatus
    {
        /// <summary>
        ///     A MICR line was found, and was successfully read.
        /// </summary>
        Good,

        /// <summary>
        ///     A MICR line was found, and was partially read.
        /// </summary>
        Partial,

        /// <summary>
        ///     A MICR line was found, but was not successfully read.
        /// </summary>
        Bad,

        /// <summary>
        ///     No MICR line was found.
        /// </summary>
        NoMicr,

        /// <summary>
        ///     MICR detection was not active.
        /// </summary>
        Inactive,

        /// <summary>
        ///     A read error occured.
        /// </summary>
        Error
    }
}