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
        public BatchReader() : this(new FileSystem())
        {
        }

        internal BatchReader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _validationErrors = new List<XmlSchemaException>();
        }

        private readonly IFileSystem _fileSystem;
        private readonly IList<XmlSchemaException> _validationErrors;
        private bool _throwOnValidationError;

        /// <summary>
        ///     The collection of errors, if any, encountered during the validation process.
        ///     NOTE: If this collection is empty, it does not automatically mean that the batch information file is valid. If no
        ///     schema is supplied, data validation will not be performed and this collection will be empty. If the schema is
        ///     supplied but in itself is not valid, those errors will be contained in this collection, though data validation will
        ///     not be performed.
        /// </summary>
        public ReadOnlyCollection<XmlSchemaException> ValidationErrors =>
            new ReadOnlyCollection<XmlSchemaException>(_validationErrors);

        /// <summary>
        ///     A method that asynchronously reads the data contained in the batch information file supplied to the reader. If
        ///     a schema definition file (*.xsd) is also supplied, this method will confirm the validity of the schema, and if it
        ///     is valid, will validate the data file against it.
        /// </summary>
        /// <param name="batchFilePath">The path to the batch information file.</param>
        /// <param name="schemaFilePath">
        ///     The path to an XSD schema definition file to validate against. By default, this argument
        ///     is null, which means no validation is performed.
        /// </param>
        /// <param name="throwOnValidationError">
        ///     If true, an exception will be thrown if any validation errors are encountered.
        ///     If false, validation errors will be added to the ValidationErrors collection, but no exception will be thrown. The
        ///     default is false.
        ///     NOTE: If no schema is supplied, this parameter has no effect.
        /// </param>
        /// <returns>An instance of Batch containing all the data in the batch information file that was supplied to the reader.</returns>
        public Batch ReadBatch(string batchFilePath, string schemaFilePath = null, bool throwOnValidationError = false)
        {
            VerifyBatchFilePath(batchFilePath);
            _throwOnValidationError = throwOnValidationError;
            var batch = new Batch();

            using (XmlReader reader = GetXmlReader(batchFilePath, schemaFilePath, false))
            {
                while (reader.Read())
                {
                    if (reader.MoveToContent() == XmlNodeType.Element && reader.Name.Equals("Batch"))
                    {
                        PopulateBatch(batch, reader);
                    }

                    if (reader.MoveToContent() == XmlNodeType.Element && reader.Name.Equals("ReferenceID"))
                    {
                        batch.Add(GetReferenceId(reader));
                    }

                    if (reader.MoveToContent() == XmlNodeType.Element && reader.Name.Equals("EndInfo"))
                    {
                        batch.EndInfo = GetEndInfo(reader);
                    }

                    if (reader.MoveToContent() == XmlNodeType.Element && reader.Name.Equals("Transaction"))
                    {
                        Transaction transaction = GetTransaction(reader);

                        using (XmlReader groupReader = reader.ReadSubtree())
                        {
                            while (groupReader.Read())
                            {
                                if (groupReader.MoveToContent() == XmlNodeType.Element &&
                                    groupReader.Name.Equals("Group"))
                                {
                                    Group group = GetGroup(groupReader);

                                    using (XmlReader pageReader = groupReader.ReadSubtree())
                                    {
                                        while (pageReader.Read())
                                        {
                                            if (pageReader.MoveToContent() == XmlNodeType.Element &&
                                                pageReader.Name.Equals("Page"))
                                            {
                                                Page page = GetPage(pageReader);

                                                using (XmlReader pageSubReader = pageReader.ReadSubtree())
                                                {
                                                    while (pageSubReader.Read())
                                                    {
                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("Image"))
                                                        {
                                                            page.Add(GetImage(pageSubReader));
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("CustomData"))
                                                        {
                                                            page.Add(GetCustomData(pageSubReader));
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("Micr"))
                                                        {
                                                            page.Add(GetMicr(pageSubReader));
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("Ocr"))
                                                        {
                                                            page.Add(GetOcr(pageSubReader));
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("Barcode"))
                                                        {
                                                            page.Add(GetBarcode(pageSubReader));
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("MarkDetect"))
                                                        {
                                                            page.Add(GetMarkDetect(pageSubReader));
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("AuditTrail"))
                                                        {
                                                            page.Add(GetAuditTrail(pageSubReader));
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("ReferenceID"))
                                                        {
                                                            page.Add(GetReferenceId(pageSubReader));
                                                        }

                                                        if (pageSubReader.MoveToContent() == XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("Tag"))
                                                        {
                                                            page.Add(GetTag(pageSubReader));
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

        /// <summary>
        ///     A method that asynchronously that reads the data contained in the batch information file supplied to the reader. If
        ///     a schema definition file (*.xsd) is also supplied, this method will confirm the validity of the schema, and if it
        ///     is valid, will validate the data file against it.
        /// </summary>
        /// <param name="batchFilePath">The path to the batch information file.</param>
        /// <param name="schemaFilePath">
        ///     The path to an XSD schema definition file to validate against. By default, this argument
        ///     is null, which means no validation is performed.
        /// </param>
        /// <param name="throwOnValidationError">
        ///     If true, an exception will be thrown if any validation errors are encountered.
        ///     If false, validation errors will be added to the ValidationErrors collection, but no exception will be thrown. The
        ///     default is false.
        ///     NOTE: If no schema is supplied, this parameter has no effect.
        /// </param>
        /// <returns>An instance of Batch containing all the data in the batch information file that was supplied to the reader.</returns>
        public async Task<Batch> ReadBatchAsync(string batchFilePath, string schemaFilePath = null,
            bool throwOnValidationError = false)
        {
            VerifyBatchFilePath(batchFilePath);
            _throwOnValidationError = throwOnValidationError;
            var batch = new Batch();

            using (XmlReader reader = GetXmlReader(batchFilePath, schemaFilePath, true))
            {
                while (await reader.ReadAsync())
                {
                    if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.Name.Equals("Batch"))
                    {
                        PopulateBatch(batch, reader);
                    }

                    if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.Name.Equals("ReferenceID"))
                    {
                        batch.Add(GetReferenceId(reader));
                    }

                    if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.Name.Equals("EndInfo"))
                    {
                        batch.EndInfo = GetEndInfo(reader);
                    }

                    if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.Name.Equals("Transaction"))
                    {
                        Transaction transaction = GetTransaction(reader);

                        using (XmlReader groupReader = reader.ReadSubtree())
                        {
                            while (await groupReader.ReadAsync())
                            {
                                if (await groupReader.MoveToContentAsync() == XmlNodeType.Element &&
                                    groupReader.Name.Equals("Group"))
                                {
                                    Group group = GetGroup(groupReader);

                                    using (XmlReader pageReader = groupReader.ReadSubtree())
                                    {
                                        while (await pageReader.ReadAsync())
                                        {
                                            if (await pageReader.MoveToContentAsync() == XmlNodeType.Element &&
                                                pageReader.Name.Equals("Page"))
                                            {
                                                Page page = GetPage(pageReader);

                                                using (XmlReader pageSubReader = pageReader.ReadSubtree())
                                                {
                                                    while (await pageSubReader.ReadAsync())
                                                    {
                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element && pageSubReader.Name.Equals("Image"))
                                                        {
                                                            page.Add(GetImage(pageSubReader));
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("CustomData"))
                                                        {
                                                            page.Add(GetCustomData(pageSubReader));
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element && pageSubReader.Name.Equals("Micr"))
                                                        {
                                                            page.Add(GetMicr(pageSubReader));
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element && pageSubReader.Name.Equals("Ocr"))
                                                        {
                                                            page.Add(GetOcr(pageSubReader));
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element && pageSubReader.Name.Equals("Barcode"))
                                                        {
                                                            page.Add(GetBarcode(pageSubReader));
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("MarkDetect"))
                                                        {
                                                            page.Add(GetMarkDetect(pageSubReader));
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("AuditTrail"))
                                                        {
                                                            page.Add(GetAuditTrail(pageSubReader));
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element &&
                                                            pageSubReader.Name.Equals("ReferenceID"))
                                                        {
                                                            page.Add(GetReferenceId(pageSubReader));
                                                        }

                                                        if (await pageSubReader.MoveToContentAsync() ==
                                                            XmlNodeType.Element && pageSubReader.Name.Equals("Tag"))
                                                        {
                                                            page.Add(GetTag(pageSubReader));
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

        private static void VerifyBatchFilePath(string batchFilePath)
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
                throw new InvalidOperationException("The file indicated by the supplied path must end in '.oxi'");
            }
        }

        private static void PopulateBatch(Batch batch, XmlReader reader)
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

        private static Group GetGroup(XmlReader reader)
        {
            return new Group {GroupId = AttributeHelpers.GetInt(reader.GetAttribute("GroupID"))};
        }

        private static Transaction GetTransaction(XmlReader reader)
        {
            return new Transaction {TransactionId = AttributeHelpers.GetInt(reader.GetAttribute("TransactionID"))};
        }

        private static CustomData GetCustomData(XmlReader reader)
        {
            return new CustomData {Entry = reader.GetAttribute("Entry")};
        }

        private static Tag GetTag(XmlReader reader)
        {
            return new Tag {Source = reader.GetAttribute("Source"), Value = reader.GetAttribute("Value")};
        }

        private static AuditTrail GetAuditTrail(XmlReader reader)
        {
            return new AuditTrail
            {
                Type = AttributeHelpers.GetAuditTrailType(reader.GetAttribute("Type")),
                Apply = AttributeHelpers.GetBooleanFromTrueFalse(reader.GetAttribute("Apply")),
                Side = AttributeHelpers.GetSide(reader.GetAttribute("Side")),
                Text = reader.GetAttribute("Text")
            };
        }

        private static MarkDetect GetMarkDetect(XmlReader reader)
        {
            return new MarkDetect
            {
                Index = AttributeHelpers.GetInt(reader.GetAttribute("Index")),
                Side = AttributeHelpers.GetSide(reader.GetAttribute("Side")),
                Result = AttributeHelpers.GetBooleanFromYesNo(reader.GetAttribute("Result")),
                Name = reader.GetAttribute("Name")
            };
        }

        private static Barcode GetBarcode(XmlReader reader)
        {
            return new Barcode
            {
                Index = AttributeHelpers.GetInt(reader.GetAttribute("Index")),
                Type = reader.GetAttribute("Type"),
                Side = AttributeHelpers.GetSide(reader.GetAttribute("Side")),
                Value = reader.GetAttribute("Value")
            };
        }

        private static Ocr GetOcr(XmlReader reader)
        {
            return new Ocr
            {
                Index = AttributeHelpers.GetInt(reader.GetAttribute("Index")),
                Side = AttributeHelpers.GetSide(reader.GetAttribute("Side")),
                Value = reader.GetAttribute("Value"),
                Name = reader.GetAttribute("Name")
            };
        }

        private static Micr GetMicr(XmlReader reader)
        {
            return new Micr
            {
                Status = AttributeHelpers.GetMicrStatus(reader.GetAttribute("Status")),
                RtStatus = AttributeHelpers.GetRtStatus(reader.GetAttribute("RtStatus")),
                CheckType = AttributeHelpers.GetCheckType(reader.GetAttribute("CheckType")),
                Side = AttributeHelpers.GetSide(reader.GetAttribute("Side")),
                Value = reader.GetAttribute("Value")
            };
        }

        private static Image GetImage(XmlReader reader)
        {
            return new Image
            {
                Index = AttributeHelpers.GetInt(reader.GetAttribute("Index")),
                RescanStatus = AttributeHelpers.GetRescanStatus(reader.GetAttribute("RescanStatus")),
                ScantimeFinalBlankAreaDecision =
                    AttributeHelpers.GetScantimeFinalBlankAreaDecision(
                        reader.GetAttribute("ScantimeFinalBlankAreaDecision")),
                Side = AttributeHelpers.GetSide(reader.GetAttribute("Side")),
                Type = AttributeHelpers.GetImageType(reader.GetAttribute("Type")),
                Depth = AttributeHelpers.GetImageDepth(reader.GetAttribute("Depth")),
                Format = AttributeHelpers.GetImageFormat(reader.GetAttribute("Format")),
                Filename = reader.GetAttribute("Filename"),
                Filesize = AttributeHelpers.GetLong(reader.GetAttribute("Filesize")),
                Length = AttributeHelpers.GetInt(reader.GetAttribute("Length")),
                Height = AttributeHelpers.GetInt(reader.GetAttribute("Height")),
                OffsetLength = AttributeHelpers.GetInt(reader.GetAttribute("OffsetLength")),
                OffsetHeight = AttributeHelpers.GetInt(reader.GetAttribute("OffsetHeight")),
                ResolutionLength = AttributeHelpers.GetImageResolution(reader.GetAttribute("ResolutionLength")),
                ResolutionHeight = AttributeHelpers.GetImageResolution(reader.GetAttribute("ResolutionHeight"))
            };
        }

        private static Page GetPage(XmlReader reader)
        {
            return new Page
            {
                DocumentLocator = AttributeHelpers.GetInt(reader.GetAttribute("DocumentLocator")),
                BatchSequence = AttributeHelpers.GetInt(reader.GetAttribute("BatchSequence")),
                TransactionSequence = AttributeHelpers.GetInt(reader.GetAttribute("TransactionSequence")),
                GroupSequence = AttributeHelpers.GetInt(reader.GetAttribute("GroupSequence")),
                ScanSequence = AttributeHelpers.GetInt(reader.GetAttribute("ScanSequence")),
                ScanTime = AttributeHelpers.GetDateTime(reader.GetAttribute("ScanTime")),
                ItemStatus = AttributeHelpers.GetItemStatus(reader.GetAttribute("ItemStatus")),
                IsVirtual = AttributeHelpers.GetBooleanFromYesNo(reader.GetAttribute("IsVirtual")),
                PageType = AttributeHelpers.GetPageType(reader.GetAttribute("PageType")),
                PageName = reader.GetAttribute("PageName"),
                SubPageName = reader.GetAttribute("SubPageName"),
                OperatorSelect = AttributeHelpers.GetBooleanFromYesNo(reader.GetAttribute("OperatorSelect")),
                Bin = reader.GetAttribute("Bin"),
                AverageThickness = AttributeHelpers.GetFloat(reader.GetAttribute("AverageThickness")),
                EnvelopeDetect = AttributeHelpers.GetEnvelopeDetect(reader.GetAttribute("EnvelopeDetect")),
                SkewDegrees = AttributeHelpers.GetFloat(reader.GetAttribute("SkewDegrees")),
                DeskewStatus = AttributeHelpers.GetDeskewStatus(reader.GetAttribute("DeskewStatus")),
                FrontStreakDetectStatus =
                    AttributeHelpers.GetFrontStreakDetectStatus(reader.GetAttribute("FrontStreakDetectStatus")),
                BackStreakDetectStatus =
                    AttributeHelpers.GetBackStreakDetectStatus(reader.GetAttribute("BackStreakDetectStatus")),
                PlugInPageMessage = reader.GetAttribute("PlugInPageMessage"),
                Length = reader.GetAttribute("Length"),
                Height = reader.GetAttribute("Height")
            };
        }

        private static EndInfo GetEndInfo(XmlReader reader)
        {
            return new EndInfo
            {
                EndTime = AttributeHelpers.GetDateTime(reader.GetAttribute("EndTime")),
                NumPages = AttributeHelpers.GetInt(reader.GetAttribute("NumPages")),
                NumGroups = AttributeHelpers.GetInt(reader.GetAttribute("NumGroups")),
                NumTransactions = AttributeHelpers.GetInt(reader.GetAttribute("NumTransactions")),
                IsModified = AttributeHelpers.GetBooleanFromTrueFalse(reader.GetAttribute("IsModified"))
            };
        }

        private static ReferenceId GetReferenceId(XmlReader reader)
        {
            return new ReferenceId
            {
                Index = AttributeHelpers.GetInt(reader.GetAttribute("Index")),
                Response = reader.GetAttribute("Response"),
                Name = reader.GetAttribute("Name")
            };
        }

        private XmlReader GetXmlReader(string batchFilePath, string schemaFilePath, bool async)
        {
            Stream batchStream = _fileSystem.FileStream.Create(batchFilePath, FileMode.Open, FileAccess.Read);

            var settings = new XmlReaderSettings
            {
                Async = async, IgnoreComments = true, IgnoreWhitespace = true, IgnoreProcessingInstructions = true
            };

            if (schemaFilePath != null)
            {
                XmlSchema schema;

                using (Stream schemaStream =
                    _fileSystem.FileStream.Create(schemaFilePath, FileMode.Open, FileAccess.Read))
                {
                    schema = XmlSchema.Read(schemaStream, ValidationCallBack);
                }

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
            if (_throwOnValidationError)
            {
                throw e.Exception;
            }

            _validationErrors.Add(e.Exception);
        }
    }
}