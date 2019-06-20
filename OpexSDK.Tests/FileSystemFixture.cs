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