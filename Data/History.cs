using System;
using Newtonsoft.Json;
using MongoDB.Driver;
using SpotifySongHistory.Data;
using System.Security.Authentication;
using MongoDB.Bson;
using System.Text.Json;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

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
        private string result = "";

        public History() {
            settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            mongoClient = new MongoClient(settings);
        }

        //runs automatically when songhistory.razor is opened
        public async Task GetHistoryAsync(string access, string refresh) {
            h = "";

            //await setUsername(access);

            if (httpClient.DefaultRequestHeaders.Contains("Authorization")) {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + access);

            List<Song> songs = new List<Song>();

            apiLink += "/player/recently-played?limit=50";
            var request = new HttpRequestMessage(HttpMethod.Get, apiLink);
            var response = await httpClient.SendAsync(request);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            RecentlyPlayed? recentlyPlayed = JsonConvert.DeserializeObject<RecentlyPlayed>(jsonResponse);

            var count = recentlyPlayed?.items?.Count != null ? recentlyPlayed.items.Count : 0;
            string ids = "";

            BsonSerializer.RegisterSerializer(
                typeof(decimal),
                new DecimalSerializer(BsonType.Double,
                new RepresentationConverter(
                    true, // allow overflow, return decimal.MinValue or decimal.MaxValue instead
                    true //allow truncation
                ))
            );

            for (int i = count - 1; i >= 0; i--) {
                h += recentlyPlayed?.items?[i].track?.name + " - " + recentlyPlayed?.items?[i].track?.artists?.First<Artist>().name + recentlyPlayed?.items?[i].played_at + recentlyPlayed?.items?[i].track?.album?.genres + "\n";
                //Console.WriteLine(recentlyPlayed?.items?[i].track?.name);
                songs.Add(SongToDocument(recentlyPlayed?.items?[i]));
                ids += recentlyPlayed?.items?[i].track?.id + ",";
            }

            string audioLink = "https://api.spotify.com/v1/audio-features";
            audioLink += "?ids=" + ids.Substring(0, ids.Length - 1);
            var audioRequest = new HttpRequestMessage(HttpMethod.Get, audioLink);
            var audioResponse = await httpClient.SendAsync(audioRequest);
            var audioJson = await audioResponse.Content.ReadAsStringAsync();
            AudioFeatures? audioFeatures = JsonConvert.DeserializeObject<AudioFeatures>(audioJson);

            for (int i = 0; i < songs.Count; i++) {
                if (audioFeatures?.audio_features?[i] != null) {
                    songs[i].acousticness = audioFeatures.audio_features[i].acousticness;
                    songs[i].analysis_url = audioFeatures.audio_features[i].analysis_url;
                    songs[i].danceability = audioFeatures.audio_features[i].danceability;
                    songs[i].energy = audioFeatures.audio_features[i].energy;
                    songs[i].instrumentalness = audioFeatures.audio_features[i].instrumentalness;
                    songs[i].key = audioFeatures.audio_features[i].key;
                    songs[i].liveness = audioFeatures.audio_features[i].liveness;
                    songs[i].loudness = audioFeatures.audio_features[i].loudness;
                    songs[i].mode = audioFeatures.audio_features[i].mode;
                    songs[i].speechiness = audioFeatures.audio_features[i].speechiness;
                    songs[i].tempo = audioFeatures.audio_features[i].tempo;
                    songs[i].time_signature = audioFeatures.audio_features[i].time_signature;
                    songs[i].valence = audioFeatures.audio_features[i].valence;
                }
            }
           
            var findDocument = mongoClient.GetDatabase("SpotifySongHistory").GetCollection<Document>("SpotifySongs").Find(a => a.username == username).SingleOrDefault() != null
                ? mongoClient.GetDatabase("SpotifySongHistory").GetCollection<Document>("SpotifySongs").Find(a => a.username == username).ToList() : null;

            if (findDocument == null) {
                Document document = new Document();
                document.username = username;
                document.access_token = access;
                document.refresh_token = refresh;
                document.time = DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMilliseconds;
                document.page = 0;
                document.type = "recently-played";
                document.songs = songs;

                string json = JsonConvert.SerializeObject(document, Formatting.Indented);

                BsonDocument bsonDocument = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(json);
                var dotnetObj = BsonTypeMapper.MapToDotNetValue(bsonDocument);
                string json2 = JsonConvert.SerializeObject(dotnetObj);
                await mongoClient.GetDatabase("SpotifySongHistory").GetCollection<BsonDocument>("SpotifySongs").InsertOneAsync(bsonDocument);
            } else {
                var mydoc = findDocument.First();
                var currentSongs = mydoc.songs;

                var filter = Builders<Document>.Filter.Eq(x => x.username, username);
                var refreshSongDefinition = Builders<Document>.Update.Set(t => t.refresh_token, refresh);

                double? timeOfLast = currentSongs?.Last().played_at;
                if (timeOfLast != songs.Last().played_at) {
                    for (int i = 0; i < songs.Count; i++) {
                        if (timeOfLast < songs[i].played_at) {
                            for (int j = i; j < songs.Count; j++) {
                                var pushSongDefinition = Builders<Document>.Update.Push(t => t.songs, songs[j]);
                                var addNewSongResult = await mongoClient.GetDatabase("SpotifySongHistory").GetCollection<Document>("SpotifySongs").UpdateOneAsync(filter, pushSongDefinition);
                            }
                            break;
                        }
                    }
                }
            }
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
            if (playHistory?.track?.album?.genres != null) {
                song.genres.Concat<string>(playHistory.track.album.genres);
            }
            song.song_id = playHistory?.track?.id;
            song.played_at = playHistory?.played_at != null ? playHistory.played_at.ToUniversalTime().Subtract(DateTime.UnixEpoch).TotalMilliseconds: 0;
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


