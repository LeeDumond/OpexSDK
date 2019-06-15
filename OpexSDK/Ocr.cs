﻿using OpexSDK.Enumerations;

namespace OpexSDK
{
    /// <summary>
    /// Contains information about a single OCR read.
    /// </summary>
    public class Ocr
    {
        /// <summary>
        /// The index of the OCR read, as configured in the job. Values are 1-indexed. (i.e., "1" is the first index.)
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Side on which the OCR read was performed.
        /// </summary>
        public Side Side { get; set; }

        /// <summary>
        /// OCR read result. Data can include alpha-numeric digits and special characters. An '!' is used for rejected / unknown characters.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Reiterates the index of the OCR read, i.e. if Index is x, this value will be 'OCR x'.
        /// </summary>
        public string Name { get; set; }
    }
}