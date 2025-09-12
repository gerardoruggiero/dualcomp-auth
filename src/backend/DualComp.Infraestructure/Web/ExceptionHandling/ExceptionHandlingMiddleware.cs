using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace DualComp.Infraestructure.Web.ExceptionHandling
{
	public sealed class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				context.Response.ContentType = "application/json";
				var payload = new { error = "UnhandledException", message = ex.Message };
				await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
			}
		}
	}
}


