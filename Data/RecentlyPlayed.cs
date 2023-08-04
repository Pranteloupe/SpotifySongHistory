using System;
using Newtonsoft.Json;

namespace SpotifyHistory.Data {
    public class RecentlyPlayed {
        [JsonProperty("items")]
        public List<PlayHistory>? items { get; set; }
        [JsonProperty("total")]
        public int total { get; set; }
        public Cursors? cursors { get; set; }
        public string? next { get; set; }
        public int limit { get; set; }
        public string? href { get; set; }
    }

        public class Cursors {
            public string? after { get; set; }
            public string? before { get; set; }
        }

        public class PlayHistory {
            public Track? track { get; set; }
            public string? played_at { get; set; }
            public Context? context { get; set; }
        }

        public class Track {
            public Album? album { get; set; }
            public List<Artist>? artists { get; set; }
            public List<string>? available_markets { get; set; }
            public int disc_number { get; set; }
            public int duration_ms { get; set; }
            [JsonProperty("explicit")]
            public bool? explicitt { get; set; }
            public ExternalID? external_ids { get; set; }
            public ExternalURL? external_urls { get; set; }
            public string? href { get; set; }
            public string? id { get; set; }
            public bool is_playable { get; set; }
            public LinkedFrom? linked_from { get; set; }
            public Restriction? restrictions { get; set; }
            public string? name { get; set; }
            public int popularity { get; set; }
            public string? preview_url { get; set; }
            public int track_number { get; set; }
            public string? type { get; set; }
            public string? uri { get; set; }
            public bool id_local { get; set; }
        }

        public class Album {
            public string? album_type { get; set; }
            public int total_tracks { get; set; }
            public List<string>? available_markets { get; set; }
            public ExternalURL? external_urls { get; set; }
            public string? href { get; set; }
            public string? id { get; set; }
            public List<Image>? images { get; set; }
            public string? name { get; set; }
            public string? release_date { get; set; }
            public string? release_date_precision { get; set; }
            public Restriction? restrictions { get; set; }
            public string? type { get; set; }
            public string? uri { get; set; }
            public List<Copyright>? copyrights { get; set; }
            public ExternalID? external_ids { get; set; }
            public List<string>? genres { get; set; }
            public string? label { get; set; }
            public int popularity { get; set; }
            public string? album_group { get; set; }
            public List<SimplifiedArtist>? artists { get; set; }
        }

        public class LinkedFrom {

        }

        public class Restriction {
            public string? reason { get; set; }
        }

        public class Copyright {
            public string? text { get; set; }
            public string? type { get; set; }
        }

        public class Image {
            public string? url { get; set; }
            public int height { get; set; }
            public int width { get; set; }
        }

        public class SimplifiedArtist {
            public ExternalURL? external_urls { get; set; }
            public string? href { get; set; }
            public string? id { get; set; }
            public string? name { get; set; }
            public string? type { get; set; }
            public string? uri { get; set; }
        }

        public class Artist {
            public ExternalURL? external_urls { get; set; }
            public Follower? followers { get; set; }
            public List<string>? genres { get; set; }
            public string? href { get; set; }
            public string? id { get; set; }
            public List<Image>? images { get; set; }
            public string? name { get; set; }
            public int popularity { get; set; }
            public string? type { get; set; }
            public string? uri { get; set; }
        }

        public class Follower {
            public string? href { get; set; }
            public int total { get; set; }
        }

        public class ExternalURL {
            public string? spotify { get; set; }
        }

        public class ExternalID {
            public string? isrc { get; set; }
            public string? ean { get; set; }
            public string? upc { get; set; }
        }

        public class Context {
            public string? type { get; set; }
            public string? href { get; set; }
            public ExternalURL? external_urls { get; set; }
            public string? uri { get; set; }
        }
    }

