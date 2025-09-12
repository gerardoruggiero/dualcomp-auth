namespace DualComp.Infraestructure.Domain.Domain.Common.Errors
{
	public sealed class DomainError
	{
		public string Code { get; }
		public string Message { get; }

		public DomainError(string code, string message)
		{
			Code = code;
			Message = message;
		}
	}
}


