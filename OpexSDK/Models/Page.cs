using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using OpexSDK.Enumerations;

namespace OpexSDK.Models
{
    /// <summary>
    /// Represents a single, physical piece of paper that was scanned.
    /// </summary>
    public class Page
    {
        private readonly IList<Image> _images;
        private readonly IList<Micr> _micrs;
        private readonly IList<Ocr> _ocrs;
        private readonly IList<Barcode> _barcodes;
        private readonly IList<MarkDetect> _markDetects;
        private readonly IList<AuditTrail> _auditTrails;
        private readonly IList<ReferenceId> _referenceIds;
        private readonly IList<Tag> _tags;

        internal Page()
        {
            _auditTrails = new List<AuditTrail>();
            _barcodes = new List<Barcode>();
            _images = new List<Image>();
            _markDetects = new List<MarkDetect>();
            _micrs = new List<Micr>();
            _ocrs = new List<Ocr>();
            _referenceIds = new List<ReferenceId>();
            _tags = new List<Tag>();
        }

        /// <summary>
        /// A 1-based number that records each and every item’s place in the batch at scan time. If an item is rescanned, the original DocumentLocator is retained.
        /// </summary>
        public int? DocumentLocator { get; internal set; }

        /// <summary>
        /// Sequence number in batch. Batch tickets have a Sequence Number of zero, The Sequence Number of the first non-batch ticket page may be 1 or another larger number as defined in the job file.
        /// </summary>
        public int? BatchSequence { get; internal set; }

        /// <summary>
        /// Sequence number of page in transaction, starting with 1.
        /// </summary>
        public int? TransactionSequence { get; internal set; }

        /// <summary>
        /// Sequence number of page in group, starting with 1.
        /// </summary>
        public int? GroupSequence { get; internal set; }

        /// <summary>
        /// Unique identifier corresponding to page scan sequence. Sometimes this equals BatchSequence Number. However, in any operation that can cause the image to be reprocessed, such as if a page is deleted, rescanned, rotated, or voided, the BatchSequence number remains the same for the page, but ScanSequence number changes.
        /// </summary>
        public int? ScanSequence { get; internal set; }

        /// <summary>
        /// Scan date and time, i.e., when item was scanned. Uses date set in OPEX Scanning Device PC.
        /// </summary>
        public DateTime? ScanTime { get; internal set; }

        /// <summary>
        /// The status of the page.
        /// </summary>
        public ItemStatus? ItemStatus { get; internal set; }

        /// <summary>
        /// Indicates whether or not this is a simulated item created from loaded front and back images and a simulated Magnetic MICR line. If false, the item is a 'normal' item created by scanning an actual piece of paper; otherwise, the item was not created by scanning a piece of paper, but is a simulated piece resulting from loaded front and back images and a simulated Magnetic MICR line.
        /// </summary>
        public bool? IsVirtual { get; internal set; }

        /// <summary>
        /// Indicates the type of page.
        /// </summary>
        public PageType? PageType { get; internal set; }

        /// <summary>
        /// General identifier assigned to page, specified during job setup.
        /// </summary>
        public string PageName { get; internal set; }

        /// <summary>
        /// Specific identifier assigned to page, specified during job setup and/or by the operator during runtime. This is used for sub-classification of PageTypes. Multiple SubPageNames may have the same PageName.
        /// </summary>
        public string SubPageName { get; internal set; }

        /// <summary>
        /// Indicates whether or not the operator specified the PageType. If true; the PageType was specified by the operator; otherwise, the PageType was determined by the system.
        /// </summary>
        public bool? OperatorSelect { get; internal set; }

        /// <summary>
        /// Bin destination in output stacker.
        /// </summary>
        public string Bin { get; internal set; }

        /// <summary>
        /// String representation of the length of the piece in either inches or centimeters.
        /// </summary>
        public string Length { get; internal set; }

        /// <summary>
        /// String representation of the length of the piece in either inches or centimeters.
        /// </summary>
        public string Height { get; internal set; }

        /// <summary>
        /// Indicator whether piece is an envelope based on thickness profile. If true, envelope was detected; if false, envelope was not detected; if null, envelope thickness detection was disabled.
        /// </summary>
        public EnvelopeDetect? EnvelopeDetect { get; internal set; }

