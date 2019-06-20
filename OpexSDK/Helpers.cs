using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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

        internal static int? GetIntFromAttribute(string attributeValue)
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

        internal static bool? GetBooleanFromTrueFalseAttribute(string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeValue))
            {
                return null;
            }

            return Convert.ToBoolean(attributeValue);
        }

        internal static bool? GetBooleanFromYesNoAttribute(string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeValue))
            {
                return null;
            }

            if (attributeValue.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (attributeValue.Equals("no", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (attributeValue.Equals("inactive", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            throw new FormatException("Value must be yes or no.");
        }

        internal static ItemStatus? GetItemStatus(string attributeValue)
        {
            switch (attributeValue)
            {
                case "VALID":
                    return ItemStatus.Valid;
                case "VOID":
                    return ItemStatus.Void;
                case "VOID MARKED":
                    return ItemStatus.VoidMarked;
                case "":
                case null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeValue));
            }
        }

        internal static PageType? GetPageTypeFromAttribute(string attributeValue)
        {
            switch (attributeValue)
            {
                case "BATCH_TICKET":
                    return PageType.BatchTicket;
                case "PERSONAL_CHECK":
                    return PageType.PersonalCheck;
                case "BUSINESS_CHECK":
                    return PageType.BusinessCheck;
                case "MONEY_ORDER":
                    return PageType.MoneyOrder;
                case "STUB":
                    return PageType.Stub;
                case "PAGE":
                    return PageType.Page;
                case "ENVELOPE":
                    return PageType.Envelope;
                case "CHECK_LIST":
                    return PageType.CheckList;
                case "CASH":
                    return PageType.Cash;
                case "CUSTOM_PAGE1":
                    return PageType.CustomPage1;
                case "CUSTOM_PAGE2":
                    return PageType.CustomPage2;
                case "CUSTOM_PAGE3":
                    return PageType.CustomPage3;
                case "":
                case null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeValue));
            }
        }

        internal static float? GetFloatFromAttribute(string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeValue))
            {
                return null;
            }

            return Convert.ToSingle(attributeValue);
        }


        public static bool? GetRescanStatusFromAttribute(string attributeValue)
        {
            switch (attributeValue)
            {
                case "RESCAN":
                    return true;
                case "NOT_RESCAN":
                    return false;
                case "":
                case null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeValue));
            }
        }
    }
}