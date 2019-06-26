using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using System.Xml.Schema;
using OpexSDK.Models;
using Xunit;

namespace OpexSDK.Tests
{
    public class SchemaValidationTests : IClassFixture<FileSystemFixture>
    {
        public SchemaValidationTests(FileSystemFixture fileSystemFixture)
        {
            _fileSystemFixture = fileSystemFixture;
        }

        private readonly FileSystemFixture _fileSystemFixture;

        [Fact]
        public void ReadBatch_InvalidSchema()
        {
            var badSchema = @"<This is=""an invalid schema"" />";
            _fileSystemFixture.FileSystem.AddFile(@"C:\Opex\badSchema.xsd", new MockFileData(badSchema));

            var reader = new BatchReader(_fileSystemFixture.FileSystem);

            Batch batch = reader.ReadBatch(@"C:\Opex\test1.oxi", @"C:\Opex\badSchema.xsd");

            Assert.Single(reader.ValidationErrors);
            Assert.Equal(
                "The root element of a W3C XML Schema should be <schema> and its namespace should be 'http://www.w3.org/2001/XMLSchema'.",
                reader.ValidationErrors[0].Message);
            Assert.NotNull(batch);
        }

        [Fact]
        public void ReadBatch_InvalidSchema_ThrowOnError()
        {
            var badSchema = @"<This is=""an invalid schema"" />";
            _fileSystemFixture.FileSystem.AddFile(@"C:\Opex\badSchema.xsd", new MockFileData(badSchema));

            var reader = new BatchReader(_fileSystemFixture.FileSystem);

            void Action()
            {
                reader.ReadBatch(@"C:\Opex\test1.oxi", @"C:\Opex\badSchema.xsd", true);
            }

            Assert.Throws<XmlSchemaException>(Action);
        }

        [Fact]
        public void ReadBatch_UnexpectedAttributeAndElement()
        {
            var batchFileContents =
                @"<Batch FormatVersion=""03.14"" BaseMachine=""MODEL_51"" ScanDevice=""AS3600i"" SoftwareVersion=""02.23.00.05"" TransportId=""MyTransport"" BatchIdentifier=""thisisbatch45"" JobType=""MULTI_WITH_PAGE"" OperatingMode=""MODIFIED"" JobName=""Lockbox 25"" OperatorName=""Lee Dumond"" StartTime=""2019-03-22 23:24:07"" ReceiveDate=""2019-03-21"" ProcessDate=""2019-03-22"" ImageFilePath=""X:\Images\OPEX\somebatchid"" PluginMessage=""XYZ Plug-in"" DeveloperReserved=""1234-56a"" UnexpectedAttribute=""Hello"">
                  <UnexpectedElement></UnexpectedElement>
            </Batch>";

            _fileSystemFixture.FileSystem.AddFile(@"C:\Opex\test.oxi", new MockFileData(batchFileContents));

            var reader = new BatchReader(_fileSystemFixture.FileSystem);

            reader.ReadBatch(@"C:\Opex\test.oxi", @"C:\Opex\schema.xsd");

            Assert.Equal(2, reader.ValidationErrors.Count);
            Assert.Equal("The 'UnexpectedAttribute' attribute is not declared.", reader.ValidationErrors[0].Message);
            Assert.StartsWith("The element 'Batch' has invalid child element 'UnexpectedElement'",
                reader.ValidationErrors[1].Message);
        }

        [Fact]
        public void ReadBatch_UnexpectedAttributeAndElement_ThrowOnError()
        {
            var batchFileContents =
                @"<Batch FormatVersion=""03.14"" BaseMachine=""MODEL_51"" ScanDevice=""AS3600i"" SoftwareVersion=""02.23.00.05"" TransportId=""MyTransport"" BatchIdentifier=""thisisbatch45"" JobType=""MULTI_WITH_PAGE"" OperatingMode=""MODIFIED"" JobName=""Lockbox 25"" OperatorName=""Lee Dumond"" StartTime=""2019-03-22 23:24:07"" ReceiveDate=""2019-03-21"" ProcessDate=""2019-03-22"" ImageFilePath=""X:\Images\OPEX\somebatchid"" PluginMessage=""XYZ Plug-in"" DeveloperReserved=""1234-56a"" UnexpectedAttribute=""Hello"">
                  <UnexpectedElement></UnexpectedElement>
            </Batch>";

            _fileSystemFixture.FileSystem.AddFile(@"C:\Opex\test.oxi", new MockFileData(batchFileContents));

            var reader = new BatchReader(_fileSystemFixture.FileSystem);

            void Action()
            {
                reader.ReadBatch(@"C:\Opex\test.oxi", @"C:\Opex\schema.xsd", true);
            }

            Assert.Throws<XmlSchemaValidationException>(Action);
        }

