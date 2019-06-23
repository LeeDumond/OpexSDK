using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

namespace OpexSDK.Tests
{
    public class FileSystemFixture
    {
        public FileSystemFixture()
        {
            string test1Contents = @"<?xml version=""1.0"" encoding=""utf-8""?>

<BATCH FormatVersion=""03.14"" BaseMachine=""MODEL_51"" ScanDevice=""AS3600i"" SoftwareVersion=""02.23.00.05"" TransportId=""MyTransport"" BatchIdentifier=""thisisbatch45"" JobType=""MULTI_WITH_PAGE""
   OperatingMode=""MODIFIED"" JobName=""Lockbox 25"" OperatorName=""Lee Dumond"" StartTime=""2019-03-22 23:24:07"" ReceiveDate=""2019-03-21"" ProcessDate=""2019-03-22"" ImageFilePath=""X:\Images\OPEX\somebatchid""
   PluginMessage=""XYZ Plug-in"" DeveloperReserved=""1234-56a"">
    <REFERENCEID Index=""1"" Response=""High Priority"" Name=""Batch 1234"" />
    <REFERENCEID Index=""2"" Response=""Normal Priority"" Name=""Batch 5678"" />
    <TRANSACTION TransactionID=""3141"">
        <GROUP GroupID=""98"">
            <PAGE DocumentLocator=""1"" TransactionSequence=""1"" GroupSequence=""1"" BatchSequence=""1"" ScanSequence=""4"" ScanTime=""2019-03-22 23:25:11""
              ItemStatus=""VALID"" IsVirtual=""NO"" PageType=""PAGE"" PageName=""Invoice"" SubPageName=""Signature"" OperatorSelect=""NO"" Bin=""MyBin"" Length=""20.32 CM"" 
              Height=""11.00 IN"" EnvelopeDetect=""INACTIVE"" AverageThickness=""1.11"" SkewDegrees=""-0.12"" DeskewStatus=""YES"" FrontStreakDetectStatus=""NO"" 
              BackStreakDetectStatus=""YES"" PlugInPageMessage=""Queue 3"">
                <IMAGE Index=""1"" RescanStatus=""RESCAN"" ScantimeFinalBlankAreaDecision=""NOT_BLANK"" Side=""FRONT"" Type=""FULL"" Depth=""8"" Format=""TIFF"" Filename=""12345.tif""
                     Filesize=""1234567"" Length=""1700"" Height=""300"" OffsetLength=""400"" OffsetHeight=""60"" ResolutionLength=""300"" ResolutionHeight=""240"">
                </IMAGE>
                <IMAGE Index=""2"" RescanStatus=""NOT_RESCAN"" ScantimeFinalBlankAreaDecision=""UNDETERMINED"" Side=""BACK"" Type=""SNIPPET"" Depth=""24"" Format=""JPEG"" Filename=""67890.jpg""
                     Filesize=""23456789"" Length=""200"" Height=""1800"" OffsetLength=""0"" OffsetHeight=""240"" ResolutionLength=""200"" ResolutionHeight=""150"">
                </IMAGE>
                <CUSTOMDATA Entry=""Hello from ScanLink"" />
                <MICR Status=""GOOD"" RtStatus=""GOOD"" CheckType=""US"" Side=""FRONT"" Value=""d031201360d8659741c0401"" />
                <MICR Status=""NO_MICR"" RtStatus=""NOT_FOUND"" CheckType=""UNKNOWN"" Side=""BACK"" Value="""" />
                <OCR Index=""1"" Side=""FRONT"" Value=""This is the !alue of first read"" Name=""OCR 1"" />
                <OCR Index=""2"" Side=""BACK"" Value="""" Name=""OCR 2"" />
                <BARCODE Index=""1"" Type=""Code 39"" Side=""FRONT"" Value=""08057423"" />
                <BARCODE Index=""2"" Type=""EAN-8"" Side=""BACK"" Value=""10110 Jupiter Hills Drive"" />
                <MARKDETECT Index=""1"" Side=""BACK"" Result=""YES"" Name=""MARK 1"" />
                <MARKDETECT Index=""2"" Side=""FRONT"" Result=""NO"" Name=""MARK 2"" />
                <AUDITTRAIL Type=""PRINTED"" Side=""BACK"" Apply=""TRUE"" Text=""Received"" />
                <AUDITTRAIL Type=""ELECTRONIC"" Side=""FRONT"" Apply=""FALSE"" Text=""CCCIS Inc."" />
                <TAG Source=""MFD Override"" Value=""Medical Record"" />
                <TAG Source=""External Camera"" Value=""Invoice"" />
                <REFERENCEID Index=""1"" Response=""High Priority"" Name=""Priority"" />
                <REFERENCEID Index=""2"" Response=""Mint Chocolate Chip"" Name=""Ice Cream"" />
            </PAGE>
            <PAGE DocumentLocator=""2"" TransactionSequence=""2"" GroupSequence=""2"" BatchSequence=""2"" ScanSequence=""5"" ScanTime=""2019-03-22 23:25:18""
              ItemStatus=""VOID"" IsVirtual=""YES"" PageType=""CUSTOM_PAGE1"" PageName=""Affidavit"" SubPageName=""Notarization"" OperatorSelect=""YES"" Bin=""MyBin2"" Length=""8.500 IN"" 
              Height=""27.94 CM"" EnvelopeDetect=""YES"" AverageThickness=""1.35"" SkewDegrees=""13.2"" DeskewStatus=""NO"" FrontStreakDetectStatus=""YES"" 
              BackStreakDetectStatus=""INACTIVE"" PlugInPageMessage=""Queue 4"">
            </PAGE>
        </GROUP>
        <GROUP GroupID=""108"">
            <PAGE DocumentLocator=""3"" TransactionSequence=""3"" GroupSequence=""5"" BatchSequence=""4"" ScanSequence=""6"" ScanTime=""2019-03-22 23:29:22""
              ItemStatus=""VOID MARKED"" IsVirtual=""YES"" PageType=""ENVELOPE"" PageName=""Lawsuit"" SubPageName=""Cover"" OperatorSelect=""NO"" Bin=""MyBin3"" Length=""11.00 IN"" 
              Height=""21.56 CM"" EnvelopeDetect=""NO"" AverageThickness=""2.14"" SkewDegrees=""-9.5"" DeskewStatus=""INACTIVE"" FrontStreakDetectStatus=""INACTIVE"" 
              BackStreakDetectStatus=""NO"" PlugInPageMessage=""Queue 4"">
            </PAGE>
        </GROUP>
    </TRANSACTION>
    <TRANSACTION TransactionID=""3152"">
        <GROUP GroupID=""125"">
        </GROUP>
    </TRANSACTION>
    <ENDINFO EndTime=""2019-03-22 23:32:45"" NumPages=""4"" NumGroups=""2"" NumTransactions=""2"" IsModified=""FALSE"" />
</BATCH>";

            string test2Contents = @"<?xml version=""1.0"" encoding=""utf-8""?>

<BATCH FormatVersion=""03.14"" BaseMachine=""MODEL_51"" ScanDevice=""AS3600i"" SoftwareVersion=""02.23.00.05"" TransportId=""MyTransport"" BatchIdentifier=""thisisbatch45"" JobType=""MULTI_WITH_PAGE""
   OperatingMode=""MODIFIED"" JobName=""Lockbox 25"" OperatorName=""Lee Dumond"" StartTime=""2019-03-22 23:24:07"" ReceiveDate=""2019-03-21"" ProcessDate=""2019-03-22"" ImageFilePath=""X:\Images\OPEX\somebatchid""
   PluginMessage=""XYZ Plug-in"" DeveloperReserved=""1234-56a"" UnexpectedAttribute=""Hello"">
    <UNEXPCTEDELEMENT></UNEXPCTEDELEMENT>
</BATCH>";

            FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\Opex\test1.oxi", new MockFileData(test1Contents) },
                { @"C:\Opex\test2.oxi", new MockFileData(test2Contents) }
            });

        }

        public MockFileSystem FileSystem { get; }
    }
}