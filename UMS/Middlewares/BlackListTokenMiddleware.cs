using UMS.Services;

namespace UMS.Middlewares;
public class BlackListTokenMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, BlackListTokenService blacklist)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Replace("Bearer ", "");
            if (await blacklist.IsBlackListToken(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Your Session has expired!. Please login again.");
                return;
            }
        }
        await next(context);
    }
}