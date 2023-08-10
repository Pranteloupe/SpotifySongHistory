using System;
namespace SpotifySongHistory.Data
{
	public class User
	{
		public string? country { get; set; }
		public string? display_name { get; set; }
		public string? email { get; set; }
		public Explicit? explicit_content { get; set; }
		public ExternalUrl? external_urls { get; set; }
		public Followers? followers { get; set; }
		public string? href { get; set; }
		public string? id { get; set; }
		public List<ImageObject>? images { get; set; }
		public string? product { get; set; }
		public string? type { get; set; }
		public string? uri { get; set; }
	}

    public class Explicit {
        public bool filter_enabled { get; set; }
        public bool filter_locked { get; set; }
    }

    public class ExternalUrl {
        public string? spotify { get; set; }
    }

    public class Followers {
        public string? href { get; set; }
        public int total { get; set; }
    }

    public class ImageObject {
        public string? url { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }
}