        [Fact]
        public void ReadBatch_UnexpectedAttributeAndElementNoValidation()
        {
            var batchFileContents =
                @"<Batch FormatVersion=""03.14"" BaseMachine=""MODEL_51"" ScanDevice=""AS3600i"" SoftwareVersion=""02.23.00.05"" TransportId=""MyTransport"" BatchIdentifier=""thisisbatch45"" JobType=""MULTI_WITH_PAGE"" OperatingMode=""MODIFIED"" JobName=""Lockbox 25"" OperatorName=""Lee Dumond"" StartTime=""2019-03-22 23:24:07"" ReceiveDate=""2019-03-21"" ProcessDate=""2019-03-22"" ImageFilePath=""X:\Images\OPEX\somebatchid"" PluginMessage=""XYZ Plug-in"" DeveloperReserved=""1234-56a"" UnexpectedAttribute=""Hello"">
                  <UnexpectedElement></UnexpectedElement>
            </Batch>";

            _fileSystemFixture.FileSystem.AddFile(@"C:\Opex\test.oxi", new MockFileData(batchFileContents));

            var reader = new BatchReader(_fileSystemFixture.FileSystem);

            reader.ReadBatch(@"C:\Opex\test.oxi");

            Assert.Empty(reader.ValidationErrors);
        }

        [Fact]
        public async Task ReadBatchAsync_InvalidSchema()
        {
            var badSchema = @"<This is=""an invalid schema"" />";
            _fileSystemFixture.FileSystem.AddFile(@"C:\Opex\badSchema.xsd", new MockFileData(badSchema));

            var reader = new BatchReader(_fileSystemFixture.FileSystem);

            Batch batch = await reader.ReadBatchAsync(@"C:\Opex\test1.oxi", @"C:\Opex\badSchema.xsd");

            Assert.Single(reader.ValidationErrors);
            Assert.Equal(
                "The root element of a W3C XML Schema should be <schema> and its namespace should be 'http://www.w3.org/2001/XMLSchema'.",
                reader.ValidationErrors[0].Message);
            Assert.NotNull(batch);
        }

        [Fact]
        public async Task ReadBatchAsync_UnexpectedAttributeAndElement()
        {
            var batchFileContents =
                @"<Batch FormatVersion=""03.14"" BaseMachine=""MODEL_51"" ScanDevice=""AS3600i"" SoftwareVersion=""02.23.00.05"" TransportId=""MyTransport"" BatchIdentifier=""thisisbatch45"" JobType=""MULTI_WITH_PAGE"" OperatingMode=""MODIFIED"" JobName=""Lockbox 25"" OperatorName=""Lee Dumond"" StartTime=""2019-03-22 23:24:07"" ReceiveDate=""2019-03-21"" ProcessDate=""2019-03-22"" ImageFilePath=""X:\Images\OPEX\somebatchid"" PluginMessage=""XYZ Plug-in"" DeveloperReserved=""1234-56a"" UnexpectedAttribute=""Hello"">
                  <UnexpectedElement></UnexpectedElement>
            </Batch>";

            _fileSystemFixture.FileSystem.AddFile(@"C:\Opex\test.oxi", new MockFileData(batchFileContents));

            var reader = new BatchReader(_fileSystemFixture.FileSystem);

            await reader.ReadBatchAsync(@"C:\Opex\test.oxi", @"C:\Opex\schema.xsd");

            Assert.Equal(2, reader.ValidationErrors.Count);
            Assert.Equal("The 'UnexpectedAttribute' attribute is not declared.", reader.ValidationErrors[0].Message);
            Assert.StartsWith("The element 'Batch' has invalid child element 'UnexpectedElement'",
                reader.ValidationErrors[1].Message);
        }

        [Fact]
        public async Task ReadBatchAsync_UnexpectedAttributeAndElementNoValidation()
        {
            var batchFileContents =
                @"<Batch FormatVersion=""03.14"" BaseMachine=""MODEL_51"" ScanDevice=""AS3600i"" SoftwareVersion=""02.23.00.05"" TransportId=""MyTransport"" BatchIdentifier=""thisisbatch45"" JobType=""MULTI_WITH_PAGE"" OperatingMode=""MODIFIED"" JobName=""Lockbox 25"" OperatorName=""Lee Dumond"" StartTime=""2019-03-22 23:24:07"" ReceiveDate=""2019-03-21"" ProcessDate=""2019-03-22"" ImageFilePath=""X:\Images\OPEX\somebatchid"" PluginMessage=""XYZ Plug-in"" DeveloperReserved=""1234-56a"" UnexpectedAttribute=""Hello"">
                  <UnexpectedElement></UnexpectedElement>
            </Batch>";

            _fileSystemFixture.FileSystem.AddFile(@"C:\Opex\test.oxi", new MockFileData(batchFileContents));

            var reader = new BatchReader(_fileSystemFixture.FileSystem);

            await reader.ReadBatchAsync(@"C:\Opex\test.oxi");

            Assert.Empty(reader.ValidationErrors);
        }
    }
}