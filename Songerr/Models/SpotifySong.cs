using Songerr.Models.Songerr.Models;

namespace Songerr.Models
{
    public class SpotifySong : ISpotifySong
    {
        public string Name { get; set; } // Title
        public List<Artist> Artists { get; set; } // Artist
        public Album SimpleAlbum { get; set; } // Album
        public int TrackNumber { get; set; } // Track Number
        public int ReleaseDatePrecision { get; set; } // Year
        public string AlbumType { get; set; } // Genre (not exactly, but close)
        public string[] ExternalUrls { get; set; } // Comment (not exactly, but close)

        // Explicitly implement the Artist and Album properties from the ISong interface
        IArtist ISong.Artist
        {
            get
            {
                // Assuming that the first artist in the list represents the main artist
                return Artists != null && Artists.Count > 0 ? Artists[0] : null;
            }
            set
            {
                // Assuming that the first artist in the list represents the main artist
                if (Artists == null)
                {
                    Artists = new List<Artist>();
                }
                Artists[0] = (Artist)value;
            }
        }

        IAlbum ISong.Album
        {
            get
            {
                return SimpleAlbum;
            }
            set
            {
                SimpleAlbum = (Album)value;
            }
        }
    }
}
