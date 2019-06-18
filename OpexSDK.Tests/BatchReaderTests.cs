using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using OpexSDK.Enumerations;
using OpexSDK.Models;
using Xunit;

namespace OpexSDK.Tests
{
    public class BatchReaderTests
    {
        private string batchFileContents = @"<?xml version=""1.0"" encoding=""utf-8""?>

<BATCH FormatVersion=""03.14"" BaseMachine=""MODEL_51"" ScanDevice=""AS3600i"" SoftwareVersion=""02.23.00.05"" TransportId=""MyTransport"" BatchIdentifier=""thisisbatch45"" JobType=""MULTI_WITH_PAGE""
       OperatingMode=""MODIFIED"" JobName=""Lockbox 25"" OperatorName=""Lee Dumond"" StartTime=""2019-03-22 23:24:07"" ReceiveDate=""2019-03-21"" ProcessDate=""2019-03-22"" ImageFilePath=""X:\Images\OPEX\somebatchid""
       PluginMessage=""XYZ Plug-in"" DeveloperReserved=""1234-56a"">
  <REFERENCEID Index="""" Response="""" Name="""" />
  <TRANSACTION TransactionID="""">
    <GROUP GroupID="""" />
    <PAGE DocumentLocator="""" TransactionSequence="""" GroupSequence="""" BatchSequence="""" ScanSequence="""" ScanTime=""""
          ItemStatus="""" IsVirtual="""" PageType="""" PageName="""" SubPageName="""" OperatorSelect="""" EnvelopeDetect=""""
          AverageThickness="""" SkewDegrees="""" DeskewStatus="""" FrontStreakDetectStatus="""" BackStreakDetectStatus=""""
          PlugInPageMessage="""">
      <IMAGE Index="""" RescanStatus="""" ScantimeFinalBlankAreaDecision="""" Side="""" Type="""" Depth="""" Format="""" FileName=""""
             FileSize="""" Lenght="""" Height="""" OffsetLength="""" OffsetHeight="""" ResolutionLength="""" ResolutionHeight="""">
      </IMAGE>
      <CUSTOMDATA Entry="""" />
      <MICR Status="""" RtStatus="""" CheckType="""" Side="""" Value="""" />
      <OCR Index="""" Side="""" Value="""" Name="""" />
      <BARCODE Index="""" Type="""" Side="""" Value="""" />
      <MARKDETECT Index="""" Side="""" Result="""" Name="""" />
      <AUDITTRAIL Index="""" Side="""" Apply="""" Text="""" />
      <TAG Source="""" Value="""" />
    </PAGE>
  </TRANSACTION>
  <ENDINFO EndTime="""" NumPages="""" NumGroups="""" NumTransactions="""" IsModified="""" />
</BATCH>";

        [Fact]
        public void BatchReader_NullPath_Throws()
        {
            Func<BatchReader> func = () => new BatchReader(null);

            Assert.Throws<ArgumentNullException>(func);
        }

        [Fact]
        public void BatchReader_EmptyPath_Throws()
        {
            Func<BatchReader> func = () => new BatchReader("");

            Assert.Throws<ArgumentException>(func);
        }

        [Fact]
        public void BatchReader_WrongFileExtension_Throws()
        {
            Func<BatchReader> func = () => new BatchReader(@"C:\Opex\test.xml");

            Assert.Throws<NotSupportedException>(func);
        }

        [Fact]
        public void BatchReader_NoFileExtension_Throws()
        {
            Func<BatchReader> func = () => new BatchReader(@"C:\Opex\");

            Assert.Throws<NotSupportedException>(func);
        }

        [Fact]
        public async Task ReadBatch_ReturnsNonNullBatch()
        {
            batchFileContents = "<BATCH></BATCH>";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\Opex\test.oxi", new MockFileData(batchFileContents) }
            });

            var reader = new BatchReader(@"C:\Opex\test.oxi", fileSystem);

            Batch batch = await reader.ReadBatch();

            Assert.NotNull(batch);
        }

