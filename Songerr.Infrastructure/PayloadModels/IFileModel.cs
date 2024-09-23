namespace Songerr.Infrastructure.PayloadModels;

public interface IFileModel
{
    string? FilePath { get; set; }
    string Author { get; set; }
    string Title { get; set; }
    string Album { get; set; }
}