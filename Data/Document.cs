using System;
using MongoDB.Bson.Serialization.Attributes;

namespace SpotifySongHistory.Data
{
    [BsonIgnoreExtraElements]
	public class Document
	{
		public string? username { get; set; }
        public string? access_token { get; set; }
        public string? refresh_token { get; set; }
        public double time { get; set; }
        public int page { get; set; }
		public string? type { get; set; }
        public List<Song>? songs { get; set; }
	}

    public class Song {
        public string? track { get; set; }
        public string? album { get; set; }
        public List<string>? artists { get; set; }
        public string? song_id { get; set; }
        public double played_at { get; set; }
        public int length { get; set; }
        public List<string>? genres { get; set; }
        public int popularity { get; set; }
        public string? track_link { get; set; }
        public string? album_link { get; set; }
        public List<string>? artist_links { get; set; }
        public double acousticness { get; set; }
        public string? analysis_url { get; set; }
        public double danceability { get; set; }
        public double energy { get; set; }
        public double instrumentalness { get; set; }
        public int key { get; set; }
        public double liveness { get; set; }
        public double loudness { get; set; }
        public int mode { get; set; }
        public double speechiness { get; set; }
        public double tempo { get; set; }
        public int time_signature { get; set; }
        public double valence { get; set; }
    }
}

