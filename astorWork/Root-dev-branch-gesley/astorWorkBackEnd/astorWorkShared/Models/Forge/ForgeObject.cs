using Microsoft.AspNetCore.Http;

namespace astorWorkShared.Models
{
    public class ForgeObject
    {
        public string BucketKey { get; set; }
        public string ObjectKey { get; set; }
        public string ObjectId { get; set; }
        public string Sha1 { get; set; }
        public int Size { get; set; }
        public string Location { get; set; }

        public IFormFile ModelFile { get; set; }
    }
}
