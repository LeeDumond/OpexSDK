namespace OpexSDK.Enumerations
{
    /// <summary>
    /// Describes the type of check and the country of origin.
    /// </summary>
    public enum CheckType
    {
        /// <summary>
        /// E13B font, country not determined.
        /// </summary>
        E13B,

        /// <summary>
        /// E13B font, United States check.
        /// </summary>
        US,

        /// <summary>
        /// E13B font, Canadian check.
        /// </summary>
        Canada,

        /// <summary>
        /// CMC7 font.
        /// </summary>
        CMC7,

        /// <summary>
        /// The check type is unknown.
        /// </summary>
        Unknown
    }
}