using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using Sportomondo.Api.Authorization;
using Sportomondo.Api.BackgroundServices;
using Sportomondo.Api.Context;
using Sportomondo.Api.Middlewares;
using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;
using Sportomondo.Api.Requests.Validators;
using Sportomondo.Api.Seeders;
using Sportomondo.Api.Services;
using System;
using System.Text;

namespace Sportomondo.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // NLog: Setup NLog for Dependency injection
            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(LogLevel.Trace);
            builder.Host.UseNLog();

            // Add services to the container.
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<SportomondoDbContext>
                (options => options.UseSqlServer(builder.Configuration.GetConnectionString("SportomondoDbConnection")));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   //options.RequireHttpsMetadata = false,
                   //options.SaveToken = true;
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = builder.Configuration["Jwt:Issuer"],
                       ValidAudience = builder.Configuration["Jwt:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                   };
               });

            builder.Services.AddAuthorizationWithPolicies();

            builder.Services.AddHostedService<ActivitySeriesReminderService>();

            builder.Services.AddTransient<IEmailSenderService, EmailSenderService>();
            builder.Services.AddScoped<DataSeeder>();
            builder.Services.AddScoped<ExceptionHandlingMiddleware>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserRequestValidator>();
            builder.Services.AddScoped<IAuthorizationHandler, AuthorizationRequirementHandler>();

            builder.Services.AddScoped<IHttpContextService, HttpContextService>();
            builder.Services.AddScoped<IActivityService, ActivityService>();
            builder.Services.AddScoped<IManageActivityService, ManageActivityService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAchievementService, AchievementService>();
            builder.Services.AddScoped<ISummaryService, SummaryService>();

            builder.Services.AddControllers();
            builder.Services.AddFluentValidationAutoValidation();    

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Bearer Authentication with JWT Token",
                    Type = SecuritySchemeType.Http
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin", policyBuilder =>
                {
                    //basic configuration only for presentation purposes - allow any origin
                    policyBuilder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowAnyOrigin();
                });
            });

            var app = builder.Build();

            app.UseCors("AllowAnyOrigin");

            var scope = app.Services.CreateScope();
            var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            
            dataSeeder.ApplyPendingMigrations();
            dataSeeder.Seed();

            app.UseMiddleware<ExceptionHandlingMiddleware>(); //it should be the first one - https://www.c-sharpcorner.com/article/overview-of-middleware-in-asp-net-core/

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}