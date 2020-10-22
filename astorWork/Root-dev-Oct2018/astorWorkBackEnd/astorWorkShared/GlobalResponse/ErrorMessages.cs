using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkShared.GlobalResponse
{
    public static class ErrorMessages
    {
        public const int BadRequest = 400;
        public const int DbRecordNotFound = 404;
        public const int DbDuplicateRecord = 405;
        public const int DbValidationError = 406;
        public const int DbUpdateConcurrencyException = 1001;
        public const int RequestNotAllowedWithinInterval = 1002;
        public const int FileInvalid = 1003;
        public const int ExternalServiceFail = 1004;
        public const int PasswordNotMatch = 401;
        public const int UnkownError = 500;


        public static string GetDefaultMessageForStatusCode(int statusCode)
        {
            switch (statusCode) { 
                case 500:
                    return "An unhandled error occurred";
                default:
                    return null;
            }
        }

        public static string DBRecordNotFoundMsg(string module, string field = null, string id = null) {
            return $"{module} with {field} {id} not found";
        }

        public static string DbDuplicateRecordMsg(string module, string field = null, string id = null)
        {
            return $"{module} with {field} : {id} already exists";
        }

        public static string BadRequestMsg() {
            return "Bad Request";
        }
    }
}
