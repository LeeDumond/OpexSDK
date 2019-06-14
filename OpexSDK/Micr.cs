using OpexSDK.Enumerations;

namespace OpexSDK
{
    public class Micr
    {
        public MicrStatus Status { get; set; }
        public RtStatus RtStatus { get; set; }
        public CheckType CheckType { get; set; }
        public Side Side { get; set; }
        public string Value { get; set; }
    }
}