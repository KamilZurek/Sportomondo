using Microsoft.Net.Http.Headers;
using Sportomondo.Api.Exceptions;
using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;
using System.Text.Json;
using System.Web;

namespace Sportomondo.Api.Services
{
    public class ManageActivityService : IManageActivityService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ManageActivityService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public Activity CreateFromRequestData(CreateActivityRequest request)
        {
            return new Activity()
            {
                Name = request.Name,
                DateStart = request.DateStart,
                DateFinish = request.DateFinish,
                Type = request.Type,
                Distance = request.Distance,
                AverageHr = request.AverageHr,
                City = request.City,
                RouteUrl = request.RouteUrl,
                UserId = request.UserId
            };
        }

        public void CalculateTime(Activity activity)
        {
            if (activity.DateStart >= activity.DateFinish)
            {
                throw new BadRequestException($"Cannot register activity. DateStart: {activity.DateStart} " +
                    $"should be less than DateFinish: {activity.DateFinish}.");
            }

            activity.Time = activity.DateFinish - activity.DateStart;
        }

        public void CalculatePace(Activity activity)
        {
            if (activity.Time <= TimeSpan.Zero || activity.Distance <= 0m)
            {
                throw new BadRequestException($"Cannot register activity. Time: {activity.Time} " +
                    $"and Distance: {activity.Distance} should be greater than 0.");
            }

            var seconds = (decimal)activity.Time.TotalSeconds;
            var paceInSec = seconds / activity.Distance;
            var pace = TimeSpan.FromSeconds((double)paceInSec);

            activity.Pace = pace;
        }

        public void CalculateCalories(Activity activity, User user)
        {
            activity.KcalBurned = user.Weight * (decimal)activity.Time.TotalHours * activity.AverageHr / 20;
        }

        public async Task<Weather> GetWeatherFromAPIAsync(Activity activity)
        {
            var baseUri = GetApiBaseUri();
            var queryParams = await GetQueryParameters(activity);
            var completeUri = $"{baseUri}?{queryParams}";

            var httpClient = _httpClientFactory.CreateClient();
            var httpRequestMessage = GetHttpRequestMessage(completeUri);

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var json = await httpResponseMessage.Content.ReadAsStringAsync();

                using (var doc = JsonDocument.Parse(json))
                {
                    try
                    {
                        return GetWeather(doc, activity);
                    }
                    catch
                    {
                        throw new BadRequestException("Invalid WeatherAPI Json structure");
                    }
                }
            }
            else
            {
                throw new BadRequestException("Cannot get the weather from WeatherAPI");
            }
        }

        private async Task<string> GetQueryParameters(Activity activity)
        {
            var apiKey = GetApiKey();

            var paramsDict = new Dictionary<string, string>
            {
                { "key", apiKey },
                { "q", activity.City },
                { "dt", activity.DateStart.ToString("yyyy-MM-dd")}
            };

            var dictFormUrlEncoded = new FormUrlEncodedContent(paramsDict);
            var paramsResult = await dictFormUrlEncoded.ReadAsStringAsync();

            return paramsResult;
        }

        private string GetApiBaseUri() => "http://api.weatherapi.com/v1/history.json";
        private string GetApiKey() => _configuration.GetValue<string>("WeatherAPIKey");
        private HttpRequestMessage GetHttpRequestMessage(string uri) => new HttpRequestMessage(
              HttpMethod.Get,
              uri);

        private void GetWeatherTemperatureAndDescriptionFromAPIResult(JsonDocument doc, out decimal avgTempC, out string description)
        {
            var root = doc.RootElement;
            var forecast = root.GetProperty("forecast");
            var forecastDay = forecast.GetProperty("forecastday")[0];
            var day = forecastDay.GetProperty("day");

            var tempTmp = day.GetProperty("avgtemp_c").ToString().Replace('.', ',');
            avgTempC = Convert.ToDecimal(tempTmp);

            var condition = day.GetProperty("condition");
            description = condition.GetProperty("text").ToString();
        }

        private Weather GetWeather(JsonDocument doc, Activity activity)
        {
            GetWeatherTemperatureAndDescriptionFromAPIResult(doc, out decimal temperature, out string description);

            return new Weather()
            {
                Day = activity.DateStart,
                City = activity.City,
                TemperatureC = temperature,
                Description = description,
                ActivityId = activity.Id
            };
        }
    }
}
