namespace DualComp.Infraestructure.Logging
{
	public interface ILoggerAdapter<T>
	{
		void LogInformation(string message, params object[] args);
		void LogWarning(string message, params object[] args);
		void LogError(Exception exception, string message, params object[] args);
	}
}


