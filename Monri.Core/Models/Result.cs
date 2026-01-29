namespace Monri.Core.Models
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None)
            {
                throw new InvalidOperationException();
            }

            if (!isSuccess && error == Error.None)
            {
                throw new InvalidOperationException();
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, Error.None);

        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

        public static Result Failure(Error error) => new(false, error);

        public static Result<TValue> Failure<TValue>(Error error) => new(default(TValue), false, error);
    }

    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        internal Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
        {
            _value = value;
        }

        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of the failure result can not be accessed");

        public static implicit operator Result<TValue>(TValue value) => Success(value);
    }

    public sealed record Error(object Code, string Message)
    {
        public static Error Create(object code, string message)
        {
            return new Error(code, message);
        }

        public static Error FromException(Exception ex)
        {
            return new Error(123,ex.Message);
        }

        public static readonly Error None = new(string.Empty, string.Empty);
        public static readonly Error Fallback = new("F00", "Something went wrong with processing the request");
        public static readonly Error Exception = new("E00", "Something went wrong");
        public static readonly Error BadRequest = new("400", "Bad request");
        public static readonly Error Unauthorized = new("401", "Unauthorized");
        public static readonly Error Forbidden = new("403", "Forbidden");
        public static readonly Error ResourceNotFound = new("404", "Resource not found");
        public static readonly Error Conflict = new("409", "Resource already exists");
        public static readonly Error InternalServerError = new("500", "Internal server error");

        public static readonly Error MaxEntriesReached = new("R00", "A maximum of 1 entry per minute is allowed");
        public static readonly Error UnableToSave = new("D00", "The data was unable to be saved");
    }
}
