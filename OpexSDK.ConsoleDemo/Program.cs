using System;
using System.Linq;
using System.Xml.Schema;
using OpexSDK.Models;

namespace OpexSDK.ConsoleDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var reader = new BatchReader("sample.oxi", "oxi1_60.xsd");

            Batch batch = reader.ReadBatch();

            Console.WriteLine("Batch:");
            Console.WriteLine($"\tFormat Version: {batch.FormatVersion}");
            Console.WriteLine($"\tBase Machine: {batch.BaseMachine}");
            Console.WriteLine($"\tScan Device: {batch.ScanDevice}");
            Console.WriteLine($"\tSoftware Version: {batch.SoftwareVersion}");
            Console.WriteLine($"\tTransport ID: {batch.TransportId}");
            Console.WriteLine($"\tBatch Identifier: {batch.BatchIdentifier}");
            Console.WriteLine($"\tJob Type: {batch.JobType}");
            Console.WriteLine($"\tOperating Mode: {batch.OperatingMode}");
            Console.WriteLine($"\tJob Name: {batch.JobName}");
            Console.WriteLine($"\tOperator Name: {batch.OperatorName}");
            Console.WriteLine($"\tStart Time: {batch.StartTime}");
            Console.WriteLine($"\tReceive Date: {batch.ReceiveDate?.ToShortDateString()}");
            Console.WriteLine($"\tProcess Date: {batch.ProcessDate?.ToShortDateString()}");
            Console.WriteLine($"\tImage File Path: {batch.ImageFilePath}");
            Console.WriteLine($"\tPlugin Message: {batch.PluginMessage}");
            Console.WriteLine($"\tDeveloper Reserved: {batch.DeveloperReserved}");

            foreach (Transaction transaction in batch.Transactions)
            {
                Console.WriteLine();
                Console.WriteLine("\tTransaction:");
                Console.WriteLine($"\t\tTransaction ID: {transaction.TransactionId}");

                foreach (Group group in transaction.Groups)
                {
                    Console.WriteLine();
                    Console.WriteLine("\t\tGroup:");
                    Console.WriteLine($"\t\t\tGroup ID: {group.GroupId}");

                    foreach (Page page in group.Pages)
                    {
                        Console.WriteLine();
                        Console.WriteLine("\t\t\tPage:");
                        Console.WriteLine($"\t\t\t\tDocument Locator: {page.DocumentLocator}");
                        Console.WriteLine($"\t\t\t\tBatch Sequence: {page.BatchSequence}");
                        Console.WriteLine($"\t\t\t\tTransaction Sequence: {page.TransactionSequence}");

                        foreach (Image image in page.Images)
                        {
                            Console.WriteLine();
                            Console.WriteLine("\t\t\t\tImage:");
                            Console.WriteLine($"\t\t\t\t\tIndex: {image.Index}");
                            Console.WriteLine($"\t\t\t\t\tRescan Status: {image.RescanStatus}");
                            Console.WriteLine(
                                $"\t\t\t\t\tScantime Final Blank Area Decision: {image.ScantimeFinalBlankAreaDecision}");
                            Console.WriteLine($"\t\t\t\t\tSide: {image.Side}");
                            Console.WriteLine($"\t\t\t\t\tType: {image.Type}");
                            Console.WriteLine($"\t\t\t\t\tDepth: {image.Depth}");
                            Console.WriteLine($"\t\t\t\t\tFormat: {image.Format}");
                            Console.WriteLine($"\t\t\t\t\tFile Name: {image.Filename}");
                            Console.WriteLine($"\t\t\t\t\tFile Size: {image.Filesize}");
                            Console.WriteLine($"\t\t\t\t\tLength: {image.Length}");
                            Console.WriteLine($"\t\t\t\t\tHeight: {image.Height}");
                            Console.WriteLine($"\t\t\t\t\tOffsetLength: {image.OffsetLength}");
                            Console.WriteLine($"\t\t\t\t\tOffsetHeight: {image.OffsetHeight}");
                            Console.WriteLine($"\t\t\t\t\tResolutionLength: {image.ResolutionLength}");
                            Console.WriteLine($"\t\t\t\t\tResolutionHeight: {image.ResolutionHeight}");
                        }

                        foreach (CustomData customData in page.CustomDatas)
                        {
                            Console.WriteLine();
                            Console.WriteLine("\t\t\t\tCustomData:");
                            Console.WriteLine($"\t\t\t\t\tEntry: {customData.Entry}");
                        }

                        foreach (Micr micr in page.Micrs)
                        {
                            Console.WriteLine();
                            Console.WriteLine("\t\t\t\tMicr:");
                            Console.WriteLine($"\t\t\t\t\tStatus: {micr.Status}");
                            Console.WriteLine($"\t\t\t\t\tRtStatus: {micr.RtStatus}");
                            Console.WriteLine($"\t\t\t\t\tCheckType: {micr.CheckType}");
                            Console.WriteLine($"\t\t\t\t\tSide: {micr.Side}");
                            Console.WriteLine($"\t\t\t\t\tValue: {micr.Value}");
                        }

                        foreach (Ocr ocr in page.Ocrs)
                        {
                            Console.WriteLine();
                            Console.WriteLine("\t\t\t\tOcr:");
                            Console.WriteLine($"\t\t\t\t\tIndex: {ocr.Index}");
                            Console.WriteLine($"\t\t\t\t\tSide: {ocr.Side}");
                            Console.WriteLine($"\t\t\t\t\tValue: {ocr.Value}");
                            Console.WriteLine($"\t\t\t\t\tName: {ocr.Name}");
                        }

                        foreach (Barcode barcode in page.Barcodes)
                        {
                            Console.WriteLine();
                            Console.WriteLine("\t\t\t\tBarcode:");
                            Console.WriteLine($"\t\t\t\t\tIndex: {barcode.Index}");
                            Console.WriteLine($"\t\t\t\t\tType: {barcode.Type}");
                            Console.WriteLine($"\t\t\t\t\tSide: {barcode.Side}");
                            Console.WriteLine($"\t\t\t\t\tValue: {barcode.Value}");
                        }

                        foreach (MarkDetect markDetect in page.MarkDetects)
                        {
                            Console.WriteLine();
                            Console.WriteLine("\t\t\t\tMarkDetect:");
                            Console.WriteLine($"\t\t\t\t\tIndex: {markDetect.Index}");
                            Console.WriteLine($"\t\t\t\t\tSide: {markDetect.Side}");
                            Console.WriteLine($"\t\t\t\t\tResult: {markDetect.Result}");
                            Console.WriteLine($"\t\t\t\t\tName: {markDetect.Name}");
                        }

                        foreach (AuditTrail auditTrail in page.AuditTrails)
                        {
                            Console.WriteLine();
                            Console.WriteLine("\t\t\t\tAuditTrail:");
                            Console.WriteLine($"\t\t\t\t\tType: {auditTrail.Type}");
                            Console.WriteLine($"\t\t\t\t\tSide: {auditTrail.Side}");
                            Console.WriteLine($"\t\t\t\t\tText: {auditTrail.Text}");
                            Console.WriteLine($"\t\t\t\t\tApply: {auditTrail.Apply}");
                        }

                        foreach (ReferenceId referenceId in page.ReferenceIds)
                        {
                            Console.WriteLine();
                            Console.WriteLine("\t\t\t\tReference ID:");
                            Console.WriteLine($"\t\t\t\t\tIndex: {referenceId.Index}");
                            Console.WriteLine($"\t\t\t\t\tResponse: {referenceId.Response}");
                            Console.WriteLine($"\t\t\t\t\tName: {referenceId.Name}");
                        }

                        foreach (Tag tag in page.Tags)
                        {
                            Console.WriteLine();
                            Console.WriteLine("\t\t\t\tTag:");
                            Console.WriteLine($"\t\t\t\t\tSource: {tag.Source}");
                            Console.WriteLine($"\t\t\t\t\tValue: {tag.Value}");
                        }
                    }
                }
            }

            if (reader.ValidationErrors.Any())
            {
                int errorCount = reader.ValidationErrors.Count;
                Console.WriteLine();
                Console.WriteLine($"{errorCount} validation {(errorCount == 1 ? "error" : "errors")} found:");

                var counter = 1;
                foreach (ValidationEventArgs error in reader.ValidationErrors)
                {
                    Console.WriteLine($"\t{counter}. {error.Message}");
                    counter++;
                }
            }
        }
    }
}