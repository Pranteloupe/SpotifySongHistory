using System;
using System.Security.Authentication;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using MongoDB.Driver;
using SpotifySongHistory.Data;

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

        public List<Song> Search(string query, string username) {
            result = "Howdy!";
            Console.WriteLine("PPP");

            var findDocument = mongoClient.GetDatabase("SpotifySongHistory").GetCollection<Document>("SpotifySongs").Find(a => a.username == username).SingleOrDefault() != null
                ? mongoClient.GetDatabase("SpotifySongHistory").GetCollection<Document>("SpotifySongs").Find(a => a.username == username).ToList() : null;
            Console.WriteLine("XXX");
            List<Song> songs = new List<Song>();

            if (findDocument != null) {
                foreach (Document document in findDocument) {
                    Console.WriteLine("Here");
                    if (document.songs != null) {
                        foreach (Song song in document.songs) {
                            if (song.track.ToLower().Contains(query.ToLower()) || song.album.ToLower().Contains(query.ToLower())) {
                                songs.Add(song);
                                result += song.track;
                            }
                            else {
                                foreach (string artist in song.artists) {
                                    if (artist.ToLower().Contains(query.ToLower())) {
                                        songs.Add(song);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return songs;
        }
    }
}

