﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sportomondo.Api;
using Sportomondo.Api.Context;
using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;
using Sportomondo.Api.Responses;
using Sportomondo.Tests.Helpers;
using System.Net;

namespace Sportomondo.Tests
{
    public class ActivityControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private const string baseUri = "api/activities";

        public ActivityControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData(null, 2)]
        [InlineData("?searchPhraseNameCity=Warsaw", 1)]
        public async Task GetAll_WithValidQueryParameter_ReturnsOkResult(string queryParameter, int expectedActivities)
        {
            // arrange

            await PrepareActivitiesInDatabase();

            // act

            var response = await _client.GetAsync($"{baseUri}{queryParameter}");
            var responseActivitiesDto = await response.Content
                .ReadFromJsonAsync<IEnumerable<ActivityResponse>>();

            // assert

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseActivitiesDto.Should().HaveCount(expectedActivities);
        }

        [Fact]
        public async Task Get_WithValidRouteParameter_ReturnsOkResult()
        {
            // arrange

            await PrepareActivitiesInDatabase();

            int id;

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SportomondoDbContext>();
                var activity = await dbContext.Activities
                    .FirstAsync();
                
                id = activity.Id;
            }

            // act

            var response = await _client.GetAsync($"{baseUri}/{id}");
            var responseActivityDto = await response.Content
                .ReadFromJsonAsync<ActivityResponse>();

            // assert

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseActivityDto.Should().NotBeNull();
            responseActivityDto?.Id.Should().Be(id);
        }

        [Fact]
        public async Task Get_WithInvalidRouteParameter_ReturnsNotFoundResult()
        {
            // arrange

            await PrepareActivitiesInDatabase();
            
            int nonExistentId = 0; //min Id is 1

            // act

            var response = await _client.GetAsync($"{baseUri}/{nonExistentId}");

            // assert

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_WithValidModel_ReturnsCreatedResult()
        {
            // arrange

            var dto = new CreateActivityRequest()
            {
                Name = "Running Test",
                DateStart = DateTime.Now,
                DateFinish = DateTime.Now.AddHours(1),
                Distance = 10,
                AverageHr = 169,
                City = "Cracow",
                RouteUrl = null,
                Type = ActivityType.Running
            };

            // act

            var response = await _client.PostAsJsonAsync($"{baseUri}", dto);

            // assert

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Create_WithInvalidModel_ReturnsBadRequestResult()
        {
            // arrange

            var dto = new CreateActivityRequest()
            {
                Name = "Test Running Invalid",
                DateStart = DateTime.Now,
                DateFinish = DateTime.Now.AddHours(-1), //wrong
                Distance = 10,
                AverageHr = 169,
                City = "WrongCityName", //wrong
                RouteUrl = null,
                Type = ActivityType.Running
            };

            // act

            var response = await _client.PostAsJsonAsync($"{baseUri}", dto);

            // assert

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_WithValidRouteParameter_ReturnsNoContentResult()
        {
            // arrange

            await PrepareActivitiesInDatabase();

            int id;

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SportomondoDbContext>();
                var activity = await dbContext.Activities
                    .FirstAsync();

                id = activity.Id;
            }

            // act

            var response = await _client.DeleteAsync($"{baseUri}/{id}");

            // assert

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SportomondoDbContext>();
                var activity = await dbContext.Activities
                    .FirstOrDefaultAsync(a => a.Id == id);

                activity.Should().BeNull();
            }
        }

        [Fact]
        public async Task Delete_WithInvalidRouteParameter_ReturnsNotFoundResult()
        {
            // arrange

            await PrepareActivitiesInDatabase();

            int nonExistentId = 0; //min Id is 1

            // act

            var response = await _client.DeleteAsync($"{baseUri}/{nonExistentId}");

            // assert

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithValidModelAndRouteParameter_ReturnsOkResult()
        {
            // arrange

            await PrepareActivitiesInDatabase();

            int id, previousWeatherId;
            string previousActivityName;

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SportomondoDbContext>();
                var activity = await dbContext.Activities
                    .Include(a => a.Weather)
                    .FirstAsync();

                id = activity.Id;
                previousActivityName = activity.Name;
                previousWeatherId = activity.Weather.Id;
            }

            var dto = new ActivityRequest()
            {
                Name = "Test Running After Update",
                DateStart = DateTime.Now,
                DateFinish = DateTime.Now.AddHours(1),
                Distance = 10,
                AverageHr = 169,
                City = "Berlin",
                RouteUrl = null,
            };

            // act

            var response = await _client.PutAsJsonAsync($"{baseUri}/{id}", dto);

            // assert

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SportomondoDbContext>();
                var activity = await dbContext.Activities
                    .Include(a => a.Weather)
                    .FirstAsync(a => a.Id == id);

                activity.Name.Should().NotBe(previousActivityName);
                activity.Weather.Id.Should().NotBe(previousWeatherId);
            }
        }

        [Fact]
        public async Task Update_WithInvalidRouteParameter_ReturnsNotFoundResult()
        {
            // arrange

            await PrepareActivitiesInDatabase();

            int nonExistentId = 0; //min Id is 1

            var dto = new ActivityRequest()
            {
                Name = "Test Running After Update",
                DateStart = DateTime.Now,
                DateFinish = DateTime.Now.AddHours(1),
                Distance = 10,
                AverageHr = 169,
                City = "Berlin",
                RouteUrl = null,
            };

            // act

            var response = await _client.PutAsJsonAsync($"{baseUri}/{nonExistentId}", dto);

            // assert

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithInvalidModel_ReturnsBadRequestResult()
        {
            // arrange

            await PrepareActivitiesInDatabase();

            int id;

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SportomondoDbContext>();
                var activity = await dbContext.Activities
                    .FirstAsync();

                id = activity.Id;
            }

            var dto = new ActivityRequest()
            {
                Name = "Test Running After Update Invalid",
                DateStart = DateTime.Now,
                DateFinish = DateTime.Now.AddHours(-1), //wrong
                Distance = 10,
                AverageHr = 169,
                City = "WrongCityName", //wrong
                RouteUrl = null,
            };

            // act

            var response = await _client.PutAsJsonAsync($"{baseUri}/{id}", dto);

            // assert

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private async Task PrepareActivitiesInDatabase()
        {
            using var scope = _factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<SportomondoDbContext>();

            await Utilities.InitializeActivitiesForTests(dbContext);
        }

        #region testingXyzServiceOld
        //private readonly Mock<IManageActivityService> _manageActivityMock = new Mock<IManageActivityService>();
        //private readonly Mock<IHttpContextService> _httpContextMock = new Mock<IHttpContextService>();

        //[Fact]
        //public async Task GetAll_Sth()
        //{
        //    var contextOptions = new DbContextOptionsBuilder<SportomondoDbContext>()
        //        .UseInMemoryDatabase("BazaTestowa")
        //        .Options;

        //    using var context = new SportomondoDbContext(contextOptions);

        //    ActivityService _service = new ActivityService(context, _manageActivityMock.Object, _httpContextMock.Object);

        //    _httpContextMock.Setup(x => x.UserRoleName).Returns("Member");

        //    var result = await _service.GetAllAsync("");

        //    result.Should().BeEmpty();

        //}
        #endregion

        #region loginMethodOld
        //var loginDto = new LoginUserRequest
        //{
        //    Email = "user",
        //    Password = "password",
        //};

        //var json = System.Text.Json.JsonSerializer.Serialize(loginDto);
        //var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        //var response = await _client.PostAsync("api/users/login", content);
        //var responseContent = await response.Content.ReadFromJsonAsync<LoginUserResponse>();

        //_client.DefaultRequestHeaders.Add("Authorization", $"Bearer {responseContent.Token}");
        #endregion

        #region factoryFromConstructorNotSureIfWorkingOld
        //_factory = factory.WithWebHostBuilder(builder => //2 razy wchodzi? chyba juz nie
        //{
        //    builder.ConfigureServices(services =>
        //    {
        //        var dbContextOptions = services
        //                .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<SportomondoDbContext>));

        //        services.Remove(dbContextOptions);

        //        services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

        //        services.AddMvc(option => option.Filters.Add(new FakeUserFilter()));

        //        services
        //             .AddDbContext<SportomondoDbContext>(options => options.UseInMemoryDatabase("SportomondoDbTest"));

        //    });
        //});

        //_client = factory.CreateClient(); //brak "_"
        #endregion

        //https://github.com/dotnet/AspNetCore.Docs.Samples/tree/main/test/integration-tests/IntegrationTestsSample
    }
}