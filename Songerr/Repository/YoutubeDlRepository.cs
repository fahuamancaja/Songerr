using System;
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
        private readonly YoutubeDL _youtubeDl;
        private readonly YoutubeClient _youtubeClient;

        public YoutubeDlRepository(IMapper mapper, YoutubeDL youtubeDl, YoutubeClient youtubeClient)
        {
            _mapper = mapper;
            _youtubeDl = youtubeDl;
            _youtubeClient = youtubeClient;
        }

        public async Task DownloadVideoAsMp3(SongModel playListModel)
        {
            try
            {
                var result = await _youtubeDl.RunAudioDownload($"https://music.youtube.com/watch?v={playListModel.Id}");
                playListModel.Filepath = result.Data;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                Console.WriteLine($"Error downloading video: {ex.Message}");
            }
        }

        public async Task GetSongMetadataFromSongId(SongModel songModel)
        {
            try
            {
                var playlistUrl = $"https://music.youtube.com/watch?v={songModel.Id}";
                var video = await _youtubeClient.Videos.GetAsync(playlistUrl);
                _mapper.Map(video, songModel);

                // Assuming songModel is an instance of a class with a property Author
                string[] parts = songModel.Author.Split('-');
                songModel.Author = parts[0].Trim();

            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                Console.WriteLine($"Error fetching song metadata: {ex.Message}");
            }
        }

        public async Task<List<SongModel>> GetSongsMetadataFromPlaylistId(string playlistId)
        {
            var playlistVideosList = new List<SongModel>();
            try
            {
                var playlistUrl = $"https://music.youtube.com/playlist?list={playlistId}";
                await foreach (var video in _youtubeClient.Playlists.GetVideosAsync(playlistUrl))
                {
                    playlistVideosList.Add(_mapper.Map<SongModel>(video));
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                Console.WriteLine($"Error fetching playlist metadata: {ex.Message}");
            }
            return playlistVideosList;
        }
    }
}
