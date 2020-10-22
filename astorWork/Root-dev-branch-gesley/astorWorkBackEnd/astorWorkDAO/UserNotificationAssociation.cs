namespace astorWorkDAO
{
    public class UserNotificationAssociation
    {
        public int ID { get; set; }

        public int UserID { get; set; }
        public UserMaster Receipient { get; set; }

        public int NotificationID { get; set; }
        public NotificationAudit Notification { get; set; }
    }
}
