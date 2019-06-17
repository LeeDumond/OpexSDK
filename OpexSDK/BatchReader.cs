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

        public async Task<Batch> ReadBatch()
        {
            var batch = new Batch();

            Stream stream = _fileSystem.FileStream.Create(_batchFilePath, FileMode.Open, FileAccess.Read);

            XmlReaderSettings settings = new XmlReaderSettings {Async = true};

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (await reader.ReadAsync())
                {
                    if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.Name.Equals("BATCH", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // this is the batch element
                        batch.BaseMachine = reader.GetAttribute("BaseMachine");
                        batch.FormatVersion = reader.GetAttribute("FormatVersion");
                        batch.BatchIdentifier = reader.GetAttribute("BatchIdentifier");
                        batch.DeveloperReserved = reader.GetAttribute("DeveloperReserved");
                        batch.ImageFilePath = reader.GetAttribute("ImageFilePath");
                        batch.JobName = reader.GetAttribute("JobName");
                        batch.JobType = GetJobType(reader.GetAttribute("JobType"));
                        batch.OperatorName = reader.GetAttribute("OperatorName");
                        batch.OperatingMode = GetOperatingMode(reader.GetAttribute("OperatingMode"));
                        batch.StartTime = XmlConvert.ToDateTime(reader.GetAttribute("StartTime") ?? throw new InvalidOperationException(), XmlDateTimeSerializationMode.Local);

                    }

                }
            }

            return batch;
        }

        internal static OperatingMode? GetOperatingMode(string attributeValue)
        {
            switch (attributeValue)
            {
                case "MANUAL_SCAN":
                    return OperatingMode.ManualScan;
                case "MODIFIED":
                    return OperatingMode.Modified;
                case "":
                case null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeValue));
            }
        }

        internal static JobType? GetJobType(string attributeValue)
        {
            switch (attributeValue)
            {
                case "SINGLE":
                    return JobType.Single;
                case "MULTI":
                    return JobType.Multi;
                case "STUB_ONLY":
                    return JobType.StubOnly;
                case "CHECK_ONLY":
                    return JobType.CheckOnly;
                case "MULTI_WITH_PAGE":
                    return JobType.MultiWithPage;
                case "PAGE_ONLY":
                    return JobType.PageOnly;
                case "UNSTRUCTURED":
                    return JobType.Unstructured;
                case "STRUCTURED":
                    return JobType.Structured;
                case "":
                case null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeValue));
            }
        }
    }
}