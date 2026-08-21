namespace HRMS.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseApplicationMiddlewares(this IApplicationBuilder app)
        {


            //app.UseMiddleware<ExceptionMiddleware>();
            //app.UseMiddleware<RequestResponseLoggingMiddleware>();
            //app.UseAuthentication();
            ////app.UseMiddleware<ClientMiddleware>();
            ////app.UseMiddleware<SubscriptionMiddleware>();
            //app.UseMiddleware<CompanyProfileMiddleware>();

            app.UseAuthorization();

            return app;
        }
    }
}
