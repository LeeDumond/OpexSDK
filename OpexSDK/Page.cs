using System;
using System.Collections.Generic;
using OpexSDK.Enumerations;

namespace OpexSDK
{
    public class Page
    {
        public int DocumentLocator { get; set; }
        public int BatchSequence { get; set; }
        public int TransactionSequence { get; set; }
        public int GroupSequence { get; set; }
        public int ScanSequence { get; set; }
        public DateTime ScanTime { get; set; }
        public ItemStatus ItemStatus { get; set; }
        public bool IsVirtual { get; set; }
        public PageType PageType { get; set; }
        public string PageName { get; set; }
        public string SubPageName { get; set; }
        public bool OperatorSelect { get; set; }
        public string Bin { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public bool? EnvelopeDetect { get; set; }
        public float AverageThickness { get; set; }
        public float? SkewDegrees { get; set; }
        public bool? DeskewStatus { get; set; }
        public bool? FrontStreakDetectStatus { get; set; }
        public bool? BackStreakDetectStatus { get; set; }
        public string PluginPageMessage { get; set; }

        public ICollection<Image> Images { get; set; }
        public ICollection<Micr> Micrs { get; set; }
        public ICollection<Ocr> Ocrs { get; set; }
        public ICollection<Barcode> Barcodes { get; set; }
        public ICollection<MarkDetect> MarkDetects { get; set; }
        public ICollection<AuditTrail> AuditTrails { get; set; }
        public ICollection<ReferenceId> ReferenceIds { get; set; }
        public ICollection<CustomData> CustomDatas { get; set; }
        public ICollection<Tag> Tags { get; set; }
        
    }
}