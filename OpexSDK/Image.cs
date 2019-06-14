using OpexSDK.Enumerations;

namespace OpexSDK
{
    public class Image
    {
        public int Index { get; set; }
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