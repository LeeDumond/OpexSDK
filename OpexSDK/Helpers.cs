using System;
using System.Runtime.CompilerServices;
using System.Xml;
using OpexSDK.Enumerations;

[assembly: InternalsVisibleTo("OpexSDK.Tests")]

namespace OpexSDK
{
    internal static class Helpers
    {
        internal static DateTime? GetTimeFromAttribute(string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeValue))
            {
                return null;
            }

            return XmlConvert.ToDateTime(attributeValue, new string[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd" });
        }

        public static int? GetIntFromAttribute(string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeValue))
            {
                return null;
            }

            return Convert.ToInt32(attributeValue);
        }

        internal static OperatingMode? GetOperatingMode(string attributeValue)
        {
            switch (attributeValue)
            {
                case "MANUAL_SCAN":
                    return OperatingMode.ManualScan;
                case "MODIFIED":
                    return OperatingMode.Modified;
                case "":
                case null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeValue));
            }
        }

        internal static JobType? GetJobType(string attributeValue)
        {
            switch (attributeValue)
            {
                case "SINGLE":
                    return JobType.Single;
                case "MULTI":
                    return JobType.Multi;
                case "STUB_ONLY":
                    return JobType.StubOnly;
                case "CHECK_ONLY":
                    return JobType.CheckOnly;
                case "MULTI_WITH_PAGE":
                    return JobType.MultiWithPage;
                case "PAGE_ONLY":
                    return JobType.PageOnly;
                case "UNSTRUCTURED":
                    return JobType.Unstructured;
                case "STRUCTURED":
                    return JobType.Structured;
                case "":
                case null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeValue));
            }
        }

        public static bool? GetBooleanFromAttribute(string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeValue))
            {
                return null;
            }

            return Convert.ToBoolean(attributeValue);
        }
    }
}