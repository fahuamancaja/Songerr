﻿using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Serilog;
using Songerr.Domain.Interfaces;
using Songerr.Domain.Models;
using Songerr.Domain.Models.OptionSettings;
using File = System.IO.File;

namespace Songerr.Domain.Services;

public class ParserService(IOptions<LocalSettings> settings) : IParserService
{
    private readonly LocalSettings _settings = settings.Value;

    public async Task MoveFileToCorrectLocation(SongModel songModel)
    {
        var fileExtension = Path.GetExtension(songModel.FilePath);
        var titleName = songModel.Title!.Contains('-')
            ? songModel.Title
            : $"{songModel.Author} - {songModel.Title}";
        var rootDirectoryPath = _settings.DownloadPath;
        var destinationDirectory = rootDirectoryPath;

        if (songModel.Author is not null && songModel.Album is not null)
            destinationDirectory = Path.Combine(rootDirectoryPath!, songModel.Author, songModel.Album);
        else if (songModel.Author is not null)
            destinationDirectory = Path.Combine(rootDirectoryPath!, songModel.Author);
        else if (songModel.Album is not null) destinationDirectory = Path.Combine(rootDirectoryPath!, songModel.Album);

        var newFileName = Path.ChangeExtension(titleName, fileExtension);
        var newFilePath = Path.Combine(destinationDirectory!, newFileName);
        Directory.CreateDirectory(Path.GetDirectoryName(newFilePath)!);

        // Copy the file to the new location, overwriting if it already exists
        await Task.Run(() => File.Copy(songModel.FilePath!, newFilePath, true));

        // Delete the original file
        await Task.Run(() => File.Delete(songModel.FilePath!));

        songModel.FilePath = newFilePath;
    }

    public void ParseVideoUrl(SongModel? songmodel)
    {
        // Updated regex to match m.youtube.com and optional parameters
        var regex = new Regex(@"(?:youtu\.be\/|youtube\.com\/watch\?v=|m\.youtube\.com\/watch\?v=)([^&?]+)(?:.*)?",
            RegexOptions.IgnoreCase);

        // Match the URL against the regex
        var match = regex.Match(songmodel?.Id!);

        // If a match is found, return the video ID; otherwise, return null
        if (match.Success) songmodel!.Id = match.Groups[1].Value;
    }

    public Task AddMetaDataToFile(SongModel? songModel)
    {
        var file = TagLib.File.Create(songModel!.FilePath);

        if (!string.IsNullOrWhiteSpace(songModel.Id))
        {
            file.Tag.Title = songModel.Title;
            file.Tag.Performers = [songModel.Author];
            file.Tag.Album = songModel.Album;

            file.Save();

            Log.Information($"Metadata Added to Song Id:{songModel.Id} Song Title:{songModel.Title}");
        }

        return Task.CompletedTask;
    }
}