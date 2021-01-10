using GuardNet;
using JetBrains.Annotations;

namespace AAS.Architecture.Models
{
    public class OperationResult
    {

        protected OperationResult([CanBeNull] string errorMessage, [CanBeNull] string errorCode,  bool succeded)
        {
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            Succeded = succeded;
        }

        [CanBeNull]
        public string ErrorMessage { get;  }

        [CanBeNull]
        public string ErrorCode { get; }

        public bool Succeded { get;  }
        
        public static OperationResult Success()
        {
            return new OperationResult(null, null, true);
        }

        public static OperationResult Error([NotNull] string error, [NotNull] string errorCode = "unknown")
        {
            Guard.NotNullOrWhitespace(error, nameof(error));
            return new OperationResult(error, errorCode, false);
        }
    }
    
    public class OperationResult<T>: OperationResult
    {
        protected OperationResult([CanBeNull] string error, [CanBeNull] string errorCode, bool succeded, T id) : base(error, errorCode, succeded)
        {
            OperationResultId = id;
        }

        public T OperationResultId { get; }
        public static OperationResult<T> Success(T id) => new OperationResult<T>(null, null, true, id);

        public new static OperationResult<T> Error([NotNull] string error, [NotNull] string errorCode = "unknown")
        {
            Guard.NotNullOrWhitespace(error, nameof(error));
            return new OperationResult<T>(error,  errorCode, false, default(T));
        }
    }
}