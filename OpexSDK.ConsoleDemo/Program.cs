using System;
using System.Linq;
using OpexSDK.Models;

namespace OpexSDK.ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            BatchReader reader = new BatchReader("sample.oxi", "oxi1_60.xsd");

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
                Console.WriteLine($"\tTransaction:");
                Console.WriteLine($"\t\tTransaction ID: {transaction.TransactionId}");

                foreach (Group group in transaction.Groups)
                {
                    Console.WriteLine();
                    Console.WriteLine($"\t\tGroup:");
                    Console.WriteLine($"\t\t\tGroup ID: {group.GroupId}");

                    foreach (Page page in group.Pages)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"\t\t\tPage:");
                        Console.WriteLine($"\t\t\t\tDocument Locator: {page.DocumentLocator}");
                        Console.WriteLine($"\t\t\t\tBatch Sequence: {page.BatchSequence}");
                        Console.WriteLine($"\t\t\t\tTransaction Sequence: {page.TransactionSequence}");
                    }
                }
            }

            if (reader.ValidationErrors.Any())
            {
                int errorCount = reader.ValidationErrors.Count;
                Console.WriteLine();
                Console.WriteLine($"{errorCount} validation {(errorCount == 1 ? "error" : "errors")} found:");

                int counter = 1;
                foreach (var error in reader.ValidationErrors)
                {
                    Console.WriteLine($"\t{counter}. {error.Message}");
                    counter++;
                }
            }

            
        }
    }
}
