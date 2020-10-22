using System;

namespace astorWorkDAO
{
    public class AttachmentMaster
    {
        public int ID { get; set; }

        public string URL { get; set; }
        public string FileName { get; set; }

        public int FileSize { get; set; }

        public UserMaster CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public UserMaster UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }

        /*
         * Type
         * 0 - Project files
         * 1 - MRF Shop drawing
         * 2 - Material drawing
         */
        public int Type { get; set; }

        public string Reference { get; set; } // e.g. project id, MRF id...
        public string Remarks { get; set; } // revision no., anything
    }
}
