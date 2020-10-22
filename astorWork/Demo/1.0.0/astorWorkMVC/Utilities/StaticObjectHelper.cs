using astorWorkMVC.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace astorWorkMVC.Utilities
{
    public class StaticObjectHelper
    {
        public static ConcurrentBag<EchoMessage> ListEchoMessages = new ConcurrentBag<EchoMessage>();
    }
}