using OpexSDK.Enumerations;

namespace OpexSDK
{
    public class Barcode
    {
        public int Index { get; set; }
        public string Type { get; set; }
        public Side Side { get; set; }
        public string Value { get; set; }
    }
}