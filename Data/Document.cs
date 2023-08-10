using System;
namespace SpotifySongHistory.Data
{
	public class Document
	{
		public string? username { get; set; }
		public DateTime lastsync { get; set; }
        public List<Song>? songs { get; set; }
	}

    public class Song {
        public string? track { get; set; }
        public string? album { get; set; }
        public List<string>? artists { get; set; }
        public DateTime played_at { get; set; }
        public int length { get; set; }
        public List<string>? genres { get; set; }
        public int popularity { get; set; }
        public string? track_link { get; set; }
        public string? album_link { get; set; }
        public List<string>? artist_links { get; set; }
    }
}

