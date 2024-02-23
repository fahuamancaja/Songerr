
using AutoMapper;
using Songerr.Models;
using YoutubeExplode.Playlists;

namespace Songerr.Middleware
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PlaylistVideo, PlaylistModel>()
                .ForMember(dest => dest.PlaylistId, opt => opt.MapFrom(src => src.PlaylistId.Value))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.ChannelTitle)); 
        }
    }

}
