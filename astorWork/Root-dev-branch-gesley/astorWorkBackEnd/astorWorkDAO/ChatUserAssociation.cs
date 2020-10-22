using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class ChatUserAssociation
    {
        public int ID { get; set; }
        public ChatData Chat { get; set; }
        public int? ChatID { get; set; }
        public UserMaster UserMaster { get; set; }
        public int? UserMasterID { get; set; }
        public DateTime SeenDateTime { get; set; }
    }
}
