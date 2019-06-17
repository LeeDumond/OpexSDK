using System;
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
            var reader = new BatchReader(@"C:/Opex/test.oxi");

            Batch batch = await reader.ReadBatch();

            Assert.NotNull(batch.ReferenceIds);
            Assert.NotNull(batch.Transactions);
        }
    }
}
