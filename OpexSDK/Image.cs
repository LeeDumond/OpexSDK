using OpexSDK.Enumerations;

namespace OpexSDK
{
    /// <summary>
    /// Contains information about a single image.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// The index of the Image, and (on AS36xx, AS7200, DS2200 or Falcon) its corresponding snippet definition in the job used to create the batch. Values are 1-indexed. (i.e., "1" is the first index.)
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool RescanStatus { get; set; }
        public bool? ScantimeFinalBlankAreaDecision { get; set; }
        public Side Side { get; set; }
        public ImageType Type { get; set; }
        public ImageDepth Depth { get; set; }
        public ImageFormat Format { get; set; }
        public string Filename { get; set; }
        public long Filesize { get; set; }
        public int Length { get; set; }
        public int Height { get; set; }
        public int OffsetLength { get; set; }
        public int OffsetHeight { get; set; }
        public ImageResolution ResolutionLength { get; set; }
        public ImageResolution ResolutionHeight { get; set; }
    }
}