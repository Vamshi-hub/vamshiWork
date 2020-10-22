using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class MaterialDrawingAudit
    {
        public int ID { get; set; }
        public string DrawingNo { get; set; }
        public int RevisionNo { get; set; }
        public DateTimeOffset DrawingIssueDate { get; set; }
    }
}
