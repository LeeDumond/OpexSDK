namespace OpexSDK
{
    /// <summary>
    /// Contains tag data, which is used to mark an item in the batch for a job-specific reason. 
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// A string associated with how this data became a tag. The string is host specific.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// A string associated with how this data became a tag. The string is host specific.
        /// </summary>
        public string Value { get; set; }
    }
}