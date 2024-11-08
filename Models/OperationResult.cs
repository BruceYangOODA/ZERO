using ZERO.Models.Enum;
namespace ZERO.Models
{
    public class OperationResult
    {
        public RequestResultCode RequestResultCode { get; set; }
        public string? ErrorMessage { get; set; }
        public OperationResult returnResult(RequestResultCode _RequestResultCode)
        {
            RequestResultCode = _RequestResultCode;
            return this;
        }
        public OperationResult returnError(RequestResultCode _RequestResultCode, string _ErrorMessage)
        {
            RequestResultCode = _RequestResultCode;
            ErrorMessage = _ErrorMessage;
            return this;
        }
    }
    public class OperationResult<T> : OperationResult
    {
        public T? Result { get; set; }

        public OperationResult<T> returnResult(T _Result)
        {
            RequestResultCode = RequestResultCode.Success;
            Result = _Result;
            return this;
        }
        public new OperationResult<T> returnError(RequestResultCode _RequestResultCode, string _ErrorMessage)
        {
            RequestResultCode = _RequestResultCode;
            ErrorMessage = _ErrorMessage;
            return this;
        }
    }
}
