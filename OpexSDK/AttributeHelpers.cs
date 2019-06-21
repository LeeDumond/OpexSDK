using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;
using OpexSDK.Enumerations;

[assembly: InternalsVisibleTo("OpexSDK.Tests")]

namespace OpexSDK
{
    internal static class AttributeHelpers
    {
        internal static DateTime? GetDateTime(string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeValue))
            {
                return null;
            }

            return XmlConvert.ToDateTime(attributeValue, new string[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd" });
        }

        internal static int? GetInt(string attributeValue)
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

        internal static bool? GetBooleanFromTrueFalse(string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeValue))
            {
                return null;
            }

            return Convert.ToBoolean(attributeValue);
        }

        internal static bool? GetBooleanFromYesNo(string attributeValue)
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

        internal static PageType? GetPageType(string attributeValue)
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

        internal static float? GetFloat(string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeValue))
            {
                return null;
            }

            return Convert.ToSingle(attributeValue);
        }


        internal static RescanStatus? GetRescanStatus(string attributeValue)
        {
            switch (attributeValue)
            {
                case "RESCAN":
                    return RescanStatus.Rescan;
                case "NOT_RESCAN":
                    return RescanStatus.NotRescan;
                case "":
                case null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeValue));
            }
        }

        internal static EnvelopeDetect? GetEnvelopeDetect(string attributeValue)
        {
            return (EnvelopeDetect?)GetYesNoInactive(attributeValue);
        }

        internal static DeskewStatus? GetDeskewStatus(string attributeValue)
        {
            return (DeskewStatus?)GetYesNoInactive(attributeValue);
        }

        internal static FrontStreakDetectStatus? GetFrontStreakDetectStatus(string attributeValue)
        {
            return (FrontStreakDetectStatus?)GetYesNoInactive(attributeValue);
        }

        internal static BackStreakDetectStatus? GetBackStreakDetectStatus(string attributeValue)
        {
            return (BackStreakDetectStatus?) GetYesNoInactive(attributeValue);
        }

        private static int? GetYesNoInactive(string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeValue))
            {
                return null;
            }

            if (attributeValue == "YES")
            {
                return 0;
            }

            if (attributeValue == "NO")
            {
                return 1;
            }

            if (attributeValue == "INACTIVE")
            {
                return 2;
            }

            throw new ArgumentOutOfRangeException(nameof(attributeValue));
        }

        internal static ScantimeFinalBlankAreaDecision? GetScantimeFinalBlankAreaDecision(string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeValue))
            {
                return null;
            }

            if (attributeValue == "BLANK")
            {
                return ScantimeFinalBlankAreaDecision.Blank;
            }

            if (attributeValue == "NOT_BLANK")
            {
                return ScantimeFinalBlankAreaDecision.NotBlank;
            }

            if (attributeValue == "UNDETERMINED")
            {
                return ScantimeFinalBlankAreaDecision.Undetermined;
            }

            throw new ArgumentOutOfRangeException(nameof(attributeValue));
        }

        internal static AuditTrailType? GetAuditTrailType(string attributeValue)
        {
            switch (attributeValue)
            {
                case "ELECTRONIC":
                    return AuditTrailType.Electronic;
                case "PRINTED":
                    return AuditTrailType.Printed;
                case "":
                case null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeValue));
            }
        }

        internal static Side? GetSide(string attributeValue)
        {
            switch (attributeValue)
            {
                case "FRONT":
                    return Side.Front;
                case "BACK":
                    return Side.Back;
                case "":
                case null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeValue));
            }
        }

        public static ImageType? GetImageType(string attributeValue)
        {
            switch (attributeValue)
            {
                case "FULL":
                    return ImageType.Full;
                case "SNIPPET":
                    return ImageType.Snippet;
                case "":
                case null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeValue));
            }
        }

        public static ImageDepth? GetImageDepth(string attributeValue)
        {
            switch (attributeValue)
            {
                case "1":
                    return ImageDepth.Bitonal;
                case "8":
                    return ImageDepth.Grayscale;
                case "24":
                    return ImageDepth.Color;
                case "":
                case null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeValue));
            }
        }
    }
}