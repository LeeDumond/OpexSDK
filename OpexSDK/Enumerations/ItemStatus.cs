namespace OpexSDK.Enumerations
{
    /// <summary>
    ///     Describes the status of a page.
    /// </summary>
    public enum ItemStatus
    {
        /// <summary>
        ///     Item should be processed normally during second pass.
        /// </summary>
        Valid,

        /// <summary>
        ///     Item should be ignored by second pass.
        /// </summary>
        Void,

        /// <summary>
        ///     Item should be ignored by second pass. The saved image is inverted and watermarked "VOID".
        /// </summary>
        VoidMarked
    }
}