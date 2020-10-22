using System;

namespace astorWorkMaterialTracking.Models
{
    public class MaterialUpdate
    {
        public int StageID { get; set; }
        public int LocationID { get; set; }
        public DateTimeOffset? CastingDate { get; set; }
    }

    public class MaterialBatchUpdate
    {
        public int[] MaterialIDs { get; set; }
        public int StageID { get; set; }
        public int LocationID { get; set; }
        public DateTimeOffset? CastingDate { get; set; }
    }
}
