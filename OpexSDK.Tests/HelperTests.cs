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
    }
}