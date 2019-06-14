using OpexSDK.Enumerations;

namespace OpexSDK
{
    public class AuditTrail
    {
        public AuditTrailType Type { get; set; }
        public Side Side { get; set; }
        public bool Apply { get; set; }
        public string Text { get; set; }
    }
}