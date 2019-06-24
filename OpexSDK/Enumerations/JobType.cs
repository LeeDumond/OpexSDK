namespace OpexSDK.Enumerations
{
    /// <summary>
    ///     Type of transactions in job.
    /// </summary>
    public enum JobType
    {
        /// <summary>
        ///     One stub, one check
        /// </summary>
        Single,

        /// <summary>
        ///     One or more stubs, one or more checks
        /// </summary>
        Multi,

        /// <summary>
        ///     One stub
        /// </summary>
        StubOnly,

        /// <summary>
        ///     One check
        /// </summary>
        CheckOnly,

        /// <summary>
        ///     One or more stubs, one or more checks, zero or more pages
        /// </summary>
        MultiWithPage,

        /// <summary>
        ///     One or more pages
        /// </summary>
        PageOnly,

        /// <summary>
        ///     Any documents
        /// </summary>
        Unstructured,

        /// <summary>
        ///     Transaction validation determined by customized job settings
        /// </summary>
        Structured
    }
}