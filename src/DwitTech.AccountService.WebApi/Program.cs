using NLog;
using NLog.Web;
using System.Text.Json.Serialization;
using DwitTech.AccountService.Core.Extension;
using DwitTech.AccountService.Core.Middleware;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;
using DwitTech.AccountService.Core.EventsHandler;
using Confluent.Kafka;

namespace DwitTech.AccountService.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    // To preserve the default behavior, capture the original delegate to call later.
                    var builtInFactory = options.InvalidModelStateResponseFactory;

                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                                            .GetRequiredService<ILogger<Program>>();
                        return builtInFactory(context);
                    };
                })
                .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddDatabaseService(builder.Configuration);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGenNewtonsoftSupport();
            builder.Services.AddHealthChecks();
            builder.Services.AddServices(builder.Configuration);

            // Add service and create Policy with options
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            builder.Services.ConfigureAuthentication(builder.Configuration);

            builder.Services.AddAuthorization();
            builder.Services.AddMvc();
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            builder.Services.AddSingleton<ProducerConfig>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();

                var bootstrapServers = configuration["KAFKA_BOOTSTRAP_SERVERS"];
                var clientId = configuration["KAFKA_CLIENT_ID"];

                return new ProducerConfig
                {
                    BootstrapServers = bootstrapServers,
                    ClientId = clientId
                    // Add any additional configuration properties as needed
                };
            });

            var app = builder.Build();

            IConfiguration configuration = app.Configuration;
            IWebHostEnvironment environment = app.Environment;

            // Configure the HTTP request pipeline.
            app.MapHealthChecks("/health");

            if (environment.IsEnvironment("Development") || environment.IsEnvironment("Local"))
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseForwardedHeaders();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<AuthorizationMiddleware>();

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            app.SetupMigrations(app.Services, app.Configuration);

            app.Run();
        }
    }
}