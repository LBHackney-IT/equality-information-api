using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EqualityInformationApi.Tests.V1.E2ETests.Steps
{
    public class BaseSteps
    {
        protected readonly HttpClient _httpClient;

        protected HttpResponseMessage _lastResponse;
        protected readonly JsonSerializerOptions _jsonOptions;

        protected const string Token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImUyZS10ZXN0aW5nQGRldmVsb3BtZW50LmNvbSIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiVGVzdGVyIiwiZ3JvdXBzIjpbImUyZS10ZXN0aW5nIl0sImlhdCI6MTYyMzA1ODIzMn0.SooWAr-NUZLwW8brgiGpi2jZdWjyZBwp4GJikn0PvEw";

        public BaseSteps(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = CreateJsonOptions();
        }

        protected JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }

        public T DecodeResponse<T>(HttpResponseMessage response)
        {
            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var responseObject = JsonSerializer.Deserialize<T>(responseContent, CreateJsonOptions());

            return responseObject;
        }
    }
}
