using System;
using Newtonsoft.Json;

namespace SpotifyHistory.Data {
    public static class History {
        private static string recentlyPlayedLink = "https://api.spotify.com/v1/me/player/recently-played";
        private static HttpClient httpClient = new HttpClient();
        private static string? h = "";

        public static async Task GetHistoryAsync(string access) {
            var request = new HttpRequestMessage(HttpMethod.Get, recentlyPlayedLink);

            if (httpClient.DefaultRequestHeaders.Contains("Authorization")) {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + access);

            var response = await httpClient.SendAsync(request);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            //Console.WriteLine($"{jsonResponse}");

            RecentlyPlayed? recentlyPlayed = JsonConvert.DeserializeObject<RecentlyPlayed>(jsonResponse);
            for (int i = 0; i < 15; i++) {
                h += recentlyPlayed?.items?[i].track?.name + " - " + recentlyPlayed?.items?[i].track?.artists?.First<Artist>().name + "\n";
            }
        }

        public static string? GetSongHistory() {
            return h;
        }
    }
}

