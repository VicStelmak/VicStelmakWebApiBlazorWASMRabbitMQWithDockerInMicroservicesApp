﻿using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VicStelmak.Sma.UserMicroservice.ApiDataLibrary.Application.Interfaces;
using VicStelmak.Sma.UserMicroservice.ApiDataLibrary.Application.Services;
using VicStelmak.Sma.UserMicroservice.ApiDataLibrary.Domain.Models;
using VicStelmak.Sma.UserMicroservice.ApiDataLibrary.Infrastructure.DataAccess;

namespace VicStelmak.Sma.UserMicroservice.ApiDataLibrary.Infrastructure
{
    public static class DependencyInjectionConfigurator
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PostgresUserDbConnection") ??
                throw new InvalidOperationException("Connection string 'PostgresUserDbConnection' not found.");
            var jwtSettings = configuration.GetSection("JwtSettings");

            services.AddCors(options =>
            {
                options.AddPolicy("OpenCorsPolicy", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
            services.AddDbContext<CustomIdentityDbContext>(options => options.UseNpgsql(connectionString));

            services.AddIdentity<UserModel, IdentityRole>(options => options.User.RequireUniqueEmail = true).AddEntityFrameworkStores<CustomIdentityDbContext>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["validIssuer"],
                    ValidAudience = jwtSettings["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["secretKey"]))
                };
            });

            services.AddScoped<IUserService, UserService>();

            services.AddMassTransit(busConfiguration =>
            {
                busConfiguration.AddConsumer<UserCreatingConsumer>((context, consumer) => 
                {
                    consumer.UseInMemoryOutbox(context);
                });
                busConfiguration.UsingRabbitMq((context, bus) =>
                {
                    bus.Host(new Uri(configuration.GetValue<string>("RabbitMqSettings:Url")), host =>
                    {
                        host.Username(configuration.GetValue<string>("RabbitMqSettings:Username"));
                        host.Password(configuration.GetValue<string>("RabbitMqSettings:Password"));
                    });

                    bus.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
