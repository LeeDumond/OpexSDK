namespace OpexSDK.Enumerations
{
    /// <summary>
    /// Describes the outcome of determining the check routing and transit field.
    /// </summary>
    public enum RtStatus
    {
        /// <summary>
        /// Passed checksum.
        /// </summary>
        Good,

        /// <summary>
        /// Failed checksum.
        /// </summary>
        Bad,

        /// <summary>
        /// Routing transit not detected.
        /// </summary>
        NotFound
    }
}