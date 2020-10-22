using astorWorkDAO;
using System.Collections.Generic;

namespace astorWorkBackEnd.Models
{
    public class AutodeskForge
    {
        public class BucketItem
        {
            public string bucketKey { get; set; }
            public long createdDate { get; set; }
            public string policyKey { get; set; }

            public List<BIMForgeModel> ObjectItems { get; set; }
        }

        public class ObjectItem
        {
            public string bucketKey { get; set; }
            public string objectKey { get; set; }
            public string objectId { get; set; }
            public string sha1 { get; set; }
            public int size { get; set; }
            public string location { get; set; }
        }

    }
}
