using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using OpexSDK.Models;
using Xunit;

namespace OpexSDK.Tests
{
    public class BatchReaderTests
    {
        [Fact]
        public void ReadBatch_NullPath_Throws()
        {
            Action action = () => new BatchReader(null);

            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void ReadBatch_EmptyPath_Throws()
        {
            Action action = () => new BatchReader("");

            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public async Task ReadBatch_AllCollectionsInitialized()
        {
            string batchFileContents = "<BATCH></BATCH>";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\Opex\test.oxi", new MockFileData(batchFileContents) },
                { @"c:\demo\jQuery.js", new MockFileData("some js") },
                { @"c:\demo\image.gif", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });


            var reader = new BatchReader(@"C:\Opex\test.oxi", fileSystem);

            Batch batch = await reader.ReadBatch();

            Assert.NotNull(batch.ReferenceIds);
            Assert.NotNull(batch.Transactions);
        }
    }
}
