using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using OpexSDK.Models;

[assembly: InternalsVisibleTo("OpexSDK.Tests")]

namespace OpexSDK
{
    /// <summary>
    ///     A utility that facilitates the reading of data contained in batch information files (*.oxi) created by OPEX
    ///     scanning devices, along with optional validation against an XSD schema file supplied by the user.
    /// </summary>
    public class BatchReader
    {
        /// <summary>
        ///     Returns an instance of a BatchReader.
        /// </summary>
        /// <param name="batchFilePath">The path to the batch information file.</param>
        /// <param name="schemaFilePath">
        ///     The path to an XSD schema definition file to validate against. By default, this argument
        ///     is null, which means no validation is performed.
        /// </param>
        public BatchReader(string batchFilePath, string schemaFilePath = null) : this(batchFilePath, schemaFilePath,
            new FileSystem())
        {
            if (batchFilePath == null)
            {
                throw new ArgumentNullException(nameof(batchFilePath));
            }

            if (string.IsNullOrWhiteSpace(batchFilePath))
            {
                throw new ArgumentException("Value cannot be empty", nameof(batchFilePath));
            }

            string ext = Path.GetExtension(batchFilePath);

            if (string.IsNullOrEmpty(ext) || !ext.Equals(".oxi", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new NotSupportedException("The file indicated by the supplied path must end in '.oxi'");
            }
        }

        internal BatchReader(string batchFilePath, string schemaFilePath, IFileSystem fileSystem)
        {
            _batchFilePath = batchFilePath;
            _schemaFilePath = schemaFilePath;
            _fileSystem = fileSystem;
            _validationErrors = new List<ValidationEventArgs>();
        }

        private readonly string _batchFilePath;
        private readonly IFileSystem _fileSystem;
        private readonly string _schemaFilePath;
        private readonly IList<ValidationEventArgs> _validationErrors;

        /// <summary>
        ///     The collection of errors, if any, encountered during the validation process.
        ///     NOTE: If this collection is empty, it does not automatically mean that the batch information file is valid. If no
        ///     schema is supplied, data validation will not be performed and this collection will be empty. If the schema is
        ///     supplied but in itself is not valid, those errors will be contained in this collection, though data validation will
        ///     not be performed.
        /// </summary>
        public ReadOnlyCollection<ValidationEventArgs> ValidationErrors =>
            new ReadOnlyCollection<ValidationEventArgs>(_validationErrors);

        /// <summary>
        ///     A method that asynchronously that reads the data contained in the batch information file supplied to the reader. If
        ///     a schema definition file (*.xsd) is also supplied, this method will confirm the validity of the schema, and if it
        ///     is valid, will validate the data file against it.
        /// </summary>
        /// <returns>An instance of Batch containing all the data in the batch information file that was supplied to the reader.</returns>
        public Batch ReadBatch()
        {
            var batch = new Batch();

            using (XmlReader reader = GetXmlReader())
            {
                while (reader.Read())
                {
                    if (reader.MoveToContent() == XmlNodeType.Element && reader.Name.Equals("Batch"))
                    {
                        batch.BaseMachine = reader.GetAttribute("BaseMachine");
                        batch.FormatVersion = reader.GetAttribute("FormatVersion");
                        batch.BatchIdentifier = reader.GetAttribute("BatchIdentifier");
                        batch.DeveloperReserved = reader.GetAttribute("DeveloperReserved");
                        batch.ImageFilePath = reader.GetAttribute("ImageFilePath");
                        batch.JobName = reader.GetAttribute("JobName");
                        batch.JobType = AttributeHelpers.GetJobType(reader.GetAttribute("JobType"));
                        batch.OperatorName = reader.GetAttribute("OperatorName");
                        batch.OperatingMode = AttributeHelpers.GetOperatingMode(reader.GetAttribute("OperatingMode"));
                        batch.StartTime = AttributeHelpers.GetDateTime(reader.GetAttribute("StartTime"));
                        batch.PluginMessage = reader.GetAttribute("PluginMessage");
                        batch.ProcessDate = AttributeHelpers.GetDateTime(reader.GetAttribute("ProcessDate"))?.Date;
                        batch.ReceiveDate = AttributeHelpers.GetDateTime(reader.GetAttribute("ReceiveDate"))?.Date;
                        batch.ScanDevice = reader.GetAttribute("ScanDevice");
                        batch.SoftwareVersion = reader.GetAttribute("SoftwareVersion");
                        batch.TransportId = reader.GetAttribute("TransportId");
                    }

                    if (reader.MoveToContent() == XmlNodeType.Element && reader.Name.Equals("ReferenceID"))
                    {
                        var referenceId = new ReferenceId
                        {
                            Index = AttributeHelpers.GetInt(reader.GetAttribute("Index")),
                            Response = reader.GetAttribute("Response"),
                            Name = reader.GetAttribute("Name")
                        };

                        batch.Add(referenceId);
                    }

                    if (reader.MoveToContent() == XmlNodeType.Element && reader.Name.Equals("EndInfo"))
                    {
                        var endInfo = new EndInfo
                        {
                            EndTime = AttributeHelpers.GetDateTime(reader.GetAttribute("EndTime")),
                            NumPages = AttributeHelpers.GetInt(reader.GetAttribute("NumPages")),
                            NumGroups = AttributeHelpers.GetInt(reader.GetAttribute("NumGroups")),
                            NumTransactions = AttributeHelpers.GetInt(reader.GetAttribute("NumTransactions")),
                            IsModified = AttributeHelpers.GetBooleanFromTrueFalse(reader.GetAttribute("IsModified"))
                        };

                        batch.EndInfo = endInfo;
                    }

                    if (reader.MoveToContent() == XmlNodeType.Element && reader.Name.Equals("Transaction"))
                    {
                        var transaction = new Transaction
                        {
                            TransactionId = AttributeHelpers.GetInt(reader.GetAttribute("TransactionID"))
                        };

                        using (XmlReader groupReader = reader.ReadSubtree())
                        {
                            while (groupReader.Read())
                            {
                                if (groupReader.MoveToContent() == XmlNodeType.Element &&
                                    groupReader.Name.Equals("Group"))
                                {
                                    var group = new Group
                                    {
                                        GroupId = AttributeHelpers.GetInt(groupReader.GetAttribute("GroupID"))
                                    };

                                    using (XmlReader pageReader = groupReader.ReadSubtree())
                                    {
                                        while (pageReader.Read())
                                        {
                                            if (pageReader.MoveToContent() == XmlNodeType.Element &&
                                                pageReader.Name.Equals("Page"))
                                            {
                                                var page = new Page
                                                {
                                                    DocumentLocator =
                                                        AttributeHelpers.GetInt(
                                                            pageReader.GetAttribute("DocumentLocator")),
                                                    BatchSequence =
                                                        AttributeHelpers.GetInt(
                                                            pageReader.GetAttribute("BatchSequence")),
                                                    TransactionSequence =
                                                        AttributeHelpers.GetInt(
                                                            pageReader.GetAttribute("TransactionSequence")),
                                                    GroupSequence =
                                                        AttributeHelpers.GetInt(
                                                            pageReader.GetAttribute("GroupSequence")),
                                                    ScanSequence =
                                                        AttributeHelpers.GetInt(
                                                            pageReader.GetAttribute("ScanSequence")),
                                                    ScanTime =
                                                        AttributeHelpers.GetDateTime(
                                                            pageReader.GetAttribute("ScanTime")),
                                                    ItemStatus =
                                                        AttributeHelpers.GetItemStatus(
                                                            pageReader.GetAttribute("ItemStatus")),
                                                    IsVirtual =
                                                        AttributeHelpers.GetBooleanFromYesNo(
                                                            pageReader.GetAttribute("IsVirtual")),
                                                    PageType =
                                                        AttributeHelpers.GetPageType(
                                                            pageReader.GetAttribute("PageType")),
                                                    PageName = pageReader.GetAttribute("PageName"),
                                                    SubPageName = pageReader.GetAttribute("SubPageName"),
                                                    OperatorSelect =
                                                        AttributeHelpers.GetBooleanFromYesNo(
                                                            pageReader.GetAttribute("OperatorSelect")),
                                                    Bin = pageReader.GetAttribute("Bin"),
                                                    AverageThickness =
                                                        AttributeHelpers.GetFloat(
                                                            pageReader.GetAttribute("AverageThickness")),
                                                    EnvelopeDetect =
                                                        AttributeHelpers.GetEnvelopeDetect(
                                                            pageReader.GetAttribute("EnvelopeDetect")),
                                                    SkewDegrees =
                                                        AttributeHelpers.GetFloat(
                                                            pageReader.GetAttribute("SkewDegrees")),
                                                    DeskewStatus =
                                                        AttributeHelpers.GetDeskewStatus(
                                                            pageReader.GetAttribute("DeskewStatus")),
                                                    FrontStreakDetectStatus =
                                                        AttributeHelpers.GetFrontStreakDetectStatus(
                                                            pageReader.GetAttribute("FrontStreakDetectStatus")),
                                                    BackStreakDetectStatus =
                                                        AttributeHelpers.GetBackStreakDetectStatus(
                                                            pageReader.GetAttribute("BackStreakDetectStatus")),
                                                    PlugInPageMessage =
                                                        pageReader.GetAttribute("PlugInPageMessage"),
                                                    Length = pageReader.GetAttribute("Length"),
                                                    Height = pageReader.GetAttribute("Height")
                                                };

                                                using (XmlReader pageSubReader = pageReader.ReadSubtree())
                                                {
                                                    while (pageSubReader.Read())
                                                    {
                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("Image"))
                                                        {
                                                            var image = new Image
                                                            {
                                                                Index =
                                                                    AttributeHelpers.GetInt(
                                                                        pageSubReader.GetAttribute("Index")),
                                                                RescanStatus =
                                                                    AttributeHelpers.GetRescanStatus(
                                                                        pageSubReader.GetAttribute("RescanStatus")),
                                                                ScantimeFinalBlankAreaDecision =
                                                                    AttributeHelpers
                                                                        .GetScantimeFinalBlankAreaDecision(
                                                                            pageSubReader.GetAttribute(
                                                                                "ScantimeFinalBlankAreaDecision")),
                                                                Side =
                                                                    AttributeHelpers.GetSide(
                                                                        pageSubReader.GetAttribute("Side")),
                                                                Type =
                                                                    AttributeHelpers.GetImageType(
                                                                        pageSubReader.GetAttribute("Type")),
                                                                Depth =
                                                                    AttributeHelpers.GetImageDepth(
                                                                        pageSubReader.GetAttribute("Depth")),
                                                                Format =
                                                                    AttributeHelpers.GetImageFormat(
                                                                        pageSubReader.GetAttribute("Format")),
                                                                Filename = pageSubReader.GetAttribute("Filename"),
                                                                Filesize =
                                                                    AttributeHelpers.GetLong(
                                                                        pageSubReader.GetAttribute("Filesize")),
                                                                Length =
                                                                    AttributeHelpers.GetInt(
                                                                        pageSubReader.GetAttribute("Length")),
                                                                Height =
                                                                    AttributeHelpers.GetInt(
                                                                        pageSubReader.GetAttribute("Height")),
                                                                OffsetLength =
                                                                    AttributeHelpers.GetInt(
                                                                        pageSubReader.GetAttribute("OffsetLength")),
                                                                OffsetHeight =
                                                                    AttributeHelpers.GetInt(
                                                                        pageSubReader.GetAttribute("OffsetHeight")),
                                                                ResolutionLength =
                                                                    AttributeHelpers.GetImageResolution(
                                                                        pageSubReader.GetAttribute(
                                                                            "ResolutionLength")),
                                                                ResolutionHeight =
                                                                    AttributeHelpers.GetImageResolution(
                                                                        pageSubReader.GetAttribute(
                                                                            "ResolutionHeight"))
                                                            };

                                                            page.Add(image);
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("CustomData"))
                                                        {
                                                            var customData = new CustomData
                                                            {
                                                                Entry = pageSubReader.GetAttribute("Entry")
                                                            };

                                                            page.CustomData = customData;
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("Micr"))
                                                        {
                                                            var micr = new Micr
                                                            {
                                                                Status =
                                                                    AttributeHelpers.GetMicrStatus(
                                                                        pageSubReader.GetAttribute("Status")),
                                                                RtStatus =
                                                                    AttributeHelpers.GetRtStatus(
                                                                        pageSubReader.GetAttribute("RtStatus")),
                                                                CheckType =
                                                                    AttributeHelpers.GetCheckType(
                                                                        pageSubReader.GetAttribute("CheckType")),
                                                                Side = AttributeHelpers.GetSide(
                                                                    pageSubReader.GetAttribute("Side")),
                                                                Value = pageSubReader.GetAttribute("Value")
                                                            };

                                                            page.Add(micr);
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("Ocr"))
                                                        {
                                                            var ocr = new Ocr
                                                            {
                                                                Index = AttributeHelpers.GetInt(
                                                                    pageSubReader.GetAttribute("Index")),
                                                                Side = AttributeHelpers.GetSide(
                                                                    pageSubReader.GetAttribute("Side")),
                                                                Value = pageSubReader.GetAttribute("Value"),
                                                                Name = pageSubReader.GetAttribute("Name")
                                                            };

                                                            page.Add(ocr);
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("Barcode"))
                                                        {
                                                            var barcode = new Barcode
                                                            {
                                                                Index = AttributeHelpers.GetInt(
                                                                    pageSubReader.GetAttribute("Index")),
                                                                Type = pageSubReader.GetAttribute("Type"),
                                                                Side = AttributeHelpers.GetSide(
                                                                    pageSubReader.GetAttribute("Side")),
                                                                Value = pageSubReader.GetAttribute("Value")
                                                            };

                                                            page.Add(barcode);
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("MarkDetect"))
                                                        {
                                                            var markDetect = new MarkDetect
                                                            {
                                                                Index = AttributeHelpers.GetInt(
                                                                    pageSubReader.GetAttribute("Index")),
                                                                Side =
                                                                    AttributeHelpers.GetSide(
                                                                        pageSubReader.GetAttribute("Side")),
                                                                Result = AttributeHelpers.GetBooleanFromYesNo(
                                                                    pageSubReader.GetAttribute("Result")),
                                                                Name = pageSubReader.GetAttribute("Name")
                                                            };

                                                            page.Add(markDetect);
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("AuditTrail"))
                                                        {
                                                            var auditTrail = new AuditTrail
                                                            {
                                                                Type = AttributeHelpers.GetAuditTrailType(
                                                                    pageSubReader.GetAttribute("Type")),
                                                                Apply =
                                                                    AttributeHelpers.GetBooleanFromTrueFalse(
                                                                        pageSubReader.GetAttribute("Apply")),
                                                                Side = AttributeHelpers.GetSide(
                                                                    pageSubReader.GetAttribute("Side")),
                                                                Text = pageSubReader.GetAttribute("Text")
                                                            };

                                                            page.Add(auditTrail);
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("ReferenceID"))
                                                        {
                                                            var referenceId = new ReferenceId
                                                            {
                                                                Index =
                                                                    AttributeHelpers.GetInt(
                                                                        pageSubReader.GetAttribute("Index")),
                                                                Response = pageSubReader.GetAttribute("Response"),
                                                                Name = pageSubReader.GetAttribute("Name")
                                                            };

                                                            page.Add(referenceId);
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("Tag"))
                                                        {
                                                            var tag = new Tag
                                                            {
                                                                Source = pageSubReader.GetAttribute("Source"),
                                                                Value = pageSubReader.GetAttribute("Value")
                                                            };

                                                            page.Add(tag);
                                                        }
                                                    }
                                                }

                                                @group.Add(page);
                                            }
                                        }
                                    }

                                    transaction.Add(@group);
                                }
                            }
                        }

                        batch.Add(transaction);
                    }
                }
            }

            return batch;
        }

