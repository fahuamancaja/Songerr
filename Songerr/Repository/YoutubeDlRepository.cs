using AutoMapper;
using Songerr.Models;
using YoutubeDLSharp;
using YoutubeExplode;
using YoutubeExplode.Playlists;

namespace Songerr.Repository
{
    public class YoutubeDlRepository : IYoutubeDlRepository
    {
        private readonly IMapper _mapper;

        public YoutubeDlRepository(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task DownloadVideoAsMp3(SongModel playListModel)
        {
            var youtubeDl = new YoutubeDL();
            var result = await youtubeDl.RunAudioDownload($"https://www.youtube.com/watch?v={playListModel.Id}");
            playListModel.Filepath = result.Data;
        }
        public async Task PlayListDownloadVideoAsMp3(SongModel playlistVideo)
        {
            var youtubeDl = new YoutubeDL();
            var result = await youtubeDl.RunAudioDownload($"https://www.youtube.com/watch?v={playlistVideo.Id}");
            playlistVideo.Filepath = result.Data;
        }
        public async Task GetSongMetadataFromSongId(SongModel songModel)
        {
            var playlistVideosList = new List<SongModel>();

            var youtube = new YoutubeClient();
            var playlistUrl = $"https://music.youtube.com/watch?v={songModel.Id}";

            var video = await youtube.Videos.GetAsync(playlistUrl);
            _mapper.Map(video, songModel);
        }
        public async Task<List<SongModel>> GetSongsMetadataFromPlaylistId(string playlistId)
        {
            var playlistVideosList = new List<SongModel>();

            var youtube = new YoutubeClient();
            var playlistUrl = $"https://music.youtube.com/playlist?list={playlistId}";

            await foreach (var video in youtube.Playlists.GetVideosAsync(playlistUrl))
            {
                playlistVideosList.Add(_mapper.Map<SongModel>(video));
            }

            return playlistVideosList;
        }
    }
}
