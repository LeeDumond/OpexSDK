﻿using System;
using OpexSDK.Enumerations;
using Xunit;

namespace OpexSDK.Tests
{
    public class AttributeHelperTests
    {
        [Fact]
        public void GetAuditTrailType_ReturnsCorrectAuditTrailType()
        {
            Assert.Equal(AuditTrailType.Electronic, AttributeHelpers.GetAuditTrailType("ELECTRONIC"));
            Assert.Equal(AuditTrailType.Printed, AttributeHelpers.GetAuditTrailType("PRINTED"));
            Assert.Null(AttributeHelpers.GetAuditTrailType(""));
            Assert.Null(AttributeHelpers.GetAuditTrailType(null));
            Assert.Null(AttributeHelpers.GetAuditTrailType("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetBackStreakDetectStatus_ReturnsCorrectBackStreakDetectStatus()
        {
            Assert.Equal(BackStreakDetectStatus.Yes, AttributeHelpers.GetBackStreakDetectStatus("YES"));
            Assert.Equal(BackStreakDetectStatus.No, AttributeHelpers.GetBackStreakDetectStatus("NO"));
            Assert.Equal(BackStreakDetectStatus.Inactive, AttributeHelpers.GetBackStreakDetectStatus("INACTIVE"));
            Assert.Null(AttributeHelpers.GetBackStreakDetectStatus(""));
            Assert.Null(AttributeHelpers.GetBackStreakDetectStatus(null));
            Assert.Null(AttributeHelpers.GetBackStreakDetectStatus("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetBooleanFromTrueFalse_ReturnsCorrectBoolean()
        {
            Assert.True(AttributeHelpers.GetBooleanFromTrueFalse("TRUE"));
            Assert.False(AttributeHelpers.GetBooleanFromTrueFalse("FALSE"));
            Assert.Null(AttributeHelpers.GetBooleanFromTrueFalse(""));
            Assert.Null(AttributeHelpers.GetBooleanFromTrueFalse(null));
            Assert.Null(AttributeHelpers.GetBooleanFromTrueFalse(null));
            Assert.Null(AttributeHelpers.GetBooleanFromTrueFalse("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetBooleanFromYesNo_ReturnsCorrectBoolean()
        {
            Assert.True(AttributeHelpers.GetBooleanFromYesNo("YES"));
            Assert.False(AttributeHelpers.GetBooleanFromYesNo("NO"));
            Assert.Null(AttributeHelpers.GetBooleanFromYesNo(""));
            Assert.Null(AttributeHelpers.GetBooleanFromYesNo(null));
            Assert.Null(AttributeHelpers.GetBooleanFromYesNo("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetCheckType_ReturnsCorrectCheckType()
        {
            Assert.Equal(CheckType.E13B, AttributeHelpers.GetCheckType("E13B"));
            Assert.Equal(CheckType.US, AttributeHelpers.GetCheckType("US"));
            Assert.Equal(CheckType.Canada, AttributeHelpers.GetCheckType("CANADA"));
            Assert.Equal(CheckType.CMC7, AttributeHelpers.GetCheckType("CMC7"));
            Assert.Equal(CheckType.Unknown, AttributeHelpers.GetCheckType("UNKNOWN"));
            Assert.Null(AttributeHelpers.GetCheckType(""));
            Assert.Null(AttributeHelpers.GetCheckType(null));
            Assert.Null(AttributeHelpers.GetCheckType("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetDateTime_ReturnsCorrectDateTime()
        {
            Assert.Equal(new DateTime(2019, 3, 22, 23, 24, 07), AttributeHelpers.GetDateTime("2019-03-22 23:24:07"));
            Assert.Equal(new DateTime(2019, 3, 22), AttributeHelpers.GetDateTime("2019-03-22"));
            Assert.Null(AttributeHelpers.GetDateTime(""));
            Assert.Null(AttributeHelpers.GetDateTime(null));
            Assert.Null(AttributeHelpers.GetDateTime("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetDeskewStatus_ReturnsCorrectDeskewStatus()
        {
            Assert.Equal(DeskewStatus.Yes, AttributeHelpers.GetDeskewStatus("YES"));
            Assert.Equal(DeskewStatus.No, AttributeHelpers.GetDeskewStatus("NO"));
            Assert.Equal(DeskewStatus.Inactive, AttributeHelpers.GetDeskewStatus("INACTIVE"));
            Assert.Null(AttributeHelpers.GetDeskewStatus(""));
            Assert.Null(AttributeHelpers.GetDeskewStatus(null));
            Assert.Null(AttributeHelpers.GetDeskewStatus("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetEnvelopeDetect_ReturnsCorrectEnvelopeDetect()
        {
            Assert.Equal(EnvelopeDetect.Yes, AttributeHelpers.GetEnvelopeDetect("YES"));
            Assert.Equal(EnvelopeDetect.No, AttributeHelpers.GetEnvelopeDetect("NO"));
            Assert.Equal(EnvelopeDetect.Inactive, AttributeHelpers.GetEnvelopeDetect("INACTIVE"));
            Assert.Null(AttributeHelpers.GetEnvelopeDetect(""));
            Assert.Null(AttributeHelpers.GetEnvelopeDetect(null));
            Assert.Null(AttributeHelpers.GetEnvelopeDetect("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetFloat_ReturnsCorrectFloat()
        {
            Assert.Equal(12345, AttributeHelpers.GetFloat("12345"));
            Assert.Equal(123.45f, AttributeHelpers.GetFloat("123.45"));
            Assert.Equal(-123.45f, AttributeHelpers.GetFloat("-123.45"));
            Assert.Equal(0.45f, AttributeHelpers.GetFloat("0.45"));
            Assert.Equal(0.45f, AttributeHelpers.GetFloat(".45"));
            Assert.Equal(-0.45f, AttributeHelpers.GetFloat("-0.45"));
            Assert.Equal(-0.45f, AttributeHelpers.GetFloat("-.45"));
            Assert.Null(AttributeHelpers.GetFloat(""));
            Assert.Null(AttributeHelpers.GetFloat(null));
            Assert.Null(AttributeHelpers.GetFloat("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetFrontStreakDetectStatus_ReturnsCorrectFrontStreakDetectStatus()
        {
            Assert.Equal(FrontStreakDetectStatus.Yes, AttributeHelpers.GetFrontStreakDetectStatus("YES"));
            Assert.Equal(FrontStreakDetectStatus.No, AttributeHelpers.GetFrontStreakDetectStatus("NO"));
            Assert.Equal(FrontStreakDetectStatus.Inactive, AttributeHelpers.GetFrontStreakDetectStatus("INACTIVE"));
            Assert.Null(AttributeHelpers.GetFrontStreakDetectStatus(""));
            Assert.Null(AttributeHelpers.GetFrontStreakDetectStatus(null));
            Assert.Null(AttributeHelpers.GetFrontStreakDetectStatus("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetImageDepth_ReturnsCorrectImageDepth()
        {
            Assert.Equal(ImageDepth.Bitonal, AttributeHelpers.GetImageDepth("1"));
            Assert.Equal(ImageDepth.Grayscale, AttributeHelpers.GetImageDepth("8"));
            Assert.Equal(ImageDepth.Color, AttributeHelpers.GetImageDepth("24"));
            Assert.Null(AttributeHelpers.GetImageDepth(""));
            Assert.Null(AttributeHelpers.GetImageDepth(null));
            Assert.Null(AttributeHelpers.GetImageDepth("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetImageFormat_ReturnsCorrectImageFormat()
        {
            Assert.Equal(ImageFormat.JPEG, AttributeHelpers.GetImageFormat("JPEG"));
            Assert.Equal(ImageFormat.TIFF, AttributeHelpers.GetImageFormat("TIFF"));
            Assert.Equal(ImageFormat.RAW, AttributeHelpers.GetImageFormat("RAW"));
            Assert.Null(AttributeHelpers.GetImageFormat(""));
            Assert.Null(AttributeHelpers.GetImageFormat(null));
            Assert.Null(AttributeHelpers.GetImageFormat("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetImageResolution_ReturnsCorrectImageResolution()
        {
            Assert.Equal(ImageResolution.Low, AttributeHelpers.GetImageResolution("100"));
            Assert.Equal(ImageResolution.MediumLow, AttributeHelpers.GetImageResolution("150"));
            Assert.Equal(ImageResolution.Medium, AttributeHelpers.GetImageResolution("200"));
            Assert.Equal(ImageResolution.MediumHigh, AttributeHelpers.GetImageResolution("240"));
            Assert.Equal(ImageResolution.High, AttributeHelpers.GetImageResolution("300"));
            Assert.Null(AttributeHelpers.GetImageResolution(""));
            Assert.Null(AttributeHelpers.GetImageResolution(null));
            Assert.Null(AttributeHelpers.GetImageResolution("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetImageType_ReturnsCorrectImageType()
        {
            Assert.Equal(ImageType.Full, AttributeHelpers.GetImageType("FULL"));
            Assert.Equal(ImageType.Snippet, AttributeHelpers.GetImageType("SNIPPET"));
            Assert.Null(AttributeHelpers.GetImageType(""));
            Assert.Null(AttributeHelpers.GetImageType(null));
            Assert.Null(AttributeHelpers.GetImageType("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetInt_ReturnsCorrectInt()
        {
            Assert.Equal(12345, AttributeHelpers.GetInt("12345"));
            Assert.Null(AttributeHelpers.GetInt(""));
            Assert.Null(AttributeHelpers.GetInt(null));
            Assert.Null(AttributeHelpers.GetInt("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetItemStatus_ReturnsCorrectItemStatus()
        {
            Assert.Equal(ItemStatus.Valid, AttributeHelpers.GetItemStatus("VALID"));
            Assert.Equal(ItemStatus.Void, AttributeHelpers.GetItemStatus("VOID"));
            Assert.Equal(ItemStatus.VoidMarked, AttributeHelpers.GetItemStatus("VOID MARKED"));
            Assert.Null(AttributeHelpers.GetItemStatus(""));
            Assert.Null(AttributeHelpers.GetItemStatus(null));
            Assert.Null(AttributeHelpers.GetItemStatus("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetJobType_ReturnsCorrectJobType()
        {
            Assert.Equal(JobType.Single, AttributeHelpers.GetJobType("SINGLE"));
            Assert.Equal(JobType.Multi, AttributeHelpers.GetJobType("MULTI"));
            Assert.Equal(JobType.StubOnly, AttributeHelpers.GetJobType("STUB_ONLY"));
            Assert.Equal(JobType.CheckOnly, AttributeHelpers.GetJobType("CHECK_ONLY"));
            Assert.Equal(JobType.MultiWithPage, AttributeHelpers.GetJobType("MULTI_WITH_PAGE"));
            Assert.Equal(JobType.PageOnly, AttributeHelpers.GetJobType("PAGE_ONLY"));
            Assert.Equal(JobType.Unstructured, AttributeHelpers.GetJobType("UNSTRUCTURED"));
            Assert.Equal(JobType.Structured, AttributeHelpers.GetJobType("STRUCTURED"));
            Assert.Null(AttributeHelpers.GetJobType(""));
            Assert.Null(AttributeHelpers.GetJobType(null));
            Assert.Null(AttributeHelpers.GetJobType("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetLong_ReturnsCorrectLong()
        {
            Assert.Equal(12345, AttributeHelpers.GetLong("12345"));
            Assert.Null(AttributeHelpers.GetLong(""));
            Assert.Null(AttributeHelpers.GetLong(null));
            Assert.Null(AttributeHelpers.GetLong("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetMicrStatus_ReturnsCorrectMicrStatus()
        {
            Assert.Equal(MicrStatus.Good, AttributeHelpers.GetMicrStatus("GOOD"));
            Assert.Equal(MicrStatus.Partial, AttributeHelpers.GetMicrStatus("PARTIAL"));
            Assert.Equal(MicrStatus.Bad, AttributeHelpers.GetMicrStatus("BAD"));
            Assert.Equal(MicrStatus.NoMicr, AttributeHelpers.GetMicrStatus("NO_MICR"));
            Assert.Equal(MicrStatus.Inactive, AttributeHelpers.GetMicrStatus("INACTIVE"));
            Assert.Equal(MicrStatus.Error, AttributeHelpers.GetMicrStatus("ERROR"));
            Assert.Null(AttributeHelpers.GetMicrStatus(""));
            Assert.Null(AttributeHelpers.GetMicrStatus(null));
            Assert.Null(AttributeHelpers.GetMicrStatus("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetOperatingMode_ReturnsCorrectOperatingMode()
        {
            Assert.Equal(OperatingMode.ManualScan, AttributeHelpers.GetOperatingMode("MANUAL_SCAN"));
            Assert.Equal(OperatingMode.Modified, AttributeHelpers.GetOperatingMode("MODIFIED"));
            Assert.Null(AttributeHelpers.GetOperatingMode(""));
            Assert.Null(AttributeHelpers.GetOperatingMode(null));
            Assert.Null(AttributeHelpers.GetOperatingMode("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetPageType_ReturnsCorrectPageType()
        {
            Assert.Equal(PageType.BatchTicket, AttributeHelpers.GetPageType("BATCH_TICKET"));
            Assert.Equal(PageType.PersonalCheck, AttributeHelpers.GetPageType("PERSONAL_CHECK"));
            Assert.Equal(PageType.BusinessCheck, AttributeHelpers.GetPageType("BUSINESS_CHECK"));
            Assert.Equal(PageType.MoneyOrder, AttributeHelpers.GetPageType("MONEY_ORDER"));
            Assert.Equal(PageType.Stub, AttributeHelpers.GetPageType("STUB"));
            Assert.Equal(PageType.Page, AttributeHelpers.GetPageType("PAGE"));
            Assert.Equal(PageType.Envelope, AttributeHelpers.GetPageType("ENVELOPE"));
            Assert.Equal(PageType.CheckList, AttributeHelpers.GetPageType("CHECK_LIST"));
            Assert.Equal(PageType.Cash, AttributeHelpers.GetPageType("CASH"));
            Assert.Equal(PageType.CustomPage1, AttributeHelpers.GetPageType("CUSTOM_PAGE1"));
            Assert.Equal(PageType.CustomPage2, AttributeHelpers.GetPageType("CUSTOM_PAGE2"));
            Assert.Equal(PageType.CustomPage3, AttributeHelpers.GetPageType("CUSTOM_PAGE3"));
            Assert.Null(AttributeHelpers.GetPageType(""));
            Assert.Null(AttributeHelpers.GetPageType(null));
            Assert.Null(AttributeHelpers.GetPageType("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetRescanStatus_ReturnsCorrectItemStatus()
        {
            Assert.Equal(RescanStatus.Rescan, AttributeHelpers.GetRescanStatus("RESCAN"));
            Assert.Equal(RescanStatus.NotRescan, AttributeHelpers.GetRescanStatus("NOT_RESCAN"));
            Assert.Null(AttributeHelpers.GetRescanStatus(""));
            Assert.Null(AttributeHelpers.GetRescanStatus(null));
            Assert.Null(AttributeHelpers.GetRescanStatus("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetRtStatus_ReturnsCorrectRtStatus()
        {
            Assert.Equal(RtStatus.Good, AttributeHelpers.GetRtStatus("GOOD"));
            Assert.Equal(RtStatus.Bad, AttributeHelpers.GetRtStatus("BAD"));
            Assert.Equal(RtStatus.NotFound, AttributeHelpers.GetRtStatus("NOT_FOUND"));
            Assert.Null(AttributeHelpers.GetRtStatus(""));
            Assert.Null(AttributeHelpers.GetRtStatus(null));
            Assert.Null(AttributeHelpers.GetRtStatus("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetScantimeFinalBlankAreaDecision_ReturnsCorrectScantimeFinalBlankAreaDecision()
        {
            Assert.Equal(ScantimeFinalBlankAreaDecision.Blank,
                AttributeHelpers.GetScantimeFinalBlankAreaDecision("BLANK"));
            Assert.Equal(ScantimeFinalBlankAreaDecision.NotBlank,
                AttributeHelpers.GetScantimeFinalBlankAreaDecision("NOT_BLANK"));
            Assert.Equal(ScantimeFinalBlankAreaDecision.Undetermined,
                AttributeHelpers.GetScantimeFinalBlankAreaDecision("UNDETERMINED"));
            Assert.Null(AttributeHelpers.GetScantimeFinalBlankAreaDecision(""));
            Assert.Null(AttributeHelpers.GetScantimeFinalBlankAreaDecision(null));
            Assert.Null(AttributeHelpers.GetScantimeFinalBlankAreaDecision("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetSide_ReturnsCorrectSide()
        {
            Assert.Equal(Side.Front, AttributeHelpers.GetSide("FRONT"));
            Assert.Equal(Side.Back, AttributeHelpers.GetSide("BACK"));
            Assert.Null(AttributeHelpers.GetSide(""));
            Assert.Null(AttributeHelpers.GetSide(null));
            Assert.Null(AttributeHelpers.GetSide("SOME_RANDOM_STRING"));
        }
    }
}