        /// <summary>
        ///     A method that asynchronously that reads the data contained in the batch information file supplied to the reader. If
        ///     a schema definition file (*.xsd) is also supplied, this method will confirm the validity of the schema, and if it
        ///     is valid, will validate the data file against it.
        /// </summary>
        /// <returns>An instance of Batch containing all the data in the batch information file that was supplied to the reader.</returns>
        public async Task<Batch> ReadBatchAsync()
        {
            var batch = new Batch();

            using (XmlReader reader = GetXmlReader(true))
            {
                while (await reader.ReadAsync())
                {
                    if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.Name.Equals("Batch"))
                    {
                        batch.BaseMachine = reader.GetAttribute("BaseMachine");
                        batch.FormatVersion = reader.GetAttribute("FormatVersion");
                        batch.BatchIdentifier = reader.GetAttribute("BatchIdentifier");
                        batch.DeveloperReserved = reader.GetAttribute("DeveloperReserved");
                        batch.ImageFilePath = reader.GetAttribute("ImageFilePath");
                        batch.JobName = reader.GetAttribute("JobName");
                        batch.JobType = AttributeHelpers.GetJobType(reader.GetAttribute("JobType"));
                        batch.OperatorName = reader.GetAttribute("OperatorName");
                        batch.OperatingMode = AttributeHelpers.GetOperatingMode(reader.GetAttribute("OperatingMode"));
                        batch.StartTime = AttributeHelpers.GetDateTime(reader.GetAttribute("StartTime"));
                        batch.PluginMessage = reader.GetAttribute("PluginMessage");
                        batch.ProcessDate = AttributeHelpers.GetDateTime(reader.GetAttribute("ProcessDate"))?.Date;
                        batch.ReceiveDate = AttributeHelpers.GetDateTime(reader.GetAttribute("ReceiveDate"))?.Date;
                        batch.ScanDevice = reader.GetAttribute("ScanDevice");
                        batch.SoftwareVersion = reader.GetAttribute("SoftwareVersion");
                        batch.TransportId = reader.GetAttribute("TransportId");
                    }

                    if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.Name.Equals("ReferenceID"))
                    {
                        var referenceId = new ReferenceId
                        {
                            Index = AttributeHelpers.GetInt(reader.GetAttribute("Index")),
                            Response = reader.GetAttribute("Response"),
                            Name = reader.GetAttribute("Name")
                        };

                        batch.Add(referenceId);
                    }

                    if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.Name.Equals("EndInfo"))
                    {
                        var endInfo = new EndInfo
                        {
                            EndTime = AttributeHelpers.GetDateTime(reader.GetAttribute("EndTime")),
                            NumPages = AttributeHelpers.GetInt(reader.GetAttribute("NumPages")),
                            NumGroups = AttributeHelpers.GetInt(reader.GetAttribute("NumGroups")),
                            NumTransactions = AttributeHelpers.GetInt(reader.GetAttribute("NumTransactions")),
                            IsModified = AttributeHelpers.GetBooleanFromTrueFalse(reader.GetAttribute("IsModified"))
                        };

                        batch.EndInfo = endInfo;
                    }

