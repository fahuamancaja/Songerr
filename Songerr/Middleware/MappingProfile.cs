
using AutoMapper;
using Songerr.Models;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace Songerr.Middleware
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PlaylistVideo, SongModel>()
                .ForMember(dest => dest.PlaylistId, opt => opt.MapFrom(src => src.PlaylistId.Value))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.Title));

            CreateMap<Video, SongModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value)) // Convert VideoId to string
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.ToString())); // Assuming Author has a suitable ToString() method
        }
    }
}
