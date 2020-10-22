namespace astorWorkBackEnd.Models
{
    public class MaterialUpdate
    {
        public string MRFNo { get; set; }
        public bool QCStatus { get; set; }
        public string QCRemarks { get; set; }
        public int TrackerID { get; set; }
        public int LocationID { get; set; }
    }
}
