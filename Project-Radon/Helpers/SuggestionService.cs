using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace Project_Radon.Helpers
{
    public class DuckDuckGoSuggestion
    {
        public string phrase { get; set; }
    }
    public static class SuggestionService
    {
        public static async Task<List<string>> GetSuggestionsAsync(string query)
        {
            // DuckDuckGo's
            if (string.IsNullOrWhiteSpace(query)) return new List<string>();

            var client = new HttpClient();
            var url = $"https://duckduckgo.com/ac/?q={Uri.EscapeDataString(query)}";
            var json = await client.GetStringAsync(url);

            var suggestions = JsonSerializer.Deserialize<List<DuckDuckGoSuggestion>>(json);
            return suggestions?.Select(s => s.phrase).ToList() ?? new List<string>();

            // Google's

        }
    }
}
