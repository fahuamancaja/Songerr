﻿using System.Diagnostics.CodeAnalysis;

namespace Songerr.Domain.Models;

[ExcludeFromCodeCoverage]
public class Album
{
    public string? album_type { get; set; }
    public List<Artist>? artists { get; set; }
    public List<string>? available_markets { get; set; }
    public ExternalUrls? external_urls { get; set; }
    public string? href { get; set; }
    public string? id { get; set; }
    public List<Image>? images { get; set; }
    public string? name { get; set; }
    public string? release_date { get; set; }
    public string? release_date_precision { get; set; }
    public int total_tracks { get; set; }
    public string? type { get; set; }
    public string? uri { get; set; }
}

[ExcludeFromCodeCoverage]
public class Artist
{
    public ExternalUrls? external_urls { get; set; }
    public string? href { get; set; }
    public string? id { get; set; }
    public string? name { get; set; }
    public string? type { get; set; }
    public string? uri { get; set; }
}

[ExcludeFromCodeCoverage]
public class ExternalIds
{
    public string? isrc { get; set; }
}

[ExcludeFromCodeCoverage]
public class ExternalUrls
{
    public string? spotify { get; set; }
}

[ExcludeFromCodeCoverage]
public class Image
{
    public int height { get; set; }
    public string? url { get; set; }
    public int width { get; set; }
}

[ExcludeFromCodeCoverage]
public class Item
{
    public Album? album { get; set; }
    public List<Artist>? artists { get; set; }
    public List<string>? available_markets { get; set; }
    public int disc_number { get; set; }
    public int duration_ms { get; set; }
    public bool @explicit { get; set; }
    public ExternalIds? external_ids { get; set; }
    public ExternalUrls? external_urls { get; set; }
    public string? href { get; set; }
    public string? id { get; set; }
    public bool is_local { get; set; }
    public string? name { get; set; }
    public int popularity { get; set; }
    public string? preview_url { get; set; }
    public int track_number { get; set; }
    public string? type { get; set; }
    public string? uri { get; set; }
}

[ExcludeFromCodeCoverage]
public class SpotifyResults
{
    public Tracks? tracks { get; set; }
}

[ExcludeFromCodeCoverage]
public class Tracks
{
    public string? href { get; set; }
    public List<Item>? items { get; set; }
    public int limit { get; set; }
    public string? next { get; set; }
    public int offset { get; set; }
    public object? previous { get; set; }
    public int total { get; set; }
}