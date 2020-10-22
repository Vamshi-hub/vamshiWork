using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkShared.Utilities
{
    public static class ExceptionUtility
    {
        public static string GetExceptionDetails(Exception exc)
        {
            var message = $"Exception happended: {exc.Message}";
            if (exc.InnerException != null)
            {
                message += Environment.NewLine;
                message += exc.InnerException.Message;
                message += Environment.NewLine;
                message += exc.InnerException.StackTrace;
            }
            else
            {
                message += Environment.NewLine;
                message += exc.StackTrace;
            }

            return message;
        }
    }
}
