﻿using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DwitTech.AccountService.Core.Events;
using Confluent.Kafka;

namespace DwitTech.AccountService.Core.Extension

{
    public static class ServiceCollectionsExtension
    {
        public static IServiceCollection AddDatabaseService(this IServiceCollection service, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("AccountDbContext");
            connectionString = connectionString.Replace("{DBHost}", configuration["DB_HOSTNAME"]);
            connectionString = connectionString.Replace("{DBPort}", configuration["DB_PORT"]);
            connectionString = connectionString.Replace("{DBName}", configuration["DB_NAME"]);
            connectionString = connectionString.Replace("{DBUser}", configuration["DB_USERNAME"]);
            connectionString = connectionString.Replace("{DBPassword}", configuration["DB_PASSWORD"]);



            service.AddDbContext<AccountDbContext>(opt =>
            {
                opt.UseNpgsql(connectionString, c => c.CommandTimeout(120));
#if DEBUG
                opt.EnableSensitiveDataLogging();
                opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
#endif
            },
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Scoped);


            return service;
        }

        public static IServiceCollection AddServices(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            service.AddScoped<IAuthenticationService, AuthenticationService>();
            service.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            service.AddScoped<IActivationService, ActivationService>();
            service.AddScoped<IUserRepository, UserRepository>();
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<IRoleRepository, RoleRepository>();
            service.AddScoped<IEmailService, EmailService>();
            service.AddSingleton<IEventPublisher, EventPublisher>();
            service.AddHttpClient();
            service.AddSingleton<IProducer<string, string>>(provider =>
            {
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = configuration["MESSAGE_BROKER_BOOTSTRAP_SERVERS"],
                    ClientId = configuration["MESSAGE_BROKER_CLIENT_ID"],
                   
                    SecurityProtocol = Enum.Parse<SecurityProtocol>(configuration["KAFKA_SECURITY_PROTOCOL"])
                };

                switch (producerConfig.SecurityProtocol)
                {
                    case null:
                    case SecurityProtocol.Plaintext:
                        break;
                    case SecurityProtocol.Ssl:
                        break;

                    case SecurityProtocol.SaslSsl:
                        producerConfig.SaslMechanism = Enum.Parse<SaslMechanism>(configuration["KAFKA_SASL_MECHANISM"]);
                        producerConfig.SaslUsername = configuration["KAFKA_SASL_USERNAME"];
                        producerConfig.SaslPassword = configuration["KAFKA_SASL_PASSWORD"];
                        break;

                    case SecurityProtocol.SaslPlaintext:
                        throw new NotImplementedException($"Security Protocol {producerConfig.SecurityProtocol} is not implemented");

                }

                return new ProducerBuilder<string, string>(producerConfig).Build();
            });

            return service;
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidIssuer = configuration["JWT_ISSUER"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT_KEY"]))
                };
            });
        }
    }
}
