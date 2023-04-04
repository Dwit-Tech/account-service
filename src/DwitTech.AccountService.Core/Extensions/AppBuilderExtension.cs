using DwitTech.AccountService.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class AppBuilderExtension
    {
        public static IApplicationBuilder SetupMigrations(this IApplicationBuilder app, IServiceProvider service, IConfiguration configuration)
        {
            var logger = service.GetService<ILogger<AccountDbContext>>();

            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<AccountDbContext>();
                    //context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return app;
        }
    }
}
