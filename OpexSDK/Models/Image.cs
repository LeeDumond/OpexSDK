using OpexSDK.Enumerations;

namespace OpexSDK.Models
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
        /// Indicates if the image is a rescan. If true, the image was rescanned; otherwise, the image was not rescanned.
        /// </summary>
        public bool RescanStatus { get; set; }

        /// <summary>
        /// Blank status of an area of the corresponding image side (specified by job settings) associated with the image.
        /// If true, the area was determined to be blank; if false, the area was determined not to be blank; if null, the image was not tested for blank area.
        /// If true, the Filename is expected to be null.
        /// </summary>
        public bool? ScantimeFinalBlankAreaDecision { get; set; }

        /// <summary>
        /// Indicates which side of the page the image was taken from.
        /// </summary>
        public Side Side { get; set; }

        /// <summary>
        /// Specifies whether the image is the full page image, or a partial image ("snippet").
        /// </summary>
        public ImageType Type { get; set; }

        /// <summary>
        /// Specifies the bit depth of the uncompressed image.
        /// </summary>
        public ImageDepth Depth { get; set; }

        /// <summary>
        /// Specifies the format of the saved image.
        /// </summary>
        public ImageFormat Format { get; set; }

        /// <summary>
        /// Image file name combining, as per scanning software job parameters, various pieces of item-wise information, such as Scan Sequence number, snippet/image number, audit trail text and possibly an ordinal (#) to prevent duplications. The default is [Scan Sequence].ext.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// The size of the compressed image file, in bytes, at the time the batch was written.
        /// </summary>
        public long Filesize { get; set; }

        /// <summary>
        /// The dimension of the image in pixels in the horizontal direction. If image Type is Snippet, equals the length of the snippet rectangle.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The dimension of the image in pixels in the vertical direction. If image Type is Snippet, equals the height of the snippet rectangle.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// If Type is Full, this value will be 0. If Type is Snippet, equals the horizontal offset in pixels from the upper left corner of the full image to the upper left corner of the snippet.
        /// </summary>
        public int OffsetLength { get; set; }

        /// <summary>
        /// If Type is Full, this value will be 0. If Type is Snippet, equals the vertical offset in pixels from the upper left corner of the full image to the upper left corner of the snippet.
        /// </summary>
        public int OffsetHeight { get; set; }

        /// <summary>
        /// Image resolution in the horizontal direction, in dots per inch (DPI).
        /// </summary>
        public ImageResolution ResolutionLength { get; set; }

        /// <summary>
        /// Image resolution in the vertical direction, in dots per inch (DPI).
        /// </summary>
        public ImageResolution ResolutionHeight { get; set; }
    }
}