                    if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.Name.Equals("Transaction"))
                    {
                        var transaction = new Transaction
                        {
                            TransactionId = AttributeHelpers.GetInt(reader.GetAttribute("TransactionID"))
                        };

                        using (XmlReader groupReader = reader.ReadSubtree())
                        {
                            while (await groupReader.ReadAsync())
                            {
                                if (await groupReader.MoveToContentAsync() == XmlNodeType.Element &&
                                    groupReader.Name.Equals("Group"))
                                {
                                    var group = new Group
                                    {
                                        GroupId = AttributeHelpers.GetInt(groupReader.GetAttribute("GroupID"))
                                    };

                                    using (XmlReader pageReader = groupReader.ReadSubtree())
                                    {
                                        while (await pageReader.ReadAsync())
                                        {
                                            if (await pageReader.MoveToContentAsync() == XmlNodeType.Element &&
                                                pageReader.Name.Equals("Page"))
                                            {
                                                var page = new Page
                                                {
                                                    DocumentLocator =
                                                        AttributeHelpers.GetInt(
                                                            pageReader.GetAttribute("DocumentLocator")),
                                                    BatchSequence =
                                                        AttributeHelpers.GetInt(
                                                            pageReader.GetAttribute("BatchSequence")),
                                                    TransactionSequence =
                                                        AttributeHelpers.GetInt(
                                                            pageReader.GetAttribute("TransactionSequence")),
                                                    GroupSequence =
                                                        AttributeHelpers.GetInt(
                                                            pageReader.GetAttribute("GroupSequence")),
                                                    ScanSequence =
                                                        AttributeHelpers.GetInt(
                                                            pageReader.GetAttribute("ScanSequence")),
                                                    ScanTime =
                                                        AttributeHelpers.GetDateTime(
                                                            pageReader.GetAttribute("ScanTime")),
                                                    ItemStatus =
                                                        AttributeHelpers.GetItemStatus(
                                                            pageReader.GetAttribute("ItemStatus")),
                                                    IsVirtual =
                                                        AttributeHelpers.GetBooleanFromYesNo(
                                                            pageReader.GetAttribute("IsVirtual")),
                                                    PageType =
                                                        AttributeHelpers.GetPageType(
                                                            pageReader.GetAttribute("PageType")),
                                                    PageName = pageReader.GetAttribute("PageName"),
                                                    SubPageName = pageReader.GetAttribute("SubPageName"),
                                                    OperatorSelect =
                                                        AttributeHelpers.GetBooleanFromYesNo(
                                                            pageReader.GetAttribute("OperatorSelect")),
                                                    Bin = pageReader.GetAttribute("Bin"),
                                                    AverageThickness =
                                                        AttributeHelpers.GetFloat(
                                                            pageReader.GetAttribute("AverageThickness")),
                                                    EnvelopeDetect =
                                                        AttributeHelpers.GetEnvelopeDetect(
                                                            pageReader.GetAttribute("EnvelopeDetect")),
                                                    SkewDegrees =
                                                        AttributeHelpers.GetFloat(
                                                            pageReader.GetAttribute("SkewDegrees")),
                                                    DeskewStatus =
                                                        AttributeHelpers.GetDeskewStatus(
                                                            pageReader.GetAttribute("DeskewStatus")),
                                                    FrontStreakDetectStatus =
                                                        AttributeHelpers.GetFrontStreakDetectStatus(
                                                            pageReader.GetAttribute("FrontStreakDetectStatus")),
                                                    BackStreakDetectStatus =
                                                        AttributeHelpers.GetBackStreakDetectStatus(
                                                            pageReader.GetAttribute("BackStreakDetectStatus")),
                                                    PlugInPageMessage =
                                                        pageReader.GetAttribute("PlugInPageMessage"),
                                                    Length = pageReader.GetAttribute("Length"),
                                                    Height = pageReader.GetAttribute("Height")
                                                };

                                                using (XmlReader pageSubReader = pageReader.ReadSubtree())
                                                {
                                                    while (await pageSubReader.ReadAsync())
                                                    {
                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element && pageSubReader.Name.Equals("Image"))
                                                        {
                                                            var image = new Image
                                                            {
                                                                Index =
                                                                    AttributeHelpers.GetInt(
                                                                        pageSubReader.GetAttribute("Index")),
                                                                RescanStatus =
                                                                    AttributeHelpers.GetRescanStatus(
                                                                        pageSubReader.GetAttribute("RescanStatus")),
                                                                ScantimeFinalBlankAreaDecision =
                                                                    AttributeHelpers
                                                                        .GetScantimeFinalBlankAreaDecision(
                                                                            pageSubReader.GetAttribute(
                                                                                "ScantimeFinalBlankAreaDecision")),
                                                                Side =
                                                                    AttributeHelpers.GetSide(
                                                                        pageSubReader.GetAttribute("Side")),
                                                                Type =
                                                                    AttributeHelpers.GetImageType(
                                                                        pageSubReader.GetAttribute("Type")),
                                                                Depth =
                                                                    AttributeHelpers.GetImageDepth(
                                                                        pageSubReader.GetAttribute("Depth")),
                                                                Format =
                                                                    AttributeHelpers.GetImageFormat(
                                                                        pageSubReader.GetAttribute("Format")),
                                                                Filename = pageSubReader.GetAttribute("Filename"),
                                                                Filesize =
                                                                    AttributeHelpers.GetLong(
                                                                        pageSubReader.GetAttribute("Filesize")),
                                                                Length =
                                                                    AttributeHelpers.GetInt(
                                                                        pageSubReader.GetAttribute("Length")),
                                                                Height =
                                                                    AttributeHelpers.GetInt(
                                                                        pageSubReader.GetAttribute("Height")),
                                                                OffsetLength =
                                                                    AttributeHelpers.GetInt(
                                                                        pageSubReader.GetAttribute("OffsetLength")),
                                                                OffsetHeight =
                                                                    AttributeHelpers.GetInt(
                                                                        pageSubReader.GetAttribute("OffsetHeight")),
                                                                ResolutionLength =
                                                                    AttributeHelpers.GetImageResolution(
                                                                        pageSubReader.GetAttribute(
                                                                            "ResolutionLength")),
                                                                ResolutionHeight =
                                                                    AttributeHelpers.GetImageResolution(
                                                                        pageSubReader.GetAttribute(
                                                                            "ResolutionHeight"))
                                                            };

                                                            page.Add(image);
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("CustomData"))
                                                        {
                                                            var customData = new CustomData
                                                            {
                                                                Entry = pageSubReader.GetAttribute("Entry")
                                                            };

                                                            page.CustomData = customData;
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element && pageSubReader.Name.Equals("Micr"))
                                                        {
                                                            var micr = new Micr
                                                            {
                                                                Status =
                                                                    AttributeHelpers.GetMicrStatus(
                                                                        pageSubReader.GetAttribute("Status")),
                                                                RtStatus =
                                                                    AttributeHelpers.GetRtStatus(
                                                                        pageSubReader.GetAttribute("RtStatus")),
                                                                CheckType =
                                                                    AttributeHelpers.GetCheckType(
                                                                        pageSubReader.GetAttribute("CheckType")),
                                                                Side = AttributeHelpers.GetSide(
                                                                    pageSubReader.GetAttribute("Side")),
                                                                Value = pageSubReader.GetAttribute("Value")
                                                            };

                                                            page.Add(micr);
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element && pageSubReader.Name.Equals("Ocr"))
                                                        {
                                                            var ocr = new Ocr
                                                            {
                                                                Index = AttributeHelpers.GetInt(
                                                                    pageSubReader.GetAttribute("Index")),
                                                                Side = AttributeHelpers.GetSide(
                                                                    pageSubReader.GetAttribute("Side")),
                                                                Value = pageSubReader.GetAttribute("Value"),
                                                                Name = pageSubReader.GetAttribute("Name")
                                                            };

                                                            page.Add(ocr);
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element && pageSubReader.Name.Equals("Barcode"))
                                                        {
                                                            var barcode = new Barcode
                                                            {
                                                                Index = AttributeHelpers.GetInt(
                                                                    pageSubReader.GetAttribute("Index")),
                                                                Type = pageSubReader.GetAttribute("Type"),
                                                                Side = AttributeHelpers.GetSide(
                                                                    pageSubReader.GetAttribute("Side")),
                                                                Value = pageSubReader.GetAttribute("Value")
                                                            };

                                                            page.Add(barcode);
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("MarkDetect"))
                                                        {
                                                            var markDetect = new MarkDetect
                                                            {
                                                                Index = AttributeHelpers.GetInt(
                                                                    pageSubReader.GetAttribute("Index")),
                                                                Side =
                                                                    AttributeHelpers.GetSide(
                                                                        pageSubReader.GetAttribute("Side")),
                                                                Result = AttributeHelpers.GetBooleanFromYesNo(
                                                                    pageSubReader.GetAttribute("Result")),
                                                                Name = pageSubReader.GetAttribute("Name")
                                                            };

                                                            page.Add(markDetect);
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("AuditTrail"))
                                                        {
                                                            var auditTrail = new AuditTrail
                                                            {
                                                                Type = AttributeHelpers.GetAuditTrailType(
                                                                    pageSubReader.GetAttribute("Type")),
                                                                Apply =
                                                                    AttributeHelpers.GetBooleanFromTrueFalse(
                                                                        pageSubReader.GetAttribute("Apply")),
                                                                Side = AttributeHelpers.GetSide(
                                                                    pageSubReader.GetAttribute("Side")),
                                                                Text = pageSubReader.GetAttribute("Text")
                                                            };

                                                            page.Add(auditTrail);
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("ReferenceID"))
                                                        {
                                                            var referenceId = new ReferenceId
                                                            {
                                                                Index =
                                                                    AttributeHelpers.GetInt(
                                                                        pageSubReader.GetAttribute("Index")),
                                                                Response = pageSubReader.GetAttribute("Response"),
                                                                Name = pageSubReader.GetAttribute("Name")
                                                            };

                                                            page.Add(referenceId);
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element && pageSubReader.Name.Equals("Tag"))
                                                        {
                                                            var tag = new Tag
                                                            {
                                                                Source = pageSubReader.GetAttribute("Source"),
                                                                Value = pageSubReader.GetAttribute("Value")
                                                            };

                                                            page.Add(tag);
                                                        }
                                                    }
                                                }

                                                group.Add(page);
                                            }
                                        }
                                    }

                                    transaction.Add(group);
                                }
                            }
                        }

                        batch.Add(transaction);
                    }
                }
            }

            return batch;
        }

        private XmlReader GetXmlReader(bool async = false)
        {
            Stream batchStream = _fileSystem.FileStream.Create(_batchFilePath, FileMode.Open, FileAccess.Read);

            var settings = new XmlReaderSettings
            {
                Async = async,
                IgnoreComments = true,
                IgnoreWhitespace = true,
                IgnoreProcessingInstructions = true
            };

            if (_schemaFilePath != null)
            {
                Stream schemaStream = _fileSystem.FileStream.Create(_schemaFilePath, FileMode.Open, FileAccess.Read);
                XmlSchema schema = XmlSchema.Read(schemaStream, ValidationCallBack);
                if (schema != null)
                {
                    settings.Schemas.Add(schema);
                    settings.ValidationType = ValidationType.Schema;
                    settings.ValidationEventHandler += ValidationCallBack;
                }
            }

            return XmlReader.Create(batchStream, settings);
        }

        private void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            _validationErrors.Add(e);
        }
    }
}