        /// <summary>
        /// Average thickness in "thickness units" where 1 thickness unit equals approximately 0.004 inches. Single pieces of "average" thickness should have a reading of about 1.00.
        /// </summary>
        public float? AverageThickness { get; internal set; }

        /// <summary>
        /// Item skew detected in degrees. Records the skew of the front image. If null, it indicates that skew was not calculated. This is meant for diagnostic purposes only and records the skew of the actual item on the track. It does not necessarily correspond to the images saved in the batch, as they may or may not have been deskewed.
        /// </summary>
        public float? SkewDegrees { get; internal set; }

        /// <summary>
        /// Indicates any deskewing that was applied. If true, deskew was enabled and the image was deskewed; if false, deskew was enabled but the image was not deskewed; if null, deskew was disabled.
        /// </summary>
        public DeskewStatus? DeskewStatus { get; internal set; }

        /// <summary>
        /// Indicates the detection of a streak in the front of the scan. If true, a streak was detected; if false, a streak was not detected; if null, no streak detection was performed.
        /// </summary>
        public FrontStreakDetectStatus? FrontStreakDetectStatus { get; internal set; }

        /// <summary>
        /// Indicates the detection of a streak in the back of the scan. If true, a streak was detected; if false, a streak was not detected; if null, no streak detection was performed.
        /// </summary>
        public BackStreakDetectStatus? BackStreakDetectStatus { get; internal set; }

        /// <summary>
        /// Text message string generated by an optional software plug-in, such as Open IEM.
        /// </summary>
        public string PlugInPageMessage { get; internal set; }

        /// <summary>
        /// One if using ScanLink and the plug-in sets custom data.
        /// </summary>
        public CustomData CustomData { get; internal set; }

        /// <summary>
        /// One for each image for the page.
        /// </summary>
        public ReadOnlyCollection<Image> Images => new ReadOnlyCollection<Image>(_images);

        /// <summary>
        /// One for each MICR read performed. Typically available only for checks and batch tickets.
        /// </summary>
        public ReadOnlyCollection<Micr> Micrs => new ReadOnlyCollection<Micr>(_micrs);

        /// <summary>
        /// One for each OCR read performed.
        /// </summary>
        public ReadOnlyCollection<Ocr> Ocrs => new ReadOnlyCollection<Ocr>(_ocrs);

        /// <summary>
        /// One for each Barcode read performed.
        /// </summary>
        public ReadOnlyCollection<Barcode> Barcodes => new ReadOnlyCollection<Barcode>(_barcodes);

        /// <summary>
        /// One for each Mark Detect performed.
        /// </summary>
        public ReadOnlyCollection<MarkDetect> MarkDetects => new ReadOnlyCollection<MarkDetect>(_markDetects);

        /// <summary>
        /// One for each Audit Trail performed
        /// </summary>
        public ReadOnlyCollection<AuditTrail> AuditTrails => new ReadOnlyCollection<AuditTrail>(_auditTrails);

        /// <summary>
        /// One for each ReferenceID configured in the pagetype.
        /// </summary>
        public ReadOnlyCollection<ReferenceId> ReferenceIds => new ReadOnlyCollection<ReferenceId>(_referenceIds);

        /// <summary>
        /// One for each event tied to a tag in the loading application that occurs while scanning the item.
        /// </summary>
        public ReadOnlyCollection <Tag> Tags => new ReadOnlyCollection<Tag>(_tags);

        internal void Add(Image image)
        {
            _images.Add(image);
        }

        internal void Add(Micr micr)
        {
            _micrs.Add(micr);
        }

        internal void Add(Ocr ocr)
        {
            _ocrs.Add(ocr);
        }

        internal void Add(Barcode barcode)
        {
            _barcodes.Add(barcode);
        }

        internal void Add(MarkDetect markDetect)
        {
            _markDetects.Add(markDetect);
        }

        internal void Add(AuditTrail auditTrail)
        {
            _auditTrails.Add(auditTrail);
        }

        internal void Add(ReferenceId referenceId)
        {
            _referenceIds.Add(referenceId);
        }

        internal void Add(Tag tag)
        {
            _tags.Add(tag);
        }
    }
}