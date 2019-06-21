using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using OpexSDK.Models;

[assembly: InternalsVisibleTo("OpexSDK.Tests")]

namespace OpexSDK
{
    public class BatchReader
    {
        private readonly string _batchFilePath;
        private readonly IFileSystem _fileSystem;

        public BatchReader(string batchFilePath) : this(batchFilePath, new FileSystem())
        {
        }

        internal BatchReader(string batchFilePath, IFileSystem fileSystem)
        {
            if (batchFilePath == null)
            {
                throw new ArgumentNullException(nameof(batchFilePath));
            }

            if (string.IsNullOrWhiteSpace(batchFilePath))
            {
                throw new ArgumentException("Value cannot be null", nameof(batchFilePath));
            }

            string ext = Path.GetExtension(batchFilePath);

            if (string.IsNullOrEmpty(ext) || !ext.Equals(".oxi", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new NotSupportedException("The file indicated by the supplied file path must end in '.oxi'");
            }

            _batchFilePath = batchFilePath;
            _fileSystem = fileSystem;
        }

        public async Task<Batch> ReadBatchAsync()
        {
            var batch = new Batch();

            Stream stream = _fileSystem.FileStream.Create(_batchFilePath, FileMode.Open, FileAccess.Read);

            var settings = new XmlReaderSettings {Async = true};

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (await reader.ReadAsync())
                {
                    if (await reader.MoveToContentAsync() == XmlNodeType.Element &&
                        reader.Name.Equals("BATCH", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // this is the batch element
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

                    if (await reader.MoveToContentAsync() == XmlNodeType.Element &&
                        reader.Name.Equals("REFERENCEID", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var referenceId = new ReferenceId
                        {
                            Index = AttributeHelpers.GetInt(reader.GetAttribute("Index")),
                            Response = reader.GetAttribute("Response"),
                            Name = reader.GetAttribute("Name")
                        };

                        batch.Add(referenceId);
                    }

                    if (await reader.MoveToContentAsync() == XmlNodeType.Element &&
                        reader.Name.Equals("ENDINFO", StringComparison.InvariantCultureIgnoreCase))
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
                        reader.Name.Equals("TRANSACTION", StringComparison.InvariantCultureIgnoreCase))
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
                                    groupReader.Name.Equals("GROUP", StringComparison.InvariantCultureIgnoreCase))
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
                                                pageReader.Name.Equals("PAGE",
                                                    StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                var page = new Page
                                                {
                                                    DocumentLocator =
                                                        AttributeHelpers.GetInt(
                                                            pageReader.GetAttribute("DocumentLocator")),
                                                    BatchSequence = AttributeHelpers.GetInt(
                                                        pageReader.GetAttribute("BatchSequence")),
                                                    TransactionSequence = AttributeHelpers.GetInt(
                                                        pageReader.GetAttribute("TransactionSequence")),
                                                    GroupSequence = AttributeHelpers.GetInt(
                                                        pageReader.GetAttribute("GroupSequence")),
                                                    ScanSequence = AttributeHelpers.GetInt(
                                                        pageReader.GetAttribute("ScanSequence")),
                                                    ScanTime = AttributeHelpers.GetDateTime(
                                                        pageReader.GetAttribute("ScanTime")),
                                                    ItemStatus =
                                                        AttributeHelpers.GetItemStatus(pageReader.GetAttribute("ItemStatus")),
                                                    IsVirtual = AttributeHelpers.GetBooleanFromYesNo(
                                                        pageReader.GetAttribute("IsVirtual")),
                                                    PageType = AttributeHelpers.GetPageType(
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
                                                    PlugInPageMessage = pageReader.GetAttribute("PlugInPageMessage"),
                                                    Length = pageReader.GetAttribute("Length"),
                                                    Height = pageReader.GetAttribute("Height")
                                                };

                                                using (XmlReader pageSubReader = pageReader.ReadSubtree())
                                                {
                                                    while (await pageSubReader.ReadAsync())
                                                    {
                                                        if (await pageSubReader.MoveToContentAsync() == XmlNodeType.Element &&
                                                            pageReader.Name.Equals("IMAGE",
                                                                StringComparison.InvariantCultureIgnoreCase))
                                                        {
                                                            var image = new Image
                                                            {
                                                                Index = AttributeHelpers.GetInt(pageSubReader.GetAttribute("Index")),
                                                                RescanStatus = AttributeHelpers.GetRescanStatus(pageSubReader.GetAttribute("RescanStatus"))
                                                            };

                                                            page.Add(image);
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() == XmlNodeType.Element &&
                                                            pageReader.Name.Equals("CUSTOMDATA",
                                                                StringComparison.InvariantCultureIgnoreCase))
                                                        {
                                                            var customData = new CustomData();

                                                            page.CustomData = customData;
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() == XmlNodeType.Element &&
                                                            pageReader.Name.Equals("MICR",
                                                                StringComparison.InvariantCultureIgnoreCase))
                                                        {
                                                            var micr = new Micr();

                                                            page.Add(micr);
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() == XmlNodeType.Element &&
                                                            pageReader.Name.Equals("OCR",
                                                                StringComparison.InvariantCultureIgnoreCase))
                                                        {
                                                            var ocr = new Ocr();

                                                            page.Add(ocr);
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() == XmlNodeType.Element &&
                                                            pageReader.Name.Equals("BARCODE",
                                                                StringComparison.InvariantCultureIgnoreCase))
                                                        {
                                                            var barcode = new Barcode();

                                                            page.Add(barcode);
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() == XmlNodeType.Element &&
                                                            pageReader.Name.Equals("MARKDETECT",
                                                                StringComparison.InvariantCultureIgnoreCase))
                                                        {
                                                            var markDetect = new MarkDetect();

                                                            page.Add(markDetect);
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() == XmlNodeType.Element &&
                                                            pageReader.Name.Equals("AUDITTRAIL",
                                                                StringComparison.InvariantCultureIgnoreCase))
                                                        {
                                                            var auditTrail = new AuditTrail();

                                                            page.Add(auditTrail);
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() == XmlNodeType.Element &&
                                                            pageReader.Name.Equals("TAG",
                                                                StringComparison.InvariantCultureIgnoreCase))
                                                        {
                                                            var tag = new Tag();

                                                            page.Add(tag);
                                                        }
                                                    }
                                                }

                                                group.Add(page);
                                            }
                                        }
                                    }

                                    transaction.Groups.Add(group);
                                }
                            }
                        }


                        batch.Add(transaction);
                    }
                }
            }

            return batch;
        }
    }
}