namespace database2.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEY = "Secret";
        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException("Not found next Delegate");
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if(context == null)
            {
                throw new ArgumentNullException("not found context");
            }
            if (!context.Request.Headers.TryGetValue(APIKEY,out var apikey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key was not provied!");
                return;
            }
            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>(APIKEY);
            if (!apiKey.Equals(apikey)) {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }
            await _next(context);
        }
    }
}
