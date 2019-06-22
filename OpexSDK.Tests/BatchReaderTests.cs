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

        public BatchReaderTests(FileSystemFixture fileSystemFixture)
        {
            _fileSystemFixture = fileSystemFixture;
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
                {@"C:\Opex\test.oxi", new MockFileData(batchFileContents)}
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

            Assert.Single(batch.Transactions[1].Groups);

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
            Assert.Equal(EnvelopeDetect.Inactive, batch.Transactions[0].Groups[0].Pages[0].EnvelopeDetect);
            Assert.Equal(1.11f, batch.Transactions[0].Groups[0].Pages[0].AverageThickness);
            Assert.Equal(-0.12f, batch.Transactions[0].Groups[0].Pages[0].SkewDegrees);
            Assert.Equal(DeskewStatus.Yes, batch.Transactions[0].Groups[0].Pages[0].DeskewStatus);
            Assert.Equal(FrontStreakDetectStatus.No, batch.Transactions[0].Groups[0].Pages[0].FrontStreakDetectStatus);
            Assert.Equal(BackStreakDetectStatus.Yes, batch.Transactions[0].Groups[0].Pages[0].BackStreakDetectStatus);
            Assert.Equal("Queue 3", batch.Transactions[0].Groups[0].Pages[0].PlugInPageMessage);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].AuditTrails);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Barcodes);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Images);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].MarkDetects);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Micrs);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Ocrs);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].ReferenceIds);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].Tags);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].CustomData);

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
            Assert.Equal(EnvelopeDetect.Yes, batch.Transactions[0].Groups[0].Pages[1].EnvelopeDetect);
            Assert.Equal(1.35f, batch.Transactions[0].Groups[0].Pages[1].AverageThickness);
            Assert.Equal(13.2f, batch.Transactions[0].Groups[0].Pages[1].SkewDegrees);
            Assert.Equal(DeskewStatus.No, batch.Transactions[0].Groups[0].Pages[1].DeskewStatus);
            Assert.Equal(FrontStreakDetectStatus.Yes, batch.Transactions[0].Groups[0].Pages[1].FrontStreakDetectStatus);
            Assert.Equal(BackStreakDetectStatus.Inactive, batch.Transactions[0].Groups[0].Pages[1].BackStreakDetectStatus);
            Assert.Equal("Queue 4", batch.Transactions[0].Groups[0].Pages[1].PlugInPageMessage);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].AuditTrails);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Barcodes);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Images);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].MarkDetects);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Micrs);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Ocrs);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].ReferenceIds);
            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[1].Tags);
            Assert.Null(batch.Transactions[0].Groups[0].Pages[1].CustomData);

            Assert.Single(batch.Transactions[0].Groups[1].Pages);

            Assert.Equal(3, batch.Transactions[0].Groups[1].Pages[0].DocumentLocator);
            Assert.Equal(4, batch.Transactions[0].Groups[1].Pages[0].BatchSequence);
            Assert.Equal(3, batch.Transactions[0].Groups[1].Pages[0].TransactionSequence);
            Assert.Equal(5, batch.Transactions[0].Groups[1].Pages[0].GroupSequence);
            Assert.Equal(6, batch.Transactions[0].Groups[1].Pages[0].ScanSequence);
            Assert.Equal(new DateTime(2019, 3, 22, 23, 29, 22), batch.Transactions[0].Groups[1].Pages[0].ScanTime);
            Assert.Equal(ItemStatus.VoidMarked, batch.Transactions[0].Groups[1].Pages[0].ItemStatus);
            Assert.True(batch.Transactions[0].Groups[1].Pages[0].IsVirtual);
            Assert.Equal(PageType.Envelope, batch.Transactions[0].Groups[1].Pages[0].PageType);
            Assert.Equal("Lawsuit", batch.Transactions[0].Groups[1].Pages[0].PageName);
            Assert.Equal("Cover", batch.Transactions[0].Groups[1].Pages[0].SubPageName);
            Assert.False(batch.Transactions[0].Groups[1].Pages[0].OperatorSelect);
            Assert.Equal("MyBin3", batch.Transactions[0].Groups[1].Pages[0].Bin);
            Assert.Equal("11.00 IN", batch.Transactions[0].Groups[1].Pages[0].Length);
            Assert.Equal("21.56 CM", batch.Transactions[0].Groups[1].Pages[0].Height);
            Assert.Equal(EnvelopeDetect.No, batch.Transactions[0].Groups[1].Pages[0].EnvelopeDetect);
            Assert.Equal(2.14f, batch.Transactions[0].Groups[1].Pages[0].AverageThickness);
            Assert.Equal(-9.5f, batch.Transactions[0].Groups[1].Pages[0].SkewDegrees);
            Assert.Equal(DeskewStatus.Inactive, batch.Transactions[0].Groups[1].Pages[0].DeskewStatus);
            Assert.Equal(FrontStreakDetectStatus.Inactive, batch.Transactions[0].Groups[1].Pages[0].FrontStreakDetectStatus);
            Assert.Equal(BackStreakDetectStatus.No, batch.Transactions[0].Groups[1].Pages[0].BackStreakDetectStatus);
            Assert.Equal("Queue 4", batch.Transactions[0].Groups[1].Pages[0].PlugInPageMessage);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].AuditTrails);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].Barcodes);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].Images);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].MarkDetects);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].Micrs);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].Ocrs);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].ReferenceIds);
            Assert.NotNull(batch.Transactions[0].Groups[1].Pages[0].Tags);
            Assert.Null(batch.Transactions[0].Groups[1].Pages[0].CustomData);
        }

        [Fact]
        public async Task ReadBatchesAsync_ImagesPopulated()
        {
            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

            Assert.Equal(2, batch.Transactions[0].Groups[0].Pages[0].Images.Count);

            Assert.Equal(1, batch.Transactions[0].Groups[0].Pages[0].Images[0].Index);
            Assert.Equal(RescanStatus.Rescan, batch.Transactions[0].Groups[0].Pages[0].Images[0].RescanStatus);
            Assert.Equal(ScantimeFinalBlankAreaDecision.NotBlank, batch.Transactions[0].Groups[0].Pages[0].Images[0].ScantimeFinalBlankAreaDecision);
            Assert.Equal(Side.Front, batch.Transactions[0].Groups[0].Pages[0].Images[0].Side);
            Assert.Equal(ImageType.Full, batch.Transactions[0].Groups[0].Pages[0].Images[0].Type);
            Assert.Equal(ImageDepth.Grayscale, batch.Transactions[0].Groups[0].Pages[0].Images[0].Depth);
            Assert.Equal(ImageFormat.TIFF, batch.Transactions[0].Groups[0].Pages[0].Images[0].Format);
            Assert.Equal("12345.tif", batch.Transactions[0].Groups[0].Pages[0].Images[0].Filename);
            Assert.Equal(1234567, batch.Transactions[0].Groups[0].Pages[0].Images[0].Filesize);
            Assert.Equal(1700, batch.Transactions[0].Groups[0].Pages[0].Images[0].Length);
            Assert.Equal(300, batch.Transactions[0].Groups[0].Pages[0].Images[0].Height);
            Assert.Equal(400, batch.Transactions[0].Groups[0].Pages[0].Images[0].OffsetLength);
            Assert.Equal(60, batch.Transactions[0].Groups[0].Pages[0].Images[0].OffsetHeight);
            Assert.Equal(ImageResolution.ThreeHundred, batch.Transactions[0].Groups[0].Pages[0].Images[0].ResolutionLength);
            Assert.Equal(ImageResolution.TwoHundredForty, batch.Transactions[0].Groups[0].Pages[0].Images[0].ResolutionHeight);

            Assert.Equal(2, batch.Transactions[0].Groups[0].Pages[0].Images[1].Index);
            Assert.Equal(RescanStatus.NotRescan, batch.Transactions[0].Groups[0].Pages[0].Images[1].RescanStatus);
            Assert.Equal(ScantimeFinalBlankAreaDecision.Undetermined, batch.Transactions[0].Groups[0].Pages[0].Images[1].ScantimeFinalBlankAreaDecision);
            Assert.Equal(Side.Back, batch.Transactions[0].Groups[0].Pages[0].Images[1].Side);
            Assert.Equal(ImageType.Snippet, batch.Transactions[0].Groups[0].Pages[0].Images[1].Type);
            Assert.Equal(ImageDepth.Color, batch.Transactions[0].Groups[0].Pages[0].Images[1].Depth);
            Assert.Equal(ImageFormat.JPEG, batch.Transactions[0].Groups[0].Pages[0].Images[1].Format);
            Assert.Equal("67890.jpg", batch.Transactions[0].Groups[0].Pages[0].Images[1].Filename);
            Assert.Equal(23456789, batch.Transactions[0].Groups[0].Pages[0].Images[1].Filesize);
            Assert.Equal(200, batch.Transactions[0].Groups[0].Pages[0].Images[1].Length);
            Assert.Equal(1800, batch.Transactions[0].Groups[0].Pages[0].Images[1].Height);
            Assert.Equal(0, batch.Transactions[0].Groups[0].Pages[0].Images[1].OffsetLength);
            Assert.Equal(240, batch.Transactions[0].Groups[0].Pages[0].Images[1].OffsetHeight);
            Assert.Equal(ImageResolution.TwoHundred, batch.Transactions[0].Groups[0].Pages[0].Images[1].ResolutionLength);
            Assert.Equal(ImageResolution.OneHundredFifty, batch.Transactions[0].Groups[0].Pages[0].Images[1].ResolutionHeight);

            
        }

        [Fact]
        public async Task ReadBatchAsync_CustomDataPopulated()
        {
            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].CustomData);
            Assert.Equal("Hello from ScanLink", batch.Transactions[0].Groups[0].Pages[0].CustomData.Entry);
        }

        [Fact]
        public async Task ReadBatchesAsync_MicrsPopulated()
        {
            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

            Assert.Equal(2, batch.Transactions[0].Groups[0].Pages[0].Micrs.Count);

            Assert.Equal(MicrStatus.Good, batch.Transactions[0].Groups[0].Pages[0].Micrs[0].Status);
            Assert.Equal(RtStatus.Good, batch.Transactions[0].Groups[0].Pages[0].Micrs[0].RtStatus);
            Assert.Equal(CheckType.US, batch.Transactions[0].Groups[0].Pages[0].Micrs[0].CheckType);
            Assert.Equal(Side.Front, batch.Transactions[0].Groups[0].Pages[0].Micrs[0].Side);
            Assert.Equal("d031201360d8659741c0401", batch.Transactions[0].Groups[0].Pages[0].Micrs[0].Value);

            Assert.Equal(MicrStatus.NoMicr, batch.Transactions[0].Groups[0].Pages[0].Micrs[1].Status);
            Assert.Equal(RtStatus.NotFound, batch.Transactions[0].Groups[0].Pages[0].Micrs[0].RtStatus);
            Assert.Equal(CheckType.Unknown, batch.Transactions[0].Groups[0].Pages[0].Micrs[0].CheckType);
            Assert.Equal(Side.Back, batch.Transactions[0].Groups[0].Pages[0].Micrs[0].Side);
            Assert.Equal("", batch.Transactions[0].Groups[0].Pages[0].Micrs[0].Value);

        }

        [Fact]
        public async Task ReadBatchesAsync_CustomDataPopulated()
        {
            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

            Assert.NotNull(batch.Transactions[0].Groups[0].Pages[0].CustomData);

        }

        [Fact]
        public async Task ReadBatchesAsync_OcrsPopulated()
        {
            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

            Assert.Single(batch.Transactions[0].Groups[0].Pages[0].Ocrs);

        }

        [Fact]
        public async Task ReadBatchesAsync_BarcodesPopulated()
        {
            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

            Assert.Single(batch.Transactions[0].Groups[0].Pages[0].Barcodes);

        }

        [Fact]
        public async Task ReadBatchesAsync_MarkDetectsPopulated()
        {
            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

            Assert.Single(batch.Transactions[0].Groups[0].Pages[0].MarkDetects);

        }

        [Fact]
        public async Task ReadBatchesAsync_AuditTrailsPopulated()
        {
            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

            Assert.Single(batch.Transactions[0].Groups[0].Pages[0].AuditTrails);

        }

        [Fact]
        public async Task ReadBatchesAsync_TagsPopulated()
        {
            var reader = new BatchReader(@"C:\Opex\test1.oxi", _fileSystemFixture.FileSystem);
            Batch batch = await reader.ReadBatchAsync();

            Assert.Single(batch.Transactions[0].Groups[0].Pages[0].Tags);

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

        [Fact]
        public async Task ReadBatchAsync_UnexpectedAttributeAndElementIgnored()
        {
            var reader = new BatchReader(@"C:\Opex\test2.oxi", _fileSystemFixture.FileSystem);
            await reader.ReadBatchAsync();
        }
    }
}