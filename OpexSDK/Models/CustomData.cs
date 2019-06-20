namespace OpexSDK.Models
{
    /// <summary>
    /// Information returned by the ScanLink plug-in solicit_custom_data call.
    /// </summary>
    public class CustomData
    {
        internal CustomData()
        {
            
        }

        /// <summary>
        /// Text returned from the ScanLink plug-in solicit_custom_data call custom_data field.
        /// </summary>
        public string Entry { get; internal set; }
    }
}