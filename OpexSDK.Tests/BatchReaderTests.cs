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
    <REFERENCEID Index=""1"" Response=""High Priority"" Name=""Batch 1234"" />
    <REFERENCEID Index=""2"" Response=""Normal Priority"" Name=""Batch 5678"" />
    <TRANSACTION TransactionID=""3141"">
        <GROUP GroupID=""98"">
            <PAGE DocumentLocator=""1"" TransactionSequence="""" GroupSequence="""" BatchSequence="""" ScanSequence="""" ScanTime=""""
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
            <PAGE DocumentLocator=""2"">
            </PAGE>
        </GROUP>
        <GROUP GroupID=""108"">
            <PAGE DocumentLocator=""3"">
            </PAGE>
        </GROUP>
    </TRANSACTION>
    <TRANSACTION TransactionID=""3152"">
        <GROUP GroupID=""125"">
        </GROUP>
    </TRANSACTION>
    <ENDINFO EndTime=""2019-03-22 23:32:45"" NumPages=""4"" NumGroups=""2"" NumTransactions=""2"" IsModified=""FALSE"" />
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
        public async Task ReadBatch_ReferenceIdsPopulated()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\Opex\test.oxi", new MockFileData(batchFileContents) }
            });

            var reader = new BatchReader(@"C:\Opex\test.oxi", fileSystem);

            Batch batch = await reader.ReadBatch();

            Assert.Equal(2, batch.ReferenceIds.Count);

            Assert.Equal(1, batch.ReferenceIds[0].Index);
            Assert.Equal("High Priority", batch.ReferenceIds[0].Response);
            Assert.Equal("Batch 1234", batch.ReferenceIds[0].Name);

            Assert.Equal(2, batch.ReferenceIds[1].Index);
            Assert.Equal("Normal Priority", batch.ReferenceIds[1].Response);
            Assert.Equal("Batch 5678", batch.ReferenceIds[1].Name);
        }

        [Fact]
        public async Task ReadBatch_TransactionsPopulated()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\Opex\test.oxi", new MockFileData(batchFileContents) }
            });

            var reader = new BatchReader(@"C:\Opex\test.oxi", fileSystem);

            Batch batch = await reader.ReadBatch();

            Assert.Equal(2, batch.Transactions.Count);

            Assert.Equal(3141, batch.Transactions[0].TransactionId);
            Assert.NotNull(batch.Transactions[0].Groups);

            Assert.Equal(3152, batch.Transactions[1].TransactionId);
            Assert.NotNull(batch.Transactions[1].Groups);
        }

        [Fact]
        public async Task ReadBatch_GroupsPopulated()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\Opex\test.oxi", new MockFileData(batchFileContents) }
            });

            var reader = new BatchReader(@"C:\Opex\test.oxi", fileSystem);

            Batch batch = await reader.ReadBatch();

            Assert.Equal(2, batch.Transactions[0].Groups.Count);

            Assert.Equal(98, batch.Transactions[0].Groups[0].GroupId);
            Assert.Equal(108, batch.Transactions[0].Groups[1].GroupId);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages);

            Assert.Equal(1, batch.Transactions[1].Groups.Count);

            Assert.Equal(125, batch.Transactions[1].Groups[0].GroupId);
            Assert.NotNull(batch.Transactions[1].Groups[0].Pages);
        }

        [Fact]
        public async Task ReadBatch_PagesPopulated()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\Opex\test.oxi", new MockFileData(batchFileContents) }
            });

            var reader = new BatchReader(@"C:\Opex\test.oxi", fileSystem);

            Batch batch = await reader.ReadBatch();

            Assert.Equal(2, batch.Transactions[0].Groups[0].Pages.Count);

            Assert.Equal(1, batch.Transactions[0].Groups[0].Pages[0].DocumentLocator);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].AuditTrails);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Barcodes);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Images);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].MarkDetects);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Micrs);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Ocrs);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].ReferenceIds);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Tags);

            Assert.Equal(2, batch.Transactions[0].Groups[0].Pages[1].DocumentLocator);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].AuditTrails);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Barcodes);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Images);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].MarkDetects);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Micrs);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Ocrs);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].ReferenceIds);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Tags);

            Assert.Equal(3, batch.Transactions[0].Groups[1].Pages[0].DocumentLocator);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].AuditTrails);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Barcodes);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Images);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].MarkDetects);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Micrs);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Ocrs);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].ReferenceIds);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Tags);
        }

        [Fact]
        public async Task ReadBatch_EndInfoPopulated()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\Opex\test.oxi", new MockFileData(batchFileContents) }
            });

            var reader = new BatchReader(@"C:\Opex\test.oxi", fileSystem);

            Batch batch = await reader.ReadBatch();

            Assert.Equal(new DateTime(2019, 3, 22, 23, 32, 45), batch.EndInfo.EndTime);
            Assert.Equal(4, batch.EndInfo.NumPages);
            Assert.Equal(2, batch.EndInfo.NumGroups);
            Assert.Equal(2, batch.EndInfo.NumTransactions);
            Assert.False(batch.EndInfo.IsModified);
        }
    }
}
