using DwitTech.Authentication;
using System.Text.Json.Serialization;

namespace DwitTech.AccountService.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
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

            var app = builder.Build();

            IConfiguration configuration = app.Configuration;
            IWebHostEnvironment environment = app.Environment;

            // Configure the HTTP request pipeline.
            app.MapHealthChecks("/health");

            app.UseSwagger();
            app.UseSwaggerUI();


            app.UseHttpsRedirection();

            app.UseRouting();

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