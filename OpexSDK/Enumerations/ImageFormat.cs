namespace OpexSDK.Enumerations
{
    /// <summary>
    /// Describes the format of an image.
    /// </summary>
    public enum ImageFormat
    {
        /// <summary>
        /// If Image Depth is 1, format is Bitonal, CCITT, Group 4. If Image Depth is 8 or 24, format is raw, uncompressed.
        /// </summary>
        TIFF,

        /// <summary>
        /// Valid only with Image Depth of 8 and 24.
        /// </summary>
        JPEG,

        /// <summary>
        /// Interleaved if Image Depth is 24.
        /// </summary>
        RAW
    }
}