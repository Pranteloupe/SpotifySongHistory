using System;
using Newtonsoft.Json;
using MongoDB.Driver;
using SpotifySongHistory.Data;
using System.Security.Authentication;
using MongoDB.Bson;

namespace SpotifyHistory.Data {
    public class History {
        private string apiLink = "https://api.spotify.com/v1/me";
        private HttpClient httpClient = new HttpClient();
        private string? h;
        private string connectionString = @"mongodb://songhistories:fGCgwjDtJB0laQWliG3OOVfX1sMEBRgcxXuOiVg5njHmQ0WGJ45FMNVsigC6O6Wsscvr1CuC7wdsACDbiJz77w==@songhistories.mongo.cosmos.azure.com:10255/?ssl=true&retrywrites=false&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@songhistories@";
        private MongoClientSettings settings;
        private MongoClient mongoClient;
        private string username = "";
        private string displayName = "";

        public History() {
            settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            mongoClient = new MongoClient(settings);
        }

        //This is the sync method. It gets all the previously listened to tracks, tranforms them into songs, and then adds it to the database
        public async Task GetHistoryAsync(string access) {
            var request = new HttpRequestMessage(HttpMethod.Get, apiLink + "/player/recently-played");
            h = "";

            if (httpClient.DefaultRequestHeaders.Contains("Authorization")) {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + access);

            var response = await httpClient.SendAsync(request);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            RecentlyPlayed? recentlyPlayed = JsonConvert.DeserializeObject<RecentlyPlayed>(jsonResponse);
            //Transforming all songs
            List<Song> songs = new List<Song>();
            for (int i = 0; i < 15; i++) {
                h += recentlyPlayed?.items?[i].track?.name + " - " + recentlyPlayed?.items?[i].track?.artists?.First<Artist>().name + recentlyPlayed?.items?[i].played_at + "\n";
                songs.Add(SongToDocument(recentlyPlayed?.items?[i]));
            }
            //Serialize songs into a list of songs
            Document d = new Document();
            d.username = "testing3";
            d.lastsync = DateTime.Now;
            d.songs = songs;
            string json = JsonConvert.SerializeObject(d, Formatting.Indented);
            
            //Check if username exists in database, but for now just add stuff
            BsonDocument document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(json);
            await mongoClient.GetDatabase("SpotifySongHistory").GetCollection<BsonDocument>("SpotifySongs").InsertOneAsync(document);
        }

        public async Task setUsername(string access) {
            var request = new HttpRequestMessage(HttpMethod.Get, apiLink);

            if (httpClient.DefaultRequestHeaders.Contains("Authorization")) {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + access);

            var response = await httpClient.SendAsync(request);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            User? user = JsonConvert.DeserializeObject<User>(jsonResponse);
            username = user?.id != null ? user.id : "";
            displayName = user?.display_name != null ? user.display_name : "";
        }

        //Creates a song element from track info
        private Song SongToDocument(PlayHistory? playHistory) {
            Song song = new Song();
            song.track = playHistory?.track?.name;
            song.album = playHistory?.track?.album?.name;
            song.artists = new List<string>();
            song.artist_links = new List<string>();
            song.genres = new List<string>();
            if (playHistory?.track?.artists != null) {
                foreach (Artist artist in playHistory.track.artists) {
                    if (artist.name != null && artist.external_urls?.spotify != null) {
                        song.artists.Add(artist.name);
                        song.artist_links.Add(artist.external_urls.spotify);
                        if (artist.genres != null) {
                            song.genres.Concat<string>(artist.genres);
                        }
                    }
                }
            }
            song.played_at = playHistory?.played_at != null ? playHistory.played_at : DateTime.MinValue;
            song.length = playHistory?.track?.duration_ms != null ? playHistory.track.duration_ms / 1000 : 0;
            song.genres = playHistory?.track?.artists?.First().genres;

            song.popularity = playHistory?.track?.popularity != null ? playHistory.track.popularity : 0;
            song.track_link = playHistory?.track?.external_urls?.spotify;
            song.album_link = playHistory?.track?.album?.external_urls?.spotify;
            return song;
        }

        //if a document with username does not exist in db
        private Document createNewDocument() {
            Document document = new Document();
            return document;
        }

        public string getDisplayName() {
            return displayName;
        }

        public string? GetSongHistory() {
            return h;
        }

        public string getUsername() {
            return username;
        }
    }
}

