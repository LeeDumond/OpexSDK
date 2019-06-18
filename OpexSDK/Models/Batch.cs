﻿using System;
using System.Collections.Generic;
using OpexSDK.Enumerations;

namespace OpexSDK.Models
{
    /// <summary>
    /// Contains data about a batch of documents scanned by an OPEX scanning device.
    /// </summary>
    public class Batch
    {
        internal Batch()
        {
            ReferenceIds = new List<ReferenceId>();
            Transactions = new List<Transaction>();
        }

        /// <summary>
        /// Version number of Batch Information File structure.
        /// </summary>
        public string FormatVersion { get; set; }

        /// <summary>
        /// Identifier of base OPEX machine connected to the scanning device.
        /// </summary>
        public string BaseMachine { get; set; }

        /// <summary>
        /// Text string identifier of OPEX Scanning Device (OSD).
        /// </summary>
        public string ScanDevice { get; set; }

        /// <summary>
        /// Software version operating the OPEX Scanning Device.
        /// </summary>
        public string SoftwareVersion { get; set; }

        /// <summary>
        /// Unique transport (scanner) identifier. Identifies batch to specific OPEX transport.
        /// </summary>
        public string TransportId { get; set; }

        /// <summary>
        /// The batch id read from batch tickets, assigned by the OPEX Scanning Device, or provided via an OpenIEM plug-in.
        /// </summary>
        public string BatchIdentifier { get; set; }

        /// <summary>
        /// Type of transactions in job.
        /// </summary>
        public JobType? JobType { get; set; }

        /// <summary>
        /// Operating mode of OPEX Scanning Device.
        /// </summary>
        public OperatingMode? OperatingMode { get; set; }

        /// <summary>
        /// Name of selected job, assigned by OPEX Scanning Device. Operator does not type in this job name, but rather selects it from a list of valid jobs.
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// Name of the operator logged into OPEX Scanning Device when the batch was initially created. Any time an OPEX employee runs the OSD, this field is set to "OPEX Technician" or "OPEX Engineer".
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// Batch creation date and time, i.e., when batch first started. Uses date set in OPEX Scanning Device PC.
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Batch receive date. A user selectable date indicating when the batch was received. Often the same date as in StartTime.
        /// </summary>
        public DateTime? ReceiveDate { get; set; }

        /// <summary>
        /// Batch process date. A user selectable date indicating when the batch was processed. Often the same date as in StartTime.
        /// </summary>
        public DateTime? ProcessDate { get; set; }

        /// <summary>
        /// Path to image files, which may or may not be the same as the location of the Batch Information File. When this property is null, it indicates that the image files are in the same folder as the Batch Information File.
        /// </summary>
        public string ImageFilePath { get; set; }

        /// <summary>
        /// Message generated by an optional software plug-in, such as Open IEM.
        /// </summary>
        public string PluginMessage { get; set; }

        /// <summary>
        /// Message defined by developer, set as static parameter on OSD.
        /// </summary>
        public string DeveloperReserved { get; set; }

        /// <summary>
        /// One for each ReferenceID configured in the job.
        /// </summary>
        public IList<ReferenceId> ReferenceIds { get; }

        /// <summary>
        /// One for each Transaction in the batch.
        /// </summary>
        public ICollection<Transaction> Transactions { get;  }

        /// <summary>
        /// Contains information about the batch that is determined once the batch is closed.
        /// </summary>
        public EndInfo EndInfo { get; internal set; }
    }
}
