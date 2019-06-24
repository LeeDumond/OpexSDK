﻿using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

namespace OpexSDK.Tests
{
    public class FileSystemFixture
    {
        public FileSystemFixture()
        {
            var test1Contents = @"<?xml version=""1.0"" encoding=""utf-8""?>

<Batch FormatVersion=""03.14"" BaseMachine=""MODEL_51"" ScanDevice=""AS3600i"" SoftwareVersion=""02.23.00.05"" TransportId=""MyTransport"" BatchIdentifier=""thisisbatch45"" JobType=""MULTI_WITH_PAGE""
   OperatingMode=""MODIFIED"" JobName=""Lockbox 25"" OperatorName=""Lee Dumond"" StartTime=""2019-03-22 23:24:07"" ReceiveDate=""2019-03-21"" ProcessDate=""2019-03-22"" ImageFilePath=""X:\Images\OPEX\somebatchid""
   PluginMessage=""XYZ Plug-in"" DeveloperReserved=""1234-56a"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
   xsi:noNamespaceSchemaLocation=""oxi1_60.xsd"">
    <Transaction TransactionID=""3141"">
        <Group GroupID=""98"">
            <Page DocumentLocator=""1"" TransactionSequence=""1"" GroupSequence=""1"" BatchSequence=""1"" ScanSequence=""4"" ScanTime=""2019-03-22 23:25:11""
              ItemStatus=""VALID"" IsVirtual=""NO"" PageType=""PAGE"" PageName=""Invoice"" SubPageName=""Signature"" OperatorSelect=""NO"" Bin=""MyBin"" Length=""20.32 CM"" 
              Height=""11.00 IN"" EnvelopeDetect=""INACTIVE"" AverageThickness=""1.11"" SkewDegrees=""-0.12"" DeskewStatus=""YES"" FrontStreakDetectStatus=""NO"" 
              BackStreakDetectStatus=""YES"" PlugInPageMessage=""Queue 3"">
                <Image Index=""1"" RescanStatus=""RESCAN"" ScantimeFinalBlankAreaDecision=""NOT_BLANK"" Side=""FRONT"" Type=""FULL"" Depth=""8"" Format=""TIFF"" Filename=""12345.tif""
                     Filesize=""1234567"" Length=""1700"" Height=""300"" OffsetLength=""400"" OffsetHeight=""60"" ResolutionLength=""300"" ResolutionHeight=""240"">
                </Image>
                <Image Index=""2"" RescanStatus=""NOT_RESCAN"" ScantimeFinalBlankAreaDecision=""UNDETERMINED"" Side=""BACK"" Type=""SNIPPET"" Depth=""24"" Format=""JPEG"" Filename=""67890.jpg""
                     Filesize=""23456789"" Length=""200"" Height=""1800"" OffsetLength=""0"" OffsetHeight=""240"" ResolutionLength=""200"" ResolutionHeight=""150"">
                </Image>
                <CustomData Entry=""Hello from ScanLink"" />
                <Micr Status=""GOOD"" RtStatus=""GOOD"" CheckType=""US"" Side=""FRONT"" Value=""d031201360d8659741c0401"" />
                <Micr Status=""NO_MICR"" RtStatus=""NOT_FOUND"" CheckType=""UNKNOWN"" Side=""BACK"" Value="""" />
                <Ocr Index=""1"" Side=""FRONT"" Value=""This is the !alue of first read"" Name=""OCR 1"" />
                <Ocr Index=""2"" Side=""BACK"" Value="""" Name=""OCR 2"" />
                <Barcode Index=""1"" Type=""Code 39"" Side=""FRONT"" Value=""08057423"" />
                <Barcode Index=""2"" Type=""EAN-8"" Side=""BACK"" Value=""10110 Jupiter Hills Drive"" />
                <MarkDetect Index=""1"" Side=""BACK"" Result=""YES"" Name=""MARK 1"" />
                <MarkDetect Index=""2"" Side=""FRONT"" Result=""NO"" Name=""MARK 2"" />
                <AuditTrail Type=""PRINTED"" Side=""BACK"" Apply=""TRUE"" Text=""Received"" />
                <AuditTrail Type=""ELECTRONIC"" Side=""FRONT"" Apply=""FALSE"" Text=""CCCIS Inc."" />                     
                <ReferenceID Index=""1"" Response=""High Priority"" Name=""Priority"" />
                <ReferenceID Index=""2"" Response=""Mint Chocolate Chip"" Name=""Ice Cream"" />
                <Tag Source=""MFD Override"" Value=""Medical Record"" />
                <Tag Source=""External Camera"" Value=""Invoice"" />
            </Page>
            <Page DocumentLocator=""2"" TransactionSequence=""2"" GroupSequence=""2"" BatchSequence=""2"" ScanSequence=""5"" ScanTime=""2019-03-22 23:25:18""
              ItemStatus=""VOID"" IsVirtual=""YES"" PageType=""CUSTOM_PAGE1"" PageName=""Affidavit"" SubPageName=""Notarization"" OperatorSelect=""YES"" Bin=""MyBin2"" Length=""8.500 IN"" 
              Height=""27.94 CM"" EnvelopeDetect=""YES"" AverageThickness=""1.35"" SkewDegrees=""13.2"" DeskewStatus=""NO"" FrontStreakDetectStatus=""YES"" 
              BackStreakDetectStatus=""INACTIVE"" PlugInPageMessage=""Queue 4"">
            </Page>
        </Group>
        <Group GroupID=""108"">
            <Page DocumentLocator=""3"" TransactionSequence=""3"" GroupSequence=""5"" BatchSequence=""4"" ScanSequence=""6"" ScanTime=""2019-03-22 23:29:22""
              ItemStatus=""VOID MARKED"" IsVirtual=""YES"" PageType=""ENVELOPE"" PageName=""Lawsuit"" SubPageName=""Cover"" OperatorSelect=""NO"" Bin=""MyBin3"" Length=""11.00 IN"" 
              Height=""21.56 CM"" EnvelopeDetect=""NO"" AverageThickness=""2.14"" SkewDegrees=""-9.5"" DeskewStatus=""INACTIVE"" FrontStreakDetectStatus=""INACTIVE"" 
              BackStreakDetectStatus=""NO"" PlugInPageMessage=""Queue 4"">
            </Page>
        </Group>
    </Transaction>
    <Transaction TransactionID=""3152"">
        <Group GroupID=""125"">
        </Group>
    </Transaction>
    <ReferenceID Index=""1"" Response=""High Priority"" Name=""Batch 1234"" />
    <ReferenceID Index=""2"" Response=""Normal Priority"" Name=""Batch 5678"" />
    <EndInfo EndTime=""2019-03-22 23:32:45"" NumPages=""4"" NumGroups=""2"" NumTransactions=""2"" IsModified=""FALSE"" />
</Batch>";

            var schemaContents = @"<?xml version=""1.0"" encoding=""utf-8""?>

<xs:schema attributeFormDefault=""unqualified"" elementFormDefault=""qualified"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element name=""Batch"">
    <xs:complexType>
      <xs:sequence>
        <xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""Transaction"">
          <xs:complexType>
			  <xs:sequence>
				<xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""Group"">
				  <xs:complexType>
					<xs:sequence>
					  <xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""Page"">
						<xs:complexType>
						  <xs:sequence>
							<xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""Image"">
							  <xs:complexType>
								<xs:attribute name=""Index"" type=""xs:unsignedShort"" use=""required"" />					  
								<xs:attribute name=""RescanStatus"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""ScantimeFinalBlankAreaDecision"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Side"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Type"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Depth"" type=""xs:unsignedByte"" use=""required"" />
								<xs:attribute name=""Format"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Filename"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Filesize"" type=""xs:unsignedInt"" use=""required"" />
								<xs:attribute name=""Length"" type=""xs:unsignedShort"" use=""required"" />
								<xs:attribute name=""Height"" type=""xs:unsignedShort"" use=""required"" />
								<xs:attribute name=""OffsetLength"" type=""xs:unsignedShort"" use=""required"" />
								<xs:attribute name=""OffsetHeight"" type=""xs:unsignedShort"" use=""required"" />
								<xs:attribute name=""ResolutionLength"" type=""xs:unsignedShort"" use=""required"" />
								<xs:attribute name=""ResolutionHeight"" type=""xs:unsignedShort"" use=""required"" />
							  </xs:complexType>
							</xs:element>
							<xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""CustomData"">
							  <xs:complexType>
								<xs:attribute name=""Entry"" type=""xs:string"" use=""required"" />					  
							  </xs:complexType>
							</xs:element>
							<xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""Micr"">
							  <xs:complexType>
								<xs:attribute name=""Status"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""RtStatus"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""CheckType"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Side"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Value"" type=""xs:string"" use=""required"" />
							  </xs:complexType>
							</xs:element>
							<xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""Ocr"">
							  <xs:complexType>
								<xs:attribute name=""Index"" type=""xs:unsignedByte"" use=""required"" />
								<xs:attribute name=""Side"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Value"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Name"" type=""xs:string"" use=""required"" />                        
							  </xs:complexType>
							</xs:element>
							<xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""Barcode"">
							  <xs:complexType>
								<xs:attribute name=""Index"" type=""xs:unsignedByte"" use=""required"" />
								<xs:attribute name=""Type"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Side"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Value"" type=""xs:string"" use=""required"" />
							  </xs:complexType>
							</xs:element>
							<xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""MarkDetect"">
							  <xs:complexType>
								<xs:attribute name=""Index"" type=""xs:unsignedByte"" use=""required"" />
								<xs:attribute name=""Side"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Result"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Name"" type=""xs:string"" use=""required"" />                                             
							  </xs:complexType>
							</xs:element>
							<xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""AuditTrail"">
							  <xs:complexType>
								<xs:attribute name=""Type"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Side"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Text"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Apply"" type=""xs:string"" use=""required"" />
							  </xs:complexType>
							</xs:element>
							<xs:element minOccurs=""0"" maxOccurs=""3"" name=""ReferenceID"">
							  <xs:complexType>
								<xs:attribute name=""Index"" type=""xs:unsignedByte"" use=""required"" />
								<xs:attribute name=""Response"" type=""xs:string"" use=""required"" />
								<xs:attribute name=""Name"" type=""xs:string"" use=""required"" />                        
							  </xs:complexType>
							</xs:element>
						  </xs:sequence>
							<xs:attribute name=""DocumentLocator"" type=""xs:unsignedInt"" use=""required"" />				  
							<xs:attribute name=""BatchSequence"" type=""xs:unsignedInt"" use=""required"" />
							<xs:attribute name=""TransactionSequence"" type=""xs:unsignedInt"" use=""required"" />
							<xs:attribute name=""ScanSequence"" type=""xs:unsignedInt"" use=""required"" />
							<xs:attribute name=""GroupSequence"" type=""xs:unsignedInt"" use=""required"" />
							<xs:attribute name=""ItemStatus"" type=""xs:string"" use=""required"" />
							<xs:attribute name=""IsVirtual"" type=""xs:string"" use=""required"" />
							<xs:attribute name=""PageType"" type=""xs:string"" use=""required"" />
							<xs:attribute name=""PageName"" type=""xs:string"" use=""required"" />
							<xs:attribute name=""SubPageName"" type=""xs:string"" use=""required"" />
							<xs:attribute name=""OperatorSelect"" type=""xs:string"" use=""required"" />				  
							<xs:attribute name=""Bin"" type=""xs:string"" use=""required"" />
							<xs:attribute name=""Length"" type=""xs:string"" use=""required"" />
							<xs:attribute name=""Height"" type=""xs:string"" use=""required"" />
							<xs:attribute name=""EnvelopeDetect"" type=""xs:string"" use=""required"" />
							<xs:attribute name=""AverageThickness"" type=""xs:decimal"" use=""required"" />
							<xs:attribute name=""SkewDegrees"" type=""xs:decimal"" use=""required"" />
							<xs:attribute name=""DeskewStatus"" type=""xs:string"" use=""required"" />
							<xs:attribute name=""FrontStreakDetectStatus"" type=""xs:string"" use=""required"" />
							<xs:attribute name=""BackStreakDetectStatus"" type=""xs:string"" use=""required"" />
							<xs:attribute name=""PlugInPageMessage"" type=""xs:string"" use=""required"" />
							<xs:attribute name=""ScanTime"" type=""xs:string"" use=""required"" />					
						</xs:complexType>
					  </xs:element>
					</xs:sequence>
					<xs:attribute name=""GroupID"" type=""xs:unsignedInt"" use=""required"" />
				  </xs:complexType>
				</xs:element>			  
			  </xs:sequence>
            <xs:attribute name=""TransactionID"" type=""xs:unsignedInt"" use=""required"" />
          </xs:complexType>
        </xs:element>
        <xs:element minOccurs=""0"" maxOccurs=""3"" name=""ReferenceID"">
          <xs:complexType>
            <xs:attribute name=""Index"" type=""xs:unsignedByte"" use=""required"" />
            <xs:attribute name=""Response"" type=""xs:string"" use=""required"" />
            <xs:attribute name=""Name"" type=""xs:string"" use=""required"" />              
          </xs:complexType>
        </xs:element>		
        <xs:element minOccurs=""1"" maxOccurs=""1"" name=""EndInfo"">
          <xs:complexType>
				<xs:attribute name=""EndTime"" type=""xs:string"" use=""required"" />
				<xs:attribute name=""NumPages"" type=""xs:unsignedInt"" use=""required"" />
				<xs:attribute name=""NumGroups"" type=""xs:unsignedInt"" use=""required"" />
				<xs:attribute name=""NumTransactions"" type=""xs:unsignedInt"" use=""required"" />
				<xs:attribute name=""IsModified"" type=""xs:string"" use=""required"" />			
          </xs:complexType>
        </xs:element>
      </xs:sequence>
      <xs:attribute name=""FormatVersion"" type=""xs:decimal"" use=""required"" />
      <xs:attribute name=""BaseMachine"" type=""xs:string"" use=""required"" />
      <xs:attribute name=""ScanDevice"" type=""xs:string"" use=""required"" />
      <xs:attribute name=""SoftwareVersion"" type=""xs:string"" use=""required"" />
      <xs:attribute name=""TransportId"" type=""xs:string"" use=""required"" />
      <xs:attribute name=""BatchIdentifier"" type=""xs:string"" use=""required"" />
      <xs:attribute name=""JobType"" type=""xs:string"" use=""required"" />
      <xs:attribute name=""OperatingMode"" type=""xs:string"" use=""required"" />
      <xs:attribute name=""JobName"" type=""xs:string"" use=""required"" />
      <xs:attribute name=""OperatorName"" type=""xs:string"" use=""required"" />
      <xs:attribute name=""StartTime"" type=""xs:string"" use=""required"" />
      <xs:attribute name=""ReceiveDate"" type=""xs:date"" use=""required"" />
      <xs:attribute name=""ProcessDate"" type=""xs:date"" use=""required"" />
      <xs:attribute name=""ImageFilePath"" type=""xs:string"" use=""required"" />
      <xs:attribute name=""PluginMessage"" type=""xs:string"" use=""required"" />
      <xs:attribute name=""DeveloperReserved"" type=""xs:string"" use=""required"" />  
    </xs:complexType>
  </xs:element>
</xs:schema>";

            FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"C:\Opex\test1.oxi", new MockFileData(test1Contents)},
                {@"C:\Opex\schema.xsd", new MockFileData(schemaContents)}
            });
        }

        public MockFileSystem FileSystem { get; }
    }
}