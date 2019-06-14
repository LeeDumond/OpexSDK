using OpexSDK.Enumerations;

namespace OpexSDK
{
    public class MarkDetect
    {
        public int Index { get; set; }
        public Side Side { get; set; }
        public bool Result { get; set; }
        public string Name { get; set; }
    }
}