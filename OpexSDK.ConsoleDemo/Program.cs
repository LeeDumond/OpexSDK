using System;
using OpexSDK.Enumerations;
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
            Console.WriteLine($"\tJobName: {batch.JobName}");

            
        }
    }
}
