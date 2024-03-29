﻿using System;
using System.Runtime.CompilerServices;
using System.Xml;
using OpexSDK.Enumerations;

[assembly: InternalsVisibleTo("OpexSDK.Tests")]

namespace OpexSDK
{
    internal static class AttributeHelpers
    {
        internal static DateTime? GetDateTime(string attributeValue)
        {
            if (attributeValue == null)
            {
                return null;
            }

            try
            {
                return XmlConvert.ToDateTime(attributeValue, new[] {"yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd"});
            }
            catch (FormatException)
            {
                return null;
            }
        }

        internal static int? GetInt(string attributeValue)
        {
            if (int.TryParse(attributeValue, out int result))
            {
                return result;
            }

            return null;
        }

        public static long? GetLong(string attributeValue)
        {
            if (long.TryParse(attributeValue, out long result))
            {
                return result;
            }

            return null;
        }

        internal static float? GetFloat(string attributeValue)
        {
            if (float.TryParse(attributeValue, out float result))
            {
                return result;
            }

            return null;
        }

        internal static bool? GetBooleanFromTrueFalse(string attributeValue)
        {
            if (bool.TryParse(attributeValue, out bool result))
            {
                return result;
            }

            return null;
        }

        internal static bool? GetBooleanFromYesNo(string attributeValue)
        {
            switch (attributeValue)
            {
                case "YES":
                    return true;
                case "NO":
                    return false;
                default:
                    return null;
            }
        }

        internal static OperatingMode? GetOperatingMode(string attributeValue)
        {
            switch (attributeValue)
            {
                case "MANUAL_SCAN":
                    return OperatingMode.ManualScan;
                case "MODIFIED":
                    return OperatingMode.Modified;
                default:
                    return null;
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
                default:
                    return null;
            }
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
                default:
                    return null;
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
                default:
                    return null;
            }
        }

        internal static RescanStatus? GetRescanStatus(string attributeValue)
        {
            switch (attributeValue)
            {
                case "RESCAN":
                    return RescanStatus.Rescan;
                case "NOT_RESCAN":
                    return RescanStatus.NotRescan;
                default:
                    return null;
            }
        }

        internal static EnvelopeDetect? GetEnvelopeDetect(string attributeValue)
        {
            return (EnvelopeDetect?) GetYesNoInactive(attributeValue);
        }

        internal static DeskewStatus? GetDeskewStatus(string attributeValue)
        {
            return (DeskewStatus?) GetYesNoInactive(attributeValue);
        }

        internal static FrontStreakDetectStatus? GetFrontStreakDetectStatus(string attributeValue)
        {
            return (FrontStreakDetectStatus?) GetYesNoInactive(attributeValue);
        }

        internal static BackStreakDetectStatus? GetBackStreakDetectStatus(string attributeValue)
        {
            return (BackStreakDetectStatus?) GetYesNoInactive(attributeValue);
        }

        internal static ScantimeFinalBlankAreaDecision? GetScantimeFinalBlankAreaDecision(string attributeValue)
        {
            switch (attributeValue)
            {
                case "BLANK":
                    return ScantimeFinalBlankAreaDecision.Blank;
                case "NOT_BLANK":
                    return ScantimeFinalBlankAreaDecision.NotBlank;
                case "UNDETERMINED":
                    return ScantimeFinalBlankAreaDecision.Undetermined;
                default:
                    return null;
            }
        }

        internal static AuditTrailType? GetAuditTrailType(string attributeValue)
        {
            switch (attributeValue)
            {
                case "ELECTRONIC":
                    return AuditTrailType.Electronic;
                case "PRINTED":
                    return AuditTrailType.Printed;
                default:
                    return null;
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
                default:
                    return null;
            }
        }

        internal static ImageType? GetImageType(string attributeValue)
        {
            switch (attributeValue)
            {
                case "FULL":
                    return ImageType.Full;
                case "SNIPPET":
                    return ImageType.Snippet;
                default:
                    return null;
            }
        }

        internal static ImageDepth? GetImageDepth(string attributeValue)
        {
            switch (attributeValue)
            {
                case "1":
                    return ImageDepth.Bitonal;
                case "8":
                    return ImageDepth.Grayscale;
                case "24":
                    return ImageDepth.Color;
                default:
                    return null;
            }
        }

        internal static ImageFormat? GetImageFormat(string attributeValue)
        {
            switch (attributeValue)
            {
                case "JPEG":
                    return ImageFormat.JPEG;
                case "RAW":
                    return ImageFormat.RAW;
                case "TIFF":
                    return ImageFormat.TIFF;
                default:
                    return null;
            }
        }

        internal static ImageResolution? GetImageResolution(string attributeValue)
        {
            switch (attributeValue)
            {
                case "100":
                    return ImageResolution.Low;
                case "150":
                    return ImageResolution.MediumLow;
                case "200":
                    return ImageResolution.Medium;
                case "240":
                    return ImageResolution.MediumHigh;
                case "300":
                    return ImageResolution.High;
                default:
                    return null;
            }
        }

        internal static MicrStatus? GetMicrStatus(string attributeValue)
        {
            switch (attributeValue)
            {
                case "GOOD":
                    return MicrStatus.Good;
                case "PARTIAL":
                    return MicrStatus.Partial;
                case "BAD":
                    return MicrStatus.Bad;
                case "NO_MICR":
                    return MicrStatus.NoMicr;
                case "INACTIVE":
                    return MicrStatus.Inactive;
                case "ERROR":
                    return MicrStatus.Error;
                default:
                    return null;
            }
        }

        internal static RtStatus? GetRtStatus(string attributeValue)
        {
            switch (attributeValue)
            {
                case "GOOD":
                    return RtStatus.Good;
                case "BAD":
                    return RtStatus.Bad;
                case "NOT_FOUND":
                    return RtStatus.NotFound;
                default:
                    return null;
            }
        }

        internal static CheckType? GetCheckType(string attributeValue)
        {
            switch (attributeValue)
            {
                case "E13B":
                    return CheckType.E13B;
                case "US":
                    return CheckType.US;
                case "CANADA":
                    return CheckType.Canada;
                case "CMC7":
                    return CheckType.CMC7;
                case "UNKNOWN":
                    return CheckType.Unknown;
                default:
                    return null;
            }
        }

        private static int? GetYesNoInactive(string attributeValue)
        {
            switch (attributeValue)
            {
                case "YES":
                    return 0;
                case "NO":
                    return 1;
                case "INACTIVE":
                    return 2;
                default:
                    return null;
            }
        }
    }
}