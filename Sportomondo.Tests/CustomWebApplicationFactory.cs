using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Context;
using Sportomondo.Tests.Authorization;
using System.Data.Common;

namespace Sportomondo.Tests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
        where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextOptions = services
                        .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<SportomondoDbContext>));

                services.Remove(dbContextOptions);

                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>(); //omijanie autoryzacji

                services.AddMvc(option => option.Filters.Add(new FakeUserFilter())); //dodawanie Usera do httpContext

                services
                     .AddDbContext<SportomondoDbContext>(options => options.UseInMemoryDatabase("SportomondoDbTest"));
            });
        }
    }
}