        [Fact]
        public async Task ReadBatch_CollectionsInitialized()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\Opex\test.oxi", new MockFileData(batchFileContents) }
            });

            var reader = new BatchReader(@"C:\Opex\test.oxi", fileSystem);

            Batch batch = await reader.ReadBatch();

            Assert.NotNull(batch.ReferenceIds);
            Assert.NotNull(batch.Transactions);
        }

        [Fact]
        public async Task ReadBatch_BatchPropertiesPopulated()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\Opex\test.oxi", new MockFileData(batchFileContents) }
            });

            var reader = new BatchReader(@"C:\Opex\test.oxi", fileSystem);

            Batch batch = await reader.ReadBatch();

            Assert.Equal("MODEL_51", batch.BaseMachine);
            Assert.Equal("03.14", batch.FormatVersion);
            Assert.Equal("thisisbatch45", batch.BatchIdentifier);
            Assert.Equal("1234-56a", batch.DeveloperReserved);
            Assert.Equal(@"X:\Images\OPEX\somebatchid", batch.ImageFilePath);
            Assert.Equal("Lockbox 25", batch.JobName);
            Assert.Equal(JobType.MultiWithPage, batch.JobType);
            Assert.Equal("Lee Dumond", batch.OperatorName);
            Assert.Equal(new DateTime(2019, 3, 22, 23, 24, 07), batch.StartTime);
            Assert.Equal(OperatingMode.Modified, batch.OperatingMode);
            Assert.Equal("XYZ Plug-in", batch.PluginMessage);
            Assert.Equal(new DateTime(2019, 3, 22), batch.ProcessDate);
            Assert.Equal(new DateTime(2019, 3, 21), batch.ReceiveDate);
            Assert.Equal("AS3600i", batch.ScanDevice);
            Assert.Equal("02.23.00.05", batch.SoftwareVersion);
            Assert.Equal("MyTransport", batch.TransportId);
        }

        [Fact]
        public void GetJobType_ReturnsCorrectJobType()
        {
            Assert.Equal(JobType.Single, BatchReader.GetJobType("SINGLE"));
            Assert.Equal(JobType.Multi, BatchReader.GetJobType("MULTI"));
            Assert.Equal(JobType.StubOnly, BatchReader.GetJobType("STUB_ONLY"));
            Assert.Equal(JobType.CheckOnly, BatchReader.GetJobType("CHECK_ONLY"));
            Assert.Equal(JobType.MultiWithPage, BatchReader.GetJobType("MULTI_WITH_PAGE"));
            Assert.Equal(JobType.PageOnly, BatchReader.GetJobType("PAGE_ONLY"));
            Assert.Equal(JobType.Unstructured, BatchReader.GetJobType("UNSTRUCTURED"));
            Assert.Equal(JobType.Structured, BatchReader.GetJobType("STRUCTURED"));
            Assert.Null(BatchReader.GetJobType(""));
            Assert.Null(BatchReader.GetJobType(null));

            Assert.Throws<ArgumentOutOfRangeException>(() => BatchReader.GetJobType("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetOperatingMode_ReturnsCorrectOperatingMode()
        {
            Assert.Equal(OperatingMode.ManualScan, BatchReader.GetOperatingMode("MANUAL_SCAN"));
            Assert.Equal(OperatingMode.Modified, BatchReader.GetOperatingMode("MODIFIED"));
            Assert.Null(BatchReader.GetOperatingMode(""));
            Assert.Null(BatchReader.GetOperatingMode(null));

            Assert.Throws<ArgumentOutOfRangeException>(() => BatchReader.GetOperatingMode("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetTimeFromAttribute_ReturnsCorrectTime()
        {
            Assert.Equal(new DateTime(2019, 3, 22, 23, 24, 07), BatchReader.GetTimeFromAttribute("2019-03-22 23:24:07"));
            Assert.Equal(new DateTime(2019, 3, 22), BatchReader.GetTimeFromAttribute("2019-03-22"));
            Assert.Null(BatchReader.GetTimeFromAttribute(""));
            Assert.Null(BatchReader.GetTimeFromAttribute(null));

            Assert.Throws<FormatException>(() => BatchReader.GetTimeFromAttribute("not_a_time"));
        }
    }
}
