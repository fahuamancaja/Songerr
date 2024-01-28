using Songerr.Models.Songerr.Models;

namespace Songerr.Models
{
    public interface ISong
    {
        IArtist Artist { get; set; }
        IAlbum Album { get; set; }
    }

    public interface IAlbum
    {
    }

    public interface IArtist
    {
    }
    public interface ISpotifySong : ISong
    {
        // Additional members specific to SpotifySong
    }

    public interface IDeezerSong : ISong
    {
        // Additional members specific to DeezerSong
    }
}
