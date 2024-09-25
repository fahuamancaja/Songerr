using System.Text.RegularExpressions;
using AutoMapper;
using Unidecode.NET;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using SongModel = Songerr.Infrastructure.PayloadModels.SongModel;

namespace Songerr.Application.Middleware;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PlaylistVideo, SongModel>()
            .ForMember(dest => dest.PlaylistId, opt => opt.MapFrom(src => src.PlaylistId.Value))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.Title,
                opt => opt.MapFrom(src => RemoveBracesAndTrailingSpacesAndSpecialChars(src.Title)))
            .ForMember(dest => dest.Author,
                opt => opt.MapFrom(src => RemoveBracesAndTrailingSpacesAndSpecialChars(src.Author.ChannelTitle)));

        CreateMap<Video, SongModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value)) // Convert VideoId to string
            .ForMember(dest => dest.Title,
                opt => opt.MapFrom(src => RemoveBracesAndTrailingSpacesAndSpecialChars(src.Title)))
            .ForMember(dest => dest.Author,
                opt => opt.MapFrom(src => RemoveBracesAndTrailingSpacesAndSpecialChars(src.Author.ToString())));
    }

    public static string RemoveBracesAndTrailingSpacesAndSpecialChars(string input)
    {
        input = input.Unidecode();
        // First, remove parentheses, square brackets, and curly braces along with their content
        var result = Regex.Replace(Regex.Replace(Regex.Replace(input, @"\([^)]*\)", ""), @"\[[^\]]*\]", ""),
            @"\{[^}]*\}", "");

        // Then, remove any special characters except spaces, underscores, and hyphens
        result = Regex.Replace(result, @"[^0-9A-Za-z _-]", "");

        // Finally, remove trailing spaces
        result = Regex.Replace(result, @"\s+$", "");

        return result;
    }
}