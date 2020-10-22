namespace astorWorkDAO
{
    public class UserMRFAssociation
    {
        public int UserID { get; set; }
        public UserMaster User { get; set; }
        public int MRFID { get; set; }
        public MRFMaster MRF { get; set; }
    }
}
