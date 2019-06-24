using System;
using System.Diagnostics;
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
    public class BatchReader
    {
        private readonly string _batchFilePath;
        private readonly string _schemaUri;
        private readonly IFileSystem _fileSystem;

        public BatchReader(string batchFilePath) : this(batchFilePath, new FileSystem())
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

        //todo implement sync reader
        //todo finish docs
        //todo validate against multiple schemas

        internal BatchReader(string batchFilePath, IFileSystem fileSystem)
        {
            _batchFilePath = batchFilePath;
            //_schemaUri = schemaUri;
            _fileSystem = fileSystem;
        }

        public async Task<Batch> ReadBatchAsync()
        {
            var batch = new Batch();

            Stream stream = _fileSystem.FileStream.Create(_batchFilePath, FileMode.Open, FileAccess.Read);

            var settings = new XmlReaderSettings
            {
                Async = true,
                ValidationType = ValidationType.Schema,
                IgnoreComments = true,
                IgnoreWhitespace = true,
                IgnoreProcessingInstructions = true
            };

            settings.Schemas.Add(null, "oxi1_60.xsd");
            settings.ValidationEventHandler += ValidationCallBack;

            using (XmlReader reader = XmlReader.Create(stream, settings))
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

                    if (await reader.MoveToContentAsync() == XmlNodeType.Element &&
                        reader.Name.Equals("Transaction"))
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
                                                                            "ResolutionHeight")),
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

        private void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                Debug.WriteLine("Warning: Matching schema not found.  No validation occurred." + e.Message);
            }
            else
            {
                Debug.WriteLine($"{e.Message} Line: {e.Exception.LineNumber}, Position: {e.Exception.LinePosition}");
            }
        }
    }
}