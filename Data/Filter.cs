using System;
namespace SpotifySongHistory.Data
{
	public class Filter {
		public Filter() {
			Track = true;
			Artists = true;
			Album = true;
			Length = (0, 10000);
			Popularity = (0, 100);
			DateAndTime = (DateTime.UnixEpoch, DateTime.Now);
			Acousticness = (0, 1);
			Danceability = (0, 1);
			Energy = (0, 1);
			Instrumentalness = (0, 1);
			Key = (-1, 11);
			Liveness = (0, 1);
			Loudness = (-60, 0);
			Mode = 2; // 2 means it doesn't matter
			Speechiness = (0, 1);
			Tempo = (0, 200);
			TimeSignature = (3, 7);
			Valence = (0, 1);
		}

		public bool Track { get; set; }
		public bool Artists { get; set; }
		public bool Album { get; set; }
		public (int min, int max) Length { get; set; }
		public (int min, int max) Popularity { get; set; }
		public (DateTime start, DateTime end) DateAndTime { get; set; }
		public (double min, double max) Acousticness { get; set; }
        public (double min, double max) Danceability { get; set; }
        public (double min, double max) Energy { get; set; }
        public (double min, double max) Instrumentalness { get; set; }
		public (int min, int max) Key { get; set; }
        public (double min, double max) Liveness { get; set; }
        public (double min, double max) Loudness { get; set; }
		public int Mode { get; set; }
        public (double min, double max) Speechiness { get; set; }
        public (double min, double max) Tempo { get; set; }
        public (int min, int max) TimeSignature { get; set; }
        public (double min, double max) Valence { get; set; }
    }
}

