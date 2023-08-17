using System;
using System.Security.Authentication;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using MongoDB.Driver;
using SpotifySongHistory.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SpotifyHistory.Data
{
	public class Searching
	{
        private string connectionString = @"mongodb://songhistories:fGCgwjDtJB0laQWliG3OOVfX1sMEBRgcxXuOiVg5njHmQ0WGJ45FMNVsigC6O6Wsscvr1CuC7wdsACDbiJz77w==@songhistories.mongo.cosmos.azure.com:10255/?ssl=true&retrywrites=false&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@songhistories@";
        private MongoClientSettings settings;
        private MongoClient mongoClient;
        private string result = "";

		public Searching() {
            settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            mongoClient = new MongoClient(settings);
        }

        public List<Song> Search(string query, string username, Filter filter, string order) {
            result = "Howdy!";
            query = query.ToLower();
            Console.WriteLine("PPP");

            var findDocument = mongoClient.GetDatabase("SpotifySongHistory").GetCollection<Document>("SpotifySongs").Find(a => a.username == username).SingleOrDefault() != null
                ? mongoClient.GetDatabase("SpotifySongHistory").GetCollection<Document>("SpotifySongs").Find(a => a.username == username).ToList() : null;
            Console.WriteLine("XXX");
            List<Song> songs = new List<Song>();

            if (findDocument != null) {
                foreach (Document document in findDocument) {
                    if (document.songs != null) {
                        foreach (Song song in document.songs) {
                            TimeSpan timeSpan = TimeSpan.FromMilliseconds(song.played_at);
                            DateTime dateTime = new DateTime(1970, 1, 1) + timeSpan;
                            bool inArtists = false;
                            foreach (string artist in song.artists) {
                                if (artist.ToLower().Contains(query)) {
                                    inArtists = true;
                                }
                            }
                            if ((!filter.Track && !filter.Artists && !filter.Album) ? true :
                                ((filter.Track ? song.track.ToLower().Contains(query) : false) ||
                                (filter.Album ? song.album.ToLower().Contains(query) : false) ||
                                (filter.Artists ? inArtists : false)) &&
                                filter.Length.min <= song.length &&
                                filter.Length.max >= song.length &&
                                filter.DateAndTime.start <= dateTime.ToLocalTime() &&
                                filter.DateAndTime.end >= dateTime.ToLocalTime()) {
                                songs.Add(song);
                            }
                        }
                    }
                } 
            }

            if (order == "Latest") {
                songs.Reverse();
            } else if (order == "Shortest") {
                songs.Sort((p, q) => p.length.CompareTo(q.length));
            } else if (order == "Longest") {
                songs.Sort((p, q) => q.length.CompareTo(p.length));
            }
            return songs;
        }
    }
}

