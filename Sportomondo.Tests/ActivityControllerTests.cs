using FluentAssertions;
using Sportomondo.Api;
using Sportomondo.Api.Context;
using Sportomondo.Api.Responses;
using Sportomondo.Tests.Helpers;
using System.Net;

namespace Sportomondo.Tests
{
    public class ActivityControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ActivityControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            #region OldFactoryNotWorking
            //_factory = factory.WithWebHostBuilder(builder => //2 razy wchodzi?
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

            //https://github.com/dotnet/AspNetCore.Docs.Samples/tree/main/test/integration-tests/IntegrationTestsSample
            #endregion
        }

        [Fact]
        public async Task GetAll_WithEmptyQueryParameter_ReturnsOkResult()
        {
            // arrange

            await PrepareActivitiesInDatabase();

            // act

            var response = await _client.GetAsync("api/activities");
            var responseActivitiesDto = await response.Content.ReadFromJsonAsync<IEnumerable<ActivityResponse>>();

            // assert

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseActivitiesDto.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAll_WithQueryParameterCity_ReturnsOkResult()
        {
            // arrange

            await PrepareActivitiesInDatabase();

            // act

            var response = await _client.GetAsync("api/activities?searchPhraseNameCity=Warsaw");
            var responseActivitiesDto = await response.Content.ReadFromJsonAsync<IEnumerable<ActivityResponse>>();

            // assert

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseActivitiesDto.Should().HaveCount(1);
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
    }
}