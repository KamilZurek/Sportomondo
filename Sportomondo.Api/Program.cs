using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

            // Add services to the container.
            builder.Services.AddHttpClient();
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

            builder.Services.AddScoped<DataSeeder>();
            builder.Services.AddScoped<ExceptionHandlingMiddleware>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<IActivityService, ActivityService>();
            builder.Services.AddScoped<IManageActivityService, ManageActivityService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddControllers();
            builder.Services.AddFluentValidationAutoValidation();

            builder.Services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserRequestValidator>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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