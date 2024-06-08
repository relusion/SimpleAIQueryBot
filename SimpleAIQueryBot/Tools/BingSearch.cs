using System.Text.Json;

namespace SimpleAIQueryBot.Tools
{
    public class BingSearch
    {
        private static readonly string _baseUri = "https://api.bing.microsoft.com/v7.0/search";
        private static readonly HttpClient _httpClient = new HttpClient();
        private static string _clientIdHeader = null;

        private const string QueryParameter = "?q=";  // Required
        private const string MktParameter = "&mkt=";  // Strongly suggested
        private const string TextDecorationsParameter = "&textDecorations=";

        private readonly string _subscriptionKey;

        public BingSearch()
        {
            _subscriptionKey = Environment.GetEnvironmentVariable("BING_SEARCH_API_KEY") 
                ?? throw new InvalidOperationException("Bing API key is missing or empty.");

            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
        }

        public async Task<string> SearchAsync(string searchQuery)
        {
            try
            {
                var queryString = BuildQueryString(searchQuery);
                var response = await MakeRequestAsync(queryString);

                _clientIdHeader = response.Headers.GetValues("X-MSEdge-ClientID").FirstOrDefault();

                var contentString = await response.Content.ReadAsStringAsync();

                var jsonDocument = JsonDocument.Parse(contentString);
                var webPages = jsonDocument.RootElement
                    .GetProperty("webPages")
                    .GetProperty("value");

                return ExtractResults(webPages);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error during Bing search: {e.Message}");
                return string.Empty;
            }
        }

        private static string BuildQueryString(string searchQuery)
        {
            var queryString = QueryParameter + Uri.EscapeDataString(searchQuery);
            queryString += MktParameter + "en-us";
            queryString += TextDecorationsParameter + bool.TrueString;
            return queryString;
        }

        private static string ExtractResults(JsonElement webPages)
        {
            var results = webPages.EnumerateArray()
                .Take(3)
                .Select(page => new
                {
                    Name = page.GetProperty("name").GetString(),
                    Url = page.GetProperty("url").GetString(),
                    Snippet = page.GetProperty("snippet").GetString()
                });

            return string.Join("\n\n", results.Select(r => $"Title: {r.Name}\nURL: {r.Url}\nSnippet: {r.Snippet}"));
        }

        private static async Task<HttpResponseMessage> MakeRequestAsync(string queryString)
        {
            return await _httpClient.GetAsync(_baseUri + queryString);
        }
    }
}
