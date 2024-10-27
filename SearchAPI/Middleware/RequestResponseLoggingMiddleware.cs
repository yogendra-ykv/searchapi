namespace SearchAPI.Middleware
{
    using System.Text;

    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                Console.WriteLine($"Request Body: {requestBody}");
            }

            await _next(context);
        }
    }

}
