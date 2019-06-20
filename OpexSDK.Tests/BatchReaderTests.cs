using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using OpexSDK.Enumerations;
using OpexSDK.Models;
using Xunit;

namespace OpexSDK.Tests
{
    public class BatchReaderTests : IClassFixture<FileSystemFixture>
    {
        private readonly FileSystemFixture _fileSystemFixture;
        //private readonly BatchReader reader;

        public BatchReaderTests(FileSystemFixture fileSystemFixture)
        {
            _fileSystemFixture = fileSystemFixture;
            //var reader = new BatchReader(@"C:\Opex\test1.oxi", fileSystemFixture.FileSystem);
        }

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
        public async Task ReadBatchAsync_ReturnsNonNullBatch()
        {
            string batchFileContents = "<BATCH></BATCH>";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\Opex\test.oxi", new MockFileData(batchFileContents) }
            });

            var reader = new BatchReader(@"C:\Opex\test.oxi", fileSystem);

            Batch batch = await reader.ReadBatchAsync();

            Assert.NotNull(batch);
        }

        [Fact]
        public async Task ReadBatchAsync_CollectionsInitialized()
        {
            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

            Assert.NotNull(batch.ReferenceIds);
            Assert.NotNull(batch.Transactions);
        }

        [Fact]
        public async Task ReadBatchAsync_BatchPropertiesPopulated()
        {
            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

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
        public async Task ReadBatchAsync_ReferenceIdsPopulated()
        {
            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

            Assert.Equal(2, batch.ReferenceIds.Count);

            Assert.Equal(1, batch.ReferenceIds[0].Index);
            Assert.Equal("High Priority", batch.ReferenceIds[0].Response);
            Assert.Equal("Batch 1234", batch.ReferenceIds[0].Name);

            Assert.Equal(2, batch.ReferenceIds[1].Index);
            Assert.Equal("Normal Priority", batch.ReferenceIds[1].Response);
            Assert.Equal("Batch 5678", batch.ReferenceIds[1].Name);
        }

        [Fact]
        public async Task ReadBatchAsync_TransactionsPopulated()
        {

            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

            Assert.Equal(2, batch.Transactions.Count);

            Assert.Equal(3141, batch.Transactions[0].TransactionId);
            Assert.NotNull(batch.Transactions[0].Groups);

            Assert.Equal(3152, batch.Transactions[1].TransactionId);
            Assert.NotNull(batch.Transactions[1].Groups);
        }

        [Fact]
        public async Task ReadBatchAsync_GroupsPopulated()
        {
            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

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
        public async Task ReadBatchAsync_PagesPopulated()
        {
            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

            Assert.Equal(2, batch.Transactions[0].Groups[0].Pages.Count);

            Assert.Equal(1, batch.Transactions[0].Groups[0].Pages[0].DocumentLocator);
            Assert.Equal(1, batch.Transactions[0].Groups[0].Pages[0].BatchSequence);
            Assert.Equal(1, batch.Transactions[0].Groups[0].Pages[0].TransactionSequence);
            Assert.Equal(1, batch.Transactions[0].Groups[0].Pages[0].GroupSequence);
            Assert.Equal(4, batch.Transactions[0].Groups[0].Pages[0].ScanSequence);
            Assert.Equal(new DateTime(2019, 3, 22, 23, 25, 11), batch.Transactions[0].Groups[0].Pages[0].ScanTime);
            Assert.Equal(ItemStatus.Valid, batch.Transactions[0].Groups[0].Pages[0].ItemStatus);
            Assert.False(batch.Transactions[0].Groups[0].Pages[0].IsVirtual);
            Assert.Equal(PageType.Page, batch.Transactions[0].Groups[0].Pages[0].PageType);
            Assert.Equal("Invoice", batch.Transactions[0].Groups[0].Pages[0].PageName);
            Assert.Equal("Signature", batch.Transactions[0].Groups[0].Pages[0].SubPageName);
            Assert.False(batch.Transactions[0].Groups[0].Pages[0].OperatorSelect);
            Assert.Equal("MyBin", batch.Transactions[0].Groups[0].Pages[0].Bin);
            Assert.Equal("20.32 CM", batch.Transactions[0].Groups[0].Pages[0].Length);
            Assert.Equal("11.00 IN", batch.Transactions[0].Groups[0].Pages[0].Height);
            Assert.Null(batch.Transactions[0].Groups[0].Pages[0].EnvelopeDetect);
            Assert.Equal(1.11f, batch.Transactions[0].Groups[0].Pages[0].AverageThickness);
            Assert.Equal(-0.12f, batch.Transactions[0].Groups[0].Pages[0].SkewDegrees);
            Assert.True(batch.Transactions[0].Groups[0].Pages[0].DeskewStatus);
            Assert.False(batch.Transactions[0].Groups[0].Pages[0].FrontStreakDetectStatus);
            Assert.True(batch.Transactions[0].Groups[0].Pages[0].BackStreakDetectStatus);
            Assert.Equal("Queue 3", batch.Transactions[0].Groups[0].Pages[0].PlugInPageMessage);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].AuditTrails);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Barcodes);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Images);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].MarkDetects);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Micrs);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Ocrs);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].ReferenceIds);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Tags);

            Assert.Equal(2, batch.Transactions[0].Groups[0].Pages[1].DocumentLocator);
            Assert.Equal(2, batch.Transactions[0].Groups[0].Pages[1].BatchSequence);
            Assert.Equal(2, batch.Transactions[0].Groups[0].Pages[1].TransactionSequence);
            Assert.Equal(2, batch.Transactions[0].Groups[0].Pages[1].GroupSequence);
            Assert.Equal(5, batch.Transactions[0].Groups[0].Pages[1].ScanSequence);
            Assert.Equal(new DateTime(2019, 3, 22, 23, 25, 18), batch.Transactions[0].Groups[0].Pages[1].ScanTime);
            Assert.Equal(ItemStatus.Void, batch.Transactions[0].Groups[0].Pages[1].ItemStatus);
            Assert.True(batch.Transactions[0].Groups[0].Pages[1].IsVirtual);
            Assert.Equal(PageType.CustomPage1, batch.Transactions[0].Groups[0].Pages[1].PageType);
            Assert.Equal("Affidavit", batch.Transactions[0].Groups[0].Pages[1].PageName);
            Assert.Equal("Notarization", batch.Transactions[0].Groups[0].Pages[1].SubPageName);
            Assert.True(batch.Transactions[0].Groups[0].Pages[1].OperatorSelect);
            Assert.Equal("MyBin2", batch.Transactions[0].Groups[0].Pages[1].Bin);
            Assert.Equal("8.500 IN", batch.Transactions[0].Groups[0].Pages[1].Length);
            Assert.Equal("27.94 CM", batch.Transactions[0].Groups[0].Pages[1].Height);
            Assert.True(batch.Transactions[0].Groups[0].Pages[1].EnvelopeDetect);
            Assert.Equal(1.35f, batch.Transactions[0].Groups[0].Pages[1].AverageThickness);
            Assert.Equal(13.2f, batch.Transactions[0].Groups[0].Pages[1].SkewDegrees);
            Assert.False(batch.Transactions[0].Groups[0].Pages[1].DeskewStatus);
            Assert.True(batch.Transactions[0].Groups[0].Pages[1].FrontStreakDetectStatus);
            Assert.Null(batch.Transactions[0].Groups[0].Pages[1].BackStreakDetectStatus);
            Assert.Equal("Queue 4", batch.Transactions[0].Groups[0].Pages[1].PlugInPageMessage);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].AuditTrails);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Barcodes);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Images);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].MarkDetects);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Micrs);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Ocrs);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].ReferenceIds);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Tags);

            Assert.Equal(1, batch.Transactions[0].Groups[1].Pages.Count);

            Assert.Equal(3, batch.Transactions[0].Groups[1].Pages[0].DocumentLocator);
            Assert.Equal(4, batch.Transactions[0].Groups[1].Pages[0].BatchSequence);
            Assert.Equal(3, batch.Transactions[0].Groups[1].Pages[0].TransactionSequence);
            Assert.Equal(5, batch.Transactions[0].Groups[1].Pages[0].GroupSequence);
            Assert.Equal(6, batch.Transactions[0].Groups[1].Pages[0].ScanSequence);
            Assert.Equal(new DateTime(2019, 3, 22, 23, 25, 11), batch.Transactions[0].Groups[1].Pages[0].ScanTime);
            Assert.Equal(ItemStatus.Valid, batch.Transactions[0].Groups[1].Pages[0].ItemStatus);
            Assert.False(batch.Transactions[0].Groups[1].Pages[0].IsVirtual);
            Assert.Equal(PageType.Page, batch.Transactions[0].Groups[1].Pages[0].PageType);
            Assert.Equal("Invoice", batch.Transactions[0].Groups[1].Pages[0].PageName);
            Assert.Equal("Signature", batch.Transactions[0].Groups[1].Pages[0].SubPageName);
            Assert.False(batch.Transactions[0].Groups[1].Pages[0].OperatorSelect);
            Assert.Equal("MyBin", batch.Transactions[0].Groups[1].Pages[0].Bin);
            Assert.Equal("20.32 CM", batch.Transactions[0].Groups[1].Pages[0].Length);
            Assert.Equal("11.00 IN", batch.Transactions[0].Groups[1].Pages[0].Height);
            Assert.Null(batch.Transactions[0].Groups[1].Pages[0].EnvelopeDetect);
            Assert.Equal(1.11f, batch.Transactions[0].Groups[1].Pages[0].AverageThickness);
            Assert.Equal(-0.12f, batch.Transactions[0].Groups[1].Pages[0].SkewDegrees);
            Assert.True(batch.Transactions[0].Groups[1].Pages[0].DeskewStatus);
            Assert.False(batch.Transactions[0].Groups[1].Pages[0].FrontStreakDetectStatus);
            Assert.True(batch.Transactions[0].Groups[1].Pages[0].BackStreakDetectStatus);
            Assert.Equal("Queue 3", batch.Transactions[0].Groups[1].Pages[0].PlugInPageMessage);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].AuditTrails);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].Barcodes);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].Images);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].MarkDetects);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].Micrs);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].Ocrs);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].ReferenceIds);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].Tags);
        }

        [Fact]
        public async Task ReadBatchAsync_EndInfoPopulated()
        {

            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

            Assert.Equal(new DateTime(2019, 3, 22, 23, 32, 45), batch.EndInfo.EndTime);
            Assert.Equal(4, batch.EndInfo.NumPages);
            Assert.Equal(2, batch.EndInfo.NumGroups);
            Assert.Equal(2, batch.EndInfo.NumTransactions);
            Assert.False(batch.EndInfo.IsModified);
        }

        [Fact(Skip = "placeholder")]
        public async Task ReadBatchAsync_UnexpectedAttributeIgnored()
        {

            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();
        }

        [Fact(Skip = "placeholder")]
        public async Task ReadBatchAsync_UnexpectedElementIgnored()
        {

            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();
        }
    }
}
