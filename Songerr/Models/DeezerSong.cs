using Songerr.Models.Songerr.Models;

namespace Songerr.Models
{
    public class DeezerSong : IDeezerSong
    {
        public string Title { get; set; } // Title
        private Artist _artist;
        public Artist Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
            }
        } // Artist

        private Album _album;
        public Album Album
        {
            get { return _album; }
            set
            {
                _album = value;
            }
        } // Album

        public int TrackPosition { get; set; } // Track Number
        public int ReleaseDate { get; set; } // Year
        public string GenreId { get; set; } // Genre (not exactly, but close)
        public string Link { get; set; } // Comment (not exactly, but close)

        // Explicitly implement the Artist and Album properties from the ISong interface
        IArtist ISong.Artist
        {
            get { return Artist; }
            set { Artist = (Artist)value; }
        }

        IAlbum ISong.Album
        {
            get { return Album; }
            set { Album = (Album)value; }
        }
    }
}
