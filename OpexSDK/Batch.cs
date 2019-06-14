using System;
using System.Collections.Generic;
using OpexSDK.Enumerations;

namespace OpexSDK
{
    public class Batch
    {
        public string FormatVersion { get; set; }
        public BaseMachine BaseMachine { get; set; }
        public string ScanDevice { get; set; }
        public string SoftwareVersion { get; set; }
        public string TransportId { get; set; }
        public string BatchIdentifier { get; set; }
        public JobType JobType { get; set; }
        public OperatingMode OperatingMode { get; set; }
        public string JobName { get; set; }
        public string OperatorName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ReceiveDate { get; set; }
        public DateTime ProcessDate { get; set; }
        public string ImageFilePath { get; set; }
        public string PluginMessage { get; set; }
        public string DeveloperReserved { get; set; }

        public ICollection<ReferenceId> ReferenceIds { get; }
        public ICollection<Transaction> Transactions { get; }

        public EndInfo EndInfo { get; }
    }
}
