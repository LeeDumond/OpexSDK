using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using OpexSDK.Enumerations;
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

            XmlReaderSettings settings = new XmlReaderSettings {Async = true};

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
                        batch.JobType = Helpers.GetJobType(reader.GetAttribute("JobType"));
                        batch.OperatorName = reader.GetAttribute("OperatorName");
                        batch.OperatingMode = Helpers.GetOperatingMode(reader.GetAttribute("OperatingMode"));
                        batch.StartTime = Helpers.GetTimeFromAttribute(reader.GetAttribute("StartTime"));
                        batch.PluginMessage = reader.GetAttribute("PluginMessage");
                        batch.ProcessDate = Helpers.GetTimeFromAttribute(reader.GetAttribute("ProcessDate"))?.Date;
                        batch.ReceiveDate = Helpers.GetTimeFromAttribute(reader.GetAttribute("ReceiveDate"))?.Date;
                        batch.ScanDevice = reader.GetAttribute("ScanDevice");
                        batch.SoftwareVersion = reader.GetAttribute("SoftwareVersion");
                        batch.TransportId = reader.GetAttribute("TransportId");
                    }

                    if (await reader.MoveToContentAsync() == XmlNodeType.Element &&
                        reader.Name.Equals("REFERENCEID", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var referenceId = new ReferenceId
                        {
                            Index = Helpers.GetIntFromAttribute(reader.GetAttribute("Index")),
                            Response = reader.GetAttribute("Response"),
                            Name = reader.GetAttribute("Name")
                        };

                        batch.ReferenceIds.Add(referenceId);
                    }

                    if (await reader.MoveToContentAsync() == XmlNodeType.Element &&
                        reader.Name.Equals("ENDINFO", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var endInfo = new EndInfo
                        {
                            EndTime = Helpers.GetTimeFromAttribute(reader.GetAttribute("EndTime")),
                            NumPages = Helpers.GetIntFromAttribute(reader.GetAttribute("NumPages")),
                            NumGroups = Helpers.GetIntFromAttribute(reader.GetAttribute("NumGroups")),
                            NumTransactions = Helpers.GetIntFromAttribute(reader.GetAttribute("NumTransactions")),
                            IsModified = Helpers.GetBooleanFromAttribute(reader.GetAttribute("IsModified"))
                        };

                        batch.EndInfo = endInfo;
                    }

                    if (await reader.MoveToContentAsync() == XmlNodeType.Element &&
                        reader.Name.Equals("TRANSACTION", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var transaction = new Transaction
                        {
                            TransactionId = Helpers.GetIntFromAttribute(reader.GetAttribute("TransactionID"))
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
                                        GroupId = Helpers.GetIntFromAttribute(reader.GetAttribute("GroupID"))
                                    };


                                    using (XmlReader pageReader = groupReader.ReadSubtree())
                                    {
                                        while (await pageReader.ReadAsync())
                                        {
                                            if (await pageReader.MoveToContentAsync() == XmlNodeType.Element &&
                                                pageReader.Name.Equals("PAGE",
                                                    StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                var page = new Page()
                                                {
                                                    DocumentLocator =
                                                        Helpers.GetIntFromAttribute(
                                                            reader.GetAttribute("DocumentLocator"))
                                                };

                                                group.Pages.Add(page);
                                            }
                                        }
                                    }



                                    transaction.Groups.Add(group);
                                }
                            }
                        }


                        batch.Transactions.Add(transaction);
                    }
                }
            }

            return batch;
        }
    }
}