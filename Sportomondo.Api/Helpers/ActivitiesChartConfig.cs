using System.Text.Json.Serialization;

namespace Sportomondo.Api.Helpers
{
    public class ActivitiesChartConfig
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("data")]
        public ActivitiesChartConfigData Data { get; set; }
    }

    public class ActivitiesChartConfigData
    {
        [JsonPropertyName("labels")]
        public string[] Labels { get; set; }
        [JsonPropertyName("datasets")]
        public ActivitiesChartConfigDataDataset[] Datasets { get; set; }
    }

    public class ActivitiesChartConfigDataDataset
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("data")]
        public int[] Data { get; set; }
    }
}
