using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Context;
using Sportomondo.Api.Models;
using Sportomondo.Api.Seeders;
using Sportomondo.Api.Services;

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

            builder.Services.AddScoped<DataSeeder>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<IActivityService, ActivityService>();
            builder.Services.AddScoped<IManageActivityService, ManageActivityService>();
            
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            var scope = app.Services.CreateScope();
            var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            
            dataSeeder.ApplyPendingMigrations();
            dataSeeder.Seed();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}