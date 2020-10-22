using Newtonsoft.Json;
using System;

namespace astorWorkShared.GlobalResponse
{
    public class APIResponse
    {
        public int Status { get; set; }
        public Object Data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        public APIResponse(int statusCode, Object data, string message = null)
        {
            Status = statusCode;
            Data = data;
            Message = message;
        }

        public APIResponse() {
        }
    }

    public class APIBadRequest : APIResponse {
        public APIBadRequest() {
            Status = ErrorMessages.BadRequest;
            Message = ErrorMessages.BadRequestMsg();
        }
    }

    public class DbRecordNotFound : APIResponse
    {
        public DbRecordNotFound(string message)
        {
            Status = ErrorMessages.DbRecordNotFound;
            Message = message;
        }
    }

    public class DbConcurrentUpdate : APIResponse
    {
        public DbConcurrentUpdate(string message)
        {
            Status = ErrorMessages.DbConcurrentUpdate;
            Message = message;
        }
    }

    public class DbDuplicateRecordMsg : APIResponse
    {
        public DbDuplicateRecordMsg(string module, string field, string value)
        {
            Status = ErrorMessages.DbDuplicateRecord;
            Message = (ErrorMessages.DbDuplicateRecordMsg(module, field, value));
        }
    }

    public class DbValidationError : APIResponse
    {
        public DbValidationError(string message)
        {
            Status = ErrorMessages.DbValidationError;
            Message = message;
        }
    }

    public class ExternalServiceFail : APIResponse
    {
        public ExternalServiceFail(string externalService)
        {
            Status = ErrorMessages.ExternalServiceFail;
            Message = externalService + " failed.";
        } 
    }

    public class RequestNotAllowedWithinInterval : APIResponse
    {
        public RequestNotAllowedWithinInterval(string interval)
        {
            Status = ErrorMessages.RequestNotAllowedWithinInterval;
            Message = "Please retry after the service interval of " + interval;
        }
    }
}



