using AutoFixture;
using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Tests.Customizations;

public class FixedFilePathCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<SongModel>(composer => composer
            .With(songModel => songModel.FilePath, Path.Combine(Path.GetTempPath(), "Recording.m4a")));
    }
}