namespace Songerr.Infrastructure.PayloadModels;

public class SongModel : IFileModel
{
    public string? PlaylistId { get; set; }
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Album { get; set; }
    public string? FilePath { get; set; }
}