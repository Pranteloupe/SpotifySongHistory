using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http;


namespace SpotifyHistory.Data {
    public static class SpotifyAuth {
        public static string apiLink = "https://accounts.spotify.com/api/token";
        private static string client_id = "3043fbd8bee54c6a8ac2a6fafd256418";
        private static string client_secret = "9d963c9982b74173b6b07f55b68fc91e";
        //private static string redirect = "https://spotifyhistory.azurewebsites.net/history"; //when testing make this a comment
        private static string redirect = "http://127.0.0.1:5157/history";
        private static string _accessToken = "";
        private static string _refreshToken = "";
        private static HttpClient client = new HttpClient();
        private static string error = "";

        private static async Task SpotifyPostAsync(string Token) {
            var queryBuilder = new QueryBuilder();
            queryBuilder.Add("grant_type", "authorization_code");
            queryBuilder.Add("code", Token);
            queryBuilder.Add("redirect_uri", redirect);

            var request = new HttpRequestMessage(HttpMethod.Post, apiLink);

            request.Content = new FormUrlEncodedContent(queryBuilder);
            if (client.DefaultRequestHeaders.Contains("Authorization")) {
                client.DefaultRequestHeaders.Remove("Authorization");
            }
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(client_id + ":" + client_secret)));

            var response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                AccessToken? access = JsonConvert.DeserializeObject<AccessToken>(jsonResponse);
                _accessToken = access?.access_token != null ? access.access_token : "Default";
                _refreshToken = access?.refresh_token != null ? access.refresh_token : "Default";

               // History history = new History();
               // await history.setUsername(_accessToken);
            }
            else {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                error = jsonResponse;
                //response.EnsureSuccessStatusCode();
            }
        }

        public static async Task SpotifyRefreshAsync(string Token) {
            var queryBuilder = new QueryBuilder();
            queryBuilder.Add("grant_type", "refresh_token");
            queryBuilder.Add("refresh_token", Token);

            var request = new HttpRequestMessage(HttpMethod.Post, apiLink);
            request.Content = new FormUrlEncodedContent(queryBuilder);
            if (client.DefaultRequestHeaders.Contains("Authorization")) {
                client.DefaultRequestHeaders.Remove("Authorization");
            }
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(client_id + ":" + client_secret)));

            var response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                AccessToken? access = JsonConvert.DeserializeObject<AccessToken>(jsonResponse);
                _accessToken = access?.access_token != null ? access.access_token : "Default";  

                History history = new History();
                await history.setUsername(_accessToken);
            }
            else {
               response.EnsureSuccessStatusCode();
            }
        }

        public static string getAccessToken() {
            return _accessToken;
        }

        public static string getError() {
            return error;
        }

        public static string getRefreshToken() {
            return _refreshToken;
        }

        public static async void getTokens(string? Token) {
            if (Token != null) {
                await SpotifyPostAsync(Token);
            }
        }

        public static string authorizationLinkBuilder() {
            string link = "https://accounts.spotify.com/authorize?";
            link += "response_type=code";
            link += "&client_id=" + client_id;
            link += "&scope=user-read-recently-played";
            link += "&redirect_uri=" + redirect;
            link += "&show_dialog=true";
            return link;
        }
    }
}

