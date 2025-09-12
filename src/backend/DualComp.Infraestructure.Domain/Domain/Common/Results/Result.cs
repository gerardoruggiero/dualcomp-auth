using DualComp.Infraestructure.Domain.Domain.Common.Errors;

namespace DualComp.Infraestructure.Domain.Domain.Common.Results
{
	public class Result
	{
		public bool IsSuccess { get; }
		public DomainError? Error { get; }

		protected Result(bool isSuccess, DomainError? error)
		{
			IsSuccess = isSuccess;
			Error = error;
		}

		public static Result Success() => new(true, null);
		public static Result Failure(DomainError error) => new(false, error);
	}

	public class Result<T> : Result
	{
		public T? Value { get; }

        private Result(bool isSuccess, T? value, DomainError? error)
            : base(isSuccess, error) => Value = value;

        public static Result<T> Success(T value) => new(true, value, null);
		public static new Result<T> Failure(DomainError error) => new(false, default, error);
	}
}


