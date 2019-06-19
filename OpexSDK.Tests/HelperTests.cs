using System;
using OpexSDK.Enumerations;
using Xunit;

namespace OpexSDK.Tests
{
    
    public class HelperTests
    {
        [Fact]
        public void GetJobType_ReturnsCorrectJobType()
        {
            Assert.Equal(JobType.Single, Helpers.GetJobType("SINGLE"));
            Assert.Equal(JobType.Multi, Helpers.GetJobType("MULTI"));
            Assert.Equal(JobType.StubOnly, Helpers.GetJobType("STUB_ONLY"));
            Assert.Equal(JobType.CheckOnly, Helpers.GetJobType("CHECK_ONLY"));
            Assert.Equal(JobType.MultiWithPage, Helpers.GetJobType("MULTI_WITH_PAGE"));
            Assert.Equal(JobType.PageOnly, Helpers.GetJobType("PAGE_ONLY"));
            Assert.Equal(JobType.Unstructured, Helpers.GetJobType("UNSTRUCTURED"));
            Assert.Equal(JobType.Structured, Helpers.GetJobType("STRUCTURED"));
            Assert.Null(Helpers.GetJobType(""));
            Assert.Null(Helpers.GetJobType(null));

            Assert.Throws<ArgumentOutOfRangeException>(() => Helpers.GetJobType("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetOperatingMode_ReturnsCorrectOperatingMode()
        {
            Assert.Equal(OperatingMode.ManualScan, Helpers.GetOperatingMode("MANUAL_SCAN"));
            Assert.Equal(OperatingMode.Modified, Helpers.GetOperatingMode("MODIFIED"));
            Assert.Null(Helpers.GetOperatingMode(""));
            Assert.Null(Helpers.GetOperatingMode(null));

            Assert.Throws<ArgumentOutOfRangeException>(() => Helpers.GetOperatingMode("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetItemStatus_ReturnsCorrectItemStatus()
        {
            Assert.Equal(ItemStatus.Valid, Helpers.GetItemStatus("VALID"));
            Assert.Equal(ItemStatus.Void, Helpers.GetItemStatus("VOID"));
            Assert.Equal(ItemStatus.VoidMarked, Helpers.GetItemStatus("VOID MARKED"));
            Assert.Null(Helpers.GetItemStatus(""));
            Assert.Null(Helpers.GetItemStatus(null));

            Assert.Throws<ArgumentOutOfRangeException>(() => Helpers.GetItemStatus("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetPageType_ReturnsCorrectPageType()
        {
            Assert.Equal(PageType.BatchTicket, Helpers.GetPageTypeFromAttribute("BATCH_TICKET"));
            Assert.Equal(PageType.PersonalCheck, Helpers.GetPageTypeFromAttribute("PERSONAL_CHECK"));
            Assert.Equal(PageType.BusinessCheck, Helpers.GetPageTypeFromAttribute("BUSINESS_CHECK"));
            Assert.Equal(PageType.MoneyOrder, Helpers.GetPageTypeFromAttribute("MONEY_ORDER"));
            Assert.Equal(PageType.Stub, Helpers.GetPageTypeFromAttribute("STUB"));
            Assert.Equal(PageType.Page, Helpers.GetPageTypeFromAttribute("PAGE"));
            Assert.Equal(PageType.Envelope, Helpers.GetPageTypeFromAttribute("ENVELOPE"));
            Assert.Equal(PageType.CheckList, Helpers.GetPageTypeFromAttribute("CHECK_LIST"));
            Assert.Equal(PageType.Cash, Helpers.GetPageTypeFromAttribute("CASH"));
            Assert.Equal(PageType.CustomPage1, Helpers.GetPageTypeFromAttribute("CUSTOM_PAGE1"));
            Assert.Equal(PageType.CustomPage2, Helpers.GetPageTypeFromAttribute("CUSTOM_PAGE2"));
            Assert.Equal(PageType.CustomPage3, Helpers.GetPageTypeFromAttribute("CUSTOM_PAGE3"));
            
            Assert.Null(Helpers.GetPageTypeFromAttribute(""));
            Assert.Null(Helpers.GetPageTypeFromAttribute(null));

            Assert.Throws<ArgumentOutOfRangeException>(() => Helpers.GetPageTypeFromAttribute("SOME_RANDOM_STRING"));
        }

        [Fact]
        public void GetTimeFromAttribute_ReturnsCorrectTime()
        {
            Assert.Equal(new DateTime(2019, 3, 22, 23, 24, 07), Helpers.GetTimeFromAttribute("2019-03-22 23:24:07"));
            Assert.Equal(new DateTime(2019, 3, 22), Helpers.GetTimeFromAttribute("2019-03-22"));
            Assert.Null(Helpers.GetTimeFromAttribute(""));
            Assert.Null(Helpers.GetTimeFromAttribute(null));

            Assert.Throws<FormatException>(() => Helpers.GetTimeFromAttribute("not_a_time"));
        }

        [Fact]
        public void GetIntFromAttribute_ReturnsCorrectInt()
        {
            Assert.Equal(12345, Helpers.GetIntFromAttribute("12345"));
            Assert.Null(Helpers.GetIntFromAttribute(""));
            Assert.Null(Helpers.GetIntFromAttribute(null));

            Assert.Throws<FormatException>(() => Helpers.GetIntFromAttribute("not_an_int"));
        }

        [Fact]
        public void GetFloatFromAttribute_ReturnsCorrectFloat()
        {
            Assert.Equal(12345, Helpers.GetFloatFromAttribute("12345"));
            Assert.Equal(123.45f, Helpers.GetFloatFromAttribute("123.45"));
            Assert.Equal(-123.45f, Helpers.GetFloatFromAttribute("-123.45"));
            Assert.Equal(0.45f, Helpers.GetFloatFromAttribute("0.45"));
            Assert.Equal(0.45f, Helpers.GetFloatFromAttribute(".45"));
            Assert.Equal(-0.45f, Helpers.GetFloatFromAttribute("-0.45"));
            Assert.Equal(-0.45f, Helpers.GetFloatFromAttribute("-.45"));
            Assert.Null(Helpers.GetFloatFromAttribute(""));
            Assert.Null(Helpers.GetFloatFromAttribute(null));

            Assert.Throws<FormatException>(() => Helpers.GetFloatFromAttribute("not_a_float"));
        }

        [Fact]
        public void GetBooleanFromTrueFalseAttribute_ReturnsCorrectBoolean()
        {
            Assert.True(Helpers.GetBooleanFromTrueFalseAttribute("TRUE"));
            Assert.False(Helpers.GetBooleanFromTrueFalseAttribute("FALSE"));
            Assert.Null(Helpers.GetBooleanFromTrueFalseAttribute(""));
            Assert.Null(Helpers.GetBooleanFromTrueFalseAttribute(null));

            Assert.Throws<FormatException>(() => Helpers.GetBooleanFromTrueFalseAttribute("not_a_bool"));
        }

        [Fact]
        public void GetBooleanFromYesNoAttribute_ReturnsCorrectBoolean()
        {
            Assert.True(Helpers.GetBooleanFromYesNoAttribute("YES"));
            Assert.False(Helpers.GetBooleanFromYesNoAttribute("NO"));
            Assert.Null(Helpers.GetBooleanFromYesNoAttribute("INACTIVE"));
            Assert.Null(Helpers.GetBooleanFromYesNoAttribute(""));
            Assert.Null(Helpers.GetBooleanFromYesNoAttribute(null));

            Assert.Throws<FormatException>(() => Helpers.GetBooleanFromYesNoAttribute("not_a_yes_or_no"));
        }


    }
}