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
        public double played_at { get; set; }
        public int length { get; set; }
        public List<string>? genres { get; set; }
        public int popularity { get; set; }
        public string? track_link { get; set; }
        public string? album_link { get; set; }
        public List<string>? artist_links { get; set; }
    }
}

