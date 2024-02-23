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
        public async Task DownloadVideoAsMp3(PlaylistModel playListModel)
        {
            var youtubeDl = new YoutubeDL();
            var result = await youtubeDl.RunAudioDownload($"https://www.youtube.com/watch?v={playListModel.Id}");
            playListModel.Filepath = result.Data;
        }
        public async Task PlayListDownloadVideoAsMp3(PlaylistModel playlistVideo)
        {
            var youtubeDl = new YoutubeDL();
            var result = await youtubeDl.RunAudioDownload($"https://www.youtube.com/watch?v={playlistVideo.Id}");
            playlistVideo.Filepath = result.Data;
        }
        public async Task<List<PlaylistModel>> GetIPlaylistVideoFromPlaylist(string playlistId)
        {
            var playlistVideosList = new List<PlaylistModel>();

            var youtube = new YoutubeClient();
            var playlistUrl = $"https://music.youtube.com/playlist?list={playlistId}";

            await foreach (var video in youtube.Playlists.GetVideosAsync(playlistUrl))
            {
                //var videoId = video.Id;
                //playlistVideosList.Add(video);
                playlistVideosList.Add(_mapper.Map<PlaylistModel>(video));
            }

            return playlistVideosList;
        }
    }
}
